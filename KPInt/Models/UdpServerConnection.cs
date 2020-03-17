using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using KPInt_Shared;
using KPInt_Shared.Communication;
using System.Net;
using System.Net.Sockets;

namespace KPInt.Models
{
    class UdpServerConnection
    {
        public event PropertyChangedEventHandler LineReceived;

        public LockedValue<NewColorLine> ReceivedLine { get; }

        public IPAddress Address { set => _configuration.EndPoint.Address = value; }

        private readonly LockedValue<bool> _running;
        private readonly LockedValue<ProtocolUdpClient> _client;
        private readonly UdpMessage _configuration;

        public UdpServerConnection()
        {
            _client = new LockedValue<ProtocolUdpClient>(new ProtocolUdpClient(new UdpClient()));
            _running = new LockedValue<bool>(false);
            _configuration = new UdpMessage();
            _configuration.EndPoint.Port = Protocol.UDP_PORT;

            ReceivedLine = new LockedValue<NewColorLine>(null);
        }

        public void SendLine(NewColorLine colorLine)
        {
            if (!_running.Retrieve(x => x)) return;

            _configuration.colorLine = colorLine;
            _client.Act(x => x.SendMessage(_configuration));
        }

        public void Start(int ID)
        {
            Stop();
            _configuration.UserId = ID;

            var line = NewColorLine.Empty;

            while (true)
            {
                SendLine(line);
                if (_client.Retrieve(x => x.RecvMessage()) != null) break;
            }

            _running.SetValue(true);
            RecvCallback(null);
        }

        public void Stop() => _running.SetValue(false);

        private void RecvCallback(UdpMessage msg)
        {
            if (msg != null && msg.colorLine.Valid)
            {
                ReceivedLine.SetValue(msg.colorLine);
                LineReceived?.Invoke(this, new PropertyChangedEventArgs("ReceivedLine"));
            }

            if (_running.Retrieve(x => x))
                _client.Act(x => x.BeginRecv(RecvCallback));
        }
    }
}
