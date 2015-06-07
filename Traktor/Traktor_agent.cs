using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Traktor.Resources;

namespace Traktor
{
	public class Traktor_agent
	{
		public int paliwo_poziom {get; set;}
		public int paliwo_max {get; private set;}
        public Pole pozycja { get; set; }
        private PoleCollection plansza;
        public Image image = new Image();
        public int jednostkiNawozu=0;


        public int poz_x;
        public int poz_y;

		public Traktor_agent(int paliwo_poziom, int paliwo_max, PoleCollection plansza)
		{
			this.paliwo_poziom = paliwo_poziom;
			this.paliwo_max = paliwo_max;
            this.image.Source = new BitmapImage(new Uri(@"\Images\0.jpg", UriKind.RelativeOrAbsolute));
            this.poz_x = 1;
            this.poz_y = 1;
            this.jednostkiNawozu=10;
            this.plansza = plansza;
            this.pozycja = plansza.getPole(1, 1);
		}

        public void tankuj(int paliwo)
        {
            paliwo_poziom += paliwo;
            if (paliwo_poziom > paliwo_max)
            {
                paliwo_poziom = paliwo_max;
            }
        }

        public List<Pole> AGwiazdka (int x, int y)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\Logs\TrasaLog.txt", true))
            {
                file.WriteLine("Log Trasy traktora:");
                file.WriteLine("Pozycja startowa x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString());
                file.WriteLine("Pozycja docelowa x:" + x.ToString() + " y:" + y.ToString());
            }

            Pole PoleX = new Pole(-1,-1,1000);
            Pole PoleTemp = new Pole(-1,-1,1000);
            List<Pole> trasa = new List<Pole>();//this.pozycja
            int tentGScore;
            bool tentIsBetter;
            List<Pole> closedSet = new List<Pole>();
            List<Pole> openset = new List<Pole>();
            List<Pole> xNeighbor = new List<Pole>();
            openset.Add(this.pozycja);
            //muszo mieć (rozmiar + 1) na (rozmiar + 1)
            int[,] cameFrom = new int[11, 11];
            for (int i = 0; i < 11; i++)
                for (int j = 0; j < 11; j++)
                    cameFrom[i, j] = -1;
            cameFrom[pozycja.x, pozycja.y] = 0;
            int[,] gScore = new int[11, 11];
            int[,] fScore = new int[11, 11];
            fScore[pozycja.x, pozycja.y] = 0;
            int[,] hScore = new int[11, 11];
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
                {
                    trasa = reconstructPath(cameFrom, PoleX, trasa);
                    break;
                    //return trasa;
                }
                openset.Remove(PoleX);
                if (!closedSet.Contains(PoleX))
                    closedSet.Add(PoleX);
                xNeighbor.RemoveAll(a => a.koszt != null);
                if (ALeft(PoleX) != null)
                {
                    PoleTemp = ALeft(PoleX);
                    //PoleTemp.kierunekDo = 4;
                    xNeighbor.Add(PoleTemp);
                }
                if (ARight(PoleX) != null)
                {
                    PoleTemp = ARight(PoleX);
                    //PoleTemp.kierunekDo = 6;
                    xNeighbor.Add(PoleTemp);
                }
                if (AUp(PoleX) != null)
                {
                    PoleTemp = AUp(PoleX);
                    //PoleTemp.kierunekDo = 8;
                    xNeighbor.Add(PoleTemp);
                }
                if (ADown(PoleX) != null)
                {
                    PoleTemp = ADown(PoleX);
                    //PoleTemp.kierunekDo = 2;
                    xNeighbor.Add(PoleTemp);
                }
                foreach (var p in xNeighbor)
                {
                    if (closedSet.Exists(a => a.x == p.x & a.y == p.y & a.koszt == p.koszt))
                        continue;
                    tentGScore = gScore[PoleX.x, PoleX.y] + p.koszt;
                    tentIsBetter = false;
                    if (!openset.Exists(a => a.x == p.x & a.y == p.y & a.koszt == p.koszt))
                    {
                        openset.Add(p);
                        hScore[p.x, p.y] = Math.Abs(p.x - x) + Math.Abs(p.y - y);
                        tentIsBetter = true;
                    }
                    else if (tentGScore < gScore[p.x, p.y])
                        tentIsBetter = true;
                    if (tentIsBetter)
                    {
                        //trasa.DodajNaKoniec(PoleX);
                        cameFrom[p.x, p.y] = PoleX.y * 10 + PoleX.x;
                        gScore[p.x, p.y] = tentGScore;
                        fScore[p.x, p.y] = gScore[p.x, p.y] + Math.Abs(p.x - x) + Math.Abs(p.y - y);
                    }
                }
            }


            for (int i = 0; i < trasa.Count - 1; i++)
            {
                if (trasa[i + 1].y == trasa[i].y && trasa[i + 1].x == trasa[i].x - 1)
                {
                    trasa[i + 1].kierunekDo = 4;
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\Logs\TrasaLog.txt", true))
                    { file.WriteLine("zachód " + "Pozycja x:" + trasa[i + 1].x.ToString() + " y:" + trasa[i + 1].y.ToString() + " Koszt: " + trasa[i + 1].koszt); }
                }
                else if (trasa[i + 1].y == trasa[i].y && trasa[i + 1].x == trasa[i].x + 1)
                {
                    trasa[i + 1].kierunekDo = 6;
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\Logs\TrasaLog.txt", true))
                    { file.WriteLine("wschód " + "Pozycja x:" + trasa[i + 1].x.ToString() + " y:" + trasa[i + 1].y.ToString() + " Koszt: " + trasa[i + 1].koszt); }
                }
                else if (trasa[i + 1].y == trasa[i].y - 1 && trasa[i + 1].x == trasa[i].x)
                {
                    trasa[i + 1].kierunekDo = 8;
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\Logs\TrasaLog.txt", true))
                    { file.WriteLine("północ " + "Pozycja x:" + trasa[i + 1].x.ToString() + " y:" + trasa[i + 1].y.ToString() + " Koszt: " + trasa[i + 1].koszt); }
                }
                else if (trasa[i + 1].y == trasa[i].y + 1 && trasa[i + 1].x == trasa[i].x)
                {
                    trasa[i + 1].kierunekDo = 2;
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\Logs\TrasaLog.txt", true))
                    { file.WriteLine("południe " + "Pozycja x:" + trasa[i + 1].x.ToString() + " y:" + trasa[i + 1].y.ToString() + " Koszt: " + trasa[i + 1].koszt); }
                }
                else
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\Logs\TrasaLog.txt", true))
                    { 
                        file.WriteLine("BŁĄD TRASOWANIA - PRZERYWAM LOGOWANIE");
                        file.WriteLine("(Log błędu pozycja aktualna: x " + trasa[i + 1].x.ToString() + " y " + trasa[i + 1].y.ToString() + "pozycja poprzednia: x " + trasa[i].x.ToString() + " y " + trasa[i].y.ToString());
                    }
                    break;
                }
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"..\..\Logs\TrasaLog.txt", true))
            {
                file.WriteLine("Koniec trasy -> Osiągnięto pozycję końcową(?)");
                file.WriteLine("***");
            }

            return trasa;
        }

        private List<Pole> reconstructPath(int[,] cameFrom, Pole current, List<Pole> trasa)
        {
            if (cameFrom[current.x, current.y] != 0 && cameFrom[current.x, current.y] != -1 && trasa.Count == 0)
            {
                int tempX;
                int tempY;
                if (cameFrom[current.x, current.y] < 100)
                {
                    tempX = cameFrom[current.x, current.y] % 10;
                    if (tempX == 0)
                        tempX = 10;
                    tempY = (cameFrom[current.x, current.y] - tempX) / 10;
                }
                else
                {
                    tempX = cameFrom[current.x, current.y] % 10;
                    if (tempX == 0)
                        tempX = 10;
                    tempY = 10;
                }
                Pole current1 = plansza.getPole(tempX, tempY);
                if (current != null)
                {
                    reconstructPath(cameFrom, current1, trasa);
                    trasa.Add(current);
                }
                else
                    return null;
            }
            else if (cameFrom[current.x, current.y] == 0)
            {
                trasa.Add(current);
            }
            else
                return null;
            //trasa.Reverse();
            return trasa;
        }

        //public void goTo (int x, int y)
        //{
        //    //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
        //    //{
        //    //    file.WriteLine("Log Trasy traktora:");
        //    //}

        //    //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
        //    //{
        //    //    file.WriteLine("Pozycja startowa x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString());
        //    //}

        //    KolejkaPol trasa = AGwiazdka(x, y);
        //    if (trasa.aktualne == this.pozycja)
        //        trasa = trasa.next;
        //    do
        //    {

        //        if (trasa.aktualne.kierunekDo == 4)
        //        {
        //            this.pozycja = ALeft(this.pozycja);
        //            trasa = trasa.next;

        //            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
        //            //{ file.WriteLine("lewo " + "Pozycja x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString() + " Koszt: " + pozycja.koszt); }
        //        }
        //        else if (trasa.aktualne.kierunekDo == 6)
        //        {
        //            this.pozycja = ARight(this.pozycja);
        //            trasa = trasa.next;

        //            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
        //            //{ file.WriteLine("prawo " + "Pozycja x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString() + " Koszt: " + pozycja.koszt); }
        //        }
        //        else if (trasa.aktualne.kierunekDo == 8)
        //        {
        //            this.pozycja = AUp(this.pozycja);
        //            trasa = trasa.next;

        //            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
        //            //{ file.WriteLine("góra " + "Pozycja x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString() + " Koszt: " + pozycja.koszt); }
        //        }
        //        else if (trasa.aktualne.kierunekDo == 2)
        //        {
        //            this.pozycja = ADown(this.pozycja);
        //            trasa = trasa.next;

        //            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
        //            //{ file.WriteLine("dół " + "Pozycja x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString() + " Koszt: " + pozycja.koszt); }
        //        }
        //        else if (trasa.aktualne.kierunekDo == 0)
        //        {
        //            //?
        //            trasa = trasa.next;

        //        }
        //        else
        //        {
        //            //do nothing for now
        //        }
        //    } while (trasa.next != null);

        //    //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"P:\logTrasy.txt", true))
        //    //{
        //    //    file.WriteLine("Pozycja końcowa x:" + pozycja.x.ToString() + " y:" + pozycja.y.ToString());
        //    //    file.WriteLine("***");
        //    //}
        //}

        public Pole ALeft(Pole p)
        {
            if (plansza.getPole(p.x - 1, p.y) != null)
                return plansza.getPole(p.x - 1, p.y);
            else
                return p;
        }
        public Pole ARight(Pole p)
        {
            if (plansza.getPole(p.x + 1, p.y) != null)
                return plansza.getPole(p.x + 1, p.y);
            else
                return p;
        }
        public Pole AUp(Pole p)
        {
            if (plansza.getPole(p.x, p.y - 1) != null)
                return plansza.getPole(p.x, p.y - 1);
            else
                return p;
        }
        public Pole ADown(Pole p)
        {
            if (plansza.getPole(p.x, p.y + 1) != null)
                return plansza.getPole(p.x, p.y + 1);
            else
                return p;
        }
	}
}
