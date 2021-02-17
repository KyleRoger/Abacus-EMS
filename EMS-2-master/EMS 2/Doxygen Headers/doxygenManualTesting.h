/*! \page SchedulingManualTesting Manual Testing - Scheduling
  \tableofcontents
  \section SchedulingTesting Scheduling
  The methods in the Scheduling_Graphics class often heavily rely on user
  input, and console output. These tests walk through the situations a user be in when operating
  the calendar in order to test them.



  <hr>   



  \subsection CalendarNavigationTesting Calendar Navigation Testing
  <table>
	  <tr>
		  <th><b> Name: </b></th>
		  <th><b> Purpose: </b></th>
		  <th><b> Description: </b></th>
		  <th><b> Type: </b></th>
		  <th><b> Sample Data Sets: </b></th>
		  <th><b> Expected Results: </b></th>
		  <th><b> Actual Results: </b></th>
	  </tr>

	  <tr>
		  <td>
			PrintAppointmentsForMonth(...) - Printing the Appointment Tile Symbol
		  </td>
		  <td>
			Tests whether the calendar is able to display how many appointments there are for all 
			days on the currently focused month. Tests the following additional methods:
				- MaxAppointmentsForDay(...): Used in the calculation to determine the colour of the tiles for a given day
		  </td>
		  <td>
			Manually navigate to the main calendar screen. Is the database being populated across
			each day in the calendar?
		  </td>
		  <td>
			Functional
		  </td>
		  <td>
			Appointment database is populated with random data at the program startup
		  </td>
		  <td>
			Each day of the month should have either no appointment tiles, or that they have a properly 
			coloured set of tiles, and no more than the limit for that day of the week.
			<i>	(\b Green = 2 or more appointment slots available,
				 \b Yellow = 1 appointment slot remaining, 
				 \b Red = No more appointment slots)	</i>
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>

	  <tr>
		  <td>
			ReceivedKeyboardInput(...) - Arrow Keys to Change the Selected Day
		  </td>
		  <td>
			Tests whether the ReceivedKeyboardInput() function is able take the arrow key user input
			sent from the parallel process. Upon receiving arrow key input, the selected day in the 
			calendar should reflect this input. Tests the following additional methods:
				- ChangeCurrentDay(...) : Receives the number of days the selected date should adjust by 
					<i> (-7 / -1 for UP / LEFT arrow; 7 / 1 for DOWN / RIGHT arrow) </i>
				- UpdateSelectedHighlight(...) : Removes the highlight around the previous day and highlights
					the new day that the user is trying to navigate to
				- ListAppointmentsForSelectedDay(...) : Lists the appointment details for the current day
					beside the calendar. This list should upate every time the user changes the selected day.
		  </td>
		  <td>
			Manually press the arrow keys and observe whether the selected day accurately updates when
			the user tries to change the selected day.
		  </td>
		  <td>
			Functional
		  </td>
		  <td>
			Appointment database is populated with random data at the program startup
		  </td>
		  <td>
			The previous highlight should no longer be present, and the detailed list of appointments 
			for the selected day should update each time a new selection is made.
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>

	  <tr>
		  <td>
			ReceivedKeyboardInput(...) - Arrow Keys to Change the Selected Month
		  </td>
		  <td>
			Tests whether the ReceivedKeyboardInput() function is able to change the calendar to a new
			month if the user is requesting to leave the current month. Tests the following additional methods:
				- ChangeCurrentDay(...) : Receives the number of days the selected date should adjust by
					<i> (-7 / -1 for UP / LEFT arrow; 7 / 1 for DOWN / RIGHT arrow) </i>
				- UpdateSelectedHighlight(...) : Removes the highlight around the previous day and highlights
					the new day that the user is trying to navigate to
				- ListAppointmentsForSelectedDay(...) : Lists the appointment details for the current day
					beside the calendar. This list should upate every time the user changes the selected day.
				- PrintCalendar(...) : Prints the skeleton of the new month, containing the days of the week,
					numbered days in the calendar, and the correct number of rows to fit the calendar.
				- PrintControls() : After PrintCalendar(...) has executed, the console height (y-position) that
					the controls should be printed at should change to reflect a position below the bottom of
					the new calendar.
				- PrintMonthHeader(...) : The month name and year should reflect the change the user made
					to the calendar display.
		  </td>
		  <td>
			Manually press the arrow keys in an attempt to leave the current month.
		  </td>
		  <td>
			Boundary
		  </td>
		  <td>
			Appointment database is populated with random data at the program startup
		  </td>
		  <td>
			Upon pressing an arrow key that would seemingly take the user out of the current month, the 
			screen should clear and do a full update, loading up a new calendar table with all the details
			a calendar should have (numbered days, appointments, a month title)
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>

  </table>





  <br>
  




  \subsection AppointmentNavigationTesting Appointment Navigation Testing
  <table>
	  <tr>
		  <th><b> Name: </b></th>
		  <th><b> Purpose: </b></th>
		  <th><b> Description: </b></th>
		  <th><b> Type: </b></th>
		  <th><b> Sample Data Sets: </b></th>
		  <th><b> Expected Results: </b></th>
		  <th><b> Actual Results: </b></th>
	  </tr>

	  <tr>
		  <td>
			ListAppointmentsForSelectedDay(...) - Inspect Appointment List (Normal)
		  </td>
		  <td>
			Tests whether the user is able to inspect a day on the calendar to interact with the appointment list
			  on the right side. Pressing ENTER while in the navigation mode of the calendar should allow the user to
			  focus on the list, and later use the arrow keys to interact with each of the appointments. Once finished,
			  the user must be able to press ESCAPE to go back to the navigation mode of the calendar. Tests the 
			  following additional methods:
				- GetNameFromHCN(...) : Upon entering the list, the name of the person to be highlighted needs to
				be re-printed to the screen in the highlight colour. After giving the HCN (and possibly subHCN) to
				the function, the first and last name can be printed in the list again.
				- PrintControls() : Upon successfully entering the appointment list, the control scheme below
				the calendar should update to reflect the new set of actions the user has access to.
		  </td>
		  <td>
			Manually choose a day on the calendar that has at least one appointment. Press ENTER and observer
			the changes to the right, indicating that the list is now focused on the first appointment in the list.
			Press ESCAPE to exit the inspection mode and return to the calendar navigation mode.
		  </td>
		  <td>
			Functional
		  </td>
		  <td>
			In order for an appointment to be inspected, there must be at least one appointment in the appointment
			database for that day.
		  </td>
		  <td>
			The user should be able to press ENTER to inspect the appointment list, and ESCAPE to back out from that
			mode. Upon inspecting the day, the control scheme should change to show that the currenlty selected
			appointment can be interacted with. Backing out should therefore change the control scheme back to
			what it was in the calendar navigation mode.
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>

	  <tr>
		  <td>
			ListAppointmentsForSelectedDay(...) - Inspect Appointment List (No appointments)
		  </td>
		  <td>
			Tests whether the user is restricted from inspecting the appointments for a day if there are no appointments.
		  </td>
		  <td>
			Manually choose a day on the calendar that has no appointments and press ENTER. There should be no changes to the
			interface, and the user should remain in calendar navigation mode (Pressing arrow keys to move selected day on
			calendar).
		  </td>
		  <td>
			Boundary
		  </td>
		  <td>
			Checks if there are any appointments in the appointment database for that day.
		  </td>
		  <td>
			Nothing should happen when the user presses ENTER on a day with no appointments. Users should see that the list 
			is empty and understand they cannot proceed to interact with something that doesn't exist.
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>

	  <tr>
		  <td>
			ListAppointmentsForSelectedDay(...) - UP / DOWN to Change Selected Appointment
		  </td>
		  <td>
			Tests whether the user is able to move up and down the list of appointments once the list has been properly 
			inspected. 
		  </td>
		  <td>
			Manually inspect an day in the calendar that has one or more appointments. After inspected, press the UP / DOWN
			arrow keys to move the selection UP / DOWN.
		  </td>
		  <td>
			Functional
		  </td>
		  <td>
			In order for an appointment to be inspected and navigated, there must be at least one appointment in the appointment
			database for that day.
		  </td>
		  <td>
			The user should be able to move the highlighted (selected) day up and down the list.
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>

  </table>






  <br>






  \subsection AppointmentInteractionTesting Appointment Interaction Testing
  <table>
	  <tr>
		  <th><b> Name: </b></th>
		  <th><b> Purpose: </b></th>
		  <th><b> Description: </b></th>
		  <th><b> Type: </b></th>
		  <th><b> Sample Data Sets: </b></th>
		  <th><b> Expected Results: </b></th>
		  <th><b> Actual Results: </b></th>
	  </tr>

	  <tr>
		  <td>
			Booking an Appointment - Based on a previous appointment
		  </td>
		  <td>
			Test whether the user is able to navigate the console UI and book an appointment using
			the details of an existing appointment. Tests the following additional methods:
				- GetUserString() : Gets the appointment date string
				- ConfirmAppointment(...) : Formats the appointment date into a question of confirmation
				- GetConfirmation(...) : Requires the user to press Y / N / ESCAPE to confirm appointment booking
		  </td>
		  <td>
			Manually proceed through the menu options in the following order:
				1. Inspect an appointment in the calendar
				2. Press [1], indicating intention to book an appointment
				3. Enter in the date of the appointment ("2018-12-5")
				4. Confirm appointment booking ("y")
		  </td>
		  <td>
			Functional
		  </td>
		  <td>
			An existing appointment to book another appointment for
		  </td>
		  <td>
			The appointment should visible be added to the calendar, and is therefore present in the database.
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>
	  <tr>
		  <td>
			Booking an Appointment - Based on a recall flag
		  </td>
		  <td>
			Tests whether an appointment can be booked based on a pre-existing recall flag. 
		  </td>
		  <td>
			Manually proceed through the menu options in the following order:
				1. Inspect an appointment in the calendar
				2. Press [1] on an appointment that has an active recall request flag
				3. Press "y" indicating the current appointment is for this recall request
				4. Review the list of nearby appointments and choose one
				5. Confirm appointment booking ("y")
		  </td>
		  <td>
			Functional
		  </td>
		  <td>
			An existing appointment with a recall flag set on it
		  </td>
		  <td>
			The recall appointment should be booked, and the recall flag on the original appointment should
			be removed.
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>
	  <tr>
		  <td>
			Booking an Appointment - For an unavailable date
		  </td>
		  <td>
			Test whether the user is able to choose from the list of nearby valid appointment dates when an unavailable date is requested.
				- PrintNearbyList(...) : Prints the list of appointments recovered, calculated to be close to the requested date
				- BookFromNearbyList(...) : Gets the user's appointment choice from the printed list
				- ConfirmAppointment(...) : Formats the appointment date into a question of confirmation
				- GetConfirmation(...) : Requires the user to press Y / N / ESCAPE to confirm appointment booking
		  </td>
		  <td>
			Manually proceed through the menu options in the following order:
				1. Inspect an appointment in the calendar
				2. Press [1], indicating intention to book an appointment
				3. Enter in the date of the appointment ("2018-12-5")
				4. Review the list of nearby appointments and choose one
				5. Confirm appointment booking ("y")
		  </td>
		  <td>
			Exception
		  </td>
		  <td>
			An existing appointment to book another appointment for
		  </td>
		  <td>
			The appointment should visible be added to the calendar, and is therefore present in the database.
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>
	  <tr>
		  <td>
			Booking an Appointment - Cancelling an appointment request
		  </td>
		  <td>
			Test whether the user is able to cancel booking an appointment at the end of the booking process
		  </td>
		  <td>
			Manually proceed through the menu options in the following order:
				1. Inspect an appointment in the calendar
				2. Press [1], indicating intention to book an appointment
				3. Enter in the date of the appointment ("2018-12-5")
				5. Decide to cancel the appointment ("n")
		  </td>
		  <td>
			Boundary
		  </td>
		  <td>
			An existing appointment to book another appointment for
		  </td>
		  <td>
			The user should be taken back to navigation view in the calendar and no appointment should be booked.
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>
	  <tr>
		  <td>
			Flagging an appointment for recall as Doctor
		  </td>
		  <td>
			Test whether a user with doctor permissions is able to modify an appointment flag on an existing appointment.
		  </td>
		  <td>
			Manually proceed through the menu options in the following order:
				1. Inspect an appointment in the calendar while having access to doctor permissions
				2. Press [4], indicating intention to flag the appointment for recall
				3. Type the number of weeks for this flag and press ENTER
		  </td>
		  <td>
			Functional
		  </td>
		  <td>
			An existing appointment
		  </td>
		  <td>
			The appointment details should now list that there is a recall flag for the requested duration on that appointment
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>
	  <tr>
		  <td>
			Flagging an appointment for recall as Receptionist
		  </td>
		  <td>
			Test whether a user without doctor permissions is able to modify an appointment flag on an existing appointment.
		  </td>
		  <td>
			Manually proceed through the menu options in the following order:
				1. Inspect an appointment in the calendar while having access to doctor permissions
				2. Press [4], indicating intention to flag the appointment for recall
		  </td>
		  <td>
			Boundary
		  </td>
		  <td>
			An existing appointment
		  </td>
		  <td>
			Nothing should happen since the user does not have doctor permissions.
		  </td>
		  <td>
			[TBD]
		  </td>
	  </tr>
  </table>



*/