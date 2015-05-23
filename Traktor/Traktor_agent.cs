using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traktor.Resources;

namespace Traktor
{
	public class Traktor_agent
	{
		private int paliwo_poziom;
		private int paliwo_max;
        public Pole pozycja { get; set; }
        private List<Pole> plansza = new List<Pole>();

        //private int poz_x;
        //private int poz_y;

        //private bool available_left = false;
        //private bool available_right = false;
        //private bool available_forward = false;

        //private int passable;

		public Traktor_agent(int paliwo_poziom, int paliwo_max, PoleCollection plansza)
		{
			this.paliwo_poziom = paliwo_poziom;
			this.paliwo_max = paliwo_max;

            //this.poz_x = 0;
            //this.poz_y = 0;
            this.plansza = plansza.ListaPol;
            this.pozycja = plansza.getPole(0, 0);
		}

		public int getPaliwo()
		{
			return paliwo_poziom;
		}

        //public int getPosition_x()
        //{
        //    return poz_x;
        //}

        //public int getPosition_y()
        //{
        //    return poz_y;
        //}

        //public void setPosition(int x, int y)
        //{
        //    this.poz_x = x;
        //    this.poz_y = y;
        //}

        //public int detectNextBlock(int left, int right, int forward)
        //{
			
        //    return passable;
        //}

		public void tankuj(int paliwo)
		{
			paliwo_poziom += paliwo;
			if (paliwo_poziom > paliwo_max)
			{
				paliwo_poziom = paliwo_max;
			}
		}

        public Pole[] AGwiazdka (int x, int y)
        {
            Pole PoleX = new Pole(-1,-1,1000);
            Pole[] trasa = new Pole[100];
            int tentGScore;
            bool tentIsBetter;
            Pole[,] cameFrom = new Pole[10, 10];
            List<Pole> closedSet = new List<Pole>();
            List<Pole> openset = new List<Pole>();
            List<Pole> xNeighbor = new List<Pole>();
            openset.Add(this.pozycja);
            int[,] gScore = new int[10, 10];
            int[,] fScore = new int[10, 10];
            fScore[pozycja.x, pozycja.y] = 0;
            int[,] hScore = new int[10, 10];
            gScore[pozycja.x, pozycja.y] = 0;
            while (openset.Count != 0)
            {
                PoleX = openset.Find(a => a.koszt != null);
                foreach (var p in openset)
                {
                    if (fScore[p.x, p.y] < PoleX.koszt)
                        PoleX = p;
                }
                if (PoleX.x == x && PoleX.y == y)
                    trasa = reconstructPath(cameFrom, PoleX);
                openset.Remove(PoleX);
                if (!closedSet.Contains(PoleX))
                    closedSet.Add(PoleX);
                xNeighbor.RemoveAll(a => a.koszt != null);
                if (plansza.Find(a => a.x == (PoleX.x - 1) & a.y == PoleX.y) != null)
                    xNeighbor.Add(plansza.Find(a => a.x == (PoleX.x - 1) & a.y == PoleX.y));
                if (plansza.Find(a => a.x == (PoleX.x + 1) & a.y == PoleX.y) != null)
                    xNeighbor.Add(plansza.Find(a => a.x == (PoleX.x + 1) & a.y == PoleX.y));
                if (plansza.Find(a => a.x == PoleX.x & a.y == (PoleX.y - 1)) != null)
                    xNeighbor.Add(plansza.Find(a => a.x == PoleX.x & a.y == (PoleX.y - 1)));
                if (plansza.Find(a => a.x == PoleX.x & a.y == (PoleX.y + 1)) != null)
                    xNeighbor.Add(plansza.Find(a => a.x == PoleX.x & a.y == (PoleX.y + 1)));
                foreach (var p in xNeighbor)
                {
                    if (closedSet.Contains(p))
                        continue;
                    tentGScore = gScore[PoleX.x, PoleX.y] + p.koszt;
                    tentIsBetter = false;
                    if (!openset.Contains(p))
                    {
                        openset.Add(p);
                        hScore[p.x, p.y] = Math.Abs(p.x - x) + Math.Abs(p.y - y);
                        tentIsBetter = true;
                    }
                    else if (tentGScore < gScore[p.x, p.y])
                        tentIsBetter = true;
                    if (tentIsBetter)
                    {
                        cameFrom[p.x, p.y] = PoleX;
                        gScore[p.x, p.y] = tentGScore;
                        fScore[p.x, p.y] = gScore[p.x, p.y] + Math.Abs(p.x - x) + Math.Abs(p.y - y);
                    }
                }
            }
            return trasa;
            //Pole[] pusta = new Pole[1];
            //pusta[0] = null;
            //return pusta;
        }

        private Pole[] reconstructPath(Pole[,] cameFrom, Pole current)
        {
            if (cameFrom[current.x, current.y] != null)
            {
                Pole[] pTemp = reconstructPath(cameFrom, cameFrom[current.x, current.y]);
                Pole[] p = new Pole[pTemp.Length + 1];
                for (int i = 0; i < pTemp.Length + 1; i++)
                {
                    if (i < pTemp.Length)
                        p[i] = pTemp[i];
                    else
                        p[i] = current;
                }
                return p;
            }
            else
            {
                //return null;
                Pole[] pusta = new Pole[1];
                pusta[0] = null;
                return pusta;
            }
        }

        public void goTo (int x, int y)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
            {
                file.WriteLine("Log Trasy traktora:");
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
            {
                file.WriteLine("Pozycja startowa x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString());
            }

            Pole[] trasa = AGwiazdka(x, y);
            for(int i = 0; i<trasa.Length; i++)
            {
                //pozycja = trasa[i];
                if (trasa[i] == null)
                    continue;
                if (this.pozycja.x - 1 == trasa[i].x)
                { 
                    left();
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
                    { file.WriteLine("lewo " + "Pozycja x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString() + " Koszt: " + pozycja.koszt); }
                }
                else if (this.pozycja.x + 1 == trasa[i].x)
                { 
                    right();
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
                    { file.WriteLine("prawo " + "Pozycja x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString() + " Koszt: " + pozycja.koszt); }
                }
                else if (this.pozycja.y - 1 == trasa[i].y)
                { 
                    up();
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
                    { file.WriteLine("góra " + "Pozycja x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString() + " Koszt: " + pozycja.koszt); }
                }
                else if (this.pozycja.y + 1 == trasa[i].y)
                { 
                    down();
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
                    { file.WriteLine("dół " + "Pozycja x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString() + " Koszt: " + pozycja.koszt); }
                }
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
            {
                file.WriteLine("Pozycja końcowa x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString());
                file.WriteLine("***");
            }
        }

        private void left()
        {
            this.pozycja = this.plansza.Find(a => a.x == (this.pozycja.x - 1) & a.y == this.pozycja.y);
        }
        private void right()
        {
            this.pozycja = this.plansza.Find(a => a.x == (this.pozycja.x + 1) & a.y == this.pozycja.y);
        }
        private void up()
        {
            this.pozycja = this.plansza.Find(a => a.x == this.pozycja.x & a.y == (this.pozycja.y - 1));
        }
        private void down()
        {
            this.pozycja = this.plansza.Find(a => a.x == this.pozycja.x & a.y == (this.pozycja.y + 1));
        }
	}
}
