using KPInt_Shared.Communication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KPInt_Server
{
    class Room
    {
        private const int TIMEOUT = Protocol.REFRESH_DELAY / 10;

        public string Name { get; }

        public string Password { get; }

        private readonly Dictionary<int, UserUdpConnection> _clients;
        private DateTime? ExpirationTime;

        public Room(string name, string password)
        {
            Name = name;
            Password = password;
            _clients = new Dictionary<int, UserUdpConnection>();
            CheckExpired();
        }

        public bool Consume(UdpMessage message, ProtocolUdpClient server)
        {
            if (!_clients.ContainsKey(message.UserId)) return false;

            var client = _clients[message.UserId];

            if (client.EndPoint == null)
                client.EndPoint = message.EndPoint;

            foreach (var roommate in _clients.Values)
                if (message.colorLine.Valid != (roommate == client))
                    roommate.Send(message, server);

            return true;
        }

        public bool CheckExpired()
        {
            foreach (var key in _clients.Keys.ToList())
            {
                if (key != _clients[key].ConnectedUser.Id)
                {
                    Console.WriteLine("Room {0}: user {1} timed out", Name, _clients[key].ConnectedUser.Name);
                    _clients.Remove(key);
                }
            }

            if (_clients.Count > 0)
            {
                if (ExpirationTime != null)
                {
                    Console.WriteLine("Room {0} removed from deletion queue", Name);
                    ExpirationTime = null;
                }

                return false;
            }

            if (ExpirationTime == null)
            {
                Console.WriteLine("Room {0} empty, deleting in {1} seconds", Name, TIMEOUT);
                ExpirationTime = DateTime.Now;
            }

            return (DateTime.Now - ExpirationTime.Value).TotalSeconds > TIMEOUT;
        }

        public void AddUser(User user)=> _clients.Add(user.Id, new UserUdpConnection(user));

        public string PublicName => (string.IsNullOrEmpty(Password) ? " |" : "x|") + Name;
    }
}
