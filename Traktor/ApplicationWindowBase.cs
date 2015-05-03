using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Traktor
{
	public class ApplicationWindowBase : Window
	{
		public void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				DragMove();
		}

		public void Button_WinClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		public void Button_WinMin_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}
	}

}
