using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace KPInt.Controls.ValueSelector
{
    class ValueSelectorVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FrameworkElement View => _control;

        private readonly ValueSelectorView _control;

        private double _value;

        public double Value
        {
            get => _value;
            set
            {
                if (_value == value) return;

                _value = value;
                UpdateTextBoxValue();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public ValueSelectorVM(double minimum, double maximum, double initial, double tick, string text)
        {
            _control = new ValueSelectorView { DataContext = this };
            _control.ValueSlider.Minimum = minimum;
            _control.ValueSlider.Maximum = maximum;
            _control.ValueSlider.TickFrequency = tick;
            _control.ValueLabel.Text = text;
            Value = initial;

            _control.ValueTextBox.TextChanged += ValueTextBox_TextChanged;
            _control.ValueSlider.MouseWheel += ValueSlider_MouseWheel;
        }

        private void ValueTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var text = _control.ValueTextBox.Text;
            if (text.Length == 0) text = "0";
            if (text.Equals(_value.ToString())) return;

            if (!double.TryParse(text, out var number) ||
                number < _control.ValueSlider.Minimum ||
                number > _control.ValueSlider.Maximum)
            {
                UpdateTextBoxValue();
                return;
            }

            Value = number;
        }

        private void ValueSlider_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var value = Math.Max(Value + e.Delta/5, _control.ValueSlider.Minimum);
            Value = Math.Min(value, _control.ValueSlider.Maximum);
        }

        private void UpdateTextBoxValue()
        {
            var text = _value.ToString();
            _control.ValueTextBox.Text = text;
            _control.ValueTextBox.CaretIndex = text.Length;
        }
    }
}
