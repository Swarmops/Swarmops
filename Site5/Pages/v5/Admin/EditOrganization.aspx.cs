using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Admin
{

    public partial class EditOrganization : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageIcon = "iconshock-box-cog";
            this.PageTitle = Resources.Pages_Admin.EditOrganization_PageTitle;
            this.InfoBoxLiteral = Resources.Pages_Admin.EditOrganization_Info;
            this.PageAccessRequired = new Access(CurrentOrganization, AccessAspect.Administration, AccessType.Write);

            this.EasyUIControlsUsed = EasyUIControl.Tabs;



        }
    }

}