using KPInt.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Input;


namespace KPInt.Controls.ServerConnection
{
    class ServerConnectionVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action RoomChanged;

        public bool IsServerConnected => _tcpConnection.Connected;
        public int RoomID { get; private set; }
        public IPAddress Address { get; private set; }

        public ObservableCollection<string> Rooms { get; }

        private readonly TcpServerConnection _tcpConnection;

        public FrameworkElement View => _control;

        private readonly ServerConnectionView _control;

        public ServerConnectionVM(TcpServerConnection connection)
        {
            Rooms = new ObservableCollection<string>();

            _tcpConnection = connection;
            _tcpConnection.IsConnectedChanged += TcpConnection_OnConnectedChanged;

            _control = new ServerConnectionView { DataContext = this };
            _control.ConnectButton.Click += ConnectButton_Click;
            _control.AddressTextBox.KeyDown += AddressTextBox_KeyDown;
            _control.RefreshRoomsButton.Click += (s, e) => _tcpConnection.RefreshRooms(Rooms);
            _control.CreateNewRoom.Click += (s, e) => _tcpConnection.CreateRoom(_control.NewRoomName.Text, _control.NewRoomPassword.Text);
            _control.ConnectToRoomButton.Click += ConnectToRoomButton_Click;
        }

        private void TcpConnection_OnConnectedChanged(object sender, PropertyChangedEventArgs e)
        {
            _control.StatusTextBox.Text = IsServerConnected ? "Connected" : "Disconnected";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsServerConnected)));
        }

        private void AddressTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ConnectButton_Click(sender, e);
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var correct = IPAddress.TryParse(_control.AddressTextBox.Text, out var addr);
            if (!correct) return;

            Address = addr;
            if (!_tcpConnection.Connect(addr))
                _control.StatusTextBox.Text = "Failed";
        }

        private void ConnectToRoomButton_Click(object sender, RoutedEventArgs e)
        {
            if (_control.RoomSelector.SelectedIndex < 0) return;

            var dialog = new TextInputDialog();
            dialog.ShowDialog();
            RoomID = _tcpConnection.ConnectToRoom(
                _control.RoomSelector.SelectedItem as string,
                dialog.PasswordTextBox.Text);
            if (RoomID < 0) return;

            RoomChanged?.Invoke();
        }
    }
}
