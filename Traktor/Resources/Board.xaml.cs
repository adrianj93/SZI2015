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

namespace Traktor.Resources
{
	/// <summary>
	/// Interaction logic for Board.xaml
	/// </summary>
	public partial class Board : UserControl
	{
        public int rozmiar { get; private set; }
        public PoleCollection poleCollection { get; private set; }

		public Board()
		{
			InitializeComponent();

            rozmiar = 10;// rozmiarPlanszy;
            this.BoardControl.DataContext = new CellCollection(rozmiar);
		}

        public bool CanMove(int x, int y)
        {
            if (x < 0 || x > rozmiar || y < 0 || y > rozmiar)
                return false;
            else
                return true;
        }

        private class Cell
        {
            public int Value { get; set; }
            public Cell(int value)
            {
                this.Value = value;
            }
        }

        private class CellCollection
        {
            public List<List<Cell>> RowCollection { get; set; }

            public CellCollection(int size)
            {
                this.RowCollection = new List<List<Cell>>();
                for (int i = 0; i < size; i++)
                {
                    List<Cell> columns = new List<Cell>();
                    for (int j = 0; j < size; j++)
                    {
                        Cell c = new Cell(i * size + j);
                        columns.Add(c);
                    }
                    this.RowCollection.Add(columns);
                }
            }
        }
	}
}
