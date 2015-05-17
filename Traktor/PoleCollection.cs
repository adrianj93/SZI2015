using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traktor
{
    public class PoleCollection //private
    {
        //public List<List<Pole>> RowCollection { get; set; }
        public List<Pole> ListaPol { get; set; }

        public PoleCollection(int size)
        {
            int k;
            ListaPol = new List<Pole>();
            //this.RowCollection = new List<List<Pole>>();
            for (int i = 0; i < size; i++)
            {
                //List<Pole> columns = new List<Pole>();
                for (int j = 0; j < size; j++)
                {
                    k = (i * size + j) % 7 + 1;
                    Pole p = new Pole(j, i, k);
                    ListaPol.Add(p);
                    //columns.Add(p);
                }
                //this.RowCollection.Add(columns);
            }
        }

        public Pole getPole(int x, int y)
        {
            Pole wynik = this.ListaPol.Find(p => p.x == x & p.y == y);
            return wynik;
        }
    }
}
