using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic;
using Swarmops.Logic.Support;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Admin
{
    public partial class Settings : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // HACK: The organization part must be removed once proper access control is in place
            this.PageAccessRequired = new Access(this.CurrentOrganization, AccessAspect.System, AccessType.Write);
            this.IncludedControlsUsed = IncludedControl.JsonParameters | IncludedControl.SwitchButton;
            this.EasyUIControlsUsed = EasyUIControl.Tabs;

            if (!Page.IsPostBack)
            {
                this.TextSmtpServer.Text = Persistence.Key["SmtpServer"];
                string smtpPort = Persistence.Key["SmtpPort"];
                if (smtpPort != "25")
                {
                    this.TextSmtpServer.Text += ":" + smtpPort;
                }
            }
        }
    }
}