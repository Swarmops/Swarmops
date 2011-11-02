using System;
using System.Collections;
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

public partial class Pages_v4_Activism_CreateParley : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            // Initialize controls

            // Fake it for now

            this.DropOrganizations.Items.Add(new ListItem("Piratpartiet SE", "1"));

            this.LiteralItems.Text =  "<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"400px\"><tr><td>Option_Name</td><td align=\"right\">Amount</td></tr></table>";
            ViewState["Options"] = string.Empty;
            this.DropGeographies.SelectedGeography=null;
        }

        this.TextDateCount.Style[HtmlTextWriterStyle.Width] = "30px"; // Override default
        this.TextNewOptionDescription.Style[HtmlTextWriterStyle.Width] = "300px";
    }

    protected void DropOrganizations_SelectedIndexChanged(object sender, EventArgs e)
    {
        int organizationId = Int32.Parse(this.DropOrganizations.SelectedValue);
        this.DropBudgets.Enabled = false;

        if (organizationId == 0)
        {
            return; // no org selected
        }

        Organization organization = Organization.FromIdentity(organizationId);
        this.DropBudgets.Populate(organization, FinancialAccountType.Cost);
        this.DropBudgets.Enabled = true;
    }



    private void AddOptionsInEdit(bool displayErrorMessages)
    {
        if (this.TextNewOptionDescription.Text.Length < 2)
        {
            if (displayErrorMessages)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "StartupMessage", "alert('Please write an option description.');", true);
            }
            return;
        }

        double amount = 0.0;
        if (!Double.TryParse(this.TextNewOptionAmount.Text, NumberStyles.Float, new CultureInfo("sv-SE"), out amount))
        {
            if (displayErrorMessages)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "StartupMessage", "alert('Please write a numeric amount as price for the option.');", true);
            }
            return;
        }
        string amountString = amount.ToString(CultureInfo.InvariantCulture);

        string currentItems = (string)ViewState["Options"];

        if (currentItems.Length > 0)
        {
            currentItems += "|";
        }

        currentItems += this.TextNewOptionDescription.Text.Replace("|", ":") + "|" + amountString;
        ViewState["Options"] = currentItems;

        // Construct the table

        string table = "<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"400px\">";
        string[] itemsArray = currentItems.Split('|');

        for (int index = 0; index < itemsArray.Length; index += 2)
        {
            double localAmount = Double.Parse(itemsArray[index + 1], CultureInfo.InvariantCulture);

            table += "<tr><td>" + Server.HtmlEncode(itemsArray[index]) + "</td><td align=\"right\">" +
                     localAmount.ToString("N2", new CultureInfo("sv-SE")) + "</td></tr>";
        }
        table += "</table>";

        this.LiteralItems.Text = table;

    }

    protected void ButtonAddOption_Click(object sender, EventArgs e)
    {
        AddOptionsInEdit(true);
        this.TextNewOptionDescription.Text = string.Empty;
        this.TextNewOptionAmount.Text = string.Empty;
        this.TextNewOptionDescription.Focus();
    }

    protected void WizardCreateConference_Finish(object sender, WizardNavigationEventArgs e)
    {
        Parley newParley = Parley.Create(Organization.FromIdentity(Int32.Parse(this.DropOrganizations.SelectedValue)),
                                         _currentUser,
                                         this.DropBudgets.SelectedFinancialAccount,
                                         this.TextConferenceName.Text, this.DropGeographies.SelectedGeography,
                                         this.TextDescription.Text, this.TextConferenceUrl.Text,
                                         (DateTime) this.DateStart.SelectedDate,
                                         ((DateTime) this.DateStart.SelectedDate).AddDays(
                                             Int32.Parse(this.TextDateCount.Text) - 1),
                                         Int32.Parse(this.TextBudgetRequested.Text)*100L,
                                         Int32.Parse(this.TextGuaranteeRequested.Text)*100L,
                                         Int32.Parse(this.TextAttendanceFee.Text)*100L);

        string currentItems = (string)ViewState["Options"];

        string[] itemsArray = currentItems.Split('|');

        for (int index = 0; index < itemsArray.Length; index += 2)
        {
            double localAmount = Double.Parse(itemsArray[index + 1], CultureInfo.InvariantCulture);
            newParley.CreateOption(itemsArray[index], (Int64)(localAmount*100L));
        }
    }

    protected void Validate_DropBudgets_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = (this.DropBudgets.SelectedFinancialAccount == null ? false : true);
    }
}
