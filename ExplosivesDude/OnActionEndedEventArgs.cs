namespace Bomberman_WPF
{
    using System;

    public class OnActionEndedEventArgs : EventArgs
    {
        public OnActionEndedEventArgs(MapObject replacement)
        {
            this.Replacement = replacement;
        }

        public MapObject Replacement { get; private set; }
    }
}