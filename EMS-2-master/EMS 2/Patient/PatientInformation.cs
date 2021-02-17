using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace EMS_2.Patients
{
    class PatientInformation
    {
        private string hcn;
        public string HCN
        {
            get { return this.hcn; }
            set { this.hcn = value; }
        }

        private string firstname;
        public string FirstName
        {
            get { return this.firstname; }
            set { this.firstname = value; }
        }

        private string lastname;
        public string LastName
        {
            get { return this.lastname; }
            set { this.lastname = value; }
        }

        private string minitial;
        public string MInitial
        {
            get { return this.minitial; }
            set { this.minitial = value; }
        }

        private string dob;
        public string DOB
        {
            get { return this.dob; }
            set { this.dob = value; }
        }

        private string gender;
        public string Gender
        {
            get { return this.gender; }
            set { this.gender = value; }
        }

        private string hoh;
        public string HoH
        {
            get { return this.hoh; }
            set { this.hoh = value; }
        }

        private string addressline1;
        public string AddressLine1
        {
            get { return this.addressline1; }
            set { this.addressline1 = value; }
        }

        private string addressline2;
        public string AddressLine2
        {
            get { return this.addressline2; }
            set { this.addressline2 = value; }
        }

        private string city;
        public string City
        {
            get { return this.city; }
            set { this.city = value; }
        }

        private string postalcode;
        public string PostalCode
        {
            get { return this.postalcode; }
            set { this.postalcode = value; }
        }

        private string prov;
        public string Prov
        {
            get { return this.prov; }
            set { this.prov = value; }
        }

        private string phone;
        public string Phone
        {
            get { return this.phone; }
            set { this.phone = value; }
        }


        /**
        * \brief Default constructor for the Patients class. Initalizes the variables to 'empty' defaults.
        * \author Kieron Higgs
        */
        public PatientInformation()
        {
            HCN = null;
            LastName = null;
            FirstName = null;
            MInitial = null;
            DOB = null;
            Gender = null;
            HoH = null;
            AddressLine1 = null;
            AddressLine2 = null;
            City = null;
            PostalCode = null;
            Prov = null;
            Phone = null;
        }

        public PatientInformation(PatientInformation originalPatient)
        {
            HCN = originalPatient.HCN;
            LastName = originalPatient.LastName;
            FirstName = originalPatient.FirstName;
            MInitial = originalPatient.MInitial;
            DOB = originalPatient.DOB;
            Gender = originalPatient.Gender;
            HoH = originalPatient.HoH;
            AddressLine1 = originalPatient.AddressLine1;
            AddressLine2 = originalPatient.AddressLine2;
            City = originalPatient.City;
            PostalCode = originalPatient.PostalCode;
            Prov = originalPatient.Prov;
            Phone = originalPatient.Phone;
        }

        public bool ValidateAddPatienInfo(List<string> errors, PatientInformation patient)
        {
            bool isValid = true;
            
            if(!PatientValidation.ValidateFirstName(patient.firstname))
            {
                isValid = false;
                errors.Add("Error: Invalid First Name.");
            }
            if (!PatientValidation.ValidateLastName(patient.LastName))
            {
                isValid = false;
                errors.Add("Error: Invalid Last Name.");
            }
            if (!PatientValidation.ValidateHCN(patient.HCN))
            {
                isValid = false;
                errors.Add("Error: Invalid Health Card Number.");
            }
            if (!PatientValidation.ValidateMInitial(patient.MInitial))
            {
                isValid = false;
                errors.Add("Error: Invalid Middle Initial.");
            }
            if (!PatientValidation.ValidateDOB(patient.DOB))
            {
                isValid = false;
                errors.Add("Error: Invalid Birth Date.");
            }
            return isValid;
        }
    }
}
