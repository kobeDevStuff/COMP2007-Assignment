using System;
using System.Collections;

namespace CSharpOneApp
{
    // one line of the csv
    public struct OrderLine
    {
        private string productCode;
        private int quantity;
        private decimal unitPrice;

        public OrderLine(string productCode, int quantity, decimal unitPrice)
        {
            this.productCode = productCode;
            this.quantity = quantity;
            this.unitPrice = unitPrice;
        }

        public string ProductCode { get { return productCode; } }
        public int Quantity { get { return quantity; } }
        public decimal UnitPrice { get { return unitPrice; } }
        public decimal LineTotal { get { return quantity * unitPrice; } }
    }

    public class Order
    {
        public const decimal DiscountThreshold = 500.00m;
        public const decimal DiscountRate = 0.10m;

        private string orderId;
        private string customerId;
        private ArrayList lines = new ArrayList(); // holds object

        public Order(string orderId, string customerId)
        {
            this.orderId = orderId;
            this.customerId = customerId;
        }

        // a property with a custom accessor
        public string OrderId { get { return orderId; } }
        public string CustomerId { get { return customerId; } }
        public ArrayList Lines { get { return ArrayList.ReadOnly(lines); } } // checked at runtime

        public void AddLine(OrderLine line)
        {
            lines.Add(line);
        }

        public decimal Total
        {
            get
            {
                decimal sum = 0m;
                foreach (OrderLine line in lines)
                {
                    sum += line.LineTotal;
                }
                return sum;
            }
        }

        public decimal Discount
        {
            get
            {
                if (Total > DiscountThreshold)
                {
                    return Math.Round(Total * DiscountRate, 2, MidpointRounding.AwayFromZero); // round half up
                }
                return 0m;
            }
        }

        public decimal NetTotal { get { return Total - Discount; } }
    }
}
