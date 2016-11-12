using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Database;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Logic.Security;
using Telerik.Web.UI;

public partial class Pages_v4_Financial_InvoiceAdvanceDebts : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PopulateGrid();
        }
    }


    private void PopulateGrid()
    {
        Dictionary<int, AdvanceDebt> debtLookup = new Dictionary<int, AdvanceDebt>();

        ExpenseClaims claims = ExpenseClaims.ForOrganization(Organization.PPSE);

        foreach (ExpenseClaim claim in claims)
        {
            // If ready for payout, add to list.

            if (claim.Open)
            {
                if (claim.Attested && claim.Validated && !claim.Repaid)
                {
                    // this should be added to the list. Check if we already have some open claims
                    // for this person:

                    if (debtLookup.ContainsKey(claim.ClaimingPersonId))
                    {
                        // Yes. Add claim to list.

                        debtLookup[claim.ClaimingPersonId].DebtCents -= claim.AmountCents;
                        debtLookup[claim.ClaimingPersonId].ClaimIds.Add(claim.Identity);
                    }
                    else
                    {
                        // No. Create a new debt for this person.

                        AdvanceDebt debt = new AdvanceDebt(claim.Claimer, -claim.AmountCents);
                        debt.ClaimIds.Add(claim.Identity);

                        debtLookup[claim.ClaimingPersonId] = debt;
                    }

                    if (debtLookup[claim.ClaimingPersonId].EarliestDate > claim.CreatedDateTime)
                    {
                        debtLookup[claim.ClaimingPersonId].EarliestDate = claim.CreatedDateTime;
                    }
                }
            }
        }

        // Now, we have grouped all ready but unsettled expenses per person. Let's add only those with a positive debt to the final list.

        List<AdvanceDebt> debtList = new List<AdvanceDebt>();

        foreach (int personId in debtLookup.Keys)
        {
            if (debtLookup[personId].DebtCents > 0)
            {
                debtList.Add(debtLookup[personId]);
            }
        }

        debtList.Sort(SortGridItems);

        this.GridDebts.DataSource = debtList;
    }


    private static int SortGridItems (AdvanceDebt debt1, AdvanceDebt debt2)
    {
        if (debt2.DebtCents == debt1.DebtCents)
        {
            return 0;
        }

        return (debt2.DebtCents - debt1.DebtCents > 0? 1: -1);
    }


    protected void GridPayouts_ItemDataBound(object sender, GridItemEventArgs e)
    {
        // Don't care -- use ItemCreated
    }


    protected void GridPayouts_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            
            AdvanceDebt debt = (AdvanceDebt) e.Item.DataItem;

            if (debt == null)
            {
                return;
            }

            /*
            Label labelDueDate = (Label)e.Item.FindControl("LabelDueDate");
            if (payout.ExpectedTransactionDate < DateTime.Now)
            {
                labelDueDate.Text = "ASAP";
            }
            else
            {
                labelDueDate.Text = payout.ExpectedTransactionDate.ToString("yyyy-MM-dd");
            }*/


            /*
            Controls_v4_DocumentList docList = (Controls_v4_DocumentList) e.Item.FindControl("DocumentListClaim");

            if (docList != null)
            {
                docList.Documents = Documents.ForObject(claim);
            }*/

            HyperLink editLink = (HyperLink)e.Item.FindControl("LinkPartialInvoice");
            editLink.Attributes["href"] = "InvoiceAdvanceDebtsPartial.aspx?PersonId=" + debt.Person.Identity + "&OrganizationId=" + Organization.PPSEid;
            /*editLink.Attributes["onclick"] = String.Format("return ShowExpenseClaimForm('{0}','{1}');",
                                                           claim.Identity, e.Item.ItemIndex);*/

        }
    }

    protected void GridPayouts_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        // This is FUCKING REDUNDANT. We already HAVE this data. We shouldn't need to get it AGAIN.

        PopulateGrid();
    }


    protected void RadAjaxManager1_AjaxRequest(object sender, AjaxRequestEventArgs e)
    {
        if (e.Argument == "Rebind")
        {
            //this.GridTransactions.MasterTableView.SortExpressions.Clear();
            //this.GridTransactions.MasterTableView.GroupByExpressions.Clear();
            PopulateGrid();
            this.GridDebts.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            /* This should not happen. */
        }
    }


    protected void ButtonInvoice_Click(object sender, EventArgs e)
    {
        if (_authority.HasPermission(Permission.CanPayOutMoney, Organization.PPSEid, -1, Authorization.Flag.ExactOrganization))
        {

            foreach (string indexString in this.GridDebts.SelectedIndexes)
            {
                // Creating the invoice closes the expense claims.

                int index = Int32.Parse(indexString);
                string protoIdentity = (string) this.GridDebts.MasterTableView.DataKeyValues[index]["ProtoIdentity"];
                string[] identityStrings = protoIdentity.Split(',');

                ExpenseClaim template = ExpenseClaim.FromIdentity(Int32.Parse(identityStrings[0]));

                CultureInfo oldCulture = CultureInfo.CurrentUICulture;
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Organization.PPSE.DefaultCountry.Culture);

                OutboundInvoice invoice = OutboundInvoice.Create(Organization.PPSE, _currentUser,
                                                                 DateTime.Today.AddDays(30),
                                                                 Organization.PPSE.FinancialAccounts.DebtsExpenseClaims,
                                                                 template.Claimer.Name,
                                                                 template.Claimer.Email,
                                                                 template.Claimer.Street + "\r\n" +
                                                                 template.Claimer.PostalCodeAndCity,
                                                                 Organization.PPSE.DefaultCountry.Currency,
                                                                 true, GetLocalResourceObject("InvoiceLiterals.ReclaimedCashAdvance").ToString());

                Thread.CurrentThread.CurrentUICulture = oldCulture;

                foreach (string idString in identityStrings)
                {
                    int claimId = Int32.Parse(idString);
                    ExpenseClaim claim = ExpenseClaim.FromIdentity(claimId);

                    invoice.AddItem("Exp #" + claim.Identity.ToString() + ": " + claim.Description, -claim.AmountCents);
                    claim.Repaid = true;
                    claim.Open = false;
                }

                // Create transaction

                FinancialTransaction transaction = FinancialTransaction.Create(Organization.PPSEid, DateTime.Now,
                                                                               "Outbound Invoice #" + invoice.Identity +
                                                                               ": " + template.ClaimerCanonical);
                transaction.AddRow(Organization.PPSE.FinancialAccounts.DebtsExpenseClaims, -invoice.AmountCents, _currentUser);
                transaction.AddRow(Organization.PPSE.FinancialAccounts.AssetsOutboundInvoices, invoice.AmountCents, _currentUser);

                transaction.Dependency = invoice;

                // Create event

                Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.OutboundInvoiceCreated,
                                                           _currentUser.Identity, 1, 1, 0, invoice.Identity,
                                                           protoIdentity);
            }

            PopulateGrid();
            this.GridDebts.Rebind();
        }

    }

    protected class AdvanceDebt
    {
        public AdvanceDebt(Person person, Int64 debtCents)
        {
            this.person = person;
            this.debtCents = debtCents;
            this.claimIds = new List<int>();
            this.earliestDate = DateTime.MaxValue;
        }

        private readonly Person person;
        private Int64 debtCents;
        private DateTime earliestDate;
        private List<int> claimIds;

        public Person Person
        {
            get { return this.person; }
        }

        public List<int> ClaimIds
        {
            get { return this.claimIds; }
        }

        public DateTime EarliestDate
        {
            get { return this.earliestDate; }
            set { this.earliestDate = value; }
        }

        public Int64 DebtCents
        {
            get { return this.debtCents; }
            set { this.debtCents = value; }
        }

        public decimal DebtDecimal
        {
            get { return DebtCents/100.0m;  }
        }

        public string PersonCanonical
        {
            get { return this.Person.Canonical; }
        }

        public string IdentitiesString
        {
            get { return Formatting.GenerateRangeString(this.ClaimIds); }
        }

        public string ProtoIdentity
        {
            get { return Formatting.JoinIdentities(this.ClaimIds); }
        }
    }
}


