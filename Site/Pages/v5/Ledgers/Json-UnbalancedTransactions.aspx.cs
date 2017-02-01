using System;
using System.Collections.Generic;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class Json_UnbalancedTransactions : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            FinancialTransactions unbalancedTransactions = FinancialTransactions.GetUnbalanced (CurrentOrganization);
            List<string> rows = new List<string>();

            if (
                CurrentAuthority.HasAccess (new Access (CurrentOrganization, AccessAspect.BookkeepingDetails, AccessType.Read)))
            {
                foreach (FinancialTransaction transaction in unbalancedTransactions)
                {
                    string accountName = string.Empty;

                    FinancialTransactionRows txRows = transaction.Rows;
                    if (txRows.Count > 1)
                    {
                        accountName = Resources.Global.Global_Several_Display;
                    }
                    else
                    {
                        // one transaction row (we know there's not zero rows, because the transaction is unbalanced, which requires at least one nonzero row)
                        accountName = txRows[0].AccountName;
                    }

                    string row =
                        String.Format (
                        "\"id\":\"{0:N0}\",\"description\":\"{1}\",\"accountName\":\"{2}\",\"delta\":\"{3:+#,#.00;−#,#.00}\",\"dateTime\":\"{4:yyyy-MMM-dd HH:mm}\"",
                            transaction.Identity, JsonSanitize (transaction.Description), JsonSanitize (accountName), txRows.AmountCentsTotal / 100.0, transaction.DateTime);

                    row +=
                        String.Format (
                            ",\"action\":\"<img src='/Images/Icons/iconshock-wrench-128x96px-centered.png' class='LocalIconFix' txId='{0}' />\"",
                            transaction.Identity);

                    rows.Add ("{" + row + "}");
                }
            }

            Response.Output.WriteLine("[" + String.Join (",", rows) + "]");

            Response.End();
        }
    }
}