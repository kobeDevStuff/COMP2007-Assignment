using System;
using System.Collections;
using System.IO;

namespace CSharpOneApp
{
    class Program
    {
        static int Main(string[] args)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "orders.csv");
            OrderProcessor processor = new OrderProcessor(new StandardLineValidator());
            // subscribing to an event
            processor.OrderRejected += new OrderRejectedHandler(OnOrderRejected);

            ArrayList orders;
            try
            {
                orders = processor.LoadOrders(path);
            }
            catch (IOException ex)
            {
                Console.WriteLine("could not read " + path + ": " + ex.Message);
                return 1;
            }

            decimal grandTotal = 0m;
            foreach (Order order in orders)
            {
                Console.WriteLine("order " + order.OrderId + ": total " + order.Total + ", net " + order.NetTotal);
                grandTotal += order.NetTotal;
            }
            foreach (Order order in OrderQueries.FindOver(orders, Order.DiscountThreshold))
            {
                Console.WriteLine("high value: " + order.OrderId);
            }
            Console.WriteLine("accepted " + processor.AcceptedLines + ", rejected " + processor.RejectedLines + ", grand total " + grandTotal);
            return 0;
        }

        static void OnOrderRejected(object sender, OrderRejectedEventArgs e)
        {
            Console.WriteLine("rejected line " + e.LineNumber + ": " + e.Reason);
        }
    }
}
