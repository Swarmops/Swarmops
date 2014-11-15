using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
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
            this.PageTitle = Resources.Pages.Admin.EditOrganization_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Admin.EditOrganization_Info;
            this.PageAccessRequired = new Access(CurrentOrganization, AccessAspect.Administration, AccessType.Write);

            if (!Page.IsPostBack)
            {
               this.DropMembersChurn.Items.Add(new ListItem(Resources.Global.Global_SelectOne, "0")); 
            }

            // Localize();

            this.EasyUIControlsUsed = EasyUIControl.Tabs;
            this.IncludedControlsUsed = IncludedControl.FileUpload | IncludedControl.SwitchButton;
        }

        [WebMethod]
        public static CallResult FlickSwitch(string switchName, bool switchOn)
        {
            // TODO

            return new CallResult();
        }


        public class CallResult
        {
            public bool Success { get; set; }
            public string OpResult { get; set; }
            public string DisplayMessage { get; set; }
            public string RequiredOn { get; set; }
        }
    }

}