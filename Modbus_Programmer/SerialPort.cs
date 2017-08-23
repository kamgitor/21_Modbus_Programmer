using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;



namespace Modbus_Programmer
{

	delegate void rx_ext_funct(byte[] buf, int size);

	class SerialPort : System.IO.Ports.SerialPort
	{
		private ComboBox combobox;

		private rx_ext_funct rx_funct;

		// ***************************************************************************
		public SerialPort(ComboBox combo, rx_ext_funct funct)
		{
			combobox = combo;
			InitSerialComboBox();

			rx_funct = funct;

			this.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(rx_handler_funct);


		}	// SerialPort



		// ***************************************************************************
		void rx_handler_funct(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
		{

			int rx_size = this.BytesToRead;
			byte[] buffer = new byte[rx_size];

			int iReaded = this.Read(buffer, 0, rx_size);

			rx_funct(buffer, rx_size);

		}	// rx_handler_funct



		// ***************************************************************************
		// Wsadzenie istniejacych portow do comboboxa
		private void InitSerialComboBox()
		{
			string [] portNames = System.IO.Ports.SerialPort.GetPortNames();

			combobox.Items.Clear();

			for (int i = 0; i < portNames.Length; i++)
			{
				combobox.Items.Add(portNames[i]);
			}

		}	// InitSerialPorts


		// ***************************************************************************
		// Przygotowuje port do odpalenia ret false - problem
		public bool PreparePort()
		{
			if (this.IsOpen)
				this.Close();

			if (combobox.SelectedIndex != -1)
			{
				this.PortName = (string)combobox.Items[combobox.SelectedIndex];
				this.Open();

				return true;
			}
			else
				return false;		// not selected

		}	// GetComboboxPos


		/*

		Wojtek uzywal
		IsOpen
		Close
		Write(buffer, 0, length);
		 
			int iReadedBytes = serialPort1.BytesToRead;
		int iReaded = serialPort1.Read(buffer, 0, iReadedBytes);
		
		BaudRate = 19200;
		DataReceived
		 
		 
		 */

	}
}
