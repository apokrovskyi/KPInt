using LinePoint = System.Drawing.Point;

namespace KPInt_Shared
{
    public class ColorLine
    {
        public bool Valid => Start.X != End.X || Start.Y != End.Y;
        public LinePoint Start { get; }
        public LinePoint End { get; }
        public System.Windows.Media.Color LineColor { get; }
        public byte Thickness { get; }

        public ColorLine(LinePoint start, LinePoint end, System.Windows.Media.Color color, byte thickness)
        {
            Start = start;
            End = end;
            LineColor = color;
            Thickness = thickness;
        }
    }
}
