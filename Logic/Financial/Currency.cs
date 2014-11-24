using System;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
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

        public static Currency FromIdentity(int currencyId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetCurrency(currencyId));
        }

        public static Currency FromIdentityAggressive(int currencyId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetCurrency(currencyId));  // "For writing" is intentional - replication timing issue
        }

        public static Currency FromCode(string code)
        {
            code = code.ToUpperInvariant();

            return FromBasic(SwarmDb.GetDatabaseForReading().GetCurrency(code));
        }

        public static Currency Create (string code, string name, string sign)
        {
            return FromIdentityAggressive(
                SwarmDb.GetDatabaseForWriting().CreateCurrency(code, name, sign));
        }

        public string DisplayCode
        {
            get
            {
                if (base.Code == "BTC")
                {
                    return "µBTC";
                }

                return base.Code;
            }
        }
    }
}
