using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;


namespace Modbus_Programmer
{
    class CliDisputant : SerialPortGeneric
    {

        // Konstruktor podobno nie jest dziedziczony - taka konstrukcja zeby byl dziwdziczony
        public CliDisputant(ComboBox combo) : base(combo)
        {

        }	// Rs485


    }
}
