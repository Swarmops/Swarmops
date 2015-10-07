using System;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using NBitcoin;
using Swarmops.Logic.Cache;
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
            this.InfoBoxLiteral = Resources.Pages.Financial.Donate_Info;
            this.LabelStatus.Text = Resources.Pages.Financial.Donate_StatusInitial;

            HotBitcoinAddress address = HotBitcoinAddress.Create (this.CurrentOrganization,
                BitcoinUtility.BitcoinDonationsIndex, this.CurrentUser.Identity);

            this.LiteralBitcoinAddress.Text = address.Address;
            string guid = Guid.NewGuid().ToString ("N");
            GuidCache.Set (guid, address.Address);
            this.LiteralGuid.Text = guid;

            // TEST TEST TEST
            /*
            BitcoinSecret secretKey =
                FinancialAccounts.BitcoinHotPrivateRoot.Derive ((uint) this.CurrentOrganization.Identity)
                    .Derive (BitcoinUtility.BitcoinDonationsIndex)
                    .Derive ((uint) this.CurrentUser.Identity)
                    .PrivateKey
                    .GetBitcoinSecret (Network.Main);

            Coin[] spendableCoin = BitcoinUtility.GetSpendableCoin (secretKey);

            TransactionBuilder txBuild = new TransactionBuilder();
            Transaction tx = txBuild.AddCoins (spendableCoin)
                .AddKeys (secretKey)
                .Send (new BitcoinAddress (BitcoinUtility.BitcoinTestAddress), "0.001")
                .SendFees ("0.0001")
                .SetChange (secretKey.GetAddress())
                .BuildTransaction (true);

            bool test = txBuild.Verify (tx);
            if (!test)
            {
                throw new InvalidOperationException("Tx is not properly signed");
            }

            BitcoinUtility.BroadcastTransaction (tx);*/

            this.BoxTitle.Text = Resources.Pages.Financial.Donate_PageTitle;
            this.LabelExplainBitcoinDonation.Text = String.Format (Resources.Pages.Financial.Donate_Explain,
                CurrentOrganization.Name, address.Address);

            this.ImageBitcoinQr.ImageUrl =
                "https://chart.googleapis.com/chart?cht=qr&chs=400x400&chl=bitcoin:" +
                HttpUtility.UrlEncode (address.Address + "?label=" +
                                       Uri.EscapeDataString (String.Format (Resources.Pages.Financial.Donate_TxLabel,
                                           CurrentOrganization.Name))); // URI scheme doesn't like &, =
        }

        [WebMethod]
        static public AjaxCallResult CheckTransactionReceived (string guid, string txHash)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture(); // just to make sure we're called properly

            string bitcoinAddress = (string) GuidCache.Get (guid);
            if (BitcoinUtility.TestUnspents (bitcoinAddress))
            {
                HotBitcoinAddressUnspents unspents = HotBitcoinAddress.FromAddress (bitcoinAddress).Unspents;

                Int64 satoshisReceived = unspents.Last().AmountSatoshis;

                if (unspents.Last().TransactionHash != txHash && txHash.Length > 0)
                {
                    // Race condition.
                    Debugger.Break();
                }

                Swarmops.Logic.Financial.Money moneyReceived = new Swarmops.Logic.Financial.Money (satoshisReceived,
                    Currency.Bitcoin);

                // Create success message and ledger transaction
                string successMessage = string.Empty;

                if (authData.CurrentOrganization.Currency.IsBitcoin)
                {
                    // The ledger is native bitcoin, so units are Satoshis 

                    FinancialTransaction ledgerTx = FinancialTransaction.Create (authData.CurrentOrganization,
                        DateTime.UtcNow, "Donation (bitcoin to hotwallet)");
                    ledgerTx.AddRow (authData.CurrentOrganization.FinancialAccounts.IncomeDonations, -satoshisReceived, authData.CurrentUser);
                    ledgerTx.AddRow (authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot, satoshisReceived, authData.CurrentUser);
                    ledgerTx.BlockchainHash = txHash;

                    if (satoshisReceived % 100 == 0)
                    {
                        successMessage = string.Format (Resources.Pages.Financial.Donate_FundsReceivedNative,
                            (satoshisReceived/100.0).ToString ("N0"));
                    }
                    else
                    {
                        successMessage = string.Format(Resources.Pages.Financial.Donate_FundsReceivedNative,
                            (satoshisReceived / 100.0).ToString("N2"));
                    }
                }
                else
                {
                    // The ledger is NOT native bitcoin, so we'll need to convert currencies

                    long nativeCents = moneyReceived.ToCurrency (authData.CurrentOrganization.Currency).Cents;
                    FinancialTransaction ledgerTx = FinancialTransaction.Create(authData.CurrentOrganization,
                        DateTime.UtcNow, "Donation (bitcoin to hotwallet)");
                    ledgerTx.AddRow(authData.CurrentOrganization.FinancialAccounts.IncomeDonations, -nativeCents, authData.CurrentUser);
                    ledgerTx.AddRow(authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot, nativeCents, authData.CurrentUser);
                    ledgerTx.BlockchainHash = txHash;
                }

                return new AjaxCallResult() {DisplayMessage = successMessage, Success = true};

                // TODO: Ack donation via mail?
                // TODO: Notify CFO/etc of donation?
            }

            return new AjaxCallResult() {Success = false};
        }
    }
}