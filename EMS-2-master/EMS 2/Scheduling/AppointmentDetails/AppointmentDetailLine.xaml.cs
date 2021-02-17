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
	public partial class AppointmentDetailLine : UserControl
	{
		private bool clickable;
		private bool bookable;

		public AppointmentDetailLine()
		{
			InitializeComponent();

			clickable = false;
			bookable = false;
		}


		public void ClearDetails()
		{
			text.Content = "";
		}


		public void UpdateAppointmentDetails(List<Demographics.Patient> patientList)
		{
			text.Content = "MISSING";
		}
		

		public void UpdateClickability(bool enabled)
		{
			background.Fill = enabled ? Brushes.White : Brushes.LightGray;
			clickable = enabled;
		}


		public void UpdateAddButton(bool enabled)
		{
			if (enabled)
			{
				text.Content = "+";
				bookable = true;
			}
			else
			{
				text.Content = "";
				bookable = false;
			}
		}

		private void GrdAppointmentDetails_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (clickable)
			{
				if (bookable)
				{
					MessageBox.Show("booking");
				}
				else
				{
					MessageBox.Show("inspecting");
				}
			}
		}
	}
}
