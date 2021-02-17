/*
 * 
 * Author:      Arie Kraayenbrink
 * Date:        January to April 2019
 * Project:     EMS2
 * File:        SelectDate.xaml.cs
 * Description: This is used to get a month and year from the user.
 * 
*/



using EMS_Billing;
using Support;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;

namespace EMS_2.Billing
{
    ///-------------------------------------------------------------------------------------------------
    /// \class  SelectDate
    ///
    /// \brief  Interaction logic for CreateBillFile.xaml
    ///
    /// \author Arie
    /// \date   2019-04-20
    ///-------------------------------------------------------------------------------------------------
    public partial class SelectDate : Window
    {
        static Nullable<DateTime> selectedDate; ///< The selected date
        string selectedMonth;          ///< The selected month
        string selectedYear;           ///< The selected year
        CultureInfo cultureInfo = new CultureInfo("en-US"); ///< Information describing the culture
        const int startYear = 2000;    ///< The start year
        bool dateSelected;             ///< True if date selected



        ///-------------------------------------------------------------------------------------------------
        /// \fn public SelectDate()
        ///
        /// \brief  Default constructor
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///-------------------------------------------------------------------------------------------------
        public SelectDate()
        {
            InitializeComponent();

            selectedMonth = "";
            selectedYear = "";

            cmdMonth.ItemsSource = cultureInfo.DateTimeFormat.MonthNames.Take(12); //fill month selector
            cmdYear.ItemsSource = Enumerable.Range(startYear, DateTime.Now.Year - startYear + 1);   //Fill year selector

            selectedDate = null;

            dateSelected = false;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void BtnSave_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by BtnSave for click events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            dateSelected = true;

            this.Close();
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void CreateBillFile_Closing(object sender, ConsoleCancelEventArgs e)
        ///
        /// \brief  Event handler. Called by CreateBillFile for closing events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Console cancel event information.
        ///-------------------------------------------------------------------------------------------------
        private void CreateBillFile_Closing(object sender, ConsoleCancelEventArgs e)
        {
            if (!dateSelected)
            {
                selectedDate = null;
            }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public static Nullable<DateTime> SelectedDate()
        ///
        /// \brief  Selected date
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \returns    A Nullable&lt;DateTime&gt;
        ///-------------------------------------------------------------------------------------------------
        public static Nullable<DateTime> SelectedDate()
        {
            SelectDate createBillFile = new SelectDate();
            createBillFile.ShowDialog();

            return selectedDate;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        ///
        /// \brief  Event handler. Called by ComboBox for selection changed events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Selection changed event information.
        ///-------------------------------------------------------------------------------------------------
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmdMonth.SelectedValue != null && cmdYear.SelectedValue != null)    // both a month and year must be selected
                {
                    selectedMonth = (cmdMonth.SelectedIndex + 1).ToString();
                    selectedYear = cmdYear.SelectedItem.ToString();
                    selectedDate = DateTime.ParseExact(selectedMonth.PadLeft(2, '0') + selectedYear, "MMyyyy", cultureInfo);

                    btnSave.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Write(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
