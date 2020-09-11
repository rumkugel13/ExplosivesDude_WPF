namespace ExplosivesDude
{
    public class ExtraBomb : Powerup
    {
        public ExtraBomb(int x, int y) : base(x, y)
        {
            this.SetImageSource(Properties.Resources.ExtraBomb);
        }

        public override void UsePowerup(Player player)
        {
            player.BombAmount++;
        }

        public override Type PowerupType()
        {
            return Type.ExtraBomb;
        }
    }
}
