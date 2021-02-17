/*
 * 
 * Author:      Bailey Mills
 * Date:        2019-03-28
 * Project:     EMS2
 * File:        TabScheduling.xaml.cs
 * Description: Contains all UI elements that allow the user to do the following (and more):
 *					o inspect and modify calendar appointments
 *					o set and view billing/recall settings for any appointment
 *					o scroll through any month of the year to load from the database
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
	public partial class TabScheduling : UserControl
	{
		const int COLUMNS_IN_MONTH = 7;
		const int ROWS_IN_MONTH = 6;

		public static TabScheduling tabScheduling;
		public DateTime selectedDate;
		public DateTime currentMonth;
		public CalendarDay selectedTile = null;

		public static AppointmentDetailLine awaitingSelectedDate = null;
		public static readonly int MODE_NORMAL = 0;
		public static readonly int MODE_CHANGE = 1;
		public static readonly int MODE_RECALL = 2;
		public static int currentMode = MODE_NORMAL;

		public SelectedPatientDetails patient1;
		public SelectedPatientDetails patient2;

		public bool highlightOnLoad = false;
		public DateTime highlightDay;

		Brush backgroundColour = Brushes.LightSlateGray;


		///-------------------------------------------------------------------------------------------------
		/// \fn public TabScheduling()
		///
		/// \brief  Prepares all event handlers and appearances for the scheduling screen
		///
		/// \author Bailey
		/// \date   2019-04-10
		///-------------------------------------------------------------------------------------------------
		public TabScheduling()
		{
			InitializeComponent();
			tabScheduling = this;

			MouseWheel += MouseWheelHandler;

			selectedDate = DateTime.Today;
			currentMonth = DateTime.Today;
			LoadCalendar(currentMonth);

			// - Set up Patients Holders -
			// Main Patients / HoH
			patient1 = new SelectedPatientDetails(SelectedPatientDetails.PATIENT_PRIMARY);
			Grid.SetColumn(patient1, 0);
			grdPatientSelection.Children.Add(patient1);
			// Secondary Patients (child)
			patient2 = new SelectedPatientDetails(SelectedPatientDetails.PATIENT_SECONDARY);
			Grid.SetColumn(patient2, 1);
			grdPatientSelection.Children.Add(patient2);

			// Set theme settings
			this.Background = backgroundColour;
			scrollRightPanel.Background = backgroundColour;

			// Refresh Timer
			System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
			dispatcherTimer.Tick += RefreshTimer_Tick;
			dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
			dispatcherTimer.Start();
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void LoadCalendar(DateTime date)
		///
		/// \brief  Loads the calendar for a given date to show all appointment details for the given month
		///
		/// \author Bailey
		/// \date   2019-04-10
		/// 
		/// \param DateTime date : the month and year to start the calendar at
		///-------------------------------------------------------------------------------------------------
		public void LoadCalendar(DateTime date)
		{
			// Delete existing calendar
			calendar.Children.Clear();

			// Set the month + year for the calendar (ex. May  -  2019)
			lblMonth.Text = String.Format("{0}  -  {1}", date.ToString("MMMM"), date.Year);

			// Load new calendar details
			DateTime[,] calendarArray = GenerateCalendar(date);
			for (int col = 0; col < COLUMNS_IN_MONTH; col++)
			{
				for (int row = 0; row < ROWS_IN_MONTH; row++)
				{
					CalendarDay day = new CalendarDay(calendarArray[col, row], date.Month, selectedDate);
					calendar.Children.Add(day);

					// Highlight a tile if requested
					if (highlightOnLoad && calendarArray[col, row] == highlightDay)
					{
						day.SpecialHighlight();
					}

					Grid.SetColumn(day, col);
					Grid.SetRow(day, row);
				}
			}

			UpdateAppointmentList(selectedDate);
			currentMonth = date;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn DateTime[ , ] GenerateCalendar (DateTime focusMonth)
		///
		/// \brief  Called once to generate the calendar and all the base objects in it that give it
		///			the appearance of a calendar (and it's shape)
		///
		/// \author Bailey
		/// \date   2019-04-10
		/// 
		/// \param DateTime focusMonth : the staring month/year of the calendar
		///-------------------------------------------------------------------------------------------------
		DateTime[ , ] GenerateCalendar (DateTime focusMonth)
		{
			DateTime[ , ] calendar = new DateTime[COLUMNS_IN_MONTH, ROWS_IN_MONTH];
			DateTime month = new DateTime(focusMonth.Year, focusMonth.Month, 1);
			
			DateTime dateToPrint = month;

			// Determine where the calendar should start printing the calendar from
			int startOffset = (int)month.DayOfWeek;
			dateToPrint = dateToPrint.AddDays(-startOffset);
			// If the calendar is very short (28 days and starting on a sunday), put the first 7 days from the prev month instead of 0
			if (startOffset == (int)DayOfWeek.Sunday && DateTime.DaysInMonth(month.Year, month.Month) <= COLUMNS_IN_MONTH * 4)
			{
				dateToPrint = dateToPrint.AddDays(-COLUMNS_IN_MONTH);
			}
			
			// Determine the calendar positions for all day in the focus month
			for (int i = 0; i < COLUMNS_IN_MONTH * ROWS_IN_MONTH; i++)
			{
				// Determine position in array
				int column = i % COLUMNS_IN_MONTH;
				int row = (int)(i / COLUMNS_IN_MONTH);

				// Save the day in the array
				calendar[column, row] = new DateTime(dateToPrint.Year, dateToPrint.Month, dateToPrint.Day);

				// Increment the date to print
				dateToPrint = dateToPrint.AddDays(1);
			}

			return calendar;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
		///
		/// \brief  Moves the focus month of the calendar up / down a month when scrolled over
		///
		/// \author Bailey
		/// \date   2019-04-10
		///-------------------------------------------------------------------------------------------------
		private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
		{
			bool focused = true;
			if (searchPatientParent.Children.Count != 0)
			{
				focused = false;
			}

			if (focused)
			{
				// Scroll Up
				if (e.Delta > 0)
				{
					AdjustMonth(-1);
				}

				// Scroll Down
				if (e.Delta < 0)
				{
					AdjustMonth(1);
				}
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void UpdateAppointmentList(DateTime date)
		///
		/// \brief  Updates the list of appointments on the right side of the calendar to be what they should
		///			be for the given day
		///
		/// \author Bailey
		/// \date   2019-04-10
		/// 
		/// \param DateTime date
		///-------------------------------------------------------------------------------------------------
		public void UpdateAppointmentList(DateTime date)
		{
			// Get all appointment detail lines from the appointment details parent
			IEnumerable<AppointmentDetailLine> detailLines = appointmentDetails.grdAppointments.Children.OfType<AppointmentDetailLine>();

			// Get appointments for day
			List<Appointment> appointments = SchedulingSupport.GetAppointmentsForDay(date);
			

			// Update the items accordingly
			int i = 0;
			foreach (AppointmentDetailLine line in detailLines)
			{
				line.ClearDetails();

				if (i == appointments.Count)
				{
					if (appointments.Count < SchedulingSupport.MaxAppointmentsForDay(date))
					{
						line.UpdateAddButton(true);
						line.UpdateClickability(true);
					}
					else
					{
						line.UpdateAddButton(false);
						line.UpdateClickability(false);
					}
				}
				else
				{
					line.UpdateAddButton(false);

					if (i < appointments.Count)
					{
						Appointment curr = appointments[i];
						line.UpdateAppointmentDetails(Database.GetPatientsByAppointmentID(curr.AppointmentID), curr);
						line.UpdateClickability(true);
					}
					else
					{
						line.UpdateClickability(false);
					}
				}

				i++;
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void AdjustMonth(int direction)
		///
		/// \brief  Adjusts the focused month of the calendar whenever the buttons are clicked, or the 
		///			mouse is scrolled up/down. The calendar is reloaded whenever this focus date is changed.
		///
		/// \author Bailey
		/// \date   2019-04-10
		/// 
		/// \param DateTime date
		///-------------------------------------------------------------------------------------------------
		private void BtnMonthBack_Click(object sender, RoutedEventArgs e)
		{
			AdjustMonth(-1);
		}
		private void BtnMonthForward_Click(object sender, RoutedEventArgs e)
		{
			AdjustMonth(1);
		}
		private void AdjustMonth(int direction)
		{
			currentMonth = currentMonth.AddMonths(direction);

			LoadCalendar(currentMonth);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void SetInformation(string information)
		///
		/// \brief  Sets the instructions to contain the string given
		///
		/// \author Bailey
		/// \date   2019-04-10
		/// 
		/// \param string informatoin
		///-------------------------------------------------------------------------------------------------
		public void SetInformation(string information)
		{
			lblInformation.Text = information;
			grdInformation.Background = Brushes.White;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void ClearInformation()
		///
		/// \brief  Clears any text in the information box
		///
		/// \author Bailey
		/// \date   2019-04-10
		///-------------------------------------------------------------------------------------------------
		public void ClearInformation()
		{
			lblInformation.Text = "";
			grdInformation.Background = Brushes.LightGray;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void AwaitDateSelection(AppointmentDetailLine line, int mode)
		///
		/// \brief  Used so that CalendarDays know that the calendar is in a special mode
		///				o booking a recall appointment
		///				o changing the appointment date
		///
		/// \author Bailey
		/// \date   2019-04-10
		/// 
		/// \param AppointmentDetailLine line : the line that is requesting this mode
		/// \param int mode	: the mode to change it to
		///-------------------------------------------------------------------------------------------------
		public void AwaitDateSelection(AppointmentDetailLine line, int mode)
		{
			awaitingSelectedDate = line;
			currentMode = mode;
		}



		int mobileRequestCount = 0;
		///-------------------------------------------------------------------------------------------------
		/// \fn private void RefreshTimer_Tick(object sender, EventArgs e)
		///
		/// \brief  Refreshes to update the mobile related elements in the UI based on remote updates
		///
		/// \author Bailey
		/// \date   2019-04-10
		///-------------------------------------------------------------------------------------------------
		private void RefreshTimer_Tick(object sender, EventArgs e)
		{
			// Update mobile book requests
			UpdateMobileRequests();

			// Update the UI to reflect any check-ins
			if (Database.UpdateMobileFlagsForDate(DateTime.Today))
			{
				LoadCalendar(currentMonth);
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void UpdateMobileRequests()
		///
		/// \brief  Updates the UI to say how many mobile booking requests there are
		///
		/// \author Bailey
		/// \date   2019-04-10
		///-------------------------------------------------------------------------------------------------
		private void UpdateMobileRequests()
		{
			mobileRequestCount = SchedulingSupport.GetMobileRequestCount();

			// Format for "There are 3" vs "There is 1" grammar
			string output = "There ";
			bool plural = false;
			if (mobileRequestCount == 1)
			{
				output += "is ";
			}
			else
			{
				output += "are ";
				plural = true;
			}
			output += String.Format("{0} mobile request", mobileRequestCount);

			// "mobile request(s)"
			if (plural)
			{
				output += "s";
			}

			// Indicate that appointments should be booked
			if (mobileRequestCount > 0)
			{
				grdMobileRequests.Background = Brushes.IndianRed;
				lblMobileRequests.Foreground = Brushes.White;
				lblMobileRequests.FontWeight = FontWeights.Bold;
			}
			else
			{
				grdMobileRequests.Background = Brushes.White;
				lblMobileRequests.Foreground = Brushes.Black;
				lblMobileRequests.FontWeight = FontWeights.Normal;
			}

			// Update label with new string
			lblMobileRequests.Text = output;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void GrdMobileRequests_MouseEnter(object sender, MouseEventArgs e)
		///
		/// \brief  Show that the mobile request button is clickable if there are requests
		///
		/// \author Bailey
		/// \date   2019-04-10
		///-------------------------------------------------------------------------------------------------
		private void GrdMobileRequests_MouseEnter(object sender, MouseEventArgs e)
		{
			if (mobileRequestCount > 0)
			{
				borderMobileRequest.BorderThickness = new Thickness(2, 2, 2, 2);
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void GrdMobileRequests_MouseLeave(object sender, MouseEventArgs e)
		///
		/// \brief  Show that the mobile request button is not clickable if there are no requests
		///
		/// \author Bailey
		/// \date   2019-04-10
		///-------------------------------------------------------------------------------------------------
		private void GrdMobileRequests_MouseLeave(object sender, MouseEventArgs e)
		{
			borderMobileRequest.BorderThickness = new Thickness(1, 1, 1, 1);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void GrdMobileRequests_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		///
		/// \brief  Open mobile requests when clicked (if there are any)
		///
		/// \author Bailey
		/// \date   2019-04-10
		///-------------------------------------------------------------------------------------------------
		private void GrdMobileRequests_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (mobileRequestCount > 0)
			{
				// Open window to edit billing information
				PopupMenus.MobileRequests requests = new PopupMenus.MobileRequests();
				requests.ShowDialog();

				// Reload calendar after updating database
				LoadCalendar(currentMonth);
			}
		}
	}
}
