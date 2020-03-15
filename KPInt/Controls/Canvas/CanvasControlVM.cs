using KPInt_Shared;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using RenderColor = System.Drawing.Color;
using RenderPen = System.Drawing.Pen;

namespace KPInt.Controls.Canvas
{
    class CanvasControlVM : UIElement
    {
        public FrameworkElement View => _control;

        private readonly CanvasControlView _control = new CanvasControlView();

        public Bitmap Picture
        {
            get
            {
                var bm = new Bitmap((int)_control.ActualWidth, (int)_control.ActualHeight);

                using (var g = Graphics.FromImage(bm))
                {
                    using (var p = new RenderPen(RenderColor.Black)
                    {
                        StartCap = LineCap.Round,
                        EndCap = LineCap.Round
                    })
                    {
                        foreach (var ch in _control.BaseCanvas.Children.OfType<Line>())
                        {
                            var color = (ch.Stroke as SolidColorBrush).Color;
                            p.Color = RenderColor.FromArgb(color.R, color.G, color.B);
                            p.Width = (float)ch.StrokeThickness;

                            g.DrawLine(p, (int)ch.X1, (int)ch.Y1, (int)ch.X2, (int)ch.Y2);
                        }
                    }
                }

                return bm;
            }
        }

        public void DrawLine(NewColorLine line)
        {
            if (line.Thickness <= 0) return;

            _control.BaseCanvas.Children.Add(new Line
            {
                X1 = line.Start.X,
                Y1 = line.Start.Y,
                X2 = line.End.X,
                Y2 = line.End.Y,
                Stroke = new SolidColorBrush(line.Color),
                StrokeThickness = line.Thickness,
                Visibility = Visibility.Visible,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            });
        }

        public void Clear()
        {
            _control.BaseCanvas.Children.Clear();
        }
    }
}
