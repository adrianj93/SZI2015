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
        //public int kierunekDo
        //{
        //    get
        //    {
        //        return this.kierunekDo;
        //    }
        //    set
        //    {
        //        if (value != 0 && value != 2 && value != 4 && value != 6 && value != 8)
        //            this.kierunekDo = 0;
        //        else
        //            this.kierunekDo = value;
        //    }
        //}
        public int kierunekDo { get; set; }

        public Pole(int x, int y, int koszt)
        {
            this.x = x;
            this.y = y;
            this.koszt = koszt;
            this.kierunekDo = 0;
        }

        //public bool isEqual(Pole p2)
        //{
        //    if (this.x == p2.x && this.y == p2.y && this.koszt == p2.koszt)
        //        return true;
        //    else
        //        return false;
        //}
    }
}
