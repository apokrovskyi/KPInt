using KPInt.Controls;
using KPInt_Shared;
using System.Threading.Tasks;
using System.Windows;
using KPInt.Controls.DrawingTools;
using KPInt.Controls.ServerConnection;
using KPInt.Controls.Canvas;
using KPInt.Controls;
using KPInt.Models;

namespace KPInt
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            int max_fps = 40;

            var udpClient = new UdpServerConnection();
            var canvas = new CanvasControlVM();
            var drawingTools = new DrawingToolsVM(this, canvas, max_fps);
            var serverConnection = new ServerConnectionVM();

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
            udpClient.LineReceived += (s, e) => canvas.DrawLine(udpClient.ReceivedLine.Retrieve(x => x));

            var visSwitch = new VisibilitySwitchView();
            visSwitch.DisplayedContent.Content = serverConnection.View;

            WPFContent.Content = drawingTools.View;
            ConnectionControls.Content = visSwitch;
        }
    }
}
