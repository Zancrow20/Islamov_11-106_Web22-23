namespace WinxGame.objects
{
    internal class Fairy : PictureBox
    {
        public int Hp { get; set; }

        public Fairy(Point location)
        {
            Location = location;
            Image = Image.FromFile(Path.Join(Directory.GetCurrentDirectory(), @"images/bloom.png"));
            Height = 100;
            Width = 100;
            SizeMode = PictureBoxSizeMode.StretchImage;
            Hp = 100;
        }

        public void Move(Keys key)
        {
            switch (key)
            {
                case Keys.Up:
                    Location = new Point(Location.X, Location.Y - 10);
                    break;
                case Keys.Down:
                    Location = new Point(Location.X, Location.Y + 10);
                    break;
                case Keys.Left:
                    Location = new Point(Location.X - 10, Location.Y);
                    break;
                case Keys.Right:
                    Location = new Point(Location.X + 10, Location.Y);
                    break;
            }
        }
    }
}
