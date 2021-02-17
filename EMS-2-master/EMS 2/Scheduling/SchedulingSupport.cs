using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scheduling;
using Data;

namespace EMS_2.Scheduling
{
	class SchedulingSupport
	{
		public const int MAX_APPOINTMENTS_WEEKDAY = 6;
		public const int MAX_APPOINTMENTS_WEEKEND = 2;

		/**
		* \fn public static int MaxAppointmentsForDay(DateTime day)
		* \author Bailey Mills
        * \brief Determines the max number of appointments a given day could have (6 normally, 2 on weekends)
        * \param DateTime day: the day to check
        * \return int max: the number of appointments that day could have
        */
		public static int MaxAppointmentsForDay(DateTime day)
		{
			int max = MAX_APPOINTMENTS_WEEKDAY;

			// Weekends can only have 2 appointments
			if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
			{
				max = MAX_APPOINTMENTS_WEEKEND;
			}

			return max;
		}



		/**
		* \fn public static List<Appointment> GetAppointmentsForDay(DateTime day)
		* \author Bailey Mills
        * \brief Gets all appointments for a given day and returns a list of Appointments
        * \param DateTime day: the day to check
        * \return List<Appointment>
        */
		public static List<Appointment> GetAppointmentsForDay(DateTime day)
		{
			List<Appointment> appointments = new List<Appointment>();
			if (Database.Appointments.ContainsKey(day))
			{
				appointments = Database.Appointments[day];
			}

			return appointments;
		}



		/**
		* \fn public static int GetAppointmentCountForDay(DateTime day)
		* \author Bailey Mills
        * \brief Gets the number of appointments for a given day through a SQL query
        * \param DateTime day: the day to check
        * \return int count
        */
		public static int GetAppointmentCountForDay(DateTime day)
		{
			int count = 0;

			Database.builder.Length = 0;
			Database.builder.Append("SELECT COUNT(*) FROM Appointment WHERE Date='");
			Database.builder.Append(day.ToShortDateString());
			Database.builder.Append("';");
			count = Convert.ToInt32(Connection.ExecuteQuery(Database.builder.ToString()).Rows[0][0]);

			return count;
		}



		/**
		* \fn public static int GetMobileRequestCount()
		* \author Bailey Mills
        * \brief Gets the number of mobile requests (total)
        * \return int count
        */
		public static int GetMobileRequestCount()
		{
			int count = 0;

			Database.builder.Length = 0;
			Database.builder.Append("SELECT COUNT(*) FROM MobileRequest;");
			count = Convert.ToInt32(Connection.ExecuteQuery(Database.builder.ToString()).Rows[0][0]);

			return count;
		}



		/**
		* \fn public static bool BookAppointment(Demographics.Patient patient, Demographics.Patient hoh, DateTime date)
		* \author Bailey Mills
        * \brief Attempts to book an appointment using all necessary components for a patient. If hoh is null then
		*			then they will not be in the appointment
        * \return int count
        */
		public static bool BookAppointment(Demographics.Patient patient, Demographics.Patient hoh, DateTime date)
		{
			bool booked = true;

			// Ensure there is enough room
			if (GetAppointmentCountForDay(date) < MaxAppointmentsForDay(date))
			{
				// Single Appointment
				if (patient != null && hoh == null)
				{
					String[] attributes = new String[]
					{
						"0",
						date.ToString(),
						patient.HCN
					};

					Appointment_Single appt = new Appointment_Single(attributes);
					appt.Add();
				}
				// Double Appointment
				else if (patient != null && hoh != null)
				{
					String[] attributes = new String[]
					{
						"0",
						date.ToString(),
						patient.HCN,
						hoh.HCN
					};

					Appointment_Double appt = new Appointment_Double(attributes);
					appt.Add();
				}
			}
			else
			{
				booked = false;
			}

			return booked;
		}
	}
}
