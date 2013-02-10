﻿using System;
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
            this.LabelAttestCostsHeader.Text = Resources.Pages.Financial.AttestCosts_Header_CostsAwaitingAttestation;
            this.LabelGridHeaderAction.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Action;
            this.LabelGridHeaderBeneficiary.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Beneficiary;
            this.LabelGridHeaderBudget.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Budget;
            this.LabelGridHeaderDescription.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Description;
            this.LabelGridHeaderDocs.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Docs;
            this.LabelGridHeaderItem.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Item;
            this.LabelGridHeaderRequested.Text = Resources.Pages.Financial.AttestCosts_GridHeader_Requested;
        }

        private enum AttestationMode
        {
            Unknown = 0,
            Attestation,
            Deattestation
        };

        [WebMethod]
        public static string Attest (string identifier)
        {
            identifier = HttpUtility.UrlDecode(identifier);

            string result = HandleAttestationDeattestation(identifier, AttestationMode.Attestation);

            return HttpUtility.UrlEncode(result).Replace("+", "%20");
        }

        [WebMethod]
        public static string Deattest(string identifier)
        {
            identifier = HttpUtility.UrlDecode(identifier);

            string result = HandleAttestationDeattestation(identifier, AttestationMode.Deattestation);

            return HttpUtility.UrlEncode(result).Replace("+", "%20");
        }


        private static string HandleAttestationDeattestation (string identifier, AttestationMode mode)
        {
            // Find various credentials

            string identity = HttpContext.Current.User.Identity.Name;
            string[] identityTokens = identity.Split(',');

            string userIdentityString = identityTokens[0];
            string organizationIdentityString = identityTokens[1];

            int currentUserId = Convert.ToInt32(userIdentityString);
            int currentOrganizationId = Convert.ToInt32(organizationIdentityString);

            Person currentUser = Person.FromIdentity(currentUserId);
            Organization currentOrganization = Organization.FromIdentity(currentOrganizationId);

            IAttestable attestableItem = null;
            string attestedTemplate = string.Empty;
            string deattestedTemplate = string.Empty;

            char costType = identifier[0];
            int itemId = Int32.Parse(identifier.Substring(1));
            Int64 amountCents;
            string beneficiary = string.Empty;
            string result = string.Empty;

            // Find the item we are attesting or deattesting

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

                    attestableItem = advance;
                    attestedTemplate = Resources.Pages.Financial.AttestCosts_AdvanceAttested;
                    deattestedTemplate = Resources.Pages.Financial.AttestCosts_AdvanceDeattested;
                    beneficiary = advance.Person.Name;
                    amountCents = advance.AmountCents;

                    break;
                default:
                    throw new InvalidOperationException("Unknown Cost Type in HandleAttestationDeattestation: \"" + identifier + "\"");
            }

            // Finally, attest or deattest

            if (mode == AttestationMode.Attestation)
            {
                attestableItem.Attest(currentUser);
                result = string.Format(attestedTemplate, itemId, beneficiary, currentOrganization.Currency.Code,
                                       amountCents/100.0);
            }
            else if (mode == AttestationMode.Deattestation)
            {
                attestableItem.Deattest(currentUser);
                result = string.Format(deattestedTemplate, itemId, beneficiary, currentOrganization.Currency.Code,
                                       amountCents / 100.0);
            }
            else
            {
                throw new InvalidOperationException("Unknown Attestation Mode: " + mode.ToString());
            }

            return result;
        }

    }
}