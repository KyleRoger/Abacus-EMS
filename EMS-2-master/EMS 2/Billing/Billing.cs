/*
 * 
 * Author:      Arie Kraayenbrink
 * Date:        January to April 2019
 * Project:     EMS2
 * File:        Billing.cs
 * Description: This class handles the billing stuff for EMS2.
 * 
*/



using System.Configuration;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Support;
using System.Text.RegularExpressions;
using System.Reflection.Emit;
using Demographics;
using Scheduling;
using Data;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using EMS_2;
using System.Data;



namespace EMS_Billing
{
    ///-------------------------------------------------------------------------------------------------
    /// \class  EMSBilling
    ///
    /// \brief  This is the billing class with its methods.
    ///         
    ///         Name:               Billing
    ///         
    ///         Purpose:            To provide billing functionality. Each method describes in more
    ///         detail what goes
    ///                             on in this class.
    ///         
    ///         Fault Detection:    This class uses bool status flags extensively to indicate when
    ///         things are not
    ///                             flowing down the happy path. Should an error happen, other
    ///                             related methods are notified as needed through these flags.
    ///                             Before saving or using data provided by an outside source, it is
    ///                             examined to make sure it matches the desired data and is valid.
    ///                             This is done through comparisons and other checks. No 'bad data'
    ///                             is allowed.
    ///         
    ///         Relationships:      The Billing class is not directly tied to any other class,
    ///         however, it uses
    ///                             several methods from the support class for thins like file IO.
    ///                             Data is also pulled from the patient and appointment classes as
    ///                             needed.
    ///         
    ///         Attributes:
    ///
    /// \author Arie
    /// \date   2019-04-20
    ///-------------------------------------------------------------------------------------------------
    public class EMSBilling : Constants
    {
        public int TotalLineCount { get; set; }
        public int ValidLineCount { get; set; }

        Dictionary<string, MasterRecord> billCodes = new Dictionary<string, MasterRecord>();    ///< / all billing codes provided in the master record
        public List<string> monthlyRecords = new List<string>();    ///< / all the appointments for the month with fee. Used for monthly bill file
        List<ResponseRecord> responseRecords = new List<ResponseRecord>();  ///< The response records
        Dictionary<DateTime, MonthlyTotals> monthlyTotals = new Dictionary<DateTime, MonthlyTotals>();  ///< The monthly totals
        private string ResponseFilePath = dbDirectory + @"\" + ConfigurationManager.AppSettings.Get("BillRespFile");    ///< .
        private string MasterBillCodePath = dbDirectory + @"\" + ConfigurationManager.AppSettings.Get("BillCodeFile");  ///< .
        private string BillExportFilePath = dbDirectory + @"\" + ConfigurationManager.AppSettings.Get("BillExpFile");   ///< .

        private Dictionary<DateTime, bool> responseRead = new Dictionary<DateTime, bool>(); ///< The response read
        CultureInfo culture = new CultureInfo("en-US"); ///< for data time settings
        DataTable billCodesTable;      ///< The bill codes table



        ///-------------------------------------------------------------------------------------------------
        /// \fn public EMSBilling()
        ///
        /// \brief  The default constructor.
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///-------------------------------------------------------------------------------------------------
        public EMSBilling()
        {
            //ParseBillFile(MasterBillCodePath);
            Logging.Write("Creating Billing object");
        }



        ///-------------------------------------------------------------------------------------------------
        /// \struct MasterRecord
        ///
        /// \brief  This struct used to hold all records provided by the MoH master billing file.
        ///         
        ///         Name:       MasterRecord Purpose:    To provide easy access to any billing record and
        ///         any part of a record. Each record is split
        ///                     into three parts: code, startDate, doctorFee. These billing records come
        ///                     from the master billing file provided by the MoH.
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// ### param [in]  N   A.
        /// ### param [out] N   A.
        /// ### returns N/A.
        ///-------------------------------------------------------------------------------------------------
        private struct MasterRecord
        {
            private string code;       ///< The code
            private DateTime startDate; ///< The start date
            private string doctorFee;   ///< The doctor fee



            ///-------------------------------------------------------------------------------------------------
            /// \property   public string Code
            ///
            /// \brief  Gets or sets the code
            ///
            /// \returns    The code.
            ///-------------------------------------------------------------------------------------------------
            public string Code
            {
                get { return code; }

                set { if (value.Length == 4) code = value; }
            }



            ///-------------------------------------------------------------------------------------------------
            /// \property   public string DoctorFee
            ///
            /// \brief  Gets or sets the doctor fee
            ///
            /// \returns    The doctor fee.
            ///-------------------------------------------------------------------------------------------------
            public string DoctorFee
            {
                get { return doctorFee; }
                set { doctorFee = value; }
            }



            ///-------------------------------------------------------------------------------------------------
            /// \property   public DateTime StartDate
            ///
            /// \brief  Gets or sets the start date
            ///
            /// \returns    The start date.
            ///-------------------------------------------------------------------------------------------------
            public DateTime StartDate
            {
                get { return startDate; }
                set { startDate = value; }
            }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \struct ResponseRecord
        ///
        /// \brief  Information about the response.
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///-------------------------------------------------------------------------------------------------
        private struct ResponseRecord
        {
            ///-------------------------------------------------------------------------------------------------
            /// \property   public string Gender
            ///
            /// \brief  Gets or sets the gender
            ///
            /// \returns    The gender.
            ///-------------------------------------------------------------------------------------------------
            public string Gender { get; set; }



            ///-------------------------------------------------------------------------------------------------
            /// \property   public string EncounterState
            ///
            /// \brief  Gets or sets the state of the encounter
            ///
            /// \returns    The encounter state.
            ///-------------------------------------------------------------------------------------------------
            public string EncounterState { get; set; }



            ///-------------------------------------------------------------------------------------------------
            /// \property   public string HCN
            ///
            /// \brief  Gets or sets the hcn
            ///
            /// \returns    The hcn.
            ///-------------------------------------------------------------------------------------------------
            public string HCN { get; set; }



            ///-------------------------------------------------------------------------------------------------
            /// \property   public DateTime Date
            ///
            /// \brief  Gets or sets the Date/Time of the date
            ///
            /// \returns    The date.
            ///-------------------------------------------------------------------------------------------------
            public DateTime Date { get; set; }



            ///-------------------------------------------------------------------------------------------------
            /// \property   public string BillCode
            ///
            /// \brief  Gets or sets the bill code
            ///
            /// \returns    The bill code.
            ///-------------------------------------------------------------------------------------------------
            public string BillCode { get; set; }



            ///-------------------------------------------------------------------------------------------------
            /// \property   public string Fee
            ///
            /// \brief  Gets or sets the fee
            ///
            /// \returns    The fee.
            ///-------------------------------------------------------------------------------------------------
            public string Fee { get; set; }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \struct MonthlyTotals
        ///
        /// \brief  A monthly totals.
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///-------------------------------------------------------------------------------------------------
        public struct MonthlyTotals
        {
            ///-------------------------------------------------------------------------------------------------
            /// \property   public int TotalBilledEncounters
            ///
            /// \brief  Gets or sets the total number of billed encounters
            ///
            /// \returns    The total number of billed encounters.
            ///-------------------------------------------------------------------------------------------------
            public int TotalBilledEncounters { get; set; }      //Total Encounters Billed (Integer sum of encounters that month)



            ///-------------------------------------------------------------------------------------------------
            /// \property   public decimal TotalBilledProcedures
            ///
            /// \brief  Gets or sets the total number of billed procedures
            ///
            /// \returns    The total number of billed procedures.
            ///-------------------------------------------------------------------------------------------------
            public decimal TotalBilledProcedures { get; set; }      //Total Billed Procedures (in Dollars)



            ///-------------------------------------------------------------------------------------------------
            /// \property   public decimal TotalReceived
            ///
            /// \brief  Gets or sets the total number of received
            ///
            /// \returns    The total number of received.
            ///-------------------------------------------------------------------------------------------------
            public decimal TotalReceived { get; set; }              //Received Total (in Dollars)



            ///-------------------------------------------------------------------------------------------------
            /// \property   public decimal TotalReceivedPercentage
            ///
            /// \brief  Gets or sets the total number of received percentage
            ///
            /// \returns    The total number of received percentage.
            ///-------------------------------------------------------------------------------------------------
            public decimal TotalReceivedPercentage { get; set; }    //Received Percentage (RT/TBP*100)



            ///-------------------------------------------------------------------------------------------------
            /// \property   public decimal TotalBillingAverage
            ///
            /// \brief  Gets or sets the total number of billing average
            ///
            /// \returns    The total number of billing average.
            ///-------------------------------------------------------------------------------------------------
            public decimal TotalBillingAverage { get; set; }        //Average Billing (RT/TEB in Dollars)



            ///-------------------------------------------------------------------------------------------------
            /// \property   public int TotalFollowUpEncounters
            ///
            /// \brief  Gets or sets the total number of follow up encounters
            ///
            /// \returns    The total number of follow up encounters.
            ///-------------------------------------------------------------------------------------------------
            public int TotalFollowUpEncounters { get; set; }    //Num. Encounters to Follow-up (Integer sum of FHCV and CMOH count)
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool EnterBillCode(string billCode, DateTime appDate, string HCN, ref Dictionary<DateTime, List<Appointment>> appointmentsDB)
        ///
        /// \brief  Add bill code to appointment
        ///         
        ///         This method takes in the appointment details and the billing code to add. It does
        ///         data validity checks and if all is well adds the bill code to the appointment.
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param          billCode        The bill code to add.
        /// \param          appDate         The date of the appointment.
        /// \param          HCN             The health card number.
        /// \param [in,out] appointmentsDB  The database of appointments.
        ///
        /// \returns    Bool: true is successful, false otherwise.
        ///-------------------------------------------------------------------------------------------------
        public bool EnterBillCode(string billCode, DateTime appDate, string HCN, ref Dictionary<DateTime, List<Appointment>> appointmentsDB)
        {
            bool success = false;

            try
            {
                bool validCode = false;

                validCode = validateBillCode(billCode, appDate);

                if (validCode)
                {

                    foreach (Appointment appt in Database.Appointments[appDate])
                    {
                        if (appt.GetBillcodesByHCN(HCN) != null)
                        {
                            appt.GetBillcodesByHCN(HCN).Add(billCode);
                        }
                    }
                }
                else
                {
                    Logging.Write("Unable to save bill code. Invalid code");
                    success = false;
                }
            }
            catch (Exception e)
            {
                Logging.Write("Error trying to add a bill code to appointment. " + e.Message);
            }

            return success;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool EnterBillCode(string billCode, Appointment appointment)
        ///
        /// \brief  Add bill code to appointment
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  billCode    The billing code to add.
        /// \param  appointment The appointment to add the bill code to.
        ///
        /// \returns    Bool: true is successful, false otherwise.
        ///-------------------------------------------------------------------------------------------------
        public bool EnterBillCode(string billCode, Appointment appointment)
        {
            bool success = false;

            try
            {
                bool validCode = false;

                validCode = validateBillCode(billCode, appointment.Date);

                if (validCode)
                {
                    //appointment.billCodes.Add(billCode);
                    //success = true;
                    //Logging.Write("Saved billing code [" + billCode + "] to appoinment [" + appointment + "]");
                    //Appointment.SaveAppointmentsDB();
                }
                else
                {
                    Logging.Write("Unable to save bill code. Invalid code");
                    success = false;
                }
            }
            catch (Exception e)
            {
                Logging.Write("Error trying to add a bill code to appointment. " + e.Message);
                success = false;
            }

            return success;

        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool UpdateBillCode(string oldBillCode, string newBillCode, DateTime appDate, string HCN, ref Dictionary<DateTime, List<Appointment>> appointmentsDB)
        ///
        /// \brief  This method is used to update a billing code in an appointment's record. It does this
        ///         by removing the old billing code and inserting in the new code.
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param          oldBillCode     The old bill code to be updated with a new one.
        /// \param          newBillCode     The new bill code.
        /// \param          appDate         The date of the appointment.
        /// \param          HCN             The health card number.
        /// \param [in,out] appointmentsDB  The database of appointments.
        ///
        /// \returns    valideCode: true if the codes passed in were valid, false if not.
        ///
        /// ### param           appointment The appointment to do the bill code update on.
        ///-------------------------------------------------------------------------------------------------
        public bool UpdateBillCode(string oldBillCode, string newBillCode, DateTime appDate, string HCN, ref Dictionary<DateTime, List<Appointment>> appointmentsDB)
        {
            bool validCode = false;
            bool success = false;

            validCode = validateBillCode(oldBillCode, appDate);
            if (validCode)
            {
                validCode = validateBillCode(newBillCode, appDate);
            }

            //remove old billing code from appointment
            if (validCode)
            {
                foreach (Appointment appt in appointmentsDB[appDate])
                {
                    if (appt.GetBillcodesByHCN(HCN) != null)
                    {
                        try
                        {
                            appt.GetBillcodesByHCN(HCN).Remove(oldBillCode);
                        }
                        catch (Exception e)
                        {
                            Logging.Write("Unable to update billing code [" + oldBillCode + "] in appointment [" + appt.ToString() + "]. " + e.Message);
                        }
                        success = true;
                    }
                }
            }

            //add new billing code to appointment
            if (validCode && success)
            {
                success = EnterBillCode(newBillCode, appDate, HCN, ref appointmentsDB);
                if (success)
                {
                    //Appointment.SaveAppointmentsDB();
                }
            }

            return success;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool UpdateBillCode(string oldBillCode, string newBillCode, Appointment appointment)
        ///
        /// \brief  Updates a billing code in an appointment
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  oldBillCode The billing code to update.
        /// \param  newBillCode The new billing code.
        /// \param  appointment The appointment to do the update on.
        ///
        /// \returns    Bool: true is successful, false otherwise.
        ///-------------------------------------------------------------------------------------------------
        public bool UpdateBillCode(string oldBillCode, string newBillCode, Appointment appointment)
        {
            bool validCode = false;
            bool success = false;

            validCode = validateBillCode(oldBillCode, appointment.Date);
            if (validCode)
            {
                validCode = validateBillCode(newBillCode, appointment.Date);
            }

            //remove old billing code from appointment
            if (validCode)
            {
                try
                {
                    //appointment.billCodes.Remove(oldBillCode);
                }
                catch (Exception e)
                {
                    Logging.Write("Unable to update billing code [" + oldBillCode + "] in appointment [" + appointment.ToString() + "]. " + e.Message);
                }
                success = true;
            }

            //add new billing code to appointment
            if (validCode && success)
            {
                success = EnterBillCode(newBillCode, appointment);
            }

            return success;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool Flag4Review(string state, string HCN, DateTime appDate)
        ///
        /// \brief  This method allows an patient to be flagged for recall.
        ///         
        ///         The health card number is used to find the patient so that a recall flag can be set.
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  state   The state.
        /// \param  HCN     The health card number of the patient who needs a revisit to the clinic.
        /// \param  appDate The date of the appointment.
        ///
        /// \returns    validAppointment: indicating if the appointment that was to be flagged is a valid
        ///             one or not.
        ///-------------------------------------------------------------------------------------------------
        public bool Flag4Review(string state, string HCN, DateTime appDate)
        {
            bool appointmentPassed = false;
            //List<string> appointmentHCNs;

            //foreach (KeyValuePair<DateTime, List<Appointment>> day in Database.Appointments)
            //{
            //    foreach (Appointment appt in day.Value)
            //    {
            //        appointmentHCNs =
            //        if (appt.Date.Month == appDate.Month
            //            && appt.Date.Year == appDate.Year
            //            && appt.HCN == HCN
            //            && (state == "FHCV" || state == "CMOH"))
            //        {
            //            appt.billFlag = state;
            //            appointmentPassed = true;
            //        }
            //    }
            //}

            //Set flag
            //appointmentPassed = Database.Appointments.SetFlag(HCN);
            
            return appointmentPassed;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool GenMonthlyBill(DateTime date)
        ///
        /// \brief  Generates a monthly bill
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  date    The year and month to create billing file for.
        ///
        /// \returns    True if it succeeds, false if it fails.
        /// \returns    success bool: True if everything passed, false if there was an error.
        ///-------------------------------------------------------------------------------------------------
        public bool GenMonthlyBill(DateTime date)
        {
            bool success = false;
            string fileName = Constants.dbDirectory + @"\" + Constants.monthlyBill + date.ToString("yyyyMM") + Constants.fileExt;   // Default file location.

            GenMonthlyBill(date, fileName);

            return success;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool GenMonthlyBill(DateTime date, string fileName)
        ///
        /// \brief  Generates a monthly bill
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  date        The year and month to create billing file for.
        /// \param  fileName    Filename of the file.
        ///
        /// \returns            True if it succeeds, false if it fails.
        /// \returns    success bool: True if everything passed, false if there was an error.
        ///
        /// ### brief   GenMonthlyBill(DateTime date)   Used to produce the monthly output billing file
        ///                                             for the MoH.
        ///                                             
        ///                                             By searching through the application’s data file,
        ///                                             this module will generate the data needed by the
        ///                                             Ministry of Health to provide payment to the
        ///                                             clinic.  
        ///                                             This process requires being able to look up and
        ///                                             then apply a fee from a fee schedule file
        ///                                             provided by the Ontario Ministry of Health,
        ///                                             against any billable encounters within a specific
        ///                                             month.
        ///-------------------------------------------------------------------------------------------------
        public bool GenMonthlyBill(DateTime date, string fileName)
        {
            bool success = false;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            string query = "SELECT attendee_hcn, code, gender, date, DollarAmount " +
                                "FROM BillReport('" +
                                date.Year + "-" + date.Month + "-" + "01 00:00:00', '" +
                                date.Year + "-" + date.Month + "-" + lastDayOfMonth.Day + " 00:00:00')"
                                ;

            //string query = "SELECT attendee_hcn, appointment_billcode.code, gender, appointment.date, DollarAmount " +
            //                    "FROM appointment_billcode " +
            //                    "INNER JOIN appointment on appointment.appointmentid = appointment_billcode.appointmentid " +
            //                    "INNER JOIN Patients on attendee_hcn = HCN " +
            //                    "INNER JOIN BillCode on appointment_billcode.code = BillCode.code " +
            //                    "WHERE Appointment.Date >= '" +
            //                    date.Year + "-" + date.Month + "-" + "01 00:00:00'" +
            //                    " AND Appointment.date <= '" +
            //                    date.Year + "-" + date.Month + "-" + lastDayOfMonth.Day + " 00:00:00'" +
            //                    " AND appointment.date >= billcode.date "
            //                    ;

            DataTable dt = Connection.ExecuteQuery(query);

            //check if file already exists
            if (FileIO.CheckFileExists(Constants.dbDirectory + @"\" + Constants.monthlyBill + date.ToString("yyyyMM") + Constants.fileExt))
            {
                //Rename current file
                success = FileIO.RenameFile(Constants.dbDirectory, Constants.monthlyBill + date.ToString("yyyyMM"), Constants.fileExt);
            }

            // Prepare to save to file
            StringBuilder allRecords = new StringBuilder();
            
            int i = 0;

            while (dt.Rows.Count > i)
            {
                allRecords.Append(dt.Rows[i]["Date"].ToString().Substring(0, 4) + 
                    dt.Rows[i]["Date"].ToString().Substring(5, 2) + 
                    dt.Rows[i]["Date"].ToString().Substring(8, 2));
                allRecords.Append(dt.Rows[i]["attendee_hcn"].ToString());
                allRecords.Append(dt.Rows[i]["gender"].ToString());
                allRecords.Append(dt.Rows[i]["Code"].ToString());
                allRecords.AppendLine(dt.Rows[i]["DollarAmount"].ToString());
                i++;
            }

            // Save the data to file
            try
            {
                if (fileName != null)
                {
                    success = FileIO.WriteFile(fileName, allRecords.ToString());
                }
                else
                {
                    success = FileIO.WriteFile(Constants.dbDirectory + @"\" + Constants.monthlyBill + date.ToString("yyyyMM") + Constants.fileExt, allRecords.ToString());
                }
            }
            catch (Exception expt)
            {
                Logging.Write(expt.Message);
            }

            return success;
        }
        


        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool RecMonthlyBill(DateTime date, string fileName)
        ///
        /// \brief  This method reconciles the monthly billing file from the MoH.
        ///         
        ///         The Ministry of Health will provide a response file to any submitted Monthly Billing
        ///         file. Each billed encounter will be assessed and replied to within the response file.
        ///         There are 4 states for an encounter in this file: PAID, DECL (Declined), FHCV (failed
        ///         health card validation), and CMOH (contact Ministry of Health). This method will read
        ///         this file and then update the monthly summary and flag encounters for review if they
        ///         are tagged FHCV or CMOH.
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  date        The month to be processed as a DateTime object.
        /// \param  fileName    The path and filename to the MoH response file.
        ///
        /// \returns    needsReview: used to indicate if the billing file has appointments that need to
        ///             be reviewed.
        ///-------------------------------------------------------------------------------------------------
        public bool RecMonthlyBill(DateTime date, string fileName)
        {
            //bool needsReview = true;
            bool success = false;

            if (fileName.Equals(null) || !FileIO.CheckFileExists(fileName))
            {
                success = false;
            }
            else
            {
                success = true;
            }

            if (success)
            {
                if (responseRead.ContainsKey(date))
                {
                    // remove old key so we can update.
                    responseRead.Remove(date);
                }

                success = ParseResponseFile(fileName);

                if (success)
                {
                    foreach (var rec in MainWindow.billing.responseRecords)
                    {
                        Flag4Review(rec.EncounterState, rec.HCN, rec.Date);
                    }

                    SubmitMOHRecord();  // Save to database

                    responseRead.Add(date, true);   //mark specified month as reviewed   
                }
            }
            else
            {
                success = false;
            }

            return success;
        }

        private void SubmitMOHRecord()
        {
            string insertCommand;

            foreach (var rec in MainWindow.billing.responseRecords)
            {
                insertCommand = "INSERT INTO ResponseRecord (appointmentDate, HCN, gender, Code, fee, encounterState) " +
                    "VALUES ('" + rec.Date + "', '" + rec.HCN + "', '" + rec.Gender + "', '" + rec.BillCode + "', '" + rec.Fee + "', '" + rec.EncounterState + "')";

                Connection.ExecuteCommand(insertCommand);
            }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool MonthlyBillSummary(DateTime date)
        ///
        /// \brief  This method produces a summary of the specified months bills.
        ///         
        ///         The Billing module displays summary information for a selected month that includes:
        ///             - Total Encounters Billed(Integer sum of encounters that month)
        ///             - Total Billed Procedures(in Dollars)
        ///             - Received Total(in Dollars)
        ///             - Received Percentage(RT/TBP*100)
        ///             - Average Billing(RT/TEB in Dollars)
        ///             - Num.Encounters to Follow-up(Integer sum of FHCV and CMOH count)
        ///         \note Most of the summary items above cannot be generated until the MoH provides a
        ///         response file (See RecMonthlyBill() method)
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  date    The month to be processed in format yyyyMM.
        ///
        /// \returns    success: true if summary was produced without issues, false otherwise.
        ///-------------------------------------------------------------------------------------------------
        public bool MonthlyBillSummary(DateTime date)
        {
            bool success = false;
            bool found = false;
            MonthlyTotals monthTotal = new MonthlyTotals();

            int billedEncounters = 0;
            decimal billedProcedures = 0;
            decimal received = 0;
            decimal receivedPercentage = 0;
            decimal billingAverage = 0;
            int followUpEncounters = 0;
    
            foreach (var record in MainWindow.billing.responseRecords)
            {
                if (record.Date.Month == date.Month && record.Date.Year == date.Year)
                {
                    billedEncounters++;
                    billedProcedures += Convert.ToDecimal(record.Fee);
                    if (record.EncounterState.Equals("PAID"))
                    {
                        received += Convert.ToDecimal(record.Fee);
                    }
                    if (record.EncounterState.Equals("FHCV") || record.EncounterState.Equals("CMOH"))
                    {
                        followUpEncounters++;
                    }

                    found = true;
                }
            }

            if (found)
            {
                receivedPercentage = (received / billedProcedures) * 100;
                billingAverage = received / billedEncounters;

                monthTotal.TotalBilledEncounters = billedEncounters;
                monthTotal.TotalBilledProcedures = billedProcedures / 10000;
                monthTotal.TotalReceived = received / 10000;
                monthTotal.TotalReceivedPercentage = receivedPercentage;
                monthTotal.TotalBillingAverage = billingAverage / 10000;
                monthTotal.TotalFollowUpEncounters = followUpEncounters;
                
                if (MainWindow.billing.monthlyTotals.ContainsKey(date))
                {
                    // get rid of the old one
                    MainWindow.billing.monthlyTotals.Remove(date);
                }

                //store specified months data
                MainWindow.billing.monthlyTotals.Add(date, monthTotal);

                success = true;
            }
            else
            {
                Logging.Write("No billing records found for " + date.ToString("yyyy-MM"));
            }

            return success;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool MonthlyBillSummary(DateTime date, string fileName)
        ///
        /// \brief  This method produces a summary of the specified months bills.
        ///         
        ///         The Billing module displays summary information for a selected month that includes:
        ///             - Total Encounters Billed(Integer sum of encounters that month)
        ///             - Total Billed Procedures(in Dollars)
        ///             - Received Total(in Dollars)
        ///             - Received Percentage(RT/TBP*100)
        ///             - Average Billing(RT/TEB in Dollars)
        ///             - Num.Encounters to Follow-up(Integer sum of FHCV and CMOH count)
        ///         \note Most of the summary items above cannot be generated until the MoH provides a
        ///         response file (See RecMonthlyBill() method)
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  date        The month to be processed in format yyyyMM.
        /// \param  fileName    The path and filename to the MoH response file.
        ///
        /// \returns    success: true if summary was produced without issues, false otherwise.
        ///-------------------------------------------------------------------------------------------------
        public bool MonthlyBillSummary(DateTime date, string fileName)
        {
            bool success = false;

            success = RecMonthlyBill(date, fileName);

            if (success)
            {
                // Generate the monthly summary
                MonthlyBillSummary(date);
            }
            else
            {
                Logging.Write("Failed to reconcile MOH file [" + fileName + "] for the month of " + date.ToString("yyyy-MM"));
            }

            return success;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public DataTable MonthlyBillSummaryTable(DateTime date)
        ///
        /// \brief  Monthly bill summary table
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  date    The year and month to create billing file for.
        ///
        /// \returns    A DataTable.
        ///-------------------------------------------------------------------------------------------------
        public DataTable MonthlyBillSummaryTable(DateTime date)
        {
            DataTable summaryTable = new DataTable("Monthly Summary");
            MonthlyTotals monthTotal = new MonthlyTotals();

            // Generate the monthly summary
            MonthlyBillSummary(date);

            // Put into a DataTable
            summaryTable.Clear();

            summaryTable.Columns.Add("Total Encounters", typeof(int));
            summaryTable.Columns.Add("Total Billed ($)", typeof(string));
            summaryTable.Columns.Add("Total Received ($)", typeof(string));
            summaryTable.Columns.Add("Percentage Received (%)", typeof(string));
            summaryTable.Columns.Add("Average Billing ($)", typeof(string));
            summaryTable.Columns.Add("Total Follow-ups", typeof(int));

            MainWindow.billing.monthlyTotals.TryGetValue(date, out monthTotal);

            summaryTable.Rows.Add(monthTotal.TotalBilledEncounters, monthTotal.TotalBilledProcedures.ToString("N4"), monthTotal.TotalReceived.ToString("N4"),
                monthTotal.TotalReceivedPercentage.ToString("N2"), monthTotal.TotalBillingAverage.ToString("N4"), monthTotal.TotalFollowUpEncounters);

            return summaryTable;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private bool ParseResponseFile(string fileName)
        ///
        /// \brief  Parse MoH response file
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  fileName    The name of the MoH file.
        ///
        /// \returns    bool: True if everything passed, false if there was an error.
        ///-------------------------------------------------------------------------------------------------
        private bool ParseResponseFile(string fileName)
        {
            bool success = false;
            string allData = "null";
            TotalLineCount = 0;
            ValidLineCount = 0;

            string recordReg = @"[0-9]{18}[A-Z]{4}[0-9]{14}(PAID|CMOH|FHCV|DECL){1}";
            string GenderReg = @"((?<=[A-Z]{2})(?<!PA))(M|F|I|H){1}((?=[A-Z]){1}(?!D))";
            string stateReg = @"(PAID|CMOH|FHCV|DECL){1}";
            string hcnReg = @"(?<=[0-9]{8})([0-9]{10}[A-Z]{2})";
            string dateReg = @"([0-9]{8})(?=[0-9]{10})";
            string billReg = @"(?<!(PAI|CMO|FHC|DEC))([A-Z]{1})([0-9]{3})(?=([0-9]{11}(PAID|CMOH|FHCV|DECL)))";
            string feeReg = @"(?<=([0-9]{18}[A-Z]{3}[A-Z]{1}[0-9]{3}))([0-9]{11})(?=(PAID|CMOH|FHCV|DECL))";

            try
            {
                success = FileIO.ReadFile(fileName, out allData);
                if (success)
                {
                    Logging.Write("Successfully read: " + fileName);
                }
                else
                {
                    Logging.Write("Failed to read file " + fileName);
                }
            }
            catch (Exception e)
            {
                Logging.Write("Error occurred while reading file. " + e.Message);
            }

            if (success)
            {
                // Count number of lines in file. This will be compared to the number of found valid records.
                TotalLineCount = FileIO.CountLines(fileName);

                //parse out each record from the data
                foreach (Match match in Regex.Matches(allData, recordReg, RegexOptions.IgnoreCase))
                {
                    //make sure there is a valid record
                    if (match.Value.Length != 40 || match.Value == null)
                    {
                        //error
                        Logging.Write("Error occurred when reading record from MoH response file.");
                    }
                    else
                    {
                        ResponseRecord record = new ResponseRecord();
                        try
                        {
                            record.Date = DateTime.ParseExact(Regex.Match(match.Value, dateReg, RegexOptions.IgnoreCase).Value.ToString(), "yyyyMMdd", culture);
                        }
                        catch (Exception e)
                        {
                            Logging.Write("Error (" + e + ") occurred when reading date from response file.");
                        }
                        record.HCN = Regex.Match(match.Value, hcnReg, RegexOptions.IgnoreCase).Value.ToString();
                        record.Gender = Regex.Match(match.Value, GenderReg, RegexOptions.IgnoreCase).Value.ToString();
                        record.BillCode = Regex.Match(match.Value, billReg, RegexOptions.IgnoreCase).Value.ToString();
                        record.Fee = Regex.Match(match.Value, feeReg, RegexOptions.IgnoreCase).Value.ToString();
                        record.EncounterState = Regex.Match(match.Value, stateReg, RegexOptions.IgnoreCase).Value.ToString();

                        try
                        {
                            //Program.billing.responseRecords.Add(record);
                            responseRecords.Add(record);
                            Logging.Write("Storing ( " + record.HCN + " ) in Dictionary");
                        }
                        catch (Exception e)
                        {
                            Logging.Write(e.Message);
                        }

                        ValidLineCount++;
                    }
                }
            }

            return success;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool ParseBillFile(string fileName)
        ///
        /// \brief  This method pulls out the billing code records from the file provided.
        ///         
        ///         The billing file will provide records to be used mainly for getting the billing fee.
        ///         Example record: A6652012040100000913500 In this case, we’re carrying the Schedule
        ///         Code (or Fee Code), the effective date as YYYYMMDD and 11 digits for the dollar
        ///         amount paid for the service. This method will parse out each part of data from each
        ///         record and store it for later use.
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  fileName    The path and filename to the MoH master billing file.
        ///
        /// \returns    bool success: true if all is well, false if an error occurred.
        ///-------------------------------------------------------------------------------------------------
        public bool ParseBillFile(string fileName)
        {
            bool success = false;
            string allData = "null";

            ValidLineCount = 0;
            TotalLineCount = 0;

            string recordReg = @"[a-z]{1}[0-9]{22}";

            try
            {
                success = FileIO.ReadFile(fileName, out allData);
                Logging.Write("Read status of " + fileName + " was " + success);
            }
            catch (Exception e)
            {
                Logging.Write("Error occurred while reading file. " + e.Message);
                success = false;
            }

            if (success)
            {
                // Count number of lines in file. This will be compared to the number of found valid records.
                TotalLineCount = FileIO.CountLines(fileName);

                //clear out old bill codes
                billCodes.Clear();

                DateTime matchDate = new DateTime();

                //parse out each record from the data
                foreach (Match match in Regex.Matches(allData, recordReg, RegexOptions.IgnoreCase))
                {
                    try
                    {
                        matchDate = DateTime.ParseExact(match.Value.Substring(4, 8), "yyyyMMdd", culture);
                    }
                    catch (Exception e)
                    {
                        Logging.Write("Error (" + e + ") when trying to read date from bill file " + fileName);
                    }

                    //make sure there is a valid record
                    if (match.Value.Length != 23 || match.Value == null || matchDate > DateTime.Now)
                    {
                        //error
                        Logging.Write("Error occurred when reading master billing file record.");
                        success = false;
                    }
                    else
                    {
                        MasterRecord record = new MasterRecord();
                        record.Code = match.Value.Substring(0, 4);
                        
                        record.StartDate = matchDate;
                       
                        record.DoctorFee = match.Value.Substring(12, 11);

                        try
                        {
                            if (!billCodes.ContainsKey(record.Code))
                            {
                                billCodes.Add(record.Code, record);
                            }

                            Connection.InsertBillCodes(record.Code, record.StartDate, record.DoctorFee);

                            ValidLineCount++;
                        }
                        catch (Exception e)
                        {
                            Logging.Write(e.Message);
                            success = false;
                        }
                    }
                }
            }
            else
            {
                success = false;
            }

            return success;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public bool validateBillCode(string billCode, DateTime billingDate)
        ///
        /// \brief  Check that a bill code is in the system.
        ///         
        ///         This method checks that the provided billing code really exists in the master file of
        ///         billing codes provided by the MoH. It also varifies that the requested billing date
        ///         is after the code came into effect.
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  billCode    The billing code to check.
        /// \param  billingDate The billing date, will be checked to make sure this code is after the
        ///                     bill code effective date. format yyyyMMdd.
        ///
        /// \returns    bool isValid: true if the checks pass, false otherwise.
        ///-------------------------------------------------------------------------------------------------
        public bool validateBillCode(string billCode, DateTime billingDate)
        {
            bool isValid = false;

            //check if bill code exists
            isValid = billCodes.ContainsKey(billCode);

            //check if bill code is valid for the desired date and not null
            var billingCodeRecord = new MasterRecord();
            billCodes.TryGetValue(billCode, out billingCodeRecord);

            //look first in local storage to save database interaction
            if (isValid && (billingDate != null) && (billingCodeRecord.StartDate != null) && billingDate >= billingCodeRecord.StartDate)
            {
                isValid = true;
            }

            // If not found locally, check Database
            if (!isValid)
            {
                string query = "SELECT code " +
                    "FROM BillCode " +
                    "WHERE code = '" + billCode +
                    "' AND date <= '" + billingDate + "'";

                DataTable codesTable = Connection.ExecuteQuery(query);

                if (codesTable.Rows.Count > 0)
                {
                    isValid = true;
                }
            }

            // Bill code couldn't be found and is deemed invalid.
            if (!isValid)
            {
                Logging.Write("[" + billCode + "] is not a valid bill code.");
                isValid = false;
            }

            return isValid;
        }
    }
}
