/*
 * 
 * Author:      Kieron Higgs
 * Date:        04-22-2018
 * Project:     EMS 2
 * File:        Patient.cs
 * Description: Contains the class definitions including attributes and methods for the Patient superclass and its two subclasses,
 *              Patient_HoH and Patient_Dependant. The methods include overrides for Patient addition and updates, as well as checks
 *              for HoH/Dependant status amongst other things. The base class is an abstract class: any Patient which is instantiated
 *              must be one of the two subclasses.
 * 
*/

using System;
using Data;

namespace Demographics
{
    /**
     *
     * Class:           Patient
     * Purpose:         To model Patients as objects.
     * Relationships:   Some other classes have knowledge of the Patient class, but the Patient class only knows the basic database class,
     *                  in order to facilitate overloads for the different subclasses
     * Fault detection: Fault detection takes place elsewhere, mostly in the Database class when it relates to the Patient class.
     */
    public abstract class Patient
    {
        private String hcn;
        public String HCN
        {
            get { return this.hcn; }
            set { this.hcn = value; }
        }
        private String lastName;
        public String LastName
        {
            get { return this.lastName; }
            set { this.lastName = value; }
        }
        private String firstName;
        public String FirstName
        {
            get { return this.firstName; }
            set { this.firstName = value; }
        }
        private String mInitial;
        public String MInitial
        {
            get { return this.mInitial; }
            set { this.mInitial = value; }

        }
        private DateTime dob;
        public DateTime DoB
        {
            get { return this.dob; }
            set { this.dob = value; }
        }

        private char gender;
        public char Gender
        {
            get { return this.gender; }
            set { this.gender = value; }
        }

        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Patient()
        ///
        /// \brief  Instantiates a default Patient object.
        ///         
        /// \description    A constructor made available for the purposes of creating Patient objects on-the-fly when the desired  attributes are not yet known.
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public Patient()
        {
            HCN = null;
            LastName = null;
            FirstName = null;
            MInitial = null;
            DoB = default(DateTime);
            Gender = '\0';
        }

        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Patient(String[] attributes)
        ///
        /// \brief  Instantiates a default Patient object.
        ///         
        /// \description    The intended instructor for all base Patient class instantiation. The given String array *should* have more attributes in it than
        ///                 are used in this constructor, to be passed to the subclass constructor for further attribute assignment.
        ///
        /// \author Kieron Higgs
        /// 
        /// \param String[] attributes
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public Patient(String[] attributes)
        {
            HCN = attributes[0];
            LastName = attributes[1];
            FirstName = attributes[2];
            MInitial = attributes[3];
            DoB = DateTime.Parse(attributes[4]);
            Gender = attributes[5][0];
        }

        public abstract void Add();
        public abstract void Update();
        public abstract String GetAddress();
        public abstract String GetPhone();
        public abstract String GetHoH();
        public abstract bool IsHoH();
    }
    /**
     *
     * Class:           Patient_Dependant
     * Purpose:         To extend the Patient object to represent a Dependant.
     * Relationships:   Some other classes have knowledge of the Patient class, but the Patient class only knows the basic database class,
     *                  in order to facilitate overloads for the different subclasses
     * Fault detection: Fault detection takes place elsewhere, mostly in the Database class when it relates to the Patient class.
     */
    public class Patient_Dependant : Patient
    {
        private String hoh_hcn;
        public String HoH_HCN
        {
            get { return this.hoh_hcn; }
            set { this.hoh_hcn = value; }
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Patient_Dependant(String[] attributes)
        ///
        /// \brief  Instantiates a Dependant Patient object.
        ///         
        /// \description    A constructor for the Patient_Dependant class which uses the base class constructor for most of the details, and the HoH_HCN 
        ///                 attribute for linking to the Patient's head-of-household.
        ///
        /// \param attributes   The attributes (Strings) with which the object is to be instantiated.
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public Patient_Dependant(String[] attributes) : base(attributes)
        {
            HoH_HCN = attributes[6];
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Add()
        ///
        /// \brief  Adds a Dependant to the database and in-program container.
        ///         
        /// \description    The Add() override which uses the Database.Add() overload intended for Patient_Dependant objects, adding the Patient to the
        ///                 Patient Dictionary and the SQL database.
        ///
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Add()
        {
            Database.Add(this);
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Update()
        ///
        /// \brief  Updates a Dependant's details in the database.
        ///         
        /// \description    The Update() override which uses the Database.Update() overload intended for Patient_Dependant objects, updating the Patient's
        ///                 details in the SQL database.
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Update()
        {
            Database.Update(this);
        }
        ///---------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn GetAddress()
        ///
        /// \brief  Retrieves a Patient_Dependant's address.
        ///         
        /// \description    Returns a string containing the first address line of a Dependant by referencing the corresponding head-of-household object.
        ///
        /// \author Kieron Higgs
        ///---------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override String GetAddress()
        {
            Patient_HoH HoH = (Patient_HoH)Database.Patients[this.HoH_HCN];
            return HoH.AddressLine1;
        }
        ///---------------------------------------------------------------------------------------------------------------------------------------
        /// \fn GetPhone()
        ///
        /// \brief  Retrieves a Patient_Dependant's phone number.
        ///         
        /// \description    Returns a string containing the phone number of a Dependant by referencing the corresponding head-of-household object.
        ///
        /// \author Kieron Higgs
        ///---------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override String GetPhone()
        {
            Patient_HoH HoH = (Patient_HoH)Database.Patients[this.HoH_HCN];
            return HoH.Phone;
        }
        ///---------------------------------------------------------------------------------------------------------------------------------------
        /// \fn GetHoH()
        ///
        /// \brief  Retrieves the HCN of a Dependant's head-of-household.
        ///         
        /// \description    Returns a string containing the phone number of a Dependant by referencing the corresponding head-of-household object.
        ///                 used in a specific situation to detect whether a Patient object is a Dependant or HoH by HCN.
        ///
        /// \author Kieron Higgs
        ///---------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override String GetHoH()
        {
            return this.HoH_HCN;
        }
        ///---------------------------------------------------------------------------------------------------------------------------------------
        /// \fn IsHoH()
        ///
        /// \brief  Identifies whether the given Patient is a head-of-household or not.
        ///         
        /// \description    Returns a bool based on whether the given Patient object belongs to the Patient_Dependant subclass or the Patient_HoH
        ///                 subclass. In this case: false.
        ///
        /// \author Kieron Higgs
        ///---------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override bool IsHoH()
        {
            return false;
        }
    }
    /**
     *
     * Class:           Patient_HoH
     * Purpose:         To extend the Patient object to represent a head-of-household.
     * Relationships:   Some other classes have knowledge of the Patient class, but the Patient class only knows the basic database class,
     *                  in order to facilitate overloads for the different subclasses
     * Fault detection: Fault detection takes place elsewhere, mostly in the Database class when it relates to the Patient class.
     */
    public class Patient_HoH : Patient
    {
        private String addressline1;
        public String AddressLine1
        {
            get { return this.addressline1; }
            set { this.addressline1 = value; }
        }
        private String addressline2;
        public String AddressLine2
        {
            get { return this.addressline2; }
            set { this.addressline2 = value; }
        }  
        private String city;
        public String City
        {
            get { return this.city; }
            set { this.city = value; }
        }

        private String postalcode;
        public String PostalCode
        {
            get { return this.postalcode; }
            set { this.postalcode = value; }
        }

        private String province;
        public String Province
        {
            get { return this.province; }
            set { this.province = value; }
        }
        private String phone;
        public String Phone
        {
            get { return this.phone; }
            set { this.phone = value; }
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Patient_HoH(String[] attributes)
        ///
        /// \brief  Instantiates a head-of-household Patient object.
        ///         
        /// \description    A constructor for the Patient_HoH class which uses the base class constructor for some the details, and then introduces a set of
        ///                 attributes for address and phone data.
        ///
        /// \param attributes   The attributes (Strings) with which the Appointment object will be instantiated.
        /// 
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public Patient_HoH(String[] attributes) : base(attributes)
        {
            AddressLine1 = attributes[6];
            AddressLine2 = attributes[7];
            City = attributes[8];
            PostalCode = attributes[9];
            Province = attributes[10];
            Phone = attributes[11];
        }
        ///---------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Add()
        ///
        /// \brief  Adds a Patient_HoH to the database and in-program container.
        ///         
        /// \description    The Add() override which uses the Database.Add() overload intended for Patient_HoH objects, adding the given Patient to the
        ///                 Patient Dictionary and the SQL database.
        ///
        /// \author Kieron Higgs
        ///---------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Add()
        {
            Database.Add(this);
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Update()
        ///
        /// \brief  Updates a head-of-household in the database.
        ///         
        /// \description    The Update() override which uses the Database.Update() overload intended for Patient_HoH objects, updating the Patient's
        ///                 details in the SQL database.
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Update()
        {
            Database.Update(this);
        }
        ///---------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn GetAddress()
        ///
        /// \brief  Retrieves a Patient_HoH's address.
        ///         
        /// \description    Returns a string containing the first address line of a head-of-household by referencing the corresponding object.
        ///
        /// \author Kieron Higgs
        ///---------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override String GetAddress()
        {
            return this.AddressLine1;
        }
        ///---------------------------------------------------------------------------------------------------------------------------------------
        /// \fn GetPhone()
        ///
        /// \brief  Retrieves a Patient_HoH's phone number.
        ///         
        /// \description    Returns a string containing the phone number of a head-of-household Patient.
        ///
        /// \author Kieron Higgs
        ///---------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override String GetPhone()
        {
            return this.Phone;
        }
        ///---------------------------------------------------------------------------------------------------------------------------------------
        /// \fn GetHoH()
        ///
        /// \brief  Retrieves the HCN of a head-of-household.
        ///         
        /// \description    Returns a string containing the phone number of a Dependant by referencing the corresponding head-of-household object.
        ///                 used in a specific situation to detect whether a Patient object is a Dependant or HoH by HCN.
        ///
        /// \author Kieron Higgs
        ///---------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override String GetHoH()
        {
            return this.HCN;
        }
        ///---------------------------------------------------------------------------------------------------------------------------------------
        /// \fn IsHoH()
        ///
        /// \brief  Identifies whether the given Patient is a head-of-household or not.
        ///         
        /// \description    Returns a bool based on whether the given Patient object belongs to the Patient_Dependant subclass or the Patient_HoH
        ///                 subclass. In this case: true.
        ///
        /// \author Kieron Higgs
        ///---------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override bool IsHoH()
        {
            return true;
        }
    }
}