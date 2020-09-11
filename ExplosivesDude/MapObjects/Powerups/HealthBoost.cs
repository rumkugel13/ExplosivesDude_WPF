namespace ExplosivesDude
{
    public class HealthBoost : Powerup
    {
        public HealthBoost(int x, int y) : base(x, y)
        {
            this.SetImageSource(Properties.Resources.HealthKit);
        }

        public override void UsePowerup(Player player)
        {
            player.Heal(100);
        }

        public override Type PowerupType()
        {
            return Type.HealthBoost;
        }
    }
}
