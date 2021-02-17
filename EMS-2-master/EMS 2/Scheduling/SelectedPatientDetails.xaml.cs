/*
 * 
 * Author:      Bailey Mills
 * Date:        2019-04-16
 * Project:     EMS2
 * File:        SelectedPatientDetails.xaml.cs
 * Description: Allows the user to open up a search screen to choose any patient from
 *				the database. This will be used to book appointments. There are two
 *				instances of this object, one for the patient, one for the head of
 *				household
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using EMS_2.Patients;
using Data;

namespace EMS_2.Scheduling
{
    public partial class SelectedPatientDetails : UserControl
	{
		SearchPatientPage searchScreen;

		public Demographics.Patient patient;

		private const string DEFAULT_TEXT = "+";

		public static readonly int PATIENT_PRIMARY = 1;
		public static readonly int PATIENT_SECONDARY = 2;

		private int patientType;


		///-------------------------------------------------------------------------------------------------
		/// \fn public SelectedPatientDetails(int patientType)
		///
		/// \brief  Sets up the search holder to be used for a given patient type (patient / hoh)
		///
		/// \author Bailey
		/// \date   2019-04-16
		///-------------------------------------------------------------------------------------------------
		public SelectedPatientDetails(int patientType)
		{
			InitializeComponent();
			ResetDetails();

			this.patientType = patientType;

			UpdateColour();
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void SelectedPatient_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		///
		/// \brief  Opens the search screen when clicked
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void SelectedPatient_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			// User cannot edit secondary patient without a primary patient first
			if (patientType == PATIENT_PRIMARY || TabScheduling.tabScheduling.patient1.patient != null)
			{
				searchScreen = new SearchPatientPage(this);
				TabScheduling.tabScheduling.searchPatientParent.Children.Add(searchScreen);
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void UpdateSelectedPatient(Demographics.Patient selectedPatient)
		///
		/// \brief  Called with the patient chosen from the search so that the patient holders can be loaded
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param Patient selectedPatient
		///-------------------------------------------------------------------------------------------------
		public void UpdateSelectedPatient(Demographics.Patient selectedPatient)
		{
			TabScheduling tab = TabScheduling.tabScheduling;
			patient = selectedPatient;

			if (searchScreen != null)
			{
				TabScheduling.tabScheduling.searchPatientParent.Children.Remove(searchScreen);
			}

			// Clearing old details
			// Updating primary should remove secondary
			if (patientType == PATIENT_PRIMARY)
			{
				// Try to load HoH details
				if (patient != null && patient is Demographics.Patient_Dependant)
				{
					// Update details if they are found
					var family = Database.HoH_Report(patient.HCN);
					if (family != null && family.Count > 0)
					{
						Demographics.Patient hoh = family[0];
						tab.patient2.UpdateSelectedPatient(hoh);
					}
				}
				else
				{
					tab.patient2.ResetDetails();
				}
			}

			// Update Details
			UpdateDetails(patient);
			if (selectedPatient == null)
			{
				ResetDetails();
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void ResetDetails()
		///
		/// \brief  Resets the details of the search result holder instance
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		public void ResetDetails()
		{
			lblPlaceholder.Content = DEFAULT_TEXT;
			lblFirstName.Content = "";
			lblHCN.Content = "";
			patient = null;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void UpdateDetails (Demographics.Patient patient)
		///
		/// \brief  Updates the details of the search holder to contain the given patient's details
		///
		/// \author Bailey
		/// \date   2019-04-18
		///
		/// \param Patient selectedPatient
		///-------------------------------------------------------------------------------------------------
		public void UpdateDetails (Demographics.Patient patient)
		{
			TabScheduling.tabScheduling.patient2.UpdateColour();

			if (patient != null)
			{
				lblFirstName.Content = patient.FirstName + " " + patient.LastName;
				lblHCN.Content = patient.HCN.ToString();

				lblPlaceholder.Content = "";
			}
			else
			{
				ResetDetails();
			}
		}


		bool enabled = false;
		///-------------------------------------------------------------------------------------------------
		/// \fn public void UpdateColour()
		///
		/// \brief  Updates the buttons colour to be white if clickable, gray if disabled
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		public void UpdateColour()
		{
			if (patientType == PATIENT_PRIMARY || TabScheduling.tabScheduling.patient1.patient != null)
			{
				selectedPatient.Background = Brushes.White;
				if (patientType == PATIENT_SECONDARY && TabScheduling.tabScheduling.patient2.patient == null)
				{
					lblPlaceholder.Content = DEFAULT_TEXT;
				}
				enabled = true;
			}
			else
			{
				selectedPatient.Background = Brushes.LightGray;
				if (patientType == PATIENT_SECONDARY)
				{
					lblPlaceholder.Content = "";
				}
				enabled = false;
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void Border_MouseEnter(object sender, MouseEventArgs e)
		///
		/// \brief  Highlight when hovered over
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void Border_MouseEnter(object sender, MouseEventArgs e)
		{
			if (enabled)
			{
				border.BorderThickness = new Thickness(2, 2, 2, 2);
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void Border_MouseLeave(object sender, MouseEventArgs e)
		///
		/// \brief  Un-highlight when not hovered over
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void Border_MouseLeave(object sender, MouseEventArgs e)
		{
			border.BorderThickness = new Thickness(1, 1, 1, 1);
		}
	}
}
