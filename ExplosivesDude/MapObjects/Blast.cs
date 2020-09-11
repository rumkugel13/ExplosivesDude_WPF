namespace ExplosivesDude
{
    using System;
    using System.Windows.Threading;

    public class Blast : MapObject
    {
        private readonly DispatcherTimer tim;

        public Blast(int x, int y, int damage) : base(x, y)
        {
            this.SetImageSource(Properties.Resources.Blast);
            this.Damage = damage;

            this.tim = new DispatcherTimer();
            this.tim.Tick += this.Tim_Tick;
            this.tim.Interval = TimeSpan.FromMilliseconds(750);
            this.tim.Start();
        }

        public event EventHandler<OnBlastFadedEventArgs> BlastFaded;

        public int Damage { get; }

        public override int GetLookupId()
        {
            return ((int)Game.ClassType.Blast * 1000000) + base.GetLookupId();
        }

        public void StopCountdown()
        {
            this.tim.Stop();
        }

        protected virtual void OnBlastFaded(OnBlastFadedEventArgs e)
        {
            this.BlastFaded?.Invoke(this, e);
        }

        private void Tim_Tick(object sender, EventArgs e)
        {
            this.tim.Stop();

            this.OnBlastFaded(new OnBlastFadedEventArgs(this.X, this.Y));
        }
    }
}
