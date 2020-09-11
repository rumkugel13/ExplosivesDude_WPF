namespace ExplosivesDude.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class NetworkServer : NetworkBase
    {
        private readonly Dictionary<int, TcpClient> clients;
        private bool isListening;
        private TcpListener tcpListener;

        public NetworkServer()
        {
            clients = new Dictionary<int, TcpClient>();
        }

        public int ConnectionCount => clients.Count;

        public bool IsActive { get; private set; }

        public void StartServer(int port)
        {
            Console.WriteLine("INFO: Server started");
            IsActive = true;
            Listen(port);
        }

        public void StopServer()
        {
            isListening = false;
            tcpListener.Stop();
            IsActive = false;
            Console.WriteLine("INFO: Server stopped");

            foreach (TcpClient tcpClient in clients.Values)
            {
                Disconnect(tcpClient);
            }
        }

        public void DisconnectAll()
        {
            foreach (TcpClient tcpClient in clients.Values)
            {
                Disconnect(tcpClient);
            }
        }

        public void Disconnect(int clientId)
        {
            if (clients.ContainsKey(clientId))
            {
                Disconnect(clients[clientId]);
                Console.WriteLine("Disconnected");
            }
        }

        public int[] GetIds()
        {
            int[] ids = new int[clients.Count];
            clients.Keys.CopyTo(ids, 0);
            return ids;
        }

        public bool SendMessage(int clientId, byte[] message)
        {
            if (clients.ContainsKey(clientId))
            {
                SendMessage(clients[clientId].GetStream(), message);
                return true;
            }

            return false;
        }

        public void Broadcast(byte[] message)
        {
            foreach (TcpClient tcpClient in clients.Values)
            {
                SendMessage(tcpClient.GetStream(), message);
            }
        }

        public void BroadcastExcept(byte[] message, int clientIdException)
        {
            foreach (int id in clients.Keys)
            {
                if (id != clientIdException)
                {
                    SendMessage(clients[id].GetStream(), message);
                }
            }
        }

        public void Multicast(byte[] message, int[] clients)
        {
            foreach (int id in clients)
            {
                if (this.clients.ContainsKey(id))
                {
                    SendMessage(this.clients[id].GetStream(), message);
                }
            }
        }

        protected override void OnConnectionChanged(OnConnectionChangedEventArgs e)
        {
            if (!e.Connected)
            {
                clients.Remove(e.ClientId);
            }

            base.OnConnectionChanged(e);
        }

        private async void Listen(int port = 25566)
        {
            Console.WriteLine("INFO: Starting Listener on " + port);
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            isListening = true;

            try
            {
                while (isListening)
                {
                    if (tcpListener.Pending())
                    {
                        TcpClient tcpClient = tcpListener.AcceptTcpClient();
                        int identifier = tcpClient.Client.RemoteEndPoint.GetHashCode();
                        tcpClient.NoDelay = true;
                        clients.Add(identifier, tcpClient);
                        ReadIncomingData(tcpClient);
                    }

                    await Task.Delay(10);
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("INFO: Stopped listening");
                tcpListener.Stop();
                isListening = false;
            }
        }
    }
}
