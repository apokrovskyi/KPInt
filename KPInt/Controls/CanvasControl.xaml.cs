using KPInt_Shared;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Linq;
using LinePoint = System.Drawing.Point;
using Microsoft.Win32;

namespace KPInt.Controls
{
    /// <summary>
    /// Логика взаимодействия для CanvasControl.xaml
    /// </summary>
    public partial class CanvasControl : UserControl
    {
        private readonly int _length;
        private DateTime? _startTime;

        public CanvasControl(int linesPerSecond)
        {
            InitializeComponent();
            _length = 1000 / linesPerSecond;
            BaseCanvas.PreviewMouseMove += BaseCanvas_MouseMove;
        }

        public void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { FileName = "KPInt_image.png" };
            if (saveFileDialog.ShowDialog() == true)
            {
                var bm = new System.Drawing.Bitmap((int)ActualWidth, (int)ActualHeight);

                using (var g = System.Drawing.Graphics.FromImage(bm))
                {
                    using (var p = new System.Drawing.Pen(System.Drawing.Color.Black)
                    {
                        StartCap = System.Drawing.Drawing2D.LineCap.Round,
                        EndCap = System.Drawing.Drawing2D.LineCap.Round
                    })
                    {
                        foreach (var ch in BaseCanvas.Children.OfType<Line>())
                        {
                            p.Color = ConvertColor((ch.Stroke as SolidColorBrush).Color);
                            p.Width = (float)ch.StrokeThickness;

                            g.DrawLine(p, new LinePoint((int)ch.X1, (int)ch.Y1), new LinePoint((int)ch.X2, (int)ch.Y2));
                        }
                    }
                }

                bm.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private System.Drawing.Color ConvertColor(Color source)
        {
            return System.Drawing.Color.FromArgb(source.R, source.G, source.B);
        }

        public event PropertyChangedEventHandler LineChanged;

        public LinePoint StartPoint => start;
        public LinePoint EndPoint => end;

        private LinePoint start;
        private LinePoint end;

        private void BaseCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this);
            start.X = (int)position.X;
            start.Y = (int)position.Y;
            end.X = (int)position.X;
            end.Y = (int)position.Y;
            _startTime = DateTime.Now;
        }

        private void BaseCanvas_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _startTime = null;
        }

        private void BaseCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_startTime == null) return;
            if ((DateTime.Now - _startTime.Value).TotalMilliseconds > _length)
            {
                _startTime = DateTime.Now;
                start.X = end.X;
                start.Y = end.Y;
                var position = e.GetPosition(this);
                end.X = (int)position.X;
                end.Y = (int)position.Y;
                LineChanged?.Invoke(this, new PropertyChangedEventArgs("Line"));
            }
            else
            {
                Thread.Sleep(_length);
            }
        }

        public void Clear()
        {
            BaseCanvas.Children.Clear();
        }

        public void DrawLine(ColorLine line)
        {
            if (line.Thickness <= 0) return;

            BaseCanvas.Children.Add(new Line
            {
                X1 = line.Start.X,
                Y1 = line.Start.Y,
                X2 = line.End.X,
                Y2 = line.End.Y,
                Stroke = new SolidColorBrush(line.LineColor),
                StrokeThickness = line.Thickness,
                Visibility = System.Windows.Visibility.Visible,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            });
        }
    }
}
