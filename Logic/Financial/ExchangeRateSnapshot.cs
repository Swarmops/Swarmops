using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    [Serializable]
    public class ExchangeRateSnapshot: BasicExchangeRateSnapshot
    {
        [Obsolete ("Do not call this ctor directly. Intended for serialization only.", true)]
        public ExchangeRateSnapshot()
        {
            // do not call
        }

        private ExchangeRateSnapshot (BasicExchangeRateSnapshot basic)
            : base (basic)
        {
            // private ctor

            // TODO: Verify that data is in, or load it
        }

        public static ExchangeRateSnapshot FromBasic (BasicExchangeRateSnapshot basic)
        {
            return new ExchangeRateSnapshot (basic);
        }


        public static void Create()
        {
            // We're getting rates against the fiat currencies from BitPay and crypto-to-crypto from Shapeshift.

            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                int bitcoinId = GetOrCreateCryptocurrency("BTC");
                int exchangeRateSnapshotId = SwarmDb.GetDatabaseForWriting().CreateExchangeRateSnapshot();

                // Download Shapeshift data

                string cryptoRateDataRaw = client.DownloadString("https://shapeshift.io/marketinfo");

                ShapeshiftRateDatapoint[] cryptoRates =
                    (ShapeshiftRateDatapoint[])serializer.Deserialize<ShapeshiftRateDatapoint[]>(cryptoRateDataRaw);

                if (exchangeRateSnapshotId > 0) // test that we're making a snapshot first
                {
                    foreach (ShapeshiftRateDatapoint shapeshiftRate in cryptoRates)
                    {
                        if (shapeshiftRate.pair.StartsWith("BTC_"))
                        {
                            try
                            {
                                string coinCode = shapeshiftRate.pair.Substring(4);
                                double btcRate = Double.Parse(shapeshiftRate.rate);

                                if (coinCode != "BCH")
                                {
                                    btcRate /= 1000000.0;
                                    // We're operating in microbitcoin, so adjust the stored exchange rate right six decimal places
                                    // EXCEPT for Bitcoin Cash which ALSO operates in microbitcoin
                                }

                                int coinId = GetOrCreateCryptocurrency(coinCode);

                                SwarmDb.GetDatabaseForWriting().
                                    CreateExchangeRateDatapoint(exchangeRateSnapshotId, coinId, bitcoinId, btcRate);
                            }
                            catch (Exception exception)
                            {
                                if (exception is FormatException || exception is ArgumentNullException ||
                                    exception is OverflowException)
                                {
                                    // Double parse error, we don't care
                                    continue;
                                }

                                throw;
                            }
                        }
                    }
                }

                // Download BitPay data

                string fiatRateDataRaw = client.DownloadString("https://bitpay.com/api/rates");

                // BitPay doesn't provide valid JSON - the rate field isn't enclosed in quotes - so we can't use JSON Deserialization; we'll
                // have to use Regex matching instead.

                string regexPattern = @"\{\""code\"":\""([A-Z]+)\"",\""name\"":\""([^\""]+)\"",\""rate\"":([0-9\.]+)\}";
                Regex regex = new Regex (regexPattern);
                Match match = regex.Match (fiatRateDataRaw);

                while (match.Success)
                {
                    string currencyCode = match.Groups[1].Value;
                    string currencyName = match.Groups[2].Value;
                    double btcRate = Double.Parse (match.Groups[3].Value, NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture); // rounding errors and loss of precision ok, don't use Formatting fn

                    btcRate /= 1000000.0; // We're operating in microbitcoin, so adjust the stored exchange rate accordingly (right-shift six decimal places)

                    int currencyId = GetOrCreateFiatCurrency (currencyCode, currencyName);
                    Currency currency = Currency.FromIdentityAggressive(currencyId);

                    // Only store if it's not a cryptocurrency provided by Shapeshift

                    if (!currency.IsCrypto)
                    {
                        SwarmDb.GetDatabaseForWriting()
                            .CreateExchangeRateDatapoint(exchangeRateSnapshotId, currencyId, bitcoinId, btcRate);
                    }

                    match = match.NextMatch();
                }

                // BitpayRateDatapoint[] fiatRates = (BitpayRateDatapoint[]) serializer.Deserialize<BitpayRateDatapoint[]> (fiatRateDataRaw);
                // Console.WriteLine(cryptoRateDataRaw);
            }
        }

        private static int GetOrCreateFiatCurrency (string currencyCode, string currencyName)
        {
            try
            {
                return Currency.FromCode (currencyCode).Identity;
            }
            catch (ArgumentException)
            {
                return Currency.CreateFiat (currencyCode, currencyName, string.Empty).Identity;
            }
        }


        private static int GetOrCreateCryptocurrency(string currencyCode)
        {
            try
            {
                return Currency.FromCode(currencyCode).Identity;
            }
            catch (Exception)
            {
                // We don't know about this crypto yet. Get its name from our cache, or populate the cache first.

                if (coinDataLookup == null || !coinDataLookup.ContainsKey(currencyCode))
                {
                    using (WebClient client = new WebClient())
                    {
                        string coins = client.DownloadString("https://shapeshift.io/getcoins");
                        JavaScriptSerializer serializer = new JavaScriptSerializer();

                        coinDataLookup = serializer.Deserialize<SerializableDictionary<string, ShapeshiftCoinData>>(coins);
                    }
                }

                string coinName = string.Empty;
                if (coinDataLookup.ContainsKey(currencyCode))
                {
                    coinName = coinDataLookup[currencyCode].name;
                }

                return Currency.CreateCrypto(currencyCode, coinName, string.Empty).Identity;
            }
        }


        private static Dictionary<string, ShapeshiftCoinData> coinDataLookup = null;
    }

    [Serializable]
    public class BitpayRateDatapoint
    {
        public string code { get; set; }
        public string name { get; set; }
        public string rate { get; set; }
    }

    // "rate":"0.04286453","limit":16718.99510938,"pair":"BLK_CLAM","maxLimit":16718.99510938,"min":0.04257204,"minerFee":0.001

    [Serializable]
    public class ShapeshiftRateDatapoint
    {
        public string rate { get; set; }  // changed to string to be able to decentralize "NaN" rates from Shapeshift
        public double limit { get; set; }
        public string pair { get; set; }
        public double maxLimit { get; set; }
        public double min { get; set; }
        public double minerFee { get; set; }
    }

    [Serializable]
    public class ShapeshiftCoinData
    {
        public string name;
        public string symbol;
        public string image;
        public string imageSmall;
        public string status;
    }
}
