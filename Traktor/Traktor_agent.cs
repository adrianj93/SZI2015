using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traktor
{
	class Traktor_agent
	{
		private int paliwo_poziom;
		private int paliwo_max;
		private int poz_x;
		private int poz_y;

		private bool available_left = false;
		private bool available_right = false;
		private bool available_forward = false;

		private int passable;

		public Traktor_agent(int paliwo_poziom, int paliwo_max)
		{
			this.paliwo_poziom = paliwo_poziom;
			this.paliwo_max = paliwo_max;

			this.poz_x = 0;
			this.poz_y = 0;
		}

		public int getPaliwo()
		{
			return paliwo_poziom;
		}

		public int getPosition_x()
		{
			return poz_x;
		}

		public int getPosition_y()
		{
			return poz_y;
		}

		public void setPosition(int x, int y)
		{
			this.poz_x = x;
			this.poz_y = y;
		}

		public int detectNextBlock(int left, int right, int forward)
		{
			
			return passable;
		}

		public void tankuj(int paliwo)
		{
			paliwo_poziom += paliwo;
			if (paliwo_poziom > paliwo_max)
			{
				paliwo_poziom = paliwo_max;
			}
		}
	}
}
