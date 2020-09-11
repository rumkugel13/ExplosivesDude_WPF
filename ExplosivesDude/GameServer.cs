namespace ExplosivesDude
{
    using System;
    using ExplosivesDude.Networking;

    public class GameServer
    {
        private readonly NetworkServer networkServer;
        private int seed;

        public GameServer()
        {
            this.networkServer = new NetworkServer();
            this.networkServer.ConnectionChanged += this.NetworkServer_OnConnectionChange;
            this.networkServer.DataReceived += this.NetworkServer_OnDataReceived;
        }

        public void StartServer()
        {
            this.networkServer.StartServer(Properties.Settings.Default.ServerPort);
        }

        public void NewGame()
        {
            Random r = new Random(); 
            this.seed = r.Next(1, int.MaxValue);
        }

        public void StopServer()
        {
            this.networkServer.StopServer();
        }

        private void NetworkServer_OnDataReceived(object sender, OnDataReceivedEventArgs e)
        {
            if (this.networkServer.IsActive)
            {
                ////this.networkServer.Broadcast(e.Data);
                this.networkServer.BroadcastExcept(e.Data, e.SenderId);
            }

            Console.WriteLine("DEBUG: Server received command \"" + ((Game.Commands)e.Data[0]).ToString() + "\" from byte array (" + string.Join(", ", e.Data) + ")");
        }

        private void NetworkServer_OnConnectionChange(object sender, OnConnectionChangedEventArgs e)
        {
            if (e.Connected)
            {
                // send the current seed to new player and add the boxes
                this.networkServer.SendMessage(e.ClientId, Common.AppendInt(new byte[] { (byte)Game.Commands.AddBoxes }, this.seed));

                // send command to add the new client
                this.networkServer.BroadcastExcept(Common.AppendInt(new byte[] { (byte)Game.Commands.NewClient }, e.ClientId), e.ClientId);

                // send new playercount to clients
                this.networkServer.Broadcast(new byte[] { (byte)Game.Commands.PlayerCount, (byte)this.networkServer.ConnectionCount });
            }
            else
            {
                if (this.networkServer.IsActive)
                {
                    // send info that a player disconnected
                    this.networkServer.BroadcastExcept(Common.AppendInt(new byte[] { (byte)Game.Commands.RemoveClient }, e.ClientId), e.ClientId);

                    // send new playercount to clients
                    this.networkServer.BroadcastExcept(new byte[] { (byte)Game.Commands.PlayerCount, (byte)this.networkServer.ConnectionCount }, e.ClientId);
                }
            }
        }
    }
}