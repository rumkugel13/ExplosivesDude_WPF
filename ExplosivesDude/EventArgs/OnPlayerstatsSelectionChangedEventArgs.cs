namespace ExplosivesDude
{
    using System;

    public class OnPlayerstatsSelectionChangedEventArgs : EventArgs
    {
        public OnPlayerstatsSelectionChangedEventArgs(bool selected)
        {
            this.Selected = selected;
        }

        public bool Selected { get; private set; }
    }
}