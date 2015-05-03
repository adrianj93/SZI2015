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

		public Traktor_agent(int paliwo_poziom, int paliwo_max)
		{
			this.paliwo_poziom = paliwo_poziom;
			this.paliwo_max = paliwo_max;
		}

		public int getPaliwo()
		{
			return paliwo_poziom;
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
