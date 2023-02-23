using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace CliConfigurator
{
    class CliPort : SerialPortGeneric
    {

        // Konstruktor podobno nie jest dziedziczony - taka konstrukcja zeby byl dziwdziczony
        public CliPort(ComboBox combo) : base(combo)
        {

        }

    }
}
