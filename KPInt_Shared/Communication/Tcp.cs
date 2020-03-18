using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace KPInt_Shared.Communication
{
    public enum MessageCode
    {
        CONNECT,
        GET_ROOMS,
        ADD_ROOM,
        JOIN_ROOM,
        PING
    }

    public class Message
    {
        public ByteArrayReader GetReader()
        {
            return new ByteArrayReader(Body ?? new byte[0]);
        }

        public byte[] GetBytes()
        {
            var w = new ByteArrayWriter()
                .Append((int)Code)
                .Append(Length);
            if (Body != null) w.Append(Body);
            return w.Array;
        }

        public Int32 Length => Body?.Length ?? 0;
        public MessageCode Code = MessageCode.PING;
        public byte[] Body = null;
    }

    public class ProtocolTcpClient
    {
        public event PropertyChangedEventHandler IsConnectedChanged;

        private TcpClient _tcpClient = null;

        public string Address { get; private set; } = "";

        public bool Connected
        {
            get => _connected;
            private set
            {
                if (_connected == value) return;

                _connected = value;
                IsConnectedChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Connected)));
            }
        }

        private bool _connected;

        public bool Connect(IPAddress address)
        {
            var client = new TcpClient();

            try
            {
                client.Connect(address, Protocol.TCP_PORT);
                Configure(client);
            }
            catch { }

            return Connected;
        }

        /// <summary> Configures the Client with the context. </summary>
        /// <param name="client">It has to be opened and connected to work properly</param>
        public void Configure(TcpClient client)
        {
            Close();
            _tcpClient = client;
            Address = _tcpClient.Client.RemoteEndPoint.ToString();
            Connected = true;
        }

        public Message RecvMessage()
        {
            if (!Connected || _tcpClient.Available == 0)
                return null;

            var buffer = new byte[8];
            ReadAll(buffer);

            var reader = new ByteArrayReader(buffer);
            var message = new Message
            {
                Code = (MessageCode)reader.ReadInt32(),
                Body = new byte[reader.ReadInt32()]
            };
            ReadAll(message.Body);

            return message;
        }

        public void SendMessage(Message message)
        {
            try
            {
                var bytes = message.GetBytes();
                _tcpClient.GetStream().Write(bytes, 0, bytes.Length);
            }
            catch
            {
                Close();
            }
        }

        public void Close()
        {
            Connected = false;
            _tcpClient?.Close();
        }

        private void ReadAll(byte[] buffer)
        {
            int index = 0;
            while (index < buffer.Length)
                index += _tcpClient.GetStream().Read(buffer, index, buffer.Length - index);
        }
    }
}
