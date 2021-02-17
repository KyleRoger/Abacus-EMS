// file:	Patient\AddPatient.xaml.cs
// Author:  Kyle Horsley
// Date:    April 20 ,2019
// summary:	Implements the add patient.xaml class

using Factory;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using Demographics;
using Data;

namespace EMS_2.Patients
{
    /// This is the AddPatient class with all of its methods.
    /// 
    /// Name:               Add Patient      
    /// 
    /// Purpose:            To provide the user functionality to add a patient to the databsae.
    /// 
    /// Fault Detection:    This class uses Regex validators and boolean flags to ensure that valid responses
    ///                     were entered into the appropriate locations.
    /// 
    /// Relationships:      This class is related to the factory class and the database class. These are both
    ///                     effeectively used to create a patient as either a Head of Household or dependant depending
    ///                     on the input from the user.

    public partial class AddPatient : UserControl
    {
        /// <summary>   The gender of the user. </summary>
        string Gender = "";
        /// <summary>   The fill patient. </summary>
        List<string> fillPatient = new List<string>();

        /// <summary>   True if name entered. </summary>
        bool fNameEntered = false;
        /// <summary>   True if name entered. </summary>
        bool lNameEntered = false;
        /// <summary>   True if hcn entered. </summary>
        bool hcnEntered = false;
        /// <summary>   True if dob entered. </summary>
        bool dobEntered = false;
        /// <summary>   True if gender entered. </summary>
        bool genderEntered = false;
        /// <summary>   True if initial entered. </summary>
        bool mInitialEntered = false;

        /// <summary>   The address one. </summary>
        string addressOne;
        /// <summary>   The address two. </summary>
        string addressTwo;
        /// <summary>   The city. </summary>
        string city;
        /// <summary>   The postal code. </summary>
        string postalCode;
        /// <summary>   The prov. </summary>
        string prov;
        /// <summary>   The phone number. </summary>
        string phoneNum;
        /// <summary>   The head of household. </summary>
        string headOfHousehold;
        /// <summary>   Zero-based index of the. </summary>
        int i;



        /// <summary>   Default constructor. 
        ///             Sets the provinces, and dateTime max. </summary>
        ///
        /// <remarks>   UNIT ONE, 2019-04-20. </remarks>

        public AddPatient()
        {
            InitializeComponent();
            i = 0;

            //Initialize provinces
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

            dobPicker.DisplayDateEnd = DateTime.Now; //Max Date time
            SubmitError.Content = "*First Name, Last Name, Middle Initial,\n HCN, Date Of Birth and Gender Must be Entered!";

            //Set initial values.
            addressOne = "";
            addressTwo = "";
            city = "" ;
            postalCode = "";
            prov = "  ";
            phoneNum = "";
            headOfHousehold = "";
        }

        /// <summary>   Determine if we can submit.
        ///             If all of the reuiqred items have been validated, 
        ///             we can submit the new patient. </summary>
        ///
        ///
        /// <returns>   True if we can submit, false if not. </returns>

        public bool CanSubmit()
        {
            bool canSubmit = false;

            //Check the booleans of the entered items
            if(fNameEntered && lNameEntered && hcnEntered && dobEntered && genderEntered && mInitialEntered)
            {
                canSubmit = true;
            }

            return canSubmit;
        }

        /// <summary>   Event handler. Called by SubmitBtn for click events.
        ///             Will attempt to submit a patient or give an error </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
         
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            int patientLength = 13;
            string[] patient = new string[patientLength]; //A string to create a patient.

            if (CanSubmit()) //If all required items were inputted
            {
               //Fill a list with patient details.
                patient[0] = hcnTextBox.Text;
                patient[1] = lNameTextBox.Text;
                patient[2] = fNameTextBox.Text;
                patient[3] = mInitialTextBox.Text;
                patient[4] = dobPicker.Text;
                patient[5] = Gender;

                patient[6] = headOfHousehold;
                patient[7] = addressOne;
                patient[8] = addressTwo;
                patient[9] = city;
                patient[10] = postalCode;
                patient[11] = prov;
                patient[12] = phoneNum;

                //See if it is a HOH or dependant
                PatientFactory.Create(patient).Add();
                MessageBox.Show("Patients Added Successfully");
                ResetBoxes();

            }
            else
            {
                //Something was not entered.. Do not submit. Show error.
                SubmitError.Visibility = Visibility.Visible;
            }
        }

        /// <summary>   Resets the boxes.
        ///             The fields all get reset and the booleans
        ///             reset to false values. </summary>
        ///
        /// <remarks> 2019-04-20. </remarks>

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
            addressOne = "";
            addressTwo = "";
            city = "";
            postalCode = "";
            prov = "  ";
            phoneNum = "";
            headOfHousehold = "";

            fNameEntered = false;
            lNameEntered = false;
            hcnEntered = false;
            dobEntered = false;
            genderEntered = false;
            mInitialEntered = false;

        }

        /// <summary>   Event handler. Called by HohTextBox for text changed events.
        ///             See if there is an Hoh with the given values and autofill 
        ///             the rest of the user information including, addresses, prov, postal
        ///             code, city and phone number </summary>
        ///
        /// <remarks> 2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>

        private void HohTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            //Set everything to empty and enable the boxes of the Hoh fields.
            addressTextBox.Text = "";
            addressTextBox.IsEnabled = true;

            addressTwoTextbox.Text = "";
            addressTwoTextbox.IsEnabled = true;

            cityTextBox.Text = "";
            cityTextBox.IsEnabled = true;

            pNumberTextBox.Text = "";
            pNumberTextBox.IsEnabled = true;

            pCodeTextBox.Text = "";
            pCodeTextBox.IsEnabled = true;

            provComboBox.SelectedIndex = 0;
            provComboBox.IsEnabled = true;


            bool isValid = true; //Validate the Healthcard number of the HoH entry
            isValid = PatientValidation.ValidateHoH(hohTextBox.Text);

            //If not vlaid and string is not null
            if(!isValid && !String.IsNullOrEmpty(hohTextBox.Text))
            {
                headOfHousehold = ""; //Set the hoh value to blank, show an error
                hohError.Visibility = Visibility.Visible;
            }
            //If it is null.. Dont shoe the error, still set value to blank
            else if(String.IsNullOrEmpty(hohTextBox.Text))
            {
                headOfHousehold = "";
                hohError.Visibility = Visibility.Hidden;
            }
            //Else, valid health card.. Input HoH details into the required boxes.
            else
            {
                //Hide the error.
               hohError.Visibility = Visibility.Hidden;
               fillPatient =  Database.HoH_GetAutoFill(hohTextBox.Text); //Get the HoH information

                //Use the list returned from the Hohautofill to fill in the contents,
                // Set all of the boxes to be disabled until the user modifies the hoh 
                // textbox again.
                addressTextBox.Text = fillPatient[0];
                addressTextBox.IsEnabled = false;

                addressTwoTextbox.Text = fillPatient[1];
                addressTwoTextbox.IsEnabled = false;

                cityTextBox.Text = fillPatient[2];
                cityTextBox.IsEnabled = false;

                //Loop through the provinces to find the correct one, or select 0.
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

                headOfHousehold = hohTextBox.Text;

            }
        }

        /// <summary>   Event handler. Called by FNameTextBox for text changed events. 
        ///             Ensure valid first name entry, set vool accordingly</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>

        private void FNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            bool isValid = true;
            //Validate First name
            isValid = PatientValidation.ValidateFirstName(fNameTextBox.Text);

            if(!isValid) //If not valid, show error, set booleam to false.
            {
                fNameEntered = false;
                fNameError.Visibility = Visibility.Visible;
            }
            else //String is valid hide error and set bool to true.
            {
                fNameEntered = true;
                fNameError.Visibility = Visibility.Hidden;
            }

            //If string is null... Set it to false but hide error
            if (String.IsNullOrEmpty(fNameTextBox.Text))
            {
                fNameEntered = false;
                fNameError.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>   Event handler. Called by LNameTextBox for text changed events. </summary>
        ///
        /// <remarks>   UNIT ONE, 2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>

        private void LNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            bool isValid = true;
            isValid = PatientValidation.ValidateLastName(lNameTextBox.Text);

            if (!isValid)
            {
                lNameEntered = false;
                lNameError.Visibility = Visibility.Visible;
            }
            else
            {
                lNameEntered = true;
                lNameError.Visibility = Visibility.Hidden;
            }

            if (String.IsNullOrEmpty(lNameTextBox.Text))
            {
                lNameEntered = false;
                lNameError.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>   Event handler. Called by HcnTextBox for text changed events. 
        ///             Ensures a valid health card, and that it does not already exist.</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>

        private void HcnTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            hcnError.Content = "*Invalid Health Card Number!";
            bool isValid = true;
            //Validate health card number
            isValid = PatientValidation.ValidateHCN(hcnTextBox.Text);

            if (!isValid) //Not valid. Make error visile. 
            {
                hcnEntered = false;
                hcnError.Visibility = Visibility.Visible;
            }
            else //Health card is valid.
            {
                if (PatientValidation.ValidateNewHCN(hcnTextBox.Text)) //Ensure it doesnt yet exist, if it does show error
                {
                    hcnEntered = false;
                    hcnError.Content = "Health Card Number Already Exists!";
                    hcnError.Visibility = Visibility.Visible;
                }

                else //Health card is valid and does not exist.  Set boolean to true.
                {
                    hcnEntered = true;
                    hcnError.Visibility = Visibility.Hidden;
                }
            }

            if (String.IsNullOrEmpty(hcnTextBox.Text)) //Nothing was entered.
            {
                hcnEntered = false;
                hcnError.Visibility = Visibility.Hidden;
            }

        }

        /// <summary>   Event handler. Called by InitialTextBox for text changed events.
        ///             Ensures one letter only was inputted </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>

        private void MInitialTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            bool isValid = true;
            //Validate middle intial
            isValid = PatientValidation.ValidateMInitial(mInitialTextBox.Text);

            if (!isValid) //If not valid. Show error
            {
                mInitialEntered = false;
                mInitialError.Visibility = Visibility.Visible;
            }
            else //Valid entrey
            {
                mInitialEntered = true;
                mInitialError.Visibility = Visibility.Hidden;
            }

            if (String.IsNullOrEmpty(mInitialTextBox.Text)) //Empty string. Not valid
            {
                mInitialEntered = false;
                mInitialError.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>   Event handler. Called by MaleButton for checked events. 
        ///             See if male button was chacked</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>

        private void MaleButton_Checked(object sender, RoutedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            Gender = "M"; //Set the gender to a male
            genderEntered = true;
        }

        /// <summary>   Event handler. Called by OtherButton for checked events. 
        ///             See if other button was chacked</summary>
        ///
        /// <remarks>   UNIT ONE, 2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>

        private void OtherButton_Checked(object sender, RoutedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            Gender = "X"; //Set the gender to other
            genderEntered = true;
        }

        /// <summary>   Event handler. Called by FemaleButton for checked events. 
        ///             See if female button was chacked</summary>
        ///
        /// <remarks>   UNIT ONE, 2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Routed event information. </param>

        private void FemaleButton_Checked(object sender, RoutedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            Gender = "F"; //Set the Gender to female
            genderEntered = true;
        }

        /// <summary>   Event handler. Called by AddressTextBox for text changed events. 
        ///             Ensure a vlaid address, otherwise set it to blank</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>

        private void AddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            bool isValid = true;
            //Validate address
            isValid = PatientValidation.ValidateAddressLine1(addressTextBox.Text);

            if (!isValid && !String.IsNullOrEmpty(addressTextBox.Text)) //If it is not valid and not empty
            {
                addressOne = ""; //Address is not set. Error is shown
                addressError.Visibility = Visibility.Visible;
            }
            else //Valid address. Set the address to the textbox string
            {
                addressOne = addressTextBox.Text;
                addressError.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>   Event handler. Called by AddressTwoTextbox for text changed events. 
        ///             Ensure a vlaid address, otherwise set it to blank</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>

        private void AddressTwoTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            bool isValid = true;
            //Validate address two
            isValid = PatientValidation.ValidateAddressLine2(addressTwoTextbox.Text);

            if (!isValid && !String.IsNullOrEmpty(addressTwoTextbox.Text)) //Not valid, string is not empty
            {
                addressTwo = "";
                address2Error.Visibility = Visibility.Visible;
            }
            else //Valid string
            {
                addressTwo = addressTwoTextbox.Text;
                address2Error.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>   Event handler. Called by CityTextBox for text changed events.
        ///             Ensure valid city was entered </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>

        private void CityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            bool isValid = true;
            //Validate the city
            isValid = PatientValidation.ValidateCity(cityTextBox.Text);

            if (!isValid && !String.IsNullOrEmpty(cityTextBox.Text)) //City is not valid and the textbox is not empty
            {
                city = ""; //City is emoty and error is visible
                cityError.Visibility = Visibility.Visible;
            }
            else //City is valid. Set it to the textbox contents/. 
            {
                city = cityTextBox.Text;
                cityError.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>   Event handler. Called by ProvComboBox for selection changed events. 
        ///             Ensures the province is entered and valid. Basically checks
        ///             the entry against selected index of 0</summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Selection changed event information. </param>

        private void ProvComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            if (provComboBox.SelectedIndex == 0) //If province is not selected set it to 2 blank spaces
            {
                prov = "  ";
            }
            else //Otherwise choose the slected index to text,.
            {
                prov = provComboBox.SelectedItem.ToString();
                provError.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>   Event handler. Called by PCodeTextBox for text changed events. 
        ///             Ensures valid postal code entry</summary>
        ///
        /// <remarks>   UNIT ONE, 2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>

        private void PCodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            bool isValid = true;
            //Check postal code validation
            isValid = PatientValidation.ValidatePostalCode(pCodeTextBox.Text);

            if (!isValid && !String.IsNullOrWhiteSpace(pCodeTextBox.Text)) //If not valid and not null
            {
                postalCode = ""; //Postal code equals nothing, error is present
                pCodeError.Visibility = Visibility.Visible;
            }
            else //Postal code is valid. And error is not visible.
            {
                postalCode = pCodeTextBox.Text; 
                pCodeError.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>   Event handler. Called by PNumberTextBox for text changed events.
        ///             Ensure valid phone number entry </summary>
        ///
        /// <remarks>   2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Text changed event information. </param>

        private void PNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            bool isValid = true;
            //Check phone validation
            isValid = PatientValidation.ValidatePhone(pNumberTextBox.Text);

            if (!isValid && !String.IsNullOrEmpty(pNumberTextBox.Text)) //see if not valid and string is not empty
            {
                //Set phone value to nothing. Show the error
                phoneNum = "";
                pNumError.Visibility = Visibility.Visible;
            }
            else //Valid. Set phone value to textbox value. Hide the error
            {
                phoneNum = pNumberTextBox.Text;
                pNumError.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>   Event handler. Called by DP for selected date changed events.
        ///             Checks the date. To ensure no future dates of blank </summary>
        ///
        /// <remarks>   UNIT ONE, 2019-04-20. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Selection changed event information. </param>

        private void DP_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SubmitError.Visibility = Visibility.Hidden; //Hide the error initally
            if (dobPicker.DisplayDate > DateTime.Now) //See if date is in the future
            {
                dobEntered = false;
                dobError.Visibility = Visibility.Visible;
            }
            else if( dobPicker.Text == "") //See if no date has been picked
            {
                dobEntered = false;
            }
            else //Valid date. User can submit that date.
            {
                dobEntered = true;
                dobError.Visibility = Visibility.Hidden;
            }
        }

    }
}
