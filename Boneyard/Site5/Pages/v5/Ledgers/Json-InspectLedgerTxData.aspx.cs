using System;
using System.Globalization;
using System.Text;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class Json_InspectLedgerTxData : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            string transactionIdString = Request.QueryString["TxId"];

            string emptyResponse = "[{\"id\":\"-\",\"accountName\":\"" +
                                   JsonSanitize (Resources.Pages.Ledgers.InspectLedgers_EmptyTransaction) + "\"}]";

            if (string.IsNullOrEmpty (transactionIdString) || transactionIdString == "undefined")
            {
                Response.Output.WriteLine (emptyResponse);
                Response.End();
            }

            int transactionId = Int32.Parse (transactionIdString);

            DateTime dawnOfMankind = new DateTime (1901, 1, 1);
            // no org will ever import bookkeeping from before this date

            if (transactionId <= 0)
            {
                Response.Output.WriteLine (emptyResponse);
                Response.End();
            }

            FinancialTransaction transaction = FinancialTransaction.FromIdentity (transactionId);
            if (transaction.OrganizationId != CurrentOrganization.Identity)
            {
                throw new UnauthorizedAccessException ("All the nopes in the world");
            }

            if (!CurrentAuthority.HasAccess (new Access (CurrentOrganization, AccessAspect.Bookkeeping, AccessType.Read)))
            {
                throw new UnauthorizedAccessException ("Access denied because security tokens say so");
            }

            FinancialTransactionRows rows = transaction.Rows;

            StringBuilder result = new StringBuilder (16384);

            foreach (FinancialTransactionRow row in rows)
            {
                string creditString = string.Empty;
                string debitString = string.Empty;

                if (row.AmountCents < 0)
                {
                    creditString = String.Format ("{0:N2}", row.AmountCents/100.0);
                }
                else if (row.AmountCents > 0)
                {
                    debitString = String.Format ("{0:N2}", row.AmountCents/100.0);
                }

                result.Append ("{" + String.Format (
                    "\"id\":\"{0}\",\"dateTime\":\"{1:yyyy-MMM-dd HH:mm}\",\"accountName\":\"{2}\"," +
                    "\"deltaPos\":\"{3}\",\"deltaNeg\":\"{4}\",\"initials\":\"{5}\"",
                    row.FinancialTransactionId,
                    row.CreatedDateTime,
                    JsonSanitize (row.AccountName),
                    debitString,
                    creditString,
                    row.CreatedByPerson != null? row.CreatedByPerson.Initials: Resources.Global.Global_System) + "},");
            }

            Int64 amountCentsTotal = transaction.Rows.AmountCentsTotal;

            if (amountCentsTotal != 0)
            {
                // If the transaction is unbalanced, make a huge deal about it

                result.Append ("{\"accountName\":\"<img src='/Images/Icons/iconshock-warning-24px.png' height='16px' width='16px' class='elementFloatFar' />" +
                               JsonSanitize (Resources.Pages.Ledgers.InspectLedgers_UnbalancedTransaction) + "\",");

                if (amountCentsTotal < 0)
                {
                    result.AppendFormat("\"deltaPos\":\"<span class='spanAnnoyingBlink'>{0:N2}</span>\"",
                        -amountCentsTotal / 100.0);
                }
                else if (amountCentsTotal > 0)
                {
                    result.AppendFormat("\"deltaNeg\":\"<span class='spanAnnoyingBlink'>{0:N2}</span>\"",
                        -amountCentsTotal / 100.0);
                }

                result.Append ("},");  // the comma isn't really necessary here but will be stripped later and is kept for cut&paste consistency of this code block
            }

            Response.Output.WriteLine ("[" + result.ToString().TrimEnd (',') + "]");
            Response.End();
        }
    }
}