<%@ WebService Language="C#" Class="Members" %>

using System;
using System.Collections;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

using Activizr.Logic.Pirates;

/// <summary>
/// Summary description for Members
/// </summary>
[WebService(Namespace = "http://pirateweb.net/2008/06")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class Members : System.Web.Services.WebService
{

    public Members()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public int GetMemberCountSinceDate (int organizationId, DateTime sinceDate)
    {
        return Memberships.GetMemberCountForOrganizationSince (organizationId, sinceDate);
    }

}

