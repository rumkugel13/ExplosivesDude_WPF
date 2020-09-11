namespace ExplosivesDude
{
    public class Point2D
    {
        public Point2D(int posX, int posY)
        {
            this.X = posX;
            this.Y = posY;
        }

        public int X { get; private set; }

        public int Y { get; private set; }
    }
}
