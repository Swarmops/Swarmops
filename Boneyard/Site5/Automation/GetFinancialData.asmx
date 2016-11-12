<%@ WebService Language="C#" Class="GetFinancialData" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Swarmops.Logic.Financial;

[WebService(Namespace = "http://swarmops.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class GetFinancialData  : System.Web.Services.WebService {

    /*
    [WebMethod]
    public ExchangeRateSnapshots GetExhangeRateDataSince(DateTime dateTimeSince)
    {
        return ExchangeRateSnapshots.GetSince (dateTimeSince);
    }

    [WebMethod]
    public double GetExchangeRate (string currencyCodeA, string currencyCodeB)
    {
        return ExchangeRateSnapshot.Latest.GetExchangeRate (currencyCodeA, currencyCodeB);
    }

    [WebMethod]
    public double GetExchangeRate (string currencyCodeA, string currencyCodeB, DateTime onDate)
    {
        return ExchangeRateSnapshot.FromDate (onDate).GetExchangeRate (currencyCodeA, currencyCodeB);
    }*/

    [WebMethod]
    public Currency GetCurrency (string currencyCode)
    {
        return Currency.FromCode (currencyCode);
    }

    public Currencies GetCurrencies()
    {
        return Currencies.GetAll();
    }
}