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

            if (suggestedOrganization == null || !CurrentUser.ParticipatesInOrganization (suggestedOrganization))
            {
                // Some work here on PPSE pilot - we want everybody to be able to switch to Sandbox, which is #1
                // except for in PPSE installation, where it is... #3 or something

                // TODO: Allow logon to organization if there is a Position active

                throw new UnauthorizedAccessException();
            }

            // when we get here, we are authorized to log on to the suggested organization.

            // The reason we're modifying the existing Authority object instead of creating a new one
            // is to minimize the risk for impersonation exploits: The only way to assign a Person identity
            // to an Authority object is on login.

            Authority newAuthority = CurrentAuthority;
            newAuthority.SetOrganization (suggestedOrganization); // will/can also modify Position
            CurrentUser.LastLogonOrganizationId = suggestedOrganization.Identity;

            if (!string.IsNullOrEmpty (returnUrlString))
            {
                FormsAuthentication.SetAuthCookie (newAuthority.ToEncryptedXml(), true);
                Response.Redirect (returnUrlString);
            }
            else
            {
                FormsAuthentication.RedirectFromLoginPage (newAuthority.ToEncryptedXml(), true);
            }
        }
    }
}