using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Types;

namespace Activizr.Logic.Financial
{
    public class Currency: BasicCurrency
    {
        private Currency (BasicCurrency basic): base (basic)
        {
            // empty ctor
        }

        public static Currency FromBasic (BasicCurrency basic)
        {
            return new Currency(basic);
        }

        public static Currency FromIdentity (int currencyId)
        {
            if (currencyId == 1)
            {
                return FromBasic(new BasicCurrency(1, "SEK", "Swedish Krona", "SEK"));
            }

            throw new NotImplementedException("Currency.FromIdentity");
        }

        public static Currency FromCode (string code)
        {
            code = code.ToUpperInvariant();

            if (code == "SEK")
            {
                return FromIdentity(1);
            }

            throw new NotImplementedException("Currency.FromIdentity");
        }
    }
}
