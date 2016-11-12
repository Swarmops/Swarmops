using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

public partial class Pages_Public_Handlers_PaypalListener : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // THIS CODE IS TAKEN FROM PAYPAL'S TEMPLATE.

        Person notifyPerson = Person.FromIdentity(1);


        //Post back to either sandbox or live
        string strLive = "https://www.paypal.com/cgi-bin/webscr";
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strLive);

        //Set values for the request back
        req.Method = "POST";
        req.ContentType = "application/x-www-form-urlencoded";
        byte[] param = Request.BinaryRead(HttpContext.Current.Request.ContentLength);
        string strRequest = Encoding.ASCII.GetString(param);
        string originalRequest = strRequest;
        strRequest += "&cmd=_notify-validate";
        req.ContentLength = strRequest.Length;

        //Send the request to PayPal and get the response
        StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
        streamOut.Write(strRequest);
        streamOut.Close();
        StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
        string strResponse = streamIn.ReadToEnd();
        streamIn.Close();

        // Decode parameters
        List<string> temp = new List<string>();
        Dictionary<string, string> parameters = new Dictionary<string, string>();

        string[] parameterStrings = originalRequest.Split('&');
        foreach (string parameter in parameterStrings)
        {
            string[] data = parameter.Split('=');

            string dataKey = Server.UrlDecode(data[0]);
            string dataValue = Server.UrlDecode(data[1]);

            parameters[dataKey] = dataValue;
            temp.Add(string.Format("{0,-20} {1}", dataKey, dataValue));
        }


        notifyPerson.SendNotice("Paypal listener point A", String.Join("\r\n",temp.ToArray()), 1);

        bool proceed = true;

        if (strResponse == "INVALID")
        {
            notifyPerson.SendNotice("Paypal listener FAIL - Response is INVALID", string.Empty, 1);
            proceed = false;
        }
        if (!parameters.ContainsKey("payment_status"))
        {
            notifyPerson.SendNotice("Paypal listener FAIL - payment_status was not supplied", string.Empty, 1);
            proceed = false;
        }
        else if (parameters["payment_status"] != "Completed" && parameters["payment_status"] != "Refunded")
        {
            notifyPerson.SendNotice("Paypal listener FAIL - payment_status is not Completed / Refunded", string.Empty, 1);
            proceed = false;
        }
        if (parameters["receiver_email"] != "donation@piratpartiet.se")
        {
            notifyPerson.SendNotice("Paypal listener FAIL - receiver_email is not donation@piratpartiet.se", string.Empty, 1);
            proceed = false;  // HACK -- adjust for multiple orgs
        }
        if (parameters["mc_currency"] != "SEK")
        {
            notifyPerson.SendNotice("Paypal listener FAIL - mc_currency is not SEK", string.Empty, 1);
            proceed = false; // HACK -- adjust for multiple orgs
        }
        if (!parameters.ContainsKey("mc_gross") || String.IsNullOrEmpty(parameters["mc_gross"]))
        {
            notifyPerson.SendNotice("Paypal listener FAIL - mc_net is null or empty", string.Empty, 1);
            proceed = false;
        }

        if (strResponse == "VERIFIED" && proceed)
        {
            string transactionId = parameters["txn_id"];
            string identifier = null;
            
            if (parameters.ContainsKey("invoice"))
            {
                identifier = parameters["invoice"];
            }
            
            OutboundInvoice invoice = null;

            if (!String.IsNullOrEmpty(identifier))
            {
                invoice = OutboundInvoice.FromReference(identifier);

                notifyPerson.SendNotice("Paypal listener - invoice identified as #" + invoice.Identity.ToString(), string.Empty, 1);
            }

            string grossString = parameters["mc_gross"];

            string feeString = "0.0";

            if (parameters.ContainsKey("mc_fee") && !string.IsNullOrEmpty(parameters["mc_fee"]))
            {
                feeString = parameters["mc_fee"];
            }
            
            string comment = "Paypal ";

            if (parameters["payment_status"] == "Completed")
            {
                comment += parameters["txn_type"];
            }
            else
            {
                // Refund

                comment += "Refund";
            }

            if (parameters.ContainsKey("item_name"))
            {
                comment = parameters["item_name"];
            }

            if (invoice != null)
            {
                comment = "Outbound Invoice #" + invoice.Identity.ToString() + " payment Paypal";
            }

            DateTime dateTime = DateTime.ParseExact(parameters["payment_date"].Replace("PDT", "-07").Replace("PST", "-08"), "HH:mm:ss MMM dd, yyyy zz", CultureInfo.InvariantCulture).ToLocalTime();

            Organization org = Organization.PPSE;

            Int64 feeCents = Int64.Parse(feeString.Replace(".", ""));
            Int64 grossCents = Int64.Parse(grossString.Replace(".", ""));

            Int64 amountCents = grossCents - feeCents;

            FinancialTransaction transaction = FinancialTransaction.ImportWithStub(org.Identity, dateTime,
                                                                                   org.FinancialAccounts.AssetsPaypal.Identity, amountCents,
                                                                                   comment, transactionId,
                                                                                   0);  // TODO: Convert to cents

            if (transaction != null)
            {
                // The transaction was created. Examine if the autobook criteria are true.

                if (amountCents < 0 && parameters["payment_status"] == "Completed")
                {
                    // Do not balance the transaction -- will have to be balanced manually
                }
                else if (amountCents > 0)
                {
                    if (invoice != null)
                    {
                        Int64 expectedNetCents = invoice.AmountCents;

                        transaction.AddRow(org.FinancialAccounts.CostsBankFees, expectedNetCents - amountCents, null);  // can be negative
                        transaction.AddRow(org.FinancialAccounts.AssetsOutboundInvoices, -expectedNetCents, null);

                        // Hack: Add a line to the outbound invoice HERE with the PayPal surcharge

                        invoice.AddItem("PayPal/Credit Card surcharge, 5%", (Int64) (invoice.AmountCents*0.05));

                        Payment payment = Payment.CreateSingle(invoice.Organization, DateTime.Now, invoice.Currency,
                                                               expectedNetCents, invoice, null);  // HACK HACK HACK: says we always received the expected amount

                        payment.AddInformation(PaymentInformationType.RefundInformation, "PayPal " + transactionId);
                        payment.AddInformation(PaymentInformationType.Name, parameters["first_name"] + " " + parameters["last_name"]);
                        payment.AddInformation(PaymentInformationType.Email, parameters["payer_email"]);

                        transaction.Dependency = payment.Group;
                    }
                    else if (feeCents > 0)
                    {
                        // This is always an autodeposit

                        transaction.AddRow(org.FinancialAccounts.CostsBankFees, feeCents, null);
                        transaction.AddRow(org.FinancialAccounts.IncomeDonations, -grossCents, null);
                    }
                    else
                    {
                        transaction.AddRow(org.FinancialAccounts.IncomeDonations, -amountCents, null);
                    }
                }
            }


        }
    }
}
