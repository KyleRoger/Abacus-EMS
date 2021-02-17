// file:	Patient\ModifyPatient.xaml.cs
// Author:  Kyle Horsley
// Date:    April 20 ,2019
// summary:	Implements the add ModifyPatient.xaml class

using Demographics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Data;
using Factory;

namespace EMS_2.Patients
{
    /// This is the ModifyPatient class with all of its methods.
    /// 
    /// Name:               ModifyPatient      
    /// 
    /// Purpose:            To provide the user functionality to Modify a patient and their information
    /// 
    /// Fault Detection:    This file uses Regexs to ensure proper information is being inputted before submitting to the database. 
    /// 
    /// 
    /// Relationships:      This class is related to the Database class and uses it to modify the patients. 

    public partial class ModifyPatient : UserControl
    {
        /// <summary>   The gender. </summary>
        char Gender = ' ';
        //ModifyPatientSearchOverlay patientToMod;
        /// <summary>   The patient. </summary>
        Patient patient;

        /// <summary>   The patient hoh. </summary>
        Patient_HoH patientHoh;
        /// <summary>   The patient dependant. </summary>
        Patient_Dependant patientDependant;
        /// <summary>   The patient to modifier. </summary>
        ModifyPatientSearchOverlay patientToMod;
        /// <summary>   The fill patient. </summary>
        List<string> fillPatient = new List<string>();
        /// <summary>   The hoh to dependant. </summary>
        string[] hohToDependant =new string[7];
        /// <summary>   The dependant to hoh. </summary>
        string[] dependantToHoh = new string[12];
        /// <summary>   Zero-based index of the. </summary>
        int i;

        /// <summary>   True if name entered. </summary>
        bool fNameEntered = true;
        /// <summary>   True if name entered. </summary>
        bool lNameEntered = true;
        /// <summary>   True if hcn entered. </summary>
        bool hcnEntered = true;
        /// <summary>   True if dob entered. </summary>
        bool dobEntered = true;
        /// <summary>   True if gender entered. </summary>
        bool genderEntered = true;
        /// <summary>   True if initial entered. </summary>
        bool mInitialEntered = true;

        /// <summary>   True to hoh entered bool. </summary>
        bool hohEnteredBool = true;
        /// <summary>   True to address bool. </summary>
        bool addressBool = true;
        /// <summary>   True to address two bool. </summary>
        bool addressTwoBool = true;
        /// <summary>   True to code bool. </summary>
        bool pCodeBool = true;
        /// <summary>   True to city bool. </summary>
        bool cityBool = true;
        /// <summary>   True to number bool. </summary>
        bool pNumberBool = true;

        ///
        /// <summary>   Default constructor. Initiates the provinces and starts a seach instants</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///

        public ModifyPatient()
        {
            
            InitializeComponent();

            i = 0;
            //initialize the province textbox.
            provComboBox.Items.Add("--Please Select--");
            provComboBox.Items.Add("AB");
            provComboBox.Items.Add("BC");
            provComboBox.Items.Add("MB");
            provComboBox.Items.Add("NB");
            provComboBox.Items.Add("NL");
            provComboBox.Items.Add("NS");
            provComboBox.Items.Add("ON");
            provComboBox.Items.Add("PE");
            provComboBox.Items.Add("QC");
            provComboBox.Items.Add("SK");
            provComboBox.Items.Add("NT");
            provComboBox.Items.Add("NU");
            provComboBox.Items.Add("YT");
            provComboBox.SelectedIndex = 0;

            StartNewSearch();

            hcnTextBox.IsEnabled = false;

        }

        ///
        /// <summary>   Starts new search from ModifySearchPatientsOverlay</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///

        public void StartNewSearch()
        {

           patientToMod = new ModifyPatientSearchOverlay("Modify");//New modifying search screen

            ModPatientSearchParent.Children.Add(patientToMod); //add the overlay
        }

        ///
        /// <summary>   Returns a patient from the search screen </summary>
        ///
        /// <remarks>    2019-04-20. </remarks>
        ///
        /// <param name="p">    A Patient to process. </param>
        ///

        public void ReturnPatient(Demographics.Patient p)
        {
            patient = p; //Fill the patient with the returned search information

            ModPatientSearchParent.Children.Remove(patientToMod);
            FillChart(); //Fill the textboxes with the given information
        }

        ///
        /// <summary>   Fill chart. Fills all the textboxes with the information that the user returned from 
        ///             the search screen.</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///

        public void FillChart()
        {
            //Fill All the textboxes.
            fNameTextBox.Text = patient.FirstName;
            lNameTextBox.Text = patient.LastName;
            hcnTextBox.Text = patient.HCN;
            mInitialTextBox.Text = patient.MInitial;
            dobPicker.DisplayDate = patient.DoB;
            dobPicker.Text = patient.DoB.ToString();

            //Select the proper gender textboxes.
            if (patient.Gender == 'M')
            {
                maleButton.IsChecked = true;
                Gender = 'M';
            }
            else if (patient.Gender == 'F')
            {
                femaleButton.IsChecked = true;
                Gender = 'F';
            }
            else
            {
                otherButton.IsChecked = true;
                Gender = 'X';
            }

            //Fill in the patient's HOH details. Enable textboxes
            if(patient.GetType() == typeof(Patient_HoH))
            {
                patientHoh = (Patient_HoH)patient;

                addressTextBox.Text = patientHoh.AddressLine1;
                addressTextBox.IsEnabled = true;

                addressTwoTextbox.Text = patientHoh.AddressLine2;
                addressTwoTextbox.IsEnabled = true;
                //Prov selection.
                for (i = 0; i < provComboBox.Items.Count; i++)
                {
                    if (string.Compare(patientHoh.Province, provComboBox.Items[i].ToString()) == 0)
                    {
                        provComboBox.SelectedIndex = i;
                    }
                }
                if (patientHoh.Province == "  ")
                {
                    provComboBox.SelectedIndex = 0;
                }

                provComboBox.IsEnabled = true;

                pCodeTextBox.Text = patientHoh.PostalCode;
                pCodeTextBox.IsEnabled = true;

                cityTextBox.Text = patientHoh.City;
                cityTextBox.IsEnabled = true;

                pNumberTextBox.Text = patientHoh.Phone;
                pNumberTextBox.IsEnabled = true;

            }
            //Fill in patients HoH details, do not make them modifiable.
            if(patient.GetType() == typeof(Patient_Dependant))
            {
                patientDependant = (Patient_Dependant)patient;
                hohTextBox.Text = patientDependant.HoH_HCN;

                fillPatient = Database.HoH_GetAutoFill(patientDependant.HoH_HCN);

                addressTextBox.Text = fillPatient[0];
                addressTextBox.IsEnabled = false;

                addressTwoTextbox.Text = fillPatient[2];
                addressTwoTextbox.IsEnabled = false;

                cityTextBox.Text = fillPatient[1];
                cityTextBox.IsEnabled = false;

                //Selection of the province
                for (i = 0; i < provComboBox.Items.Count; i++)
                {
                    if (string.Compare(fillPatient[4], provComboBox.Items[i].ToString()) == 0)
                    {
                        provComboBox.SelectedIndex = i;
                    }
                }

                if (fillPatient[4] == "  ")
                {
                    provComboBox.SelectedIndex = 0;
                }
                provComboBox.IsEnabled = false;

                pCodeTextBox.Text = fillPatient[3];
                pCodeTextBox.IsEnabled = false;

                pNumberTextBox.Text = fillPatient[5];
                pNumberTextBox.IsEnabled = false;
            }


            
        }

        ///
        /// <summary>   Resets the textboxes to their original state and ensures that the
        ///             validators are reset to false</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///

        public void ResetBoxes()
        {
            fNameTextBox.Text = "";
            lNameTextBox.Text = "";
            hcnTextBox.Text = "";
            mInitialTextBox.Text = "";
            dobPicker.Text = "";
            maleButton.IsChecked = false;
            femaleButton.IsChecked = false;
            otherButton.IsChecked = false;
            hohTextBox.Text = "";
            addressTextBox.Text = "";
            addressTwoTextbox.Text = "";
            cityTextBox.Text = "";
            pCodeTextBox.Text = "";
            provComboBox.SelectedIndex = 0;
            pNumberTextBox.Text = "";

        }

        ///
        /// <summary>   Updates a HoH contained within the databse. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///

        public void UpdateHoH()
        {
            patientHoh.FirstName = fNameTextBox.Text;
            patientHoh.LastName = lNameTextBox.Text;
            patientHoh.HCN = hcnTextBox.Text;
            patientHoh.MInitial = Gender.ToString();
            patientHoh.DoB = dobPicker.DisplayDate;

            //Probs need validation here
            patientHoh.AddressLine1 = addressTextBox.Text;
            patientHoh.AddressLine2 = addressTwoTextbox.Text;
            patientHoh.City = cityTextBox.Text;
            patientHoh.Province = provComboBox.Text;
            patientHoh.PostalCode = pCodeTextBox.Text;
            patientHoh.Phone = pNumberTextBox.Text;

            patientHoh.Update();
        }

        ///
        /// <summary>   Updates a dependants details in the database.. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///

        public void UpdateDependant()
        {
            patientDependant.FirstName = fNameTextBox.Text;
            patientDependant.LastName = lNameTextBox.Text;
            patientDependant.HCN = hcnTextBox.Text;
            patientDependant.MInitial = mInitialTextBox.Text;
            patientDependant.Gender = Gender;
            patientDependant.DoB = dobPicker.DisplayDate;

            patientDependant.Update();
        }

        ///
        /// <summary>   Event handler. Called by SubmitBtn for click events. 
        ///             ensures all required items are valid and then submits the request to
        ///             update the HoH or dependant</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ///

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            bool passOrFail = false;

            if (CanSubmit()) //Ensure all required information was submitted.
            {
                //If any dependant information is not valid, set them to blank
                if(!addressBool)
                {
                    addressTextBox.Text = "";
                }
                if(!addressTwoBool)
                {
                    addressTwoTextbox.Text = "";
                }
                if(!cityBool)
                {
                    cityTextBox.Text = "";
                }
                if(provComboBox.SelectedIndex == 0)
                {
                    provComboBox.Text = "  ";
                }
                if(!pCodeBool)
                {
                    pCodeTextBox.Text = "";
                }
                if(!pNumberBool)
                {
                    pNumberTextBox.Text = "";
                }

                //Send either patient or dependent depending on type.
                //Send it to the factory
                if (patient.GetType() == typeof(Patient_HoH)) //patient is a hoh
                {
                    //If patient is HOH... See if a HOH is entered... Make patient a depenendant..  Then HCN_To_Dependant... 
                    //Then update.
                    if (hohTextBox.Text != "" && hohEnteredBool) //Change hohTo Dependant
                    { 

                        hohToDependant[0] = hcnTextBox.Text;
                        hohToDependant[1] = lNameTextBox.Text;
                        hohToDependant[2] = fNameTextBox.Text;
                        hohToDependant[3] = mInitialTextBox.Text;
                        hohToDependant[4] = dobPicker.Text;
                        hohToDependant[5] = Gender.ToString();
                        hohToDependant[6] = hohTextBox.Text;
                        
                        passOrFail = Database.HoH_To_Dependant(hohToDependant); //See if the cahnge is allowed,
                        //Not allowed if the HoH has dependants themself
                        

                        if(passOrFail)
                        {
                           //Get a new patient and update.. 
                            patientDependant = (Patient_Dependant)PatientFactory.Create(hohToDependant);
                            patientDependant.Update();

                        }
                        else
                        {
                            UpdateHoH(); 
                        }
                    }
                    else
                    {
                        UpdateHoH(); //HoH info did not change.. Update HoH
                    }
        
                }
                else
                {
                    //If PAtietn is dependant... No HoH is entered... We use Dependant_To_Hoh()... Then update after.. Otherwise just update what has been updated.
                    if (!hohEnteredBool)
                    {

                        dependantToHoh[2] = fNameTextBox.Text;
                        dependantToHoh[1] = lNameTextBox.Text;
                        dependantToHoh[0] = hcnTextBox.Text;
                        dependantToHoh[3] = mInitialTextBox.Text;
                        dependantToHoh[5] = Gender.ToString();
                        dependantToHoh[4] = dobPicker.Text;
                        
                        dependantToHoh[6] = addressTextBox.Text;
                        dependantToHoh[7] = addressTwoTextbox.Text;
                        dependantToHoh[8] = cityTextBox.Text;
                        dependantToHoh[10] = provComboBox.SelectedValue.ToString();
                        dependantToHoh[9] = pCodeTextBox.Text;
                        dependantToHoh[11] = pNumberTextBox.Text;

                        //Make the dependent a HoH
                        Database.Dependant_To_HoH(dependantToHoh);

                        if (passOrFail)
                        {
                            //Create a new HoH
                            patientHoh = (Patient_HoH)PatientFactory.Create(dependantToHoh);

                            patientHoh.Update(); //Update the patient

                        }
                        else
                        {
                            UpdateDependant();
                        }
                    }
                    else //Hoh did not switch.. Update dependant.
                    {
                        UpdateDependant();
                    }
                    
                }

                ResetBoxes(); //Reset all boxes
                StartNewSearch(); //Start a new search
            }
        }

        ///
        /// <summary>   Event handler. Called by FNameTextBox for text changed events. </summary>
        ///
        /// <remarks>   UNIT ONE, 2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>
        ///

        private void FNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isValid = true;
            //CheckValidation
            isValid = PatientValidation.ValidateFirstName(fNameTextBox.Text);

            if (!isValid) //Not valid
            {
                fNameEntered = false;
                fNameError.Visibility = Visibility.Visible;
            }
            else
            { //Valid
                fNameEntered = true;
                fNameError.Visibility = Visibility.Hidden;
            }

            if (String.IsNullOrEmpty(fNameTextBox.Text)) //Empty string
            {
                fNameEntered = false;
                fNameError.Visibility = Visibility.Hidden;
            }
        }

        ///
        /// <summary>   Event handler. Called by HohTextBox for text changed events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>
        ///

        private void HohTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Sets all textboxes to be enabled
            addressTextBox.IsEnabled = true;
            addressTwoTextbox.IsEnabled = true;
            cityTextBox.IsEnabled = true;
            pNumberTextBox.IsEnabled = true;
            pCodeTextBox.IsEnabled = true;
            provComboBox.IsEnabled = true;


            bool isValid = true;
            //Check healthcard validation
            isValid = PatientValidation.ValidateHoH(hohTextBox.Text);

            if (!isValid && !String.IsNullOrEmpty(hohTextBox.Text)) //Not valid, non-empty string
            {
                hohEnteredBool = false;
                hohError.Visibility = Visibility.Visible;
            }
            else if (String.IsNullOrEmpty(hohTextBox.Text)) //Empty string
            {
                hohEnteredBool = false;
                hohError.Visibility = Visibility.Hidden;
            }
            else //valid
            {
                hohEnteredBool = true;
                hohError.Visibility = Visibility.Hidden;
                //Get autofill information
                fillPatient = Database.HoH_GetAutoFill(hohTextBox.Text);

                //Fill textboxes with returned list information accordingly
                addressTextBox.Text = fillPatient[0];
                addressTextBox.IsEnabled = false;

                addressTwoTextbox.Text = fillPatient[2];
                addressTwoTextbox.IsEnabled = false;

                cityTextBox.Text = fillPatient[1];
                cityTextBox.IsEnabled = false;

                //Province 
                for (i = 0; i < provComboBox.Items.Count; i++)
                {
                    if (string.Compare(fillPatient[4], provComboBox.Items[i].ToString()) == 0)
                    {
                        provComboBox.SelectedIndex = i;
                    }
                }
                if (fillPatient[4] == "  ")
                {
                    provComboBox.SelectedIndex = 0;
                }
                provComboBox.IsEnabled = false;

                pCodeTextBox.Text = fillPatient[3];
                pCodeTextBox.IsEnabled = false;

                pNumberTextBox.Text = fillPatient[5];
                pNumberTextBox.IsEnabled = false;

            }
        }

        ///
        /// <summary>   Event handler. Called by LNameTextBox for text changed events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>
        ///

        private void LNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isValid = true;
            //Ensure last name is valid
            isValid = PatientValidation.ValidateLastName(lNameTextBox.Text);

            if (!isValid) //not valid
            {
                lNameEntered = false;
                lNameError.Visibility = Visibility.Visible;
            }
            else //Valid entry
            {
                lNameEntered = true;
                lNameError.Visibility = Visibility.Hidden;
            }
            if (String.IsNullOrEmpty(lNameTextBox.Text)) //Null string
            {
                lNameEntered = false;
                lNameError.Visibility = Visibility.Hidden;
            }
        }

        ///
        /// <summary>   Event handler. Called by InitialTextBox for text changed events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>
        ///

        private void MInitialTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isValid = true;
            //Ensure that the middle name is valid
            isValid = PatientValidation.ValidateMInitial(mInitialTextBox.Text);

            if (!isValid) //Not valid
            {
                mInitialEntered = false;
                mInitialError.Visibility = Visibility.Visible;
            }
            else //Valid
            {
                mInitialEntered = true;
                mInitialError.Visibility = Visibility.Hidden;
            }

            if (String.IsNullOrEmpty(mInitialTextBox.Text)) //Empty string
            {
                mInitialEntered = false;
                mInitialError.Visibility = Visibility.Hidden;
            }
        }

        ///
        /// <summary>   Event handler. Called by MaleButton for checked events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ///

        private void MaleButton_Checked(object sender, RoutedEventArgs e)
        {
            //Set the gender to male
            Gender = 'M';
            genderEntered = true;
        }

        ///
        /// <summary>   Event handler. Called by OtherButton for checked events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ///

        private void OtherButton_Checked(object sender, RoutedEventArgs e)
        {
            //Set Gender to other.
            Gender = 'X';
            genderEntered = true;
        }

        ///
        /// <summary>   Event handler. Called by FemaleButton for checked events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ///

        private void FemaleButton_Checked(object sender, RoutedEventArgs e)
        {
            //Change the gender to female
            Gender = 'F';
            genderEntered = true;
        }

        ///
        /// <summary>   Event handler. Called by AddressTextBox for text changed events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>
        ///

        private void AddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isValid = true;
            //Ensure valid address
            isValid = PatientValidation.ValidateAddressLine1(addressTextBox.Text);

            if (!isValid && !String.IsNullOrEmpty(addressTextBox.Text)) //Not valid and not empty
            {
                addressBool = false;
                addressError.Visibility = Visibility.Visible;
            }
            else //valid
            {
                addressBool = true;
                addressError.Visibility = Visibility.Hidden;
            }
        }

        ///
        /// <summary>   Event handler. Called by AddressTwoTextbox for text changed events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>
        ///

        private void AddressTwoTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isValid = true;
            //Address 2 textbox validation
            isValid = PatientValidation.ValidateAddressLine2(addressTwoTextbox.Text);

            if (!isValid && !String.IsNullOrEmpty(addressTwoTextbox.Text)) //Not valid and not an empty string
            {
                addressTwoBool = false;
                address2Error.Visibility = Visibility.Visible;
            }
            else //false
            {
                addressTwoBool = true;
                address2Error.Visibility = Visibility.Hidden;
            }
        }

        ///
        /// <summary>   Event handler. Called by CityTextBox for text changed events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>
        ///

        private void CityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isValid = true;
            isValid = PatientValidation.ValidateCity(cityTextBox.Text);

            if (!isValid && !String.IsNullOrEmpty(cityTextBox.Text))
            {
                cityBool = false;
                cityError.Visibility = Visibility.Visible;
            }
            else
            {
                cityBool = true;
                cityError.Visibility = Visibility.Hidden;
            }
        }

        ///
        /// <summary>   Event handler. Called by PCodeTextBox for text changed events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>
        ///

        private void PCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isValid = true;
            //Postal Code Validation
            isValid = PatientValidation.ValidatePostalCode(pCodeTextBox.Text);

            if (!isValid && !String.IsNullOrWhiteSpace(pCodeTextBox.Text)) //String is not valid and not empty.
            {
                pCodeBool = false;
                pCodeError.Visibility = Visibility.Visible;
            }
            else //Valid
            {
                pCodeBool = true;
                pCodeError.Visibility = Visibility.Hidden;
            }
        }

        ///
        /// <summary>   Event handler. Called by PNumberTextBox for text changed events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>
        ///

        private void PNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool isValid = true;
            //Phone number validation
            isValid = PatientValidation.ValidatePhone(pNumberTextBox.Text);

            if (!isValid && !String.IsNullOrEmpty(pNumberTextBox.Text)) //Not valid and not empty
            {
                pNumberBool = false;
                pNumError.Visibility = Visibility.Visible;
            }
            else //Valid
            {
                pNumberBool = true;
                pNumError.Visibility = Visibility.Hidden;
            }
        }

        ///
        /// <summary>   Event handler. Called by DP for selected date changed events. </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Selection changed event information. </param>
        ///

        private void DP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //ensure the display date is greater than today.

            if (dobPicker.DisplayDate > DateTime.Now)
            {
                dobEntered = false;
                dobError.Visibility = Visibility.Visible;
            }
            else //Valid date
            {
                dobEntered = true;
                dobError.Visibility = Visibility.Hidden;
            }
        }

        ///
        /// <summary>   Event handler. Called by NewModBtn for click events.
        ///             Button to select for a new search screen</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>
        ///

        private void NewModBtn_Click(object sender, RoutedEventArgs e)
        {
            StartNewSearch();
        }

        ///
        /// <summary>   Determine if we can submit by checking that all information was entered correctly that is needed. </summary>
        ///
        /// <remarks>   UNIT ONE, 2019-04-20. </remarks>
        ///
        /// <returns>   True if we can submit, false if not. </returns>
        ///

        public bool CanSubmit()
        {
            bool canSubmit = false;
            //All the required items.
            if (fNameEntered && lNameEntered && hcnEntered && dobEntered && genderEntered && mInitialEntered)
            {
                canSubmit = true;
            }

            return canSubmit;
        }
    }
}
