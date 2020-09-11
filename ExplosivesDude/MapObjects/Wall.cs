namespace ExplosivesDude
{
    public class Wall : MapObject
    {
        public Wall(int x, int y) : base(x, y)
        {
            this.SetImageSource(Properties.Resources.Wall);
        }

        public override int GetLookupId()
        {
            return ((int)Game.ClassType.Wall * 1000000) + base.GetLookupId();
        }
    }
}
