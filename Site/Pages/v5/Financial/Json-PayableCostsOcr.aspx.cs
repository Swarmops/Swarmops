using System;
using System.Globalization;
using System.Text;
using Resources;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.Financial
{
    public partial class JsonPayableCostsOcr : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            // Access required is change access to financials

            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.Financials, AccessType.Write);

            // Get all payable items

            Payouts prototypePayouts = Payouts.Construct (CurrentOrganization);
            Payouts previousPayouts = Payouts.ForOrganization (CurrentOrganization); // gets all currently open payouts - enabled for undoing

            // Format as JSON and return

            string elements = FormatPrototypesAsJson (prototypePayouts);

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

                if (bitcoinHotWalletActive && payout.Account.StartsWith ("bitcoin"))
                {
                    // This is a payout registered to be paid in bitcoin, so don't show for manual payout

                    continue;
                }

                if (payout.Reference.Length < 2)
                {
                    continue; // must have at least a reference and a checksum - two digits minimum
                }

                if (!Formatting.CheckLuhnChecksum (payout.Reference))
                {
                    // invalid checksum

                    continue;
                }

                if (!Formatting.CheckLuhnChecksum (payout.Account))
                {
                    continue;
                }

                result.Append ("{");
                result.AppendFormat (
                    "\"itemId\":\"{0}\"," +
                    "\"due\":\"{1}\","  +
                    "\"reference\":\"<span class='ocrFont'>{2}</span>\"," +
                    "\"amount\":\"<span class='ocrFont'>{3}</span>\"," +
                    "\"account\":\"<span class='ocrFont'>{4}</span>\"," +
                    "\"action\":\"" +
                    "<img class='IconApproval{5} LocalIconApproval LocalPrototype action-icon' baseid='{0}' />" +
                    "<img class='IconApproved{5} LocalIconApproved LocalPrototype status-icon' baseid='{0}' />" +
                    "<img class='IconWait{5} LocalIconWait status-icon' baseid='{0}' />" +
                    "<img class='IconUndo{5} LocalIconUndo LocalPrototype action-icon' baseid='{0}' />" +
                    "<img class='IconDenial{5} LocalIconDenial LocalPrototype action-icon' baseid='{0}' />" +
                    "<img class='IconDenied{5} LocalIconDenied LocalPrototype status-icon' baseid='{0}' />" +
                    "\"" ,
                    payout.ProtoIdentity,
                    (payout.ExpectedTransactionDate <= today
                        ? Global.Global_ASAP
                        : payout.ExpectedTransactionDate.ToShortDateString()),
                    GetReferenceOcr (payout),
                    GetAmountOcr (payout),
                    GetAccountOcr (payout),
                    payout.ProtoIdentity.Replace ("|", ""));
                result.Append ("},");
            }

            if (result.Length > 0)
            {
                result.Remove (result.Length - 1, 1); // remove last comma, if there are any elements
            }

            return result.ToString();
        }

        private string GetReferenceOcr (Payout payout)
        {
            return payout.Reference + " #&nbsp;";
        }

        private string GetAmountOcr (Payout payout)
        {
            return string.Format ("{0} {1:00} &nbsp; {2} &gt;", // three spaces between the cents and the checksum
                payout.AmountCents/100, payout.AmountCents %100,
                Formatting.GetLuhnChecksum (payout.AmountCents.ToString (CultureInfo.InvariantCulture)));
        }

        private string GetAccountOcr (Payout payout)
        {
            return Formatting.CleanNumber (payout.Account) + "#41#";
        }
    }
}