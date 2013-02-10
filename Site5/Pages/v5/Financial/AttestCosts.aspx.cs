using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class AttestCosts : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect("/Pages/v5/Financial/EconomyNotEnabled.aspx", true);
                return;
            }

            this.PageIcon = "iconshock-stamped-paper";

            if (!Page.IsPostBack)
            {
                Localize();
            }
        }


        private void Localize()
        {
            this.PageTitle = Resources.Pages.Financial.AttestCosts_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Financial.AttestCosts_Info;
        }

        [WebMethod]
        public static string Attest (string identifier)
        {
            identifier = HttpUtility.UrlDecode(identifier);

            string identity = HttpContext.Current.User.Identity.Name;
            string[] identityTokens = identity.Split(',');

            string userIdentityString = identityTokens[0];
            string organizationIdentityString = identityTokens[1];

            int currentUserId = Convert.ToInt32(userIdentityString);
            int currentOrganizationId = Convert.ToInt32(organizationIdentityString);

            Person currentUser = Person.FromIdentity(currentUserId);
            Organization currentOrganization = Organization.FromIdentity(currentOrganizationId);

            char costType = identifier[0];
            int itemId = Int32.Parse(identifier.Substring(1));
            string beneficiary = string.Empty;
            string result = string.Empty;

            switch(costType)
            {
                case 'A': // Case advance
                    CashAdvance advance = CashAdvance.FromIdentity(itemId);
                    if (advance.OrganizationId != currentOrganizationId)
                    {
                        throw new InvalidOperationException("Called to attest out-of-org line item");
                    }
                    if (advance.FinancialAccount.OwnerPersonId != currentUserId && advance.FinancialAccount.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityAccessDeniedException("Called without attestation privileges");
                    }

                    result = "XYZ Cash Advance";
                    advance.Attest(currentUser);
                    break;
            }

            return HttpUtility.UrlEncode(result);
        }

        [WebMethod]
        public static string Deattest(string identifier)
        {
            identifier = HttpUtility.UrlDecode(identifier);

            string identity = HttpContext.Current.User.Identity.Name;
            string[] identityTokens = identity.Split(',');

            string userIdentityString = identityTokens[0];
            string organizationIdentityString = identityTokens[1];

            int currentUserId = Convert.ToInt32(userIdentityString);
            int currentOrganizationId = Convert.ToInt32(organizationIdentityString);

            Person currentUser = Person.FromIdentity(currentUserId);
            Organization currentOrganization = Organization.FromIdentity(currentOrganizationId);

            char costType = identifier[0];
            int itemId = Int32.Parse(identifier.Substring(1));
            string beneficiary = string.Empty;
            string result = string.Empty;

            switch (costType)
            {
                case 'A': // Case advance
                    CashAdvance advance = CashAdvance.FromIdentity(itemId);
                    if (advance.OrganizationId != currentOrganizationId)
                    {
                        throw new InvalidOperationException("Called to attest out-of-org line item");
                    }
                    if (advance.FinancialAccount.OwnerPersonId != currentUserId && advance.FinancialAccount.OwnerPersonId != Person.NobodyId)
                    {
                        throw new SecurityAccessDeniedException("Called without attestation privileges");
                    }

                    result = "XYZ Cash Advance for " + beneficiary;
                    advance.Deattest(currentUser);
                    break;
            }

            return HttpUtility.UrlEncode(result).Replace("+", "%20");
        }
    }
}