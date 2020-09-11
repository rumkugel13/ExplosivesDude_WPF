namespace ExplosivesDude
{
    public class Floor : MapObject
    {
        public Floor(int x, int y) : base(x, y)
        {
            this.SetImageSource(Properties.Resources.Floor);
        }
    }
}
