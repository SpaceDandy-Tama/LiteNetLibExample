using LiteNetLib;

namespace LiteNetLib_HostTest
{
    public static class ServerManager
    {
        public static bool Initialized;

        private static EventBasedNetListener Listener;
        private static NetManager NetManager;

        public static void Initialize(int port)
        {
            Listener = new EventBasedNetListener();
            NetManager = new NetManager(Listener);
            NetManager.Start(port);

            Listener.ConnectionRequestEvent += Listener_ConnectionRequestEvent;
            Listener.PeerConnectedEvent += Listener_PeerConnectedEvent;
            Listener.PeerDisconnectedEvent += Listener_PeerDisconnectedEvent;
            Listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;

            Initialized = true;

            Console.WriteLine("Initialized Host");
        }

        public static void PollEvents() => NetManager.PollEvents();
        public static void Stop() => NetManager.Stop();

        private static void Listener_ConnectionRequestEvent(ConnectionRequest request)
        {
            request.Accept();
        }

        private static void Listener_PeerConnectedEvent(NetPeer peer)
        {
            Console.WriteLine($"Server: Peer Connected: {peer.Address.ToString()}:{peer.Port}");
            peer.Send(new byte[] { 255 }, DeliveryMethod.ReliableOrdered);
        }

        private static void Listener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine($"Server: PeerDisconected: {peer.Address.ToString()}:{peer.Port}. Reason: {disconnectInfo.Reason}");
        }

        private static void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            for(int i = 0; i < NetManager.ConnectedPeerList.Count; i++)
            {
                if (NetManager.ConnectedPeerList[i] != peer) //This check ensures the message is relayed to others but not sent back to the sender.
                    NetManager.ConnectedPeerList[i].Send(reader.RawData, reader.UserDataOffset, reader.UserDataSize, deliveryMethod);
            }
        }
    }
}
