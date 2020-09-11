namespace ExplosivesDude
{
    using System;

    public class OnAnimationCompletedEventArgs : EventArgs
    {
        public OnAnimationCompletedEventArgs(int oldX, int oldY, int newX, int newY)
        {
            this.OldX = oldX;
            this.OldY = oldY;
            this.NewX = newX;
            this.NewY = newY;
        }

        public int NewX { get; private set; }

        public int NewY { get; private set; }

        public int OldX { get; private set; }

        public int OldY { get; private set; }
    }
}