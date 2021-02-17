/*
 * 
 * Author:      Kieron Higgs
 * Date:        04-22-2018
 * Project:     EMS 2
 * File:        Database.cs
 * Description: Contains the attributes and methods used to parse SQL data and construct SQL commands or queries in order to request that objects be
 *              created by the Factory classes or to update or request information from the database.
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Support;
using Demographics;
using Scheduling;
using Factory;
using EMS_2.Scheduling;
using System.Diagnostics;

namespace Data
{
	/**
     * 
     * Class:           Database
     * Purpose:         To create SQL queries and commands or parse data resulting from queries and populate the object containers accordingly.
     * Attributes:      Dictionary<String, Patient>
     *                  Dictionary<DateTime, List<Appointments>
     *                  StringBuilder builder
     * Relationships:   Drives the program's ability to represent the actual database with objects and containers. Contains methods used to parse
     *                  data received from the database as well as create the commands necessary to make new ones or update them.
     * Fault detection: Errors will be caught by the try-catch blocks in the Connection class; otherwise, some checking is done to ensure that a
     *                  day's appointments do not exceed the maximum, and checks for pre-existing HCNs in the database.
     */
	public class Database
	{
		// The Patients Dictionary - Key: HCN, Value: Patients Object
		public static Dictionary<String, Patient> Patients = new Dictionary<String, Patient>();

		// The Appointments Dictionary - Key: DateTime, Value: List<Appointment>;
		public static Dictionary<DateTime, List<Appointment>> Appointments = new Dictionary<DateTime, List<Appointment>>();

		// A StringBuilder object used to create SQL queries and commands:
		public static StringBuilder builder = new StringBuilder("", 255);

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Add(Patient_HoH newHoH)
		///
		/// \brief  Adds a head-of-household Patient to the SQL database and in-program object container.
		///         
		/// \description    Parses the attributes held in the passed head-of-household Patient object and submits the necessary commands to the SQL database to
		///                 reflect the addition of a new Patient. Adds a head-of-household Patient to the Patients Dictionary and then adds the corresponding
		///                 records to the Patient and Patient_HoH tables.
		///
		/// \author Kieron Higgs
		///
		/// \param  newHoH    The Patient object which needs its information to be submitted to the database for saving. 
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static void Add(Patient_HoH newHoH)
		{
			Patients.Add(newHoH.HCN, newHoH);

			builder.Length = 0;
			builder.Append(@"INSERT INTO Patient (HCN, LastName, FirstName, MInitial, DoB, Gender) VALUES ('");
			builder.Append(newHoH.HCN);
			builder.Append(@"' , '");
			builder.Append(newHoH.LastName);
			builder.Append(@"' , '");
			builder.Append(newHoH.FirstName);
			builder.Append(@"' , '");
			builder.Append(newHoH.MInitial);
			builder.Append(@"' , '");
			builder.Append(newHoH.DoB);
			builder.Append(@"' , '");
			builder.Append(newHoH.Gender);
			builder.Append(@"');");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"INSERT INTO Patient_HoH (HoH_HCN, AddressLine1, AddressLine2, City, PostalCode, Prov, Phone) VALUES ('"); //Leaving Prov as two spaces if blank. 
			builder.Append(newHoH.HCN);
			builder.Append(@"' , '");
			builder.Append(newHoH.AddressLine1);
			builder.Append(@"' , '");
			builder.Append(newHoH.AddressLine2);
			builder.Append(@"' , '");
			builder.Append(newHoH.City);
			builder.Append(@"' , '");
			builder.Append(newHoH.PostalCode);
			builder.Append(@"' , '");
			builder.Append(newHoH.Province.Substring(0, 2));
			builder.Append(@"' , '");
			builder.Append(newHoH.Phone);
			builder.Append("');");
			Connection.ExecuteCommand(builder.ToString());

            Logging.Write("Patient_HoH.Add(" + newHoH.HCN + ")");
        }

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Add(Patient_Dependant newDependant)
		///
		/// \brief  Adds a dependant Patient to the SQL database and in-program object container.
		///         
		/// \description    Parses the attributes held in the passed dependant Patient object and submits the necessary commands to the SQL database to
		///                 reflect the addition of a new Patient. Adds a dependant Patient to the Patients Dictionary and then adds records to the Patient and 
		///                 Patient_Dependant tables
		///
		/// \author Kieron Higgs
		///
		/// \param  newHoH    The Patient object which needs its information to be submitted to the database for saving. 
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static void Add(Patient_Dependant newDependant)
		{
			Patients.Add(newDependant.HCN, newDependant);

			builder.Length = 0;
			builder.Append(@"INSERT INTO Patient (HCN, LastName, FirstName, MInitial, DoB, Gender) VALUES ('");
			builder.Append(newDependant.HCN);
			builder.Append(@"' , '");
			builder.Append(newDependant.LastName);
			builder.Append(@"' , '");
			builder.Append(newDependant.FirstName);
			builder.Append(@"' , '");
			builder.Append(newDependant.MInitial);
			builder.Append(@"' , '");
			builder.Append(newDependant.DoB);
			builder.Append(@"' , '");
			builder.Append(newDependant.Gender);
			builder.Append(@"');");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"INSERT INTO Patient_Dependant (Dependant_HCN, HoH_HCN) VALUES ('");
			builder.Append(newDependant.HCN);
			builder.Append(@"' , '");
			builder.Append(newDependant.HoH_HCN);
			builder.Append("');");
			Connection.ExecuteCommand(builder.ToString());

            Logging.Write("Patient_HoH.Add(" + newDependant.HCN + ")");
        }

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void ParseImportedPatients(DataTable patient_Table, DataTable dependant_Table, DataTable HoH_Table)
		///
		/// \brief  Adds Patients in-program object container based on imported SQL data.
		///         
		/// \description    Parses the DataTable rows received from the database which pertain to Patients and instantiates the corresponding Patient objects
		///                 based on that data. Loops through the table rows, determines whether a given Patient is a head-of-household or dependant, and then
		///                 uses the Factory to create the required object before passing it into the Patients Dictionary.
		///
		/// \author Kieron Higgs
		///
		/// \param  patient_Table       The table containing demographic data which every Patient object contains: Name, Gender, DoB
		/// \param  dependant_Table     The table containing dependant HCNs and their corresponding head-of-household's HCN
		/// \param  HoH_Table           The table containing HoH data: Address, Phone
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static void ParseImportedPatients(DataTable patient_Table, DataTable dependant_Table, DataTable HoH_Table)
		{
			// Use a list of Strings to initialize each patient.
			List<String> attributesList = new List<String>();
			String HCN = "";
			DataRow[] foundRow;

			// For each row in the table containing base data for every patient...
			foreach (DataRow row in patient_Table.Rows)
			{
				builder.Length = 0;
				builder.Append(@"Dependant_HCN = '");

				// Add each field in that row to the List of attributes (as a String):
				foreach (object field in row.ItemArray)
				{
					attributesList.Add(field.ToString());
				}
				HCN = attributesList[0];

				// Append the expression filter string with the patient's HCN so it reads "Dependant_HCN = '##########AA'":
				builder.Append(HCN);
				builder.Append(@"'");

				// If the dependantTable does not contain the HCN of the current patient as a primary key...
				if ((foundRow = dependant_Table.Select(builder.ToString())).Length != 1)
				{
					// The current patient is a head of household. Append the HoH filter so that it reads "HoH_HCN = '##########AA'":
					builder.Length = 0;
					builder.Append(@"HoH_HCN = '");
					builder.Append(HCN);
					builder.Append(@"'");

					foundRow = HoH_Table.Select(builder.ToString());
					foreach (object field in foundRow[0].ItemArray)
					{
						attributesList.Add(field.ToString());
					}
					// Elimiate the redundant "HoH_HCN" from the attributes List--it is already in the first element of the List:
					attributesList.RemoveAt(6);

					// Reset the HoH filter:
					builder.Length = 11;
				}
				else
				{
					// Otherwise, the current patient is a dependant. Just add the second field (HoH_HCN) to the attributes List:
					attributesList.Add(foundRow[0][1].ToString());
				}

				// Convert the attributes List to a String array and shove it in the PatientFactory and shove the result into the database:
				Database.Patients.Add(HCN, PatientFactory.Create(attributesList.ToArray()));

				// Reset the List and Dependant filter:
				attributesList.Clear();
			}
            Logging.Write("ParseImportedPatients(" + Database.Patients.Count + ")");
        }

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static List<Patient> GetPatientsByAppointmentID(int appointmentID)
		///
		/// \brief  Finds one or two Patients, depending on whether the scheduled appointment is a single or double
		///         
		/// \description    Takes an appointmentID as a parameter and returns a List of Strings containing the HCNs of the patient(s) who will be attending the
		///                 appointment. This method is used to populate the GUI with Patient names and data when browsing the calendar.
		///
		/// \author Kieron Higgs
		///
		/// \param  appointmentID       The ID of a given appointment (an arbitrary integer assigned by the database for tracking purposes)
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		public static List<Patient> GetPatientsByAppointmentID(int appointmentID)
		{
			builder.Length = 0;
			builder.Append(@"SELECT Attendee_HCN FROM Appointment_Attendee WHERE AppointmentID=");
			builder.Append(appointmentID.ToString());
			builder.Append(";\0");

			DataTable queryResult = Connection.ExecuteQuery(builder.ToString());
			List<Patient> results = new List<Patient>();

			foreach (DataRow patient in queryResult.Rows)
			{
				results.Add(Database.Patients[patient["Attendee_HCN"].ToString()]);
			}

			// Re-order the list so that HoH is the second item in the list
			if (results.Count == 2)
			{
				Patient patient1 = results[0];
				Patient patient2 = results[1];

				if (patient1 is Patient_HoH)
				{
					results.Clear();
					results.Add(patient2);
					results.Add(patient1);
				}
			}

            Logging.Write(" GetPatientsByAppointmentID(" + appointmentID.ToString() + ")");

            return results;
		}

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Add(Appointment_Single newAppointment)
		///
		/// \brief  Inserts a new appointment into the database.
		///         
		/// \description    Creates an INSERT statement based on the Appointment_Single object passed to the method in order to make an insertion into the SQL
		///                 database to accurately reflect the new Patient data.
		///
		/// \author Kieron Higgs
		///
		/// \param  Appointment_Single      The Appointment object whose data is to be inserted into the database.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Add(Appointment_Single newAppointment)
		{
			// Make an Appointment table entry with the desired date:
			builder.Length = 0;
			builder.Append("INSERT INTO Appointment (\"Date\", RecallFlag, MinistryFlag, MobileFlag) VALUES ('");
			builder.Append(newAppointment.Date);
			builder.Append(@"', ");
			builder.Append(newAppointment.RecallFlag);
			builder.Append(@", ");
			builder.Append(newAppointment.MinistryFlag);
			builder.Append(@", ");
			builder.Append(newAppointment.MobileFlag);
			builder.Append(@");");
			Connection.ExecuteCommand(builder.ToString());

			if (newAppointment.AppointmentID == 0)
			{
				// The ID was auto-assigned during insert. Retrieve it for use:
				builder.Length = 0;
				builder.Append("SELECT MAX(AppointmentID) FROM Appointment;");
				newAppointment.AppointmentID = Convert.ToInt32(Connection.ExecuteQuery(builder.ToString()).Rows[0][0]);
			}


			// Now make an insertion into the Attendee table with the HCN belonging to the patient:
			builder.Length = 0;
			builder.Append("INSERT INTO Appointment_Attendee (Attendee_HCN, AppointmentID) VALUES ('");
			builder.Append(newAppointment.HCN);
			builder.Append(@"', ");
			builder.Append(newAppointment.AppointmentID);
			builder.Append(@");");
			Connection.ExecuteCommand(builder.ToString());

            AddToContainer(newAppointment);

            Logging.Write("Appointment_Single.Add(" + newAppointment.AppointmentID + ")");

        }

        ///----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn public static void Import(Appointment_Single newAppointment)
        ///
        /// \brief  Adds a newly created Appointment_Single object (one based on SQL data) to the container.
        ///         
        /// \description    Takes a new Appointment_Single object which has been recently instantiated based on data taken from the SQL database and adds it to
        ///                 the in-program container. This method is used to distinguish between "importing" SQL data-based objects and "adding" newly created
        ///                 objects to the database.
        ///
        /// \author Kieron Higgs
        ///
        /// \param  Appointment_Single      The Appointment object whose data is to be inserted into the container.
        ///----------------------------------------------------------------------------------------------------------------------------------------------------
        public static void Import(Appointment_Single newAppointment)
		{
			AddToContainer(newAppointment);
		}

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void AddToContainer(Appointment_Single newAppointment)
		///
		/// \brief  Adds an Appointment_Single object to the Dictionary container.
		///         
		/// \description    Takes a new Appointment_Single object and adds it to the Appointments Dictionary, checking that the maximum number of Appointments
		///                 has not been exceeded in the process.
		///
		/// \author Kieron Higgs
		///
		/// \param  Appointment_Single      The Appointment object whose data is to be inserted into the container.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		public static void AddToContainer(Appointment_Single newAppointment)
		{
			// Insert the new appointment into the in-program container:
			if (Appointments.ContainsKey(newAppointment.Date))
			{
				Appointments[newAppointment.Date].Add(newAppointment);
			}
			else
			{
				List<Appointment> newList = new List<Appointment>();
				newList.Add(newAppointment);
				Appointments.Add(newAppointment.Date, newList);
			}
		}

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Add(Appointment_Double newAppointment)
		///
		/// \brief  Inserts a new appointment into the database.
		///         
		/// \description    Creates an INSERT statement based on the Appointment_Double object passed to the method in order to make an insertion into the SQL
		///                 database to accurately reflect the new Patient data.
		///
		/// \author Kieron Higgs
		///
		/// \param  Appointment_Double      The Appointment object whose data is to be inserted into the database.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Add(Appointment_Double newAppointment)
		{
			// Make an Appointment table entry with the desired date:
			builder.Length = 0;
			builder.Append("INSERT INTO Appointment (\"Date\", RecallFlag, MinistryFlag, MobileFlag) VALUES ('");
			builder.Append(newAppointment.Date);
			builder.Append(@"', ");
			builder.Append(newAppointment.RecallFlag);
			builder.Append(@", ");
			builder.Append(newAppointment.MinistryFlag);
			builder.Append(@", ");
			builder.Append(newAppointment.MobileFlag);
			builder.Append(@");");
			Connection.ExecuteCommand(builder.ToString());

			if (newAppointment.AppointmentID == 0)
			{
				// The ID was auto-assigned during insert. Retrieve it for use:
				builder.Length = 0;
				builder.Append("SELECT MAX(AppointmentID) FROM Appointment;");
				newAppointment.AppointmentID = Convert.ToInt32(Connection.ExecuteQuery(builder.ToString()).Rows[0][0]);
			}

			// Now make an insertion into the Attendee table with the HCN belonging to the patient:
			builder.Length = 0;
			builder.Append("INSERT INTO Appointment_Attendee (Attendee_HCN, AppointmentID) VALUES ('");
			builder.Append(newAppointment.HCN);
			builder.Append(@"' , ");
			builder.Append(newAppointment.AppointmentID);
			builder.Append(@");");
			Connection.ExecuteCommand(builder.ToString());

			// Now make an insertion into the Attendee table with the HCN belonging to the patient:
			builder.Length = 0;
			builder.Append("INSERT INTO Appointment_Attendee (Attendee_HCN, AppointmentID) VALUES ('");
			builder.Append(newAppointment.SecondaryHCN);
			builder.Append(@"' , ");
			builder.Append(newAppointment.AppointmentID);
			builder.Append(@");");
			Connection.ExecuteCommand(builder.ToString());

			AddToContainer(newAppointment);

            Logging.Write("Appointment_Double.Add(" + newAppointment.AppointmentID + ")");
        }

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Import(Appointment_Double newAppointment)
		///
		/// \brief  Adds a newly created Appointment_Double object (one based on SQL data) to the container.
		///         
		/// \description    Takes a new Appointment_Double object which has been recently instantiated based on data taken from the SQL database and adds it to
		///                 the in-program container. This method is used to distinguish between "importing" SQL data-based objects and "adding" newly created
		///                 objects to the database.
		///
		/// \author Kieron Higgs
		///
		/// \param  Appointment_Double      The Appointment object whose data is to be inserted into the container.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		public static void Import(Appointment_Double newAppointment)
		{
			AddToContainer(newAppointment);
		}

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void AddToContainer(Appointment_Double newAppointment)
		///
		/// \brief  Adds an Appointment_Double object to the Dictionary container.
		///         
		/// \description    Takes a new Appointment_Double object and adds it to the Appointments Dictionary, checking that the maximum number of Appointments
		///                 has not been exceeded in the process.
		///
		/// \author Kieron Higgs
		///
		/// \param  Appointment_Double      The Appointment object whose data is to be inserted into the container.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		public static void AddToContainer(Appointment_Double newAppointment)
		{
			// Insert the new appointment into the in-program container:
			if (Appointments.ContainsKey(newAppointment.Date))
			{
				Appointments[newAppointment.Date].Add(newAppointment);
			}
			else
			{
				List<Appointment> newList = new List<Appointment>();
				newList.Add(newAppointment);
				Appointments.Add(newAppointment.Date, newList);
			}
		}

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void ParseImportedAppointments(DataTable appointment_Table, DataTable attendee_Table, DataTable billcode_Table)
		///
		/// \brief  Adds Appointments to the in-program object container based on imported SQL data.
		///         
		/// \description    Parses the DataTable rows received from the database which pertain to Appointments and instantiates the corresponding Appointment 
		///                 objects based on that data. Loops through the table rows, determines whether a given Appointment is a single or double and whether
		///                 it has any billcodes associated with it, and then uses the Factory to create the required object before passing it into the
		///                 Appointments Dictionary.
		///
		/// \author Kieron Higgs
		///
		/// \param  appointment_Table   The table containing Appointment IDs and Dates
		/// \param  attendee_table      The table containing the HCNs of Patients and the ID of the Appointment they are attending
		/// \param  billcode_Table      The table containing the billcodes pertaining to a certain Patient who attended an Appointment
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		public static void ParseImportedAppointments(DataTable appointment_Table, DataTable attendee_Table, DataTable billcode_Table)
		{
			List<String> attributesList = new List<String>();
			List<String> billcodes = new List<String>();
			List<String> secondaryBillcodes = new List<String>();
			String AppointmentID = "";
			String HCN = "";
			String secondaryHCN = "";
			DataRow[] foundRows;

			// For each row in the table containing base data for every appointment...
			foreach (DataRow row in appointment_Table.Rows)
			{
				// Add each field in that row to the List of attributes (as a String):
				foreach (object field in row.ItemArray)
				{
					attributesList.Add(field.ToString());
				}
				AppointmentID = attributesList[0];

				// Append the expression filter string with the appointment's ID so it reads "AppointmentID = '#######'":
				builder.Length = 0;
				builder.Append(@"AppointmentID = '");
				builder.Append(AppointmentID);
				builder.Append(@"'");

				// If the attendee table only contains one matching record, then the appointment is for one patient.
				if ((foundRows = attendee_Table.Select(builder.ToString())).Length == 1)
				{
					// Add the single attendee's HCN to the attributes List:
					HCN = foundRows[0][0].ToString();
					attributesList.Add(HCN);

					// Append the expression filter string with the patient's HCN so it reads "Attendee_HCN = '##########AA'":
					builder.Append(@" AND Attendee_HCN = '");
					builder.Append(HCN);
					builder.Append(@"'");

					// If there are any corresponding billcodes in the billcode table, add them to the billcodes List:
					if ((foundRows = billcode_Table.Select(builder.ToString())).Length > 0)
					{
						foreach (DataRow billcodeRecord in foundRows)
						{
							billcodes.Add(billcodeRecord[2].ToString());
						}
					}

					// Call the factory to make the Appointment object and put it in the Dictionary:
					AppointmentFactory.Create(attributesList.ToArray(), billcodes).Import();
				}
				// If there are more than one record in the attendee table with the given Appointment ID, then the appointment
				// is for two patients.
				else
				{
					// Grab the two HCNs and put them in the attributes List.
					HCN = foundRows[0][0].ToString();
					attributesList.Add(HCN);
					secondaryHCN = foundRows[1][0].ToString();
					attributesList.Add(secondaryHCN);

					// Append the ID filter to include the HCN in order to find billcodes which correspond with the correct patient:
					builder.Append(@" AND Attendee_HCN = '");
					builder.Append(HCN);
					builder.Append(@"'");

					// If there are any billcodes for the first patient, add them to the billcodes List:
					if ((foundRows = billcode_Table.Select(builder.ToString())).Length > 0)
					{
						foreach (DataRow billcodeRecord in foundRows)
						{
							billcodes.Add(billcodeRecord[2].ToString());
						}
					}

					// Reset the ID filter so it reads "AppointmentID = '#######' AND Attendee_HCN = '" again, then append it with the second HCN:
					builder.Length = 0;
					builder.Append(@"AppointmentID = '");
					builder.Append(AppointmentID);
					builder.Append(@"' AND Attendee_HCN = '");
					builder.Append(secondaryHCN);
					builder.Append(@"'");

					// If there are any billcodes for the second patient, add them to the second billcodes List:
					if ((foundRows = billcode_Table.Select(builder.ToString())).Length > 0)
					{
						foreach (DataRow billcodeRecord in foundRows)
						{
							secondaryBillcodes.Add(billcodeRecord[2].ToString());
						}
					}

					// Convert the attributes List to a String array and shove it in the PatientFactory and shove the result into the database:
					AppointmentFactory.Create(attributesList.ToArray(), billcodes, secondaryBillcodes).Import();
					secondaryBillcodes.Clear();
				}

				// Reset the Lists and ID filter:
				attributesList.Clear();
				billcodes.Clear();
			}

            Logging.Write("ParseImportedAppointments(" + Database.Appointments.Count + ")");
        }

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Update(Patient_HoH updatedHoH)
		///
		/// \brief  Updates the SQL database to reflect changed information pertaining to a head-of-household Patient.
		///         
		/// \description    Used to update the SQL database with the attributes of a Patient_HoH object/Appointment. Takes a head-of-household object with the
		///                 updated attributes and constructs the SQL command needed to submit the changes to the database.
		///
		/// \author Kieron Higgs
		///
		/// \param  updatedHoH      The updated Patient_HoH object whose changes need to be reflected in the SQL database.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static void Update(Patient_HoH updatedHoH)
		{
			builder.Length = 0;
			builder.Append(@"UPDATE Patient SET LastName='");
			builder.Append(updatedHoH.LastName);
			builder.Append(@"' , FirstName='");
			builder.Append(updatedHoH.FirstName);
			builder.Append(@"' , MInitial='");
			builder.Append(updatedHoH.MInitial);
			builder.Append(@"' , DoB='");
			builder.Append(updatedHoH.DoB);
			builder.Append(@"' , Gender='");
			builder.Append(updatedHoH.Gender);
			builder.Append(@"' WHERE HCN='");
			builder.Append(updatedHoH.HCN);
			builder.Append("';\0");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"UPDATE Patient_HoH SET HoH_HCN='");
			builder.Append(updatedHoH.HCN);
			builder.Append(@"' , AddressLine1='");
			builder.Append(updatedHoH.AddressLine1);
			builder.Append(@"' , AddressLine2='");
			builder.Append(updatedHoH.AddressLine2);
			builder.Append(@"' , City='");
			builder.Append(updatedHoH.City);
			builder.Append(@"' , PostalCode='");
			builder.Append(updatedHoH.PostalCode);
			builder.Append(@"' , Prov='");
			builder.Append(updatedHoH.Province);
			builder.Append(@"' , Phone='");
			builder.Append(updatedHoH.Phone);
			builder.Append(@"' WHERE HoH_HCN='");
			builder.Append(updatedHoH.HCN);
			builder.Append("';\0");
			Connection.ExecuteCommand(builder.ToString());

            Logging.Write("Patient_HoH.Update(" + updatedHoH.HCN + ")");
        }

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Update(Patient_Dependant updatedDependant)
		///
		/// \brief  Updates the SQL database to reflect changed information pertaining to a dependant Patient.
		///         
		/// \description    Used to update the SQL database with the attributes of a Patient_Dependant object/Appointment. Takes a head-of-household object 
		///                 with the updated attributes and constructs the SQL command needed to submit the changes to the database.
		///
		/// \author Kieron Higgs
		///
		/// \param  updatedDependant     The updated Patient_Dependant object whose changes need to be reflected in the SQL database.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static void Update(Patient_Dependant updatedDependant)
		{
			builder.Length = 0;
			builder.Append(@"UPDATE Patient SET LastName='");
			builder.Append(updatedDependant.LastName);
			builder.Append(@"' , FirstName='");
			builder.Append(updatedDependant.FirstName);
			builder.Append(@"' , MInitial='");
			builder.Append(updatedDependant.MInitial);
			builder.Append(@"' , DoB='");
			builder.Append(updatedDependant.DoB);
			builder.Append(@"' , Gender='");
			builder.Append(updatedDependant.Gender);
			builder.Append(@"' WHERE HCN='");
			builder.Append(updatedDependant.HCN);
			builder.Append("';\0");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"UPDATE Patient_Dependant SET HoH_HCN='");
			builder.Append(updatedDependant.HoH_HCN);
			builder.Append(@"' WHERE Dependant_HCN='");
			builder.Append(updatedDependant.HCN);
			builder.Append("';\0");
            Connection.ExecuteCommand(builder.ToString());

            Logging.Write("Patient_Dependant.Update(" + updatedDependant.HCN + ")");
        }

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Update(Appointment_Single updatedAppointment)
		///
		/// \brief   Updates an Appointment record in the database with new data--for adding billcodes and/or changing dates.
		///         
		/// \description    Used to reflect in-program changes to Appointment_Single objects within the SQL database.
		///
		/// \author Kieron Higgs
		///
		/// \param  updatedAppointment      The Appointment which has had its attributes changed or updated.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static void Update(Appointment_Single updatedAppointment)
		{
			// Query database for old date in order to change container contents
			builder.Length = 0;
			builder.Append(@"SELECT Date FROM Appointment WHERE AppointmentID=");
			builder.Append(updatedAppointment.AppointmentID);
			builder.Append(";");
			DataTable queryResult = Connection.ExecuteQuery(builder.ToString());
			DateTime previousDate = DateTime.Parse(queryResult.Rows[0][0].ToString());
			DateTime newDate = updatedAppointment.Date;

			if (previousDate != updatedAppointment.Date)
			{
				foreach (Appointment appointment in Database.Appointments[previousDate])
				{
					if (appointment.AppointmentID == updatedAppointment.AppointmentID)
					{
						Database.Appointments[previousDate].Remove(appointment);
						updatedAppointment.Date = previousDate;
						updatedAppointment.Delete();
						break;
					}
				}
				updatedAppointment.Date = newDate;
				updatedAppointment.AppointmentID = 0;
				updatedAppointment.Add();
			}

			builder.Length = 0;
			builder.Append(@"UPDATE Appointment SET Date='");
			builder.Append(updatedAppointment.Date);
			builder.Append(@"', RecallFlag=");
			builder.Append(updatedAppointment.RecallFlag);
			builder.Append(@", MinistryFlag=");
			builder.Append(updatedAppointment.MinistryFlag);
			builder.Append(@", MobileFlag=");
			builder.Append(updatedAppointment.MobileFlag);
			builder.Append(@" WHERE AppointmentID=");
			builder.Append(updatedAppointment.AppointmentID);
			builder.Append(";");

			Connection.ExecuteCommand(builder.ToString());

			if (updatedAppointment.Billcodes.Count > 0)
			{
				UpdateBillcodes(updatedAppointment.AppointmentID, updatedAppointment.HCN, updatedAppointment.Billcodes);
			}

            Logging.Write("Appointment_Single.Update(" + updatedAppointment.AppointmentID + ")");
        }

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Update(Appointment_Double updatedAppointment)
		///
		/// \brief   Updates an Appointment record in the database with new data--for adding billcodes and/or changing dates.
		///         
		/// \description    Used to reflect in-program changes to Appointment_Double objects within the SQL database.
		///
		/// \author Kieron Higgs
		///
		/// \param  updatedAppointment      The Appointment which has had its attributes changed or updated.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static void Update(Appointment_Double updatedAppointment)
		{
			// Query database for old date in order to change container contents
			builder.Length = 0;
			builder.Append(@"SELECT Date FROM Appointment WHERE AppointmentID=");
			builder.Append(updatedAppointment.AppointmentID);
			builder.Append(";");
			DataTable queryResult = Connection.ExecuteQuery(builder.ToString());
			DateTime previousDate = DateTime.Parse(queryResult.Rows[0][0].ToString());
			DateTime newDate = updatedAppointment.Date;

			if (previousDate != updatedAppointment.Date)
			{
				foreach (Appointment appointment in Database.Appointments[previousDate])
				{
					if (appointment.AppointmentID == updatedAppointment.AppointmentID)
					{
						Database.Appointments[previousDate].Remove(appointment);
						updatedAppointment.Date = previousDate;
						updatedAppointment.Delete();
						break;
					}
				}
				updatedAppointment.Date = newDate;
				updatedAppointment.AppointmentID = 0;
				updatedAppointment.Add();
			}

			builder.Length = 0;
			builder.Append(@"UPDATE Appointment SET Date='");
			builder.Append(updatedAppointment.Date);
			builder.Append(@"', RecallFlag=");
			builder.Append(updatedAppointment.RecallFlag);
			builder.Append(@", MinistryFlag=");
			builder.Append(updatedAppointment.MinistryFlag);
			builder.Append(@", MobileFlag=");
			builder.Append(updatedAppointment.MobileFlag);
			builder.Append(@" WHERE AppointmentID=");
			builder.Append(updatedAppointment.AppointmentID);
			builder.Append(";");
			Connection.ExecuteCommand(builder.ToString());

			if (updatedAppointment.Billcodes.Count > 0)
			{
				UpdateBillcodes(updatedAppointment.AppointmentID, updatedAppointment.HCN, updatedAppointment.Billcodes);
			}
			if (updatedAppointment.SecondaryBillcodes.Count > 0)
			{
				UpdateBillcodes(updatedAppointment.AppointmentID, updatedAppointment.SecondaryHCN, updatedAppointment.SecondaryBillcodes);
			}

            Logging.Write("Appointment_Double.Update(" + updatedAppointment.AppointmentID + ")");
        }

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void UpdateBillcodes(int AppointmentID, String Attendee_HCN, List<String> billcodes)
		///
		/// \brief  Updates the SQL database with a new list of billcodes pertaining to a certain Patient/Appointment.
		///         
		/// \description    Used to update or add billcodes to a Patient/Appointment. Takes a List of Strings representing the billcodes (either containing the
		///                 first billcode to be applied to an Appointment or an updated list) and createes the SQL command necessary to reflect the updated
		///                 billcodes in the database.
		///
		/// \author Kieron Higgs
		///
		/// \param  AppointmentID   The ID of the Appointment which pertains to the given list of billcodes.
		/// \param  Attendee_HCN    The HCN of the Patient to whom the billcodes apply.
		/// \param  billcodes       The billcodes to be submitted to the database.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static void UpdateBillcodes(int AppointmentID, String Attendee_HCN, List<String> billcodes)
		{
			builder.Length = 0;
			builder.Append(@"DELETE FROM Appointment_Billcode WHERE AppointmentID=");
			builder.Append(AppointmentID);
			builder.Append(" AND Attendee_HCN='");
			builder.Append(Attendee_HCN);
			builder.Append("';");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"INSERT INTO Appointment_Billcode (AppointmentID, Attendee_HCN, Code) VALUES ");
			foreach (String Code in billcodes)
			{
				builder.Append("(");
				builder.Append(AppointmentID);
				builder.Append(", '");
				builder.Append(Attendee_HCN);
				builder.Append("', '");
				builder.Append(Code);
				builder.Append("'),");
			}
			builder.Length = builder.Length - 1;
			builder.Append(";");
			Connection.ExecuteCommand(builder.ToString());
        }

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void UpdateMinistryFlag(String givenHCN, DateTime givenDate, int flagValue)
		///
		/// \brief  Updates the SQL database with a flag indicating that it requires review.
		///         
		/// \description    Used to update the Ministry of Health recall flag in-program and in the database.
		///
		/// \author Kieron Higgs
		///
		/// \param  givenHCN   The ID of the Appointment which pertains to the given list of billcodes.
		/// \param  givenDate    The HCN of the Patient to whom the billcodes apply.
		/// \param  flagValue       The billcodes to be submitted to the database.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static void UpdateMinistryFlag(String givenHCN, DateTime givenDate, int flagValue)
		{
			builder.Length = 0;
			builder.Append(@"UPDATE Appointment SET MinistryFlag=");
			builder.Append(flagValue.ToString());
			builder.Append(@" FROM Appointment INNER JOIN Appointment_Attendee ON Appointment.AppointmentID=Appointment_Attendee.AppointmentID WHERE Appointment.Date='");
			builder.Append(givenDate.ToString());
			builder.Append("' AND Appointment_Attendee.Attendee_HCN='");
			builder.Append(givenHCN);
			builder.Append("';");
			Connection.ExecuteCommand(builder.ToString());

			foreach (Appointment appointment in Database.Appointments[givenDate])
			{
				if (appointment.Date == givenDate)
				{
					List<Patient> attendees = GetPatientsByAppointmentID(appointment.AppointmentID);
					{
						foreach (Patient attendee in attendees)
						{
							if (attendee.HCN == givenHCN)
							{
								appointment.MinistryFlag = flagValue;
							}
						}
					}
				}
			}
        }

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Delete(Appointment givenAppointment)
		///
		/// \brief  Removes an Appointment from the database.
		///         
		/// \description    Used to delete an Appointment's record from the database and the in-program container.
		///
		/// \author Kieron Higgs
		///
		/// \param  givenAppointment    The appointment to be deleted.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static void Delete(Appointment_Single givenAppointment)
		{
			if (givenAppointment.Billcodes.Count > 0)
			{
				builder.Length = 0;
				builder.Append(@"DELETE FROM Appointment_Billcode WHERE AppointmentID=");
				builder.Append(givenAppointment.AppointmentID);
				builder.Append(";");
				Connection.ExecuteCommand(builder.ToString());
			}

			// Delete attendee info
			builder.Length = 0;
			builder.Append(@"DELETE FROM Appointment_Attendee WHERE AppointmentID=");
			builder.Append(givenAppointment.AppointmentID);
			builder.Append(";");
			Connection.ExecuteCommand(builder.ToString());

			// Delete appointment info
			builder.Length = 0;
			builder.Append(@"DELETE FROM Appointment WHERE AppointmentID=");
			builder.Append(givenAppointment.AppointmentID);
			builder.Append(";");
			Connection.ExecuteCommand(builder.ToString());

			Appointments[givenAppointment.Date].Remove(givenAppointment);
		}

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Delete(Appointment givenAppointment)
		///
		/// \brief  Removes an Appointment from the database.
		///         
		/// \description    Used to delete an Appointment's record from the database and the in-program container.
		///
		/// \author Kieron Higgs
		///
		/// \param  givenAppointment    The appointment to be deleted.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static void Delete(Appointment_Double givenAppointment)
		{
			if (givenAppointment.Billcodes.Count > 0 || givenAppointment.SecondaryBillcodes.Count > 0)
			{
				builder.Length = 0;
				builder.Append(@"DELETE FROM Appointment_Billcode WHERE AppointmentID=");
				builder.Append(givenAppointment.AppointmentID);
				builder.Append(";");
				Connection.ExecuteCommand(builder.ToString());
			}

			// Delete attendee info
			builder.Length = 0;
			builder.Append(@"DELETE FROM Appointment_Attendee WHERE AppointmentID=");
			builder.Append(givenAppointment.AppointmentID);
			builder.Append(";");
			Connection.ExecuteCommand(builder.ToString());

			// Delete appointment info
			builder.Length = 0;
			builder.Append(@"DELETE FROM Appointment WHERE AppointmentID=");
			builder.Append(givenAppointment.AppointmentID);
			builder.Append(";");
			Connection.ExecuteCommand(builder.ToString());

			Appointments[givenAppointment.Date].Remove(givenAppointment);
		}

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void HoH_Exists(String HoH_HCN)
		///
		/// \brief   Checks whether an HoH HCN is present in the database.
		///         
		/// \description    Used to ascertain whether a given HCN is pre-existing in the database and corresponds to a head-of-household Patient. This is done
		///                 for the purposes of validating the form used to add new Patients.
		///
		/// \author Kieron Higgs
		///
		/// \param  HoH_HCN      The HCN of the supposed head-of-household whose HCN has been typed into the Patient addition form.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static bool HoH_Exists(String HoH_HCN)
		{
			builder.Length = 0;
			builder.Append(@"SELECT 1 FROM Patient_HoH WHERE HoH_HCN='");
			builder.Append(HoH_HCN);
			builder.Append("';");
			DataTable queryResult = Connection.ExecuteQuery(builder.ToString());

			if (queryResult.Rows.Count == 0)
			{
				return false;
			}
			return true;
		}

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void HoH_GetAutoFill(String HoH_HCN)
		///
		/// \brief   Retrieves head-of-household data.
		///         
		/// \description    Used to grab the head-of-household data (Address, Phone) which corresponds to a head-of-household Patient for the purposes of
		///                 "auto filling" a new Patient form.
		///
		/// \author Kieron Higgs
		///
		/// \param  HoH_HCN      The HCN of the supposed head-of-household whose HCN has been typed into the Patient addition form.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static List<String> HoH_GetAutoFill(String HoH_HCN)
		{
			builder.Length = 0;
			builder.Append(@"SELECT AddressLine1, AddressLine2, City, PostalCode, Prov, Phone FROM Patient_HoH WHERE HoH_HCN='");
			builder.Append(HoH_HCN);
			builder.Append("';");
			DataTable queryResult = Connection.ExecuteQuery(builder.ToString());

			List<String> result = new List<string>();

			foreach (object field in queryResult.Rows[0].ItemArray)
			{
				result.Add(field.ToString());
			}
			return result;
		}

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void HoH_To_Dependant(String[] attributes)
		///
		/// \brief  Changes an HoH Patient to Dependant.
		///         
		/// \description    Takes the attributes intended for a new Dependant object and attempts to modify the container and dataabase to reflect the changes.
		///
		/// \author Kieron Higgs
		///
		/// \param  givenAppointment    The attributes to be applied to the new Dependant object.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static bool HoH_To_Dependant(String[] attributes)
		{
			// Check whether the given String array contains the right number of attributes (return false if not)
			if (attributes.Length != 7)
			{
				return false;
			}
			// Make sure that the HCN given in the String array is not referred to by any Dependants as their HoH:
			else
			{
				builder.Length = 0;
				builder.Append(@"SELECT Dependant_HCN FROM Patient_Dependant WHERE HoH_HCN='");
				builder.Append(attributes[0]);
				builder.Append("';");
				DataTable queryResult = Connection.ExecuteQuery(builder.ToString());
				if (queryResult.Rows.Count != 0)
				{
					return false;
				}
			}

			// Okay, the given attributes and selected Patient are valid for HoH -> Dependant.
			// Temporarily disable foreign key constraints which would otherwise disallow insertion + deletion:
			builder.Length = 0;
			builder.Append(@"ALTER TABLE Appointment_Attendee NOCHECK CONSTRAINT FK__Appointment_Attendee__Attendee_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Appointment_Billcode NOCHECK CONSTRAINT FK__Appointment_Billcode__Attendee_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Patient_Dependant NOCHECK CONSTRAINT FK__Patient_Dependant__Dependant_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Patient_Dependant NOCHECK CONSTRAINT FK__Patient_Dependant__HoH_HCN__Patient_HoH__HoH_HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"DELETE FROM Patient_HoH WHERE HoH_HCN='");
			builder.Append(attributes[0]);
			builder.Append("';");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"DELETE FROM Patient WHERE HCN='");
			builder.Append(attributes[0]);
			builder.Append("';");
			Connection.ExecuteCommand(builder.ToString());

			// Remove the old Patient object from the container and add the new one to the container and database:
			Database.Patients.Remove(attributes[0]);
			Factory.PatientFactory.Create(attributes).Add();

			// Reinstate the foreign key constraints:
			builder.Length = 0;
			builder.Append(@"ALTER TABLE Appointment_Attendee CHECK CONSTRAINT FK__Appointment_Attendee__Attendee_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Appointment_Billcode CHECK CONSTRAINT FK__Appointment_Billcode__Attendee_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Patient_Dependant CHECK CONSTRAINT FK__Patient_Dependant__Dependant_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Patient_Dependant CHECK CONSTRAINT FK__Patient_Dependant__HoH_HCN__Patient_HoH__HoH_HCN;");
			Connection.ExecuteCommand(builder.ToString());

			return true;
		}

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void Dependant_To_HoH(String[] attributes)
		///
		/// \brief  Changes a Dependant Patient to an HoH.
		///         
		/// \description    Takes the attributes intended for a new HoH object and attempts to modify the container and database to reflect the changes.
		///
		/// \author Kieron Higgs
		///
		/// \param  attributes    The attributes to be applied to the new HoH object.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static bool Dependant_To_HoH(String[] attributes)
		{
			// Check whether the given String array contains the right number of attributes (return false if not)
			if (attributes.Length != 12)
			{
				return false;
			}
			// Make sure that the HCN given in the String array belongs to a Dependant and not an HoH:
			else
			{
				builder.Length = 0;
				builder.Append(@"SELECT Dependant_HCN FROM Patient_Dependant WHERE Dependant_HCN='");
				builder.Append(attributes[0]);
				builder.Append("';");
				DataTable queryResult = Connection.ExecuteQuery(builder.ToString());
				if (queryResult.Rows.Count != 1)
				{
					return false;
				}
			}

			// Okay, the given attributes and selected Patient are valid for HoH -> Dependant.
			// Temporarily disable foreign key constraints which would otherwise disallow insertion + deletion:
			builder.Length = 0;
			builder.Append(@"ALTER TABLE Appointment_Attendee NOCHECK CONSTRAINT FK__Appointment_Attendee__Attendee_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Appointment_Billcode NOCHECK CONSTRAINT FK__Appointment_Billcode__Attendee_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Patient_Dependant NOCHECK CONSTRAINT FK__Patient_Dependant__Dependant_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Patient_Dependant NOCHECK CONSTRAINT FK__Patient_Dependant__HoH_HCN__Patient_HoH__HoH_HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"DELETE FROM Patient_Dependant WHERE Dependant_HCN='");
			builder.Append(attributes[0]);
			builder.Append("';");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"DELETE FROM Patient WHERE HCN='");
			builder.Append(attributes[0]);
			builder.Append("';");
			Connection.ExecuteCommand(builder.ToString());

			// Remove the old Patient object from the container and add the new one to the container and database:
			Database.Patients.Remove(attributes[0]);
			Factory.PatientFactory.Create(attributes).Add();

			// Reinstate the foreign key constraints:
			builder.Length = 0;
			builder.Append(@"ALTER TABLE Appointment_Attendee CHECK CONSTRAINT FK__Appointment_Attendee__Attendee_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Appointment_Billcode CHECK CONSTRAINT FK__Appointment_Billcode__Attendee_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Patient_Dependant CHECK CONSTRAINT FK__Patient_Dependant__Dependant_HCN__Patient__HCN;");
			Connection.ExecuteCommand(builder.ToString());

			builder.Length = 0;
			builder.Append(@"ALTER TABLE Patient_Dependant CHECK CONSTRAINT FK__Patient_Dependant__HoH_HCN__Patient_HoH__HoH_HCN;");
			Connection.ExecuteCommand(builder.ToString());

			return true;
		}

		///----------------------------------------------------------------------------------------------------------------------------------------------------
		/// \fn public static void HoH_Report(String HCN)
		///
		/// \brief  Generates a list of patients belonging to the same household.
		///         
		/// \description    Takes an HCN and returns a list of Patients: the HoH of the household belonging to the patient with that HCN and any Dependants.
		///
		/// \author Kieron Higgs
		///
		/// \param  HCN    The HCN of a Patient--possibly the HoH, or a Dependant.
		///----------------------------------------------------------------------------------------------------------------------------------------------------
		///
		public static List<Patient> HoH_Report(String HCN)
		{
			List<Patient> household = new List<Patient>();

			// If the Patient to which the given HCN belongs is the HoH, add it to the list and search for the Dependants:
			if (Database.Patients[HCN].IsHoH())
			{
				household.Add(Database.Patients[HCN]);
				foreach (KeyValuePair<String, Patient> record in Database.Patients)
				{
					if (!record.Value.IsHoH() && record.Value.GetHoH() == HCN)
					{
						household.Add(record.Value);
					}
				}
			}
			// Otherwise, the given HCN belongs to a Dependant. Locate the HoH first, add it to the list, and then add Dependants:
			else
			{
				foreach (KeyValuePair<String, Patient> record in Database.Patients)
				{
					if (record.Value.IsHoH() && record.Value.GetHoH() == Database.Patients[HCN].GetHoH())
					{
						household.Add(record.Value);
						break;
					}
				}
				foreach (KeyValuePair<String, Patient> record in Database.Patients)
				{
					if (!record.Value.IsHoH() && record.Value.GetHoH() == Database.Patients[HCN].GetHoH())
					{
						household.Add(record.Value);
					}
				}
			}
			return household;
		}



		///---------------------------------------------------------------------------------------------
		/// \fn public static List<MobileRequest> GetMobileRequests()
		///
		/// \brief  Gets a list of all mobile requests when requested
		/// 
		/// \author Bailey Mills 
		///---------------------------------------------------------------------------------------------
		public static List<MobileRequest> GetMobileRequests()
		{
			List<MobileRequest> requests = new List<MobileRequest>();

			builder.Length = 0;
			builder.Append("SELECT HCN, Date FROM MobileRequest;");
			DataTable queryResult = Connection.ExecuteQuery(builder.ToString());

			// For each row in the table, construct a MobileRequest object
			foreach (DataRow row in queryResult.Rows)
			{
				String HCN = row[0].ToString();
				String Date = ((DateTime)row[1]).ToShortDateString();

				requests.Add(new MobileRequest(new String[] { HCN, Date }));
			}

			return requests;
		}


		///---------------------------------------------------------------------------------------------
		/// \fn public static void DeleteMobileRequest(string HCN)
		///
		/// \brief  Deletes a mobile request (after it has been read)
		/// 
		/// \author Bailey Mills 
		/// 
		/// \param string HCN : deletes based on the given HCN
		///---------------------------------------------------------------------------------------------
		public static void DeleteMobileRequest(string HCN)
		{
			builder.Length = 0;
			builder.Append(@"DELETE FROM MobileRequest WHERE HCN='");
			builder.Append(HCN);
			builder.Append("';");
			Connection.ExecuteCommand(builder.ToString());
		}



		///---------------------------------------------------------------------------------------------
		/// \fn public static void AddMobileRequestResponse(string HCN, string status)
		///
		/// \brief  Inserts a mobile response based on what the scheduling user gave it
		/// 
		/// \author Bailey Mills 
		/// 
		/// \param string HCN : inserts based on the given HCN
		/// \param string status : the status message to send to the mobile app
		///---------------------------------------------------------------------------------------------
		public static void AddMobileRequestResponse(string HCN, string status)
		{
			builder.Length = 0;
			builder.Append(@"INSERT INTO MobileResponse (HCN, Status) VALUES ('");
			builder.Append(HCN);
			builder.Append(@"' , '");
			builder.Append(status);
			builder.Append(@"');");
			Connection.ExecuteCommand(builder.ToString());
		}



		///---------------------------------------------------------------------------------------------
		/// \fn public static bool UpdateMobileFlagsForDate(DateTime date)
		///
		/// \brief  Updates the local database whenever a mobile app makes changes to their check-in
		///			status
		/// 
		/// \author Bailey Mills 
		/// 
		/// \param DateTime date
		///---------------------------------------------------------------------------------------------
		public static bool UpdateMobileFlagsForDate(DateTime date)
		{
			bool changesWereMade = false;

			List<Appointment> appointments = SchedulingSupport.GetAppointmentsForDay(date);
			foreach (Appointment appointment in appointments)
			{
				// Get the actual check-in flag
				int flag = 0;
				builder.Length = 0;
				builder.Append("SELECT MobileFlag FROM Appointment WHERE AppointmentID='");
				builder.Append(appointment.AppointmentID);
				builder.Append("';");
				flag = Convert.ToInt32(Connection.ExecuteQuery(builder.ToString()).Rows[0][0]);

				// Update the appointment object if necessary
				if (appointment.MobileFlag != flag)
				{
					appointment.MobileFlag = flag;
					appointment.Update();

					changesWereMade = true;
				}
			}

			return changesWereMade;
		}



		///---------------------------------------------------------------------------------------------
		/// \fn public static bool LoginAttempt(string username, string password)
		///
		/// \brief  Attempts to log in to the program using the given username and password.
		/// 
		/// \author Bailey Mills 
		/// 
		/// \param string username
		/// \param string password
		///---------------------------------------------------------------------------------------------
		public static bool LoginAttempt(string username, string password)
		{
			bool valid = false;

			builder.Length = 0;
			builder.Append("SELECT COUNT(*) FROM Account WHERE username='");
			builder.Append(username);
			builder.Append("' AND pass='");
			builder.Append(password);
			builder.Append("';");
			if (Convert.ToInt32(Connection.ExecuteQuery(builder.ToString()).Rows[0][0]) > 0)
			{
				valid = true;
			}

			return valid;
		}



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void ExportDB()
        ///
        /// \brief  Export database
        ///
        /// \author Arie
        /// \date   2019-04-22
        ///         
        /// Credit: https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
        ///-------------------------------------------------------------------------------------------------
        public static string ExportDB(string fileName)
        {
            string err = "";
            string output = "";

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = @"C:\Program Files (x86)\Microsoft SQL Server\140\DAC\bin\SqlPackage.exe";
                process.StartInfo.Arguments = "/a:Export /tf:\"" + fileName + "\" /scs:\"Data Source=abacus-ems2.database.windows.net;Initial Catalog=EMS2;\" /ua:\"True\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();

                //* Read the output (or the error)
                output = process.StandardOutput.ReadToEnd();
                err = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception expt)
            {
                Logging.Write(expt.Message);
                Logging.Write(err);

                return err;
            }
            
            if (err != "")
            {
                return err;
            }
            
            return output;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public void ImportDB_Click()
        ///
        /// \brief  Import database click
        ///
        /// \author Arie
        /// \date   2019-04-22
        ///         
        ///  Credit: https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
        ///-------------------------------------------------------------------------------------------------
        public static string ImportDB(string fileName)
        {
            string err = "";
            string output = "";

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = @"C:\Program Files (x86)\Microsoft SQL Server\140\DAC\bin\SqlPackage.exe";
                process.StartInfo.Arguments = "/a:Import /sf:\"" + fileName + "\" /tdn:EMS2 /tsn:\"abacus-ems2.database.windows.net\" /ua:True";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                //* Read the output (or the error)
                output = process.StandardOutput.ReadToEnd();
                err = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception expt)
            {
                Logging.Write(expt.Message);
                Logging.Write(err);

                return err;
            }

            if (err != "")
            {
                return err;
            }

            return output;
        }
    }
}
