using System;
using System.Windows;
using System.Windows.Controls;

namespace KPInt.Controls
{
    /// <summary>
    /// Логика взаимодействия для ServerConnectionControl.xaml
    /// </summary>
    public partial class ServerConnectionControl : UserControl
    {
        public ServerConnectionControl()
        {
            InitializeComponent();

            CreateRoomRadioButton.Click += SelectedRadioButtonChanged;
            ConnectToRoomRadioButton.Click += SelectedRadioButtonChanged;
        }

        void SelectedRadioButtonChanged(object sender, EventArgs e)
        {
            CreateRoomControls.Visibility =
                (CreateRoomRadioButton.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
            ConnectToRoomControls.Visibility =
                (ConnectToRoomRadioButton.IsChecked == true) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
