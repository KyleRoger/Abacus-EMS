/*
 * 
 * Author:      Arie Kraayenbrink
 * Date:        January to April 2019
 * Project:     EMS2
 * File:        TabBilling.xaml.cs
 * Description: This class handles the billing UI for EMS2.
 * 
*/



using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EMS_Billing;
using System.Data;
using Support;



namespace EMS_2.Billing
{
    ///-------------------------------------------------------------------------------------------------
    /// \class  TabBilling
    ///
    /// \brief  Interaction logic for TabBilling.xaml
    ///
    /// \author Arie
    /// \date   2019-04-20
    ///-------------------------------------------------------------------------------------------------
	public partial class TabBilling : UserControl
	{
        bool unsavedData = false;      ///< /Used at closing to check for unsaved data in text box
        System.Windows.Forms.Timer feedBackTimer;   ///< The feed back timer
        DateTime date;                 ///< The date Date/Time
        DateTime selectedMonth;        ///< The selected month



        ///-------------------------------------------------------------------------------------------------
        /// \fn public TabBilling()
        ///
        /// \brief  Default constructor
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///-------------------------------------------------------------------------------------------------
        public TabBilling()
		{
			InitializeComponent();
            //billing = new EMSBilling(); //Wondering if this should be here or in the main file where the program starts.
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private OpenFileDialog OpenFile()
        ///
        /// \brief  Opens the file
        ///
        /// \author Arie
        /// \date   2019-04-12
        ///
        /// \returns    An OpenFileDialog.
        ///-------------------------------------------------------------------------------------------------
        private OpenFileDialog OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = @".";
            ofd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (ofd.ShowDialog() == true)
            {
                try
                {
                    ToggleTable(false);

                    //Display imported data
                    txtResults.Clear();

                    txtResults.Text = File.ReadAllText(ofd.FileName);
                }
                catch (Exception expt)
                {
                    Logging.Write(expt.Message);
                    System.Windows.MessageBox.Show("Exception attempting to open file : " + expt.Message);
                }
            }

            return ofd;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void LookupBillCodes_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by LookupBillCodes for click events
        ///
        /// \author Arie
        /// \date   2019-04-12
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private void LookupBillCodes_Click(object sender, RoutedEventArgs e)
        {
            string query = "SELECT [Code], [Date], [DollarAmount] FROM [dbo].[viewBillCodes]";

            LookupTable(query);
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void LookupTable(string query)
        ///
        /// \brief  Looks up a given key to find its associated table
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  query   The query.
        ///-------------------------------------------------------------------------------------------------
        private void LookupTable(string query)
        {
            
            DataTable dt = Connection.ExecuteQuery(query);
            try
            {
                txtResults.Clear();

                ToggleTable(true);

                dgData.DataContext = dt.DefaultView;
            }
            catch (Exception expt)
            {
                Logging.Write(expt.Message);
                System.Windows.MessageBox.Show("Exception : " + expt.Message);
            }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void ToggleTable(bool gridVisiable)
        ///
        /// \brief  Toggle table
        ///
        /// \author Arie
        /// \date   2019-04-12
        ///
        /// \param  gridVisiable    True if grid visiable.
        ///-------------------------------------------------------------------------------------------------
        private void ToggleTable(bool gridVisiable)
        {
            if (gridVisiable)
            {
                txtResults.Visibility = Visibility.Hidden;
                txtResults.IsEnabled = false;

                dgData.IsEnabled = true;
                dgData.Visibility = Visibility.Visible;
            }
            else
            {
                dgData.IsEnabled = false;
                dgData.Visibility = Visibility.Hidden;

                txtResults.Visibility = Visibility.Visible;
                txtResults.IsEnabled = true;
            }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void SetTimer()
        ///
        /// \brief  Sets the timer. This timer is used to hide the status/feedback message.
        ///
        /// \author Arie
        /// \date   2019-04-12
        ///-------------------------------------------------------------------------------------------------
        private void SetTimer()
        {
            feedBackTimer = new System.Windows.Forms.Timer();

            /* Adds the event and the event handler for the method that will 
          process the timer event to the timer. */
            feedBackTimer.Tick += new EventHandler(ClearFeedBack);

            feedBackTimer.Enabled = true;

            // Sets the timer interval to 10 seconds.
            feedBackTimer.Interval = 10000;
            feedBackTimer.Start();
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private async void ImportBilling_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by ImportBilling for click events
        ///
        /// \author Arie
        /// \date   2019-04-12
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private async void ImportBilling_Click(object sender, RoutedEventArgs e)
        {
            //Which file?
            OpenFileDialog ofd = OpenFile();

            try
            {
                if (ofd != null)
                {
                    txbFeedBack.Text = "Importing bill codes";

                    //Import bill codes from selected file
                    if(await Task.Run(() => MainWindow.billing.ParseBillFile(ofd.FileName)))
                    {
                        txbFeedBack.Text = "Successfully imported " + MainWindow.billing.ValidLineCount + " out of " + MainWindow.billing.TotalLineCount + " bill codes";
                    }
                    else
                    {
                        txbFeedBack.Text = "Failed to import all bill codes, some records may be corrupted. Imported " + MainWindow.billing.ValidLineCount +" records, found " + MainWindow.billing.TotalLineCount + " lines";
                    }

                    SetTimer();
                }
            }
            catch (Exception expt)
            {
                Logging.Write(expt.Message);
                System.Windows.MessageBox.Show("Exception : " + expt.Message);
            }

        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void ImportResponse_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by ImportResponse for click events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private void ImportResponse_Click(object sender, RoutedEventArgs e)
        {
            //Which file?
            OpenFile();

            // What month is desired?
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private async void CreateBillingFile_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by CreateBillingFile for click events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private async void CreateBillingFile_Click(object sender, RoutedEventArgs e)
        {
            // Get the month and year
            Nullable<DateTime> selectedDate = SelectDate.SelectedDate();

            if (selectedDate != null)
            {
                DateTime date = (DateTime)selectedDate;
                string fileName = null;

                // Ask where to save the file.
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.FileName = Constants.monthlyBill + date.ToString("yyyyMM"); // The default name of the file
                saveFileDialog.DefaultExt = Constants.fileExt; // The default extension
                saveFileDialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

                if (saveFileDialog.ShowDialog() == true)
                {
                    fileName = saveFileDialog.FileName;
                }
                else
                {
                    // If user cancels, use default file name.
                    fileName = Constants.dbDirectory + @"\" + Constants.monthlyBill + date.ToString("yyyyMM") + Constants.fileExt;
                }

                // Create the file
                if (await Task.Run(() => MainWindow.billing.GenMonthlyBill(date, fileName)))
                {
                    // Display results
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                    string query = "SELECT attendee_hcn, code, gender, date, DollarAmount " +
                                        "FROM BillReport('" +
                                        firstDayOfMonth + "', '" +
                                        lastDayOfMonth + "')"
                                        //date.Year + "-" + date.Month + "-" + "01 00:00:00', '" +
                                        //date.Year + "-" + date.Month + "-" + lastDayOfMonth.Day + " 00:00:00')"
                                        ;

                    LookupTable(query);

                    txbFeedBack.Text = "Successfully created billing file";
                }
            }
            else
            {
                txbFeedBack.Text = "Failed to generate billing file";
            }

            SetTimer();
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void BtnReconcile_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by BtnReconcile for click events
        ///
        /// \author Arie
        /// \date   2019-04-12
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private void BtnReconcile_Click(object sender, RoutedEventArgs e)
        {
            Reconcile reconcile = new Reconcile(MainWindow.billing);
            reconcile.ShowDialog();

            int badRecordCount = (MainWindow.billing.TotalLineCount - MainWindow.billing.ValidLineCount);

            if (badRecordCount != 0)
            {
                txbFeedBack.Text = "File contains " + badRecordCount + " corrupted records out of " + 
                    MainWindow.billing.TotalLineCount + ", successfully read " + MainWindow.billing.ValidLineCount + " records";
            }
            else
            {
                txbFeedBack.Text = "Read " + MainWindow.billing.ValidLineCount + " valid records";
            }

            SetTimer();

            btnMonthlySummary.IsEnabled = true;     // Monthly summary is only available after importing the MOH file
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        ///
        /// \brief  Used to save to a file
        ///
        /// \author Arie
        /// \date   10/6/2018
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Executed routed event information.
        ///-------------------------------------------------------------------------------------------------
        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.FileName = "New Text Document"; // The default name of the file
                saveFileDialog.DefaultExt = ".txt"; // The default extension
                saveFileDialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, txtResults.Text);
                    this.unsavedData = false;
                }
            }
            catch (Exception expt)
            {
                Logging.Write(expt.Message);
                System.Windows.MessageBox.Show("Exception: " + expt.Message);
            }
            finally
            {

            }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void BtnSave_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by BtnSave for click events
        ///
        /// \author Arie
        /// \date   2019-04-12
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                openFileDialog.FileName = "New Text Document"; // The default name of the file
                openFileDialog.DefaultExt = ".txt"; // The default extension
                openFileDialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
                if (openFileDialog.ShowDialog() == true)
                {
                    MainWindow.billing.ParseBillFile(openFileDialog.FileName);
                }
            }
            catch (Exception expt)
            {
                Logging.Write(expt.Message);
                System.Windows.MessageBox.Show("Exception: " + expt.Message);
            }
            finally
            {
            }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void BtnMenuExpand_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by BtnMenuExpand for click events. This expands the drop down.
        ///
        /// \author Arie
        /// \date   2019-04-12
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private void BtnMenuExpand_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void BtnExportExpand_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by BtnExportExpand for click events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private void BtnExportExpand_Click(object sender, RoutedEventArgs e)
        {
            BtnMenuExpand_Click(sender, e);
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void ClearFeedBack(object source, EventArgs e)
        ///
        /// \brief  Clears the feed back
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  source  Source for the.
        /// \param  e       Event information.
        ///-------------------------------------------------------------------------------------------------
        private void ClearFeedBack(object source, EventArgs e)
        {
            txbFeedBack.Text = "";
            feedBackTimer.Stop();
            feedBackTimer.Dispose();
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void MenuItem_LostFocus(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by MenuItem for lost focus events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private void MenuItem_LostFocus(object sender, RoutedEventArgs e)
        {

        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void DpStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        ///
        /// \brief  Event handler. Called by DpStartDate for selected date changed events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Selection changed event information.
        ///-------------------------------------------------------------------------------------------------
        private void DpStartDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            // ... Get DatePicker reference.
            var picker = sender as DatePicker;

            // ... Get nullable DateTime from SelectedDate.
            DateTime? date = picker.SelectedDate;
            if (date == null)
            {
                // ... A null object.
                //this.Title = "No date";
            }
            else
            {

                btnCreateBillFile.IsEnabled = true;

                selectedMonth = date.Value;
            }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private async void BtnCreateMonthlySummary_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by BtnCreateMonthlySummary for click events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private async void BtnCreateMonthlySummary_Click(object sender, RoutedEventArgs e)
        {
            // Get the month and year
            Nullable<DateTime> selectedDate = SelectDate.SelectedDate();

            if (selectedDate != null)
            {
                DateTime date = (DateTime)selectedDate;

                // Create the file
                if (await Task.Run(() => MainWindow.billing.MonthlyBillSummary(date)))
                {
                    // Display results
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                    //string query = "SELECT attendee_hcn, code, gender, date, DollarAmount " +
                    //                    "FROM BillReport('" +
                    //                    date.Year + "-" + date.Month + "-" + "01 00:00:00', '" +
                    //                    date.Year + "-" + date.Month + "-" + lastDayOfMonth.Day + " 00:00:00')"
                    //                    ;

                    //LookupTable(query);

                    txbFeedBack.Text = "Successfully created billing summary";
                }
            }
            else
            {
                txbFeedBack.Text = "Failed to generate billing summary";
            }

            SetTimer();
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private async void BtnShowMonthlySummary_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by BtnShowMonthlySummary for click events
        ///
        /// \author Arie
        /// \date   2019-04-20
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private async void BtnShowMonthlySummary_Click(object sender, RoutedEventArgs e)
        {
            // Get the month and year
            Nullable<DateTime> selectedDate = SelectDate.SelectedDate();

            if (selectedDate != null)
            {
                DateTime date = (DateTime)selectedDate;
                
                DataTable dt = await Task.Run(() => MainWindow.billing.MonthlyBillSummaryTable(date));
                try
                {
                    txtResults.Clear();

                    ToggleTable(true);

                    dgData.DataContext = dt.DefaultView;
                }
                catch (Exception expt)
                {
                    Logging.Write(expt.Message);
                    System.Windows.MessageBox.Show("Exception : " + expt.Message);
                }
            }
            else
            {
                txbFeedBack.Text = "Failed to generate billing summary. Have you reconciled the MOH file?";
                SetTimer();
            }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void ViewResponse_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by ViewResponse for click events
        ///
        /// \author Arie
        /// \date   2019-04-21
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///-------------------------------------------------------------------------------------------------
        private void ViewResponse_Click(object sender, RoutedEventArgs e)
        {
            // Get the month and year
            Nullable<DateTime> selectedDate = SelectDate.SelectedDate();

            if (selectedDate != null)
            {
                DateTime date = (DateTime)selectedDate;

                var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                string query = "SELECT appointmentDate, HCN, gender, Code, fee, encounterState " +
                    "FROM ResponseRecord " +
                    "WHERE appointmentDate >= '" + firstDayOfMonth + "' " +
                    "AND appointmentDate <= '" + lastDayOfMonth + "'";

                LookupTable(query);
            }
        }
    }
}
