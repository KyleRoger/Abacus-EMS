/*
 * 
 * Author:      Bailey Mills
 * Date:        2019-04-20
 * Project:     EMS2
 * File:        BillingDetails.xaml.cs
 * Description: Opens a screen to view all billing details for a given appointment. User can choose
 *				whether this is for the patient or the hoh, and can add/update/delete bill codes
 * 
*/

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
using System.Windows.Shapes;

using Scheduling;
using Demographics;
using Data;
using EMS_Billing;

namespace EMS_2.Scheduling.PopupMenus
{
	public partial class BillingDetails : Window
	{
		Appointment appointment;

		Patient patient = null;
		Patient hoh = null;
		Patient selectedPerson = null;


		///-------------------------------------------------------------------------------------------------
		/// \fn public BillingDetails(Appointment appointment)
		///
		/// \brief  Opens the billcode screen for the given appointment object, pre-made with all present
		///			billcodes for the patient.
		///
		/// \author Bailey
		/// \date   2019-04-16
		///-------------------------------------------------------------------------------------------------
		public BillingDetails(Appointment appointment)
		{
			InitializeComponent();

			this.appointment = appointment;

			// Determine if the appointment is for multiple patients
			var people = Database.GetPatientsByAppointmentID(appointment.AppointmentID);
			if (people.Count >= 1)
			{
				patient = people[0];
				selectedPerson = patient;
			}
			if (people.Count == 2)
			{
				hoh = people[1];
				radHOH.IsEnabled = true;
			}

			LoadBillCodes();

			// Display MoH flag if necessary
			if (appointment.MinistryFlag == 1)
			{
				Label moh = new Label();
				moh.Content = "Please contact the Ministry of Health";
				moh.Foreground = Brushes.Red;
				moh.FontStyle = FontStyles.Italic;
				moh.HorizontalAlignment = HorizontalAlignment.Center;
				this.Height += 20;

				stkDetails.Children.Add(moh);
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void BtnAdd_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  Try to add a billcode when the add button is clicked. It is validated before adding
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void BtnAdd_Click(object sender, RoutedEventArgs e)
		{
			// Try to add
			EMSBilling billing = new EMSBilling();
			string newCode = txtAdd.Text;

			if (billing.validateBillCode(newCode, appointment.Date))
			{
				// Add new billcode to the list
				List<string> codes = appointment.GetBillcodesByHCN(selectedPerson.HCN);
				codes.Add(newCode);

				// Update database
				Database.UpdateBillcodes(appointment.AppointmentID, selectedPerson.HCN, codes);

				// Reset user input
				txtAdd.Clear();
			}
			else
			{
				MessageBox.Show("Invalid Billcode");
			}

			// Reload Bill Codes
			LoadBillCodes();
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void BtnUpdate_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  Try to update an existing billcode when the update button is clicked. It is validated 
		///			before editing, and an existing billcode must be selecetd for this to be possible
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void BtnUpdate_Click(object sender, RoutedEventArgs e)
		{
			// Try to update
			EMSBilling billing = new EMSBilling();
			string replacementCode = txtUpdate.Text;

			if (billing.validateBillCode(replacementCode, appointment.Date))
			{
				// Remove old, add new billcode to list
				List<string> codes = appointment.GetBillcodesByHCN(selectedPerson.HCN);
				codes.Add(replacementCode);
				codes.Remove(lstBillCodes.SelectedItem.ToString());

				// Update database
				Database.UpdateBillcodes(appointment.AppointmentID, selectedPerson.HCN, codes);

				// Reset user input
				txtUpdate.Clear();
			}

			// Reload Bill Codes
			LoadBillCodes();
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void BtnDelete_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  Try to delete an existing billcode when the delete button is clicked. An existing 
		///			billcode must be selecetd for this to be possible.
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void BtnDelete_Click(object sender, RoutedEventArgs e)
		{
			// Remove old, add new billcode to list
			List<string> codes = appointment.GetBillcodesByHCN(selectedPerson.HCN);
			codes.Remove(lstBillCodes.SelectedItem.ToString());

			// Update database
			Database.UpdateBillcodes(appointment.AppointmentID, selectedPerson.HCN, codes);

			// Reload Bill Codes
			LoadBillCodes();
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void LstBillCodes_Selection_Changed(object sender, RoutedEventArgs e)
		///
		/// \brief  Update button availability whenever a new selection is made
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void LstBillCodes_Selection_Changed(object sender, RoutedEventArgs e)
		{
			int index = lstBillCodes.SelectedIndex;
			if (index >= 0)
			{
				btnDelete.IsEnabled = true;
				btnUpdate.IsEnabled = true;
			}
			else
			{
				btnDelete.IsEnabled = false;
				btnUpdate.IsEnabled = false;
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void LoadBillCodes()
		///
		/// \brief  Load all billcodes for the given appointment on construction
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void LoadBillCodes()
		{
			// Empty list
			lstBillCodes.Items.Clear();

			// Gather all items
			List<string> codes = appointment.GetBillcodesByHCN(selectedPerson.HCN);

			foreach (string code in codes)
			{
				lstBillCodes.Items.Add(code);
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void RadPatient_Checked(object sender, RoutedEventArgs e)
		///
		/// \brief  Specify and load the list to load the patient's bill codes
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void RadPatient_Checked(object sender, RoutedEventArgs e)
		{
			if (patient != null)
			{
				selectedPerson = patient;
				LoadBillCodes();
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void RadHOH_Checked(object sender, RoutedEventArgs e)
		///
		/// \brief  Specify and load the list to load the hoh's bill codes (if there is a hoh)
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void RadHOH_Checked(object sender, RoutedEventArgs e)
		{
			if (hoh != null)
			{
				selectedPerson = hoh;
				LoadBillCodes();
			}
		}
	}
}
