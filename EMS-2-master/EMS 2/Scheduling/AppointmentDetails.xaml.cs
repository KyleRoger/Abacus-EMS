/*
 * 
 * Author:      Bailey Mills
 * Date:        2019-04-16
 * Project:     EMS2
 * File:        AppointmentDetails.xaml.cs
 * Description: Holds all AppointmentDetailLines in itself and has a method to
 *				reset all of them if necessary
 * 
*/

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

namespace EMS_2.Scheduling
{
	public partial class AppointmentDetails : UserControl
	{
		List<AppointmentDetailLine> lines = new List<AppointmentDetailLine>();

		///-------------------------------------------------------------------------------------------------
		/// \fn public AppointmentDetails()
		///
		/// \brief  Adds all 6 appointment detail lines to their proper place
		///
		/// \author Bailey
		/// \date   2019-04-16
		///-------------------------------------------------------------------------------------------------
		public AppointmentDetails()
		{
			InitializeComponent();

			for (int i = 0; i < SchedulingSupport.MAX_APPOINTMENTS_WEEKDAY; i++)
			{
				AppointmentDetailLine appointment = new AppointmentDetailLine();
				appointment.ClearDetails();
				grdAppointments.Children.Add(appointment);
				Grid.SetColumn(appointment, 0);
				Grid.SetRow(appointment, i);

				lines.Add(appointment);
			}
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn public void ResetLines()
		///
		/// \brief  Resets all appointment detail lines in the object's holders
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		public void ResetLines()
		{
			foreach (AppointmentDetailLine line in lines)
			{
				line.ClearDetails();
				line.UpdateClickability(false);
			}
		}
	}
}
