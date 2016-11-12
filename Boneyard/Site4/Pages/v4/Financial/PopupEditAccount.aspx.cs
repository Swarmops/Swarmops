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
using Activizr.Basic.Types;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

using Telerik.Web.UI;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;

public partial class Pages_v4_Accounting_PopupEditAccount : PageV4Base
{
    private FinancialAccount _account = null;

    protected void Page_Load (object sender, EventArgs e)
    {
        // Get account

        int financialAccountId = Int32.Parse(Request.QueryString["FinancialAccountId"]);
        _account = FinancialAccount.FromIdentity(financialAccountId);
        int year = DateTime.Today.Year;

        if (!_authority.HasPermission(Permission.CanDoEconomyTransactions, _account.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            throw new UnauthorizedAccessException("Access Denied");
        }

        if (!Page.IsPostBack)
        {
            // Populate all data

            this.LabelAccount.Text = _account.Name;
            this.LabelOrganization.Text = _account.Organization.Name;
            this.TextTransactionTag.Text = _account.BankTransactionTag;

            if (_account.AssignedGeography != null)
            {
                this.DropGeographies.SelectedGeography = _account.AssignedGeography;
            }
        }

        // Page.Title = "Editing Budget: " + _account.Name + ", " + year.ToString();
    }



    protected void ButtonSetTag_Click(object sender, EventArgs e)
    {
        if (_authority.HasPermission(Permission.CanDoEconomyTransactions, _account.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            _account.BankTransactionTag = this.TextTransactionTag.Text;
        }

        // ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);

    }

    protected void ButtonSetGeography_Click(object sender, EventArgs e)
    {
        if (this.DropGeographies.SelectedGeography != null)
        {
            if (_authority.HasPermission(Permission.CanDoEconomyTransactions, _account.OrganizationId, -1, Authorization.Flag.ExactOrganization))
            {
                _account.AssignedGeography = this.DropGeographies.SelectedGeography;
            }
        }

        // ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);

    }

}