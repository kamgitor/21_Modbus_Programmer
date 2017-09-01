using System;
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
		// SerialPortGeneric KamSerial;
		Rs485 KamSerial;

		// ***************************************************************
		public Window1()
		{
			InitializeComponent();

			AppInit();
		
		}	// Window1


		// ***************************************************************
		public void AppInit()
		{
			// KamSerial = new SerialPortGeneric(comboBoxPorts);
			KamSerial = new Rs485(comboBoxPorts);

			

		}	// AppInit


		// ***************************************************************
		private void button1_Click(object sender, RoutedEventArgs e)
		{
			KamSerial.PreparePort();
			KamSerial.BaudRate = 19200;
			KamSerial.TxRxStartProcess(TxRxProcess);

		}	// AppInit


		// ***************************************************************************
		private void TxRxProcess()
		{
			byte[] tx_buf = {1, 2, 3, 4};
			byte[] rx_buf;
			int rx_size;

			KamSerial.SendFrame(tx_buf, 4, false);
			bool ret = KamSerial.ReceiveFrame(100, out rx_buf, out rx_size);

		}	// TxRxProcess


		// ***************************************************************************
		private void TxRxEnd()
		{

		}	// TxRxEnd


		/*
		// ***************************************************************************
		void rx_function(byte [] buf, int size)
		{
			// throw new NotImplementedException();

		}	// SerialPort
		*/

	}

}
