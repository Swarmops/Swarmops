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

                if (base.Code == "BCH")
                {
                    return "&micro;BCH"; // HTML entity as above
                }

                return base.Code;
            }
        }

        public bool IsBitcoinCore
        {
            get { return (base.Code == "BTC"); }
        }


        public bool IsBitcoinCash
        {
            get { return (base.Code == "BCH"); }
        }

        public static Currency BitcoinCore
        {
            get { return Currency.FromCode ("BTC"); }
        }

        public static Currency BitcoinCash
        {
            get {  return Currency.FromCode("BCH"); }
        }


        public double GetConversionRate (Currency otherCurrency)
        {
            return GetConversionRate (otherCurrency, DateTime.UtcNow);
        }


        public double GetConversionRate (Currency otherCurrency, DateTime valuationDateTime)
        {
            if (this.IsBitcoinCore)
            {
                return SwarmDb.GetDatabaseForReading()
                    .GetCurrencyExchangeRate (otherCurrency.Identity, this.Identity, valuationDateTime);
            }
            else if (otherCurrency.IsBitcoinCore)
            {
                return 1.0 / SwarmDb.GetDatabaseForReading()
                    .GetCurrencyExchangeRate (this.Identity, otherCurrency.Identity, valuationDateTime);
            }
            else
            {
                // Neither is bitcoin core, so go via bitcoin core

                int bitcoinCurrencyId = Currency.BitcoinCore.Identity;
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

        public static Currency CreateFiat (string code, string name, string sign)
        {
            return FromIdentityAggressive (
                SwarmDb.GetDatabaseForWriting().CreateCurrency (code, name, sign));
        }

        public static Currency CreateCrypto(string code, string name, string sign)
        {
            return FromIdentityAggressive(
                SwarmDb.GetDatabaseForWriting().CreateCryptocurrency(code, name, sign));
        }
    }
}