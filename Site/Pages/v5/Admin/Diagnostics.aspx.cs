using System;
using System.Collections.Generic;
using System.Security;
using System.Web;
using System.Web.Services;
using Resources;

using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;
using Swarmops.Logic.UITests;

namespace Swarmops.Frontend.Pages.Admin
{
    public partial class Diagnostics : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            this.PageGuid = Guid.NewGuid().ToString();

            PageIcon = "iconshock-group-search";
            this.PageAccessRequired = new Access (CurrentOrganization, AccessAspect.PersonalData, AccessType.Read);

            if (!Page.IsPostBack)
            {
                Localize();
            }

            RegisterControl (EasyUIControl.Tree | EasyUIControl.DataGrid);

            // Add all test groups

            List<IUITestGroup> testGroups = new List<IUITestGroup>();

            testGroups.Add(new SocketTests());

            // Create the document.ready() function body

            string javascriptDocReady = string.Empty;

            foreach (IUITestGroup testGroup in testGroups)
            {
                javascriptDocReady += testGroup.JavaScriptClientCodeDocReady;
            }

        }

        private void Localize()
        {
            PageTitle = @"Troubleshooting";
            InfoBoxLiteral = @"This runs a series of self-tests to check that the installation is working correctly. If one or more tests fail, or just don't pass, this helps you diagnose precisely what is needed to correct the problem.";

            /* no localization for debug pages */
        }


 

        public string JavascriptDocReady { get; set; }

        public string PageGuid { get; set; }

    }
}