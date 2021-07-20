using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSchalter
{
    class Customer
    {
        static int count = 0;
        public int id;
        public TimeSpan serviceTime;
        public TimeSpan arrivalTime;
        public TimeSpan leaveTime;
        public int counterId;
        public bool isServed;

        public Customer(TimeSpan serviceTime, TimeSpan arrivalTime)
        {
            id = count;
            count++;
            this.serviceTime = serviceTime;
            this.arrivalTime = arrivalTime;
            isServed = false;

        }
    }
}
