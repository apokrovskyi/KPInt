using KPInt_Shared.Communication;
using System;

namespace KPInt_Server
{
    class UserTcpConnection
    {
        private const int TIMEOUT = Protocol.REFRESH_DELAY * 10;

        public User ConnectedUser { get; }

        public string Address => _client.Address;

        public bool Connected => _client.Connected;

        private readonly ProtocolTcpClient _client;
        private DateTime _lastInteraction;

        public UserTcpConnection(string name, ProtocolTcpClient client)
        {
            ConnectedUser = new User(name);
            _client = client;
            Shake();
        }

        public Message Recv()
        {
            var msg = _client.RecvMessage();

            if (msg == null)
            {
                if ((DateTime.Now - _lastInteraction).TotalMilliseconds > TIMEOUT)
                    Send(new Message { Code = MessageCode.PING });
            }
            else
                Shake();

            return msg;
        }

        public void Send(Message message)
        {
            if (!Connected) return;

            _client.SendMessage(message);

            if (Connected) Shake();
            else ConnectedUser.Id = -1;
        }

        private void Shake() => _lastInteraction = DateTime.Now;
    }
}
