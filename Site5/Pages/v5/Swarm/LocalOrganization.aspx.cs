using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Logic;
using Swarmops.Logic.Support;
using Swarmops.Logic.Security;
using System.Globalization;
using System.Web.Services;
using System.Text.RegularExpressions;
using Swarmops.Frontend.Controls.v5.Base;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Swarm
{
    public partial class LocalOrganization : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access (this.CurrentOrganization, AccessAspect.Participant);
            this.PageTitle = Resources.Pages.Swarm.LocalOrganization_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Swarm.LocalOrganization_Info;

            if (!Page.IsPostBack)
            {
                Localize();
            }

            RegisterControl (EasyUIControl.Tabs);
            RegisterControl (IncludedControl.JsonParameters | IncludedControl.SwitchButton);

            Geography displayGeography = CurrentUser.Geography;

            if (CurrentAuthority.Position.Geography != CurrentUser.Geography)
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




        private void Localize()
        {
        }



    }

}