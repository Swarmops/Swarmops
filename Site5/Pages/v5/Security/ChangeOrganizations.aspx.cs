using System;
using System.Collections.Generic;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Security
{
    public partial class ChangeOrganizations : PageV5Base
    {
        public int OrganizationCount { get; set; }

        protected void Page_Load (object sender, EventArgs e)
        {
            PageTitle = Resources.Pages.Security.ChangeOrganizations_PageTitle;
            PageIcon = "iconshock-organizations";
            InfoBoxLiteral = Resources.Pages.Security.ChangeOrganizations_Info;
            this.LabelNoOrganizations.Text = Resources.Pages.Security.ChangeOrganizations_NothingToChange;

            PopulateRepeater();
        }

        private void PopulateRepeater()
        {
            Memberships memberships = CurrentUser.GetMemberships();

            List<OrganizationParameters> availableOrganizations = new List<OrganizationParameters>();
            foreach (Membership membership in memberships)
            {
                if (membership.OrganizationId == 1 && !PilotInstallationIds.IsPilot (PilotInstallationIds.PiratePartySE))
                {
                    // sandbox. Ignore.
                    continue;
                }

                OrganizationParameters newOrganization = new OrganizationParameters();
                newOrganization.LogoUrl = "/Images/Flags/txl-64px.png";
                newOrganization.OrganizationId = membership.OrganizationId;
                newOrganization.OrganizationName = membership.Organization.Name;

                availableOrganizations.Add (newOrganization);
            }

            OrganizationCount = availableOrganizations.Count;

            this.RepeaterOrganizations.DataSource = availableOrganizations;
            this.RepeaterOrganizations.DataBind();
        }

        private class OrganizationParameters
        {
            public string LogoUrl { get; set; }
            public int OrganizationId { get; set; }
            public string OrganizationName { get; set; }
        }
    }
}