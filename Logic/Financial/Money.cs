using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.Financial
{
    public class Money
    {
        public Currency Currency { get; set; }
        public Int64 Cents { get; set; }
        public DateTime ValuationDateTime { get; set; } 

        public Money ToCurrency (Currency newCurrency)
        {
            double conversionRate = this.Currency.GetConversionRate (newCurrency, this.ValuationDateTime);

            return new Money {
                Currency = newCurrency,
                Cents = (Int64) (this.Cents * conversionRate),
                ValuationDateTime = this.ValuationDateTime };
        }
    }
}
