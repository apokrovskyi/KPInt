using KPInt.Controls;
using KPInt_Shared;
using System.Threading.Tasks;
using System.Windows;
using KPInt.Controls.DrawingTools;

namespace KPInt
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ServerConnectionVM _connection;

        public MainWindow()
        {
            InitializeComponent();

            int max_fps = 40;

            var connectionControl = new ServerConnectionControl();
            var tools = new DrawingToolsVM(this, max_fps);
            
            connectionControl.ConnectButton.Click += (s, e) => tools.OpenToolWindow();

            ConnectionControls.Content = connectionControl;
            WPFContent.Content = tools.View;

            //var colorSelector = new ColorSelectionControl();
            //var visibilitySwitch = new VisibilitySwitchControl(true, connectionControl, colorSelector);
            //var canvasControl = new CanvasControl(approx_fps);

            //var runFlag = new LockedValue<bool>(true);
            //var client = new RoomClient(runFlag, approx_fps);
            //Task.Run(client.Receive);

            //_connection = new ServerConnectionVM(new ConnectionManager(client), connectionControl, canvasControl);

            //Closed += (s, e) => runFlag.SetValue(false);
            //canvasControl.LineChanged += (s, e) => client.Line =
            //    new ColorLine(canvasControl.StartPoint, canvasControl.EndPoint, colorSelector.SelectedColor, colorSelector.Thickness);
            //_connection.RoomChanged += (s, e) => canvasControl.Clear();
            //client.LineReceived += (s, e) => Dispatcher.Invoke(() => canvasControl.DrawLine(client.ReceivedLine.Retrieve(x => x)));

            //colorSelector.SaveButton.Click += (s, e) => canvasControl.Save();


            //WPFContent.Content = canvasControl;
            //VisibilitySwitch.Content = visibilitySwitch;
            //ColorSelector.Content = colorSelector;
        }
    }
}
