using Project2.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project2.Server
{
    class ChickenFarm
    {
        private int pCounter;   //  to count price cuts
        private Random generator = new Random();
        //private int seed;
        private int flag;
        private MultiCellBuffer buffer;
        private readonly CancellationTokenSource session = new CancellationTokenSource();
        
        public int price { get; set; }
        
        public event Program.priceCutEvent priceCut;

        
        public ChickenFarm(MultiCellBuffer buf, int p = 9)
        {
            buffer = buf;
            price = PricingModel();
            pCounter = p;
        }

        //  Thread starter
        public void StartServer()
        {
            Thread serverThread = new Thread(new ThreadStart(run));
            serverThread.Start();
        }

        //  Thread Runner
        public void run()
        {
            CancellationToken token = session.Token;

            //priceUpdate();

            while (!token.IsCancellationRequested)
                //while (pCounter > 0)
            {
                Thread.Sleep(generator.Next(500,2000));
                try
                {

                    if (pCounter > 0)
                    {
                        priceUpdate();
                        Console.WriteLine("\n-------- P Counter " + pCounter + " : Price $" + price + " --------");
                    }

                    if (pCounter < 1 && buffer.isEmpty())
                        session.Cancel();
                }
                catch (Exception e)
                {
                    Console.WriteLine(" Server terminating. ");
                }

            }

        }

        //  Process order from buffer
        public void ProcessOrder()
        {
            // get order from Buffer
            string data = buffer.getOneCell();
            
            OrderClass order = OrderClass.decode(data);
            //seed = DateTime.Now.Millisecond;
            Thread thread = new Thread(() => OrderProcessing.ProcessOrder(order));
            thread.Start();
        }

        //  Get session validity token. Expires on server session end.
        public CancellationToken getSessionToken()
        {
            return session.Token;
        }

        //  Update Price, notify retailers if price cut
        public void priceUpdate()   //  make private later
        {
            int newPrice = PricingModel();
            
            if (newPrice < price)  //  Later change to less than 
            {
                pCounter--;

                Console.WriteLine("\nServer: Price Update -> CurrentPrice = $" + newPrice + " : OldPrice = $" + price);
                price = newPrice;

                var eventObj = priceCut;
                if (eventObj != null)   // check for atleast 1 subscriber
                    eventObj(price);
            }
            else //if (newPrice == price)
            {
                flag++;
                if (flag >= 4)
                {
                    flag = 0;
                    price = price + 1;
                }
            }
                
            
        }

        //  random pricing model
        private int PricingModel()
        {
            return generator.Next(5, 100);
            return generator.Next(5, 100);
        }
    }
}
