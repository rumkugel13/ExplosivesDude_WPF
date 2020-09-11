namespace ExplosivesDude
{
    public class Box : MapObject
    {
        public Box(int x, int y, int content) : base(x, y)
        {
            this.SetImageSource(Properties.Resources.Box);
            this.Content = content;
        }

        public int Content { get; private set; }

        public override int GetLookupId()
        {
            return ((int)Game.ClassType.Box * 1000000) + base.GetLookupId();
        }
    }
}
