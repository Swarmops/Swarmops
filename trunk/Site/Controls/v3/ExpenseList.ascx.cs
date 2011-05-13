using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Logic.Financial;
using Activizr.Basic.Enums;

public partial class Controls_ExpenseList : System.Web.UI.UserControl
{
	protected void Page_Load(object sender, EventArgs e)
	{

	}
	protected void ExpenseDataSource_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
	{
		if (PersonId != 0)
		{
			e.InputParameters["personId"] = PersonId;
			this.ExpenseDataSource.SelectMethod = "SelectOpenByClaimer";
			GridExpenses.Columns[GridExpenses.Columns.Count - 1].Visible = false;
			GridExpenses.Columns[2].Visible = false;
			GridExpenses.PageSize = 7;

		}
		else
		{
			e.InputParameters["organizationId"] = OrganizationId;
			this.ExpenseDataSource.SelectMethod = "SelectUnapprovedByOrganization";
			GridExpenses.PageSize = 25;
		}
	}


	protected void GridExpenses_RowCommand(object sender, GridViewCommandEventArgs e)
	{
		switch (e.CommandName)
		{
			case "Approve":
				int currentUserId = Convert.ToInt32 (HttpContext.Current.User.Identity.Name);
				int index = Convert.ToInt32(e.CommandArgument);
				int expenseId = Convert.ToInt32(this.GridExpenses.DataKeys[index].Value);
				ExpenseClaim expenseClaim = ExpenseClaim.FromIdentity(expenseId);

				expenseClaim.CreateEvent(ExpenseEventType.Approved, currentUserId);
				Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.ExpenseChanged, currentUserId, expenseClaim.OrganizationId, expenseClaim.GeographyId, expenseClaim.ClaimingPersonId, expenseClaim.Identity, ExpenseEventType.Approved.ToString());
				Repopulate();
				break;
		}
	}


	public void Repopulate()
	{
		this.GridExpenses.DataBind();
	}

	public int PersonId;
	public int OrganizationId;
}


