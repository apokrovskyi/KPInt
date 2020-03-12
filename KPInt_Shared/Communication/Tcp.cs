using System;
using System.Net;
using System.Net.Sockets;

namespace KPInt_Shared.Communication
{
    public enum MessageCode
    {
        UNDEFINED,
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

        public bool UnDefined => Length == 0 && Code == MessageCode.UNDEFINED;
        public Int32 Length => Body?.Length ?? 0;
        public MessageCode Code = MessageCode.UNDEFINED;
        public byte[] Body = null;
    }

    public class ProtocolTcpClient
    {
        public int Timeout { get; set; } = 5; //seconds

        private TcpClient _tcpClient = null;

        public string Address { get; private set; } = "";

        public bool Connected { get; private set; } = false;

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

        /// <summary>
        /// Configures the Client with the context
        /// </summary>
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
            var message = new Message();
            if (!Connected)
                return message;

            if (_tcpClient.Available == 0) return message;

            var buffer = new byte[8];
            ReadAll(buffer);

            message.Code = (MessageCode)BitConverter.ToInt32(buffer, 0);
            var len = BitConverter.ToInt32(buffer, 4);

            message.Body = new byte[len];
            ReadAll(message.Body);

            //Console.WriteLine("[{0}] >> {1}", Address, string.Join(" ", message.GetBytes()));

            return message;
        }

        public bool SendMessage(Message message)
        {
            try
            {
                WriteAll(message.GetBytes());
            }
            catch
            {
                Close();
                return false;
            }

            //Console.WriteLine("[{0}] << {1}", Address, string.Join(" ", message.GetBytes()));

            return true;
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

        private void WriteAll(byte[] data)
        {
            _tcpClient.GetStream().Write(data, 0, data.Length);
        }
    }
}
