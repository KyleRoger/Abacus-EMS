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

using System.Reflection;

namespace EMS_2
{
	public partial class GenericSearchBar : UserControl
	{
		private string className;
		private string methodName;

		private List<Demographics.Patient> patientList;

		private bool bound;

		public GenericSearchBar(string className, string methodName)
		{
			InitializeComponent();

			Bind(className, methodName);

			txtSearch.Text = "";
		}
		
		public void Bind(string className, string methodName)
		{
			this.className = className;
			this.methodName = methodName;

			bound = true;
		}

		private void Search()
		{
			if (bound)
			{
				patientList = InvokePatientSearch(className, methodName, txtSearch.Text);
				lstSearchResults.Items.Clear();
				scrollList.Height = 0;

				int colourController = 0;
				foreach (Demographics.Patient p in patientList)
				{
					Label l = new Label();
					l.Content = String.Format(" {0} {1}, {2}, {3}, {4} ", p.FirstName, p.LastName, p.HCN, p.GetAddress(), p.GetPhone());
					l.FontSize = 15;

					if (colourController % 2 == 0)
					{
						l.Background = Brushes.White;
					}
					else
					{
						l.Background = new SolidColorBrush(Color.FromRgb(225, 225, 225));
					}
					colourController++;

					scrollList.Height += l.Height;

					lstSearchResults.Items.Add(l);
					l.PreviewMouseDown += new MouseButtonEventHandler(SearchItem_PreviewMouseDown);
				}
			}
			else
			{
				MessageBox.Show("Search bar has not been bound to a method.");
			}
		}


		// Dynamic Method Binding Source:
		// https://www.codeproject.com/Articles/19911/Dynamically-Invoke-A-Method-Given-Strings-with-Met
		public static List<Demographics.Patient> InvokePatientSearch (string className, string methodName, string stringParam)
		{
			// Get the Type for the class
			Type calledType = Type.GetType(className);

			List<Demographics.Patient> list = new List<Demographics.Patient>();

			// Invoke the method itself. The string returned by the method winds up in s.
			// Note that stringParam is passed via the last parameter of InvokeMember,
			// as an array of Objects.
			try
			{
				list = (List<Demographics.Patient>)calledType.InvokeMember(
					methodName,
					BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
					null,
					null,
					new Object[] { stringParam }
				);
			}
			catch { };

			// Return the string that was returned by the called method.
			return list;
		}

		private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
		{
			Search();
		}
		
		// Choosing an item from the search results
		private void SearchItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			int selectedIndex = lstSearchResults.Items.IndexOf(sender);
			ExitSearch(patientList[selectedIndex]);
		}

		private void ExitSearch(Demographics.Patient selectedItem)
		{
			Grid holder = (Grid)this.Parent;
			Patients.SearchPatientPage parent = (Patients.SearchPatientPage)holder.Parent;

			parent.ReturnPatient(selectedItem);
		}

		private void OnLoad(object sender, RoutedEventArgs e)
		{
			txtSearch.Focus();
		}
	}
}
