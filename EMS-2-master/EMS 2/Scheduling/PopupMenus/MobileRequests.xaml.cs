/*
 * 
 * Author:      Bailey Mills
 * Date:        2019-04-16
 * Project:     EMS2
 * File:        MobileRequest.xaml.cs
 * Description: Window that lets the receptionist adjust and view all book requests from
 *				the mobile application(s)
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
    public partial class MobileRequests : Window
	{
		List<MobileRequest> requests;
		List<Patient> patients;

		///-------------------------------------------------------------------------------------------------
		/// \fn public MobileRequests()
		///
		/// \brief  Loads all mobile requests when opened
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		public MobileRequests()
        {
            InitializeComponent();

			// Get list of requests
			LoadRequests();
        }



		///-------------------------------------------------------------------------------------------------
		/// \fn private void LoadRequests()
		///
		/// \brief  Gets and adds all mobile requests to the UI list
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void LoadRequests()
		{
			lstRequests.Items.Clear();

			requests = Database.GetMobileRequests();
			patients = new List<Patient>();
			foreach (MobileRequest request in requests)
			{
				Patient patient = Database.Patients[request.HCN];
				patients.Add(patient);

				lstRequests.Items.Add(String.Format("{0} - {1} {2}", request.Date.ToShortDateString(), patient.FirstName, patient.LastName));
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void LstRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
		///
		/// \brief  Whenever a new item is selected, update the buttons based on what they can do
		///				o cannot book if the date is already full of appointments
		///				o cancel (always available)
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void LstRequests_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int i = lstRequests.SelectedIndex;
			if (i >= 0)
			{
				btnCancel.IsEnabled = true;

				// Only allow booking if there are enough slots
				if (SchedulingSupport.GetAppointmentCountForDay(requests[i].Date) < SchedulingSupport.MaxAppointmentsForDay(requests[i].Date))
				{
					btnBook.IsEnabled = true;
				}
				else
				{
					btnBook.IsEnabled = false;
				}
			}
			else
			{
				btnBook.IsEnabled = false;
				btnCancel.IsEnabled = false;
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void BtnBook_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  Book button clicked. Only possible if there is atleast one appointment slot available.
		///			Mobile client is informed when this happens with a notification
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void BtnBook_Click(object sender, RoutedEventArgs e)
		{
			int i = lstRequests.SelectedIndex;
			if (i >= 0)
			{
				// Get HoH (unless they are the hoh)
				Patient hoh = null;
				string hohHCN = patients[i].GetHoH();
				if (hohHCN != patients[i].HCN)
				{
					hoh = Database.Patients[hohHCN];
				}

				bool booked = SchedulingSupport.BookAppointment(patients[i], hoh, requests[i].Date);

				if (!booked)
				{
					Cancel();
				}

				SubmitResponse(patients[i].HCN, booked ? BuildResponseBook(requests[i].Date) : BuildResponseReject(requests[i].Date));
				LoadRequests();
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void BtnCancel_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  Cancel button clicked. Informs the client that this has happened and  remove the
		///			mobile request from table.
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void BtnCancel_Click(object sender, RoutedEventArgs e)
		{
			Cancel();
			LoadRequests();
		}
		private void Cancel()
		{
			int i = lstRequests.SelectedIndex;
			if (i >= 0)
			{
				SubmitResponse(patients[i].HCN, BuildResponseReject(requests[i].Date));
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void SubmitResponse(string HCN, string status)
		///
		/// \brief  Submits a mobile response whenever the request has been booked or rejected
		///
		/// \author Bailey
		/// \date   2019-04-22
		/// 
		/// \param string HCN
		/// \param string status
		///-------------------------------------------------------------------------------------------------
		private void SubmitResponse(string HCN, string status)
		{
			// Delete request
			Database.DeleteMobileRequest(HCN);

			// Send response
			Database.AddMobileRequestResponse(HCN, status);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private string BuildResponseBook(DateTime date)
		///
		/// \brief  Builds the response for a successful book request
		///
		/// \author Bailey
		/// \date   2019-04-22
		/// 
		/// \param DateTime date
		///-------------------------------------------------------------------------------------------------
		private string BuildResponseBook(DateTime date)
		{
			return String.Format("Your appointment on {0} was booked successfully!", date.ToShortDateString());
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private string BuildResponseReject(DateTime date)
		///
		/// \brief  Builds the response for a rejected book request
		///
		/// \author Bailey
		/// \date   2019-04-22
		/// 
		/// \param DateTime date
		///-------------------------------------------------------------------------------------------------
		private string BuildResponseReject (DateTime date)
		{
			return String.Format("Sorry. {0} is not available for booking.", date.ToShortDateString());
		}
	}
}
