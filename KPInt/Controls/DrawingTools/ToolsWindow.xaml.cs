using KPInt.Controls.ValueSelector;
using KPInt.Models.Tools;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace KPInt.Controls.DrawingTools
{
    /// <summary>
    /// Логика взаимодействия для ToolsWindow.xaml
    /// </summary>
    public partial class ToolsWindow : Window
    {
        private readonly ValueSelectorVM _rValueSelector;
        private readonly ValueSelectorVM _gValueSelector;
        private readonly ValueSelectorVM _bValueSelector;

        public Color GetDrawingColor(bool primary) => primary ? _primaryColor.Color : _secondaryColor.Color;

        public void SetDrawingColor(Color color, bool primary)
        {
            if (primary)
            {
                _rValueSelector.Value = color.R;
                _gValueSelector.Value = color.G;
                _bValueSelector.Value = color.B;
            }
            else
            {
                _secondaryColor.Color = color;
            }
        }

        private readonly SolidColorBrush _primaryColor;
        private readonly SolidColorBrush _secondaryColor;

        public ToolsWindow(IDrawingTool pencil, IDrawingTool line, IDrawingTool picker)
        {
            InitializeComponent();
            _rValueSelector = new ValueSelectorVM(0, 255, 0, 1);
            _gValueSelector = new ValueSelectorVM(0, 255, 0, 1);
            _bValueSelector = new ValueSelectorVM(0, 255, 0, 1);

            _rValueSelector.PropertyChanged += Color_Updated;
            _gValueSelector.PropertyChanged += Color_Updated;
            _bValueSelector.PropertyChanged += Color_Updated;

            RColorPlaceholder.Content = _rValueSelector.View;
            GColorPlaceholder.Content = _gValueSelector.View;
            BColorPlaceholder.Content = _bValueSelector.View;

            _primaryColor = new SolidColorBrush();
            PrimaryColorRect.Background = _primaryColor;
            PrimaryColorRect.MouseRightButtonDown += PrimaryColorRect_MouseDown;

            _secondaryColor = new SolidColorBrush(Colors.White);
            SecondaryColorRect.Background = _secondaryColor;

            PencilToolButton.Checked += Tool_Selected;
            LineToolButton.Checked += Tool_Selected;
            PickerToolButton.Checked += Tool_Selected;

            PencilToolButton.Checked += (s, e) => pencil?.Attach();
            PencilToolButton.Unchecked += (s, e) => pencil?.Detach();

            LineToolButton.Checked += (s, e) => line?.Attach();
            LineToolButton.Unchecked += (s, e) => line?.Detach();

            PickerToolButton.Checked += (s, e) => picker?.Attach();
            PickerToolButton.Unchecked += (s, e) => picker?.Detach();

            PencilToolButton.IsChecked = true;
            Color_Updated(this, null);
        }

        private void PrimaryColorRect_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _secondaryColor.Color = _primaryColor.Color;
        }

        private void Color_Updated(object sender, PropertyChangedEventArgs e)
        {
            _primaryColor.Color = Color.FromRgb((byte)_rValueSelector.Value, (byte)_gValueSelector.Value, (byte)_bValueSelector.Value);
        }

        private void Tool_Selected(object sender, RoutedEventArgs e)
        {
            if (PencilToolButton != sender) PencilToolButton.IsChecked = false;
            if (LineToolButton != sender) LineToolButton.IsChecked = false;
            if (PickerToolButton != sender) PickerToolButton.IsChecked = false;
        }
    }
}
