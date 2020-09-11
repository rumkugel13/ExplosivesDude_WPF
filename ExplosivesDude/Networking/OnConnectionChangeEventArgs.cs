namespace ExplosivesDude.Networking
{
    using System;

    public class OnConnectionChangedEventArgs : EventArgs
    {
        public OnConnectionChangedEventArgs(bool connected, int clientId)
        {
            Connected = connected;
            ClientId = clientId;
        }

        public int ClientId { get; set; }

        public bool Connected { get; set; }
    }
}
