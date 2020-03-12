using System.Windows;
using System.Windows.Controls;

namespace KPInt.Controls
{
    /// <summary>
    /// Логика взаимодействия для VisibilitySwitchControls.xaml
    /// </summary>
    public partial class VisibilitySwitchControl : UserControl
    {
        private readonly UIElement[] _controls;

        public VisibilitySwitchControl(bool initialValue, params UIElement[] controls)
        {
            InitializeComponent();

            _controls = controls;

            VisibilitySwitchCheckBox.Click += (s, e) => UpdateVisibility(VisibilitySwitchCheckBox.IsChecked == true);

            VisibilitySwitchCheckBox.IsChecked = initialValue;
            UpdateVisibility(initialValue);
        }

        private void UpdateVisibility(bool value)
        {
            var visibility = value ? Visibility.Visible : Visibility.Hidden;

            foreach (var control in _controls)
                control.Visibility = visibility;
        }
    }
}
