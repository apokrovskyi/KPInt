using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace KPInt.Controls.ValueInput
{
    class ValueInputVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FrameworkElement View => _control;

        private readonly ValueInput _control;

        private double _value;

        public double Value
        {
            get => _value;
            set
            {
                if (_value == value) return;

                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public string Text { get { return _control.ValueLabel.Text; } set { _control.ValueLabel.Text = value; } }

        public ValueInputVM(double minimum, double maximum, int initial, int tick)
        {
            _control = new ValueInput();
            _control.ValueSlider.Minimum = minimum;
            _control.ValueSlider.Maximum = maximum;
            _control.ValueSlider.TickFrequency = tick;
            Value = initial;
        }

        private void TextUpdated()
        {
            if (Equals(ValueTextBox.Text, _prevValue)) return;

            byte value = 0;

            if (ValueTextBox.Text.Length > 0 && !byte.TryParse(ValueTextBox.Text, out value))
            {
                UpdateValue(_prevValue);
                return;
            }

            Value = value;
        }

        private void UpdateValue(byte value)
        {
            _prevValue = value;
            ValueTextBox.Text = _prevValue.ToString();
            ValueTextBox.CaretIndex = ValueTextBox.Text.Length;
            ValueChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
        }
    }
}
