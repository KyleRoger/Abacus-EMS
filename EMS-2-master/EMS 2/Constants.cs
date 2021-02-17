/*
 * 
 * Author:      Arie Kraayenbrink, Kieron Higgs
 * Date:        12-10-2018
 * Project:     EMS1
 * File:        Constants.cs
 * Description: This file holds the constants for the EMS1 project.
 * 
*/



using System.Collections.Generic;
using System.Text.RegularExpressions;



namespace Support
{
    /**
     * 
     * Class:           Constants
     * Purpose:         This class contains constants that are used in the EMS1 project. All members are static read-only so they 
     *                  can not be changed by outside sources.
     * Attributes:
     * Relationships:   This class is used by other classes as needed throughout the EMS1 but is not in itself tied to any class.
     *                  It has no dependencies on any others.
     * Fault detection: A visual inspection for spelling errors and typos.
     */
    public class Constants
    {
        //menu stuff
        public static readonly string[] mainMenuButtons = { "Calendar F2", "Patients F3", "Billing F4" };
        public static readonly string[] calMenuButtonsDoctor = { "Flag recall", "Billing Codes" };
        public static readonly string[] calMenuButtons = { "" };
        public static readonly string[] patientMenuButtons = { "Add F5", "Search F6", "Modify F7", "View Appointments F8", "View Household F9" };
        public static readonly string[] billingMenuButtons = { "Export MOH file F5", "Import MOH file F6", "View MOH file F7", "Generate Monthly Billing Report F8", " View Bill Codes F9" };

        //pages
        public enum Page { Help, startPage, Calendar, Patients, Billing, Billing_Export, Billing_Import, Billing_View, Billing_Generate_Report, Bill_ViewBillCodes, Bill_Settings, updateScreen, Scheduling_Graphics, F5, F6, F7, F8, F9, F10 };

        //file paths
        public static readonly string[] DBFiles = { @".\DBase", @".\DBase\patients.txt", @".\DBase\appointments.txt" };
        public static readonly string logDir = @".\log\";
        public static readonly string monthlyBill = @"Billing_File";
        public static readonly string fileExt = ".txt";
        public static readonly string dbDirectory = @".\DBase";  //as per requirements
        public static readonly string testBillCode = @".\MasterSample.txt"; //Testing only

        // Validation Regexes:
        public static readonly Regex nameRegex = new Regex(@"^[a-zA-Z'-]+$");
        public static readonly Regex dobRegex = new Regex("^(0[1-9]|1[012])[-/. ](0[1-9]|[12][0-9]|3[01])[-/. ](19|20)\\d\\d$");
        public static readonly Regex addressRegex = new Regex("^\\d+\\s[a-zA-Z]+\\s[a-zA-Z]+");
        public static readonly Regex cityRegex = new Regex("/^[a-zA-Z ,.'-]+$/i");
        public static readonly Regex hcnRegex = new Regex(@"[0-9]{10}[A-Za-z]{2}");
        public static readonly Regex pCodeRegex = new Regex("[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ] ?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]");
        public static readonly Regex phoneNumRegex = new Regex(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$");
        public static readonly Regex numRegex = new Regex("^[0-9]+$");

        // Population filepaths and sample data:
        public static readonly List<string> DBGen = new List<string> { "EMS1.Populate.firstNames.txt",  "EMS1.Populate.lastNames.txt",
                                                    "EMS1.Populate.streetNames.txt", "EMS1.Populate.cityNames.txt" };
        public static readonly string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public static readonly string[] nums = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        public static readonly string[] gender = { "M", "F", "X" };
        public static readonly string[] streetSuffix = { "Street", "Road", "Cresent", "Circle", "Alley", "Boulevard", "Lane", "Avenue", "Drive", "Parkway", "Court" };
        public static readonly string[] addressUnit = { "Unit", "Apt" };
        public static readonly string[] provAbbr = { "NL", "PE", "NS", "NB", "QC", "ON", "MB", "SK", "AB", "BC", "YT", "NT", "NU" };

        // Database import SQL queries (will edit to get rid of '*' eventually):
        public static readonly string patientQuery = "SELECT HCN, LastName, FirstName, MInitial, DoB, Gender FROM Patient;";
        public static readonly string dependantQuery = "SELECT Dependant_HCN, HoH_HCN FROM Patient_Dependant;";
        public static readonly string HoHQuery = "SELECT HoH_HCN, AddressLine1, AddressLine2, City, PostalCode, Prov, Phone FROM Patient_HoH;";
        public static readonly string appointmentQuery = "SELECT AppointmentID, Date, RecallFlag, MinistryFlag, MobileFlag FROM Appointment;";
        public static readonly string attendeeQuery = "SELECT Attendee_HCN, AppointmentID FROM Appointment_Attendee;";
        public static readonly string billcodeQuery = "SELECT AppointmentID, Attendee_HCN, Code FROM Appointment_Billcode;";
    }
}