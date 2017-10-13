using System;
using NBitcoin;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
{
    public class Currency : BasicCurrency
    {
        private Currency (BasicCurrency basic) : base (basic)
        {
            // empty ctor
        }

        public string DisplayCode
        {
            get
            {
                if (base.Code == "BTC")
                {
                    return "&micro;BTC"; // HTML entity -- for display only
                }

                return base.Code;
            }
        }

        public bool IsBitcoin
        {
            get { return (base.Code == "BTC"); }
        }

        public static Currency Bitcoin
        {
            get { return Currency.FromCode ("BTC"); }
        }


        public double GetConversionRate (Currency otherCurrency)
        {
            return GetConversionRate (otherCurrency, DateTime.UtcNow);
        }


        public double GetConversionRate (Currency otherCurrency, DateTime valuationDateTime)
        {
            if (this.IsBitcoin)
            {
                return SwarmDb.GetDatabaseForReading()
                    .GetCurrencyExchangeRate (otherCurrency.Identity, this.Identity, valuationDateTime);
            }
            else if (otherCurrency.IsBitcoin)
            {
                return 1.0 / SwarmDb.GetDatabaseForReading()
                    .GetCurrencyExchangeRate (this.Identity, otherCurrency.Identity, valuationDateTime);
            }
            else
            {
                // Neither is bitcoin, so go via bitcoin

                int bitcoinCurrencyId = Currency.Bitcoin.Identity;
                double thisPerBitcoin = SwarmDb.GetDatabaseForReading()
                    .GetCurrencyExchangeRate (this.Identity, bitcoinCurrencyId, valuationDateTime);
                double otherPerBitcoin = SwarmDb.GetDatabaseForReading()
                    .GetCurrencyExchangeRate (otherCurrency.Identity, bitcoinCurrencyId, valuationDateTime);

                return otherPerBitcoin/thisPerBitcoin;  // returns other-per-this, allowing multiplication with exchange rate
            }
        }


        public static Currency FromBasic (BasicCurrency basic)
        {
            return new Currency (basic);
        }

        public static Currency FromIdentity (int currencyId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetCurrency (currencyId));
        }

        public static Currency FromIdentityAggressive (int currencyId)
        {
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetCurrency (currencyId));
            // "For writing" is intentional - replication timing issue
        }

        public static Currency FromCode (string code)
        {
            code = code.ToUpperInvariant();

            return FromBasic (SwarmDb.GetDatabaseForReading().GetCurrency (code));
        }

        public static Currency Create (string code, string name, string sign)
        {
            return FromIdentityAggressive (
                SwarmDb.GetDatabaseForWriting().CreateCurrency (code, name, sign));
        }
    }
}