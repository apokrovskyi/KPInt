using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;
using KPInt.Controls.Canvas;
using KPInt_Shared;
using System.Windows.Input;

namespace KPInt.Controls.DrawingTools
{
    class DrawingToolsVM
    {
        private const int WHEEL_SPEED_FRACTION = 20;

        public event PropertyChangedEventHandler LineDrawn;

        public FrameworkElement View => _control;

        private readonly DrawingToolsView _control = new DrawingToolsView();
        private ToolsWindow _window;

        private readonly CanvasControlVM _canvas = new CanvasControlVM();

        public NewColorLine DrawnLine { get; private set; }

        private double _thickness;

        private UIElement InputPanel => _control.DrawingPanel;
        private readonly double _frameLength;
        private DateTime? _drawStartTime;

        public DrawingToolsVM(Window parent, double fps)
        {
            _frameLength = 1000 / fps;
            _thickness = 1;

            _control.Loaded += (s, e) => Control_Loaded(parent);
            InputPanel.PreviewMouseLeftButtonDown += Control_PreviewMouseLeftButtonDown;
            InputPanel.PreviewMouseWheel += Control_PreviewMouseWheel;

            _control.DrawingPanel.Children.Add(_canvas.View);
        }

        private void StartDrawing(Point start)
        {
            InputPanel.PreviewMouseLeftButtonUp += Control_PreviewMouseLeftButtonUp;
            InputPanel.PreviewMouseMove += Control_PreviewMouseMove;
            InputPanel.MouseEnter += Control_MouseEnter;
            InputPanel.MouseLeave += InputPanel_MouseLeave;

            DrawnLine = new NewColorLine(start, start, _window.DrawingColor, 0);
            _drawStartTime = DateTime.Now;
        }

        private void ChangeLine(Point end)
        {
            DrawnLine = new NewColorLine(DrawnLine.End, end, _window.DrawingColor, (byte)_thickness);
            _canvas.DrawLine(DrawnLine);
            LineDrawn?.Invoke(this, new PropertyChangedEventArgs(nameof(DrawnLine)));
        }

        private void StopDrawing()
        {
            _drawStartTime = null;

            InputPanel.PreviewMouseLeftButtonUp -= Control_PreviewMouseLeftButtonUp;
            InputPanel.PreviewMouseMove -= Control_PreviewMouseMove;
            InputPanel.MouseEnter -= Control_MouseEnter;
            InputPanel.MouseLeave -= InputPanel_MouseLeave;
        }

        private void Control_Loaded(Window parent)
        {
            _window = new ToolsWindow { Owner = parent };
            _window.Closing += ToolWindow_Closing;
            _window.Show();
        }

        private void Control_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var value = Math.Max(1, _thickness + e.Delta / WHEEL_SPEED_FRACTION);
            _thickness = Math.Min(value, byte.MaxValue);
        }

        private void Control_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartDrawing(e.GetPosition(InputPanel));
        }

        private void Control_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChangeLine(e.GetPosition(InputPanel));
            StopDrawing();
        }

        private void Control_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StopDrawing();
                StartDrawing(e.GetPosition(InputPanel));
            }
        }

        private void InputPanel_MouseLeave(object sender, MouseEventArgs e) =>
            ChangeLine(e.GetPosition(InputPanel));

        private void Control_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if ((DateTime.Now - _drawStartTime.Value).TotalMilliseconds < _frameLength) return;
            ChangeLine(e.GetPosition(InputPanel));
            _drawStartTime = DateTime.Now;
        }

        public void OpenToolWindow() =>
            _window.Visibility = Visibility.Visible;

        private void ToolWindow_Closing(object sender, CancelEventArgs e)
        {
            _window.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }
    }
}
