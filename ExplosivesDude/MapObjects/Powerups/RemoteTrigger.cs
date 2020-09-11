namespace ExplosivesDude
{
    public class RemoteTrigger : Powerup
    {
        public RemoteTrigger(int x, int y) : base(x, y)
        {
            this.SetImageSource(Properties.Resources.Trigger);
        }

        public override void UsePowerup(Player player)
        {
            player.RemoteEnabled = true;
        }

        public override Type PowerupType()
        {
            return Type.RemoteTrigger;
        }
    }
}
