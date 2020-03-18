using KPInt_Shared;
using KPInt_Shared.Communication;
using System;
using System.Net;
using System.Net.Sockets;

namespace KPInt.Models
{
    class UdpServerConnection
    {
        public event Action LineReceived;
        public event Action Disconnected;

        public LockedValue<ColorLine> ReceivedLine { get; }

        public IPAddress Address { set => _configuration.EndPoint.Address = value; }

        private readonly LockedValue<ProtocolUdpClient> _client;
        private readonly UdpMessage _configuration;

        private LockedValue<bool> _token;

        public UdpServerConnection()
        {
            _client = new LockedValue<ProtocolUdpClient>(null);
            _configuration = new UdpMessage();
            _configuration.EndPoint.Port = Protocol.UDP_PORT;

            ReceivedLine = new LockedValue<ColorLine>(null);
        }

        public void SendLine(ColorLine colorLine)
        {
            if (_configuration.EndPoint.Address == null || _configuration.UserId < 0) return;
            _configuration.colorLine = colorLine;
            _client.Act(x => x?.SendMessage(_configuration));
        }

        public void Start(int ID)
        {
            Stop();
            _client.SetValue(new ProtocolUdpClient(new UdpClient()));
            _configuration.UserId = ID;

            var line = ColorLine.Empty;

            while (true)
            {
                SendLine(line);
                if (_client.Retrieve(x => x.RecvMessage()) != null) break;
            }

            _token = new LockedValue<bool>(true);
            _client.Act(x => x.StartReceiver(_token, RecvCallback));
        }

        public void Stop()
        {
            _configuration.UserId = -1;
            _client.SetValue(null);
            _token?.SetValue(false);
        }

        private void RecvCallback(UdpMessage msg)
        {
            if (msg == null)
            {
                Stop();
                Disconnected?.Invoke();
            }
            else
            {
                ReceivedLine.SetValue(msg.colorLine);
                LineReceived?.Invoke();
            }
        }
    }
}
