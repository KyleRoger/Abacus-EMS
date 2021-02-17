/*
 * 
 * Author:      Bailey Mills
 * Date:        2019-04-16
 * Project:     EMS2
 * File:        CalendarDay.xaml.cs
 * Description: Displays all appointments and visuals necessary to represent any given day
 *				on the calendar.
 *					o the date (number)
 *					o a place for all appointments (up to 6)
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

namespace EMS_2.Scheduling
{
    public partial class CalendarDay : UserControl
	{
		private const double FILTER_DARKEN = 0.15;
		private const double FILTER_WEEKEND = 0.05;
		private const double FILTER_SELECTED = 0.05;

		private DateTime date;
		private int targetMonth;

		private int appointmentCount = 0;
		private int maxAppointments = SchedulingSupport.MAX_APPOINTMENTS_WEEKDAY;

		// Possible colours of the day
		private Brush COLOUR_OK = Brushes.Green;
		private Brush COLOUR_FULL = Brushes.Red;
		private Brush selectedColour = Brushes.Green;

		private ContextMenu cm;


		///-------------------------------------------------------------------------------------------------
		/// \fn public CalendarDay(DateTime day, int targetMonth, DateTime selectedDate)
		///
		/// \brief  Constructs all necessary components of the calendar day to display all appointment 
		///			and calendar day information
		///
		/// \author Bailey
		/// \date   2019-04-16
		/// 
		/// \param DateTime day
		/// \param int targetMonth
		/// \param DateTime selectedDate
		///-------------------------------------------------------------------------------------------------
		public CalendarDay(DateTime day, int targetMonth, DateTime selectedDate)
		{
			InitializeComponent();
			this.date = day;
			this.targetMonth = targetMonth;

			// Set the day of the month (number)
			number.Content = day.Day.ToString();

			// Make the day appear less focused (darker) when it is the previous / next month
			if (day.Month != targetMonth)
			{
				filter.Opacity = FILTER_DARKEN;
			}

			// Darken on weekends
			if (SchedulingSupport.MaxAppointmentsForDay(day) == SchedulingSupport.MAX_APPOINTMENTS_WEEKEND)
			{
				weekendFilter.Opacity = FILTER_WEEKEND;
			}

			maxAppointments = SchedulingSupport.MaxAppointmentsForDay(day);

			AddAppointmentsFromList(SchedulingSupport.GetAppointmentsForDay(date));

			// Highlight if Today
			if (day == DateTime.Today)
			{
				todayFilter.Opacity = 0.15f;
				number.FontWeight = FontWeights.Bold;
			}

			// Highlight the currently selected date
			if (day == selectedDate)
			{
				Select();
			}
			else
			{
				Deselect(selectedDate);
			}

			// Setup context menu for right click
			cm = new ContextMenu();
			MenuItem itemCheckedIn = new MenuItem();
			itemCheckedIn.Header = "View Checked-In";
			itemCheckedIn.Click += CheckedIn_Click;
			cm.Items.Add(itemCheckedIn);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void AddAppointmentsFromList(List<Appointment> appointments)
		///
		/// \brief  Adds all given appointments to the calendar by adding AppointmentLines to the grid slots
		///
		/// \author Bailey
		/// \date   2019-04-16
		/// 
		/// \param List<Appointment> appointments
		///-------------------------------------------------------------------------------------------------
		public void AddAppointmentsFromList(List<Appointment> appointments)
		{
			// Clear existing
			IEnumerable<AppointmentLine> allLines = day.Children.OfType<AppointmentLine>();
			foreach (AppointmentLine existingLine in allLines)
			{
				day.Children.Remove(existingLine);
			}

			// Add new appointments
			int i = 0;
			foreach (Appointment a in appointments)
			{
				AddAppointment(a, i++);
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void AddAppointment (Appointment appointment, int i)
		///
		/// \brief  Adds a single appointment line to the calendar in the requested slot
		///
		/// \author Bailey
		/// \date   2019-04-16
		/// 
		/// \param Appointment appointment
		/// \param int i
		///-------------------------------------------------------------------------------------------------
		public void AddAppointment (Appointment appointment, int i)
		{
			AppointmentLine line = new AppointmentLine(appointment, i);

			// Add to the calendar day tile
			day.Children.Add(line);
			Grid.SetRow(line, ++appointmentCount);

			UpdateColour();
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn protected override void OnMouseEnter(MouseEventArgs e)
		///
		/// \brief  Updates the outline around the calendar day to show it is focused
		///
		/// \author Bailey
		/// \date   2019-04-16
		/// 
		/// \param MouseEventArgs e
		///-------------------------------------------------------------------------------------------------
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			if (TabScheduling.tabScheduling.selectedDate != date)
			{
				SelectOutline();
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn protected override void OnMouseLeave(MouseEventArgs e)
		///
		/// \brief  Updates the outline around the calendar day to show it is not focused anymore
		///
		/// \author Bailey
		/// \date   2019-04-16
		/// 
		/// \param MouseEventArgs e
		///-------------------------------------------------------------------------------------------------
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			Deselect(TabScheduling.tabScheduling.selectedDate);
			DeselectOutline();
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void Select()
		///
		/// \brief  Selects the CalendarDay
		///				o adds an outline around iut
		///				o highlights with the required colour
		///						- colour is based on how many appointments can still be booked for the day  
		///								(green = OK, red = none)
		///
		/// \author Bailey
		/// \date   2019-04-16
		///-------------------------------------------------------------------------------------------------
		public void Select()
		{
			// Save the new tile as selected
			TabScheduling.tabScheduling.selectedTile = this;

			// Update Colour
			selectionFilter.Fill = selectedColour;
			selectionFilter.Opacity = FILTER_SELECTED;

			// Update Border
			currentlySelectedOutline.BorderBrush = selectedColour;
			currentlySelectedOutline.BorderThickness = new Thickness(2, 2, 2, 2);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void Deselect()
		///
		/// \brief  Deselects the CalendarDay so it doesn't look focused
		///
		/// \author Bailey
		/// \date   2019-04-16
		/// 
		/// \param DateTime selectedDate : ensures the selected day doesn't get deselected
		///-------------------------------------------------------------------------------------------------
		public void Deselect(DateTime selectedDate)
		{
			if (selectedDate != date)
			{
				selectionFilter.Opacity = 0.0f;

				currentlySelectedOutline.BorderThickness = new Thickness(0, 0, 0, 0);
			}
		}


		///-------------------------------------------------------------------------------------------------
		/// \fn public void SelectOutline()
		///
		/// \brief  Outlines the calendar day to show that it is currently selected in the calendar
		///
		/// \author Bailey
		/// \date   2019-04-16
		///-------------------------------------------------------------------------------------------------
		public void SelectOutline()
		{
			if (TabScheduling.currentMode == TabScheduling.MODE_NORMAL)
			{
				selectionOutine.BorderBrush = selectedColour;
			}
			else
			{
				selectionOutine.BorderBrush = Brushes.Red;
			}

			selectionOutine.BorderThickness = new Thickness(2, 2, 2, 2);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void DeselectOutline()
		///
		/// \brief  Un-outlines the calendar day to show that it is not the selected date
		///
		/// \author Bailey
		/// \date   2019-04-16
		///-------------------------------------------------------------------------------------------------
		public void DeselectOutline()
		{
			selectionOutine.BorderThickness = new Thickness(0, 0, 0, 0);
		}



		// Selecting a day on the calendar
		// Loads up appointment details on the right side
		// Allows booking

		///-------------------------------------------------------------------------------------------------
		/// \fn private void Day_LClick(object sender, MouseButtonEventArgs e)
		///
		/// \brief  Selects the day when clicked. Removes the last selection when updated.
		///
		/// \author Bailey
		/// \date   2019-04-16
		///
		/// \param object sender
		/// \param MouseButtonEventArgs e 
		///-------------------------------------------------------------------------------------------------
		private void Day_LClick(object sender, MouseButtonEventArgs e)
		{
			SelectDay();
		}
		private void SelectDay()
		{
			TabScheduling tab = TabScheduling.tabScheduling;
			// Changing the selected date (as normal)
			if (TabScheduling.currentMode == TabScheduling.MODE_NORMAL)
			{
				tab.selectedDate = date;

				// Remove the old selection
				if (tab.selectedTile != null)
				{
					tab.selectedTile.Deselect(tab.selectedDate);
				}

				// Make it look like it's been selected
				Select();

				// Update the appointment list
				TabScheduling.tabScheduling.UpdateAppointmentList(date);
			}
			// Request a date change
			else if (TabScheduling.currentMode == TabScheduling.MODE_CHANGE)
			{
				TabScheduling.awaitingSelectedDate.RequestDateChange(date);
			}
			// Request a recall booking
			else if (TabScheduling.currentMode == TabScheduling.MODE_RECALL)
			{
				TabScheduling.awaitingSelectedDate.RequestBookFromFlag(date);
			}

			TabScheduling.awaitingSelectedDate = null;
			TabScheduling.tabScheduling.ClearInformation();
			TabScheduling.currentMode = TabScheduling.MODE_NORMAL;

			DeselectOutline();
		}


		

		///-------------------------------------------------------------------------------------------------
		/// \fn private void Day_RClick(object sender, MouseButtonEventArgs e)
		///
		/// \brief  Opens the context menu to view mobile check-ins when right-clicked
		///
		/// \author Bailey
		/// \date   2019-04-16
		///
		/// \param object sender
		/// \param MouseButtonEventArgs e
		///-------------------------------------------------------------------------------------------------
		private void Day_RClick(object sender, MouseButtonEventArgs e)
		{
			SelectDay();

			// Display Context Menu
			cm.PlacementTarget = sender as Button;
			cm.IsOpen = true;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void UpdateColour()
		///
		/// \brief  Updates the colour of the tile based on how many appointments there are for the day
		///				o green = OK
		///				o red = full
		///
		/// \author Bailey
		/// \date   2019-04-16
		///-------------------------------------------------------------------------------------------------
		private void UpdateColour()
		{
			int remainingSlots = maxAppointments - appointmentCount;

			// No more slots available
			if (remainingSlots <= 0)
			{
				selectedColour = COLOUR_FULL;
			}
			else
			{
				selectedColour = COLOUR_OK;
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void SpecialHighlight()
		///
		/// \brief  Highlights the day if it's for today's date
		///
		/// \author Bailey
		/// \date   2019-04-16
		///-------------------------------------------------------------------------------------------------
		public void SpecialHighlight()
		{
			specialFilter.Opacity = 0.2f;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void CheckedIn_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  View check-in details when the menu item is clicked
		///
		/// \author Bailey
		/// \date   2019-04-16
		///
		/// \param object sender
		/// \param RoutedEventArgs e
		///-------------------------------------------------------------------------------------------------
		private void CheckedIn_Click(object sender, RoutedEventArgs e)
		{
			// Open window to edit billing information
			PopupMenus.CheckedInList checkin = new PopupMenus.CheckedInList(date);
			checkin.ShowDialog();

			// Reload calendar after updating database
			TabScheduling.tabScheduling.LoadCalendar(TabScheduling.tabScheduling.currentMonth);
		}
	}
}
