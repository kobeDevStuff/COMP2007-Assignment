using System;

namespace CSharpModernisation
{
    class Program
    {
        static void Main(string[] args)
        {
            Before();
            After();
        }

#if ERROR_ADD_QUANTITY
        static void AddQuantityToMoney()
        {
            Money<Aud> total = new Money<Aud>(0m);
            Quantity quantity = new Quantity(10);
            total = total + quantity;
            // error CS0019: Operator '+' cannot be applied to operands of type 'Money<Aud>' and 'Quantity'
        }
#endif

#if ERROR_MIX_CURRENCY
        static void MixCurrencies()
        {
            Money<Aud> total = new Money<Aud>(0m);
            Money<Usd> usd = new Money<Usd>(5m);
            total = total + usd;
            // error CS0019: Operator '+' cannot be applied to operands of type 'Money<Aud>' and 'Money<Usd>'
        }
#endif

        // plain decimals
        static void Before()
        {
            decimal[] prices = { 1.50m, 24.99m };
            int[] quantities = { 10, 2 };
            decimal total = 0m;
            for (int i = 0; i < prices.Length; i++)
            {
                total += prices[i] * quantities[i];
                total += quantities[i]; // a count added to money compiles
            }
            Console.WriteLine("[before] order 1001 total: " + total);
        }

        // typed values
        static void After()
        {
            Money<Aud>[] prices = { new Money<Aud>(1.50m), new Money<Aud>(24.99m) };
            Quantity[] quantities = { new Quantity(10), new Quantity(2) };
            Money<Aud> total = new Money<Aud>(0m);
            for (int i = 0; i < prices.Length; i++)
            {
                total = total + prices[i] * quantities[i];
                // total = total + quantities[i]; // does not compile
            }
            Console.WriteLine("[after] order 1001 total: " + total);

            Money<Aud> desk = new Money<Aud>(520.00m) * new Quantity(1);
            Money<Aud> discount = (desk * new Rate(0.10m)).Round();
            Console.WriteLine("[after] order 1002 net: " + (desk - discount));
        }
    }
}
