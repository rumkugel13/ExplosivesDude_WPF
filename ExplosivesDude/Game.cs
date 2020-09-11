namespace ExplosivesDude
{
    using System;
    using System.Collections.Generic;
    using ExplosivesDude.Networking;

    public class Game
    {
        private readonly int gameSizeX, gameSizeY, baseDamage, minDamage;
        private readonly int maxRandom, wallResistance;
        private readonly int[] startPosX, startPosY;
        private readonly int[] playerNextDir;
        private readonly PlayerStats[] stats;
        private readonly Dictionary<int, MapObject> objects;
        private readonly Dictionary<int, Player> players;
        private readonly IUIOperationProvider uiManager;
        private readonly Dictionary<int, Player> clients;
        private NetworkClient networkClient;
        private int playerId, clientId, seed;
        private bool hasId;

        ////private bool countdownEnabled, continueWalk;

        public Game(int sizeX, int sizeY, int blockSize, PlayerStats[] stats, IUIOperationProvider ui)
        {
            this.gameSizeX = sizeX;
            this.gameSizeY = sizeY;
            BlockSize = blockSize;
            this.uiManager = ui;

            this.startPosX = new int[4] { 1, this.gameSizeX - 2, this.gameSizeX - 2, 1 };
            this.startPosY = new int[4] { 1, this.gameSizeY - 2, 1, this.gameSizeY - 2 };

            this.stats = stats;
            for (int i = 0; i < this.stats.Length; i++)
            {
                this.stats[i].Id = i;
                this.stats[i].PlayerstatsSelectionChanged += this.P_PlayerstatsSelectionChanged;
            }

            this.playerNextDir = new int[2];
            this.maxRandom = 110;
            this.wallResistance = 5;
            this.baseDamage = 100;
            this.minDamage = 34;
            this.IsRunning = false;
            this.PlayerId = 0;
            this.ClientId = 0;
            this.hasId = false;
            this.objects = new Dictionary<int, MapObject>();
            this.players = new Dictionary<int, Player>();
            this.clients = new Dictionary<int, Player>();
        }

        public enum Commands
        {
            AddPlayer, RemovePlayer, AddBoxes, AddExplosive, ChangeBombType, TriggerBomb, ChangeSeed,
            MovePlayer, RemoveClient, Message, ItemPickedUp, BlastHit, NewClient, PlayerDied, PlayerIsReady, 
            SelectPlayer, ChangeHealth, ChangeRange, ChangeSpeed, ChangeBombs, ChangeC4, ChangeDynamite, ChangeState,
            PlayerCount, ActivateRemote,
        }

        public enum ClassType
        {
            Floor, Wall, Powerup, Blast, Box, Explosive, Player
        }

        public static int BlockSize { get; private set; }

        public bool IsRunning { get; set; }

        public int PlayerCount => this.players.Count;

        public int Seed
        {
            get => this.seed;
            set
            {
                this.seed = value;
                Console.WriteLine("DEBUG: Changing Seed to " + value);
            }
        }

        public bool IsMultiplayer { get; set; }

        public int PlayerId
        {
            get => this.playerId;
            set
            {
                this.playerId = value;
                Console.WriteLine("DEBUG: Setting PlayerID to " + value);
            }
        }

        public int ClientId
        {
            get => this.clientId;
            set
            {
                this.clientId = value;
                Console.WriteLine("DEBUG: Setting ClientID to " + value);
                this.uiManager.UpdateClientId(value);
            }
        }

        public void CreateDefaultMap()
        {
            for (int i = 0; i < this.gameSizeX; i++)
            {
                for (int j = 0; j < this.gameSizeY; j++)
                {
                    if ((i == 0 || j == 0 || i == this.gameSizeX - 1 || j == this.gameSizeY - 1 || (i % 2 == 0 && j % 2 == 0))
                        && !(i > 0.33 * this.gameSizeX && i < 0.66 * this.gameSizeX && j > 0.33 * this.gameSizeY && j < 0.66 * this.gameSizeY))
                    {
                        this.AddObject(new Wall(i, j));
                    }
                }
            }
        }

        public void CreateNewMap()
        {
            // wip
            string[] map = new string[21];
            map[0] = "wwwwwwwwwwwwwwwwwwwwwwwwwwwww";
            map[1] = "wpp   w               w   ppw";
            map[2] = "wpwww w               w wwwpw";
            map[3] = "w w                       w w";
            map[4] = "w w w                   w w w";
            map[5] = "w                           w";
            map[6] = "www                       www";
            map[7] = "w                           w";
            map[8] = "w                           w";
            map[9] = "w                           w";
            map[10] = "w                           w";
            map[11] = "w                           w";
            map[12] = "w                           w";
            map[13] = "w                           w";
            map[14] = "www                       www";
            map[15] = "w                           w";
            map[16] = "w w w                   w w w";
            map[17] = "w w                       w w";
            map[18] = "wpwww w               w wwwpw";
            map[19] = "wpp   w               w   ppw";
            map[20] = "wwwwwwwwwwwwwwwwwwwwwwwwwwwww";

            for (int j = 0; j < this.gameSizeY; j++)
            {
                for (int i = 0; i < this.gameSizeX; i++)
                {
                    if (map[j].ToCharArray()[i] == 'w')
                    {
                        this.AddObject(new Wall(i, j));
                    }
                }
            }
        }

        public void ResetGame()
        {
            this.PlayerId = 0;
            this.hasId = false;

            // reset boxes etc
            foreach (MapObject m in this.players.Values)
            {
                m.Dispose();
            }

            this.players.Clear();

            foreach (MapObject m in this.objects.Values)
            {
                m.Dispose();
            }

            this.objects.Clear();

            foreach (PlayerStats p in this.stats)
            {
                p.Reset();
            }
        }

        public void AddBoxes()
        {
            this.AddBoxes(0);
        }

        public void AddBoxes(int seed)
        {
            Random rand;
            if (seed != 0)
            {
                this.Seed = seed;
                rand = new Random(seed);
                Console.WriteLine("DEBUG: Adding Boxes with seed " + seed);
            }
            else
            {
                rand = new Random();
                Console.WriteLine("DEBUG: Adding Boxes with random seed");
            }

            int i = 0;

            // set boxes on 30% of the map (approx 50% wall, so 20% floor left)
            // no boxes in 3x3 field of player spawnpoints allowed
            while (i < this.gameSizeX * this.gameSizeY * 0.3)
            {
                int x = rand.Next(this.gameSizeX);
                int y = rand.Next(this.gameSizeY);
                if (!this.objects.ContainsKey(this.LookupId(ClassType.Box, x, y)) && !this.objects.ContainsKey(this.LookupId(ClassType.Wall, x, y))
                    && !(x < 3 && y < 3) && !(x > this.gameSizeX - 4 && y < 3)
                    && !(x < 3 && y > this.gameSizeY - 4) && !(x > this.gameSizeX - 4 && y > this.gameSizeY - 4))
                {
                    this.AddObject(new Box(x, y, rand.Next(this.maxRandom)));
                    i++;
                }
            }
        }

        public void Player_AddExplosive(int playerId)
        {
            this.Player_AddExplosive(playerId, false);
        }

        public void Player_AddExplosive(int playerId, bool send)
        {
            if (this.players.TryGetValue(playerId, out Player p))
            {
                if (p != null && !p.IsDead && !this.objects.ContainsKey(this.LookupId(ClassType.Explosive, p.X, p.Y)) && p.BombAmount > 0)
                {
                    Explosive e = new Explosive(p);
                    e.BombExploded += this.B_BombExploded;
                    this.AddObject(e);

                    Console.WriteLine($"DEBUG: Player {playerId} drops Bomb at {p.X}|{p.Y} with range {p.BombRange} and power {p.BombPower}");

                    if (send)
                    {
                        this.ClientSend(new byte[]
                        {
                        (byte)Commands.AddExplosive, (byte)playerId, (byte)p.X, (byte)p.Y, (byte)p.BombRange, (byte)p.BombPower
                        });
                    }
                }
                else if (p != null && !p.IsDead && this.objects.ContainsKey(this.LookupId(ClassType.Explosive, p.X, p.Y)) && p.RemoteEnabled)
                {
                    this.Player_ActivateRemote(p.X, p.Y);
                    if (send)
                    {
                        this.ClientSend(new byte[]
                        {
                        (byte)Commands.ActivateRemote, (byte)p.X, (byte)p.Y,
                        });
                    }
                }
            }
        }

        public void Player_AddExplosive(int playerId, int posX, int posY, int range, int power)
        {
            if (this.players.TryGetValue(playerId, out Player p))
            {
                if (p != null)
                {
                    p.X = posX;
                    p.Y = posY;
                    p.BombRange = range;
                    p.BombPower = power;

                    Explosive e = new Explosive(p);
                    Console.WriteLine($"DEBUG: Player {playerId} drops Bomb at {posX}|{posY} with range {range} and power {power}");
                    e.BombExploded += this.B_BombExploded;
                    this.AddObject(e);
                }
            }
        }

        public void Player_ActivateRemote(int posX, int posY)
        {
            if (this.objects.ContainsKey(this.LookupId(ClassType.Explosive, posX, posY)))
            {
                ((Explosive)this.objects[this.LookupId(ClassType.Explosive, posX, posY)]).ActivateRemote();
            }
        }

        public void Player_Trigger(int playerId)
        {
            this.Player_Trigger(playerId, false);
        }

        public void Player_Trigger(int playerId, bool send)
        {
            if (this.players.TryGetValue(playerId, out Player p))
            {
                p.TriggerExplosion();
                Console.WriteLine("DEBUG: Player " + playerId + " triggered Bombs");

                if (send)
                {
                    this.ClientSend(new byte[]
                    {
                        (byte)Commands.TriggerBomb, (byte)playerId,
                    });
                }
            }
        }

        public void Player_Walk(int dx, int dy)
        {
            this.playerNextDir[0] = dx;
            this.playerNextDir[1] = dy;
            if (this.players.TryGetValue(this.PlayerId, out Player p))
            {
                int x = p.X;
                int y = p.Y;

                bool h = (x + dx >= 0) && (x + dx < this.gameSizeX);
                bool v = (y + dy >= 0) && (y + dy < this.gameSizeY);
                bool hasDirection = !(dx == 0 && dy == 0);
                bool noObstacle = 
                    !this.objects.ContainsKey(this.LookupId(ClassType.Wall, x + dx, y + dy)) 
                    && !this.objects.ContainsKey(this.LookupId(ClassType.Explosive, x + dx, y + dy)) 
                    && !this.objects.ContainsKey(this.LookupId(ClassType.Box, x + dx, y + dy));
                bool moveAllowed = h && v && hasDirection && noObstacle;

                if (p != null && p.MoveAllowed && moveAllowed)
                {
                    this.Player_Walk(this.PlayerId, x, y, x + dx, y + dy);
                    ////p.MoveAnimation(p.X, p.Y, newX, newY, p.Delay);
                    ////Console.WriteLine("Player " + this.playerID + " moves from " + p.X + "|" + p.Y + " to " + newX + "|" + newY);

                    ////if (id == this.PlayerID)
                    this.ClientSend(new byte[]
                    {
                    (byte)Commands.MovePlayer, (byte)this.PlayerId, (byte)x, (byte)y, (byte)(x + dx), (byte)(y + dy),
                    });
                }
            }
        }

        public void Player_Walk(int playerId, int currentX, int currentY, int newX, int newY)
        {
            ////this.players.Get(id).X = currentX;
            ////this.players.Get(id).Y = currentY;
            if (this.players.TryGetValue(playerId, out Player p))
            {
                p.MoveAnimation(currentX, currentY, newX, newY, p.Delay);
                Console.WriteLine("DEBUG: Player " + playerId + " moves from " + currentX + "|" + currentY + " to " + newX + "|" + newY);
            }
        }

        public void Player_StopWalking(int dx, int dy)
        {
            if (dx == this.playerNextDir[0] && dy == this.playerNextDir[1])
            {
                this.playerNextDir[0] = 0;
                this.playerNextDir[1] = 0;
            }
        }

        public void Player_ItemPickedUp(int playerId, int posX, int posY)
        {
            this.Player_ItemPickedUp(playerId, posX, posY, false);
        }

        public void Player_ItemPickedUp(int playerId, int posX, int posY, bool send)
        {
            if (this.objects.ContainsKey(this.LookupId(ClassType.Powerup, posX, posY)) && this.players.TryGetValue(playerId, out Player p))
            {
                ((Powerup)this.objects[this.LookupId(ClassType.Powerup, posX, posY)]).UsePowerup(p);
                Powerup.Type powerupType = ((Powerup)this.objects[this.LookupId(ClassType.Powerup, posX, posY)]).PowerupType();
                this.RemoveObject(this.LookupId(ClassType.Powerup, posX, posY));
                Console.WriteLine("DEBUG: Player " + playerId + " picked up " + powerupType + " at " + posX + "|" + posY);

                if (send)
                {
                    this.ClientSend(new byte[]
                    {
                        (byte)Commands.ItemPickedUp, (byte)playerId, (byte)posX, (byte)posY,
                    });
                }
            }
        }

        public void Player_BlastHit(int playerId, int damage)
        {
            this.Player_BlastHit(playerId, damage, false);
        }

        public void Player_BlastHit(int playerId, int damage, bool send)
        {
            if (this.players.TryGetValue(playerId, out Player p))
            {
                if (p != null)
                {
                    Console.WriteLine("DEBUG: Player " + playerId + " was hit by blastwave and was dealt " + damage + " damage");
                    p.DoDamage(damage);

                    if (send)
                    {
                        this.ClientSend(new byte[]
                        {
                        (byte)Commands.BlastHit, (byte)playerId, (byte)damage,
                        });
                    }
                }
            }
        }

        public void Player_Died(int playerId)
        {
            this.Player_Died(playerId, false);
        }

        public void Player_Died(int playerId, bool send)
        {
            if (this.players.TryGetValue(playerId, out Player p))
            {
                p.IsDead = true;
                Console.WriteLine("DEBUG: Player " + playerId + " died");

                if (send)
                {
                    this.ClientSend(new byte[]
                    {
                        (byte)Commands.PlayerDied, (byte)playerId, 
                    });
                }
            }
        }

        public void Player_IsReady(int playerId, bool isReady)
        {
            this.Player_IsReady(playerId, isReady, false);
        }

        public void Player_IsReady(int playerId, bool isReady, bool send)
        {
            if (this.players.TryGetValue(playerId, out Player p))
            {
                p.IsReady = isReady;
                Console.WriteLine("DEBUG: Player " + playerId + " is " + (isReady ? "" : "not") + " ready");
                
                if (this.CheckPlayerReady())
                {
                    this.uiManager.StartCountdown(3);
                }

                if (send)
                {
                    this.ClientSend(new byte[]
                    {
                        (byte)Commands.PlayerIsReady, (byte)playerId, (byte)(isReady ? 1 : 0), 
                    });
                }
            }
        }

        //public void Player_ChangeState(int playerId, Player.States state)
        //{
        //    this.Player_ChangeState(playerId, state, false);
        //}

        //public void Player_ChangeState(int playerId, Player.States state, bool send)
        //{
        //    if (this.players.TryGetValue(playerId, out Player p))
        //    {
        //        p.State = state;
        //        Console.WriteLine("DEBUG: Player " + playerId + " changed state to " + state.ToString());

        //        switch (state)
        //        {
        //            case Player.States.Ready:
        //                if (this.CheckPlayerReady())
        //                {
        //                    this.uiManager.StartCountdown(3);
        //                }

        //                break;
        //            case Player.States.Waiting:
        //                this.uiManager.StopCountdown();
        //                break;
        //            case Player.States.Absent:
        //                this.uiManager.StopCountdown();
        //                break;
        //        }

        //        if (send)
        //        {
        //            this.ClientSend(new byte[]
        //            {
        //                (byte)Commands.ChangeState, (byte)playerId, (byte)state,
        //            });
        //        }
        //    }
        //}

        public void AddPlayer(int clientId, int playerId, bool send)
        {
            if (!this.players.ContainsKey(playerId))
            {
                Player p = this.NewPlayer(playerId, this.startPosX[playerId], this.startPosY[playerId], this.stats[playerId]);
                if (this.clients.ContainsKey(clientId))
                {
                    this.clients[clientId] = p;
                }
                else
                {
                    this.clients.Add(clientId, p);
                }

                Console.WriteLine("DEBUG: Client " + clientId + " now controls player with Id " + playerId);
                this.uiManager.UpdatePlayerCount(this.PlayerCount);
                if (send)
                {
                    this.ClientSend(Common.AppendInt(new byte[] { (byte)Game.Commands.AddPlayer, (byte)playerId, }, clientId));
                }
            }
        }

        public void RemovePlayer(int playerId, bool send)
        {
            if (this.players.TryGetValue(playerId, out Player p))
            {
                p.Dispose();
                this.players.Remove(playerId);
                Console.WriteLine("DEBUG: Removing player " + playerId);
                this.uiManager.UpdatePlayerCount(this.PlayerCount);
                if (send)
                {
                    this.ClientSend(new byte[] 
                    {
                        (byte)Game.Commands.RemovePlayer, (byte)playerId,
                    });
                }
            }
        }

        public void AddClient(int clientId)
        {
            this.AddClient(clientId, false);
        }

        public void AddClient(int clientId, bool send)
        {
            if (!this.clients.ContainsKey(clientId))
            {
                this.clients.Add(clientId, null);
                Console.WriteLine("DEBUG: Adding new client with id " + clientId);
                if (send)
                {
                    // todo: send direct message to new client // send this clients id to new client
                    this.ClientSend(Common.AppendInt(new byte[] { (byte)Game.Commands.NewClient, }, this.ClientId));
                    if (this.hasId)
                    {
                        // wip: send if player is ready
                        this.ClientSend(Common.AppendInt(new byte[] { (byte)Game.Commands.AddPlayer, (byte)this.PlayerId, }, this.ClientId)); //// (byte)this.players.Get(this.PlayerID).State });
                    }
                }
            }

            this.uiManager.UpdateClientCount(this.clients.Count);
        }

        public void RemoveClient(int clientId)
        {
            // find client in client list and mark as absent (removes player etc)
            if (this.clients.ContainsKey(clientId))
            {
                if (this.clients[clientId] != null)
                {
                    //this.Player_ChangeState(this.clients[clientId].PlayerId, Player.States.Absent);
                    this.uiManager.StopCountdown();
                    this.RemovePlayer(this.clients[clientId].PlayerId, false);
                }
                ////this.RemovePlayer(this.clients.Get(clientId), false); -> check if client even has an id (how?)
                this.clients.Remove(clientId);
                Console.WriteLine("DEBUG: Removing client with id " + clientId);
            }

            this.uiManager.UpdateClientCount(this.clients.Count);
        }

        public void SetStatsSelectable(bool selectable)
        {
            foreach (PlayerStats p in this.stats)
            {
                p.IsSelectable = selectable;
            }
        }

        public void StartGame()
        {
            this.IsRunning = true;
            this.SetStatsSelectable(false);
        }

        public void StopGame()
        {
            this.IsRunning = false;
            this.uiManager.StopCountdown();
            this.SetStatsSelectable(false);
            if (this.IsMultiplayer)
            {
                this.ClientDisconnect();
                this.IsMultiplayer = false;
            }
        }

        public void ClientConnect(string address, int port)
        {
            this.IsMultiplayer = true;
            this.networkClient = new NetworkClient();
            this.networkClient.Connect(address, port);
            this.networkClient.ConnectionChanged += this.NetworkClient_OnConnectionChange;
            this.networkClient.DataReceived += this.NetworkClient_OnDataReceived;
        }

        public void ClientDisconnect()
        {
            if (this.networkClient != null)
            {
                this.networkClient.Disconnect();
            }
        }

        public void ClientSend(byte[] data)
        {
            if (data != null && this.networkClient != null && this.networkClient.Connected)
            {
                this.networkClient.SendMessage(data);
            }
        }

        public void ProcessNetworkData(Commands command, byte[] data)
        {
            switch (command)
            {
                case Commands.AddExplosive:
                    this.Player_AddExplosive(data[1], data[2], data[3], data[4], data[5]);
                    break;

                case Commands.ActivateRemote:
                    this.Player_ActivateRemote(data[1], data[2]);
                    break;

                case Commands.TriggerBomb:
                    this.Player_Trigger(data[1]);
                    break;

                case Commands.MovePlayer:
                    this.Player_Walk(data[1], data[2], data[3], data[4], data[5]);
                    break;

                case Commands.ItemPickedUp:
                    this.Player_ItemPickedUp(data[1], data[2], data[3]);
                    break;

                case Commands.BlastHit:
                    this.Player_BlastHit(data[1], data[2]);
                    break;

                case Commands.PlayerDied:
                    this.Player_Died(data[1]);
                    break;

                case Commands.PlayerIsReady:
                    this.Player_IsReady(data[1], data[2] == 1);
                    break;

                case Commands.AddPlayer:
                    this.AddPlayer(BitConverter.ToInt32(data, 2), data[1], false);
                    break;

                case Commands.RemovePlayer:
                    this.RemovePlayer(data[1], false);
                    break;

                case Commands.AddBoxes:
                    this.AddBoxes(BitConverter.ToInt32(data, 1));
                    break;

                case Commands.ChangeSeed:
                    this.Seed = BitConverter.ToInt32(data, 1);
                    break;

                case Commands.NewClient:
                    this.AddClient(BitConverter.ToInt32(data, 1), true);
                    break;

                case Commands.RemoveClient:
                    this.RemoveClient(BitConverter.ToInt32(data, 1));
                    break;

                case Commands.PlayerCount:
                    this.uiManager.WriteConnectionStatus(data[1] + " Players Connected", true);
                    break;

                default:
                    Console.WriteLine("ERROR: Unknown command: " + command.ToString());
                    break;
            }
        }

        private Player NewPlayer(int id, int posX, int posY, PlayerStats stats = null)
        {
            Player p = new Player(id, posX, posY, 10, stats);
            p.MovementCompleted += this.Player_MovementComplete;
            p.PlayerDied += this.Player_Died;
            this.players.Add(p.PlayerId, p);

            if (stats != null)
            {
                stats.IsAvailable = false;
                if (stats.Id == this.PlayerId)
                {
                    stats.IsSelected = true;
                }
            }

            return p;
        }

        private Powerup NewPowerup(int posX, int posY, int rand)
        {
            Powerup pow = null;
            if (rand < 10)
            {
                pow = new SpeedBoost(posX, posY);
            }
            else if (rand < 19)
            {
                pow = new RangeExtender(posX, posY);
            }
            else if (rand < 28)
            {
                pow = new ExtraBomb(posX, posY);
            }
            else if (rand < 33)
            {
                pow = new HealthBoost(posX, posY);
            }
            else if (rand < 39)
            {
                pow = new ShieldBoost(posX, posY);
            }
            ////else if (rand < 45)
            ////{
            ////    pow = new PowerIncreaser(x, y);
            ////}
            ////else if (rand < 52)
            ////{
            ////    pow = new RemoteTrigger(x, y);
            ////}

            if (pow != null)
            {
                this.AddObject(pow);
            }

            return pow;
        }

        private void BlastWave(int posX, int posY, int dx, int dy, int range, int damage, int ddamage, int power = 0)
        {
            if (range < 0)
            {
                return;
            }

            if (power > this.wallResistance || !this.objects.ContainsKey(this.LookupId(ClassType.Wall, posX, posY)))
            {
                this.AddBlast(posX, posY, damage);
                this.CheckPlayerHit(posX, posY, damage);
                this.RemoveObject(this.LookupId(ClassType.Powerup, posX, posY));

                // chain reaction
                if (this.objects.ContainsKey(this.LookupId(ClassType.Explosive, posX, posY)))
                {
                    ((Explosive)this.objects[this.LookupId(ClassType.Explosive, posX, posY)]).Trigger();
                }

                if (power > this.wallResistance && this.objects.ContainsKey(this.LookupId(ClassType.Wall, posX, posY)))
                {
                    this.RemoveObject(this.LookupId(ClassType.Wall, posX, posY));
                }
                else if (!(dx == 0 && dy == 0) && !this.objects.ContainsKey(this.LookupId(ClassType.Box, posX, posY)))
                {
                    // continue blastwave
                    this.BlastWave(posX + dx, posY + dy, dx, dy, range - 1, damage - ddamage, ddamage, power);
                }
            }
        }

        private void AddBlast(int posX, int posY, int damage)
        {
            Blast b = new Blast(posX, posY, damage);
            if (this.objects.ContainsKey(b.GetLookupId()))
            {
                ((Blast)this.objects[b.GetLookupId()]).StopCountdown();
                this.RemoveObject(b.GetLookupId());
            }

            this.AddObject(b);
            b.BlastFaded += this.B_BlastFaded;
        }

        private void CheckPlayerHit(int posX, int posY, int damage)
        {
            if (this.players.TryGetValue(this.PlayerId, out Player p))
            {
                if (p != null && p.X == posX && p.Y == posY && !p.IsDead)
                {
                    if (p.PlayerId == this.PlayerId)
                    {
                        this.Player_BlastHit(p.PlayerId, damage, true);
                    }
                }

                if (CheckAllPlayersDead())
                {
                    // this.GameOver();
                }
            }
        }

        private bool CheckAllPlayersDead()
        {
            foreach (Player pl in this.players.Values)
            {
                if (!pl.IsDead)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckAllPlayersReady()
        {
            foreach (Player pl in this.players.Values)
            {
                if (!pl.IsReady)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckPlayerReady()
        {
            if (this.PlayerCount > 1 || (this.PlayerCount == 1 && !this.IsMultiplayer))
            {
                return CheckAllPlayersReady();
            }

            return false;
        }

        private bool ObjectExists(int id)
        {
            return this.objects.ContainsKey(id);
        }

        private void AddObject(MapObject m)
        {
            if (m != null)
            {
                this.objects.Add(m.GetLookupId(), m);
            }
        }

        private void RemoveObject(int id)
        {
            if (this.objects.ContainsKey(id))
            {
                this.objects[id].Dispose();
                this.objects.Remove(id);
            }
        }

        private int LookupId(ClassType type, int x, int y)
        {
            ////return (int)(x << 32) | (int)(y);
            return (1000000 * (int)type) + (1000 * x) + y;
        }

        private void B_BlastFaded(object sender, OnBlastFadedEventArgs e)
        {
            if (this.objects.ContainsKey(this.LookupId(ClassType.Box, e.X, e.Y)))
            {
                this.NewPowerup(e.X, e.Y, ((Box)this.objects[this.LookupId(ClassType.Box, e.X, e.Y)]).Content);
                this.RemoveObject(this.LookupId(ClassType.Box, e.X, e.Y));
            }

            this.RemoveObject(this.LookupId(ClassType.Blast, e.X, e.Y));
        }

        private void B_BombExploded(object sender, OnBombExplodedEventArgs e)
        {
            this.RemoveObject(this.LookupId(ClassType.Explosive, e.X, e.Y));
            int[] dx = new int[4] { +1, -1, 0, 0 };
            int[] dy = new int[4] { 0, 0, +1, -1 };

            int damageDelta = e.Range > 1 ? (this.baseDamage - this.minDamage) / (e.Range - 1) : 0;
            this.BlastWave(e.X, e.Y, 0, 0, e.Range, this.baseDamage, damageDelta, e.Power);
            for (int i = 0; i < dx.Length; i++)
            {
                this.BlastWave(e.X + dx[i], e.Y + dy[i], dx[i], dy[i], e.Range - 1, this.baseDamage, damageDelta, e.Power);
            }
            ////this.BlastWave(e.X + 1, e.Y, +1, 0, e.Range - 1, e.Power);
            ////this.BlastWave(e.X - 1, e.Y, -1, 0, e.Range - 1, e.Power);
            ////this.BlastWave(e.X, e.Y + 1, 0, +1, e.Range - 1, e.Power);
            ////this.BlastWave(e.X, e.Y - 1, 0, -1, e.Range - 1, e.Power);
        }

        private void P_PlayerstatsSelectionChanged(object sender, OnPlayerstatsSelectionChangedEventArgs e)
        {
            PlayerStats ps = (PlayerStats)sender;
            if (e.Selected)
            {
                if (this.hasId)
                {
                    this.RemovePlayer(this.clients[this.ClientId].PlayerId, true);
                }

                this.hasId = true;
                this.PlayerId = ps.Id;
                ////this.Client_ChangePlayerID(this.PlayerId, ps.Id);
                this.AddPlayer(this.ClientId, ps.Id, true);
                //this.Player_ChangeState(ps.Id, Player.States.Ready, true);
                this.Player_IsReady(ps.Id, true);
            }
            else
            {
                // unselect, change player id to not defined
                //this.Player_ChangeState(ps.Id, Player.States.Absent, true);
                this.uiManager.StopCountdown();
                this.RemovePlayer(ps.Id, true);
                this.hasId = false;
            }
        }

        private void Player_MovementComplete(object sender, OnMovementCompleteEventArgs e)
        {
            Player p = (Player)sender;

            if (p.PlayerId == this.PlayerId)
            {
                // if player walks into blastwave
                if (this.objects.ContainsKey(this.LookupId(ClassType.Blast, e.NewX, e.NewY)))
                {
                    this.CheckPlayerHit(e.NewX, e.NewY, ((Blast)this.objects[this.LookupId(ClassType.Blast, e.NewX, e.NewY)]).Damage);
                }

                // continue walking in multiplayer? until stopwalking?
                this.Player_Walk(this.playerNextDir[0], this.playerNextDir[1]);
                this.Player_ItemPickedUp(p.PlayerId, e.NewX, e.NewY, true);
            }
        }

        private void Player_Died(object sender, EventArgs e)
        {
            Player p = (Player)sender;
            this.Player_Died(p.PlayerId);
        }

        private void NetworkClient_OnDataReceived(object sender, OnDataReceivedEventArgs e)
        {
            this.ProcessNetworkData((Game.Commands)e.Data[0], e.Data);
        }

        private void NetworkClient_OnConnectionChange(object sender, OnConnectionChangedEventArgs e)
        {
            this.uiManager.WriteConnectionStatus(e.Connected ? "Connected" : "Not Connected", false);

            if (e.Connected)
            {
                this.CreateDefaultMap();
                this.SetStatsSelectable(true);
                this.ClientId = e.ClientId;
                this.AddClient(e.ClientId);
            }
            else
            {
                this.IsRunning = false;
                this.SetStatsSelectable(false);
                this.RemoveClient(e.ClientId);
                ////client.StopClient();
                ////this.networkClient = null;
                ////this.ClientDisconnect();
            }
        }
    }
}
