using Project2.Server;
using Project2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Project2.Client
{
    class Retailer
    {
        private int currentPrice;
        private int oldPrice;
        private CancellationToken serverSessionToken;
        public event Program.orderPlacedEvent orderPlaced; // Event to let chickenFarm know that a new order is available to process
        private AutoResetEvent priceCutEvent = new AutoResetEvent(false);
        private Random generator = new Random();
        private int noOfThreads;
        private bool[] done;    //  set if current thread placed order (to avoid duplicate orders for a retailer thread)
        private MultiCellBuffer buffer; //  Get buffer instance

        public Retailer(CancellationToken token, MultiCellBuffer buf)
        {
            buffer = buf;
            serverSessionToken = token;
            currentPrice = 9999;
            oldPrice = 9999;
        }

        //  Threads starter
        public void StartRetailers(int threads)
        {
            noOfThreads = threads;

            Thread[] retailers = new Thread[noOfThreads]; //create 5 threads 
            for (int i = 0; i < noOfThreads; i++) // N = 5 here
            {
                //Start N retailer threads

                retailers[i] = new Thread(new ThreadStart(run)); //starts thread with retailer function
                retailers[i].Name = (i + 1).ToString();
                retailers[i].Start();
            }
        }


        //  Thread runner
        public void run()
        {
            Console.WriteLine("Retailer ID: " + Thread.CurrentThread.Name.ToString() + " is online.");

            while (!serverSessionToken.IsCancellationRequested)
            {
                //WaitHandle.WaitAny(new WaitHandle[] { priceCutEvent, orderProcessed });

                priceCutEvent.WaitOne();

                lock (this)
                {
                    int idx = int.TryParse(Thread.CurrentThread.Name, out idx) ? idx - 1 : 0;

                    //if (priceCutEvent.WaitOne())
                    if (currentPrice < oldPrice && !done[idx])  //  Check price cut and if its not a second order from the same thread
                    {
                        OrderClass newOrder = BuildOrder();
                        Console.WriteLine("Retailer " + newOrder.senderId + " : order ready : TIMESTAMP " + newOrder.timestamp);

                        buffer.setOneCell(newOrder.encode());
                        // send order placed event to server
                        orderPlaced();

                        done[idx] = true;
                        
                        if (!done.All(x => x == true))
                            priceCutEvent.Set();
                    }
                }


            }
        }


        // Event Handler for Order Processes Confirmation from OrderProcessing thread
        public void OrderProcessed(OrderClass order, float totalPayment)
        {
            Console.WriteLine("Retailer {0} : order processed. Bill Total = $" + totalPayment + " ($" + order.orderPrice + " per chicken for " + order.amount + " chicken/s) : TIMESTAMP " + order.timestamp, order.senderId);
        }


        // Event handler on Price Cut Event from Server
        public void UpdatePrice(int pricePerChick) 
        {
            object obj = currentPrice;

            lock (this)
            {
                currentPrice = pricePerChick;
                done = new bool[noOfThreads];
            }
            
            priceCutEvent.Set();
        }

        //  Build Order object
        private OrderClass BuildOrder()
        {
            string senderId = Thread.CurrentThread.Name;
            int cardNo = generator.Next(4575, 7025);  //  Random no generator for credit card
            int amount = generator.Next(1, 10);
            return new OrderClass(senderId, cardNo, amount, currentPrice);
        }
    }
}
