using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using NBitcoin;
using RestSharp.Serializers;
using Swarmops.Common.Enums;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.BackendServices;
using Satoshis = NBitcoin.Money; // Sets it apart from Swarmops' Money class

namespace Swarmops.Frontend.Pages.v5.Admin
{
    public partial class BitcoinEchoTest : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access (this.CurrentOrganization, AccessAspect.Administration);

            // This page is copied from Donate, with the echo payout added to the end

            this.PageTitle = Resources.Pages.Admin.BitcoinEchoTest_PageTitle;
            this.InfoBoxLiteral = String.Format(Resources.Pages.Admin.BitcoinEchoTest_Info, BitcoinUtility.EchoFeeSatoshis / 100.0);
            this.LabelStatus.Text = Resources.Pages.Admin.BitcoinEchoTest_StatusInitial;
            this.SuppressStatisticHeaders = true;

            if (this.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot == null)
            {
                this.PanelDisabled.Visible = true;
                this.PanelEnabled.Visible = false;
            }

            HotBitcoinAddress address = HotBitcoinAddress.CreateUnique(this.CurrentOrganization, BitcoinChain.Cash,
                BitcoinUtility.BitcoinEchoTestIndex, this.CurrentUser.Identity);

            this.BitcoinCashAddressUsed = address.Address;
            string guid = Guid.NewGuid().ToString("N");
            GuidCache.Set(guid, address.Address);
            this.TransactionGuid = guid;

            // Calculate conversion rate (satoshi-cents to unit-cents, so we're good, even if the conversion rate
            // is calculated on microbitcoin to whole units)

            this.ConversionRateSatoshisToCents = Currency.BitcoinCash.GetConversionRate(CurrentOrganization.Currency);

            // BEGIN TEST CODE

            // END TEST CODE

            this.BoxTitle.Text = Resources.Pages.Admin.BitcoinEchoTest_PageTitle;
            this.LabelExplainBitcoinEchoTest.Text = String.Format (Resources.Pages.Admin.BitcoinEchoTest_Explain,
                CurrentOrganization.Name, address.Address, BitcoinUtility.EchoFeeSatoshis / 100.0);

            this.ImageBitcoinQr.ImageUrl =
                "https://chart.googleapis.com/chart?cht=qr&chs=400x400&chl=bitcoincash:" +
                HttpUtility.UrlEncode (address.Address + "?label=" +
                                       Uri.EscapeDataString ("Swarmops Bitcoin Echo Test")); // URI scheme doesn't like &, =
        }


        public static Int64 EchoFeeSatoshis
        {
            get { return BitcoinUtility.EchoFeeSatoshis; }
        }

        public string BitcoinCashAddressUsed { get; private set; }
        public string TransactionGuid { get; private set; }
        public double ConversionRateSatoshisToCents { get; private set; }

        [WebMethod]
        static public AjaxCallResult ProcessTransactionReceived (string guid, string txHash)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture(); // just to make sure we're called properly
            BitcoinChain chain = BitcoinChain.Cash;

            string bitcoinAddress = (string) GuidCache.Get (guid);
            if (BitcoinUtility.TestUnspents (chain, bitcoinAddress))
            {
                HotBitcoinAddressUnspents unspents = HotBitcoinAddress.FromAddress (chain, bitcoinAddress).Unspents;
                Int64 satoshisReceived = unspents.Last().AmountSatoshis;

                if (unspents.Last().TransactionHash != txHash && txHash.Length > 0)
                {
                    // Race condition.
                    Debugger.Break();
                }

                HotBitcoinAddressUnspent utxoToReturn = unspents.Last();

                Swarmops.Logic.Financial.Money moneyReceived = new Swarmops.Logic.Financial.Money(satoshisReceived,
                    Currency.BitcoinCash);

                // Make sure that the hotwallet native currency is bitcoin
                authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot.ForeignCurrency = Currency.BitcoinCash;

                // Create success message and ledger transaction
                string successMessage = string.Empty;

                // TODO: Get the tx, get the input

                string returnAddress = BitcoinUtility.GetInputAddressesForTransaction(chain, txHash) [0]; // assumes at least one input address

                // Return the money, too. Set fee for a 300-byte transaction.

                ReturnBitcoinEchoUtxoOrder backendOrder = new ReturnBitcoinEchoUtxoOrder(utxoToReturn);
                backendOrder.Create(authData.CurrentOrganization, authData.CurrentUser);

                string tx1Description = "Bitcoin technical echo test (will be repaid immediately)";


                if (authData.CurrentOrganization.Currency.IsBitcoinCash)
                {
                    // The ledger is native bitcoin, so cent units are satoshis

                    FinancialTransaction ledgerTx1 = FinancialTransaction.Create(authData.CurrentOrganization,
                        DateTime.UtcNow, tx1Description);
                    ledgerTx1.AddRow(authData.CurrentOrganization.FinancialAccounts.DebtsOther, -(satoshisReceived), authData.CurrentUser);
                    ledgerTx1.AddRow(authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot, satoshisReceived, authData.CurrentUser);
                    ledgerTx1.BlockchainHash = txHash;

                    // The return payment will be logged when made, so its hash can be recorded

                    if (satoshisReceived % 100 == 0)
                    {
                        successMessage = string.Format (Resources.Pages.Admin.BitcoinEchoTest_FundsReceivedNative,
                            (satoshisReceived/100.0).ToString ("N0"));
                    }
                    else
                    {
                        successMessage = string.Format(Resources.Pages.Admin.BitcoinEchoTest_FundsReceivedNative,
                            (satoshisReceived / 100.0).ToString("N2"));
                    }
                }
                else
                {
                    // The ledger is NOT native bitcoin, so we'll need to convert currencies

                    long orgNativeCents = moneyReceived.ToCurrency(authData.CurrentOrganization.Currency).Cents;
                    FinancialTransaction ledgerTx1 = FinancialTransaction.Create(authData.CurrentOrganization,
                        DateTime.UtcNow, tx1Description);
                    ledgerTx1.AddRow(authData.CurrentOrganization.FinancialAccounts.DebtsOther, -orgNativeCents, authData.CurrentUser);
                    ledgerTx1.AddRow(authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot, orgNativeCents, authData.CurrentUser).AmountForeignCents = new Swarmops.Logic.Financial.Money(satoshisReceived, Currency.BitcoinCash);
                    ledgerTx1.BlockchainHash = txHash;

                    // The second transaction is logged when executed in the back-end order

                    successMessage = string.Format(Resources.Pages.Admin.BitcoinEchoTest_FundsReceived,
                        authData.CurrentOrganization.Currency.DisplayCode, orgNativeCents/100.0, satoshisReceived/100.0);
                }

                return new AjaxCallResult() {DisplayMessage = successMessage, Success = true};

                // TODO: Ack donation via mail?
                // TODO: Notify CFO/etc of donation?
            }

            return new AjaxCallResult() {Success = false};
        }

        public string Localized_Verifying
        {
            get { return CommonV5.JavascriptEscape(Resources.Pages.Financial.Donate_TransactionDetected); }
        }

        public string Localized_DustCollected
        {
            get
            {
                return
                    CommonV5.JavascriptEscape(String.Format(Resources.Pages.Admin.BitcoinEchoTest_DustCollected,
                        BitcoinUtility.EchoFeeSatoshis/100.0)); 
            }
        }

    }
}