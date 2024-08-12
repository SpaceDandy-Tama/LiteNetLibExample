using LiteNetLib;
using static System.Net.Mime.MediaTypeNames;

namespace LiteNetLib_HostTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Press Enter for host (server & client)");
            Console.WriteLine("Press any other key for client only");

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                ServerManager.Initialize(9100);
            }

            ClientManager.Initialize();
            ClientManager.Connect("localhost", 9100);

            while(true)
            {
                await Task.Delay(50);

                if(ServerManager.Initialized)
                    ServerManager.PollEvents();

                ClientManager.PollEvents();

                if (Console.KeyAvailable)
                {
                    keyInfo = Console.ReadKey();
                    if (keyInfo.Key == ConsoleKey.Escape)
                        break;
                }
            }

            ClientManager.Stop();
            if (ServerManager.Initialized)
                ServerManager.Stop();
        }
    }
}
