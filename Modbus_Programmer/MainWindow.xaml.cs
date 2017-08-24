﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Modbus_Programmer
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		SerialPortGeneric KamSerial;

		// ***************************************************************
		public Window1()
		{
			InitializeComponent();

			AppInit();
		
		}	// Window1


		// ***************************************************************
		public void AppInit()
		{
			KamSerial = new SerialPortGeneric(comboBoxPorts);

			

		}	// AppInit


		// ***************************************************************
		private void button1_Click(object sender, RoutedEventArgs e)
		{
			byte[] buf = {1, 2, 3, 4, 5};

			KamSerial.PreparePort();

			KamSerial.BaudRate = 19200;
			KamSerial.SendFrame(buf, 5, false);

		}	// AppInit


		// ***************************************************************************
		void rx_function(byte [] buf, int size)
		{
			// throw new NotImplementedException();

		}	// SerialPort

	}

}
