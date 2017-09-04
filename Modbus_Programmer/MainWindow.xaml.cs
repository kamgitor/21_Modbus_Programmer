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
		byte rs_speed;
		int rs_adres;

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
			// KamSerial.BaudRate = 115200;
			KamSerial.TxRxStartProcess(TxRxProcess);

		}	// AppInit


		// ***************************************************************************
		private void TxRxProcess()
		{
			// #define		MODBUS_BAUD_DEFAULT				1		// 9600
			// #define		MODBUS_ADDRESS_DEFAULT			1

			// 0		MODBUS_BROADCAST_ADDRESS		255
			// 1		Read Holgind Register
			// 2		Start addres hi
			// 3		Start addres lo
			// 4		No of points hi		(16bit data)
			// 5		No of points lo
			// 6		crc l
			// 7		crc h


			if (AskModbusModule(2400) == false)
				if (AskModbusModule(9600) == false)
					if (AskModbusModule(19200) == false)
						if (AskModbusModule(57600) == false)
							AskModbusModule(115200);




/*
			if (ret == false)
			{
				// ret = SendReceiveFrame(tx_buf, 115200);

				KamSerial.BaudRate = 115200;	// 4
				KamSerial.SendFrame(tx_buf, false);
				ret = KamSerial.ReceiveFrame(100, out rx_buf, out rx_size);
				rs_speed = 4;
				rs_adres = (rx_buf[5] << 8) + rx_buf[6];
			}
*/

		}	// TxRxProcess


		// ***************************************************************************
		// private bool SendReceiveFrame(byte [] tx_buf, int baud)
		private bool AskModbusModule(int baud)
		{
			bool ret;
			byte[] rx_buf;
			int rx_size;
			byte[] tx_buf = { 255, 3, 0, 0, 0, 3 };
			
			rs_speed = 0xFF;
			rs_adres = 0xFF;

			KamSerial.BaudRate = baud;
			KamSerial.SendFrame(tx_buf, false);
			ret = KamSerial.ReceiveFrame(100, out rx_buf, out rx_size);
			if (ret == true)
			{
				rs_speed = 4;
				rs_adres = (rx_buf[5] << 8) + rx_buf[6];
				return true;
			}
			else
				return false;

		}	// SendReceiveFrame


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
