using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Controls.Swarm
{
    public partial class ImpersonationWarningBox : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PanelImpersonationWarning.Visible = CurrentAuthority.ImpersonationActive;
            Localize();
        }

        private void Localize()
        {
            this.LabelImpersonationWarningHeader.Text = Resources.Pages.Admin.CommenceImpersonation_Active_Header;
            this.LiteralImpersonationWarningText.Text = String.Format(Resources.Pages.Admin.CommenceImpersonation_Active_Text, CurrentUser.Canonical);
        }
    }
}