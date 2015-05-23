using System;
using System.Collections.Generic;
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

namespace Traktor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : ApplicationWindowBase
	{
        public Traktor_agent traktorek { get; private set; }
		public MainWindow()
		{
			InitializeComponent();
			
            PoleCollection plansza = new PoleCollection(10);
            this.traktorek = new Traktor_agent(25, 100, plansza);
		}

        private void GoToButt_Click(object sender, RoutedEventArgs e)
        {
            this.traktorek.goTo(int.Parse(GoToX.Text), int.Parse(GoToY.Text));
        }

	}
}
