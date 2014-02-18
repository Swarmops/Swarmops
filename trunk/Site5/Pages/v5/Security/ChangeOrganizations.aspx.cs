using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.Security
{
    public partial class ChangeOrganizations : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Localize();
            }

            this.PageAccessRequired = new Access(AccessAspect.Bookkeeping, AccessType.Write);  // bogus, but will prevent bad ppl from entering until real security done

            this.PageTitle = Resources.Pages.Security.ChangeOrganizations_PageTitle;
            this.PageIcon = "iconshock-organizations";
            this.InfoBoxLiteral = Resources.Pages.Security.ChangeOrganizations_Info;
            this.LabelCurrentOrganizationName.Text = CurrentOrganization.Name;
        }

        private void Localize()
        {
            this.LabelCurrentOrganization.Text = Resources.Pages.Security.ChangeOrganizations_CurrentOrganization;
            this.LabelNewOrganization.Text = Resources.Pages.Security.ChangeOrganizations_NewOrganization;
        }

        protected void ButtonSwitch_Click(object sender, EventArgs e)
        {
            int newOrganizationId = Int32.Parse(this.Request.Form["DropOrganizations"]);

            // TODO: Re-authorize user's ability to log onto this org

            FormsAuthentication.RedirectFromLoginPage(this.CurrentUser.Identity.ToString(CultureInfo.InvariantCulture) + "," + newOrganizationId.ToString(CultureInfo.InvariantCulture), true);
        }
    }
}