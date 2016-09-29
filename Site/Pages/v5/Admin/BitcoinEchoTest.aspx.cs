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
using Swarmops.Logic.Cache;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Satoshis = NBitcoin.Money; // Sets it apart from Swarmops' Money class

namespace Swarmops.Frontend.Pages.v5.Admin
{
    public partial class BitcoinEchoTest : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access (this.CurrentOrganization, AccessAspect.Administration);

            // This page is copied from Donate, with the echo payout added to the end


            /* TEMP TEMP TEMP - REMOVE THIS CODE */
            /*
            Organization fwn = Organization.FromIdentity (2);

            Salaries salaries = Salaries.ForOrganization (fwn);
            foreach (Salary salary in salaries)
            {
                if (salary.PayrollItem.Person.BitcoinPayoutAddress.Length > 0 && salary.Attested == false)
                {
                    salary.Attest (salary.PayrollItem.Person); // null for system apparently isn't allowed here
                }
            }*/

            this.PageTitle = Resources.Pages.Admin.BitcoinEchoTest_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Admin.BitcoinEchoTest_Info;
            this.LabelStatus.Text = Resources.Pages.Admin.BitcoinEchoTest_StatusInitial;
            this.LiteralTxDetected.Text = JavascriptEscape (Resources.Pages.Admin.BitcoinEchoTest_TransactionDetected);

            if (this.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot == null)
            {
                this.PanelDisabled.Visible = true;
                this.PanelEnabled.Visible = false;
                this.LiteralEnable.Text = @"false";
            }

            HotBitcoinAddress address = HotBitcoinAddress.Create (this.CurrentOrganization,
                BitcoinUtility.BitcoinDonationsIndex, this.CurrentUser.Identity);

            this.LiteralBitcoinAddress.Text = address.Address;
            string guid = Guid.NewGuid().ToString ("N");
            GuidCache.Set (guid, address.Address);
            this.LiteralGuid.Text = guid;

            // TEST TEST TEST
            /*
            BitcoinSecret secretKey = address.PrivateKey;
            Coin[] spendableCoin = BitcoinUtility.GetSpendableCoin (secretKey);

            TransactionBuilder txBuild = new TransactionBuilder();
            Transaction tx = txBuild.AddCoins (spendableCoin)
                .AddKeys (secretKey)
                .Send (new BitcoinAddress (BitcoinUtility.BitcoinTestAddress), new Satoshis (address.UnspentSatoshis - BitcoinUtility.FeeSatoshisPerThousandBytes))
                .SendFees (new Satoshis(BitcoinUtility.FeeSatoshisPerThousandBytes))
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

                // TODO: Update the HotBitcoinAddress with the new amount?

                Int64 satoshisReceived = unspents.Last().AmountSatoshis;

                if (unspents.Last().TransactionHash != txHash && txHash.Length > 0)
                {
                    // Race condition.
                    Debugger.Break();
                }

                Swarmops.Logic.Financial.Money moneyReceived = new Swarmops.Logic.Financial.Money(satoshisReceived,
                    Currency.Bitcoin);

                // Make sure that the hotwallet native currency is bitcoin
                authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot.ForeignCurrency = Currency.Bitcoin;

                // Create success message and ledger transaction
                string successMessage = string.Empty;

                // TODO: Get the tx, get the input

                string returnAddress = BitcoinUtility.GetInputAddressesForTransaction(txHash) [0]; // assumes at least one input address

                if (authData.CurrentOrganization.Currency.IsBitcoin)
                {
                    // The ledger is native bitcoin, so units are Satoshis 

                    FinancialTransaction ledgerTx = FinancialTransaction.Create (authData.CurrentOrganization,
                        DateTime.UtcNow, "Bitcoin echo test (will be repaid immediately)");
                    ledgerTx.AddRow (authData.CurrentOrganization.FinancialAccounts.DebtsOther, -satoshisReceived, authData.CurrentUser);
                    ledgerTx.AddRow (authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot, satoshisReceived, authData.CurrentUser);
                    ledgerTx.BlockchainHash = txHash;

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

                    // TODO: Log the payout back, as an inbound invoice for immediate payout 

                }
                else
                {
                    // The ledger is NOT native bitcoin, so we'll need to convert currencies

                    long orgNativeCents = moneyReceived.ToCurrency (authData.CurrentOrganization.Currency).Cents;
                    FinancialTransaction ledgerTx = FinancialTransaction.Create(authData.CurrentOrganization,
                        DateTime.UtcNow, "Bitcoin echo test (will be repaid immediately)");
                    ledgerTx.AddRow(authData.CurrentOrganization.FinancialAccounts.DebtsOther, -orgNativeCents, authData.CurrentUser);
                    ledgerTx.AddRow(authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot, orgNativeCents, authData.CurrentUser).AmountForeignCents = new Swarmops.Logic.Financial.Money(satoshisReceived, Currency.Bitcoin);
                    ledgerTx.BlockchainHash = txHash;

                    successMessage = string.Format(Resources.Pages.Admin.BitcoinEchoTest_FundsReceived,
                        authData.CurrentOrganization.Currency.DisplayCode, orgNativeCents/100.0, satoshisReceived/100.0);

                    // TODO: Create a payout back for this amount -- needs to be specified in bitcoin -- as an inbound invoice
                }

                return new AjaxCallResult() {DisplayMessage = successMessage, Success = true};

                // TODO: Ack donation via mail?
                // TODO: Notify CFO/etc of donation?
            }

            return new AjaxCallResult() {Success = false};
        }
    }
}