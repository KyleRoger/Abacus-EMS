// file:	Patient\ModifyPatientSearchOverlay.xaml.cs
// Author:  Kyle Horsley
// Date:    April 20 ,2019
// summary:	Implements the add ModifyPatientSearchOverlay.xaml class

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
    /// This is the ModifyPatientSearchOverlay class with all of its methods.
    /// 
    /// Name:               ModifyPatientSearchOverlay      
    /// 
    /// Purpose:            To provide the user functionality of a search menu upon modifying a patient and getting a HoH report.
    /// 
    /// Fault Detection:    This class ensures that a patient that is searched is within the database.
    /// 
    /// Relationships:      This class is related to the Database class, the modify class and the HoHReport

    public partial class ModifyPatientSearchOverlay : UserControl
    {
       
        /// <summary>   List of patients. </summary>
        private List<Demographics.Patient> patientList;

        /// <summary>   The modify or report. </summary>
        string modifyOrReport;

        ///
        /// <summary>   Constructor. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="modOrReport">  Whether to modify or report </param>
        ///

        public ModifyPatientSearchOverlay(string modOrReport)
        {
            InitializeComponent();

            if(string.Compare(modOrReport, "Modify") == 0) //If the user wants to modify change the information
            {
                lblTitle.Content = "Modify Patient";
                whatToDo.Content = "Please Enter The Known Information Of The Patient To Modify :";
            }
            else if(string.Compare(modOrReport, "Report") == 0) //If the user wants to get an HoH report.
            {
                lblTitle.Content = "Head Of Household Report";
                whatToDo.Content = "Please Enter The Known Information About The Household  :";
            }
            modifyOrReport = modOrReport;
        }

        ///
        /// <summary>   Event handler. Called by SearchItem for preview mouse down events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Mouse button event information. </param>
        ///

        private void SearchItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int selectedIndex = lstSearchResults.Items.IndexOf(sender); //Select the item.
            ExitSearch(patientList[selectedIndex]); //Return the selected patient.
        }

        ///
        /// <summary>   Called upon exiting the search  To return a patient.</summary>
        ///
        /// <remarks>    2019-04-20. </remarks>
        ///
        /// <param name="selectedItem"> The selected item. </param>
        ///

        private void ExitSearch(Demographics.Patient selectedItem)
        {
            Grid holder = (Grid)this.Parent;

            if (string.Compare(modifyOrReport, "Modify") == 0)
            {
                //Return to modify
                Patients.ModifyPatient parent = (Patients.ModifyPatient)holder.Parent;
                parent.ReturnPatient(selectedItem);
            }
            else if (string.Compare(modifyOrReport, "Report") == 0)
            {
                //Return to HoH
                Patients.HoHReport parent = (Patients.HoHReport)holder.Parent;
                parent.ReturnPatient(selectedItem);
            }
        }

        ///
        /// <summary>   Event handler. Called by TxtSearch for text changed events.
        ///                 Searches for the appropriate patients and displays them in a list.</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>
        ///

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            patientList = SearchPatientPage.FilterPatients(txtSearch.Text); //Patients to search
            if (lstSearchResults != null)
            {
                lstSearchResults.Items.Clear(); //Clear results
                scrollList.Height = 0;
            }

            int colourController = 0;
            foreach (Demographics.Patient p in patientList)
            {
                Label l = new Label();//List patients in format
			    l.Content = String.Format("{0} {1}, {2}, {3}", p.FirstName, p.LastName, p.HCN, p.GetAddress());
				l.FontSize = 15;

                if (colourController % 2 == 0)//Colour opposite lines
                {
                    l.Background = Brushes.White;
                }
                else
                {
                    l.Background = new SolidColorBrush(Color.FromRgb(225, 225, 225));
                }
                colourController++;

                scrollList.Height += l.Height;

                lstSearchResults.Items.Add(l); //Add the items
                l.PreviewMouseDown += new MouseButtonEventHandler(SearchItem_PreviewMouseDown);
            }
        }
    }
}
