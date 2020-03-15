using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KPInt.Controls.ValueSelector;
using System.ComponentModel;

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

        public Color DrawingColor => _primaryColor.Color;

        private readonly SolidColorBrush _primaryColor;

        public ToolsWindow()
        {
            InitializeComponent();
            _rValueSelector = new ValueSelectorVM(0, 255, 1, 1, "R");
            _gValueSelector = new ValueSelectorVM(0, 255, 1, 1, "G");
            _bValueSelector = new ValueSelectorVM(0, 255, 1, 1, "B");

            _rValueSelector.PropertyChanged += Color_Updated;
            _gValueSelector.PropertyChanged += Color_Updated;
            _bValueSelector.PropertyChanged += Color_Updated;

            RColorPlaceholder.Content = _rValueSelector.View;
            GColorPlaceholder.Content = _gValueSelector.View;
            BColorPlaceholder.Content = _bValueSelector.View;

            _primaryColor = new SolidColorBrush();
            PrimaryColorRect.Background = _primaryColor;

            Color_Updated(this, null);
        }

        private void Color_Updated(object sender, PropertyChangedEventArgs e)
        {
            _primaryColor.Color = Color.FromRgb((byte)_rValueSelector.Value, (byte)_gValueSelector.Value, (byte)_bValueSelector.Value);
        }
    }
}
