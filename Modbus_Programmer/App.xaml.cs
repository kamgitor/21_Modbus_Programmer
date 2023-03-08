using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace CliConfigurator
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{

        public delegate void AppExitDelegate();
        public AppExitDelegate AppExit;

        private void Application_Exit(object sender, ExitEventArgs e)
        {

            AppExit();
        }

    }
}
