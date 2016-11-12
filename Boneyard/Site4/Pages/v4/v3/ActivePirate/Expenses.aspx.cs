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
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

public partial class Pages_ActivePirate_Expenses : PageV4Base
{
	protected void Page_Load(object sender, EventArgs e)
	{
		int currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
		Person currentUser = Person.FromIdentity (currentUserId);

		this.ListOpenExpenses.PersonId = currentUserId;

		// Get currently expensed amount

		ExpenseClaims expenseClaims = ExpenseClaims.FromClaimingPerson(currentUser);
        
        decimal totalOpen = 0.0m;

		foreach (ExpenseClaim expense in expenseClaims)
		{
			if (expense.Open)
			{
				totalOpen += expense.Amount;
			}
		}

		if (totalOpen > 0)
		{
			this.LabelOpenExpensesParagraph.Text = String.Format(LocalizationManager.GetLocalString("Interface.Pages.ActivePirate.Expenses.Open.IntroParagraph",
				"This is a list of your filed but still open expenses, currently totalling {0:N2}. They will be removed from this list when the treasurer has received your receipts and reimbursed the amount."),
				totalOpen);
		}
		else
		{
			this.LabelOpenExpensesParagraph.Text = LocalizationManager.GetLocalString("Interface.Pages.ActivePirate.Expenses.Open.IntroParagraph",
				"When you have open expenses, this section lists them.");
		}

		if (!Page.IsPostBack)
		{
			// Init

			this.TextExpenseBy.Text = currentUser.Name;

			// Populate list of organizations (initial population)

			Organizations organizationList = currentUser.GetAuthority().GetOrganizations(RoleTypes.AllRoleTypes);
			organizationList = organizationList.RemoveRedundant();
			organizationList.Add(Organization.Sandbox);

			foreach (Organization organization in organizationList)
			{
				Organizations organizationTree = organization.GetTree();

				foreach (Organization organizationOption in organizationTree)
				{
					DropOrganizations.Items.Add(new ListItem(organizationOption.NameShort, organizationOption.OrganizationId.ToString()));
				}
			}

			PopulateGeographies();

			// Localize

			this.LabelNewExpenseParagraph.Text = LocalizationManager.GetLocalString("Interface.Pages.ActivePirate.Expenses.New.IntroParagraph",
                "To file an expense:<ul><li>Take the receipt and affix it to an A4 size paper. </li><li>Select the organization and geography that should be charged</li><li>Write the receipt date and a good short description</li><li>Press Claim Expense. </li><li>You will be asked to mark the receipt with a number and send it to the treasurer.<br><b>Note the number and the address, they will only be shown at this time!</B> </li></ul>Don't forget to specify your bank account details (in your member profile).");

			this.LabelExpenseBy.Text = LocalizationManager.GetLocalString("Interface.Pages.ActivePirate.Expenses.New.ExpensingPersonName", "Expensed By") + "&nbsp;&nbsp;";
			this.LabelOrganization.Text = LocalizationManager.GetLocalString("Interface.Pages.ActivePirate.Expenses.New.Organization", "Organization") + "&nbsp;&nbsp;";
			this.LabelGeography.Text = LocalizationManager.GetLocalString("Interface.Pages.ActivePirate.Expenses.New.Geography", "Geography") + "&nbsp;&nbsp;";
			this.LabelDate.Text = LocalizationManager.GetLocalString("Interface.Pages.ActivePirate.Expenses.New.Date", "Expense Date") + "&nbsp;&nbsp;";
			this.LabelDescription.Text = LocalizationManager.GetLocalString("Interface.Pages.ActivePirate.Expenses.New.Description", "Description") + "&nbsp;&nbsp;";
			this.LabelAmount.Text = LocalizationManager.GetLocalString("Interface.Pages.ActivePirate.Expenses.New.Amount", "Amount (incl VAT)") + "&nbsp;&nbsp;";

			this.ButtonClaimExpense.Text = LocalizationManager.GetLocalString("Interface.Pages.ActivePirate.Expenses.New.ButtonClaim", "Claim Expense");

			// Set focus

			this.TextDate.Focus();
		}
	}



	private void PopulateGeographies()
	{
		Person currentUser = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
		Authority authority = currentUser.GetAuthority();

		Organization org = Organization.FromIdentity(Convert.ToInt32(DropOrganizations.SelectedValue));

		Geographies geoList = authority.GetGeographiesForOrganization(org);

		geoList = geoList.RemoveRedundant();
		geoList = geoList.FilterAbove(Geography.FromIdentity(org.AnchorGeographyId));

		if (org.Identity == Organization.SandboxIdentity)
		{
			geoList = Geographies.FromSingle(Geography.Root);
		}

		this.DropGeographies.Items.Clear();

		foreach (Geography nodeRoot in geoList)
		{
			Geographies nodeTree = nodeRoot.GetTree();

			foreach (Geography node in nodeTree)
			{
				string nodeLabel = node.Name;

				for (int loop = 0; loop < node.Generation; loop++)
				{
					nodeLabel = "|-- " + nodeLabel;
				}
				DropGeographies.Items.Add(new ListItem(nodeLabel, node.GeographyId.ToString()));
			}
		}
	}


	protected void DropOrganizations_SelectedIndexChanged(object sender, EventArgs e)
	{
		PopulateGeographies();
	}

	protected void ButtonClaimExpense_Click(object sender, EventArgs e)
	{
		// First, clear the errors (assume good faith).

		this.LabelDateMessage.Text = string.Empty;
		this.LabelDescriptionMessage.Text = string.Empty;
		this.LabelAmountMessage.Text = string.Empty;

		bool badData = false;
		decimal amount = 0.0m;
		string description = this.TextDescription.Text;
		DateTime expenseDate = DateTime.MinValue;

		// Parse amount -- culture-sensitive

		try
		{
			amount = Decimal.Parse(this.TextAmount.Text);
		}
		catch (Exception)
		{
			badData = true;
			this.LabelAmountMessage.Text = "Invalid amount!"; // Localize
			this.TextAmount.Focus();
		}


		if (description.Length < 5) // If too short (5 is arbitrary)
		{
			badData = true;
			this.LabelDescriptionMessage.Text = "Specify the expense!"; // Localize
			this.TextDescription.Focus();
		}

		// Parse date -- culture sensitive

		try
		{
			expenseDate = DateTime.Parse(this.TextDate.Text);
		}
		catch (Exception)
		{
			badData = true;
			this.LabelDateMessage.Text = "Invalid date!"; // Localize
			this.TextDate.Focus();
		}


		if (badData)
		{
			return; // no further processing
		}

		// Create the expense claim.

		int geographyId = Convert.ToInt32(this.DropGeographies.SelectedValue);
		int organizationId = Convert.ToInt32(this.DropOrganizations.SelectedValue);
		int personId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

        // DO NOT CREATE:
        // this page has been superseded by its v4 counterpart

        /*
		ExpenseClaim expenseClaim = ExpenseClaim.Create(personId, organizationId, geographyId, expenseDate, description, amount);

		PirateWeb.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.ExpenseCreated, personId,
			organizationId, geographyId, personId, expenseClaim.Identity, string.Empty);

		Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('Your expense was successfully claimed. MARK the receipt CLEARLY with the number --[" + expenseClaim.Identity.ToString() + @"]-- and send it to the following address:\r\n\r\nPiratpartiet Kassor\r\nc/o Anna Svensson\r\nHagagatan 41\r\nSE-60215 Norrkoping');", true);
        */

		// Clear the text fields, reset focus

		this.TextDate.Text = string.Empty;
		this.TextDescription.Text = string.Empty;
		this.TextAmount.Text = string.Empty;

		this.TextDate.Focus();

		this.ListOpenExpenses.Repopulate();
	}

}
