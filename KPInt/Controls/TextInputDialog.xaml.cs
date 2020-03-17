using System.Windows;
using System.Windows.Input;

namespace KPInt.Controls
{
    /// <summary>
    /// Логика взаимодействия для TextInputDialog.xaml
    /// </summary>
    public partial class TextInputDialog : Window
    {
        public TextInputDialog()
        {
            InitializeComponent();
            PasswordTextBox.KeyDown += (s, e) => { if (e.Key == Key.Enter) Close(); };
            SubmitButton.Click += (s, e) => Close();
            PasswordTextBox.Focus();
        }
    }
}
