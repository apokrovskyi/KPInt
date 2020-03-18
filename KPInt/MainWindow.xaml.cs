using KPInt.Controls;
using KPInt.Controls.Canvas;
using KPInt.Controls.DrawingTools;
using KPInt.Controls.ServerConnection;
using KPInt.Models;
using System.Windows.Input;
using System.Windows;
using System.Drawing.Imaging;
using Microsoft.Win32;

namespace KPInt
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static int _lineId = 0;
        public static int GetId() => ++_lineId;

        public MainWindow()
        {
            InitializeComponent();

            int max_fps = 40;

            var tcpClient = new TcpServerConnection();
            var udpClient = new UdpServerConnection();
            var canvas = new CanvasControlVM();
            var drawingTools = new DrawingToolsVM(this, canvas, max_fps);
            var serverConnection = new ServerConnectionVM(tcpClient);

            KeyDown += (s, e) =>
            {
                if (Keyboard.Modifiers != ModifierKeys.Control) return;

                if (e.Key == Key.X) canvas.Clear();
                if (e.Key == Key.S)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog { FileName = "unnamed.png" };
                    if (saveFileDialog.ShowDialog() == true)
                        canvas.Picture.Save(saveFileDialog.FileName, ImageFormat.Png);
                }
                if(e.Key == Key.T) drawingTools.OpenToolWindow();
            };

            serverConnection.PropertyChanged += (s, e) =>
            {
                if (serverConnection.IsServerConnected) udpClient.Address = serverConnection.Address;
                else udpClient.Stop();
            };
            serverConnection.RoomChanged += () => udpClient.Start(serverConnection.RoomID);
            drawingTools.LineDrawn += () =>
            {
                canvas.DrawLine(drawingTools.DrawnLine);
                udpClient.SendLine(drawingTools.DrawnLine);
            };

            udpClient.LineReceived += () => Dispatcher.Invoke(() => canvas.DrawLine(udpClient.ReceivedLine.Retrieve(x => x)));
            udpClient.Disconnected += () => Dispatcher.Invoke(() => tcpClient.Disconnect());

            var visSwitch = new VisibilitySwitchView();
            visSwitch.DisplayedContent.Content = serverConnection.View;

            WPFContent.Content = drawingTools.View;
            ConnectionControls.Content = visSwitch;
        }
    }
}
