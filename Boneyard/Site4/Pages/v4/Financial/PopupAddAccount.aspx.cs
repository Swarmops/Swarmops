using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

using Telerik.Web.UI;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;

public partial class Pages_v4_PopupAddAccount : PageV4Base
{
    int _organizationId = 0;

    protected void Page_Load (object sender, EventArgs e)
    {
        _organizationId = Int32.Parse(Request.QueryString["OrganizationId"]);

        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, _organizationId, -1, Authorization.Flag.ExactOrganization))
            throw new UnauthorizedAccessException("Access Denied");


        if (!Page.IsPostBack)
        {
            // Populate all data
            PopulateAccountTypes();
            PopulateAccounts(_organizationId);
        }


        Page.Title = "Add Account";
    }

    private void PopulateAccountTypes ()
    {
        this.DropAccountType.Items.Clear();
        for (int i = 1; i < 5; ++i)
        {
            FinancialAccountType t = ((FinancialAccountType[])Enum.GetValues(typeof(FinancialAccountType)))[i];
            this.DropAccountType.Items.Add(new ListItem(t.ToString(), t.ToString()));
        }
    }

    private void PopulateAccounts (int organizationId)
    {
        FinancialAccounts accounts = FinancialAccounts.ForOrganization(Organization.FromIdentity(organizationId));

        this.DropAccounts.Items.Add(new ListItem("-- None --", "0"));
        foreach (FinancialAccount account in accounts)
        {
            this.DropAccounts.Items.Add(new ListItem("[" + account.AccountType.ToString().Substring(0, 1) + "] " + account.Name, account.Identity.ToString()));
        }
    }

    protected void ButtonClose_Click (object sender, EventArgs e)
    {

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);
    }

    protected void ButtonAdd_Click (object sender, EventArgs e)
    {

        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, _organizationId, -1, Authorization.Flag.ExactOrganization))
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "getlost",
                                                "alert ('You do not have access to change financial records.');", true);
            return;
        }
        FinancialAccountType pAccountType=(FinancialAccountType)Enum.Parse(typeof(FinancialAccountType),this.DropAccountType.SelectedValue);
        int pParentFinancialAccountId=Int32.Parse(this.DropAccounts.SelectedValue);
        FinancialAccount.Create(_organizationId, TextAccountName.Text, pAccountType, pParentFinancialAccountId);

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);
    }



}