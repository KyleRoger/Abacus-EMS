/*
 * 
 * Author:      Bailey Mills
 * Date:        2019-04-16
 * Project:     EMS2
 * File:        AppointmentLine.xaml.cs
 * Description: Contains the text that is present in a single line of a CalendarDay tile object
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Scheduling;
using Data;

namespace EMS_2.Scheduling
{
	public partial class AppointmentLine : UserControl
	{
		///-------------------------------------------------------------------------------------------------
		/// \fn public AppointmentLine(Appointment appointment, int i)
		///
		/// \brief  Loads appointment details into the appointment line
		///
		/// \author Bailey
		/// \date   2019-04-16
		/// 
		/// \param Appointment appointment
		/// \param int i
		///-------------------------------------------------------------------------------------------------
		public AppointmentLine(Appointment appointment, int i)
		{
			InitializeComponent();

			string content = "---";
			var people = Database.GetPatientsByAppointmentID(appointment.AppointmentID);

			// Set content for line
			if (people.Count > 0)
			{
				content = people[0].FirstName + " " + people[0].LastName;
				if (people.Count == 2)
				{
					content += ", " + people[1].FirstName + " " + people[1].LastName;
				}
			}

			lblAppointmentLine.Content = content;


			// - Update Flag Details -
			bool flagRecal = false;
			bool flagBilling = false;

			// Determine if Recall Flag
			if (appointment.RecallFlag > 0)
			{
				flagRecal = true;
				rectRecall.Fill = Brushes.Red;
			}
			
			// Determine if Billing Flag
			if (appointment.GetBillcodesByHCN(people[0].HCN).Count > 0)
			{
				flagBilling = true;
			}
			else if (people.Count == 2 && appointment.GetBillcodesByHCN(people[1].HCN).Count > 0)
			{
				flagBilling = true;
			}
			
			// Both
			if (flagBilling)
			{
				rectBilling.Fill = Brushes.Blue;

				if (flagRecal)
				{
					Grid.SetColumnSpan(rectRecall, 1);

					Grid.SetColumn(rectBilling, 1);
					Grid.SetColumnSpan(rectBilling, 1);
				}
			}


			// Check-In Display (only if today)
			if (appointment.Date == DateTime.Today)
			{
				Grid.SetColumnSpan(viewAppointmentInfo, 1);

				string symbol = "x";
				if (appointment.MobileFlag == PopupMenus.CheckedInList.APPT_CHECKED_IN)
				{
					symbol = "o";
					grdLabel.Background = PopupMenus.CheckedInList.COLOUR_CHECKED_IN;
				}
				else if (appointment.MobileFlag == PopupMenus.CheckedInList.APPT_CLEARED)
				{
					symbol = "✓";
					grdLabel.Background = PopupMenus.CheckedInList.COLOUR_CLEARED;
				}
				
				lblCheckIn.Content = symbol;
			}


			// Offsetting the background colour (white, gray, white, gray, ...)
			if (i % 2 == 0)
			{
				this.Background = Brushes.White;
			}
			else
			{
				this.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
			}
		}

	}
}
