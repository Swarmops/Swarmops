using System;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NBitcoin;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class Donate : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access (this.CurrentOrganization, AccessAspect.Participant);

            this.PageTitle = Resources.Pages.Financial.Donate_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Financial.FileExpenseClaim_Info;
            string bitcoinAddress =
                FinancialAccounts.BitcoinHotPublicRoot
                    .Derive ((uint) this.CurrentOrganization.Identity)
                    .Derive (FinancialAccounts.BitcoinDonationsIndex)
                    .Derive ((uint) this.CurrentUser.Identity)
                    .PubKey.GetAddress (Network.Main)
                    .ToString();

            // TEST TEST TEST
            /*
            BitcoinSecret secretKey =
                FinancialAccounts.BitcoinHotPrivateRoot.Derive ((uint) this.CurrentOrganization.Identity)
                    .Derive (FinancialAccounts.BitcoinDonationsIndex)
                    .PrivateKey
                    .GetBitcoinSecret (Network.Main);

            Coin[] spendableCoin = BitcoinUtility.GetSpendableCoin (secretKey);

            TransactionBuilder txBuild = new TransactionBuilder();
            Transaction tx = txBuild.AddCoins (spendableCoin)
                .AddKeys (secretKey)
                .Send (new BitcoinAddress (FinancialAccounts.BitcoinTestAddress), "0.001")
                .SendFees ("0.0001")
                .SetChange (secretKey.GetAddress())
                .BuildTransaction (true);

            bool test = txBuild.Verify (tx);
            if (!test)
            {
                throw new InvalidOperationException("Tx is not properly signed");
            }

            string txHex = tx.ToHex();*/

            this.BoxTitle.Text = Resources.Pages.Financial.Donate_PageTitle;
            this.LabelExplainBitcoinDonation.Text = String.Format(Resources.Pages.Financial.Donate_Explain, CurrentOrganization.Name, bitcoinAddress);

            this.ImageBitcoinQr.ImageUrl =
                "https://chart.googleapis.com/chart?cht=qr&chs=400x400&chl=bitcoin:" + HttpUtility.UrlEncode(bitcoinAddress + "?label=" + CurrentOrganization.Name.Replace ("&","et").Replace ("=", string.Empty)); // URI scheme doesn't like &, =
        }
    }
}