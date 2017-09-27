using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Common.Enums;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Automation
{
    public partial class OrgFunctions : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        [WebMethod]
        static public AjaxInputCallResult AdminUploads(string guid, string cookie)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            Documents newDocuments = Documents.RecentFromDescription(guid);
            if (newDocuments.Count < 1)
            {
                throw new ArgumentException(
                    "No docs found in /Automation/OrgFunctions.aspx/AdminUploads using supplied GUID (" + guid + ")");

            }

            switch (cookie)
            {
                case "LogoLandscape":
                    if (newDocuments.Count > 1)
                    {
                        throw new ArgumentException("More than one doc matches");
                    }

                    newDocuments[0].SetDocumentTypeDirect(DocumentType.Logo, authData.CurrentOrganization);
                    authData.CurrentOrganization.LogoLandscape = newDocuments[0];

                    return new AjaxInputCallResult { Success = true, ObjectIdentity = newDocuments[0].Identity };

                default:
                    throw new ArgumentException("Unimplemented cookie -- '" + cookie +
                                                "' -- in /Automation/OrgFunctions.aspx/AdminUploads");
            }
        }
    }
}