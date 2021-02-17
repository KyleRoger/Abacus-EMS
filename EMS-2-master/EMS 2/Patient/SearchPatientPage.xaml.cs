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

namespace EMS_2.Patients
{
    /// <summary>
    /// Interaction logic for searchFromMain.xaml
    /// </summary>
    public partial class SearchPatientPage : UserControl
    {
        GenericSearchBar searchBar;
        Scheduling.SelectedPatientDetails parent;
        

        public SearchPatientPage(Scheduling.SelectedPatientDetails parent)
        {
            InitializeComponent();

			// Bind to the current method, required so that the search bar can be used in any situation / method / class
            searchBar = new GenericSearchBar("EMS_2.Patients.SearchPatientPage", "FilterPatients");
            searchBar.Margin = new Thickness(0, 20, 0, 20);
            grdParent.Children.Add(searchBar);
			Keyboard.Focus(searchBar.txtSearch);

            this.parent = parent;

			// Adding a cancel button
			Button cancelButton = new Button();
			cancelButton.Height = 35;
			cancelButton.Height = 35;
			cancelButton.Content = "X";
			cancelButton.FontSize = 17;
			cancelButton.Foreground = Brushes.Crimson;
			cancelButton.FontWeight = FontWeights.ExtraBold;
			cancelButton.Background = Brushes.AliceBlue;
			cancelButton.VerticalAlignment = VerticalAlignment.Center;
			cancelButton.Click += CancelButton_Click;

			// Add to search menu
			searchBar.grdSearchBarGroup.Children.Add(cancelButton);
			Grid.SetColumn(cancelButton, 2);
		}

        private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			ReturnPatient(null);
		}
		

        public static List<Demographics.Patient> FilterPatients(string searchString)
        {
            List<Demographics.Patient> filteredList = new List<Demographics.Patient>();

			string[] paramList = searchString.ToLower().Split(',',';');

            foreach (KeyValuePair<string, Demographics.Patient> p in Data.Database.Patients.Where(p =>
                paramList.Contains(p.Value.FirstName.ToLower()) ||
                paramList.Contains(p.Value.LastName.ToLower()) ||
                paramList.Contains(p.Value.HCN.ToLower()) ||
				paramList.Contains(p.Value.GetAddress().ToLower()) ||
				paramList.Contains(p.Value.GetPhone().ToLower())
				))
            {
                filteredList.Add(p.Value);
            }

            return filteredList;
        }

        public void ReturnPatient(Demographics.Patient p)
        {
            parent.UpdateSelectedPatient(p);
        }
    }
}
