using LineColor = System.Windows.Media.Color;
using LinePoint = System.Windows.Point;

namespace KPInt_Shared
{
    public class ColorLine
    {
        public static ColorLine Empty => new ColorLine(new LinePoint(), new LinePoint(), new LineColor(), 0);

        public bool Valid => Start.X != End.X || Start.Y != End.Y;
        public LinePoint Start { get; }
        public LinePoint End { get; }
        public LineColor Color { get; }
        public byte Thickness { get; }

        public ColorLine(LinePoint start, LinePoint end, LineColor color, byte thickness)
        {
            Start = start;
            End = end;
            Color = color;
            Thickness = thickness;
        }
    }
}
