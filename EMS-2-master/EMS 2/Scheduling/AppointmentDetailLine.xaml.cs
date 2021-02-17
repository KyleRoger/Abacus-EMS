/*
 * 
 * Author:      Bailey Mills
 * Date:        2019-04-16
 * Project:     EMS2
 * File:        AppointmentDetailLine.xaml.cs
 * Description: Displays information about a single appointment for a single day. Allows the user
 *				to click to interact with it:
 *					o book appointment
 *					o interact with recall flag
 *					o open / edit billing
 *					o change date
 *					o delete appointment
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
	public partial class AppointmentDetailLine : UserControl
	{
		private bool clickable;
		private bool bookable;
		
		private Appointment appointment;

		private Brush backgroundColour = Brushes.White;

		Demographics.Patient[] patients = new Demographics.Patient[2];

		// Context Menu Items
		private ContextMenu cm;
		private MenuItem itemRecallBook;
		private MenuItem itemRecallClear;
		private MenuItem itemRecall1;
		private MenuItem itemRecall2;
		private MenuItem itemRecall3;

		///-------------------------------------------------------------------------------------------------
		/// \fn public AppointmentDetailLine()
		///
		/// \brief  Prepares an empty appointment line detail that is ready to be given appointment details
		///			to display when the time comes
		///
		/// \author Bailey
		/// \date   2019-04-16
		///-------------------------------------------------------------------------------------------------
		public AppointmentDetailLine()
		{
			InitializeComponent();

			clickable = false;
			bookable = false;


			// Create the context menu that will be added to the right click event
			cm = new ContextMenu();

			// - Recall -
			MenuItem itemRecall = new MenuItem();
			itemRecall.Header = "Recall";
			cm.Items.Add(itemRecall);
			// Book
			itemRecallBook = new MenuItem();
			itemRecallBook.Header = "Book";
			itemRecall.Items.Add(itemRecallBook);
			itemRecallBook.Click += Recall_Book_Click;
			// Separator
			itemRecall.Items.Add(new Separator());
			// Clear
			itemRecallClear = new MenuItem();
			itemRecallClear.Header = "Clear";
			itemRecall.Items.Add(itemRecallClear);
			itemRecallClear.Click += Recall_Clear_Click;
			// 1-Week
			itemRecall1 = new MenuItem();
			itemRecall1.Header = "1-Week";
			itemRecall.Items.Add(itemRecall1);
			itemRecall1.Click += Recall_1_Click;
			// 2-Weeks
			itemRecall2 = new MenuItem();
			itemRecall2.Header = "2-Week";
			itemRecall.Items.Add(itemRecall2);
			itemRecall2.Click += Recall_2_Click;
			// 3-Weeks
			itemRecall3 = new MenuItem();
			itemRecall3.Header = "3-Week";
			itemRecall.Items.Add(itemRecall3);
			itemRecall3.Click += Recall_3_Click;


			// Billing
			MenuItem itemBilling = new MenuItem();
			itemBilling.Header = "Billing";
			itemBilling.Click += Billing_Click;
			cm.Items.Add(itemBilling);

			// Changing the appointment date
			MenuItem itemDate = new MenuItem();
			itemDate.Header = "Change Date";
			itemDate.Click += Change_Click;
			cm.Items.Add(itemDate);

			cm.Items.Add(new Separator());

			// Deleting the appointment
			MenuItem itemDelete = new MenuItem();
			itemDelete.Header = "Delete Appointment";
			itemDelete.Click += Delete_Click;
			cm.Items.Add(itemDelete);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void ClearDetails()
		///
		/// \brief  Resets any appointment details
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		public void ClearDetails()
		{
			// Clear patient
			ClearPatientDetails();

			// Clear head of household
			ClearHoHDetails();

			// Reset fill colour
			backgroundColour = Brushes.White;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void ClearPatientDetails()
		///
		/// \brief  Clears any details in the patient slot of the grid element (Left / middle)
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void ClearPatientDetails()
		{
			lblPatientName.Content = "";
			lblPatientHCN.Content = "";
			lblPatientDescription.Content = "";
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void ClearHoHDetails()
		///
		/// \brief  Clears any details in the head of household slot of the grid element (right)
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void ClearHoHDetails()
		{
			lblHoHName.Content = "";
			lblHoHHCN.Content = "";
			lblHoHDescription.Content = "";
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void SetPatientDetails(Demographics.Patient patient, string strDescription, Label name, Label hcn, Label description)
		///
		/// \brief  Updates the given ui elements with the necessary content from the Patient object
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  Patient patient
		/// \param	string strDescription
		/// \param	Label name
		/// \param	Label hcn
		/// \param	Label description
		///-------------------------------------------------------------------------------------------------
		private void SetPatientDetails(Demographics.Patient patient, string strDescription, Label name, Label hcn, Label description)
		{
			name.Content = patient.FirstName + " " + patient.LastName;
			hcn.Content = patient.HCN;
			description.Content = strDescription;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void UpdateAppointmentDetails(List<Demographics.Patient> patientList, Appointment appointment)
		///
		/// \brief  Updates the appointment details for the appointmentdetailline using the given details about
		///			the appointment
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  List<Demographics.Patient> patientList : either 1 or both people in the 
		///				appointment (0: patient, 1: hoh)
		/// \param	Appointment appointment : the appointment instance
		///-------------------------------------------------------------------------------------------------
		public void UpdateAppointmentDetails(List<Demographics.Patient> patientList, Appointment appointment)
		{
			int count = patientList.Count;

			bool recallFlag = false;
			bool billingFlag = false;

			if (count >= 1)
			{
				Demographics.Patient patient = patientList[0];
				patients[0] = patient;

				SetPatientDetails(patient, "Patient", lblPatientName, lblPatientHCN, lblPatientDescription);

				// Update recall flag
				if (appointment.RecallFlag > 0)
				{
					recallFlag = true;
				}

				// Updating billing flag
				if (appointment.GetBillcodesByHCN(patient.HCN).Count > 0)
				{
					billingFlag = true;
				}

				if (count == 2)
				{
					Demographics.Patient hoh = patientList[1];
					patients[1] = hoh;

					SetPatientDetails(hoh, "Head of Household", lblHoHName, lblHoHHCN, lblHoHDescription);

					// Update width (shrink width of patient info to fit hoh info)
					Grid.SetColumnSpan(viewPatient, 1);
					Grid.SetColumnSpan(lblPatientHCN, 1);
					Grid.SetColumnSpan(lblPatientDescription, 1);

					// Updating billing flag
					if (!billingFlag && appointment.GetBillcodesByHCN(hoh.HCN).Count > 0)
					{
						billingFlag = true;
					}
				}
				else
				{
					ClearHoHDetails();

					// Update width (expand width of patient info)
					Grid.SetColumnSpan(viewPatient, 2);
					Grid.SetColumnSpan(lblPatientHCN, 2);
					Grid.SetColumnSpan(lblPatientDescription, 2);
				}

				// Update colouring based on the recall and billing contents
				if (recallFlag && billingFlag)
				{
					backgroundColour = Brushes.Plum;
				}
				else if (recallFlag)
				{
					backgroundColour = Brushes.LightPink;
				}
				else if (billingFlag)
				{
					backgroundColour = Brushes.LightBlue;
				}
				else
				{
					backgroundColour = Brushes.White;
				}
			}
			else
			{
				ClearDetails();
			}

			this.appointment = appointment;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void UpdateClickability(bool enabled)
		///
		/// \brief  Allows / disallows the clicking of itself
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  bool enabled : whether the button will be enabled or not
		///-------------------------------------------------------------------------------------------------
		public void UpdateClickability(bool enabled)
		{
			background.Fill = enabled ? backgroundColour : Brushes.LightGray;
			clickable = enabled;
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void UpdateAddButton(bool enabled)
		///
		/// \brief  Sets whether the object will be used to book appointments
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  bool enabled : whether the button will be enabled or not
		///-------------------------------------------------------------------------------------------------
		public void UpdateAddButton(bool enabled)
		{
			if (enabled)
			{
				bookable = true;
				lblAdd.Visibility = Visibility.Visible;

				// Reset fill colour
				background.Fill = Brushes.White;
			}
			else
			{
				bookable = false;
				lblAdd.Visibility = Visibility.Hidden;
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void GrdAppointmentDetails_Click(object sender, MouseButtonEventArgs e)
		///
		/// \brief  Handles all possible cases for when the object is clicked
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  object sender
		/// \param  MouseButtonEventArgs e
		///-------------------------------------------------------------------------------------------------
		private void GrdAppointmentDetails_Click(object sender, MouseButtonEventArgs e)
		{
			if (clickable)
			{
				// Booking an appointment
				if (bookable)
				{
					TabScheduling tab = TabScheduling.tabScheduling;
					Demographics.Patient patient = tab.patient1.patient;
					Demographics.Patient hoh = tab.patient2.patient;

					// Book appointment
					if (patient != null)
					{
						// Only allow booking on and after today
						if (tab.selectedDate >= DateTime.Today)
						{
							SchedulingSupport.BookAppointment(patient, hoh, tab.selectedDate);

							// Clear instructions
							tab.ClearInformation();

							// Refresh Calendar
							tab.LoadCalendar(tab.currentMonth);
						}
						else
						{
							TabScheduling.tabScheduling.SetInformation("You cannot book an appointment for a date that has already past");
						}
					}
					// No patient(s) given
					else
					{
						tab.SetInformation("You must first select the patient that the appointment is for.");
					}
				}
				// Inspecting actions for a tile
				else
				{
					// Enable all items
					itemRecallBook.IsEnabled = false;
					itemRecallClear.IsEnabled = true;
					itemRecall1.IsEnabled = true;
					itemRecall2.IsEnabled = true;
					itemRecall3.IsEnabled = true;

					// Update Context Menu
					int recallFlag = appointment.RecallFlag;
					if (recallFlag == 0)
					{
						itemRecallClear.IsEnabled = false;
					}
					else if (recallFlag > 0)
					{
						itemRecallBook.IsEnabled = true;

						if (TabScheduling.currentMode == TabScheduling.MODE_RECALL)
						{
							itemRecall1.IsEnabled = false;
							itemRecall2.IsEnabled = false;
							itemRecall3.IsEnabled = false;
							itemRecallClear.IsEnabled = false;
							itemRecallBook.Header = "Cancel";
						}
						else
						{
							itemRecallBook.Header = "Book";
							if (recallFlag == 1)
							{
								itemRecall1.IsEnabled = false;
							}
							else if (recallFlag == 2)
							{
								itemRecall2.IsEnabled = false;
							}
							else if (recallFlag == 3)
							{
								itemRecall3.IsEnabled = false;
							}
						}
					}

					// Display Context Menu
					cm.PlacementTarget = sender as Button;
					cm.IsOpen = true;
				}
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void Recall_Book_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  Handles the click event for the book request for a recall appointment
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  object sender
		/// \param  RoutedEventArgs e
		///-------------------------------------------------------------------------------------------------
		private void Recall_Book_Click(object sender, RoutedEventArgs e)
		{
			TabScheduling tab = TabScheduling.tabScheduling;

			// Cancel booking if already booking
			if (TabScheduling.currentMode == TabScheduling.MODE_RECALL)
			{
				// Remove the highlight around the target date
				TabScheduling.currentMode = TabScheduling.MODE_NORMAL;
				TabScheduling.awaitingSelectedDate = null;
				tab.highlightOnLoad = false;

				// Clear instructions
				tab.ClearInformation();

				// Refresh Calendar
				tab.LoadCalendar(tab.currentMonth);
			}
			// Determine date to book
			else
			{
				DateTime recallDate = tab.selectedDate;
				recallDate = recallDate.AddDays(7 * appointment.RecallFlag);

				tab.highlightDay = recallDate;
				tab.highlightOnLoad = true;
				tab.LoadCalendar(recallDate);

				tab.AwaitDateSelection(this, TabScheduling.MODE_RECALL);
			}
		}


		///-------------------------------------------------------------------------------------------------
		/// \fn private void Recall_X_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  Sets the recall flag value for the current appointment to whatever value it should be.
		///				o Clear (0)
		///				o 1, 2, 3 weeks
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  object sender
		/// \param  RoutedEventArgs e
		///-------------------------------------------------------------------------------------------------
		private void Recall_Clear_Click(object sender, RoutedEventArgs e)
		{
			UpdateRecallFlag(0);
		}
		private void Recall_1_Click(object sender, RoutedEventArgs e)
		{
			UpdateRecallFlag(1);
		}
		private void Recall_2_Click(object sender, RoutedEventArgs e)
		{
			UpdateRecallFlag(2);
		}
		private void Recall_3_Click(object sender, RoutedEventArgs e)
		{
			UpdateRecallFlag(3);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void UpdateRecallFlag(int weeks)
		///
		/// \brief  Updates the appointment recall value to the given value and updates the database after
		///			setting it
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  int weeks : the number of weeks the recall should be set to
		///-------------------------------------------------------------------------------------------------
		private void UpdateRecallFlag(int weeks)
		{
			// Update the recall flag for the appointment
			appointment.RecallFlag = weeks;
			appointment.Update();
			TabScheduling.tabScheduling.LoadCalendar(TabScheduling.tabScheduling.currentMonth);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void Billing_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  Opens the billing screen when the context menu item is clicked
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  object sender
		/// \param  MouseButtonEventArgs e
		///-------------------------------------------------------------------------------------------------
		private void Billing_Click(object sender, RoutedEventArgs e)
		{
			// Open window to edit billing information
			PopupMenus.BillingDetails billing = new PopupMenus.BillingDetails(appointment);
			billing.ShowDialog();

			// Reload calendar after updating database
			TabScheduling.tabScheduling.LoadCalendar(TabScheduling.tabScheduling.currentMonth);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void Change_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  Updates the UI to prepare for a date change request for the object's appointment
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  object sender
		/// \param  RoutedEventArgs e
		///-------------------------------------------------------------------------------------------------
		private void Change_Click(object sender, RoutedEventArgs e)
		{
			TabScheduling tab = TabScheduling.tabScheduling;
			tab.SetInformation("Choose a new date for the appointment");
			tab.AwaitDateSelection(this, TabScheduling.MODE_CHANGE);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void Delete_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  Delets the appointment if the user confirms that they really want to
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  object sender
		/// \param  RoutedEventArgs e
		///-------------------------------------------------------------------------------------------------
		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxResult result = MessageBox.Show(
				"Are you sure you want to delete this appointment?", 
				"Delete Appointment", 
				MessageBoxButton.YesNo, 
				MessageBoxImage.Warning
			);

			// Delete the appointment
			if (result == MessageBoxResult.Yes)
			{
				appointment.Delete();
			}

			// Reload calendar after updating database
			TabScheduling.tabScheduling.LoadCalendar(TabScheduling.tabScheduling.currentMonth);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void RequestDateChange(DateTime date)
		///
		/// \brief  Handles the event when the CalendarDay is clicked during a date change. The new
		///			date cannot be before today's date, and it must be available for booking
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  DateTime date : The new date request
		///-------------------------------------------------------------------------------------------------
		public void RequestDateChange(DateTime date)
		{
			// Change the appointment date
			if (date != appointment.Date)
			{
				if (date >= DateTime.Today)
				{
					appointment.Date = date;
					appointment.Update();

					TabScheduling.tabScheduling.LoadCalendar(date);
				}
				else
				{
					TabScheduling.tabScheduling.SetInformation("You cannot change the appointment to a date in the past");
				}
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void RequestBookFromFlag(DateTime requestedDate)
		///
		/// \brief  Handles the event when the CalendarDay is clicked during a recall choice stage.
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  DateTime requestedDate
		///-------------------------------------------------------------------------------------------------
		public void RequestBookFromFlag(DateTime requestedDate)
		{
			TabScheduling tab = TabScheduling.tabScheduling;

			tab.highlightOnLoad = false;

			if (SchedulingSupport.GetAppointmentsForDay(requestedDate).Count < SchedulingSupport.MaxAppointmentsForDay(requestedDate))
			{
				SchedulingSupport.BookAppointment(patients[0], patients[1], requestedDate);
				appointment.RecallFlag = 0;
				appointment.Update();
			}

			// Clear instructions
			tab.ClearInformation();

			// Refresh Calendar
			tab.LoadCalendar(tab.selectedDate);
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void GrdAppointmentDetails_MouseEnter(object sender, MouseEventArgs e)
		///
		/// \brief  Highlights to indicate the area can be clicked if it should be
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  object sender
		/// \param  MouseEventArgs e
		///-------------------------------------------------------------------------------------------------
		private void GrdAppointmentDetails_MouseEnter(object sender, MouseEventArgs e)
		{
			if (clickable)
			{
				border.BorderThickness = new Thickness(2, 2, 2, 2);
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void GrdAppointmentDetails_MouseLeave(object sender, MouseEventArgs e)
		///
		/// \brief  Un-Highlights to indicate the area cannot be clicked if it shouldn't be
		///
		/// \author Bailey
		/// \date   2019-04-18
		/// 
		/// \param  object sender
		/// \param  MouseEventArgs e
		///-------------------------------------------------------------------------------------------------
		private void GrdAppointmentDetails_MouseLeave(object sender, MouseEventArgs e)
		{
			border.BorderThickness = new Thickness(1, 1, 1, 1);
		}
	}
}
