using Project2.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static Project2.Program;

namespace Project2.Server
{
    //  Server side static order processing class
    public static class OrderProcessing
    {
        // Event is triggered on order processing completion
       public static event Program.orderProcessedEvent processingDone;

        //  Process Order and notify retailer
        public static void ProcessOrder(OrderClass order)
        {
            if (isCardValid(order.cardNo))
            {
                // 9% tax added to total payment with $10 fixed shipping handling charge
                float totalPayment = (float)(1.9 * order.amount * order.orderPrice + 10);
                processingDone(order, totalPayment);
            }
            else
                Console.WriteLine("Retailer " + order.senderId + " : Card No. " + order.cardNo.ToString() + " failed validity check.");
        }

        //  Check credit card validity
        private static bool isCardValid(int cardNo)
        {
            if (cardNo > 5000 && cardNo < 7000)
                return true;
            return false;
        }
    }
}
