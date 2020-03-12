using LinePoint = System.Windows.Point;
using LineColor = System.Windows.Media.Color;

namespace KPInt_Shared
{
    public class NewColorLine
    {
        public bool Valid => Start.X != End.X || Start.Y != End.Y;
        public LinePoint Start { get; }
        public LinePoint End { get; }
        public LineColor Color { get; }
        public byte Thickness { get; }

        public NewColorLine(LinePoint start, LinePoint end, LineColor color, byte thickness)
        {
            Start = start;
            End = end;
            Color = color;
            Thickness = thickness;
        }
    }
}
