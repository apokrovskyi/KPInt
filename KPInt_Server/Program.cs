using KPInt_Shared;
using System;
using System.Threading.Tasks;

namespace KPInt_Server
{
    class Program
    {
        static void Main()
        {
            var flag = new LockedValue<bool>(true);
            var hostAddress = Console.ReadLine();

            var Server = new ServerApplication(flag, hostAddress);

            Task.Run(Server.Listen);
            Task.Run(Server.Process);
            Task.Run(Server.Broadcast);

            Console.ReadLine();
            flag.SetValue(false);
        }
    }
}
