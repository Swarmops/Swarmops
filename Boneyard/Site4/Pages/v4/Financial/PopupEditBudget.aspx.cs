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

public partial class Pages_v4_Accounting_PopupEditBudget : PageV4Base
{
    private FinancialAccount _account = null;

    protected void Page_Load (object sender, EventArgs e)
    {
        // Get account

        int financialAccountId = Int32.Parse(Request.QueryString["FinancialAccountId"]);
        _account = FinancialAccount.FromIdentity(financialAccountId);
        int year = DateTime.Today.Year;

        if (!_authority.HasPermission(Permission.CanDoBudget, _account.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            throw new UnauthorizedAccessException("Access Denied");
        }

        this.ComboOwner.Authority = _authority;


        if (!Page.IsPostBack)
        {
            // Populate all data

            this.LabelBudgetYear.Text = year.ToString();
            this.LabelAccount.Text = _account.Name;
            this.LabelOrganization.Text = _account.Organization.Name;
            this.TextBudget.Text = _account.GetBudget(year).ToString(CultureInfo.InvariantCulture);

            if (_account.Owner != null)
            {
                this.ComboOwner.Text = _account.Owner.Name + " (#" + _account.Owner.Identity.ToString() + ")";
            }
        }

        // Page.Title = "Editing Budget: " + _account.Name + ", " + year.ToString();
    }



    protected void ButtonSetBudget_Click(object sender, EventArgs e)
    {
        if (_authority.HasPermission(Permission.CanDoBudget, _account.OrganizationId, -1, Authorization.Flag.ExactOrganization))
        {
            _account.SetBudget(Int32.Parse(this.LabelBudgetYear.Text), Int32.Parse(this.TextBudget.Text));
        }

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);

    }

    protected void ButtonSetOwner_Click(object sender, EventArgs e)
    {
        if (this.ComboOwner.HasSelection)
        {
            if (_authority.HasPermission(Permission.CanDoBudget, _account.OrganizationId, -1, Authorization.Flag.ExactOrganization))
            {
                _account.Owner = this.ComboOwner.SelectedPerson;
            }
        }

        ClientScript.RegisterStartupScript(Page.GetType(), "mykey", "CloseAndRebind();", true);

    }

}