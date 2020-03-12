using KPInt.Controls;
using KPInt_Shared;
using System.Threading.Tasks;
using System.Windows;

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

            int approx_fps = 25;

            var connectionControl = new ServerConnectionControl();
            var colorSelector = new ColorSelectionControl();
            var visibilitySwitch = new VisibilitySwitchControl(true, connectionControl, colorSelector);
            var canvasControl = new CanvasControl(approx_fps);

            var runFlag = new LockedValue<bool>(true);
            var client = new RoomClient(runFlag, approx_fps);
            Task.Run(client.Receive);

            _connection = new ServerConnectionVM(new ConnectionManager(client), connectionControl, canvasControl);

            Closed += (s, e) => runFlag.SetValue(false);
            canvasControl.LineChanged += (s, e) => client.Line =
                new ColorLine(canvasControl.StartPoint, canvasControl.EndPoint, colorSelector.SelectedColor, colorSelector.Thickness);
            _connection.RoomChanged += (s, e) => canvasControl.Clear();
            client.LineReceived += (s, e) => Dispatcher.Invoke(() => canvasControl.DrawLine(client.ReceivedLine.Retrieve(x => x)));

            colorSelector.SaveButton.Click += (s, e) => canvasControl.Save();


            WPFContent.Content = canvasControl;
            ConnectionControls.Content = connectionControl;
            VisibilitySwitch.Content = visibilitySwitch;
            ColorSelector.Content = colorSelector;
        }
    }
}
