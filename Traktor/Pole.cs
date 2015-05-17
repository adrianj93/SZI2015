using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traktor
{
    public class Pole
    {
        public int y { get; private set; }
        public int x { get; private set; }

        public int koszt { get; private set; }

        public Pole(int x, int y, int koszt)
        {
            this.x = x;
            this.y = y;
            this.koszt = koszt;
        }
    }
}
