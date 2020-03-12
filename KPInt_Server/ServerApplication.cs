using KPInt_Shared;
using KPInt_Shared.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace KPInt_Server
{
    class ServerApplication
    {
        private readonly LockedValue<bool> _flag;
        private readonly LockedValue<List<UserTcpConnection>> _clients;
        private readonly LockedValue<Dictionary<string, Room>> _rooms;
        private string _host;

        public ServerApplication(LockedValue<bool> flag, string host)
        {
            _host = host;
            _flag = flag;
            _clients = new LockedValue<List<UserTcpConnection>>(new List<UserTcpConnection>());
            _rooms = new LockedValue<Dictionary<string, Room>>(new Dictionary<string, Room>());
            var newRoom = new Room("default_room", "");
            _rooms.Act(x => { x[newRoom.PublicName] = newRoom; });
        }

        public void Listen()
        {
            TcpListener server = null;
            server = new TcpListener(IPAddress.Parse(_host), Protocol.TCP_PORT);

            server.Start();

            while (_flag.Retrieve((x) => x))
            {
                if (!server.Pending())
                {
                    Protocol.Yield();
                    continue;
                }
                var client = new ProtocolTcpClient();
                client.Configure(server.AcceptTcpClient());
                var message = client.RecvMessage();

                if (message.Length > 0 && message.Code == MessageCode.CONNECT &&
                    Equals(Protocol.VERSION, message.GetReader().ReadString()))
                {
                    var name = RandomNameGen.GetName();
                    Console.WriteLine("Connection: {0} => {1}", client.Address, name);
                    _clients.Act(x => x.Add(new UserTcpConnection(name, client)));
                }
                else
                    client.Close();
            }

            server.Stop();
        }

        public void Process()
        {
            while (_flag.Retrieve((x) => x))
            {
                var len = _clients.Retrieve(x => x.Count);

                for (int i = 0; i < len; i++)
                {
                    var user = _clients.Retrieve(x => x[i]);
                    if (!user.Connected)
                    {
                        Console.WriteLine("Disconnected: " + user.Address);
                        _clients.Act(x => x[i] = null);
                    }

                    var msg = user.Recv();
                    if (msg.UnDefined) continue;

                    if (msg.Code == MessageCode.GET_ROOMS)
                    {
                        var writer = new ByteArrayWriter();
                        _rooms.Act(x =>
                        {
                            foreach (var room in x.Values)
                                writer.Append(room.PublicName);
                        });
                        user.Send(new Message { Code = MessageCode.GET_ROOMS, Body = writer.Array });
                    }
                    else if (msg.Code == MessageCode.ADD_ROOM)
                    {
                        var reader = msg.GetReader();
                        var name = reader.ReadString();
                        var pass = reader.ReadString();
                        var newRoom = new Room(name, pass);
                        _rooms.Act(x =>
                        {
                            if (!x.ContainsKey(name))
                                x[newRoom.PublicName] = newRoom;
                        });
                        Console.WriteLine("User {0} added a room: {1}({2})", user.ConnectedUser.Name, newRoom.Name, newRoom.Password);
                    }
                    else if (msg.Code == MessageCode.JOIN_ROOM)
                    {
                        var reader = msg.GetReader();
                        var name = reader.ReadString();
                        var pass = reader.ReadString();
                        _rooms.Act(x =>
                        {
                            if (x.ContainsKey(name))
                            {
                                var room = x[name];
                                if (Equals(room.Password, pass))
                                {
                                    user.Send(new Message
                                    {
                                        Code = MessageCode.JOIN_ROOM,
                                        Body = new ByteArrayWriter()
                                        .Append(room.AddUser(user.ConnectedUser)).Array
                                    });

                                    Console.WriteLine("User {0} joined room {1} => {2}", user.ConnectedUser.Name, room.Name, user.ConnectedUser.Id);
                                }
                            }
                        });
                    }
                }

                _clients.Act(x => x.RemoveAll(y => y == null));

                Protocol.Yield();
            }
        }

        public void Broadcast()
        {
            var server = new ProtocolUdpClient(new UdpClient(Protocol.UDP_PORT));

            while (_flag.Retrieve((x) => x))
            {
                bool recv = false;

                var msg = server.RecvMessage();

                _rooms.Act(x =>
                {
                    foreach (var room in x.Values.ToList())
                    {
                        if (msg != null && room.Consume(msg, server))
                        {
                            recv = true;
                            msg = null;
                        }
                        else if (room.CheckExpired())
                        {
                            Console.WriteLine("Room {0} timed out, removing", room.Name);
                            x.Remove(room.PublicName);
                        }
                    }
                });

                if (!recv) Protocol.Yield();
            }
        }
    }
}
