using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ExifInfo.Models
{
    public class Staf
    {
        public UIElement Point { set; get; }

        public double X { set; get; }

        public double Y { set; get; }

        public double Vx { set; get; }

        public double Vy { set; get; }

        public void RandomStaf(Random ran)
        {
            var staf = this;
            _ran = ran;
            staf.Vx = (double)ran.Next(-1000, 1000) / 1000;
            staf.Vy = (double)ran.Next(-1000, 1000) / 1000;
            staf.Time = ran.Next(100);
        }
        private Random _ran;
        public int Time
        {
            set
            {
                _time = value;
                if (value == 0)
                {
                    RandomStaf(_ran);
                }
            }
            get
            {
                return _time;
            }
        }

        private int _time;
    }
}
