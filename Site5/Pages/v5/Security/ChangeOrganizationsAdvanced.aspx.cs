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

            PageTitle = Resources.Pages.Security.ChangeOrganizations_PageTitle;
            PageIcon = "iconshock-organizations";
            InfoBoxLiteral = Resources.Pages.Security.ChangeOrganizations_Info;
            this.LabelCurrentOrganizationName.Text = CurrentOrganization.Name;
        }

        private void Localize()
        {
            this.LabelCurrentOrganization.Text = Resources.Pages.Security.ChangeOrganizations_CurrentOrganization;
            this.LabelNewOrganization.Text = Resources.Pages.Security.ChangeOrganizations_NewOrganization;
        }

        protected void ButtonSwitch_Click(object sender, EventArgs e)
        {
            int newOrganizationId = Int32.Parse(Request.Form["DropOrganizations"]);

            // TODO: Re-authorize user's ability to log onto this org

            FormsAuthentication.RedirectFromLoginPage(
                CurrentUser.Identity.ToString(CultureInfo.InvariantCulture) + "," +
                newOrganizationId.ToString(CultureInfo.InvariantCulture), true);
        }
    }
}