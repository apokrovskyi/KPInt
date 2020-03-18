using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KPInt.Models.Tools
{
    class PickerTool : IDrawingTool
    {
        private FrameworkElement _parent;
        private FrameworkElement _input;

        private readonly Action<Color, bool> _output;

        private readonly ClickTool _clickTool;

        public PickerTool(FrameworkElement trigger, FrameworkElement input, Action<Color, bool> output)
        {
            _parent = trigger;
            _input = input;
            _output = output;

            _clickTool = new ClickTool(trigger);
        }

        public void Attach()
        {
            _clickTool.MouseDown += ClickTool_MouseDown;
            _clickTool.Attach();
        }

        public void Detach()
        {
            Abort();
            _clickTool.MouseDown -= ClickTool_MouseDown;
            _clickTool.Detach();
        }

        private void ClickTool_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _input.IsHitTestVisible = true;
            _clickTool.MouseUp += Input_MouseUp;
            _input.MouseUp += Input_MouseUp;
            _input.MouseLeave += Input_MouseLeave;
        }

        private void Input_MouseLeave(object sender, MouseEventArgs e)
        {
            Abort();
        }

        private void Input_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Abort();
            if (e.ChangedButton != MouseButton.Left && e.ChangedButton != MouseButton.Right) return;
            if ((e.ChangedButton == MouseButton.Left) != _clickTool.LeftButton) return;

            var color = e.OriginalSource is Line line ? (line.Stroke as SolidColorBrush).Color : Colors.White;
            _output(color, _clickTool.LeftButton);
        }

        private void Abort()
        {
            _input.IsHitTestVisible = false;
            _input.MouseUp -= Input_MouseUp;
            _clickTool.MouseUp -= Input_MouseUp;
            _input.MouseLeave -= Input_MouseLeave;
        }
    }
}
