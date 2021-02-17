/*
 * 
 * Author:      Kieron Higgs, Arie Kraayenbrink
 * Date:        04-22-2018
 * Project:     EMS 2
 * File:        Connection.cs
 * Description: Contains the attributes and methods used to send SQL queries and commands to the database and put query results into DataTables.
 * 
*/

using System;
using System.Data.SqlClient;
using System.Data;
using Support;
using Data;

/**
 * 
 * Class:           Connection
 * Purpose:         To enact the commands and queries proposed by the Strings passed from the Database class.
 * Attributes:      DataTable patient_Table
 *                  DataTable dependant_Table
 *                  DataTable HoH_Table
 *                  DataTable appointment_Table
 *                  DataTable attendee_Table
 *                  DataTabe billcode_Table
 *                  SqlConnectionStringBuilder builder
 *                  SqlConnection connection
 *                  SqlDataAdapter adapter
 * Relationships:   Called upon by the Database class to retrieve Patient, Appointment, and Billcode data, as well as the Billing class to insert
 *                  the variety of valid billcodes which may be applied to Appintments.
 * Fault detection: Uses try-catch blocks to detect errors in syntax and connection problems.
 */
public static class Connection
{
    public static DataTable patient_Table = new DataTable();
    public static DataTable dependant_Table = new DataTable();
    public static DataTable HoH_Table = new DataTable();

    public static DataTable appointment_Table = new DataTable();
    public static DataTable attendee_Table = new DataTable();
    public static DataTable billcode_Table = new DataTable();

    public static SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
    public static SqlConnection connection;
    public static SqlDataAdapter adapter = new SqlDataAdapter();

    ///----------------------------------------------------------------------------------------------------------------------------------------------------
    /// \fn public static void ExecuteQuery()
    ///
    /// \brief  A basic method used to execute any query upon the EMS database.
    /// 
    /// \description    Takes any query constructed in the Database class and sends it to the SQL database, filling the returned DataTable with the result.
    ///
    /// \author Kieron Higgs
    /// 
    /// \param query    The intended query.
    ///----------------------------------------------------------------------------------------------------------------------------------------------------
    public static DataTable ExecuteQuery(String query)
    {
        DataTable foundRows = new DataTable();

        try
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        foundRows.Load(reader);
                    }
                }
            }
        }
        catch (SqlException e)
        {
            Logging.Write(e.ToString());
        }
        return foundRows;
    }

    ///----------------------------------------------------------------------------------------------------------------------------------------------------
    /// \fn public static void ExecuteCommand()
    ///
    /// \brief  A basic method used to execute any command upon the EMS database.
    /// 
    /// \description    Takes any command constructed in the Database class and sends it to the SQL database to conduct an update or deletion.
    ///
    /// \author Kieron Higgs
    /// 
    /// \param query    The intended command.
    ///----------------------------------------------------------------------------------------------------------------------------------------------------
    ///
    public static void ExecuteCommand(String givenCommand)
    {
        try
        {
            using (connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(givenCommand, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SqlException e)
        {
            Logging.Write(e.ToString());
        }
    }

    ///----------------------------------------------------------------------------------------------------------------------------------------------------
    /// \fn public static void ImportData()
    ///
    /// \brief  Creates a connection string and bundles the Import methods together.
    ///         
    /// \description    Combines the methods used to import Patient and Appointment data into one method after creating the connection String necessary to
    ///                 connect to the EMS database.
    ///
    /// \author Kieron Higgs
    ///
    /// \param  newHoH    The Patient object which needs its information to be submitted to the database for saving. 
    ///----------------------------------------------------------------------------------------------------------------------------------------------------
    ///
    public static void ImportData()
    {
        builder.DataSource = "abacus-ems2.database.windows.net";
        builder.UserID = "abacus";
        builder.Password = "7HQabV9&At#u";
        builder.InitialCatalog = "EMS2";
		builder.ColumnEncryptionSetting = SqlConnectionColumnEncryptionSetting.Enabled;
        ImportPatients();
        ImportAppointments();
    }

    ///----------------------------------------------------------------------------------------------------------------------------------------------------
    /// \fn public static void ImportPatients()
    ///
    /// \brief  Retrieves tables of data used to instantiate Patient objects.
    /// 
    /// \description    Establishes a connection to the database and conducts three queries to fill DataTables with Patient, HoH, and Dependant data. This
    ///                 method passes the reslting data to the Database class for parsing into objects.
    ///
    /// \author Kieron Higgs
    ///----------------------------------------------------------------------------------------------------------------------------------------------------
    ///
    public static void ImportPatients()
    {
        // First, connect to the database and fill three DataTables with the Patients, Patient_Dependant, and Patient_HoH data:
        try
        {
            using (connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(Constants.patientQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        patient_Table.Load(reader);
                    }
                }
                using (SqlCommand command = new SqlCommand(Constants.dependantQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        dependant_Table.Load(reader);
                    }
                }
                using (SqlCommand command = new SqlCommand(Constants.HoHQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        HoH_Table.Load(reader);
                    }
                }
                connection.Close();
            }
        }
        catch (SqlException e)
        {
            Logging.Write(e.ToString());
        }

        // Call the patient parsing method to turn the data into objects:
        Database.ParseImportedPatients(patient_Table, dependant_Table, HoH_Table);
    }

    ///----------------------------------------------------------------------------------------------------------------------------------------------------
    /// \fn public static void ImportAppointments()
    ///
    /// \brief  Retrieves tables of data used to instantiate Appointment objects.
    /// 
    /// \description    Establishes a connection to the database and conducts three queries to fill DataTables with Appointment, Attendee, and Billcode
    ///                 data. This method passes the reslting data to the Database class for parsing into objects.
    ///
    /// \author Kieron Higgs
    ///----------------------------------------------------------------------------------------------------------------------------------------------------
    ///
    public static void ImportAppointments()
    {
        // First, connect to the database and fill three DataTables with the Appointment, Appointment_Attendee, and Appointment_Billcode data:
        try
        {
            using (connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(Constants.appointmentQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        appointment_Table.Load(reader);
                    }
                }
                using (SqlCommand command = new SqlCommand(Constants.attendeeQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        attendee_Table.Load(reader);
                    }
                }
                using (SqlCommand command = new SqlCommand(Constants.billcodeQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        billcode_Table.Load(reader);
                    }
                }
                connection.Close();
            }
        }
        catch (SqlException e)
        {
            Logging.Write(e.ToString());
        }

        // Call the appointment parsing method to convert the data into objects:
        Database.ParseImportedAppointments(appointment_Table, attendee_Table, billcode_Table);
    }



    ///-------------------------------------------------------------------------------------------------
    /// \fn public static void InsertBillCodes(string code, DateTime date, string fee)
    ///
    /// \brief  Inserts a bill codes
    ///         
    /// \description    This method saves a bill code to the data base using a stored procedure.
    ///
    /// \author Arie
    /// \date   2019-04-12
    ///
    /// \param  code    The code.
    /// \param  date    The date Date/Time.
    /// \param  fee     The fee.
    ///-------------------------------------------------------------------------------------------------
    public static void InsertBillCodes(string code, DateTime date, string fee) 
    {
        try
        {
            using (SqlConnection conn = new SqlConnection("Server=tcp:abacus-ems2.database.windows.net,1433;Initial Catalog=EMS2;Persist Security Info=False;User ID=abacus;Password=7HQabV9&At#u;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("SaveBillCodes", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new SqlParameter("@BillCode", code));
                cmd.Parameters.Add(new SqlParameter("@EffectDate", date));
                cmd.Parameters.Add(new SqlParameter("@Fee", fee));

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                }
            }
        }
        catch (Exception e)
        {
            Logging.Write(e.Message);
        }
    }
}


