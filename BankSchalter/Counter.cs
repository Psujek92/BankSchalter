using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSchalter
{
    class Counter
    {
        static int count = 0;
        public int id;
        public List<Customer> customersQueue;
        public bool isOpen;
        public bool hasCustomers;
        public TimeSpan timeSinceLastCustomer;

        public Counter()
        {
            id = count;
            count++;
            isOpen = false;
            hasCustomers = false;
            timeSinceLastCustomer = DateTime.Now.TimeOfDay;
            customersQueue = new List<Customer>();

        }

        public void OpenCounter()
        {
            isOpen = true;
        }

        public void CloseCounter()
        {
            isOpen = false;
        }
    }
}
