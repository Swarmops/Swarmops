using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Types;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Activizr.Basic.Enums;

public partial class Pages_v4_ImportPayments : PageV4Base
{

    private static readonly int PPOrgId = Organization.PPSEid;


    protected void Page_Load (object sender, EventArgs e)
    {
        // Identify person

        // HACK as to who has access to what data -- improve this later
        if (!IsPostBack)
        {
            if (_currentUser.Identity == 1)
            {
                this.DropOrganizations.Items.Add(new ListItem("Piratpartiet SE", Organization.PPSEid.ToString()));
                this.DropOrganizations.Items.Add(new ListItem("Sandbox", "3"));
                this.DropOrganizations.Items.Add(new ListItem("Rick's Sandbox", "55")); // Debug & Test
            }
            else if (_authority.HasPermission(Permission.CanDoEconomyTransactions, PPOrgId, -1, Authorization.Flag.ExactOrganization))
            {


                this.DropOrganizations.Items.Add(new ListItem("Piratpartiet SE", Organization.PPSEid.ToString() ));
            }
            else
            {
                // Show some dialog saying that the user has no access to the tools on this page
            }
        }

        // Styles

        this.TextData.Style[HtmlTextWriterStyle.Width] = "100%";
    }



    protected void DropOrganizations_SelectedIndexChanged (object sender, EventArgs e)
    {
        int organizationId = Int32.Parse(this.DropOrganizations.SelectedValue);
        Organization organization = Organization.FromIdentity(organizationId);

        // TODO: CHANGE FILTER SETTINGS

    }

    protected void DropFilters_SelectedIndexChanged (object sender, EventArgs e)
    {
        /*
        switch (this.DropFilters.SelectedValue)
        {
            case "SEBgmax":
                this.LabelFilterInstructions.Text =
                    "Download the payment file from the bank. Open it - it's a text file. Paste its contents here.";
                break;
            default:
                this.LabelFilterInstructions.Text =
                    "Select an import filter.";
                break;
        }*/
    }


    protected void ButtonProcessText_Click (object sender, EventArgs e)
    {
        _currentUser = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
        // Process the contents of the paste area.

        switch (this.DropFilters.SelectedValue)
        {
            case "SEBgmax":
                ImportBgmaxText(this.TextData.Text);
                break;
            default:
                throw new InvalidOperationException("Bad translation filter for this import");
                // TODO: Print error message and bail out
        }

        this.TextData.Text = string.Empty;
    }

    protected void ImportBgmaxText (string text)
    {
        string[] lines = text.Split("\r\n".ToCharArray());

        DateTime timestamp = DateTime.MinValue;
        int bgMaxVersion = 0;

        PaymentGroup curPaymentGroup = null;
        Payment curPayment = null;
        Currency currency = null;
        Int64 curPaymentGroupAmountCents = 0;

        foreach (string line in lines)
        {
            if (line.Length < 2)
            {
                continue; // CR/LF split causes every other line to be empty
            }

            switch (line.Substring(0,2))
            {
                case "01": // BGMAX intro
                    string bgmaxmarker = line.Substring(2, 20).Trim();
                    if (bgmaxmarker != "BGMAX")
                    {
                        throw new Exception("bad format -- not bgmax");
                    }
                    bgMaxVersion = Int32.Parse(line.Substring(22, 2));
                    timestamp = DateTime.ParseExact(line.Substring(24, 20), "yyyyMMddHHmmssffffff",
                                                    CultureInfo.InvariantCulture);
                    break;
                case "05": // Begin payment group
                    if (bgMaxVersion < 1)
                    {
                        throw new InvalidOperationException("BGMax record must precede first payment group");
                    }
                    currency = Currency.FromCode(line.Substring(22, 3));
                    curPaymentGroup = PaymentGroup.Create(Organization.PPSE, timestamp, currency, _currentUser);
                    curPaymentGroupAmountCents = 0;
                    break;
                case "20": // Begin payment
                    if (curPaymentGroup == null)
                    {
                        throw new InvalidOperationException("Payment group start must precede first payment");
                    }
                    string fromAccount = line.Substring(2, 10);
                    string reference = line.Substring(12, 25).Trim(); // left space padded in BgMax format
                    Int64 amountCents = Int64.Parse(line.Substring(37, 18), CultureInfo.InvariantCulture);
                    string key = "SEBGM" + DateTime.Today.Year.ToString() + line.Substring(57, 12);
                    bool hasImage = (line[69] == '1' ? true : false);

                    // TODO: Check if existed already -- must do -- IMPORTANT

                    curPayment = curPaymentGroup.CreatePayment(amountCents, reference, fromAccount, key, hasImage);
                    curPaymentGroupAmountCents += amountCents;
                    break;
                case "25": // Payment info: Freeform
                    if (curPayment == null)
                    {
                        throw new InvalidOperationException("Payment start must precede payment information");
                    }
                    curPayment.AddInformation(PaymentInformationType.Freeform, line.Substring(2, 50).Trim());
                    break;
                case "26": // Payment info: Name
                    if (curPayment == null)
                    {
                        throw new InvalidOperationException("Payment start must precede payment information");
                    }
                    curPayment.AddInformation(PaymentInformationType.Name, line.Substring(2, 35).Trim());
                    break;
                case "27": // Payment info: Street, postal code
                    if (curPayment == null)
                    {
                        throw new InvalidOperationException("Payment start must precede payment information");
                    }
                    curPayment.AddInformation(PaymentInformationType.Street, line.Substring(2, 35).Trim());
                    curPayment.AddInformation(PaymentInformationType.PostalCode, line.Substring(37, 9).Replace(" ", "")); // also removes inspace
                    break;
                case "28": // Payment info: City, Country
                    if (curPayment == null)
                    {
                        throw new InvalidOperationException("Payment start must precede payment information");
                    }
                    curPayment.AddInformation(PaymentInformationType.City, line.Substring(2, 35).Trim());
                    curPayment.AddInformation(PaymentInformationType.Country, line.Substring(37, 35).Trim());
                    curPayment.AddInformation(PaymentInformationType.CountryCode, line.Substring(72, 2).Trim());
                    break;
                case "29": // Payment info: City, Country
                    if (curPayment == null)
                    {
                        throw new InvalidOperationException("Payment start must precede payment information");
                    }
                    curPayment.AddInformation(PaymentInformationType.OrgNumber, line.Substring(2, 12).Trim());
                    break;
                case "15": // End payment group
                    if (curPaymentGroup == null)
                    {
                        throw new InvalidOperationException("Payment group start must precede payment group end");
                    }
                    string tag = DateTime.Today.Year.ToString() + line.Substring(45, 5);
                    Int64 reportedAmountCents = Int64.Parse(line.Substring(50, 18), CultureInfo.InvariantCulture);
                    curPaymentGroup.AmountCents = curPaymentGroupAmountCents;
                    curPaymentGroup.Tag = tag;
                    curPaymentGroup.Open = true; // flags payment group as ready

                    curPaymentGroup.MapTransaction();

                    curPaymentGroup = null;
                    break;
                case "70": // BGMAX termination
                    break; // don't care
                default:
                    break; // don't care about other fields
            }
        }

        ScriptManager.RegisterStartupScript(this, Page.GetType(), "alldone",
                                    "alert ('Timestamp: " + timestamp.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + "');",
                                    true);



    }
}