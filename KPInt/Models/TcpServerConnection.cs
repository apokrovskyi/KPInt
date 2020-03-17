using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using KPInt_Shared.Communication;
using System.Net;

namespace KPInt.Models
{
    class TcpServerConnection
    {
        public PropertyChangedEventHandler IsConnectedChanged;

        private readonly ProtocolTcpClient _client;

        public bool Connected => _client.Connected;

        public TcpServerConnection()
        {
            _client = new ProtocolTcpClient();
            _client.IsConnectedChanged += (s, e) => IsConnectedChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Connected)));
        }

        public bool Connect(IPAddress address)
        {
            if (!_client.Connect(address)) return false;

            _client.SendMessage(new Message
            {
                Code = MessageCode.CONNECT,
                Body = new ByteArrayWriter().Append(Protocol.VERSION).Array
            });

            return true;
        }

        public void RefreshRooms(IList<string> rooms)
        {
            rooms.Clear();

            _client.SendMessage(new Message { Code = MessageCode.GET_ROOMS });

            Message msg = RecvSpecific(MessageCode.GET_ROOMS);

            if (msg != null)
            {
                var reader = msg.GetReader();
                while (reader.Length > 0)
                    rooms.Add(reader.ReadString());
            }
        }

        public void CreateRoom(string name, string password)
        {
            _client.SendMessage(new Message
            {
                Code = MessageCode.ADD_ROOM,
                Body = new ByteArrayWriter().Append(name).Append(password).Array
            });
        }

        public int ConnectToRoom(string room, string password)
        {
            _client.SendMessage(new Message
            {
                Code = MessageCode.JOIN_ROOM,
                Body = new ByteArrayWriter().Append(room).Append(password).Array
            });

            var recv = RecvSpecific(MessageCode.JOIN_ROOM);
            if (recv == null) return -1;
            return recv.GetReader().ReadInt32();
        }

        private Message RecvSpecific(MessageCode code, int timeout = 5)
        {
            var startTime = DateTime.Now;

            while (true)
            {
                var msg = _client.RecvMessage();
                if (msg == null)
                {
                    if ((DateTime.Now - startTime).TotalSeconds > timeout)
                    {
                        _client.Close();
                        return null;
                    }
                    Protocol.Yield();
                }
                if (msg.Code == code) return msg;
                else startTime = DateTime.Now;
            }
        }
    }
}
