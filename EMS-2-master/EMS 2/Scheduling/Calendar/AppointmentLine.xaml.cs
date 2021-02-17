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

using Scheduling;

namespace EMS_2.Scheduling.Calendar
{
	public partial class AppointmentLine : UserControl
	{
		// Loads appointment details into the appointment line
		public AppointmentLine(Appointment appointment)
		{
			InitializeComponent();

			lblAppointmentLine.Content = appointment.AppointmentID.ToString();
		}

	}
}
