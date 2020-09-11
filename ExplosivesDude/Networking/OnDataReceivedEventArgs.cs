namespace ExplosivesDude.Networking
{
    using System;

    public class OnDataReceivedEventArgs : EventArgs
    {
        public OnDataReceivedEventArgs(int senderId, byte[] data)
        {
            SenderId = senderId;
            Data = data;
        }

        public int SenderId { get; set; }

        public byte[] Data { get; private set; }
    }
}
