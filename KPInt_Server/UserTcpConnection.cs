using KPInt_Shared.Communication;
using System;

namespace KPInt_Server
{
    class UserTcpConnection
    {
        private const int TIMEOUT = Protocol.REFRESH_DELAY * 10;

        public User ConnectedUser { get; }

        private readonly ProtocolTcpClient _client;

        private DateTime _lastInteraction;

        public string Address => _client.Address;

        public bool Connected => _client.Connected;

        private void Shake()
        {
            _lastInteraction = DateTime.Now;
        }

        public Message Recv()
        {
            var msg = _client.RecvMessage();

            if (msg.UnDefined)
            {
                if ((DateTime.Now - _lastInteraction).TotalMilliseconds > TIMEOUT)
                    Send(new Message { Code = MessageCode.PING });
            }
            else
                Shake();

            return msg;
        }

        public bool Send(Message message)
        {
            if (!Connected) return false;

            if (_client.SendMessage(message))
            {
                Shake();
                return true;
            }
            else
            {
                Close();
                return false;
            }
        }

        public void Close()
        {
            _client.Close();
            ConnectedUser.Id = -1;
        }

        public UserTcpConnection(string name, ProtocolTcpClient client)
        {
            ConnectedUser = new User(name);
            _client = client;
            Shake();
        }
    }
}
