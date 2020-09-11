namespace ExplosivesDude
{
    using System;

    public class OnBombExplodedEventArgs : EventArgs
    {
        public OnBombExplodedEventArgs(Player player, int x, int y, int range, int power)
        {
            this.Owner = player;
            this.X = x;
            this.Y = y;
            this.Range = range;
            this.Power = power;
        }

        public Player Owner { get; private set; }

        public int Range { get; private set; }

        public int Power { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }
    }
}