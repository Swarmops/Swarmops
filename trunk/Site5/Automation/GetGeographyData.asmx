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
    
    
    
}