using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project2.Util
{
    public class OrderClass
    {
        public string senderId { get; set; }
        public int cardNo { get; set; }
        public int amount { get; set; }
        public int orderPrice { get; set; }

        public string timestamp { get; }


        public OrderClass(string senderid, int cardno, int amt, int price)
        {
            senderId = senderid;
            cardNo = cardno;
            amount = amt;
            orderPrice = price;
            timestamp = GetTimestamp(DateTime.Now);
        }

        private static String GetTimestamp(DateTime value)
        {
            return value.ToString("MM-dd-yyyy HH:mm:ss:ffff");
        }

        public string encode()
        {
            // Encode class object to string format => "senderId:cardNo:amount"
            return senderId + ":" + cardNo.ToString()
                            + ":" + amount.ToString()
                            + ":" + orderPrice.ToString();
        }

        public static OrderClass decode(string objString)
        {
            var data = objString.Split(':');

            string senderid = data[0].ToString();
            int cardno = int.TryParse(data[1], out cardno) ? cardno : -1;
            int amt = int.TryParse(data[2], out amt) ? amt : -1;
            int price = int.TryParse(data[3], out price) ? price : -1;


            if (cardno == -1)
                Console.WriteLine("Decoder parsing error for Card No.");

            if (amt == -1)
                Console.WriteLine("Decoder parsing error for Amount.");

            if (price == -1)
                Console.WriteLine("Decoder parsing error for Price.");

            return new OrderClass(senderid, cardno, amt, price);
            
        }
    }
}
