using System;
using System.Collections;

namespace CSharpOneApp
{
    // demos for section 2.3
    class AnalysisDemos
    {
        static void Main(string[] args)
        {
            ArrayList lines = new ArrayList();
            lines.Add(new OrderLine("PEN", 10, 1.50m));
            lines.Add(new OrderLine("BOOK", 2, 24.99m));
            Console.WriteLine("[Simplicity] " + TotalForeach(lines) + " " + TotalFor(lines));

            Orthogonality();
            DataTypes();
            TypeChecking();
            RestrictedAliasing();
        }

        static decimal TotalForeach(ArrayList lines)
        {
            decimal sum = 0m;
            foreach (OrderLine line in lines) { sum += line.LineTotal; }
            return sum;
        }

        static decimal TotalFor(ArrayList lines)
        {
            decimal sum = 0m;
            for (int i = 0; i < lines.Count; i++) { sum += ((OrderLine)lines[i]).LineTotal; }
            return sum;
        }

        static void Orthogonality()
        {
            OrderLine a = new OrderLine("PEN", 10, 1.50m); // struct
            Order x = new Order("1001", "C01"); // class
            Order y = x;
            Console.WriteLine("[Orthogonality] same object, class : " + Object.ReferenceEquals(x, y));
            Console.WriteLine("[Orthogonality] same object, struct: " + Object.ReferenceEquals(a, a));
            // prints True for the class and False for the struct
        }

        static void DataTypes()
        {
            double d = 0.1 + 0.2;
            decimal m = 0.1m + 0.2m;
            Console.WriteLine("[Data types] double: " + (d == 0.3) + ", decimal: " + (m == 0.3m));
            // prints: double: False, decimal: True
        }

#if ERROR_ASSIGN_IN_IF
        static void AssignInCondition(int quantity) { if (quantity = 0) { } }
        // error CS0029: Cannot implicitly convert type 'int' to 'bool'
#endif

        static void TypeChecking()
        {
            ArrayList orders = new ArrayList();
            orders.Add(new Order("1001", "C01"));
            orders.Add(new OrderLine("PEN", 1, 1.50m)); // compiles: the list holds object
            try
            {
                foreach (Order o in orders) {
                    Console.WriteLine("[Type checking] read " + o.OrderId);
                }
            }
            catch (InvalidCastException ex) {
                Console.WriteLine("[Type checking] " + ex.Message);
            }
            // prints: [Type checking] read 1001
            // then: [Type checking] Unable to cast object of type 'CSharpOneApp.OrderLine' to type 'CSharpOneApp.Order'.
        }

        static void RestrictedAliasing()
        {
            Order order = new Order("1002", "C02");
            order.AddLine(new OrderLine("DESK", 1, 520.00m));
            ArrayList all = new ArrayList();
            all.Add(order);
            ArrayList high = OrderQueries.FindOver(all, 500m);
            ((Order)high[0]).AddLine(new OrderLine("LAMP", 1, 15.00m));
            Console.WriteLine("[Aliasing] total through the other list: " + ((Order)all[0]).Total);

            try { order.Lines.Add(new OrderLine("X", 1, 1m)); }
            catch (NotSupportedException ex) { Console.WriteLine("[Aliasing] " + ex.Message); }
            // prints 535.00, then: Collection is read-only.
        }
    }
}
