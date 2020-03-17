using KPInt_Shared.Communication;
using System.Net;

namespace KPInt_Server
{
    class UserUdpConnection
    {
        public User ConnectedUser;
        public IPEndPoint EndPoint;

        public UserUdpConnection(User user)
        {
            ConnectedUser = user;
            EndPoint = null;
        }

        public void Send(UdpMessage message, ProtocolUdpClient client)
        {
            if (EndPoint == null) return;

            message.EndPoint = EndPoint;

            try
            {
                client.SendMessage(message);
            }
            catch
            {
                ConnectedUser.Id = -1;
            }
        }
    }
}
