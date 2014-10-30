using System;
using System.Globalization;
using System.Web.Security;

namespace Swarmops.Frontend.Pages.Security
{
    public partial class ChangeOrganizationsAdvanced : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Localize();
            }

            // this.PageAccessRequired = new Access(AccessAspect.Bookkeeping, AccessType.Write);  // bogus, but will prevent bad ppl from entering until real security done

            this.PageTitle = Resources.Pages_Security.ChangeOrganizations_PageTitle;
            this.PageIcon = "iconshock-organizations";
            this.InfoBoxLiteral = Resources.Pages_Security.ChangeOrganizations_Info;
            this.LabelCurrentOrganizationName.Text = CurrentOrganization.Name;
        }

        private void Localize()
        {
            this.LabelCurrentOrganization.Text = Resources.Pages_Security.ChangeOrganizations_CurrentOrganization;
            this.LabelNewOrganization.Text = Resources.Pages_Security.ChangeOrganizations_NewOrganization;
        }

        protected void ButtonSwitch_Click(object sender, EventArgs e)
        {
            int newOrganizationId = Int32.Parse(this.Request.Form["DropOrganizations"]);

            // TODO: Re-authorize user's ability to log onto this org

            FormsAuthentication.RedirectFromLoginPage(this.CurrentUser.Identity.ToString(CultureInfo.InvariantCulture) + "," + newOrganizationId.ToString(CultureInfo.InvariantCulture), true);
        }
    }
}