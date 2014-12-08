using System;
using System.Collections.Generic;
using System.Threading;
using Swarmops.Logic.Financial;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_AvailableCurrencies : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            GetAuthenticationDataAndCulture();

            Response.ContentType = "application/json";

            // Is this stuff in cache already?

            string cacheKey = "Currencies-Json-" + Thread.CurrentThread.CurrentCulture.Name;

            string currenciesJson =
                (string) Cache[cacheKey];

            if (currenciesJson != null)
            {
                Response.Output.WriteLine (currenciesJson);
                Response.End();
                return;
            }

            Currencies currencies = Currencies.GetAll();

            List<string> currencyStrings = new List<string>();

            foreach (Currency currency in currencies)
            {
                currencyStrings.Add (String.Format ("{0} {1}|{0}", currency.Code, currency.Name));
            }

            currencyStrings.Sort();
            currenciesJson = "[";

            foreach (string currencyString in currencyStrings)
            {
                string[] parts = currencyString.Split ('|');

                currenciesJson += "{" + String.Format ("\"label\":\"{0}\",\"code\":\"{1}\"", parts[0], parts[1]) + "},";
            }

            currenciesJson = currenciesJson.TrimEnd (',') + "]";

            Cache.Insert (cacheKey, currenciesJson, null, DateTime.Now.AddMinutes (60), TimeSpan.Zero);
            // cache lasts for sixty minutes, no sliding expiration
            Response.Output.WriteLine (currenciesJson);

            Response.End();
        }
    }
}