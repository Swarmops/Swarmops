using System;
using System.Text;
using Resources;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.Financial
{
    public partial class JsonPayableCosts : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            // Access required is change access to financials

            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Financials, AccessType.Write);

            // Get all payable items

            Payouts prototypePayouts = Payouts.Construct (CurrentOrganization);
            Payouts previousPayouts = Payouts.ForOrganization (CurrentOrganization); // gets all currently open payouts - enabled for undoing

            // Format as JSON and return

            string prototypes = FormatPrototypesAsJson (prototypePayouts);
            string previous = FormatPreviousAsJson (previousPayouts);

            string elements = (prototypes.Length > 0 && previous.Length > 0
                ? prototypes + "," + previous  // if both have elements, add a comma between them
                : prototypes + previous);  // one or both strings are empty, so no comma

            Response.ContentType = "application/json";
            string json = "{\"rows\":[" + elements + "]}";
            Response.Output.WriteLine (json);
            Response.End();
        }

        private string FormatPrototypesAsJson (Payouts payouts)
        {
            StringBuilder result = new StringBuilder (16384);

            DateTime today = DateTime.Today;
            bool bitcoinHotWalletActive = (this.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot != null
                ? true
                : false);

            foreach (Payout payout in payouts)
            {
                if (bitcoinHotWalletActive && payout.RecipientPerson != null && payout.RecipientPerson.BitcoinPayoutAddress.Length > 0 && payout.Account.Length < 4)  // 4 because an empty account will be " / ", length 3
                {
                    // This is a person who will be paid in bitcoin per personal preferences, so don't show for manual payout

                    continue;
                }

                if (bitcoinHotWalletActive && payout.Account.StartsWith ("bitcoin:"))
                {
                    // This is a payout registered to be paid in bitcoin, so don't show for manual payout

                    continue;
                }

                result.Append ("{");
                result.AppendFormat (
                    "\"itemId\":\"{0}\"," +
                    "\"due\":\"{1}\"," +
                    "\"recipient\":\"{2}\"," +
                    "\"bank\":\"{3}\"," +
                    "\"account\":\"{4}\"," +
                    "\"reference\":\"{5}\"," +
                    "\"amount\":\"{6:N2}\"," +
                    "\"action\":\"" +
                    "<img id=\\\"IconApproval{7}\\\" class=\\\"LocalIconApproval LocalPrototype\\\" baseid=\\\"{0}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                    "<img id=\\\"IconApproved{7}\\\" class=\\\"LocalIconApproved LocalPrototype\\\" baseid=\\\"{0}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                    "<img id=\\\"IconDenial{7}\\\" class=\\\"LocalIconDenial LocalPrototype\\\" baseid=\\\"{0}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                    "<img id=\\\"IconDenied{7}\\\" class=\\\"LocalIconDenied LocalPrototype\\\" baseid=\\\"{0}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                    "<img id=\\\"IconUndo{7}\\\" class=\\\"LocalIconUndo LocalPrototype\\\" baseid=\\\"{0}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                    "\"",
                    payout.ProtoIdentity,
                    (payout.ExpectedTransactionDate <= today
                        ? Global.Global_ASAP
                        : payout.ExpectedTransactionDate.ToShortDateString()),
                    JsonSanitize (TryLocalize (payout.Recipient)),
                    JsonSanitize (payout.Bank),
                    JsonSanitize (payout.Account),
                    JsonSanitize (TryLocalize (payout.Reference)),
                    payout.AmountCents/100.0,
                    payout.ProtoIdentity.Replace ("|", ""));
                result.Append ("},");
            }

            if (result.Length > 0)
            {
                result.Remove (result.Length - 1, 1); // remove last comma, if there are any elements
            }

            return result.ToString();
        }

        private string FormatPreviousAsJson(Payouts payouts)
        {
            StringBuilder result = new StringBuilder(16384);

            DateTime today = DateTime.Today;

            foreach (Payout payout in payouts)
            {
                result.Append("{");
                result.AppendFormat(
                    "\"itemId\":\"{0}\"," +
                    "\"databaseId\":\"{8}\"," +
                    "\"due\":\"{1}\"," +
                    "\"recipient\":\"{2}\"," +
                    "\"bank\":\"{3}\"," +
                    "\"account\":\"{4}\"," +
                    "\"reference\":\"{5}\"," +
                    "\"amount\":\"{6:N2}\"," +
                    "\"action\":\"" +
                    "<img id=\\\"IconApproval{7}\\\" class=\\\"LocalIconApproval LocalPrevious\\\" databaseid=\\\"{8}\\\" baseid=\\\"{0}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                    "<img id=\\\"IconApproved{7}\\\" class=\\\"LocalIconApproved LocalPrevious\\\" baseid=\\\"{0}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                    "<img id=\\\"IconDenial{7}\\\" class=\\\"LocalIconDenial LocalPrevious\\\" baseid=\\\"{0}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                    "<img id=\\\"IconDenied{7}\\\" class=\\\"LocalIconDenied LocalPrevious\\\" baseid=\\\"{0}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                    "<img id=\\\"IconUndo{7}\\\" class=\\\"LocalIconUndo LocalPrevious\\\" baseid=\\\"{0}\\\" height=\\\"18\\\" width=\\\"24\\\" />" +
                    "\"",
                    payout.ProtoIdentity,
                    payout.ExpectedTransactionDate.ToShortDateString(),
                    JsonSanitize(TryLocalize(payout.Recipient)),
                    JsonSanitize(payout.Bank),
                    JsonSanitize(payout.Account),
                    JsonSanitize(TryLocalize(payout.Reference)),
                    payout.AmountCents / 100.0,
                    payout.ProtoIdentity.Replace("|", ""),
                    payout.Identity);
                result.Append("},");
            }

            if (result.Length > 0)
            {
                result.Remove(result.Length - 1, 1); // remove last comma, if there are any elements
            }

            return result.ToString();
        }
    }
}