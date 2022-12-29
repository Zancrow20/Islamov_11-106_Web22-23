using Timer = System.Threading.Timer;

namespace WinxGame.objects
{
    internal class Spider : PictureBox
    {        
        private TimerCallback _callback;

        public int Hp { get; set; }

        private readonly List<FireBall> _fireBalls;
        private readonly Form _form;
        private readonly Random _random = new();

        public int CreateFireBallTime { get; }

        public Spider(Point location, Form form)
        {
            Location = location;
            Image = Image.FromFile(Path.Join(Directory.GetCurrentDirectory(), @"images/Skullback Spider.png"));
            Height = 50;
            Width = 50;
            SizeMode = PictureBoxSizeMode.StretchImage;
            Hp = 10;

            _form = form;
            _fireBalls = new List<FireBall>();

            CreateFireBallTime = _random.Next(5000, 15000);

            _callback = new TimerCallback(Redraw);
            var timer = new Timer(_callback, null, CreateFireBallTime, CreateFireBallTime);
        }

        public void CreateFireBall()
        {
            var fireBall = new FireBall(Location, _form);

            _form.BeginInvoke(new Action(() =>
            {
                _form.Controls.Add(fireBall);
            }));

            _fireBalls.Add(fireBall);
        }

        public void Redraw(object obj)
        {  // if (time % CreateFireBallTime == 0)
            CreateFireBall();
        }
    }
}
