/*
 * 
 * Author		: Arie Kraayenbrink
 * Date			: Jan to April 2019
 * Project		: EMS 2
 * File			: Reconcile.xaml.cs
 * Description	: 
 * Credit		: 
 * 
*/



using EMS_Billing;
using Microsoft.Win32;
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
    /// \class  Reconcile
    ///
    /// \brief  Interaction logic for Reconcile.xaml
    ///
    /// \author Arie
    /// \date   2019-04-20
    ///-------------------------------------------------------------------------------------------------
    public partial class Reconcile : Window
    {
        EMSBilling billing;            ///< The billing
        DateTime selectedDate;         ///< The selected date
        string selectedMonth;          ///< The selected month
        string selectedYear;           ///< The selected year
        string filePath = null;        ///< Full pathname of the file
        CultureInfo cultureInfo = new CultureInfo("en-US"); ///< Information describing the culture
        const int startYear = 2000;    ///< The start year



        ///-------------------------------------------------------------------------------------------------
        /// \fn public Reconcile()
        ///
        /// \brief  Default constructor
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///-------------------------------------------------------------------------------------------------
        public Reconcile()
        {
            InitializeComponent();
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public Reconcile(Object obj)
        ///
        /// \brief  Constructor
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  obj The object.
        ///-------------------------------------------------------------------------------------------------
        public Reconcile(Object obj)
        {
            InitializeComponent();
            billing = (EMSBilling)obj;
            cmdMonth.ItemsSource = cultureInfo.DateTimeFormat.MonthNames.Take(12); //fill month selector
            cmdYear.ItemsSource = Enumerable.Range(startYear, DateTime.Now.Year - startYear + 1);   //Fill year selector
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
                if (cmdMonth.SelectedValue != null && cmdYear.SelectedValue != null && txtFile.Text != "")    // Both month, year and file must be selected.
                {
                    selectedMonth = (cmdMonth.SelectedIndex + 1).ToString();
                    selectedYear = cmdYear.SelectedItem.ToString();
                    selectedDate = DateTime.ParseExact(selectedMonth.PadLeft(2, '0') + selectedYear, "MMyyyy", cultureInfo);

                    btnReconcile.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Logging.Write(ex.Message);
            }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void BtnReconcile_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by BtnReconcile for click events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private async void BtnReconcile_Click(object sender, RoutedEventArgs e)
        {          
            try
            {
                if (cmdMonth.SelectedValue != null && cmdYear.SelectedValue != null)    // Both month and year must be selected
                {
                    selectedMonth = (cmdMonth.SelectedIndex + 1).ToString();
                    selectedYear = cmdYear.SelectedItem.ToString();
                    selectedDate = DateTime.ParseExact(selectedMonth.PadLeft(2, '0') + selectedYear, "MMyyyy", cultureInfo);
                }

                await Task.Run(() => billing.RecMonthlyBill(selectedDate, filePath));

                this.Close();
            }
            catch (Exception ex)
            {
                Logging.Write(ex.Message);
            }

            //billing.RecMonthlyBill(selectedDate, filePath);
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by BtnBrowse for click events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {            
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = @".";
            ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (ofd.ShowDialog() == true)
            {
                filePath = ofd.FileName.ToString();
                txtFile.Text = ofd.FileName.ToString();
            }
        }
    }
}
