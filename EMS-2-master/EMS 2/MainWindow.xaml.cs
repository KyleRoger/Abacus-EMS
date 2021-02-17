using System.Windows;
using EMS_2.Scheduling;
using Data;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Diagnostics;



namespace EMS_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static EMS_Billing.EMSBilling billing;

        public MainWindow()
		{          
            // DEBUG
            // Backup.Test();
            // DEBUG

            InitializeComponent();

            billing = new EMS_Billing.EMSBilling();
        }

		private void TabParent_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			// Refresh the calendar for any updates in the database whenever it is opened
			if (tabItemScheduling.IsSelected)
			{
				TabScheduling.tabScheduling.LoadCalendar(TabScheduling.tabScheduling.currentMonth);
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
        /// \fn private void ExportDB_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by ExportDB for click events
        ///
        /// \author Arie
        /// \date   2019-04-22
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///                 
        /// Credit: https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
        ///-------------------------------------------------------------------------------------------------
        private async void ExportDB_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "(*.bacpac)|*.bacpac";

            if (sfd.ShowDialog() == true)
            {
                string result = await Task.Run(() => Database.ExportDB(sfd.FileName));  // run asynchronously 
                System.Windows.MessageBox.Show(result);
            }
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn private void ImportDB_Click(object sender, RoutedEventArgs e)
        ///
        /// \brief  Event handler. Called by ImportDB for click events
        ///
        /// \author Arie
        /// \date   2019-04-22
        ///
        /// \param  sender  Source of the event.
        /// \param  e       Routed event information.
        ///                 
        /// Credit: https://stackoverflow.com/questions/4291912/process-start-how-to-get-the-output
        ///-------------------------------------------------------------------------------------------------
        private async void ImportDB_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.InitialDirectory = @".";
            ofd.Filter = "(*.bacpac)|*.bacpac";

            if (ofd.ShowDialog() == true)
            {
                string result = await Task.Run(() => Database.ImportDB(ofd.FileName));  // run asynchronously 
                System.Windows.MessageBox.Show(result);
            }
        }
    }
}
