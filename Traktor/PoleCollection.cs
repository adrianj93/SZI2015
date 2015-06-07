using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traktor
{
    public class PoleCollection
    {
        public List<Pole> ListaPol { get; set; }

        public PoleCollection(int sizeX, int sizeY, int[,] tablicaTypow)
        {
            int k = 0;
            ListaPol = new List<Pole>();
            for (int i = 1; i <= sizeY; i++)
            {
                for (int j = 1; j <= sizeX; j++)
                {
                    //k = (i * size + j) % 7 + 1;
                    k = tablicaTypow[j, i];
                    Pole p = new Pole(j, i, k);
                    ListaPol.Add(p);
                }
            }
        }

        public Pole getPole(int x, int y)
        {
            Pole wynik;
            if (this.ListaPol.Exists(p => p.x == x && p.y == y))
                wynik = this.ListaPol.Find(p => p.x == x & p.y == y);
            else
                wynik = null;
            return wynik;
        }
    }

    //public class KolejkaPol
    //{
    //    public Pole aktualne { get; set; }
    //    public KolejkaPol next { get; set; }

    //    public KolejkaPol(Pole p)
    //    {
    //        this.aktualne = p;
    //        this.next = null;
    //    }

    //    public KolejkaPol(Pole p, KolejkaPol n)
    //    {
    //        this.aktualne = p;
    //        this.next = n;
    //    }
    //    public void DodajNaKoniec(Pole p)
    //    {
    //        KolejkaPol temp = this;
    //        while (temp.next != null)
    //            temp = temp.next;
    //        temp.next = new KolejkaPol(p);
    //    }
    //}
}
