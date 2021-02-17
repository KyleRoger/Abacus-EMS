/*
 * 
 * Author:      Kieron Higgs
 * Date:        04-22-2018
 * Project:     EMS 2
 * File:        Factory.cs
 * Description: Contains the methods used by the Factory class to instantiate Patient and Appointment objects. The factory pattern used here
 *              encapsulates object creation and simplifies it by taking String arrays as parameters and returning the appropriate object
 *              according to how many Strings are in the array.
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demographics;
using Scheduling;
using Data;

namespace Factory
{
    /**
     * 
     * Class:           PatientFactory
     * Purpose:         To create Patient objects.
     * Attributes:      [none]
     * Relationships:   Called upon by methods in the Data class and elsewhere when Patient objects need to be instantiated, either via the menu by the user or
     *                  in-program on startup or whenever the object containers are refreshed.
     * Fault detection: Most methods return null if the passed String array contains an improper number of Strings.
     */
    public class PatientFactory
    {
        ///----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn public static Patient Create(String[] attributes)
        ///
        /// \brief  Creates a Patient object.
        ///         
        /// \description    Creates a Patient object based on an array of strings. This method will either be called by the program during usage, when the user
        ///                 needs to insert new Patients.
        ///
        /// \author Kieron Higgs
        ///
        /// \param  attributes    The array of strings containing the attributes to be assigned to the Patient object when it is instantiated.
        ///----------------------------------------------------------------------------------------------------------------------------------------------------
        public static Patient Create(String[] attributes)
        {
            // During startup - if there are 7 Strings in the array, it represents a Dependant:
            if (attributes.Length == 7)
            {
                return new Patient_Dependant(attributes);
            }
            // During startup - if there are 12 Strings in the array, it represents a Head-Of-Household:
            else if (attributes.Length == 12)
            {
                return new Patient_HoH(attributes);
            }
            // During usage - there will always be 13 Strings during usage:
            else if (attributes.Length == 13)
            {
                // If the given array represents a Dependant Patients, the sixth string will not be null (it will have the HoH_HCN in it instead):
                if (!String.IsNullOrEmpty(attributes[6]))
                {
                   return new Patient_Dependant(new String[] { attributes[0], attributes[1], attributes[2], attributes[3], attributes[4], attributes[5], attributes[6] });
                  
                }
                // Otherwise, the array represents a Head-Of-Household:
                else
                {
                    return new Patient_HoH(new String[] { attributes[0], attributes[1], attributes[2], attributes[3], attributes[4], attributes[5],
                                                          attributes[7], attributes[8], attributes[9], attributes[10], attributes[11], attributes[12]});
                }
            }
            // Error - Incorrect usage:
            else
            {
                return null;
            }
        }
    }

    /**
     * 
     * Class:           AppointmentFactory
     * Purpose:         To create Appointment objects.
     * Attributes:      [none]
     * Relationships:   Called upon by methods in the Data class and elsewhere when Appointment objects need to be instantiated, either via the menu by the user or
     *                  in-program on startup or whenever the object containers are refreshed.
     * Fault detection: Most methods return null if the passed String array contains an improper number of Strings.
     */
    public class AppointmentFactory
    {
        ///---------------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn public static Appointment Create(String[] attributes)
        ///
        /// \brief  Creates an Appointment object.
        ///         
        /// \description    Creates an Appointment object based on an array of strings. This method will either be called by the program during usage, when the user
        ///                 needs to make new Appointments.
        ///
        /// \author Kieron Higgs
        ///
        /// \param  attributes    The array of strings containing the attributes to be assigned to the Appointment object when it is instantiated.
        ///---------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Appointment Create(String[] attributes)
        {
            if (attributes.Length == 3)
            {
                return new Appointment_Single(attributes);
            }
            else if (attributes.Length == 4)
            {
                return new Appointment_Double(attributes);
            }
            else
            {
                return null;
            }
        }

        ///-------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn public static Appointment Create(String[] attributes, List<String> billcodes)
        ///
        /// \brief  Creates an Appointment object.
        ///         
        /// \description    Creates an Appointment object based on an array of strings. This overload is called when the program imports Appointments from the database,
        ///                 to create Appointment_Single objects (Appointments with only one Patient attending).
        ///
        /// \author Kieron Higgs
        ///
        /// \param  attributes    The array of strings containing the attributes to be assigned to the Appointment object when it is instantiated.
        /// \param  billcodes     The list of billcodes for the Patient.
        ///-------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Appointment Create(String[] attributes, List<String> billcodes)
        {
            return new Appointment_Single(attributes, billcodes);
        }

        ///-------------------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn public static Appointment Create(String[] attributes, List<String> billcodes, List<String> secondaryBillcodes)
        ///
        /// \brief  Creates an Appointment object.
        ///         
        /// \description    Creates an Appointment object based on an array of strings. This overload is called when the program imports Appointments from the database,
        ///                 to create Appointment_Double objects (Appointments with two Patients attending).
        ///
        /// \author Kieron Higgs
        ///
        /// \param  attributes          The array of strings containing the attributes to be assigned to the Appointment object when it is instantiated.
        /// \param  billcodes           The list of billcodes for the first Patient.
        /// \param  secondaryBillcodes  The list of billcodes for the second Patient.
        ///-------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Appointment Create(String[] attributes, List<String> billcodes, List<String> secondaryBillcodes)
        {
            return new Appointment_Double(attributes, billcodes, secondaryBillcodes);
        }
    }
}
