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

namespace EMS_2.Patients
{
	/// <summary>
	/// Interaction logic for TabPatient.xaml
	/// </summary>
	public partial class TabPatient : UserControl
	{
        //string modPatient = "Modify Patients";
        //string addPatient = "Add Patients";
        //string searchPatient = "Search Patients";
        //string hohReport = "Head Of Household Report";

        PatientInformation Patient = new PatientInformation();
        ModifyPatient searchMod = new ModifyPatient();
        HoHReport searchReport = new HoHReport();

        public TabPatient()
		{
			InitializeComponent();
        }

        public void dbPatientInsert()
        {
            // call database connect
            //Insert into database into patients file if the health card number does not already exist.
        }

        void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                searchMod.StartNewSearch();
                searchReport.StartNewSearch();

            }
        }

    }
}
