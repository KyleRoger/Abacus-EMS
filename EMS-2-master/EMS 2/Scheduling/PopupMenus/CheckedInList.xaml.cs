/*
 * 
 * Author:      Bailey Mills
 * Date:        2019-04-16
 * Project:     EMS2
 * File:        CheckedInList.xaml.cs
 * Description: Popup window that displays the list of check-in statuses for the given date. User
 *				can manually check in a patient, clear them, or reduce this status down to unconfirmed.
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
	public partial class CheckedInList : Window
	{
		List<Appointment> appointments;

		public static readonly Brush COLOUR_NOTHING = Brushes.Transparent;
		public static readonly Brush COLOUR_CHECKED_IN = Brushes.LightPink;
		public static readonly Brush COLOUR_CLEARED = Brushes.LightGreen;

		public static readonly int APPT_NOTHING = 0;
		public static readonly int APPT_CHECKED_IN = 1;
		public static readonly int APPT_CLEARED = 2;



		///-------------------------------------------------------------------------------------------------
		/// \fn public CheckedInList(DateTime date)
		///
		/// \brief  Loads all appointments and their check-in status on construction of the window
		///
		/// \author Bailey
		/// \date   2019-04-16
		///-------------------------------------------------------------------------------------------------
		public CheckedInList(DateTime date)
		{
			InitializeComponent();

			// Update the database to reflect any mobile changes
			Database.UpdateMobileFlagsForDate(date);

			// Load interface with appointments for day
			appointments = SchedulingSupport.GetAppointmentsForDay(date);
			foreach (Appointment appointment in appointments)
			{
				string content = "---";
				var people = Database.GetPatientsByAppointmentID(appointment.AppointmentID);

				if (people.Count > 0)
				{
					content = people[0].FirstName + " " + people[0].LastName;
					if (people.Count == 2)
					{
						content += ", " + people[1].FirstName + " " + people[1].LastName;
					}
				}

				bool cleared = false;
				if (!cleared)
				{
					Label l = new Label();
					l.Width = 240;
					l.Content = content;

					if (appointment.MobileFlag == APPT_CHECKED_IN)
					{
						l.Background = COLOUR_CHECKED_IN;
					}
					else if (appointment.MobileFlag == APPT_CLEARED)
					{
						l.Background = COLOUR_CLEARED;
					}

					lstAppointments.Items.Add(l);
				}
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void UpdateButtons()
		///
		/// \brief  Updates the buttons to adjust check-in status. Disables options if they are already 
		///			the current value. This should be updated whenever the selected item is changed.
		///
		/// \author Bailey
		/// \date   2019-04-16
		///-------------------------------------------------------------------------------------------------
		private void UpdateButtons()
		{
			UpdateButtonAvailability(APPT_NOTHING, btnCheckIn);
			UpdateButtonAvailability(APPT_CHECKED_IN, btnClear);

			int i = lstAppointments.SelectedIndex;
			if (i >= 0 && appointments[i].MobileFlag > APPT_NOTHING)
			{
				btnReset.IsEnabled = true;
			}
			else
			{
				btnReset.IsEnabled = false;
			}
		}
		private void LstAppointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateButtons();
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void UpdateButtonAvailability(int flag, Button btn)
		///
		/// \brief  Enables / disables buttons if they should be based on the appointment's check-in
		///			status
		///
		/// \author Bailey
		/// \date   2019-04-16
		/// 
		/// \param int flag
		/// \param Button btn
		///-------------------------------------------------------------------------------------------------
		private void UpdateButtonAvailability(int flag, Button btn)
		{
			int i = lstAppointments.SelectedIndex;
			if (i >= 0 && appointments[i].MobileFlag == flag)
			{
				btn.IsEnabled = true;
			}
			else
			{
				btn.IsEnabled = false;
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void UpdateFlag(int expected, int newValue, Brush colour)
		///
		/// \brief  Updates the appointment flag based on the selected item in the list of appointments
		///			for the current day. Scheduling user has full control over what this flag's value is.
		///			After pressing any interaction button, the appointment object is updated, and so is the
		///			database.
		///
		/// \author Bailey
		/// \date   2019-04-16
		/// 
		/// \param int flag
		/// \param Button btn
		///-------------------------------------------------------------------------------------------------
		private void BtnCheckIn_Click(object sender, RoutedEventArgs e)
		{
			UpdateFlag(APPT_NOTHING, APPT_CHECKED_IN, COLOUR_CHECKED_IN);
		}
		private void BtnClear_Click(object sender, RoutedEventArgs e)
		{
			UpdateFlag(APPT_CHECKED_IN, APPT_CLEARED, COLOUR_CLEARED);
		}
		private void BtnReset_Click(object sender, RoutedEventArgs e)
		{
			int i = lstAppointments.SelectedIndex;

			if (i >= 0)
			{
				Label item = (Label)lstAppointments.Items[i];
				Appointment appointment = appointments[i];

				// Update the flag to the set value
				if (appointment.MobileFlag == APPT_CHECKED_IN)
				{
					item.Background = COLOUR_NOTHING;
					appointment.MobileFlag = APPT_NOTHING;
				}
				else if (appointment.MobileFlag == APPT_CLEARED)
				{
					item.Background = COLOUR_CHECKED_IN;
					appointment.MobileFlag = APPT_CHECKED_IN;
				}

				appointment.Update();
				UpdateButtons();
			}
		}
		private void UpdateFlag(int expected, int newValue, Brush colour)
		{
			int i = lstAppointments.SelectedIndex;

			if (i >= 0)
			{
				Label item = (Label)lstAppointments.Items[i];
				Appointment appointment = appointments[i];

				// Update the flag to the set value
				if (appointment.MobileFlag == expected)
				{
					item.Background = colour;

					appointment.MobileFlag = newValue;
					appointment.Update();

					UpdateButtons();
				}
			}
		}

	}
}
