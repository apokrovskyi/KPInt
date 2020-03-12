using KPInt_Shared.Communication;
using System;
using System.Collections.Generic;
using System.Net;

namespace KPInt
{
    class ConnectionManager
    {
        private readonly ProtocolTcpClient _client;
        private readonly RoomClient _roomClient;

        public bool Connected => _client.Connected;

        public ConnectionManager(RoomClient client)
        {
            _client = new ProtocolTcpClient();
            _roomClient = client;
        }

        public List<string> RefreshRooms()
        {
            _client.SendMessage(new Message { Code = MessageCode.GET_ROOMS });

            Message msg = RecvSpecific(MessageCode.GET_ROOMS);

            var res = new List<string>();

            if (msg != null)
            {
                var reader = msg.GetReader();
                while (reader.Length > 0)
                    res.Add(reader.ReadString());
            }

            return res;
        }

        public void CreateRoom(string name, string password)
        {
            _client.SendMessage(new Message
            {
                Code = MessageCode.ADD_ROOM,
                Body = new ByteArrayWriter().Append(name).Append(password).Array
            });
        }

        public void ConnectToRoom(string name, string password)
        {
            _client.SendMessage(new Message
            {
                Code = MessageCode.JOIN_ROOM,
                Body = new ByteArrayWriter().Append(name).Append(password).Array
            });

            var recv = RecvSpecific(MessageCode.JOIN_ROOM);
            if (recv == null) return;
            _roomClient.Id = recv.GetReader().ReadInt32();
        }

        public bool Connect(IPAddress address)
        {
            Disconnect();
            if (!_client.Connect(address)) return false;

            _roomClient.Server = address;
            return _client.SendMessage(new Message { Code = MessageCode.CONNECT, Body = new ByteArrayWriter().Append(Protocol.VERSION).Array });
        }

        public void Disconnect()
        {
            _roomClient.Id = -1;
        }

        private Message RecvSpecific(MessageCode code, int timeout = 5)
        {
            var startTime = DateTime.Now;

            while (true)
            {
                var msg = _client.RecvMessage();
                if (msg.Code == code) return msg;
                if (msg.UnDefined)
                {
                    if ((DateTime.Now - startTime).TotalSeconds > timeout)
                    {
                        _client.Close();
                        return null;
                    }
                    Protocol.Yield();
                }
                else startTime = DateTime.Now;
            }
        }
    }
}
