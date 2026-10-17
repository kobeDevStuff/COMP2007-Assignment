using System;
using System.Globalization;

namespace CSharpModernisation
{
    // currency markers
    public readonly struct Aud { }
    public readonly struct Usd { }

    // a count, not money
    public readonly struct Quantity
    {
        public int Value { get; }

        public Quantity(int value)
        {
            Value = value;
        }
    }

    // a ratio, e.g. a discount
    public readonly struct Rate
    {
        public decimal Value { get; }

        public Rate(decimal value)
        {
            Value = value;
        }
    }

    // money in one currency
    public readonly struct Money<TCurrency>
    {
        public decimal Amount { get; }

        public Money(decimal amount)
        {
            Amount = amount;
        }

        // same currency only
        public static Money<TCurrency> operator +(Money<TCurrency> a, Money<TCurrency> b)
        {
            return new Money<TCurrency>(a.Amount + b.Amount);
        }

        public static Money<TCurrency> operator -(Money<TCurrency> a, Money<TCurrency> b)
        {
            return new Money<TCurrency>(a.Amount - b.Amount);
        }

        // price times count
        public static Money<TCurrency> operator *(Money<TCurrency> price, Quantity quantity)
        {
            return new Money<TCurrency>(price.Amount * quantity.Value);
        }

        // money times rate
        public static Money<TCurrency> operator *(Money<TCurrency> amount, Rate rate)
        {
            return new Money<TCurrency>(amount.Amount * rate.Value);
        }

        // round half up to cents
        public Money<TCurrency> Round()
        {
            return new Money<TCurrency>(Math.Round(Amount, 2, MidpointRounding.AwayFromZero));
        }

        public override string ToString()
        {
            return Amount.ToString("F2", CultureInfo.InvariantCulture);
        }
    }
}
