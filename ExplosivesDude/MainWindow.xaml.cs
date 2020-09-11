namespace ExplosivesDude
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUIOperationProvider
    {
        private static Grid gameGrid;
        private Game game;
        private GameServer server;
        private string serverHost;
        private DispatcherTimer tim;
        private int countdown;

        public MainWindow()
        {
            this.InitializeComponent();
            gameGrid = this.grid_game;
            this.game = new Game(29, 21, 24, new PlayerStats[4] { this.stats0, this.stats1, this.stats2, this.stats3 }, this);

            this.server = new GameServer();
            this.lb_version.Content = this.Version;
        }

        public string Version
        {
            get { return FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion; }
        }

        public static void AddImage(Image image)
        {
            gameGrid.Children.Add(image);
        }

        public static void RemoveImage(Image image)
        {
            gameGrid.Children.Remove(image);
        }

        public void WriteConnectionStatus(string status, bool playerCount)
        {
            if (playerCount)
            {
                lb_player_count.Content = status;
            }
            else
            {
                lb_client_conn.Content = status;
                if ("Connected".Equals(status))
                {
                    bt_connect.Content = "Disconnect";
                }
            }
        }

        public void UpdateClientCount(int count)
        {
            this.lb_clients.Content = "Clients: " + count;
        }

        public void UpdatePlayerCount(int count)
        {
            this.lb_players.Content = "Players: " + count;
        }

        public void UpdateClientId(int id)
        {
            lb_clientId.Content = id.ToString();
        }

        public void StartCountdown(int seconds)
        {
            this.StopCountdown();
            if (this.tim == null || !this.tim.IsEnabled)
            {
                this.countdown = seconds;
                this.tim = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                this.tim.Tick += this.Tim_Tick;
                this.tim.Start();
            }
        }

        public void StopCountdown()
        {
            if (this.tim != null && this.tim.IsEnabled)
            {
                this.tim.Stop();
                lb_countdown.Content = "Countdown: " + 3;
            }
        }

        private void Tim_Tick(object sender, EventArgs e)
        {
            this.countdown--;
            switch (this.countdown)
            {
                case 0:
                    lb_countdown.Content = "Countdown: " + "Go!";
                    this.game.StartGame();
                    this.tim.Stop();
                    break;
                default:
                    lb_countdown.Content = "Countdown: " + this.countdown;
                    break;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.game.IsRunning)
            {
                switch (e.Key)
                {
                    case Key.Up:
                    case Key.W:
                        ////this.game.Player_Walk(Game.Direction.Up);
                        this.game.Player_Walk(0, -1);
                        break;
                    case Key.Down:
                    case Key.S:
                        ////this.game.Player_Walk(Game.Direction.Down);
                        this.game.Player_Walk(0, +1);
                        break;
                    case Key.Right:
                    case Key.D:
                        ////this.game.Player_Walk(Game.Direction.Right);
                        this.game.Player_Walk(+1, 0);
                        break;
                    case Key.Left:
                    case Key.A:
                        ////this.game.Player_Walk(Game.Direction.Left);
                        this.game.Player_Walk(-1, 0);
                        break;
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.game.IsRunning)
            {
                switch (e.Key)
                {
                    case Key.Up:
                    case Key.W:
                        ////this.game.Player_StopWalking(Game.Direction.Up);
                        this.game.Player_StopWalking(0, -1);
                        break;
                    case Key.Down:
                    case Key.S:
                        ////this.game.Player_StopWalking(Game.Direction.Down);
                        this.game.Player_StopWalking(0, +1);
                        break;
                    case Key.Right:
                    case Key.D:
                        ////this.game.Player_StopWalking(Game.Direction.Right);
                        this.game.Player_StopWalking(+1, 0);
                        break;
                    case Key.Left:
                    case Key.A:
                        ////this.game.Player_StopWalking(Game.Direction.Left);
                        this.game.Player_StopWalking(-1, 0);
                        break;

                    case Key.Space:
                        this.game.Player_AddExplosive(this.game.PlayerId, true);
                        break;

                    case Key.Enter:
                        this.game.Player_Trigger(this.game.PlayerId, true);
                        break;
                }
            }
        }

        private void Bt_single_Click(object sender, RoutedEventArgs e)
        {
            if (bt_single.Content.Equals("Singleplayer"))
            {
                this.game.ResetGame();
                this.game.CreateDefaultMap();
                ////this.game.CreateNewMap();
                this.game.AddBoxes();
                this.game.SetStatsSelectable(true);
                bt_single.Content = "Stop";
                bt_connect.IsEnabled = false;
                cb_server.IsEnabled = false;
                this.SetFocusable(false);
            }
            else
            {
                this.game.StopGame();
                bt_single.Content = "Singleplayer";
                bt_connect.IsEnabled = true;
                cb_server.IsEnabled = true;
                this.SetFocusable(true);
            }
        }

        private void Bt_connect_Click(object sender, RoutedEventArgs e)
        {
            if (bt_connect.Content.Equals("Connect"))
            {
                this.game.ResetGame();
                this.game.ClientConnect(this.serverHost, Properties.Settings.Default.ServerPort);
                bt_connect.Content = "Stop Conn.";
                bt_single.IsEnabled = false;
                this.SetFocusable(false);
            }
            else
            {
                this.game.StopGame();
                bt_connect.Content = "Connect";
                bt_single.IsEnabled = true;
                this.SetFocusable(true);
            }
        }

        private void Bt_server_Click(object sender, RoutedEventArgs e)
        {
            if (bt_server.Content.Equals("Start Server"))
            {
                this.server.StartServer();
                this.server.NewGame();
                bt_server.Content = "Stop Server";
            }
            else
            {
                this.server.StopServer();
                bt_server.Content = "Start Server";
            }
        }

        private void SetFocusable(bool state)
        {
            bt_single.Focusable = state;
            bt_connect.Focusable = state;
            bt_server.Focusable = state;
            cb_server.Focusable = state;
        }

        private void Cb_server_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cb_server.SelectedIndex)
            {
                case 0:
                    this.serverHost = Properties.Settings.Default.ServerMain;
                    break;
                case 1:
                    this.serverHost = Properties.Settings.Default.ServerBackup;
                    break;
                case 2:
                    this.serverHost = Properties.Settings.Default.ServerLocal;
                    break;
            }
        }
    }
}