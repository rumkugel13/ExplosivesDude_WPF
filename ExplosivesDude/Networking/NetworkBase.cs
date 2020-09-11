namespace ExplosivesDude.Networking
{
    using System;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public abstract class NetworkBase
    {
        public NetworkBase()
        {
        }

        public event EventHandler<OnDataReceivedEventArgs> DataReceived;

        public event EventHandler<OnConnectionChangedEventArgs> ConnectionChanged;

        public enum Commands
        {
            Broadcast, BroadcastExcept, Authenticate, Verify, Message, Error, Warning,
        }

        public enum Status
        {
            ConnectionEstablished, ConnectionTerminated, DataReceived, RemoteHostClosed, SendSuccess, SendFail
        }

        protected bool SendMessage(NetworkStream clientStream, byte[] message)
        {
            if (clientStream != null && clientStream.CanWrite && message.Length <= ushort.MaxValue)
            {
                byte[] header = BitConverter.GetBytes((ushort)message.Length);
                byte[] buffer = new byte[header.Length + message.Length];
                Array.Copy(header, 0, buffer, 0, header.Length);
                Array.Copy(message, 0, buffer, header.Length, message.Length);
                clientStream.Write(buffer, 0, buffer.Length);
                ////clientStream.Write(header, 0, header.Length);
                ////clientStream.Write(message, 0, message.Length);
                return true;
            }

            return false;
        }

        protected void Disconnect(TcpClient client)
        {
            if (client != null && client.Connected)
            {
                client.GetStream()?.Close();
                client.Close();
                client = null;
            }
        }

        protected virtual void OnConnectionChanged(OnConnectionChangedEventArgs e)
        {
            ConnectionChanged?.Invoke(this, e);
        }

        protected virtual void OnDataReceived(OnDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        protected async void ReadIncomingData(TcpClient tcpClient)
        {
            NetworkStream networkStream = tcpClient.GetStream();
            string remoteAddress = tcpClient.Client.RemoteEndPoint.ToString();
            int identifier = tcpClient.Client.RemoteEndPoint.GetHashCode();
            Console.WriteLine("INFO: Connection established to " + remoteAddress);
            OnConnectionChanged(new OnConnectionChangedEventArgs(true, identifier));

            try
            {
                while (true)
                {
                    // read header first
                    byte[] header = new byte[sizeof(ushort)];
                    if (await networkStream.ReadAsync(header, 0, header.Length) == 0)
                    {
                        Console.WriteLine("ERROR: Remotehost closed the connection.");
                        break;
                    }

                    // interpret header and read actual message
                    int bytesToRead = BitConverter.ToUInt16(header, 0);
                    byte[] message = new byte[bytesToRead];
                    int bytesRead = 0;

                    while (bytesRead < bytesToRead)
                    {
                        bytesRead += await networkStream.ReadAsync(message, bytesRead, bytesToRead - bytesRead);
                    }

                    OnDataReceived(new OnDataReceivedEventArgs(identifier, message));
                }
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("INFO: Connection manually terminated, shutting down");
            }
            finally
            {
                networkStream?.Close();
                tcpClient?.Close();
            }

            Console.WriteLine("INFO: Connection terminated from " + remoteAddress);
            OnConnectionChanged(new OnConnectionChangedEventArgs(false, identifier));
        }

        // Checks if a socket has disconnected
        // Adapted from -- http://stackoverflow.com/questions/722240/instantly-detect-client-disconnection-from-server-socket
        private static bool _isDisconnected(TcpClient client)
        {
            try
            {
                Socket s = client.Client;
                return s.Poll(10 * 1000, SelectMode.SelectRead) && s.Available == 0;
            }
            catch (SocketException)
            {
                // We got a socket error, assume it's disconnected
                return true;
            }
        }
    }
}
