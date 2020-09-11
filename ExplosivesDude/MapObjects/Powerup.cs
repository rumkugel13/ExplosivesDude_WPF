namespace ExplosivesDude
{
    public abstract class Powerup : MapObject
    {
        protected Powerup(int x, int y) : base(x, y)
        {
        }

        public enum Type
        {
            ExtraBomb, HealthBoost, PowerIncreaser, RangeExtender, RemoteTrigger, ShieldBoost, SpeedBoost, None
        }

        public virtual void UsePowerup(Player player)
        {
        }

        public virtual Type PowerupType()
        {
            return Type.None;
        }

        public override int GetLookupId()
        {
            return ((int)Game.ClassType.Powerup * 1000000) + base.GetLookupId();
        }
    }
}
