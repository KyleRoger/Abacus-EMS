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

namespace EMS_2.Scheduling.AppointmentDetails
{
	public partial class AppointmentDetails : UserControl
	{
		public AppointmentDetails()
		{
			InitializeComponent();

			for (int i = 1; i <= SchedulingSupport.MAX_APPOINTMENTS_WEEKDAY; i++)
			{
				AppointmentDetailLine appointment = new AppointmentDetailLine();
				appointment.ClearDetails();
				grdAppointments.Children.Add(appointment);
				Grid.SetColumn(appointment, 0);
				Grid.SetRow(appointment, i);
			}
		}
	}
}
