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
using EMS_2.Scheduling.AppointmentDetails;

namespace EMS_2.Scheduling.Calendar
{
    public partial class CalendarDay : UserControl
	{
		private const double FILTER_DARKEN = 0.25;
		private const double FILTER_SELECTED = 0.1;

		private DateTime date;
		private int targetMonth;

		private int appointmentCount = 0;
		private int maxAppointments = SchedulingSupport.MAX_APPOINTMENTS_WEEKDAY;

		// Possible colours of the day
		private SolidColorBrush COLOUR_OK = new SolidColorBrush(Colors.Green);
		private SolidColorBrush COLOUR_FULL = new SolidColorBrush(Colors.Red);

		public CalendarDay(DateTime day, int targetMonth)
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

			maxAppointments = SchedulingSupport.MaxAppointmentsForDay(day);

			AddAppointmentsFromList(SchedulingSupport.GetAppointmentsForDay(date));
		}


		public void AddAppointmentsFromList(List<Appointment> appointments)
		{
			// Clear existing
			IEnumerable<AppointmentLine> allLines = day.Children.OfType<AppointmentLine>();
			foreach (AppointmentLine existingLine in allLines)
			{
				day.Children.Remove(existingLine);
			}

			// Add new appointments
			foreach (Appointment a in appointments)
			{
				AddAppointment(a);
			}
		}


		public void AddAppointment (Appointment appointment)
		{
			AppointmentLine line = new AppointmentLine(appointment);
			day.Children.Add(line);
			Grid.SetRow(line, ++appointmentCount);

			UpdateColour();
		}



		protected override void OnMouseEnter(MouseEventArgs e)
		{
			SelectOutline();
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			Deselect();
			DeselectOutline();
		}

		public void Select()
		{
			selectionFilter.Fill = appointmentColourFilter.Fill;
			selectionFilter.Opacity = FILTER_SELECTED;

			currentlySelectedOutline.BorderBrush = appointmentColourFilter.Fill;
			currentlySelectedOutline.BorderThickness = new Thickness(2, 2, 2, 2);
		}

		public void Deselect()
		{
			if (TabScheduling.tabScheduling.selectedDate != date)
			{
				selectionFilter.Opacity = 0.0f;

				currentlySelectedOutline.BorderThickness = new Thickness(0, 0, 0, 0);
			}
		}

		public void SelectOutline()
		{
			selectionOutine.BorderThickness = new Thickness(2, 2, 2, 2);
		}

		public void DeselectOutline()
		{
			selectionOutine.BorderThickness = new Thickness(0, 0, 0, 0);
		}



		// Selecting a day on the calendar
		// Loads up appointment details on the right side
		// Allows booking
		private void Day_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			// Load up the appointment details based on the selected day
			TabScheduling.tabScheduling.LoadAppointmentDetails(this);
			TabScheduling.tabScheduling.selectedDate = date;

			// Remove the old selection
			if (TabScheduling.tabScheduling.selectedTile != null)
			{
				TabScheduling.tabScheduling.selectedTile.Deselect();
			}

			// Save the new tile as selected
			TabScheduling.tabScheduling.selectedTile = this;

			// Make it look like it's been selected
			Select();

			// Update the appointment list
			TabScheduling.tabScheduling.UpdateAppointmentList(this, date);
		}

		private void UpdateColour()
		{
			int remainingSlots = maxAppointments - appointmentCount;

			// No more slots available
			if (remainingSlots <= 0)
			{
				appointmentColourFilter.Fill = COLOUR_FULL;
			}
			else
			{
				appointmentColourFilter.Fill = COLOUR_OK;
			}
		}
	}
}
