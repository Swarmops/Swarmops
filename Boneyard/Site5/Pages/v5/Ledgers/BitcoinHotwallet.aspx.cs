using System;
using System.Globalization;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Pages.v5.Ledgers
{
    public partial class BitcoinHotwallet : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            if (!CurrentOrganization.IsEconomyEnabled)
            {
                Response.Redirect ("/Financial/EconomyNotEnabled.aspx", true);
                return;
            }
            if (CurrentOrganization.FinancialAccounts.AssetsBitcoinHot == null)
            {
                Response.Redirect("/Financial/BitcoinHotwalletNotEnabled.aspx", true);
                return;
            }

            Localize();

            PageIcon = "iconshock-wallet-money";
            PageTitle = Resources.Pages.Ledgers.BitcoinHotwallet_PageTitle;
            InfoBoxLiteral = Resources.Pages.Ledgers.BitcoinHotwallet_Info;

            // Security: BookkeepingDetails.Read. This is sensitive data.

            PageAccessRequired = new Access (CurrentOrganization, AccessAspect.BookkeepingDetails, AccessType.Read);
            RegisterControl (EasyUIControl.DataGrid | EasyUIControl.Tree);
        }


        private void Localize()
        {
            this.LabelContentHeader.Text = Resources.Pages.Ledgers.BitcoinHotwallet_Header;
            this.LiteralHeaderDerivationPath.Text = Resources.Pages.Ledgers.BitcoinHotwallet_DerivationPath;
            this.LiteralHeaderAddress.Text = Resources.Pages.Ledgers.BitcoinHotwallet_Address + @" / " + Resources.Pages.Ledgers.BitcoinHotWallet_TransactionHash;
            this.LiteralHeaderMicrocoins.Text = Resources.Pages.Ledgers.BitcoinHotwallet_BalanceMicrocoins;
            this.LiteralHeaderFiatValue.Text = String.Format(Resources.Pages.Ledgers.BitcoinHotwallet_ValueFiat, CurrentOrganization.Currency.DisplayCode);
        }
    }
}