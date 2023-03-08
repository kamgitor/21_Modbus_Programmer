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

// using System.Diagnostics;			// Trace

using System.Threading;				// Watki Thread
using System.Windows.Threading;

using Console_Manager;
using System.Configuration;

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

		private enum app
		{
			zero,
			one,
			two,
			three,
		}

		struct cfg_t
		{
			public app AppNumber;
			public string com1;
			public string com2;
		};

		private cfg_t cfg = new cfg_t();

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
		public void LoadAppConfig()
        {
			/*	Witek settings
			Properties.Settings.Default.myColor = "COM5";
			Properties.Settings.Default.Save();
			*/


			cfg.com1 = ConfigurationManager.AppSettings["com1"];
			if (cfg.com1 == null)
			{
				cfg.com1 = "COM5";
				ConfigurationManager.AppSettings["com1"] = cfg.com1;
			}

			cfg.com1 = ConfigurationManager.AppSettings["com1"];

			cfg.com2 = ConfigurationManager.AppSettings["com2"];
			if (cfg.com2 == null)
			{
				cfg.com2 = "COM6";
				ConfigurationManager.AppSettings["com2"] = cfg.com2;
			}

			string ap_num = ConfigurationManager.AppSettings["app_num"];
			if (ap_num == null)
			{
				ap_num = "3";
				ConfigurationManager.AppSettings["app_num"] = ap_num;
			}

			switch (ap_num)
            {
				case "1":
					cfg.AppNumber = app.one;
					break;
				case "3":
					cfg.AppNumber = app.three;
					break;
				default:
					cfg.AppNumber = app.one;
					break;
            }

		}   // LoadAppConfig


		// ***************************************************************
		public void SaveAppConfig()
        {
			cfg.com1 = Cli1.GetPortInCombo();
			cfg.com2 = Cli2.GetPortInCombo();

			ConfigurationManager.AppSettings["com1"] = cfg.com1;
			ConfigurationManager.AppSettings["com2"] = cfg.com2;

			string ap_num;
			switch (cfg.AppNumber)
			{
				case app.one:
					ap_num = "1";
					break;
				case app.three:
					ap_num = "3";
					break;
				default:
					ap_num = "1";
					break;
			}
			ConfigurationManager.AppSettings["app_num"] = ap_num;

		}   // SaveAppConfig


		// ***************************************************************
		public void AppInit()
		{
			RadioApp3.IsChecked = true;

			Cli1 = new CliPort(comboBoxPorts1);
			Cli2 = new CliPort(comboBoxPorts2);

			LoadAppConfig();
			((App)Application.Current).AppExit = new App.AppExitDelegate(SaveAppConfig);

			Cli1.SetPortInCombo(cfg.com1);
			Cli2.SetPortInCombo(cfg.com2);
			SetAppNumber(cfg.AppNumber);

			// NU for now
			CliDisp = new CliDisputant(Cli1, Cli2);

			ConsoleManager.Show();
			Console.WriteLine("HELLO");
			Console.WriteLine("Reset both boards");

		}   // AppInit




		// ***************************************************************
		private bool AreComboParamsSelected()
        {
			if ((comboBoxPorts1.SelectedIndex != -1) && (comboBoxPorts2.SelectedIndex != -1))
			{
				Cli1.BaudRate = 115200;
				Cli1.PreparePort();

				Cli2.BaudRate = 115200;
				Cli2.PreparePort();

				return true;
			}
			else
			{
				MessageBox.Show("Select serial ports", "Info");
				return false;
			}

		}   // AreComboParamsSelected


		// ***************************************************************
		private bool ClosePorts()
        {
			Cli1.ClosePort();
			Cli2.ClosePort();

			return true;

		}   // ClosePorts


		// ***************************************************************
		private void ButtonsShow(bool state)
        {
			if (state)
            {
				button_Prepare.Visibility = Visibility.Visible;
				button_Start.Visibility = Visibility.Visible;
			}
			else
            {
				button_Prepare.Visibility = Visibility.Hidden;
				button_Start.Visibility = Visibility.Hidden;
			}
		
		}   // ButtonsShow


		// ***************************************************************
		private app GetAppNumber()
        {
			if (RadioApp1.IsChecked == true)
				return app.one;

			if (RadioApp3.IsChecked == true)
				return app.three;

			return app.zero;

		}   // GetAppNumber


		// ***************************************************************
		private void SetAppNumber(app ap_num)
        {
			RadioApp1.IsChecked = false;
			RadioApp3.IsChecked = false;

			if (ap_num == app.one)
			{
				RadioApp1.IsChecked = true;
				return;
			}

			if (ap_num == app.three)
			{
				RadioApp3.IsChecked = true;
				return;
			}

		}   // SetAppNumber


		// ***************************************************************
		private void Button_Prepare_Click(object sender, RoutedEventArgs e)
		{
			cfg.AppNumber = GetAppNumber();

			this.

			ButtonsShow(false);
			try
			{
				Console.Clear();		// It crashes in debug mode
			}
			catch { }

			switch (cfg.AppNumber)
			{
				case app.one:
					
					if (AreComboParamsSelected())
						Cli1.TxRxStartProcess(App1PrepareProcess);
					
					break;

			case app.three:

				if (AreComboParamsSelected())
					Cli1.TxRxStartProcess(App3PrepareProcess);
				
					break;
			}

			ButtonsShow(true);

		}   // Button_Prepare_Click


		// ***************************************************************
		private void Button_Start_Click(object sender, RoutedEventArgs e)
		{
			ButtonsShow(false);


			switch (cfg.AppNumber)
			{
				case app.one:
					if (AreComboParamsSelected())
						Cli1.TxRxStartProcess(App1StartProcess);

					break;

				case app.three:

					if (AreComboParamsSelected())
						Cli1.TxRxStartProcess(App3StartProcess);

					break;
			}

			ButtonsShow(true);

		}   // Button_Start_Click


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

			ClosePorts();

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
		private void App1PrepareProcess()
        {
			bool success = false;
			string rxbuf;
			string dev_number;

			do
			{
				// SOURCE
				if (CliSendCommand(Cli2, "11 1\n", 200, out rxbuf, 10) == false)
					break;

				// SINK
				if (CliSendCommand(Cli1, "2 1\n", 2000, out rxbuf, 20) == false)
					break;

				// SINK
				if (CliSendCommand(Cli1, "2 0\n", 3000, out rxbuf, 40) == false)
					break;

				if (CliGetDeviceNumber(rxbuf, out dev_number, "App1_Source") == false)
					break;

				decimal value;
				if (Decimal.TryParse(dev_number, out value) == false)
					break;

				if (CliSendCommand(Cli1, "7\n", 200, out rxbuf, 50) == false)
					break;

				if (CliSendCommand(Cli1, "5 0 " + dev_number + " 1\n", 1000, out rxbuf, 80) == false)
					break;

				if (CliSendCommand(Cli1, "1\n", 200, out rxbuf, 90) == false)
					break;

				if (CliSendCommand(Cli1, "2 1\n", 2000, out rxbuf, 100) == false)
					break;

				success = true;

			} while (false);

			if (success)
				Console.WriteLine("SUCCESS");
			else
				Console.WriteLine("FAIL");

			ClosePorts();

		}   // App1PrepareProcess


		// ***************************************************************************
		private void App1StartProcess()
		{
			string rxbuf;

			do
			{
				// SOURCE
				if (CliSendCommand(Cli2, "12\n", 200, out rxbuf, 100) == false)
					break;

			} while (false);

			ClosePorts();

		}   // App1PrepareProcess


		// ***************************************************************************
		private void App3PrepareProcess()
		{
			bool success = false;
			string rxbuf;
			string dev_number;

			do
			{
				// SOURCE
				if (CliSendCommand(Cli2, "1 1\n", 200, out rxbuf, 5) == false)
					break;

				if (CliSendCommand(Cli2, "1 0\n", 3000, out rxbuf, 10) == false)
					break;

				if (CliGetDeviceNumber(rxbuf, out dev_number, "App3_Server") == false)
					break;

				decimal value;
				if (Decimal.TryParse(dev_number, out value) == false)
					break;
				
				if (CliSendCommand(Cli2, "5 " + dev_number + "\n", 1000, out rxbuf, 15) == false)
					break;

				if (CliSendCommand(Cli2, "3\n", 3000, out rxbuf, 20) == false)
					break;

				if (CliSendCommand(Cli2, "20 1 4\n", 1000, out rxbuf, 25) == false)
					break;

				if (CliSendCommand(Cli2, "37\n", 100, out rxbuf, 30) == false)
					break;

				if (CliSendCommand(Cli2, "3\n", 100, out rxbuf, 35) == false)
					break;

				if (CliSendCommand(Cli2, "1\n", 500, out rxbuf, 40) == false)
					break;

				if (CliSendCommand(Cli2, "22 1 1 1 1 40000\n", 5000, out rxbuf, 45) == false)
					break;

				if (CliSendCommand(Cli2, "23 1 1\n", 1000, out rxbuf, 50) == false)
					break;

				// SINK
				if (CliSendCommand(Cli1, "26\n", 1000, out rxbuf, 55) == false)
					break;

				if (CliSendCommand(Cli1, "7 1 1 0", 1000, out rxbuf, 60) == false)
					break;

				// SOURCE
				if (CliSendCommand(Cli2, "37\n", 1000, out rxbuf, 65) == false)
					break;

				if (CliSendCommand(Cli2, "5 1 1 0\n", 1000, out rxbuf, 70) == false)
					break;

				// SINK
				if (CliSendCommand(Cli1, "\n", 500, out rxbuf, 75) == false)
					break;

				if (CliSendCommand(Cli1, "18 1 0 4\n", 1000, out rxbuf, 80) == false)
					break;

				// SOURCE
				if (CliSendCommand(Cli2, "18 0 0 4\n", 1000, out rxbuf, 85) == false)
					break;

				if (CliSendCommand(Cli2, "1\n", 100, out rxbuf, 90) == false)
					break;

				if (CliSendCommand(Cli2, "38\n", 1000, out rxbuf, 90) == false)
					break;

				if (CliSendCommand(Cli2, "5 1\n", 1000, out rxbuf, 95) == false)
					break;

				if (CliSendCommand(Cli2, "6 1 4 2 1 2\n", 1000, out rxbuf, 100) == false)
					break;

				success = true;

			} while (false);

			if (success)
				Console.WriteLine("SUCCESS");
			else
				Console.WriteLine("FAIL");

			ClosePorts();

			// MessageBox.Show("Nie znaleziono urządzeń", "Informacja");



			/*
			????????? No idea is this OK
			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
			{
				button_Prepare.Visibility = Visibility.Visible;
			});
			*/

		}   // ReadParamsProcess


		// ***************************************************************************
		private void App3StartProcess()
        {
			string rxbuf;

			do
			{
				// SOURCE
				if (CliSendCommand(Cli2, "7 10000 16000 1\n", 1000, out rxbuf, 100) == false)
					break;

			} while (false);

			ClosePorts();

		}   // App3StartProcess


		// ***************************************************************************
			private bool CliSendCommand(CliPort cli, string frame, int timeout, out string rxbuf, byte progress)
		{
			byte[] rx_buf;
			int size;
			bool ret;

			rxbuf = "";

			this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
			{
				ProgressBarRead.Visibility = Visibility.Visible;
				ProgressBarRead.Value = progress;
			});

			cli.BaudRate = 115200;
			cli.SendFrame(Encoding.ASCII.GetBytes(frame));
			ret = cli.ReceiveFrame(timeout, out rx_buf, out size);

			if (rx_buf != null)
			{
				rxbuf = System.Text.Encoding.Default.GetString(rx_buf);

				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(rxbuf);
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("***************************************************************");
			}
			else
            {
				Console.WriteLine("***************************************************************");
				Console.WriteLine("rx_buf is null");
				return false;
			}

			return ret;

		}   // CliSendCommand


		// ***************************************************************************
		private bool CliGetDeviceNumber(string console_data, out string dev_num, string app_server_name)
		{
			dev_num = "";

			if (console_data == null)
				return false;

			string [] str_tab = console_data.Split('\n');

			foreach (string s in str_tab)
            {
				if (s.EndsWith(app_server_name))
                {
					string[] str_tab2 = s.Split(':');
					dev_num = str_tab2[0];
					if (dev_num != "  AD Name")
						return true;
				}
            }
			return false;

		}   // CliGetDeviceNumber


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

		}   // WriteParamsProcess


        /*
		// ***************************************************************************
		void rx_function(byte [] buf, int size)
		{
			// throw new NotImplementedException();

		}	// SerialPort
		*/

    }

}
