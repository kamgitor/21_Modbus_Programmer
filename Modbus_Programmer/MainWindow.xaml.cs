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

using System.Diagnostics;

using System.Threading;				// Watki Thread
using System.Windows.Threading;

namespace CliConfigurator
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		// SerialPortGeneric KamSerial;
		CliPort Cli1;
		CliPort Cli2;

		CliDisputant CliDisp;

		byte rs_speed;
		byte rs_new_speed;
		ushort rs_adres;

		private readonly int[] baud_tab = { 2400, 9600, 19200, 57600, 115200 };
		private const int MODBUS_OFFSET_OF_ADDRES = 1;
		private const int MODBUS_OFFSET_OF_BAUD = 2;


		// ***************************************************************
		public Window1()
		{
			InitializeComponent();

			InitControlHide();

			AppInit();

		}   // Window1


		// ***************************************************************
		public void AppInit()
		{
			Cli1 = new CliPort(comboBoxPorts1);
			Cli2 = new CliPort(comboBoxPorts2);

			Cli1.SetPortInCombo("COM35");
			Cli2.SetPortInCombo("COM36");

			// Cli1.SetPortInCombo("COM2");

			// NU for now
			CliDisp = new CliDisputant(Cli1, Cli2);

		}   // AppInit


		// ***************************************************************
		private void Button_SendSetOfCommand_Click(object sender, RoutedEventArgs e)
		{
			button_ReadParams.Visibility = Visibility.Hidden;

			if ((comboBoxPorts1.SelectedIndex != -1) && (comboBoxPorts2.SelectedIndex != -1))
			{
				Cli1.BaudRate = 115200;
				Cli1.PreparePort();

				Cli2.BaudRate = 115200;
				Cli2.PreparePort();

				Cli1.TxRxStartProcess(ReadParamsProcess);
			}
			else
			{
				MessageBox.Show("Select serial ports", "Info");
				button_ReadParams.Visibility = Visibility.Visible;
			}

		}   // AppInit


		// ***************************************************************************
		private void Button_SaveParams_Click(object sender, RoutedEventArgs e)
		{
			button_SaveParams.Visibility = Visibility.Hidden;

			if (comboBoxPorts1.SelectedIndex != -1)
			{
				Cli1.PreparePort();

				rs_new_speed = (byte)ComboBoxBaud.SelectedIndex;
				rs_adres = (ushort)Int32.Parse(TextBoxAdres.Text);

				if ((rs_adres <= 0) || (rs_adres >= 255))
				{
					MessageBox.Show("Wybrany niepoprawny adres (1 - 254)", "Informacja");
					button_SaveParams.Visibility = Visibility.Visible;
				}
				else
					Cli1.TxRxStartProcess(WriteParamsProcess);
			}
			else
			{
				MessageBox.Show("Wybierz port szeregowy", "Informacja");
				button_SaveParams.Visibility = Visibility.Visible;
			}

		}   // Button_SaveParams_Click


		// ***************************************************************************
		// na starcie ukrycie kontrolek
		private void InitControlHide()
		{
			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
			{
				ModuleGroupBox.Visibility = Visibility.Hidden;
				ComboBoxBaud.Visibility = Visibility.Hidden;
				TextBoxAdres.Visibility = Visibility.Hidden;
				button_SaveParams.Visibility = Visibility.Hidden;
				ProgressBarWrite.Visibility = Visibility.Hidden;
				ProgressBarRead.Visibility = Visibility.Hidden;
			});

		}   // InitControlHide


		// ***************************************************************************
		private void HideProgressBars()
		{
			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
			{
				ProgressBarWrite.Visibility = Visibility.Hidden;
				ProgressBarRead.Visibility = Visibility.Hidden;
			});

		}   // HideProgressBars


		// ***************************************************************************
		private void PresentReadParams()
		{
			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
			{
				ModuleGroupBox.Visibility = Visibility.Visible;
				ComboBoxBaud.Visibility = Visibility.Visible;
				TextBoxAdres.Visibility = Visibility.Visible;
				button_SaveParams.Visibility = Visibility.Visible;

				ComboBoxBaud.SelectedIndex = rs_speed;
				TextBoxAdres.Text = rs_adres.ToString();
			});

		}   // PresentReadParams


		// ***************************************************************************
		private void ReadParamsProcess()
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


			bool ret;
			string rxbuf;
			string rxbuf2;
			string dev_number;

			ret = CliSendCommand(Cli2, "1 1\n", 2000, out rxbuf, 10);

			Trace.WriteLine(rxbuf);
			Trace.WriteLine("*************************************************************** 1");
			ret = CliSendCommand(Cli2, "1 0\n", 2000, out rxbuf2, 20);
			ret = CliGetDeviceNumber(rxbuf2, out dev_number);
			Trace.WriteLine(rxbuf2);
			Trace.WriteLine("*************************************************************** 2");



			// test
			ret = CliSendCommand(Cli1, "27\n", 1000, out rxbuf, 10);



			/*
			if (AskModbusModule(2400, 20) == false)
				if (AskModbusModule(9600, 40) == false)
					if (AskModbusModule(19200, 60) == false)
						if (AskModbusModule(57600, 80) == false)
							if (AskModbusModule(115200, 100) == false)
								success = false;
		*/

			/*
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
			*/

			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
			{
				button_ReadParams.Visibility = Visibility.Visible;
			});

		}   // ReadParamsProcess


		private bool CliSendCommand(CliPort cli, string frame, int timeout, out string rxbuf, byte progress)
		{
			byte[] rx_buf;
			int size;
			bool ret;

			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
			{
				ProgressBarRead.Visibility = Visibility.Visible;
				ProgressBarRead.Value = progress;
			});

			cli.BaudRate = 115200;
			cli.SendFrame(Encoding.ASCII.GetBytes(frame));
			ret = cli.ReceiveFrame(timeout, out rx_buf, out size);

			rxbuf = System.Text.Encoding.Default.GetString(rx_buf);

			return ret;

		}   // CliSendCommand


		private bool CliGetDeviceNumber(string console_data, out string dev_num)
		{
			dev_num = "";

			if (console_data == null)
				return false;

			string [] str_tab = console_data.Split('\n');

			foreach (string s in str_tab)
            {
				if (s.EndsWith("App3_Server"))
                {
					string[] str_tab2 = s.Split(':');
					dev_num = str_tab2[0];
					break;
				}
            }

			return true;
		}


		// ***************************************************************************
		// private bool SendReceiveFrame(byte [] tx_buf, int baud)
		private bool AskModbusModule(int baud, byte progress)
		{
			bool ret;
			byte[] rx_buf;
			int rx_size;
			byte[] tx_buf = { 255, 3, 0, 0, 0, 3 };		// 255 - brodcast addres, 3 - get holding regs, 0 - adr_hi, 0 - adr_lo, 0 - size_hi, 3 - size_lo


			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,(ThreadStart)delegate()
			{
				ProgressBarRead.Visibility = Visibility.Visible;
				ProgressBarRead.Value = progress;
			});

			rs_speed = 0xFF;
			rs_adres = 0xFF;

			Cli1.BaudRate = baud;
			Cli1.SendFrame(tx_buf);
			ret = Cli1.ReceiveFrame(150, out rx_buf, out rx_size);		// 100ms
			if (ret == true)
			{
				rs_speed = rx_buf[8];
				rs_adres = rx_buf[6];
				return true;
			}
			else
				return false;

		}	// SendReceiveFrame


		// ***************************************************************************
		private bool PresetSingleModbusRegister(int baud, ushort adr, ushort val, byte progress)
		{
			bool ret;
			byte[] rx_buf;
			int rx_size;
			byte[] tx_buf = { 255, 6, 0, 1, 0, 0 };			// 255 - broadcast slave addres, 6 - function Preset Single Register, 0 - adr_hi, 1 - adr_lo, 0 - data_hi, 0 - data_lo

			// 16 Preset Multiple Registers		- moduly tego moga nie wspierac
			// 06 Preset Single Register

			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
			{
				ProgressBarWrite.Visibility = Visibility.Visible;
				ProgressBarWrite.Value = progress;
			});

			tx_buf[2] = (byte)(adr >> 8);
			tx_buf[3] = (byte)adr;
			tx_buf[4] = (byte)(val >> 8);
			tx_buf[5] = (byte)val;

			Cli1.BaudRate = baud;
			
			Cli1.SendFrame(tx_buf);

			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
			{
				ProgressBarWrite.Visibility = Visibility.Visible;
				ProgressBarWrite.Value = progress + 25;
			});

			ret = Cli1.ReceiveFrame(100, out rx_buf, out rx_size);

			return ret;
		
		}	// PresetSingleModbusRegister


		// ***************************************************************************
		private void WriteParamsProcess()
		{
			bool ret;

			ret = PresetSingleModbusRegister(baud_tab[rs_speed], MODBUS_OFFSET_OF_ADDRES, rs_adres, 25);

			if (ret == true)
				PresetSingleModbusRegister(baud_tab[rs_speed], MODBUS_OFFSET_OF_BAUD, rs_new_speed, 75);			// Takie troche oszukanstwo, bo ta ramka juz zmienia bauda
																												// i odpowiedzi idzie juz po innym baudzue

			if (ret == false)
				MessageBox.Show("Nie znaleziono urządzenia", "Informacja");

			
			rs_speed = rs_new_speed;		// zeby przy zmianie adresu dzialo dwukrotne wcisniecie zapis

			HideProgressBars();

			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
			{
				button_SaveParams.Visibility = Visibility.Visible;
			});

		}	// WriteParamsProcess





		/*
		// ***************************************************************************
		void rx_function(byte [] buf, int size)
		{
			// throw new NotImplementedException();

		}	// SerialPort
		*/

	}

}
