/*
 * 
 * Author:      Kieron Higgs
 * Date:        04-22-2018
 * Project:     EMS 2
 * File:        Appointment.cs
 * Description: Contains the class definitions including attributes and methods for the Appointment superclass and its two subclasses,
 *              Appointment_Single and Appointment_Double. The methods include overrides for Appointment addition and updates. The base 
 *              class is an abstract class: any Appointment which is instantiated must be one of the two subclasses.
 * 
*/

using System;
using System.Collections.Generic;
using Data;

namespace Scheduling
{
    /**
     *
     * Class:           Appointment
     * Purpose:         To model Appointments as objects.
     * Relationships:   Has knowledge of the Database class, but otherwise, the other classes use their knowledge of Appointments, not the other way around.
     * Fault detection: Fault detection occurs elsewhere, in validation methods and Database methods.
     */
    public abstract class Appointment
    {
        private int appointmentID;
        public int AppointmentID
        {
            get { return this.appointmentID; }
            set { this.appointmentID = value; }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return this.date; }
            set { this.date = value; }
        }

        private int recallFlag;
        public int RecallFlag
        {
            get { return this.recallFlag; }
            set { this.recallFlag = value; }
        }

        private int ministryFlag;
        public int MinistryFlag
        {
            get { return this.ministryFlag; }
            set { this.ministryFlag = value; }
        }

        private int mobileFlag;
        public int MobileFlag
        {
            get { return this.mobileFlag; }
            set { this.mobileFlag = value; }
        }
        ///-------------------------------------------------------------------------------------------------------------------
        /// \fn Appointment()
        ///
        /// \brief  Creates a default Appointment object.
        ///         
        /// \description    A constructor for the abstract base Appointment class for creating Appointment objects on-the-fly.
        ///
        /// \author Kieron Higgs
        ///-------------------------------------------------------------------------------------------------------------------
        ///
        public Appointment()
        {
            appointmentID = 0;
            date = default(DateTime);
            recallFlag = 0;
            ministryFlag = 0;
            mobileFlag = 0;
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------
        /// \fn Appointment()
        ///
        /// \brief  Creates an Appointment object.
        ///         
        /// \description    A constructor for the abstract base Appointment class which takes an array of Strings for the intended attributes.
        ///
        /// \param attributes   The given attributes (Strings) which will be used to instantiate the Appointment object.
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------
        ///
        public Appointment(String[] attributes)
        {
            appointmentID = int.Parse(attributes[0]);
            String[] MMDDYYYY = attributes[1].Split('-', '/', ' ');
            date = new DateTime(Int32.Parse(MMDDYYYY[0]), Int32.Parse(MMDDYYYY[1]), Int32.Parse(MMDDYYYY[2]));
        }
        public abstract void Add();
        public abstract void Import();
        public abstract void Update();
        public abstract void Delete();
        public abstract List<String> GetBillcodesByHCN(String givenHCN);
    }
    /**
     *
     * Class:           Appointment_Single
     * Purpose:         To model Appointments for one Patient, inheriting from the abstract base Appointment class.
     * Relationships:   Has knowledge of the Database class, but otherwise, the other classes use their knowledge of Appointments, not the other way around.
     * Fault detection: Fault detection occurs elsewhere, in validation methods and Database methods.
     */
    public class Appointment_Single : Appointment
    {
        private String hcn;
        public String HCN
        {
            get { return this.hcn; }
            set { this.hcn = value; }
        }

        private List<String> billcodes;
        public List<String> Billcodes
        {
            get { return this.billcodes; }
            set { this.billcodes = value; }
        }

        ///-----------------------------------------------------------------------------------------------------------------------------------
        /// \fn Appointment_Single(String[] attributes)
        ///
        /// \brief  Creates an Appointment_Single object.
        ///         
        /// \description    A constructor for the Appointment_Single class which takes an array of Strings for the intended attributes.
        ///
        /// \param attributes   The given attributes (Strings) which will be used to instantiate the Appointment object.
        /// 
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------
        ///
        public Appointment_Single(String[] attributes) : base(attributes)
        {
            RecallFlag = 0;
            MinistryFlag = 0;
            MobileFlag = 0;
            HCN = attributes[2];
            Billcodes = new List<String>();
        }

        ///-----------------------------------------------------------------------------------------------------------------------------------
        /// \fn Appointment_Single(String[] attributes)
        ///
        /// \brief  Creates an Appointment_Single object.
        ///         
        /// \description    A constructor for the Appointment_Single class which takes an array of Strings for the intended attributes, as
        ///                 a parameter and a list of billcodes pertaining to the Appointment.
        ///
        /// \param attributes   The given attributes (Strings) which will be used to instantiate the Appointment object.
        /// \param billcodes    The given billcodes which pertain to the new Appointment object.
        /// 
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------
        ///
        public Appointment_Single(String[] attributes, List<String> billcodes) : base(attributes)
        {
            RecallFlag = int.Parse(attributes[2]);
            MinistryFlag = int.Parse(attributes[3]);
            MobileFlag = int.Parse(attributes[4]);
            HCN = attributes[5];
            Billcodes = new List<String>();
            foreach (String billcode in billcodes)
            {
                Billcodes.Add(billcode);
            }
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Add()
        ///
        /// \brief  Adds an Appointment to the database and in-program container.
        ///         
        /// \description    The Add() override which uses the Database.Add() overload intended for Appointment_Single objects, adding the Appointment to the
        ///                 Appointment Dictionary and the SQL database.
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Add()
        {
            Database.Add(this);
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Import()
        ///
        /// \brief  "Imports" an Appointment by adding ito the in-program container.
        ///         
        /// \description    Adds the Appointment upon which .Import() is called an adds it to the in-program container. Used to distinguish between Appointments
        ///                 bineg "imported" vs. "added."
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Import()
        {
            Database.AddToContainer(this);
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Update()
        ///
        /// \brief  Updates a Appointment's details in the database.
        ///         
        /// \description    The Update() override which uses the Database.Update() overload intended for Appointment_Single objects, updating the Appointment's
        ///                 details in the SQL database.
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Update()
        {
            Database.Update(this);
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Delete()
        ///
        /// \brief  Deletes an Appointment from the database.
        ///         
        /// \description    The Delete() override which uses the Database.Update() overload intended for Appointment_Single objects, deleting the Appointment
        ///                 from the SQL database and removing it from the Dictionary.
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Delete()
        {
            Database.Delete(this);
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn GetBillcodesByHCN(String givenHCN)
        ///
        /// \brief  Retrieves a Patient's billcodes from an Appointment's list.
        ///         
        /// \description    Takes an HCN and returns the list of billcodes pertaining to the Patient to which the HCN belongs, who is attending the Appointment.
        ///
        /// \param givenHCN     The HCN of the Patient to which the desired billcode list belongs.
        /// 
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override List<String> GetBillcodesByHCN(String givenHCN)
        {
            if (this.hcn == givenHCN)
            {
                return Billcodes;
            }
            else
            {
                return null;
            }
        }
    }
    /**
     *
     * Class:           Appointment_Double
     * Purpose:         To model Appointments for two Patients, inheriting from the abstract base Appointment class.
     * Relationships:   Has knowledge of the Database class, but otherwise, the other classes use their knowledge of Appointments, not the other way around.
     * Fault detection: Fault detection occurs elsewhere, in validation methods and Database methods.
     */
    public class Appointment_Double : Appointment_Single
    {
        private String secondaryHCN;
        public String SecondaryHCN
        {
            get { return this.secondaryHCN; }
            set { this.secondaryHCN = value; }
        }

        private List<String> secondaryBillcodes;
        public List<String> SecondaryBillcodes
        {
            get { return this.secondaryBillcodes; }
            set { this.secondaryBillcodes = value; }
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------
        /// \fn Appointment_Double(String[] attributes)
        ///
        /// \brief  Creates an Appointment_Double object.
        ///         
        /// \description    A constructor for the Appointment_Double class which takes an array of Strings for the intended attributes as a
        ///                 parameter.
        ///
        /// \param attributes   The given attributes (Strings) which will be used to instantiate the Appointment object.
        /// 
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------
        ///
        public Appointment_Double(String[] attributes) : base(attributes)
        {
            SecondaryHCN = attributes[3];
            SecondaryBillcodes = new List<String>();
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------
        /// \fn Appointment_Double(String[] attributes)
        ///
        /// \brief  Creates an Appointment_Double object.
        ///         
        /// \description    A constructor for the Appointment_Double class which takes an array of Strings for the intended attributes, as
        ///                 a parameter and two lists of billcodes pertaining to the Appointment.
        ///
        /// \param attributes   The given attributes (Strings) which will be used to instantiate the Appointment object.
        /// \param billcodes    The first list of billcodes pertaining to the Appointment.
        /// \param billcodes    The second list of billcodes, attached to the second Patient attending the appointment.
        /// 
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------
        ///
        public Appointment_Double(String[] attributes, List<String> billcodes, List<String> secondaryBillcodes) : base(attributes, billcodes)
        {
            SecondaryHCN = attributes[6];
            SecondaryBillcodes = new List<String>();
            foreach (String secondaryBillcode in secondaryBillcodes)
            {
                SecondaryBillcodes.Add(secondaryBillcode);
            }
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Add()
        ///
        /// \brief  Adds an Appointment to the database and in-program container.
        ///         
        /// \description    The Add() override which uses the Database.Add() overload intended for Appointment_Double objects, adding the Appointment to the
        ///                 Appointment Dictionary and the SQL database.
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Add()
        {
            Database.Add(this);
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Import()
        ///
        /// \brief  "Imports" an Appointment by adding ito the in-program container.
        ///         
        /// \description    Adds the Appointment upon which .Import() is called an adds it to the in-program container. Used to distinguish between Appointments
        ///                 bineg "imported" vs. "added."
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Import()
        {
            Database.AddToContainer(this);
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Update()
        ///
        /// \brief  Updates a Appointment's details in the database.
        ///         
        /// \description    The Update() override which uses the Database.Update() overload intended for Appointment_Single objects, updating the Appointment's
        ///                 details in the SQL database.
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Update()
        {
            Database.Update(this);
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn Delete()
        ///
        /// \brief  Deletes an Appointment from the database.
        ///         
        /// \description    The Delete() override which uses the Database.Update() overload intended for Appointment_Double objects, deleting the Appointment
        ///                 from the SQL database and removing it from the Dictionary.
        ///
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override void Delete()
        {
            Database.Delete(this);
        }
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        /// \fn GetBillcodesByHCN(String givenHCN)
        ///
        /// \brief  Retrieves a Patient's billcodes from an Appointment's list.
        ///         
        /// \description    Takes an HCN and returns the list of billcodes pertaining to the Patient to which the HCN belongs, who is attending the Appointment.
        ///
        /// \param givenHCN     The HCN of the Patient to which the desired billcode list belongs.
        /// 
        /// \author Kieron Higgs
        ///-----------------------------------------------------------------------------------------------------------------------------------------------------
        ///
        public override List<String> GetBillcodesByHCN(String givenHCN)
        {
            if (this.HCN == givenHCN)
            {
                return Billcodes;
            }
            else if (this.secondaryHCN == givenHCN)
            {
                return SecondaryBillcodes;
            }
            else
            {
                return null;
            }
        }
    }
    /**
     *
     * Class:           MobileRequest
     * Purpose:         To model a request made via mobile device for an appointment.
     * Relationships:   Pretty insular--used in one menu tab for gathering/applying mobile requests.
     * Fault detection: Fault detection occurs elsewhere, in validation methods and Database methods.
     */
    public class MobileRequest
    {
        private String hcn;
        public String HCN
        {
            get
            {
                return this.hcn;
            }
            set
            {
                this.hcn = value;
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get
            {
                return this.date;
            }
            set
            {
                this.date = value;
            }
        }

        public MobileRequest(String[] attributes)
        {
            HCN = attributes[0];
            String[] MMDDYYYY = attributes[1].Split('-', '/');
            Date = new DateTime(Int32.Parse(MMDDYYYY[0]), Int32.Parse(MMDDYYYY[1]), Int32.Parse(MMDDYYYY[2]));
        }
    }
}
