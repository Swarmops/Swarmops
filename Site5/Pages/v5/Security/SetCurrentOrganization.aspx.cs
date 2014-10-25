using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend.Pages.v5.Security
{
    public partial class SetCurrentOrganization : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CurrentUser == null)
            {
                throw new UnauthorizedAccessException("No"); // may cause problems on login screen?
            }

            string organizationIdString = Request.QueryString["OrganizationId"];
            int organizationId = Int32.Parse(organizationIdString);
            Organization suggestedOrganization = null;
            try
            {
                suggestedOrganization = Organization.FromIdentity(organizationId);
            }
            catch (ArgumentException)
            {
                suggestedOrganization = null;
            }

            if (suggestedOrganization == null || !CurrentUser.MemberOf(suggestedOrganization))
            {
                // Some work here on PPSE pilot - we want everybody to be able to switch to Sandbox, which is #1
                // except for in PPSE installation, where it is... #3 or something

                throw new UnauthorizedAccessException();
            }

            FormsAuthentication.RedirectFromLoginPage(
                CurrentUser.Identity.ToString(CultureInfo.InvariantCulture) + "," +
                suggestedOrganization.Identity.ToString(CultureInfo.InvariantCulture), true); // we're not using the original input for security reasons

        }
    }
}