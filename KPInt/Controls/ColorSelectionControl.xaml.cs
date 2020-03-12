using System.Windows.Controls;
using System.Windows.Input;


using LineColor = System.Windows.Media.Color;

namespace KPInt.Controls
{
    /// <summary>
    /// Логика взаимодействия для ColorSelectionControl.xaml
    /// </summary>
    public partial class ColorSelectionControl : UserControl
    {
        public byte Thickness => TValue.Value;
        public LineColor SelectedColor => _color;


        public LineColor _color;

        private LineColor _previewColor;

        public ColorSelectionControl()
        {
            InitializeComponent();
            _previewColor = _color = LineColor.FromRgb(RValue.Value, GValue.Value, BValue.Value);
            UpdateColor();
            RValue.ValueChanged += (s, e) => { _previewColor.R = RValue.Value; UpdateColor(); };
            GValue.ValueChanged += (s, e) => { _previewColor.G = GValue.Value; UpdateColor(); };
            BValue.ValueChanged += (s, e) => { _previewColor.B = BValue.Value; UpdateColor(); };
            ColorPreviewRectangle.MouseDown += (s, e) => { if (e.ChangedButton == MouseButton.Left) _color = _previewColor; };
        }

        private void UpdateColor()
        {
            ColorPreviewRectangle.Fill = new System.Windows.Media.SolidColorBrush(_previewColor);
        }
    }
}
