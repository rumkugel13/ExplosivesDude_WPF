namespace ExplosivesDude.Networking
{
    using System;
    using System.Net.Sockets;

    public class NetworkClient : NetworkBase
    {
        private TcpClient tcpClient;

        public NetworkClient()
        {
        }

        public bool Connected { get; private set; }

        public async void Connect(string ip, int port)
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(ip, port);
                tcpClient.NoDelay = true;
                ReadIncomingData(tcpClient);
            }
            catch (SocketException)
            {
                Console.WriteLine("ERROR: Server couldn't be reached.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Disconnect()
        {
            Disconnect(tcpClient);
        }

        public void SendMessage(byte[] message)
        {
            SendMessage(tcpClient.GetStream(), message);
        }

        protected override void OnConnectionChanged(OnConnectionChangedEventArgs e)
        {
            Connected = e.Connected;
            base.OnConnectionChanged(e);
        }
    }
}
