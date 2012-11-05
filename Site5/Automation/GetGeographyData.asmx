<%@ WebService Language="C#" Class="GetGeographyData" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Activizr.Logic.Structure;

[WebService(Namespace = "http://activizr.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class GetGeographyData  : System.Web.Services.WebService {

    [WebMethod]
    public Countries GetCountries()
    {
        return Countries.GetAll();
    }
    
    [WebMethod]
    public Countries GetCountriesInUse() {
        return Countries.GetInUse();
    }
    
    [WebMethod]
    public Cities GetCitiesForCountry (string countryCode)
    {
        return Cities.ForCountry(countryCode);
    }
    
    [WebMethod]
    public PostalCodes GetPostalCodesForCountry (string countryCode)
    {
        return PostalCodes.ForCountry(countryCode);
    }
    
    [WebMethod]
    public Geographies GetGeographiesFromRoot (int rootId)
    {
        return Geography.FromIdentity(rootId).GetTree();
    }
    
    [WebMethod]
    public Geographies GetGeographiesForCountry (string countryCode)
    {
        Country country = Country.FromCode(countryCode);
        return GetGeographiesFromRoot(country.GeographyId);
    }
    
    /*
    
        [WebMethod]
    public BasicGeography[] GetGeographiesFromRoot (int rootId)
    {
        return PirateDb.GetDatabase().GetGeographyTree(rootId);
    }
    
    [WebMethod]
    public BasicGeography[] GetGeographiesForCountry (string countryCode)
    {
        Country country = Country.FromCode(countryCode);
        return PirateDb.GetDatabase().GetGeographyTree(country.GeographyId);
    }

     */
}