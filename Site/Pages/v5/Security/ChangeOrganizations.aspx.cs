using System;
using System.Collections.Generic;
using System.Globalization;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
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

            PageAccessRequired = new Access (AccessAspect.Null, AccessType.Unknown);

            if (CurrentUser.Identity < 0)
            {
                // We're logged on as open-something

                Response.Redirect ("/");  // back to dashboard
            }

            PopulateRepeater();
        }

        private void PopulateRepeater()
        {
            Participations participations = CurrentUser.GetParticipations();

            List<OrganizationParameters> availableOrganizations = new List<OrganizationParameters>();
            foreach (Participation membership in participations)
            {
                if (membership.OrganizationId == 1 && !PilotInstallationIds.IsPilot (PilotInstallationIds.PiratePartySE))
                {
                    // sandbox. Ignore.
                    continue;
                }

                Organization organization = membership.Organization;
                OrganizationParameters newOrganizationParameters = new OrganizationParameters();

                string logoUrl = "/Images/Other/blank-logo-640x360.png";

                Document logoLandscape = organization.LogoLandscape;

                try
                {
                    if (logoLandscape != null)
                    {
                        newOrganizationParameters.LogoImage = logoLandscape.Image.GetBase64(64, 36);
                    }
                }
                catch (Exception)
                {
                     newOrganizationParameters.LogoImage = "/Images/Flags/txl-64px.png";
                }
                newOrganizationParameters.OrganizationId = membership.OrganizationId;
                newOrganizationParameters.OrganizationName = membership.Organization.Name;

                availableOrganizations.Add (newOrganizationParameters);
            }

            OrganizationCount = availableOrganizations.Count;

            this.RepeaterOrganizations.DataSource = availableOrganizations;
            this.RepeaterOrganizations.DataBind();
        }

        private class OrganizationParameters
        {
            public string LogoImage { get; set; }
            public int OrganizationId { get; set; }
            public string OrganizationName { get; set; }
        }
    }
}