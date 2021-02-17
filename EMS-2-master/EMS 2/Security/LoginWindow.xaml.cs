/*
 * 
 * Author:      Bailey Mills
 * Date:        2019-04-20
 * Project:     EMS2
 * File:        LoginWindow.xaml.cs
 * Description: Gets the username and password from the user before allowing them to enter the
 *				main application
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
using System.Windows.Shapes;

using Data;
using EMS_2;

namespace EMS_2.Security
{
    public partial class LoginWindow : Window
	{
		///-------------------------------------------------------------------------------------------------
		/// \fn public LoginWindow()
		///
		/// \brief  Setup the db connectivity, begin allowing the user to submit login attempts to the server
		///
		/// \author Bailey
		/// \date   2019-04-20
		///-------------------------------------------------------------------------------------------------
		public LoginWindow()
        {
            InitializeComponent();
			Connection.ImportData();

			txtUsername.Focus();
        }



		///-------------------------------------------------------------------------------------------------
		/// \fn private void LoadProgram()
		///
		/// \brief  Launches the main application after login credientals have been validated
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void LoadProgram()
		{
			var cApp = ((App)Application.Current);
			cApp.MainWindow = new MainWindow();
			cApp.MainWindow.Show();
			this.Close();
		}



		///-------------------------------------------------------------------------------------------------
		/// \fn private void BtnLogin_Click(object sender, RoutedEventArgs e)
		///
		/// \brief  Attempts to log into the system when the button is pressed using the text boxes content
		///
		/// \author Bailey
		/// \date   2019-04-18
		///-------------------------------------------------------------------------------------------------
		private void BtnLogin_Click(object sender, RoutedEventArgs e)
		{
			if (Database.LoginAttempt(txtUsername.Text, txtPassword.Password))
			{
				LoadProgram();
				Support.Logging.Write("User logged in successfully");
			}
			else
			{
				txtPassword.Password = "";
				Support.Logging.Write("User failed to log in");
			}
		}
	}
}
