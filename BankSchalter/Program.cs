using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSchalter
{
    class Program
    {
        static void Main()
        {
            Bank bank = new Bank();
            bank.SimulationInit();
            bank.SimulationStart();
        }
    }
}
