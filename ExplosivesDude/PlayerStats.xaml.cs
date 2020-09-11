namespace ExplosivesDude
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaktionslogik für PlayerStats.xaml
    /// </summary>
    public partial class PlayerStats : UserControl, INotifyPropertyChanged
    {
        private bool isSelected;
        private Brush forecolor;

        public PlayerStats()
        {
            this.InitializeComponent();
            this.IsAvailable = true;
        }

        public event EventHandler<OnPlayerstatsSelectionChangedEventArgs> PlayerstatsSelectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public ImageSource PlayerImage
        {
            get { return im_player.Source; }
            set { im_player.Source = value; }
        }

        public int Id { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsSelectable { get; set; }

        public bool IsSelected
        {
            get => this.isSelected;

            set
            {
                this.isSelected = value;
                lb_status.Foreground = this.isSelected ? Brushes.LightGreen : Brushes.Black;
                ////this.ForeColor = this.isSelected ? Brushes.LightGreen : Brushes.Black;
            }
        }

        public Brush ForeColor
        {
            get => this.forecolor;

            set
            {
                this.forecolor = value;
                this.RaisePropertyChanged("ForeColor");
            }
        }

        public void Reset()
        {
            ////this.PlayerImage = UIManager.StringToSource("player" + this.ID + ".png");
            this.IsSelected = false;
            this.IsAvailable = true;
            ////this.Status = "Inactive";
            ////this.Speed = this.Amount = this.Range  = this.Power = this.Health = 0;
        }

        protected virtual void OnPlayerstatsSelectionChange(OnPlayerstatsSelectionChangedEventArgs e)
        {
            this.PlayerstatsSelectionChanged?.Invoke(this, e);
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsSelectable && (this.IsAvailable || this.IsSelected))
            {
                lb_status.Foreground = Brushes.Cyan;
                ////this.ForeColor = Brushes.Cyan;
            }
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.IsSelectable && (this.IsAvailable || this.IsSelected))
            {
                this.IsSelected = this.isSelected;
            }
        }

        private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsSelectable && (this.IsAvailable || this.IsSelected))
            {
                this.IsSelected = !this.IsSelected;
                this.OnPlayerstatsSelectionChange(new OnPlayerstatsSelectionChangedEventArgs(this.IsSelected));
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
