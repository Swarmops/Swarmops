using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Financial;
using Activizr.Logic.Structure;
using Telerik.Web.UI;

public partial class Pages_v4_Accounting_ViewEditPayroll : PageV4Base
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
        Payroll payroll = Payroll.ForOrganization(Organization.FromIdentity(Organization.PPSEid));

        // allClaims.Sort(SortGridItems);

        this.GridPayroll.DataSource = payroll;
    }


    private static int SortGridItems (ExpenseClaim claim1, ExpenseClaim claim2)
    {
        return DateTime.Compare(claim2.CreatedDateTime, claim1.CreatedDateTime);
    }


    protected void GridPayroll_ItemDataBound(object sender, GridItemEventArgs e)
    {
        // Don't care -- use ItemCreated
    }


    protected void GridPayroll_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            PayrollItem payrollItem = (PayrollItem) e.Item.DataItem;

            if (payrollItem == null)
            {
                return;
            }

            Label labelBudget = (Label)e.Item.FindControl("LabelBudget");
            Label labelReportsTo = (Label)e.Item.FindControl("LabelReportsTo");
            Label labelMonthlyCost = (Label)e.Item.FindControl("LabelMonthlyCost");

            if (payrollItem.BudgetId == 0)
            {
                labelBudget.Text = "UNBUDGETED!";
            }
            else
            {
                labelBudget.Text = payrollItem.Budget.Name;
            }

            labelReportsTo.Text = payrollItem.ReportsToPerson.Canonical;
            labelMonthlyCost.Text = String.Format("{0:N2}", (payrollItem.BaseSalaryCents/100.0)*(1.0 + payrollItem.AdditiveTaxLevel));

            
            HyperLink editLink = (HyperLink)e.Item.FindControl("LinkEdit");
            editLink.Attributes["href"] = "#";
            editLink.Attributes["onclick"] = String.Format("return ShowPayrollAdjustmentsForm('{0}','{1}');",
                                                           payrollItem.Identity, e.Item.ItemIndex);






        }
    }

    protected void GridPayroll_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
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
            this.GridPayroll.Rebind();
        }
        else if (e.Argument == "RebindAndNavigate")
        {
            /* This should not happen. */
        }
    }

}


