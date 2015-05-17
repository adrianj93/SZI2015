using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traktor.Resources;

namespace Traktor
{
	class Traktor_agent
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
            int tentGScore;
            bool tentIsBetter;
            Pole[,] cameFrom = new Pole[10, 10];
            cameFrom = null;
            List<Pole> closedSet = new List<Pole>();
            List<Pole> openset = new List<Pole>();
            List<Pole> xNeighbor = new List<Pole>();
            openset.Add(this.pozycja);
            int[,] gScore = new int[10, 10];
            gScore = null;
            int[,] fScore = new int[10, 10];
            fScore = null;
            fScore[pozycja.x, pozycja.y] = 0;
            int[,] hScore = new int[10, 10];
            hScore = null;
            gScore[pozycja.x, pozycja.y] = 0;
            while (openset.Count!=0)
            {
                foreach (var p in openset)
                {
                    if (fScore[p.x, p.y] < PoleX.koszt)
                        PoleX = p;
                }
                if (PoleX.x == x && PoleX.y == y)
                    reconstructPath(cameFrom, PoleX);
                openset.Remove(PoleX);
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
            return null;
        }

        private Pole[] reconstructPath(Pole[,] cameFrom, Pole current)
        {
            if (cameFrom[current.x, current.y] != null)
            {
                Pole[] p = new Pole[100];
                p = null;
                p = reconstructPath(cameFrom, cameFrom[current.x, current.y]);
                for (int i = 0; i < 100; i++)
                    if (p[i] == null)
                        p[i] = current;
                return p;
            }
            else
                return null;
        }

        public void goTo (int x, int y)
        {
            Pole[] trasa = AGwiazdka(x, y);
            for(int i = 0; i<trasa.Length; i++)
            {
                pozycja = trasa[i];
            }
        }
	}
}
