using KPInt_Shared;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KPInt.Models.Tools
{
    class PreviewTool : IDrawingTool
    {
        private readonly Panel _parent;
        private readonly Ellipse _control;

        private readonly DispatcherTimer _timer;

        private Point _position;
        private double _radius;

        public PreviewTool(Panel parent)
        {
            _parent = parent;
            _control = new Ellipse
            {
                IsHitTestVisible = false,
                Fill = new SolidColorBrush(),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            Panel.SetZIndex(_control, 1);
            _parent.Children.Add(_control);
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _timer.Tick += (s, e) => Hide();
        }

        public void Attach()
        {
            _parent.PreviewMouseMove += Parent_PreviewMouseMove;
        }

        public void Detach()
        {
            Hide();
            _parent.PreviewMouseMove -= Parent_PreviewMouseMove;
        }

        public void Show(Color color, double thickness)
        {
            (_control.Fill as SolidColorBrush).Color = color;
            _control.Height = _control.Width = thickness;
            _radius = thickness / 2;
            Update();

            _control.Visibility = Visibility.Visible;

            _timer.Stop();
            _timer.Start();
        }

        private void Parent_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _position = e.GetPosition(sender as IInputElement);
            if (_timer.IsEnabled)
                Update();
        }

        private void Hide()
        {
            _control.Visibility = Visibility.Collapsed;
            _timer.Stop();
        }

        private void Update() =>
            _control.Margin = new Thickness(_position.X - _radius, _position.Y - _radius, 0, 0);
    }

    class PencilTool : IDrawingTool
    {
        private const int WHEEL_SPEED_FRACTION = 20;

        public double Thickness { get; private set; }

        private readonly Func<Color> _getColor;
        private readonly Panel _input;
        private readonly Action<ColorLine> _output;

        private ColorLine _drawnLine;
        private readonly double _frameLength;
        private DateTime? _drawStartTime;

        private readonly ClickTool _clickTool;
        private readonly PreviewTool _drawPreview;

        public PencilTool(Panel input, Func<bool, Color> color, Action<ColorLine> output, double fps)
        {
            _frameLength = 1000 / fps;
            _input = input;
            _getColor = () => color(_clickTool.LeftButton);
            _output = output;
            Thickness = 1;

            _drawPreview = new PreviewTool(input);
            _clickTool = new ClickTool(input);
        }

        public void Attach()
        {
            _drawPreview.Attach();
            _clickTool.Attach();
            _clickTool.MouseDown += Input_MouseDown;
            _input.PreviewMouseWheel += Input_PreviewMouseWheel;
        }

        public void Detach()
        {
            StopDrawing();
            _drawPreview.Detach();
            _clickTool.Detach();
            _clickTool.MouseDown -= Input_MouseDown;
            _input.PreviewMouseWheel -= Input_PreviewMouseWheel;
        }

        private void StartDrawing(Point start)
        {
            _clickTool.MouseUp += Input_MouseUp;
            _input.PreviewMouseMove += Input_PreviewMouseMove;
            _clickTool.MouseEnter += Input_MouseEnter;
            _input.MouseLeave += Input_MouseLeave;

            _drawnLine = new ColorLine(start, start, _getColor(), 0);
            _drawStartTime = DateTime.Now;
        }

        private void ChangeLine(Point end)
        {
            _drawnLine = new ColorLine(_drawnLine.End, end, _getColor(), (byte)Thickness);
            _output(_drawnLine);
        }

        private void StopDrawing()
        {
            _drawStartTime = null;

            _clickTool.MouseUp -= Input_MouseUp;
            _input.PreviewMouseMove -= Input_PreviewMouseMove;
            _clickTool.MouseEnter -= Input_MouseEnter;
            _input.MouseLeave -= Input_MouseLeave;
        }

        private void Input_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var value = Math.Max(1, Thickness + e.Delta / WHEEL_SPEED_FRACTION);
            Thickness = Math.Min(value, byte.MaxValue);
            _drawPreview.Show(_getColor(), Thickness);
        }

        private void Input_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartDrawing(e.GetPosition(_input));
        }

        private void Input_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangeLine(e.GetPosition(_input));
            StopDrawing();
        }

        private void Input_MouseEnter(object sender, MouseEventArgs e)
        {
            StopDrawing();
            if (_clickTool.Pressed)
                StartDrawing(e.GetPosition(_input));
        }

        private void Input_MouseLeave(object sender, MouseEventArgs e) =>
            ChangeLine(e.GetPosition(_input));

        private void Input_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if ((DateTime.Now - _drawStartTime.Value).TotalMilliseconds < _frameLength) return;
            ChangeLine(e.GetPosition(_input));
            _drawStartTime = DateTime.Now;
        }
    }
}
