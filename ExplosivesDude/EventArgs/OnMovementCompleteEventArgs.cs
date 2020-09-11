namespace ExplosivesDude
{
    using System;

    public class OnMovementCompleteEventArgs : EventArgs
    {
        public OnMovementCompleteEventArgs(int oldX, int oldY, int newX, int newY)
        {
            this.OldX = oldX;
            this.OldY = oldY;
            this.NewX = newX;
            this.NewY = newY;
        }

        public int OldX { get; private set; }

        public int OldY { get; private set; }

        public int NewX { get; private set; }

        public int NewY { get; private set; }
    }
}