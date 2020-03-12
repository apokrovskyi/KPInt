using System.Windows;
using System.Windows.Input;

namespace KPInt.Controls
{
    /// <summary>
    /// Логика взаимодействия для TextInputDialog.xaml
    /// </summary>
    public partial class TextInputDialog : Window
    {
        public TextInputDialog(string prompt)
        {
            InitializeComponent();
            PromptTextBlock.Text = prompt;
            ValueTextBox.KeyDown += (s, e) => { if (e.Key == Key.Enter) Close(); };
            ValueTextBox.Focus();
            SubmitButton.Click += (s, e) => Close();
        }
    }
}
