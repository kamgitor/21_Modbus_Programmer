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

using System.Threading;				// Watki Thread
using System.Windows.Threading;

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

			InitControlHide();

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
			if (comboBoxPorts.SelectedIndex != -1)
			{
				KamSerial.PreparePort();
				KamSerial.BaudRate = 19200;
				// KamSerial.BaudRate = 115200;
				KamSerial.TxRxStartProcess(TxRxProcess);
			}
			else
				MessageBox.Show("Wybierz port szeregowy", "Informacja");

		}	// AppInit


		// ***************************************************************************
		// na starcie ukrycie kontrolek
		private void InitControlHide()
		{
			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
			{
				ModuleGroupBox.Visibility = Visibility.Hidden;
				ComboBoxBaud.Visibility = Visibility.Hidden;
				TextBoxAdres.Visibility = Visibility.Hidden;
				button_saveParams.Visibility = Visibility.Hidden;
				ProgressBarWrite.Visibility = Visibility.Hidden;
				ProgressBarRead.Visibility = Visibility.Hidden;
			});

		}	// InitControlHide


		// ***************************************************************************
		private void HideProgressBars()
		{
			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
			{
				ProgressBarWrite.Visibility = Visibility.Hidden;
				ProgressBarRead.Visibility = Visibility.Hidden;
			});

		}	// HideProgressBars

		// ***************************************************************************
		private void PresentReadParams()
		{
			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
			{
				ModuleGroupBox.Visibility = Visibility.Visible;
				ComboBoxBaud.Visibility = Visibility.Visible;
				TextBoxAdres.Visibility = Visibility.Visible;
				button_saveParams.Visibility = Visibility.Visible;

				ComboBoxBaud.SelectedIndex = rs_speed;
				TextBoxAdres.Text = rs_adres.ToString();
			});

		}	// PresentReadParams


		// ***************************************************************************
		private void TxRxProcess()
		{
			bool success = true;

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


			if (AskModbusModule(2400, 20) == false)
				if (AskModbusModule(9600, 40) == false)
					if (AskModbusModule(19200, 60) == false)
						if (AskModbusModule(57600, 80) == false)
							if (AskModbusModule(115200, 100) == false)
								success = false;


			if (success)
			{
				PresentReadParams();
				HideProgressBars();
			}
			else
			{
				MessageBox.Show("Nie znaleziono urządzeń", "Informacja");
				InitControlHide();
			}

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
		private bool AskModbusModule(int baud, byte progress)
		{
			bool ret;
			byte[] rx_buf;
			int rx_size;
			byte[] tx_buf = { 255, 3, 0, 0, 0, 3 };


			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,(ThreadStart)delegate()
			{
				ProgressBarRead.Visibility = Visibility.Visible;
				ProgressBarRead.Value = progress;
			});

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
		private void Button_SaveParams_Click(object sender, RoutedEventArgs e)
		{

		}	// Button_SaveParams_Click


		/*
		// ***************************************************************************
		void rx_function(byte [] buf, int size)
		{
			// throw new NotImplementedException();

		}	// SerialPort
		*/

	}

}
