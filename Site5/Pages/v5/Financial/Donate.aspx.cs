using System;
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
            this.PageAccessRequired = new Access (this.CurrentOrganization, AccessAspect.Null);

            this.PageTitle = Resources.Pages.Financial.Donate_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Financial.FileExpenseClaim_Info;
            string bitcoinAddress =
                FinancialAccounts.BitcoinHotPublicRoot
                    .Derive ((uint) this.CurrentOrganization.Identity)
                    .Derive (FinancialAccounts.RootDonationsIndex)
                    .PubKey.GetAddress (Network.Main)
                    .ToString();

            this.BoxTitle.Text = Resources.Pages.Financial.Donate_PageTitle;
            this.LabelExplainBitcoinDonation.Text = String.Format(Resources.Pages.Financial.Donate_Explain, CurrentOrganization.Name, bitcoinAddress);

            this.ImageBitcoinQr.ImageUrl =
                "https://chart.googleapis.com/chart?cht=qr&chs=400x400&chl=bitcoin:" + bitcoinAddress + "&label=" + CurrentOrganization.Name.Replace ("&","et").Replace ("=", string.Empty); // URI scheme doesn't like &, =
        }
    }
}