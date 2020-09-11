namespace ExplosivesDude
{
    public class SpeedBoost : Powerup
    {
        public SpeedBoost(int x, int y) : base(x, y)
        {
            this.SetImageSource(Properties.Resources.Shoe);
        }

        public override void UsePowerup(Player player)
        {
            player.Delay = (int)(player.Delay * 0.9);
        }

        public override Type PowerupType()
        {
            return Type.SpeedBoost;
        }
    }
}
