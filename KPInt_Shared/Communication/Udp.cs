using System;
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
            _client.Client.ReceiveTimeout = Protocol.REFRESH_DELAY * 10;
        }

        public UdpMessage RecvMessage()
        {
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

            ReadMsg(res, msg);

            return res;
        }

        public void StartReceiver(LockedValue<bool> alive, Action<UdpMessage> callback)
        {
            _client.BeginReceive(x =>
            {
                var res = new UdpMessage();
                byte[] msg;

                try
                {
                    msg = _client.EndReceive(x, ref res.EndPoint);
                }
                catch
                {
                    callback(null);
                    return;
                }

                ReadMsg(res, msg);

                callback(res);
                if (alive.Retrieve(y => y))
                    StartReceiver(alive, callback);
            }, _client);
        }

        public void SendMessage(UdpMessage message)
        {
            var msg = message.GetBytes();
            _client.Send(msg, msg.Length, message.EndPoint);
        }

        private void ReadMsg(UdpMessage message, byte[] buffer)
        {
            var reader = new ByteArrayReader(buffer);
            message.UserId = reader.ReadInt32();
            message.colorLine = reader.ReadColorLine();
        }
    }
}
