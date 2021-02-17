using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Support;
using Data;

namespace EMS_2.Patients
{
    public partial class PatientValidation
    {
        /**
        * \brief Validates a health card identification number's format.
        * \param string data - the given HCN
        * \return bool - true if valid
        * \author Kieron Higgs, Kyle Horsley
        */
        public static bool ValidateHCN(string data)
        {
            if (!Constants.hcnRegex.IsMatch(data) ||
                data.Length > 12)
            {
                return false;
            }
            return true;
        }

        /**
        * \brief Validates a NEW health card identification number for entry into the database (for entering NEW patients).
        * \param string data - the given HCN
        * \return bool - true if valid
        * \author Kieron Higgs, Kyle Horsley
        */
        public static bool ValidateNewHCN(string data)
        {
            if (!Database.Patients.ContainsKey(data))
            {
                return false;
            }
            return true;
        }

        /**
        * \brief Validates a new first name field for a patient to be modified or added to the database.
        * \param string data - the given first name
        * \return bool - true if valid
        * \author Kyle Horsley
        */
        public static bool ValidateFirstName(string data)
        {
            if (!Constants.nameRegex.IsMatch(data))
            {
                return false;
            }
            return true;
        }

        /**
        * \brief Validates a new last name field for a patient to be modified or added to the database.
        * \param string data - the given last name
        * \return bool - true if valid
        * \author Kyle Horsley
        */
        public static bool ValidateLastName(string data)
        {
            if (!Constants.nameRegex.IsMatch(data))
            {
                return false;
            }
            return true;
        }

        /**
        * \brief Validates a new middle initial field for a patient to be modified or added to the database.
        * \param string data - the given middle initial
        * \return bool - true if valid
        * \author Kieron Higgs
        */
        public static bool ValidateMInitial(string data)
        {
            if (!Constants.nameRegex.IsMatch(data))
            {
                return false;
            }
            if (data.Length > 1)
            {
                return false;
            }

            return true;
        }


        /**
        * \brief Validates a new date of birth field for a patient to be modified or added to the database.
        * \param string data - the given date of birth
        * \return bool - true if valid
        * \author Kieron Higgs
        */
        public static bool ValidateDOB(string data)
        {
            if (String.IsNullOrWhiteSpace(data))
            {
                return false;
            }

            string[] MMDDYYYY = data.Split('-', '/');

            if (MMDDYYYY.Length != 3 ||
               MMDDYYYY[0].Length != 2 || !Constants.numRegex.IsMatch(MMDDYYYY[0]) ||
               MMDDYYYY[1].Length != 2 || !Constants.numRegex.IsMatch(MMDDYYYY[1]) ||
               MMDDYYYY[2].Length != 4 || !Constants.numRegex.IsMatch(MMDDYYYY[2]) ||
               Convert.ToInt32(MMDDYYYY[0]) > 12)
            {
                return false;
            }


            DateTime givenDate = new DateTime(Convert.ToInt32(MMDDYYYY[2]),
                                              Convert.ToInt32(MMDDYYYY[0]),
                                              Convert.ToInt32(MMDDYYYY[1]));
            if (!Constants.dobRegex.IsMatch(data) || givenDate > DateTime.Today)
            {
                return false;
            }
            return true;
        }

        /**
        * \brief Validates a new gender field for a patient to be modified or added to the database.
        * \param string data - the given gender
        * \return bool - true if valid
        * \author Kyle Horsley, Kieron Higgs
        */
        public static bool ValidateGender(string data)
        {
            if (string.Equals(data, "M") || string.Equals(data, "F") || string.Equals(data, "X") || string.Equals(data, "x") ||
                string.Equals(data, "m") || string.Equals(data, "f"))
            {
                return true;
            }
            else
            {
                return false;

            }
        }

        /**
        * \brief Validates a new HoH field for a patient to be modified or added to the database.
        * \param string data - the given HoH
        * \return bool - true if valid
        * \author Kieron Higgs
        */
        public static bool ValidateHoH(string data)
        {
            //Aslso needd to check if its in the datbase
            if ( !Constants.hcnRegex.IsMatch(data) || data.Length > 12)
            {
                return false;
            }
            else if (!Database.HoH_Exists(data))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /**
        * \brief Validates a new address line 1 field for a patient to be modified or added to the database.
        * \param string data - the given address line 1
        * \return bool - true if valid
        * \author Kyle Horsley, Kieron Higgs
        */
        public static bool ValidateAddressLine1(string data)
        {
            if (Constants.addressRegex.IsMatch(data))
            {
                return true;
            }
            return false; ;
        }

        /**
        * \brief Validates a new address line 2 field for a patient to be modified or added to the database.
        * \param string data - the given address line 2
        * \return bool - true if valid
        * \author Kyle Horsley, Kieron Higgs
        */
        public static bool ValidateAddressLine2(string data)
        {
            return true;
        }

        /**
        * \brief Validates a new city field for a patient to be modified or added to the database.
        * \param string data - the given city
        * \return bool - true if valid
        * \author Kyle Horsley, Kieron Higgs
        */
        public static bool ValidateCity(string data)
        {
            if (!Constants.cityRegex.IsMatch(data))
            {
                return true;
            }
            return false;
        }

        /**
        * \brief Validates a new postal code field for a patient to be modified or added to the database.
        * \param string data - the given postal code
        * \return bool - true if valid
        * \author Kyle Horsley, Kieron Higgs
        */
        public static bool ValidatePostalCode(string data)
        {
            if (data == null)
            {
                return true;
            }
            if (!Constants.pCodeRegex.IsMatch(data))
            {
                return false;
            }
            return true;
        }

        /**
        * \brief Validates a new province field for a patient to be modified or added to the database.
        * \param string data - the given province
        * \return bool - true if valid
        * \author Kieron Higgs
        */
        public static bool ValidateProv(string data)
        {
            if (!Constants.provAbbr.Contains(data))
            {
                return false;
            }
            return true;
        }

        /**
        * \brief Validates a new phone number field for a patient to be modified or added to the database.
        * \param string data - the given phone number
        * \return bool - true if valid
        * \author Kyle Horsley
        */
        public static bool ValidatePhone(string data)
        {
            if (!Constants.phoneNumRegex.IsMatch(data))
            {
                return false;
            }
            return true;
        }
    }
}
