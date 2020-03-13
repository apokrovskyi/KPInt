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
            private readonly Visual _content;
            protected override int VisualChildrenCount => 1;
            protected override Visual GetVisualChild(int index) => _content;

            public VisualWrap(Visual visual) => _content = visual;
        }

        private const int DPI = 96;

        public FrameworkElement View => _control;

        private readonly CanvasControlView _control;
        private readonly ContainerVisual _container;

        public BitmapImage Picture
        {
            get
            {
                var bm = new BitmapImage();
                var rt = new RenderTargetBitmap((int)_control.ActualWidth, (int)_control.ActualHeight, DPI, DPI, PixelFormats.Pbgra32);
                rt.Render(_container);
                return bm;
            }
        }

        public CanvasControlVM()
        {
            _container = new ContainerVisual();
            _control = new CanvasControlView();
            _control.BaseCanvas.Children.Add(new VisualWrap(_container));
        }

        public void DrawLine(NewColorLine line)
        {
            var target = new DrawingVisual();
            using (var renderer = target.RenderOpen())
            {
                var pen = new Pen(new SolidColorBrush(line.Color), line.Thickness)
                {
                    StartLineCap = PenLineCap.Round,
                    EndLineCap = PenLineCap.Round
                };
                renderer.DrawLine(pen, line.Start, line.End);
            }
            _container.Children.Add(target);
        }

        public void Clear()
        {
            _container.Children.Clear();
        }
    }
}
