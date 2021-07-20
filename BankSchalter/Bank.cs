using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankSchalter
{
    class Bank
    {
        TimeSpan startTime;
        bool customerLeft = false;
        //public TimeSpan openingTime;
        public int numberOfCounters;
        public TimeSpan waitForCustomerTime;
        public TimeSpan newCustomerTime;
        public TimeSpan minServiceTime;
        public TimeSpan maxServiceTime;
        public TimeSpan acceptedWaitingTime;

        int customersChangedQueue;
        int customersLeft;
        int customersServed;
        List<Counter> counters;
        List<Customer> customers;

        public TimeSpan eventTime;

        public Bank()
        {
            customersChangedQueue = 0;
            customersLeft = 0;
            customersServed = 0;
            counters = new List<Counter>();
            customers = new List<Customer>();
        }

        // Initial Bank settings
        public void SimulationInit()
        {
            bool initSuccesful = false;

            while (!initSuccesful)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("Bank initialization. Answer the following questions to start the simulation:");
                    //Console.Write("What's the opening time of the bank (amount of hours)?: ");
                    //openingTime = TimeSpan.Parse($"{Console.ReadLine()}:00:00");
                    Console.Write("What's the number of available counters?: ");
                    numberOfCounters = Convert.ToInt32(Console.ReadLine());
                    Console.Write("What's the time a counter waits for a new customer before closing (seconds)?: ");
                    waitForCustomerTime = TimeSpan.FromSeconds(Convert.ToDouble(Console.ReadLine()));
                    Console.Write("What's the time a new customer arrives at the bank (seconds)?: ");
                    newCustomerTime = TimeSpan.FromSeconds(Convert.ToDouble(Console.ReadLine()));
                    Console.Write("What's the minimum service time of a customer (seconds)?: ");
                    minServiceTime = TimeSpan.FromSeconds(Convert.ToDouble(Console.ReadLine()));
                    Console.Write("What's the maximum service time of a customer (seconds)?: ");
                    maxServiceTime = TimeSpan.FromSeconds(Convert.ToDouble(Console.ReadLine()));
                    Console.Write("What's the maximum time a customer is willing to wait in a queue (seconds)?: ");
                    acceptedWaitingTime = TimeSpan.FromSeconds(Convert.ToDouble(Console.ReadLine()));
                    if (numberOfCounters > 0
                        || Convert.ToDouble(waitForCustomerTime) > 0
                        || Convert.ToDouble(newCustomerTime) > 0
                        || Convert.ToDouble(minServiceTime) > 0
                        || Convert.ToDouble(maxServiceTime) > 0
                        || Convert.ToDouble(maxServiceTime) > Convert.ToDouble(minServiceTime)
                        || Convert.ToDouble(acceptedWaitingTime) > 0)
                    {
                        initSuccesful = true;
                    }
                    else
                    {
                        Console.Clear();
                        Console.Write("At least one of the required values didn't meet the criteria. Restarting the initialization.\nPress ENTER to continue... ");
                        Console.ReadLine();
                    }

                }
                catch (Exception)
                {
                    Console.Clear();
                    Console.Write("Oops, something went wrong. Restarting the initialization.\nPress ENTER to continue... ");
                    Console.ReadLine();
                }
            }

            Console.Clear();

            //Console.WriteLine($"Opening time: {openingTime}");
            Console.WriteLine($"Number of counters: {numberOfCounters}");
            Console.WriteLine($"New customer time: {newCustomerTime}");
            Console.WriteLine($"Min service time: {minServiceTime}");
            Console.WriteLine($"Max service time: {maxServiceTime}");
            Console.WriteLine();
            Console.Write("Initialization complete.\nPress ENTER to continue...");
            Console.ReadLine();      
        }
        public void SimulationStart()
        {
            startTime = DateTime.Now.TimeOfDay;
            bool exitSimulation = false;
            Random random = new Random();
            TimeSpan nextCustomerTime = startTime.Add(newCustomerTime);
            Console.Clear();
            // Starting variables
            for (int i = 1; i <= numberOfCounters; i++)
            {
                counters.Add(new Counter() { timeSinceLastCustomer = startTime, isOpen = true }) ;
            }
            // Main App loop
            do
            {

                while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Spacebar))
                {
                    // New customer arrival
                    if (DateTime.Now.TimeOfDay >= nextCustomerTime)
                    {
                        customers.Add(new Customer(TimeSpan.FromSeconds(random.Next(Convert.ToInt32(minServiceTime.TotalSeconds), Convert.ToInt32(maxServiceTime.TotalSeconds))), DateTimeOffset.Now.TimeOfDay));
                        nextCustomerTime = DateTime.Now.TimeOfDay.Add(newCustomerTime);
                        JoinQueue(customers[customers.Count - 1]);
                    }
                    // Customer served
                    foreach (Customer customer in customers.ToList())
                    {
                        if (customer.isServed == false && DateTime.Now.TimeOfDay >= customer.arrivalTime.Add(customer.serviceTime))
                        {
                            LeaveQueue(customer);
                            LeaveBank(customer);
                        }
                    }

                    // Customer changing queue
                    foreach (Customer customer in customers.ToList())
                    {
                        if (customer.isServed == false && DateTime.Now.TimeOfDay >= customer.arrivalTime.Add(acceptedWaitingTime))
                        {
                            LeaveQueue(customer);
                            JoinQueue(customer);
                            customersChangedQueue++;
                        }
                    }
                    // Closing a counter
                    foreach (Counter counter in counters)
                    {
                        if (counter.isOpen && !counter.hasCustomers && DateTime.Now.TimeOfDay - counter.timeSinceLastCustomer > waitForCustomerTime)
                        {
                            counter.isOpen = false;
                        }
                    }
                    // Opening a counter
                    if (AreQueuesFull() && AreCountersClosed())
                    {
                        foreach (Counter counter in counters)
                        {
                            if (!counter.isOpen) { counter.isOpen = true; break; }
                        }
                    }
                    PrintUserInterface();
                    Thread.Sleep(100);
                }
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.WriteLine("Simulation PAUSED. If you want to resume press SPACEBAR again. If you want to exit press ESCAPE... ");
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Spacebar: Console.Clear();  break;
                    case ConsoleKey.Escape: exitSimulation = true; break;
                }
            } while (!exitSimulation);

            
        }

        private void PrintUserInterface()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("The BANK\n");
            Console.WriteLine("Counter\t\tCustomers");
            foreach(Counter counter in counters)
            {
                Console.Write($"{counter.id} ");
                if(counter.isOpen){ Console.Write("(Open)"); } else { Console.Write("(Closed)"); }
                Console.Write($"\t{counter.customersQueue.Count}\n");
            }
            Console.WriteLine($"\nNumber of customers: {customers.Count}");
            Console.WriteLine($"Number of available counters: {counters.Count}");
            Console.WriteLine();
            Console.WriteLine($"Customers served: {customersServed}");
            Console.WriteLine($"Customers changed queue: {customersChangedQueue}");
            Console.WriteLine($"Customers left: {customersLeft}");
            
            Console.WriteLine("\nTo pause the simulation press SPACEBAR... ");
        }

        public Counter findShortestQueueCounter()
        {
            Counter shortestQueueCounter = counters[0];
            foreach (Counter counter in counters)
            {
                if(counter.isOpen && counter.customersQueue.Count < shortestQueueCounter.customersQueue.Count)
                {
                    shortestQueueCounter = counter;
                }
            }
            return shortestQueueCounter;
        }

        public void JoinQueue(Customer customer)
        {
            Counter shortestQueueCounter = findShortestQueueCounter();
            shortestQueueCounter.customersQueue.Add(customer);
            customer.counterId = shortestQueueCounter.id;
            shortestQueueCounter.hasCustomers = true;
        }

        public void LeaveQueue(Customer customer)
        {
            eventTime = DateTime.Now.TimeOfDay;
            counters[customer.counterId].customersQueue.Remove(customer);
            counters[customer.counterId].timeSinceLastCustomer = eventTime;
            if (counters[customer.counterId].customersQueue.Count == 0) { counters[customer.counterId].hasCustomers = false; }
            
        }
        public void LeaveBank(Customer customer)
        {
            customer.leaveTime = eventTime;
            customer.isServed = true;
            customers.Remove(customer);
            customersServed++;
        }
        bool AreQueuesFull()
        {
            bool areQueuesFull = true;
            foreach(Counter counter in counters)
            {
                if (counter.isOpen && counter.customersQueue.Count < 5) { areQueuesFull = false; }
            }
            return areQueuesFull;
        }

        bool AreCountersClosed()
        {
            bool areCountersClosed = false;
            foreach(Counter counter in counters)
            {
                if (!counter.isOpen) { areCountersClosed = true;}
            }
            return areCountersClosed;
        }
    }
}
