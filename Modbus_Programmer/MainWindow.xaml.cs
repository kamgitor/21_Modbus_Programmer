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
		
		}	// Window1


		// ***************************************************************
		public void AppInit()
		{
			// KamSerial = new SerialPortGeneric(comboBoxPorts);
			KamSerial = new Rs485(comboBoxPorts);

		}	// AppInit


		// ***************************************************************
		private void Button_ReadParams_Click(object sender, RoutedEventArgs e)
		{
			button_ReadParams.Visibility = Visibility.Hidden;

			if (comboBoxPorts.SelectedIndex != -1)
			{
				KamSerial.PreparePort();
				// KamSerial.BaudRate = 19200;
				// KamSerial.BaudRate = 115200;
				KamSerial.TxRxStartProcess(ReadParamsProcess);
			}
			else
			{
				MessageBox.Show("Wybierz port szeregowy", "Informacja");
				button_ReadParams.Visibility = Visibility.Visible;
			}

		}	// AppInit


		// ***************************************************************************
		private void Button_SaveParams_Click(object sender, RoutedEventArgs e)
		{
			button_SaveParams.Visibility = Visibility.Hidden;

			if (comboBoxPorts.SelectedIndex != -1)
			{
				KamSerial.PreparePort();

				rs_new_speed = (byte)ComboBoxBaud.SelectedIndex;
				rs_adres = (ushort)Int32.Parse(TextBoxAdres.Text);

				if ((rs_adres <= 0) || (rs_adres >= 255))
				{
					MessageBox.Show("Wybrany niepoprawny adres (1 - 254)", "Informacja");
					button_SaveParams.Visibility = Visibility.Visible;
				}
				else
					KamSerial.TxRxStartProcess(WriteParamsProcess);
			}
			else
			{
				MessageBox.Show("Wybierz port szeregowy", "Informacja");
				button_SaveParams.Visibility = Visibility.Visible;
			}

		}	// Button_SaveParams_Click


		// ***************************************************************************
		// na starcie ukrycie kontrolek
		private void InitControlHide()
		{
			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
			{
				ModuleGroupBox.Visibility = Visibility.Hidden;
				ComboBoxBaud.Visibility = Visibility.Hidden;
				TextBoxAdres.Visibility = Visibility.Hidden;
				button_SaveParams.Visibility = Visibility.Hidden;
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
				button_SaveParams.Visibility = Visibility.Visible;

				ComboBoxBaud.SelectedIndex = rs_speed;
				TextBoxAdres.Text = rs_adres.ToString();
			});

		}	// PresentReadParams


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

			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
			{
				button_ReadParams.Visibility = Visibility.Visible;
			});

		}	// ReadParamsProcess


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

			KamSerial.BaudRate = baud;
			KamSerial.SendFrame(tx_buf);
			ret = KamSerial.ReceiveFrame(100, out rx_buf, out rx_size);
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

			KamSerial.BaudRate = baud;
			
			KamSerial.SendFrame(tx_buf);

			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate()
			{
				ProgressBarWrite.Visibility = Visibility.Visible;
				ProgressBarWrite.Value = progress + 25;
			});

			ret = KamSerial.ReceiveFrame(100, out rx_buf, out rx_size);

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
