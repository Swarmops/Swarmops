using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
            throw new NotImplementedException();
        }
    }
}