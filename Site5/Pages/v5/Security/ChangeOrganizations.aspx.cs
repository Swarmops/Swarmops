using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Services;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Security
{
    public partial class ChangeOrganizations : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageTitle = Resources.Pages_Security.ChangeOrganizations_PageTitle;
            this.PageIcon = "iconshock-organizations";
            this.InfoBoxLiteral = Resources.Pages_Security.ChangeOrganizations_Info;
            this.LabelNoOrganizations.Text = Resources.Pages_Security.ChangeOrganizations_NothingToChange;

            PopulateRepeater();
        }

        private void PopulateRepeater()
        {
            Memberships memberships = CurrentUser.GetMemberships();

            List<OrganizationParameters> availableOrganizations = new List<OrganizationParameters>();
            foreach (Membership membership in memberships)
            {
                if (membership.OrganizationId == 1 && !PilotInstallationIds.IsPilot(PilotInstallationIds.PiratePartySE))
                {
                    // sandbox. Ignore.
                    continue;
                }

                OrganizationParameters newOrganization = new OrganizationParameters();
                newOrganization.LogoUrl = "/Images/Flags/txl-64px.png";
                newOrganization.OrganizationId = membership.OrganizationId;
                newOrganization.OrganizationName = membership.Organization.Name;

                availableOrganizations.Add(newOrganization);
            }

            this.OrganizationCount = availableOrganizations.Count;

            this.RepeaterOrganizations.DataSource = availableOrganizations;
            this.RepeaterOrganizations.DataBind();
        }

        public int OrganizationCount { get; set; }

        private class OrganizationParameters
        {
            public string LogoUrl { get; set; }
            public int OrganizationId { get; set; }
            public string OrganizationName { get; set; }
        }
    }
}