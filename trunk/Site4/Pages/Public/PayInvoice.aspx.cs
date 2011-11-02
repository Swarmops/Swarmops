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
using PirateWeb.Basic.Types;
using PirateWeb.Logic.Financial;

public partial class Pages_Public_PayInvoice : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (!String.IsNullOrEmpty(Request.QueryString["InvoiceRef"]))
            {
                this.TextInvoiceReference.Text = Request.QueryString["InvoiceRef"];
                LookupReference(Request.QueryString["InvoiceRef"]);
            }

            if (!String.IsNullOrEmpty(Request.QueryString["merchant_return_link"]))
            {
                this.PanelPaypalReturn.Visible = true;
            }
        }
    }

    protected void ButtonLookup_Click(object sender, EventArgs e)
    {
        LookupReference(this.TextInvoiceReference.Text);
    }

    private void LookupReference (string reference)
    {
        OutboundInvoice invoice = null;
        try
        {
            invoice = OutboundInvoice.FromReference(reference);
        }
        catch (Exception)
        {
            this.PanelInvoice.Visible = false;
            this.LabelReferenceNotFound.Text = "Invoice not found or already paid.";
            return;
        }

        if (!invoice.Open)
        {
            this.PanelInvoice.Visible = false;
            this.LabelReferenceNotFound.Text = "Invoice not found or already paid.";
            return;
        }

        this.LabelReferenceNotFound.Text = string.Empty;
        this.PanelInvoice.Visible = true;
        this.LabelInvoiceReference.Text = invoice.Reference;
        this.LabelAmount.Text = invoice.Amount.ToString("N2", new CultureInfo("sv-SE")) + " " + invoice.Currency.Code +
                                " (+5% Credit Card / Paypal surcharge)";
        this.LabelIssuer.Text = invoice.Organization.Name;
        this.LabelRecipient.Text = invoice.CustomerName;
        this.LinkInvoiceImage.NavigateUrl = "http://data.piratpartiet.se/Forms/DisplayOutboundInvoice.aspx?Reference=" +
                                            invoice.Reference + "&Culture=" + (invoice.Domestic
                                                ? invoice.Organization.DefaultCountry.Culture.Replace("-", "").ToLower()
                                                : "enus");


        OutboundInvoiceItems items = invoice.Items;
        items.Add(OutboundInvoiceItem.FromBasic(new BasicOutboundInvoiceItem(0, 0, "PayPal/Credit Card surcharge", (Int64) (invoice.AmountCents * 0.05))));
        items.Add(OutboundInvoiceItem.FromBasic(new BasicOutboundInvoiceItem(0, 0, "INVOICE TOTAL", (Int64) (invoice.AmountCents * 1.05))));

        this.RepeaterItems.DataSource = items;
        this.RepeaterItems.DataBind();

        this.LiteralPaypalButton.Text = String.Format(
            "<input type=\"hidden\" name=\"cmd\" value=\"_xclick\">" +
            "<input type=\"hidden\" name=\"business\" value=\"FM8HU676ABYMA\">" +
            "<input type=\"hidden\" name=\"lc\" value=\"{2}\">" +
            "<input type=\"hidden\" name=\"item_name\" value=\"Invoice #{0}\">" +
            "<input type=\"hidden\" name=\"amount\" value=\"{1}\">" +
            "<input type=\"hidden\" name=\"currency_code\" value=\"SEK\">" +
            "<input type=\"hidden\" name=\"button_subtype\" value=\"services\">" +
            "<input type=\"hidden\" name=\"no_note\" value=\"1\">" +
            "<input type=\"hidden\" name=\"no_shipping\" value=\"1\">" +
            "<input type=\"hidden\" name=\"rm\" value=\"1\">" +
            "<input type=\"hidden\" name=\"return\" value=\"https://pirateweb.net/Pages/Public/PayInvoice.aspx\">" +
            "<input type=\"hidden\" name=\"cancel_return\" value=\"https://pirateweb.net/Pages/Public/PayInvoice.aspx\">" +
            "<input type=\"hidden\" name=\"bn\" value=\"PP-BuyNowBF:btn_paynowCC_LG.gif:NonHosted\">" +
            "<input type=\"hidden\" name=\"invoice\" value=\"{0}\">" +

            "<a href=\"javascript:theForm.__VIEWSTATE.value='';theForm.encoding='application/x-www-form-urlencoded';theForm.action='https://www.paypal.com/cgi-bin/webscr';theForm.submit();\"><img src=\"https://www.paypal.com/en_US/i/btn/btn_paynowCC_LG.gif\" border=\"0\"></a>" +  // Asp.Net hack for PayPal


            "<img alt=\"\" border=\"0\" src=\"https://www.paypal.com/en_US/i/scr/pixel.gif\" width=\"1\" height=\"1\">",
            invoice.Reference, (invoice.Amount * 1.05m).ToString("N2", CultureInfo.InvariantCulture), (invoice.Domestic?invoice.Organization.DefaultCountry.Code:"US"));
    }
}
