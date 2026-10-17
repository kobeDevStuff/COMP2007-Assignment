using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace CSharpOneApp
{
    // a delegate type, event and raising it
    public delegate void OrderRejectedHandler(object sender, OrderRejectedEventArgs e);

    public class OrderRejectedEventArgs : EventArgs
    {
        private int lineNumber;
        private string rawLine;
        private string reason;

        public OrderRejectedEventArgs(int lineNumber, string rawLine, string reason)
        {
            this.lineNumber = lineNumber;
            this.rawLine = rawLine;
            this.reason = reason;
        }

        public int LineNumber { get { return lineNumber; } }
        public string RawLine { get { return rawLine; } }
        public string Reason { get { return reason; } }
    }

    public interface IOrderLineValidator
    {
        // null means valid
        string Validate(OrderLine line);
    }

    public class StandardLineValidator : IOrderLineValidator
    {
        public string Validate(OrderLine line)
        {
            if (line.Quantity <= 0)
            {
                return "quantity " + line.Quantity + " must be greater than zero";
            }
            if (line.UnitPrice < 0m || Math.Round(line.UnitPrice, 2) != line.UnitPrice)
            {
                return "unit price " + line.UnitPrice + " is not a valid amount";
            }
            return null;
        }
    }

    public class OrderProcessor
    {
        private IOrderLineValidator validator;
        private int acceptedLines;
        private int rejectedLines;

        // raised when there's a rejected line
        public event OrderRejectedHandler OrderRejected;

        public OrderProcessor(IOrderLineValidator validator)
        {
            this.validator = validator;
        }

        public int AcceptedLines { get { return acceptedLines; } }
        public int RejectedLines { get { return rejectedLines; } }

        public ArrayList LoadOrders(string path)
        {
            ArrayList orders = new ArrayList();
            Hashtable ordersById = new Hashtable();

            using (StreamReader reader = new StreamReader(path)) // closes the file
            {
                reader.ReadLine(); // skip header
                string text;
                int lineNumber = 1;
                while ((text = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    string[] fields = text.Split(',');
                    if (fields.Length != 5)
                    {
                        Reject(lineNumber, text, "expected 5 fields");
                        continue;
                    }
                    string quantityText = fields[3];

                    int quantity;
                    try
                    {
                        quantity = int.Parse(quantityText, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        Reject(lineNumber, text, "quantity '" + quantityText + "' is not a whole number");
                        continue;
                    }
                    catch (OverflowException)
                    {
                        Reject(lineNumber, text, "quantity '" + quantityText + "' is out of range");
                        continue;
                    }

                    decimal unitPrice = decimal.Parse(fields[4], NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                    OrderLine line = new OrderLine(fields[2], quantity, unitPrice);
                    string reason = validator.Validate(line);
                    if (reason != null)
                    {
                        Reject(lineNumber, text, reason);
                        continue;
                    }

                    Order order = (Order)ordersById[fields[0]]; // needs a cast
                    if (order == null)
                    {
                        order = new Order(fields[0], fields[1]);
                        ordersById[fields[0]] = order;
                        orders.Add(order);
                    }
                    order.AddLine(line);
                    acceptedLines++;
                }
            }
            return orders;
        }

        private void Reject(int lineNumber, string rawLine, string reason)
        {
            rejectedLines++;
            if (OrderRejected != null)
            {
                OrderRejected(this, new OrderRejectedEventArgs(lineNumber, rawLine, reason));
            }
        }
    }

    // no static classes in c# 1.0
    public sealed class OrderQueries
    {
        private OrderQueries() { }

        public static ArrayList FindOver(ArrayList orders, decimal threshold)
        {
            ArrayList result = new ArrayList();
            foreach (Order order in orders)
            {
                if (order.Total > threshold)
                {
                    result.Add(order);
                }
            }
            return result;
        }
    }
}
