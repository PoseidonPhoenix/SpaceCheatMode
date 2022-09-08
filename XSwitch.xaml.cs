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

namespace SpaceCheatMode
{
	/// <summary>
	/// Interaction logic for XSwitch.xaml
	/// </summary>
	public partial class XSwitch : UserControl
	{
		bool switchToggled = false;

		public XSwitch()
		{
			InitializeComponent();
			SCircle.Visibility = Visibility.Visible;
			SCircle2.Visibility = Visibility.Hidden;
			switchToggled = false;
		}

		public bool Toggled1 { get => switchToggled; set => switchToggled = value; }

		private void SCircle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (switchToggled == false)
			{
				SCircle.Visibility = Visibility.Hidden;
				SCircle2.Visibility = Visibility.Visible;
				switchToggled = true;
			}
			else
			{
				SCircle.Visibility = Visibility.Visible;
				SCircle2.Visibility = Visibility.Hidden;
				switchToggled = false;
			}
		}

		private void SCircle2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (switchToggled == false)
			{
				SCircle.Visibility = Visibility.Hidden;
				SCircle2.Visibility = Visibility.Visible;
				switchToggled = true;
			}
			else
			{
				SCircle.Visibility = Visibility.Visible;
				SCircle2.Visibility = Visibility.Hidden;
				switchToggled = false;
			}
		}

		private void SBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (switchToggled == false)
			{
				SCircle.Visibility = Visibility.Hidden;
				SCircle2.Visibility = Visibility.Visible;
				switchToggled = true;
			}
			else
			{
				SCircle.Visibility = Visibility.Visible;
				SCircle2.Visibility = Visibility.Hidden;
				switchToggled = false;
			}
		}
	}
}
