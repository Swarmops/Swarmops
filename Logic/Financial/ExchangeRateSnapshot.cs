using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            using(WebClient client = new WebClient())
            {
                SupportFunctions.DisableSslCertificateChecks(); // MONO BUG/MISFEATURE: Mono has no root certificates, so can't verify cert

                client.Encoding = Encoding.UTF8;
                string rateDataRaw = client.DownloadString("https://bitpay.com/api/rates");

                // BitPay doesn't provide valid JSON - the rate field isn't enclosed in quotes - so we can't use JSON Deserialization; we'll
                // have to use Regex matching instead.

                string regexPattern = @"\{\""code\"":\""([A-Z]+)\"",\""name\"":\""([^\""]+)\"",\""rate\"":([0-9\.]+)\}";
                Regex regex = new Regex (regexPattern);
                Match match = regex.Match (rateDataRaw);

                int exchangeRateSnapshotId = 0;
                int bitcoinId = GetOrCreateCurrency ("BTC", "Bitcoin");

                if (match.Success)
                {
                    // We have at least one match, so prepare a new ExchangeRate snapshot

                    exchangeRateSnapshotId = SwarmDb.GetDatabaseForWriting().CreateExchangeRateSnapshot();
                }

                while (match.Success)
                {
                    string currencyCode = match.Groups[1].Value;
                    string currencyName = match.Groups[2].Value;
                    double btcRate = Double.Parse (match.Groups[3].Value, NumberStyles.AllowDecimalPoint,
                        CultureInfo.InvariantCulture);

                    btcRate /= 1000000.0; // We're operating in microbitcoin, so adjust the stored exchange rate accordingly (right-shift six decimal places)

                    int currencyId = GetOrCreateCurrency (currencyCode, currencyName);

                    SwarmDb.GetDatabaseForWriting()
                        .CreateExchangeRateDatapoint (exchangeRateSnapshotId, currencyId, bitcoinId, btcRate);

                    match = match.NextMatch();
                }

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                BitpayRateDatapoint[] rates = (BitpayRateDatapoint[]) serializer.Deserialize<BitpayRateDatapoint[]> (rateDataRaw);
            }
        }

        private static int GetOrCreateCurrency (string currencyCode, string currencyName)
        {
            try
            {
                return Currency.FromCode (currencyCode).Identity;
            }
            catch (ArgumentException)
            {
                return Currency.Create (currencyCode, currencyName, string.Empty).Identity;
            }
        }

    }

    [Serializable]
    public class BitpayRateDatapoint
    {
        string code { get; set; }
        string name { get; set; }
        string rate { get; set; }
    }
}
