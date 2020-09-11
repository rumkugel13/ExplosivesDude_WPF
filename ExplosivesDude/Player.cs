namespace ExplosivesDude
{
    using System;
    using System.ComponentModel;

    public class Player : MapObject, INotifyPropertyChanged
    {
        private readonly Animator animator;
        private readonly int startX, startY;
        private int duration, bombRange, bombPower, bombAmount, health, shield;
        private bool isReady, isDead;
        private PlayerStats stats = null;
        private bool remoteEnabled;

        public Player(int id, int x, int y, int z, PlayerStats stats) : base(x, y, z)
        {
            this.PlayerId = id;
            ////this.SetImageSource(UIManager.ImageToSource(UIManager.Images.Player, this.PlayerID));
            ////switch (this.PlayerID)
            ////{
            ////    case 0: this.SetImageSource(Properties.Resources.player0); break;
            ////    case 1: this.SetImageSource(Properties.Resources.player1); break;
            ////    case 2: this.SetImageSource(Properties.Resources.player2); break;
            ////    case 3: this.SetImageSource(Properties.Resources.player3); break;
            ////    default: this.SetImageSource(Properties.Resources.player0); break;
            ////}
            this.SetImageSource("player" + this.PlayerId + ".png");
            this.Image.DataContext = this;
            this.Image.SetBinding(System.Windows.Controls.ToolTipService.ToolTipProperty, new System.Windows.Data.Binding() { Source = this, Path = new System.Windows.PropertyPath("TooltipMessage") });
            
            this.SetPlayerStats(stats);
            this.IsDead = false;
            this.IsReady = false;
            this.startX = this.X = x;
            this.startY = this.Y = y;
            this.Delay = 300;
            this.BombRange = 1;
            this.BombAmount = 1;
            this.BombPower = 1;
            this.Health = 100;
            this.animator = new Animator();
            this.animator.AnimationCompleted += this.Animator_AnimationCompleted;
        }

        public event EventHandler TriggerActivated;

        public event EventHandler PlayerDied;

        public event EventHandler<OnMovementCompleteEventArgs> MovementCompleted;

        public event PropertyChangedEventHandler PropertyChanged;

        public enum Attributes
        {
            MoveDelay, BombRange, BombAmount, Health,
        }

        public int PlayerId { get; private set; }

        public new int X { get; set; }

        public new int Y { get; set; }

        public int Delay
        {
            get => this.duration;

            set
            {
                if (value > 145)
                {
                    this.duration = value;
                }

                this.RaisePropertyChanged("Speed");
            }
        }

        public string Speed => (1000.0 / this.duration).ToString("0.##");

        public int BombRange
        {
            get => this.bombRange;

            set
            {
                this.bombRange = value;
                this.RaisePropertyChanged("BombRange");
            }
        }

        public int BombPower
        {
            get => this.bombPower;

            set
            {
                this.bombPower = value;
                this.RaisePropertyChanged("BombPower");
            }
        }

        public int BombAmount
        {
            get => this.bombAmount;

            set
            {
                this.bombAmount = value;
                this.RaisePropertyChanged("BombAmount");
            }
        }

        public bool RemoteEnabled
        {
            get => this.remoteEnabled;

            set
            {
                this.remoteEnabled = value;
                ////if (this.stats != null)
                ////{
                ////    this.stats.RemoteEnabled = this.remoteEnabled;
                ////}
            }
        }

        public int Health
        {
            get => this.health;

            private set
            {
                this.health = value;
                this.RaisePropertyChanged("Health");
            }
        }

        public int Shield
        {
            get => this.shield;

            private set
            {
                this.shield = value;
                this.RaisePropertyChanged("Shield");
            }
        }

        public bool IsDead
        {
            get
            {
                return this.isDead;
            }

            set
            {
                this.isDead = value;
                if (value)
                {
                    ////this.SetImageSource(UIManager.ImageToSource(UIManager.Images.PlayerDead, this.PlayerID));
                    ////switch (this.PlayerID)
                    ////{
                    ////    case 0: this.SetImageSource(Properties.Resources.player0_dead); break;
                    ////    case 1: this.SetImageSource(Properties.Resources.player1_dead); break;
                    ////    case 2: this.SetImageSource(Properties.Resources.player2_dead); break;
                    ////    case 3: this.SetImageSource(Properties.Resources.player3_dead); break;
                    ////}
                    this.SetImageSource("player" + this.PlayerId + "_dead.png");
                }
            }
        }

        public bool IsReady
        {
            get
            {
                return this.isReady;
            }

            set
            {
                this.isReady = value;
            }
        }

        public string TooltipMessage
        {
            get
            {
                return "ID: " + this.PlayerId +
                    "\nX: " + this.X + " Y: " + this.Y +
                    "\nExplosives: " + this.BombAmount +
                    "\nRange: " + this.BombRange +
                    "\nPower: " + this.BombPower +
                    "\nSpeed: " + this.Speed +
                    "\nHealth: " + this.Health +
                    "\nShield: " + this.Shield;
            }
        }

        public bool MoveAllowed => !this.IsMoving && !this.IsDead;

        private bool IsMoving => this.animator.IsRunning;

        public override int GetLookupId()
        {
            return ((int)Game.ClassType.Player * 1000000) + (1000 * this.startX) + this.startY;
        }

        public override void Dispose()
        {
            this.ResetStats();
            base.Dispose();
        }

        public void DoDamage(int value)
        {
            if (value < this.Shield)
            {
                this.Shield -= value;
            }
            else
            {
                value -= this.Shield;
                this.Shield = 0;

                this.Health -= value;
                if (this.Health <= 0)
                {
                    this.Health = 0;
                    this.IsDead = true;
                    this.PlayerDied?.Invoke(this, null);
                }
            }
        }

        public void Heal(int value)
        {
            this.Health += value;
            if (this.Health > 100)
            {
                this.Health = 100;
            }
        }
               
        public void ChargeShield(int value)
        {
            this.Shield += value;
            if (this.Shield > 100)
            {
                this.Shield = 100;
            }
        }

        public void ResetStats()
        {
            if (this.stats != null)
            {
                this.stats.IsSelected = false;
                this.stats.IsAvailable = true;
                ////this.State = States.Absent;
                this.IsReady = false;
                this.BombRange = this.BombAmount = this.BombPower = this.Health = 0;
                this.Delay = int.MaxValue;
            }
        }

        public void MoveAnimation(int oldX, int oldY, int newX, int newY, int duration)
        {
            this.animator.MoveAnimation(this.Image, Game.BlockSize, oldX, oldY, newX, newY, duration);
        }

        public void TriggerExplosion()
        {
            if (this.RemoteEnabled)
            {
                this.OnTriggerActivate();
            }
        }

        protected virtual void OnMovementComplete(OnMovementCompleteEventArgs e)
        {
            this.MovementCompleted?.Invoke(this, e);
        }

        protected virtual void OnTriggerActivate()
        {
            this.TriggerActivated?.Invoke(this, null);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TooltipMessage"));
        }

        private void Animator_AnimationCompleted(object sender, OnAnimationCompletedEventArgs e)
        {
            int oldX = this.X;
            int oldY = this.Y;
            this.X = e.NewX;
            this.Y = e.NewY;

            this.OnMovementComplete(new OnMovementCompleteEventArgs(oldX, oldY, e.NewX, e.NewY));
        }

        private void SetPlayerStats(PlayerStats playerStats)
        {
            this.stats = playerStats;
            this.stats.DataContext = this;
            ////if (this.stats != null)
            ////{
            ////    this.stats.PlayerImage = this.Image.Source;
            ////}
        }
    }
}
