using System.Net;
using System.Net.Sockets;

namespace KPInt_Shared.Communication
{
    public class UdpMessage
    {
        public byte[] GetBytes()
        {
            return new ByteArrayWriter()
                .Append(UserId)
                .Append(colorLine)
                .Array;
        }

        public IPEndPoint EndPoint = new IPEndPoint(0, 0);
        public int UserId = -1;
        public ColorLine colorLine = null;
    }

    public class ProtocolUdpClient
    {
        private readonly UdpClient _client;

        public ProtocolUdpClient(UdpClient client)
        {
            _client = client;
        }

        public UdpMessage RecvMessage(bool wait = false)
        {
            if (!wait && _client.Available == 0) return null;

            var res = new UdpMessage();
            byte[] msg;

            try
            {
                msg = _client.Receive(ref res.EndPoint);
            }
            catch
            {
                return null;
            }

            var reader = new ByteArrayReader(msg);
            res.UserId = reader.ReadInt32();
            res.colorLine = reader.ReadColorLine();

            return res;
        }

        public void SendMessage(UdpMessage message)
        {
            var msg = message.GetBytes();
            _client.Send(msg, msg.Length, message.EndPoint);
        }
    }
}
