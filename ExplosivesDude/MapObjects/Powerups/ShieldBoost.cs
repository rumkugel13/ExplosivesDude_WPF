namespace ExplosivesDude
{
    public class ShieldBoost : Powerup
    {
        public ShieldBoost(int x, int y) : base(x, y)
        {
            this.SetImageSource(Properties.Resources.Battery);
        }

        public override void UsePowerup(Player player)
        {
            player.ChargeShield(20);
        }

        public override Type PowerupType()
        {
            return Type.ShieldBoost;
        }
    }
}
