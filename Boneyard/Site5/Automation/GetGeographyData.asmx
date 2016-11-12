<%@ WebService Language="C#" Class="GetGeographyData" %>

using System;
using System.Web.Services;
using Swarmops.Logic.Structure;

[WebService(Namespace = "http://swarmops.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class GetGeographyData : System.Web.Services.WebService
{
    [WebMethod]
    public Countries GetCountries()
    {
        return Countries.All;
    }

    [WebMethod]
    public Countries GetCountriesInUse()
    {
        return Countries.GetInUse();
    }

    [WebMethod]
    public Cities GetCitiesForCountry(string countryCode)
    {
        return Cities.ForCountry(countryCode);
    }

    [WebMethod]
    public PostalCodes GetPostalCodesForCountry(string countryCode)
    {
        return PostalCodes.ForCountry(countryCode);
    }

    [WebMethod]
    public Geography GetGeography(int nodeId)
    {
        return Geography.FromIdentity(nodeId);
    }

    [WebMethod]
    public Geography GetGeographyForCountry(string countryCode)
    {
        return Country.FromCode(countryCode).Geography;
    }

    [WebMethod]
    public GeographyUpdates GetGeographyUpdatesSince (DateTime dateTime)
    {
        return GeographyUpdates.GetCreatedSince (dateTime);
    }

    [WebMethod]
    public GeographyUpdates GetGeographyUpdates()
    {
        return GeographyUpdates.GetCreatedSince (new DateTime (1900, 1, 1));
    }
}