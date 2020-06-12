//sjain106@asu.edu | 1215218576
    

using Project2.Client;
using Project2.Server;
using Project2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project2
{
    public class Program
    {
        //  Event Delegates
        public delegate void orderProcessedEvent(OrderClass order, float totalPayment);
        public delegate void priceCutEvent(int pricePerChick);
        public delegate void orderPlacedEvent();

        static void Main(string[] args)
        {

            //  Buffer between server and client
            MultiCellBuffer buffer = new MultiCellBuffer(3);

            ChickenFarm c = new ChickenFarm(buffer);    //  Give buffer and P value (optional) to server program
            Retailer r = new Retailer(c.getSessionToken(), buffer); //  Give server session token and buffer to client program

            
            c.priceCut += new priceCutEvent(r.UpdatePrice);
            r.orderPlaced += new orderPlacedEvent(c.ProcessOrder);
            OrderProcessing.processingDone += new orderProcessedEvent(r.OrderProcessed);

            //  Start Threads
            r.StartRetailers(5);
            c.StartServer();


        }

    }
}
