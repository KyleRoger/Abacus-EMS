/*
 * 
 * Author:      Kieron Higgs, Arie Kraayenbrink
 * Date:        12-10-2018
 * Project:     EMS1
 * File:        Support - Logging.cs
 * Description: Contains the code pertaining to the Logging class used in the EMS application.
 * 
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Demographics;

namespace Support
{
    /// <summary>
    /// The Logging class contains all of the methods used for writing logs to document system processes.
    /// 
    /// Fault Detection: If the necessary logging files are not present, an exception is thrown and caught, and the exception message
    /// is displayed.
    /// Relationships: Logging is used by many classes but it does not use them. Some specialized logging methods are for Appointment-related issues.
    /// </summary>
    public class Logging
    {
        /**
        * \brief The default log for the EMS application; writes a log with the date and calling method, and a passed status string.
        * \param string loggingEvent - the status string describing the event
        * \param string callingFunction - the name of the method using the logger
        * \author Kieron Higgs
        */
        public static void Write(string loggingEvent,
                                    [System.Runtime.CompilerServices.CallerMemberName] string callingFunction = "")
        {
            DateTime date = DateTime.Now;
            string logPath = @".\log\ems." + date.ToString("yyyy-MM-dd") + ".log";

            if (!File.Exists(logPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(@".\log\");
                    File.WriteAllText(logPath, "");
                }
                // In case of error, catch the exception for use by the Logging class:
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            string log = date.ToString("dd-MM-yyyy") + " "
                       + date.ToString("T") + " ["
                       + callingFunction + "] - "
                       + loggingEvent + ".\n";

            using (StreamWriter LogWriter = File.AppendText(logPath))
            {
                LogWriter.WriteLine(log);
            }
        }





        //public static void BookingLog(string patientHCN, string hohHCN, DateTime date,
        //                            [System.Runtime.CompilerServices.CallerMemberName] string callingFunction = "")
        //{
        //    string log = String.Format("Booked an appointment for {0}", GetNamesForAppointment(patientHCN, hohHCN));

        //    log += String.Format(" on {0}", date.ToShortDateString());

        //    Write(log, callingFunction);
        //}



        //public static void AppointmentFlagLog(string patientHCN, string hohHCN, int recallFlag, DateTime date,
        //                            [System.Runtime.CompilerServices.CallerMemberName] string callingFunction = "")
        //{
        //    string log = String.Format("Update appointment flag for {0}", GetNamesForAppointment(patientHCN, hohHCN));

        //    log += String.Format(" on {0} to {1} week(s)", date.ToShortDateString(), recallFlag);

        //    Write(log, callingFunction);
        //}



        //public static void AppointmentUpdateLog(string patientHCN, string hohHCN, DateTime oldDate, DateTime newDate,
        //                            [System.Runtime.CompilerServices.CallerMemberName] string callingFunction = "")
        //{
        //    string log = String.Format("Updated the appointment for {0}", GetNamesForAppointment(patientHCN, hohHCN));

        //    log += String.Format(" from {0} to {1}", oldDate, newDate);

        //    Write(log, callingFunction);
        //}


        //public static void AppointmentDeletionLog(string patientHCN, string hohHCN, DateTime date,
        //                            [System.Runtime.CompilerServices.CallerMemberName] string callingFunction = "")
        //{
        //    string log = String.Format("Deleted the appointment for {0}", GetNamesForAppointment(patientHCN, hohHCN));

        //    log += String.Format(" on {0}", date.ToShortDateString());

        //    Write(log, callingFunction);
        //}


        //static string GetNamesForAppointment(string patientHCN, string hohHCN)
        //{
        //    string names = String.Format("{0} ({1})", Patients.GetNameFromHCN(patientHCN), patientHCN);

        //    if (hohHCN != null)
        //    {
        //        names += String.Format(", accompanied by {0} ({1})", Patients.GetNameFromHCN(hohHCN), hohHCN);
        //    }

        //    return names;
        //}


        //public static void BillCodeRemovalLog(string patientHCN, string hohHCN, string billCode, DateTime date,
        //                            [System.Runtime.CompilerServices.CallerMemberName] string callingFunction = "")
        //{
        //    string log = String.Format("Removed the bill code '{0}' from the appointment for {1}", billCode, GetNamesForAppointment(patientHCN, hohHCN));

        //    log += String.Format(" on {0}", date.ToShortDateString());

        //    Write(log, callingFunction);
        //}
    }
}