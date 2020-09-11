namespace ExplosivesDude.Networking
{
    using System;

    class OnStatusUpdateEventArgs : EventArgs
    {
        private readonly byte[] rawData;
        private readonly NetworkBase.Status status;

        public OnStatusUpdateEventArgs(NetworkBase.Status status) : this(status, null)
        {
        }

        public OnStatusUpdateEventArgs(NetworkBase.Status status, byte[] rawData)
        {
            this.status = status;
            this.rawData = rawData;
        }

        public byte[] RawData => rawData;

        public NetworkBase.Status Status => status;

        public string GetStatusMessage()
        {
            switch (status)
            {
                case NetworkBase.Status.ConnectionEstablished:
                    return "Connection successfully established.";
                case NetworkBase.Status.ConnectionTerminated:
                    return "Connection has been terminated.";
                case NetworkBase.Status.DataReceived:
                    return "New Data is available to read.";
                case NetworkBase.Status.RemoteHostClosed:
                    return "The remote host has closed the connection.";

                // todo: only when debug enabled
                case NetworkBase.Status.SendFail:
                    return "Failed to send message.";
                case NetworkBase.Status.SendSuccess:
                    return "Successfully sent message.";
                default:
                    return "No Message available for " + status.ToString();
            }
        }
    }
}
