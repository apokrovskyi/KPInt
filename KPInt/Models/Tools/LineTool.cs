using KPInt_Shared;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KPInt.Models.Tools
{
    class LineTool : IDrawingTool
    {
        private readonly Panel _parent;
        private readonly Line _linePreview;

        private readonly Func<Color> _getColor;
        private readonly Func<double> _getThickness;
        private readonly Action<ColorLine> _output;

        private readonly ClickTool _clickTool;

        public LineTool(Panel input, Func<bool, Color> color, Func<double> thickness, Action<ColorLine> output)
        {
            _parent = input;
            _getColor = () => color(_clickTool.LeftButton);
            _getThickness = thickness;
            _output = output;

            _linePreview = new Line
            {
                IsHitTestVisible = false,
                Stroke = new SolidColorBrush(),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            Panel.SetZIndex(_linePreview, 1);

            _parent.Children.Add(_linePreview);

            _clickTool = new ClickTool(input);
        }

        public void Attach()
        {
            _clickTool.Attach();
            _linePreview.StrokeThickness = _getThickness();

            _clickTool.MouseDown += Input_MouseDown;
        }

        public void Detach()
        {
            StopDrawing();
            _clickTool.Detach();
            _clickTool.MouseDown -= Input_MouseDown;
        }

        private void StartDrawing(Point start)
        {
            _linePreview.X1 = _linePreview.X2 = start.X;
            _linePreview.Y1 = _linePreview.Y2 = start.Y;
            (_linePreview.Stroke as SolidColorBrush).Color = _getColor();

            _linePreview.Visibility = Visibility.Visible;
            _clickTool.MouseUp += Input_MouseUp;
            _parent.PreviewMouseMove += Input_PreviewMouseMove;
            _clickTool.MouseEnter += Input_MouseEnter;
        }

        private void StopDrawing()
        {
            _linePreview.Visibility = Visibility.Collapsed;
            _clickTool.MouseUp -= Input_MouseUp;
            _parent.PreviewMouseMove -= Input_PreviewMouseMove;
            _clickTool.MouseEnter -= Input_MouseEnter;
        }

        private void Input_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartDrawing(e.GetPosition(_parent));
        }

        private void Input_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _output(new ColorLine(
                new Point(_linePreview.X1, _linePreview.Y1),
                new Point(_linePreview.X2, _linePreview.Y2),
                _getColor(), (byte)_getThickness()));
            StopDrawing();
        }

        private void Input_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!_clickTool.Pressed) StopDrawing();
        }

        private void Input_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var end = e.GetPosition(_parent);
            _linePreview.X2 = end.X;
            _linePreview.Y2 = end.Y;
        }
    }
}
