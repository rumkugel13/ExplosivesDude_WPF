namespace ExplosivesDude
{
    public class RangeExtender : Powerup
    {
        public RangeExtender(int x, int y) : base(x, y)
        {
            this.SetImageSource(Properties.Resources.Range);
        }

        public override void UsePowerup(Player player)
        {
            player.BombRange++;
        }

        public override Type PowerupType()
        {
            return Type.RangeExtender;
        }
    }
}
