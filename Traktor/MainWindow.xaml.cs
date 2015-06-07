using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.XPath;




namespace Traktor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : ApplicationWindowBase
	{
        public Traktor_agent traktorek { get; private set; }
        List<Image> mainBoard = new List<Image>();

        //tu ustawiamy wartoscui
        int xOfBoard = 10;
        int yOfBoard = 10;
        //musi mieć wymiary (xOfBoard + 1) na (yOfBoard + 1)
        int[,] tablicaTypowPol = new int[11,11];

        int stepCounterHelper = 0;
        bool needToRefill = false;

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer dispatcherTimerMoving = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer dispatcherTimerToHome = new System.Windows.Threading.DispatcherTimer();

        public List<Pole> trasa;
        public int directionX;
        public int directionY;

		public MainWindow()
		{
			InitializeComponent();
			
            CreateBoard(xOfBoard, yOfBoard);
            CreateTablicaTypowPol();

            PoleCollection plansza = new PoleCollection(xOfBoard, yOfBoard, tablicaTypowPol);
            this.traktorek = new Traktor_agent(25, 100, plansza);
            BoardToTraktor.Children.Add(traktorek.image);

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0,500);

            dispatcherTimerMoving.Tick += dispatcherTimerMoving_Tick;
            dispatcherTimerMoving.Interval = new TimeSpan(0, 0, 0, 0,500);

            directionX = RandomNumber(1, xOfBoard + 1);
            directionY = RandomNumber(1, yOfBoard + 1);
            //trasa = traktorek.AGwiazdka(directionX, directionY);

            //dispatcherTimer.Start();
            changeSeason();

            dispatcherTimerToHome.Tick += dispatcherTimerToHome_Tick;
            dispatcherTimerToHome.Interval = new TimeSpan(0, 0, 0, 0, 500);

		}
        private void CreateTablicaTypowPol()
        {
            string typ = string.Empty;
            for (int i = 1; i <= yOfBoard; i++ )
            {
                for (int j = 1; j <= xOfBoard; j++ )
                {
                    typ = GetTypeOfField(j, i);
                    switch (typ)
                    {
                        case "ground":
                            tablicaTypowPol[j, i] = 2;
                            break;
                        case "grass":
                            tablicaTypowPol[j, i] = 3;
                            break;
                        case "barn":
                            tablicaTypowPol[j, i] = 1;
                            break;
                        case "weed":
                            tablicaTypowPol[j, i] = 4;
                            break;
                        case "stone":
                            tablicaTypowPol[j, i] = 1000;
                            break;
                        default:
                            tablicaTypowPol[j, i] = random.Next(1, 6);
                            break;
                    }
                }
            }
        }

        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
        //UWAGA indeksuje od 1.
        public string GetTypeOfField(int x, int y)
        {
            int noOfElement;
            if (x > xOfBoard || y > yOfBoard || y < 1 || x < 1) return "error";
            else
            {
                noOfElement = (y - 1) * xOfBoard + x;
                //Console.WriteLine(noOfElement);
                //Console.WriteLine(mainBoard[noOfElement].Source.ToString());
                if (mainBoard[noOfElement].Source.ToString() =="pack://application:,,,/Images/3.jpg") return "ground";
                else
                    if (mainBoard[noOfElement].Source.ToString() == "pack://application:,,,/Images/2.jpg") return "grass";
                    else
                        if (mainBoard[noOfElement].Source.ToString() == "pack://application:,,,/Images/1.jpg") return "stone";
                        else
                            if (mainBoard[noOfElement].Source.ToString() == "pack://application:,,,/Images/4.jpg") return "weed";
                            else
                                if (mainBoard[noOfElement].Source.ToString() == "pack://application:,,,/Images/9.jpg") return "barn";
                            else return "error";
            }

        }

        void upgradeField(int x, int y)
        {
            int noOfElement;
            if (x > xOfBoard || y > yOfBoard || y < 1 || x < 1) return;
            else
            {
                noOfElement = (y - 1) * xOfBoard + x;
                //Console.WriteLine(noOfElement);
                //Console.WriteLine(mainBoard[noOfElement].Source.ToString());
                if (mainBoard[noOfElement].Source.ToString() == "pack://application:,,,/Images/3.jpg") mainBoard[noOfElement].Source=new BitmapImage(new Uri(@"\Images\" + (2).ToString() + ".jpg", UriKind.RelativeOrAbsolute));
                else
                    if (mainBoard[noOfElement].Source.ToString() == "pack://application:,,,/Images/2.jpg") mainBoard[noOfElement].Source = new BitmapImage(new Uri(@"\Images\" + (4).ToString() + ".jpg", UriKind.RelativeOrAbsolute));
                    /*else
                        if (mainBoard[noOfElement].Source.ToString() == "pack://application:,,,/Images/1.jpg") return "stone";
                        else
                            if (mainBoard[noOfElement].Source.ToString() == "pack://application:,,,/Images/4.jpg") return "weed";
                            else return "error";
                     * */
            }
        }

        void MoveTraktor(int x, int y, Traktor_agent traktor)
        {
            if (traktorek.paliwo_poziom <= 0 || traktorek.jednostkiNawozu < 1)
            {
                goTo(1, 1, traktorek);
                needToRefill = true;
                return;
            }
            NawozLabel.Content = "Pozostało nawozu: " + traktorek.jednostkiNawozu.ToString();
            PaliwoLabel.Content = "Pozostało paliwa: " + traktorek.paliwo_poziom.ToString();
            //if (x > xOfBoard || y > yOfBoard || GetTypeOfField(x,y)=="stone") return;
            if (GetTypeOfField(x,y)=="stone") return;
            //Canvas.SetLeft(traktor.image, (x-1)*50);
            //Canvas.SetTop(traktor.image, (y-1)*50);
            //traktor.poz_x = x;
            //traktor.poz_y = y;
            Console.WriteLine("Jadę do: x " + directionX.ToString() + " y " + directionY.ToString());
            goTo(x, y, traktor);
            Console.WriteLine(GetTypeOfField(traktor.poz_x, traktor.poz_y));
            Console.WriteLine(zasiac(PoraRokuLabel.Content.ToString(), PoraDniaLabel.Content.ToString(), GetTypeOfField(traktor.poz_x, traktor.poz_y)));

            zasiej(traktorek);
            //if (traktorek.jednostkiNawozu < 1) dispatcherTimerToHome.Start();


        }
        public void goTo(int x, int y, Traktor_agent traktor)
        {
            //dispatcherTimer.Stop();
            directionX = x;
            directionY = y;

            LogJazdy.Content = "Jadę do: x " + x.ToString() + " y " + y.ToString();
            trasa = traktor.AGwiazdka(x, y);
            int i = 0;
            stepCounterHelper = i;
            while(trasa[i]==traktorek.pozycja && i<trasa.Count-1)
            {
                i++;
                stepCounterHelper = i;
            }
            dispatcherTimerMoving.Start();
        }

        public void goToStep()
        {
            if (trasa == null || trasa[stepCounterHelper] == null)
            {
                dispatcherTimerMoving.Stop();
                //dispatcherTimer.Start();
                return;
            }
            if (trasa[stepCounterHelper].kierunekDo == 4)
            {
                traktorek.pozycja = traktorek.ALeft(traktorek.pozycja);
            }
            else if (trasa[stepCounterHelper].kierunekDo == 6)
            {
                traktorek.pozycja = traktorek.ARight(traktorek.pozycja);
            }
            else if (trasa[stepCounterHelper].kierunekDo == 8)
            {
                traktorek.pozycja = traktorek.AUp(traktorek.pozycja);
            }
            else if (trasa[stepCounterHelper].kierunekDo == 2)
            {
                traktorek.pozycja = traktorek.ADown(traktorek.pozycja);
            }
            else if (trasa[stepCounterHelper].kierunekDo == 0)
            {
                //do nothing for now
            }
            else
            {
                //do nothing for now
            }
            traktorek.poz_x = traktorek.pozycja.x;
            traktorek.poz_y = traktorek.pozycja.y;
            traktorek.paliwo_poziom -= traktorek.pozycja.koszt;
            PaliwoLabel.Content = "Pozostało paliwa: " + traktorek.paliwo_poziom.ToString();
            if (traktorek.paliwo_poziom < 0)
            {
                traktorek.paliwo_poziom = 0;
                PaliwoLabel.Content = "Paliwo: " + traktorek.paliwo_poziom.ToString() + " Jadę na rezerwie!";
            }
            stepCounterHelper += 1;

            Canvas.SetLeft(traktorek.image, (traktorek.poz_x - 1) * 50);
            Canvas.SetTop(traktorek.image, (traktorek.poz_y - 1) * 50);

            if (traktorek.poz_x == directionX && traktorek.poz_y == directionY)
            {
                if (needToRefill)
                {
                    refill(traktorek);
                    needToRefill = false;
                }
                dispatcherTimerMoving.Stop();
                //dispatcherTimer.Start();
            }
        }
        
        // Wersja 1. funkcji dispatcherTimer_Tick(object sender, EventArgs e)
        //private void dispatcherTimer_Tick(object sender, EventArgs e)
        //{
        //    //1-prawo, 2-lewo, 3-gora, 4-dol
        //    int direction = RandomNumber(0, 5);
        //   // while direction
        //    /*
        //    while ((!(direction == 1 && GetTypeOfField(traktorek.poz_x + 1, traktorek.poz_y)=="stone") && 
        //        (!(direction == 2 && GetTypeOfField(traktorek.poz_x - 1, traktorek.poz_y)=="stone"))&& 
        //        (!(direction == 3 && GetTypeOfField(traktorek.poz_x, traktorek.poz_y-1)=="stone")) && 
        //        (!(direction == 4 && GetTypeOfField(traktorek.poz_x + 1, traktorek.poz_y+1)=="stone"))))
        //    {
        //        direction = RandomNumber(0, 5);

        //    }
        //     * */
        //    if (direction == 1 && traktorek.poz_x < xOfBoard)
        //    {
        //        MoveTraktor(traktorek.poz_x + 1, traktorek.poz_y, traktorek);
        //      //  Console.WriteLine("prawo");


        //    }
        //    else
        //        if (direction == 2 && traktorek.poz_x > 1)
        //        {
        //            MoveTraktor(traktorek.poz_x - 1, traktorek.poz_y, traktorek);
        //           // Console.WriteLine("lewo");

        //        }
        //        else
        //            if (direction == 3 && traktorek.poz_y > 1)
        //            {
        //               // Console.WriteLine("gora");

        //                MoveTraktor(traktorek.poz_x, traktorek.poz_y - 1, traktorek);

        //            }
        //            else
        //                if (direction == 4 && traktorek.poz_y < yOfBoard)
        //                {
        //                    MoveTraktor(traktorek.poz_x, traktorek.poz_y + 1, traktorek);
        //                    // Console.WriteLine("dol");

        //                }
        //                else { }
        //                   // Console.WriteLine("Errrrrror");

        //}

        //Wersja 2 funkcji dispatcherTimer_Tick(object sender, EventArgs e)

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //1-prawo, 2-lewo, 3-gora, 4-dol
            directionX = RandomNumber(1, xOfBoard + 1);
            directionY = RandomNumber(1, yOfBoard + 1);

            MoveTraktor(directionX, directionY, traktorek);

            //for now do nothing
        }

        public void dispatcherTimerMoving_Tick(object sender, EventArgs e)
        {
            goToStep();
            //for now do nothing
        }

        void refill(Traktor_agent traktor)
        {
            traktor.jednostkiNawozu = 10;
            traktorek.tankuj(traktorek.paliwo_max);
            dispatcherTimerToHome.Stop();
            //dispatcherTimer.Start();
        }
        private void dispatcherTimerToHome_Tick(object sender, EventArgs e)
        {
            //dispatcherTimer.Stop();
            if (traktorek.poz_x != 1 && traktorek.poz_y != 1)
                MoveTraktor(traktorek.poz_x - 1, traktorek.poz_y - 1, traktorek);
            else
                if (traktorek.poz_x != 1) MoveTraktor(traktorek.poz_x - 1, traktorek.poz_y, traktorek);
                else
                    if (traktorek.poz_y != 1) MoveTraktor(traktorek.poz_x, traktorek.poz_y - 1, traktorek);
                    else
                        refill(traktorek);

        }
        //plansza ma 500x500
        void CreateBoard(int x, int y)
        {
            if (mainBoard.Count < x * y)
            {
                int pomX = 0;
                int pomY = 0;
                Image b = new Image();
                b.Source = new BitmapImage(new Uri(@"\Images\2.jpg", UriKind.RelativeOrAbsolute));
                b.Margin = Field1.Margin;
                mainBoard.Add(b);
                for (int yi = 0; yi < y; yi++)
                {
                    for (int xi = 0; xi < x; xi++)
                    {
                        Image k = new Image();
                        //nie chce zeby tworzyly sie sciezki bez wyjscia
                        
                        if (howManyStoneAroudField(xi+1, yi+1) > 1)
                        {
                            k.Source = new BitmapImage(new Uri(@"\Images\" + RandomNumber(2, 4).ToString() + ".jpg", UriKind.RelativeOrAbsolute));
                        }

                        else
                        {
                            k.Source = new BitmapImage(new Uri(@"\Images\" + RandomNumber(2, 5).ToString() + ".jpg", UriKind.RelativeOrAbsolute));

                        }
                            
                        
                        k.Margin = Field1.Margin;
                        Board.Children.Add(k);
                        mainBoard.Add(k);
                        Console.WriteLine("Dodanych pol" +mainBoard.Count.ToString());
                        Canvas.SetLeft(mainBoard[mainBoard.Count - 1], pomX);
                        Canvas.SetTop(mainBoard[mainBoard.Count - 1], pomY);
                        pomX = pomX + 50;
                        Console.WriteLine(mainBoard[xi].Source.ToString());
                        Console.WriteLine("Kamieni:" + howManyStoneAroudField(xi+1, yi+1).ToString());
                        Console.WriteLine("---------------------");

                    }
                    pomX = 0;
                    pomY = pomY + 50;

                }
            }
            mainBoard[1].Source = new BitmapImage(new Uri(@"\Images\" + (3).ToString() + ".jpg", UriKind.RelativeOrAbsolute));


        }


        int howManyStoneAroudField(int x, int y)
        {
            int i=0;
            if (x > xOfBoard || y > yOfBoard) return -1;
            else
            {
                if (GetTypeOfField(x - 1, y) == "stone") i = i + 1;
                if (GetTypeOfField(x - 1, y - 1) == "stone") i = i + 1;
                if (GetTypeOfField(x, y - 1) == "stone") i = i + 1;
                if (GetTypeOfField(x + 1, y - 1) == "stone") i = i + 1;
                return i;
            }
        }
        //DRZEWWWWOOO,
        void zasiej(Traktor_agent traktor)
        {
            if (traktor.jednostkiNawozu > 0)
            {
                if (GetTypeOfField(traktor.poz_x, traktor.poz_y) != "stone" && GetTypeOfField(traktor.poz_x, traktor.poz_y) != "error" && zasiac(PoraRokuLabel.Content.ToString(), PoraDniaLabel.Content.ToString(), GetTypeOfField(traktor.poz_x, traktor.poz_y)))
                {
                    upgradeField(traktor.poz_x, traktor.poz_y);
                    traktor.jednostkiNawozu--;
                    NawozLabel.Content = "Pozostało nawozu: " + traktor.jednostkiNawozu.ToString();
                }
            }
            else Console.WriteLine("brakuje nawozu");
        }

        private void LosujButt_Click(object sender, RoutedEventArgs e)
        {
            changeSeason();
        }
        void changeSeason()
        {
            int day=RandomNumber(1,5);
            if (day==1) PoraDniaLabel.Content="rano";
            else
                if (day==2) PoraDniaLabel.Content="poludnie";
                else 
                    if (day==3) PoraDniaLabel.Content="popoludnie";
                    else 
                        if (day==4) PoraDniaLabel.Content="wieczor";
            int year = RandomNumber(1,5);
            if (year == 1) PoraRokuLabel.Content = "wiosna";
            else
                if (year == 2) PoraRokuLabel.Content = "lato";
                else
                    if (year == 3) PoraRokuLabel.Content = "jesien";
                    else
                        if (year == 4) PoraRokuLabel.Content = "zima";
           
        }


        
        XmlDocument document;
        bool zasiac(string pora_roku,string pora_dnia, string rodzaj_gleby)
        {
            document = new XmlDocument();
            
            using (XmlReader reader = XmlReader.Create("./DecisionTree/decisionTree.xml"))

            {

                bool poraDniaFound = false;
                bool poraRokuFound = false;
                bool rodzajGlebyFound = false;
                bool o = false;
                bool b = false;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: // The node is an element.
                           // Console.Write("<" + reader.Name);

                            //bezpiecznik bo coś wywala czasem
                            int bezp = 0;
                            while (reader.MoveToNextAttribute() && bezp < 100)
                            {// Read the attributes.
                               // Console.Write(" " + reader.Name + "='" + reader.Value + "'");
                                bezp++;

                                if (reader.Value == pora_dnia)
                                {
                                    Console.WriteLine("znaleziono dzień!");
                                    poraDniaFound = true;
                                    o = true;
                                    
                                }
                                if (poraDniaFound && reader.AttributeCount == 2 & o)
                                {
                                   
                                    if (reader.Value == "tak")
                                        return true;
                                    else return false;
                                }
                                else o = false;

                                if (poraDniaFound && reader.Value == rodzaj_gleby)
                                {
                                    Console.WriteLine("znaleziono rodzaj gleby!");
                                    rodzajGlebyFound = true;
                                    b = true;
                                }

                                if (rodzajGlebyFound && reader.AttributeCount == 2 & b)
                                {
                                    if (reader.Value == "tak")
                                        return true;
                                    else return false;
                                }
                                else b = false;

                                if (rodzajGlebyFound && pora_roku == reader.Value && reader.AttributeCount == 2)
                                {
                                    Console.WriteLine("znaleziono wartość!");
                                    reader.MoveToNextAttribute();
                                    //Console.Write("<" + reader.Value);
                                    if (reader.Value == "tak")
                                        return true;
                                    else return false;

                                }
                                

                            }
                            //Console.WriteLine(reader.AttributeCount.ToString()+">");
                            break;
                        case XmlNodeType.Text: //Display the text in each element.
                            //Console.WriteLine(reader.Value+ "valllue");
                            break;
                        case XmlNodeType.EndElement: //Display the end of the element.
                           // Console.Write("</" + reader.Name);
                            //Console.WriteLine(">");
                            break;
                    }
                }
               Console.WriteLine("errro");
                return false;

              
            }
                
        }
  
                     
        private static string GetStringValues(string description, XPathNavigator navigator, string xpath)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(description);
            XPathNodeIterator bookNodesIterator = navigator.Select(xpath);
            while (bookNodesIterator.MoveNext())
                sb.Append(string.Format("{0} ", bookNodesIterator.Current.Value));
            return sb.ToString();
        }

        private void GoToButt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int tempX = int.Parse(GoToX.Text);
                int tempY = int.Parse(GoToY.Text);
                if (tempX > 0 && tempX < 11 && tempY > 0 && tempY < 11)
                {
                    directionX = tempX;
                    directionY = tempY;
                    LogJazdy.Content = "Jadę";
                    MoveTraktor(directionX, directionY, traktorek);
                    LogJazdy.Content = "OK [podaj pole docelowe]";
                }
                else
                {
                    LogJazdy.Content = "Błędne pole docelowe!";
                }
            }
            catch(FormatException frmatEx)
            {
                LogJazdy.Content = "BŁĄD - Wpisz liczby [1-10]";
            }
        }
  



	}
}
