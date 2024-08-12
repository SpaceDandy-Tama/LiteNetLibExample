using LiteNetLib;

namespace LiteNetLib_HostTest
{
    public static class ClientManager
    {
        public static bool Initialized;

        private static EventBasedNetListener Listener = new EventBasedNetListener();
        private static NetManager NetManager;

        public static void Initialize()
        {
            Listener = new EventBasedNetListener();
            NetManager = new NetManager(Listener);
            NetManager.Start();

            Listener.PeerDisconnectedEvent += Listener_PeerDisconnectedEvent;
            Listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;

            Initialized = true;

            Console.WriteLine("Initialized Client");
        }

        public static void Connect(string IP, int port) => NetManager.Connect(IP, port, string.Empty);
        public static void PollEvents() => NetManager.PollEvents();
        public static void Stop() => NetManager.Stop();

        private static void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine($"Client: PeerDisconected: {peer.Address.ToString()}:{peer.Port}. Reason: {disconnectInfo.Reason}");
        }

        private static void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            byte receivedValue = reader.GetByte();
            if(receivedValue == 255)
            {
                Random rnd = new Random();
                byte randomValue = (byte)rnd.Next(0, byte.MaxValue);
                peer.Send(new byte[] { randomValue }, DeliveryMethod.ReliableOrdered);
                Console.WriteLine($"Client: Random Value Sent: {randomValue}");
            }
            else
            {
                Console.WriteLine($"Client: Received {receivedValue} from {peer.Address.ToString()}:{peer.Port}");
            }
        }
    }
}
