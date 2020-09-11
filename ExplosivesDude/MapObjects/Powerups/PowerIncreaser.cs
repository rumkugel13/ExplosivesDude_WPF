namespace ExplosivesDude
{
    public class PowerIncreaser : Powerup
    {
        public PowerIncreaser(int x, int y) : base(x, y)
        {
            this.SetImageSource(Properties.Resources.Power);
        }

        public override void UsePowerup(Player player)
        {
            player.BombPower++;
        }

        public override Type PowerupType()
        {
            return Type.PowerIncreaser;
        }
    }
}
