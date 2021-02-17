
// file:	Patient\HoHReport.xaml.cs
// Author:  Kyle Horsley
// Date:    April 20 ,2019
// summary:	Implements the add patient.xaml class

using Data;
using Demographics;
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
    /// This is the HoHReport class with all of its methods.
    /// 
    /// Name:               HoHReport      
    /// 
    /// Purpose:            To provide the user functionality to view an entire household.
    /// 
    /// Fault Detection:    This class requiresvery little to function. In order to provide fault detection We 
    ///                     ensure the list has items to no cause faults or errors.
    /// 
    /// Relationships:      This class is related to the Database class and calls upon this class to gain the Hoh information
    ///                     Along with the infomration of their kin.
                            

    public partial class HoHReport : UserControl
    {
        /// <summary>   The patient. </summary>
        Patient patient;
        /// <summary>   The patient to search. </summary>
        ModifyPatientSearchOverlay patientToSearch;
        /// <summary>   The hoh family. </summary>
        List<Patient> hohFamily = new List<Patient>();
        /// <summary>   The head of household. </summary>
        Patient_HoH headOfHousehold;

        ///
        /// <summary>   Default constructor. 
        ///             Initializes the HoH report and starts a new search.</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///

        public HoHReport()
        {
            InitializeComponent();

            StartNewSearch(); //Initialize a new search screen
        }

        ///
        /// <summary>   Starts new search.
        ///             This will pull up a new search bar for the user to search the database </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///

        public void StartNewSearch()
        {
            //Search bar with the Report information
            patientToSearch = new ModifyPatientSearchOverlay("Report");

            HoHPatient.Children.Add(patientToSearch);
        }

        ///
        /// <summary>   Returns a patient from the search bar </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="p">    A Patient to process. </param>
        ///

        public void ReturnPatient(Demographics.Patient p)
        {
            patient = p; //Set the reurned patient  to the patient to use for the report
            HoHPatient.Children.Remove(patientToSearch); //Remove the search overlay
            hohFamily = Database.HoH_Report(patient.HCN); //Get the Report on the returned patient in a list
            UpdateDisplayList(); //Show the list
        }

        ///
        /// <summary>   Updates the display list.
        ///             Fills the HoH value with their information, followed by the family informaition underneath </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        public void UpdateDisplayList()
        {
            if (lstSearchResults != null) //Ensure list is null if it is not
            {
                lstSearchResults.Items.Clear(); //Clear the items
                scrollList.Height = 0;

            }

            //Format the head of household string to be listed in the the hoh of household tex block
            string k;
            k = String.Format("{0} {1}, {2}, {3}, {4}, {5}", hohFamily[0].FirstName, hohFamily[0].LastName, hohFamily[0].HCN, hohFamily[0].MInitial, hohFamily[0].DoB.ToLongDateString(), hohFamily[0].Gender);
            hohName.FontSize = 15;
            hohName.Text = k;

            //Set the appropriate text boxes to the correct values
            headOfHousehold = (Patient_HoH)hohFamily[0];
            addressTextBlock.Text = headOfHousehold.AddressLine1;
            addressTwoTextBlock.Text = headOfHousehold.AddressLine2;
            cityTextBlock.Text = headOfHousehold.City;
            provtextBlock.Text = headOfHousehold.Province;
            pPCodeTextBlock.Text = headOfHousehold.PostalCode;
            pNumTextBlock.Text = headOfHousehold.Phone;


            int colourController = 0;
            foreach (Patient p in hohFamily.Skip(1)) //Skip the head of household
            {
                //Insert a list of fmaily members in the scrollviewer
                Label l = new Label();
                l.Content = String.Format("{0} {1}, {2}, {3}, {4}, {5}", p.FirstName, p.LastName, p.HCN, p.MInitial, p.DoB.ToLongDateString(), p.Gender);
                l.FontSize = 15;
                l.HorizontalAlignment = HorizontalAlignment.Center;

                //Alternate colours
                if (colourController % 2 == 0)
                {
                    l.Background = Brushes.White;
                }
                else
                {
                    l.Background = new SolidColorBrush(Color.FromRgb(225, 225, 225));
                }
                colourController++;

                scrollList.Height += l.Height; //Adjust the height

                lstSearchResults.Items.Add(l);
            }
        }

        ///
        /// <summary>   Event handler. Called by Button for click events. 
        ///             Starts a new search</summary>
        ///
        /// <remarks>   UNIT ONE, 2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ///

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartNewSearch(); //Start a new search
        }
    }
}
