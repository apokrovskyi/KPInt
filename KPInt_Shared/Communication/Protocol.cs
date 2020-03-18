using System.Threading;

namespace KPInt_Shared.Communication
{
    public static class Protocol
    {
        public const string VERSION = "20200318";

        public const int TCP_PORT = 60060;

        public const int UDP_PORT = 60061;

        public const int REFRESH_DELAY = 500;

        public static void Yield()
        {
            Thread.Sleep(REFRESH_DELAY);
        }
    }
}
