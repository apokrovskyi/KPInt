using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace KPInt.Controls
{
    /// <summary>
    /// Логика взаимодействия для ByteValueSlider.xaml
    /// </summary>
    public partial class ByteValueSlider : UserControl
    {
        public static readonly DependencyProperty FillColourProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ByteValueSlider),
                new PropertyMetadata(string.Empty, (d, e) => (d as ByteValueSlider).Text = e.NewValue as string));

        public event PropertyChangedEventHandler ValueChanged;

        private byte _prevValue;
        public byte Value
        {
            get
            {
                return _prevValue;
            }
            set
            {
                ValueSlider.Value = value;
            }
        }

        public string Text { get { return ValueName.Text; } set { ValueName.Text = value; } }

        public ByteValueSlider()
        {
            InitializeComponent();
            Value = 0;
            ValueTextBox.TextChanged += (s, e) => TextUpdated();
            ValueSlider.ValueChanged += (s, e) => UpdateValue((byte)ValueSlider.Value);
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
