using KPInt.Controls;
using System.ComponentModel;
using System.Net;

namespace KPInt
{
    class ServerConnectionVM
    {
        private readonly ServerConnectionControl _control;
        private readonly ConnectionManager _client;

        public event PropertyChangedEventHandler RoomChanged;

        public ServerConnectionVM(ConnectionManager client, ServerConnectionControl control, CanvasControl canvasControl)
        {
            _client = client;
            _control = control;
            _control.ConnectButton.Click += ConnectButton_Click;
            _control.ConnectToRoomButton.Click += ConnectToRoomButton_Click;
            _control.RefreshRoomsButton.Click += RefreshRoomsButton_Click;
            _control.CreateNewRoom.Click += CreateNewRoom_Click;
        }

        private void ConnectButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var correct = IPAddress.TryParse(_control.AddressTextBox.Text, out var addr);
            if (!correct) return;

            if (!_client.Connect(addr))
            {
                _control.StatusTextBox.Text = "Failed";
                _control.RoomSelectionControls.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            _control.StatusTextBox.Text = "Connected";
            _control.RoomSelectionControls.Visibility = System.Windows.Visibility.Visible;
        }

        private void RefreshRoomsButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _control.ConnectToRoomSelector.ItemsSource = _client.RefreshRooms();
            CheckDisconnected();
        }

        private void ConnectToRoomButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_control.ConnectToRoomSelector.SelectedIndex < 0) return;

            var dialog = new TextInputDialog("Enter the password");
            dialog.ShowDialog();
            _client.ConnectToRoom(_control.ConnectToRoomSelector.SelectedItem as string, dialog.ValueTextBox.Text);
            if (CheckDisconnected()) return;
            RoomChanged?.Invoke(this, new PropertyChangedEventArgs("RoomChanged"));
        }

        private void CreateNewRoom_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _client.CreateRoom(_control.NewRoomName.Text, _control.NewRoomPassword.Text);
        }

        private bool CheckDisconnected()
        {
            if (!_client.Connected)
            {
                _control.StatusTextBox.Text = "Disconnected";
                _control.RoomSelectionControls.Visibility = System.Windows.Visibility.Collapsed;
                _client.Disconnect();
                _control.ConnectToRoomSelector.ItemsSource = new string[0];
                return true;
            }
            return false;
        }
    }
}
