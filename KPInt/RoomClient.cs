using KPInt_Shared;
using KPInt_Shared.Communication;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace KPInt
{
    class RoomClient
    {
        public event PropertyChangedEventHandler LineReceived;

        public LockedValue<ColorLine> ReceivedLine { get; }

        private readonly LockedValue<ProtocolUdpClient> _client;
        private readonly LockedValue<bool> _flag;
        private readonly UdpMessage _configuration;
        private bool _registered = false;
        private readonly int _timeout;

        public IPAddress Server { set { _configuration.EndPoint.Address = value; } }
        public int Id
        {
            set
            {
                _configuration.UserId = value;
            }
        }
        public ColorLine Line
        {
            set
            {
                if (_configuration.UserId < 0) return;
                _configuration.colorLine = value;
                ReceivedLine.SetValue(_configuration.colorLine);
                LineReceived?.Invoke(this, new PropertyChangedEventArgs("ReceivedLine"));
                Send();
            }
        }

        public RoomClient(LockedValue<bool> flag, int fps)
        {
            _configuration = new UdpMessage();
            _configuration.EndPoint.Port = Protocol.UDP_PORT;
            _flag = flag;
            _timeout = 1000 / fps;
            var client = new UdpClient();
            _client = new LockedValue<ProtocolUdpClient>(new ProtocolUdpClient(client));

            ReceivedLine = new LockedValue<ColorLine>(null);
        }

        public void Receive()
        {
            while (_flag.Retrieve(x => x))
            {
                var msg = _client.Retrieve(x => x.RecvMessage());

                // timeout
                if (msg == null)
                {
                    if (!_registered) Send();
                    Thread.Sleep(_timeout);
                    continue;
                }

                _registered = true;

                if (msg.colorLine.Valid)
                {
                    ReceivedLine.SetValue(msg.colorLine);
                    LineReceived?.Invoke(this, new PropertyChangedEventArgs("ReceivedLine"));
                }
            }
        }

        public void Send()
        {
            if (_configuration.UserId >= 0 && _configuration.colorLine != null)
                _client.Act(x => x.SendMessage(_configuration));
        }
    }
}
