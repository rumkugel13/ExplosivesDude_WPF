namespace ExplosivesDude
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;

    public abstract class MapObject : IDisposable
    {
        protected MapObject(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Image = this.CreateImage(x, y, z);
        }

        protected MapObject(int x, int y) : this(x, y, 0)
        {
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        public System.Windows.Controls.Image Image { get; private set; }

        public virtual void Dispose()
        {
            if (this.Image != null)
            {
                MainWindow.RemoveImage(this.Image);
            }
        }

        public virtual int GetLookupId()
        {
            return (this.X * 1000) + this.Y;
        }

        protected void SetImageSource(string source)
        {
            if (this.Image != null)
            {
                this.Image.Source = this.StringToSource(source);
            }
        }

        private System.Windows.Media.ImageSource StringToSource(string source)
        {
            if (source == null)
            {
                source = Properties.Resources.Background;
            }

            try
            {
                return new BitmapImage(new Uri("/Resources/" + source, UriKind.Relative));
            }
            catch (System.IO.FileNotFoundException)
            {
                return new BitmapImage(new Uri("/Resources/" + Properties.Resources.Nothing, UriKind.Relative));
            }
        }

        private Image CreateImage(int x, int y, int z = 0, string ims = "background_32.png")
        {
            Image im = new Image()
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                Height = Game.BlockSize,
                Width = Game.BlockSize,
                Source = new BitmapImage(new Uri("/Resources/" + ims, UriKind.Relative)),
                Margin = new System.Windows.Thickness(x * Game.BlockSize, y * Game.BlockSize, 0, 0)
            };
            Panel.SetZIndex(im, z);
            MainWindow.AddImage(im);
            return im;
        }
    }
}
