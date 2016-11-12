using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Telerik.Web.UI;

public partial class Pages_v4_Financial_AuditGeneralLedger : PageV4Base
{
    private string _sessionObjectName;

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            // Populate dropdowns, etc

            this.DropAccountGroups.Style[HtmlTextWriterStyle.Width] = "300px";

            foreach (FinancialAccountType type in Enum.GetValues(typeof(FinancialAccountType)))
            {
                if (type == FinancialAccountType.Unknown)
                {
                    continue;
                }

                this.DropAccountGroups.Items.Add(new ListItem(type.ToString(), ((int) type).ToString()));
            }

            this.DropAccounts.Populate(Organization.PPSE, FinancialAccountType.Unknown);
            this.DateStart.SelectedDate = new DateTime(Organization.PPSE.Parameters.FiscalBooksAuditedUntilYear+1, 1, 1);
            this.DateEnd.SelectedDate = new DateTime(Organization.PPSE.Parameters.FiscalBooksAuditedUntilYear + 1, 12, 31);

            if (Request.QueryString["Preset"] == "AssetCredits")
            {
                this.RadioAccountGroup.Checked = true;
                this.DropAccountGroups.SelectedValue = ((int) FinancialAccountType.Asset).ToString();
                this.RadioCreditingTransactions.Checked = true;
            }
            else
            {
                this.RadioAllAccounts.Checked = true;
                this.RadioAllTransactions.Checked = true;
            }

        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PopulateLedger();
        }
    }


    private void PopulateLedger()
    {
        FinancialAccounts accounts = new FinancialAccounts();

        if (this.RadioAllAccounts.Checked)
        {
            accounts =
                FinancialAccounts.ForOrganization(
                    Organization.FromIdentity(Int32.Parse(this.DropOrganizations.SelectedValue)));
        }
        else if (this.RadioAccountGroup.Checked)
        {
            accounts =
                FinancialAccounts.ForOrganization(
                    Organization.FromIdentity(Int32.Parse(this.DropOrganizations.SelectedValue)), (FinancialAccountType) Int32.Parse(this.DropAccountGroups.SelectedValue));
        }
        else if (this.RadioSpecificAccount.Checked)
        {
            accounts.Add(this.DropAccounts.SelectedFinancialAccount);
        }

        Ledger.Accounts = accounts;

        Ledger.DateStart = (DateTime)this.DateStart.SelectedDate;
        Ledger.DateEnd = (DateTime)this.DateEnd.SelectedDate;

        Ledger.MaxAmount = 1.0e12m; // catches all transactions -- if not, PW is kind of outscaled

        if (this.RadioCreditingTransactions.Checked)
        {
            Ledger.MaxAmount = 0.0m;
        }
        else if (this.RadioCreditingTransactionsLarge.Checked)
        {
            Ledger.MaxAmount = -1000.0m;
        }

        Ledger.Populate();
    }


    protected void ButtonFilter_Click(object sender, EventArgs e)
    {
        PopulateLedger();
    }
}
