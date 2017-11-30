using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Frontend;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

public partial class Pages_v5_Finance_Json_ListInvoicesOutbound : DataV5Base
{
    private OutboundInvoices _invoices;

    protected void Page_Load (object sender, EventArgs e)
    {
        // Get invoices and attestation rights

        this._invoices = OutboundInvoices.ForOrganization(this.CurrentOrganization, true);

        _invoices.Sort(SortInvoicesByCreatedDateReverse);

        // Format as JSON and return

        Response.ContentType = "application/json";
        string json = FormatAsJson();
        Response.Output.WriteLine (json);
        Response.End();
    }

    private static int SortInvoicesByCreatedDateReverse(OutboundInvoice a, OutboundInvoice b)
    {
        return DateTime.Compare(b.CreatedDateTime, a.CreatedDateTime);
    }


    private string FormatAsJson()
    {
        // Fields: item, dueDate, sender, budget, amount, progress, docs, action

        StringBuilder result = new StringBuilder (16384);

        string hasDoxString =
            "<img src='/Images/Icons/iconshock-search-256px.png' onmouseover=\\\"this.src='/Images/Icons/iconshock-search-hot-256px.png';\\\" onmouseout=\\\"this.src='/Images/Icons/iconshock-search-256px.png';\\\" baseid='O{6}' class='LocalViewDox' style='cursor:pointer' height='20' width='20' />";

        result.Append ("{\"rows\":[");

        DateTime dueDateFormatBreakDate = DateTime.Today.AddDays(-240);

        foreach (OutboundInvoice invoice in _invoices)
        {
            result.Append("{");
            result.AppendFormat(
                "\"item\":\"{0}\",\"dueDate\":\"{1}\",\"sender\":\"{2}\",\"budget\":\"{3}\",\"amount\":\"{4}\",\"progress\":\"{5}\"," +
                "\"dox\":\"" + (invoice.Documents.Count > 0 ? hasDoxString : "&nbsp;") + "\"," +
                "\"actions\":\"<span style='position:relative;left:-2px'>" +
                //"<img id=\\\"IconApproval{5}\\\" class=\\\"LocalIconApproval LocalNew LocalFundsInsufficient\\\" accountid=\\\"{6}\\\" amount=\\\"{4}\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                //"<img id=\\\"IconApproved{5}\\\" class=\\\"LocalIconApproved LocalNew\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                //"<img id=\\\"IconDenial{5}\\\" class=\\\"LocalIconDenial LocalNew\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                //"<img id=\\\"IconDenied{5}\\\" class=\\\"LocalIconDenied LocalNew\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                //"<img id=\\\"IconUndo{5}\\\" class=\\\"LocalIconUndo LocalNew\\\" baseid=\\\"{5}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                "</span>\"",
                "#" + invoice.Identity.ToString("N0"),

                JsonSanitize(invoice.DueDate.ToString(invoice.DueDate < dueDateFormatBreakDate ? "yyyy-MMM" : "MMM-dd")),
                "Supplier",
                JsonSanitize(invoice.Budget.Name),
                JsonSanitize((invoice.AmountCents/100.0).ToString("N2")),
                GetProgressTicks(invoice),
                invoice.Identity); // Item #6 is only present in hasDoxString above
            result.Append("},");
        }

        if (_invoices.Count > 1)
        {
            result.Remove(result.Length - 1, 1); // remove last comma
        }

        result.Append ("]}");

        return result.ToString();
    }

    private string _emptyTick = "<img src='/Images/Icons/iconshock-empty-tick-128x96px.png' height='12' width='16'>";
    private string _greenTick = "<img src='/Images/Icons/iconshock-green-tick-128x96px.png' height='12' width='16'>";
    private string _redCross = "<img src='/Images/Icons/iconshock-red-cross-128x96px.png' height='12' width='16'>";
    private string _filler = "<img src='/Images/Icons/transparency-16px.png' height='12' width='16'>";


    private string GetProgressTicks(OutboundInvoice invoice)
    {
        StringBuilder ticks = new StringBuilder(512);

        // The first tick is whether the invoice was even received yet, in anticipation of Purchase Orders
        // For now, it is always filled

        ticks.Append(_greenTick);

        // For the time being, the other three are either all filled or all empty

        if (invoice.Open)
        {
            ticks.Append(_emptyTick + _emptyTick + _emptyTick);
        }
        else
        {
            ticks.Append(_greenTick + _greenTick + _greenTick);
        }

        return ticks.ToString();
    }




}