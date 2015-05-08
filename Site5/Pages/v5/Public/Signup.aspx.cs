using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend.Pages.Public
{
    public partial class Signup : System.Web.UI.Page
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            this.Organization = Organization.FromVanityDomain (Request.Url.Host);

            if (this.Organization == null)
            {
                int organizationId = Convert.ToInt32 (Request["OrganizationId"]); // may throw - not pretty but ok for now. // TODO
                this.Organization = Organization.FromIdentity (organizationId); // may also throw, as above
            }

            if (this.Organization == null)
            {
                throw new ArgumentException("Organization not specified");  // TODO: make a friendly landing page instead
            }
        }

        protected Organization Organization;
    }
}