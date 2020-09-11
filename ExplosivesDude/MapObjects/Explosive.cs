namespace ExplosivesDude
{
    using System;
    using System.Windows.Threading;

    public class Explosive : MapObject
    {
        private readonly int range, power;
        private readonly DispatcherTimer tim;
        private int seconds;
        private bool triggered;

        public Explosive(Player p) : this(p.X, p.Y, p.BombRange, p.BombPower, p)
        {
        }

        public Explosive(int x, int y, int range, int power, Player owner) : base(x, y)
        {
            this.range = range;
            this.power = power;
            this.Owner = owner;
            this.Owner.BombAmount--;

            this.SetImageSource(Properties.Resources.Bomb3);
            this.seconds = 3;
            this.triggered = false;

            owner.TriggerActivated += this.Owner_TriggerActivated;

            this.tim = new DispatcherTimer();
            this.StartCountdown(1000);
            this.tim.Tick += this.Tim_Tick;
        }

        public event EventHandler<OnBombExplodedEventArgs> BombExploded;

        private Player Owner { get; }

        public override int GetLookupId()
        {
            return ((int)Game.ClassType.Explosive * 1000000) + base.GetLookupId();
        }

        public void Trigger()
        {
            if (!this.triggered)
            {
                this.triggered = true;
                this.StartCountdown(100);
            }
        }
        
        public void ActivateRemote()
        {
            this.tim.Stop();
            this.SetImageSource(Properties.Resources.C4);
        }

        public void StartCountdown(int interval)
        {
            if (this.seconds > 0)
            {
                this.seconds--;
            }

            this.tim.Interval = TimeSpan.FromMilliseconds(interval);
            this.tim.Start();
        }

        private void Explode()
        {
            this.Owner.BombAmount++;
            this.BombExploded?.Invoke(this, new OnBombExplodedEventArgs(this.Owner, this.X, this.Y, this.range, this.power));
        }

        private void Owner_TriggerActivated(object sender, EventArgs e)
        {
            Player owner = (Player)sender;
            owner.TriggerActivated -= this.Owner_TriggerActivated;
            this.Explode();
        }

        private void Tim_Tick(object sender, EventArgs e)
        {
            switch (this.seconds)
            {
                case 0:
                    this.tim.Stop();
                    this.Owner.TriggerActivated -= this.Owner_TriggerActivated;
                    this.Explode();
                    break;
                case 1:
                    this.SetImageSource(Properties.Resources.Bomb1);
                    this.seconds--;
                    break;
                case 2:
                    this.SetImageSource(Properties.Resources.Bomb2);
                    this.seconds--;
                    break;
            }
        }
    }
}
