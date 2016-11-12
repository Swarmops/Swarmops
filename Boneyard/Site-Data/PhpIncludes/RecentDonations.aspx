<%@ Page Language="C#" Debug="true" %>
<%@ Import Namespace="Activizr.Logic.Structure"%>
<%@ Import Namespace="Activizr.Logic.Financial"%>
<%@ Import Namespace="System.Globalization" %>

<%@ Import Namespace="Activizr.Interface.Objects" %>
<%
    OrganizationMetadata metadata = OrganizationMetadata.FromUrl(Request.Url.Host);
    Organization org = Organization.FromIdentity(metadata.OrganizationId);

    DateTime start = new DateTime (DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths (-1);
    
    /*
    if (DateTime.Today.Day < 5)
    {
        start = start.AddMonths (-1);
    }*/

    decimal donations = -org.FinancialAccounts.IncomeDonations.GetDelta(start, start.AddMonths(1));

    string variableName = Request.QueryString["VariableName"];

    Response.ContentType = "text/plain";
    Response.Write(start.ToString ("MMMM", CultureInfo.CreateSpecificCulture (Organization.FromIdentity (metadata.OrganizationId).DefaultCountry.Culture)) + ": " + donations.ToString ("N0")); 
%>