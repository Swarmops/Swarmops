using System;
using System.Globalization;
using System.Web;
using System.Web.Security;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend.Pages.v5.Security
{
    public partial class SetCurrentOrganization : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            if (CurrentUser == null)
            {
                throw new UnauthorizedAccessException ("No"); // may cause problems on login screen?
            }

            PageAccessRequired = new Access (AccessAspect.Null);

            string returnUrlString = Request.QueryString["ReturnUrl"];
            string organizationIdString = Request.QueryString["OrganizationId"];
            int organizationId = Int32.Parse (organizationIdString);
            Organization suggestedOrganization = null;
            try
            {
                suggestedOrganization = Organization.FromIdentity (organizationId);
            }
            catch (ArgumentException)
            {
                suggestedOrganization = null;
            }

            if (suggestedOrganization == null || !CurrentUser.MemberOf (suggestedOrganization))
            {
                // Some work here on PPSE pilot - we want everybody to be able to switch to Sandbox, which is #1
                // except for in PPSE installation, where it is... #3 or something

                throw new UnauthorizedAccessException();
            }

            // this MUST be replaced with CommonV5 function
            throw new NotImplementedException();



            string logonFlags = string.Empty;
            string[] currentIdentityParts = HttpContext.Current.User.Identity.Name.Split (',');

            if (currentIdentityParts.Length > 2)
            {
                logonFlags = currentIdentityParts[3];
            }

            string userIdentityString = CurrentUser.Identity.ToString (CultureInfo.InvariantCulture) + "," +
                                        suggestedOrganization.Identity.ToString (CultureInfo.InvariantCulture) + ",," +
                                        logonFlags;

            CurrentUser.LastLogonOrganizationId = suggestedOrganization.Identity;

            if (!string.IsNullOrEmpty (returnUrlString))
            {
                FormsAuthentication.SetAuthCookie (userIdentityString, true);
                Response.Redirect (returnUrlString);
            }
            else
            {
                FormsAuthentication.RedirectFromLoginPage (userIdentityString, true);
            }
        }
    }
}