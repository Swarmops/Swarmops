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

using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Interface.Localization;
using Activizr.Interface.Collections;
using Activizr.Interface.Objects;
using Activizr.Logic.Structure;


public partial class Pages_Financials_ExpensePayouts : PageV4Base
{

	protected void GridPayouts_RowCommand(object sender, GridViewCommandEventArgs e)
	{
		switch (e.CommandName)
		{
			case "Repaid":
				int currentUserId = Convert.ToInt32 (HttpContext.Current.User.Identity.Name);
				int index = Convert.ToInt32(e.CommandArgument);

				// WARNING - THIS CODE CREATES A RACE CONDITION - HACK - BUG - UGLY
				// new payout approvals may have appeared between user display and this execution,
				// which rebinds the data to execute on

				int personId = Convert.ToInt32(this.GridPayouts.DataKeys[index].Value);
                ExpensePayout payout = ExpensePayout.FromOrganizationAndPerson(Organization.PPSEid, personId);

				foreach (ExpenseClaim expense in payout.ExpenseClaims)
				{
					expense.CreateEvent(ExpenseEventType.RepaidClosed, currentUserId);
					expense.Close();
				}

				Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.ExpensesRepaidClosed, currentUserId, payout.ExpenseClaims [0].OrganizationId, payout.ExpenseClaims[0].GeographyId, personId, 0, payout.ReceiptIdentitiesString);
				this.GridPayouts.DataBind();
				break;
		}
	}
}
