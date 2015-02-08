using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.Financial
{
    [Serializable]
    public class Money
    {
        public Money()
        {
            // default ctor for serializability
        }

        public Money (Int64 cents, Currency currency)
            : this (cents, currency, DateTime.UtcNow)
        {
        }

        public Money (Int64 cents, Currency currency, DateTime valuationDateTime)
        {
            this.Cents = cents;
            this.Currency = currency;
            this.ValuationDateTime = valuationDateTime;
        }

        public Currency Currency { get; set; }
        public Int64 Cents { get; set; }
        public DateTime ValuationDateTime { get; set; } 

        public Money ToCurrency (Currency newCurrency)
        {
            if (newCurrency.Identity == this.Currency.Identity)
            {
                // no conversion
                return this;
            }

            double conversionRate = this.Currency.GetConversionRate (newCurrency, this.ValuationDateTime);

            return new Money {
                Currency = newCurrency,
                Cents = (Int64) (this.Cents * conversionRate),
                ValuationDateTime = this.ValuationDateTime };
        }
    }
}
