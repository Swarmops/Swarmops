using System;
using Resources;
using Swarmops.Logic;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend
{

    public partial class Default : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            IsDashboard = true;
            PageTitle = Global.Dashboard_PageTitle;
            PageIcon = "iconshock-steering-wheel";
            this.PageAccessRequired = new Access (AccessAspect.Null, AccessType.Read);
                // dummy security until there's something to show on Dashboard

            // BEGIN TEST CODE

            // END TEST CODE

            InfoBoxLiteral =
                "This is a Dashboard placeholder. It will contain a snapshot of the state of things as soon as the basic functions are re-implemented in the new interface.";

            InitializePositionPanel();

            // If Open Ledgers, redirect to Balance Sheet: Don't show Dashboard

            if (CurrentUser.Identity == Person.OpenLedgersIdentity)
            {
                Response.Redirect ("/Ledgers/Balance");
            }
        }

        internal void InitializePositionPanel()
        {
            Geography displayGeography = CurrentUser.Geography;

            if (CurrentAuthority.Position != null && CurrentAuthority.Position.Geography != CurrentUser.Geography)
            {
                if (CurrentAuthority.Position.Geography == null)
                {
                    displayGeography = Geography.Root;
                }
                else
                {
                    displayGeography = CurrentAuthority.Position.Geography;
                }
            }

            if (displayGeography.Identity == Geography.RootIdentity)
            {
                this.LabelHeaderLocal.Text = Resources.Pages.Swarm.LocalOrganization_HeaderGlobal;
            }
            else
            {
                this.LabelHeaderLocal.Text = String.Format(Resources.Pages.Swarm.LocalOrganization_Header, displayGeography.Name);
            }

            this.TreePositions.GeographyId = displayGeography.Identity;
        }
    }
}