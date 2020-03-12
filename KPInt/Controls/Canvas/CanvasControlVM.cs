using KPInt_Shared;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KPInt.Controls.Canvas
{
    class CanvasControlVM : UIElement
    {
        private class VisualWrap : UIElement
        {
            public Visual Visual { get; set; }
            protected override int VisualChildrenCount => 1;
            protected override Visual GetVisualChild(int index) => Visual;
        }

        private const int DPI = 96;

        private readonly CanvasControlView _control;

        private readonly DrawingVisual _canvas;
        private readonly DrawingContext _renderer;
        private readonly Pen _pen;
        private readonly SolidColorBrush _brush;

        public FrameworkElement View => _control;
        public BitmapImage Picture
        {
            get
            {
                var bm = new BitmapImage();
                var rt = new RenderTargetBitmap((int)_control.ActualWidth, (int)_control.ActualHeight, DPI, DPI, PixelFormats.Pbgra32);
                rt.Render(_canvas);
                return bm;
            }
        }

        public CanvasControlVM()
        {
            _canvas = new DrawingVisual();
            _renderer = _canvas.RenderOpen();
            _pen = new Pen { Brush = _brush = new SolidColorBrush(), StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };

            _control = new CanvasControlView();
            _control.BaseCanvas.Children.Add(new VisualWrap { Visual = _canvas });
        }

        public void DrawLine(NewColorLine line)
        {
            _pen.Thickness = line.Thickness;
            _brush.Color = line.Color;
            _renderer.DrawLine(_pen, line.Start, line.End);
        }

        public void Clear()
        {
            _renderer.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(0, 0, _control.ActualWidth, _control.ActualHeight));
        }
    }
}
