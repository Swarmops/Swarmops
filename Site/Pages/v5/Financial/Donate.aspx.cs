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
using Newtonsoft.Json.Linq;
using Swarmops.Common.Enums;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Satoshis = NBitcoin.Money; // Sets it apart from Swarmops' Money class
using WebSocketSharp;

namespace Swarmops.Frontend.Pages.v5.Financial
{
    public partial class Donate : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access (this.CurrentOrganization, AccessAspect.Participant);
            this.IsDashboard = true; // prevents encaps and enables odometer

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
            
            this.PageTitle = Resources.Pages.Financial.Donate_PageTitle;
            this.InfoBoxLiteral = Resources.Pages.Financial.Donate_Info;
            this.LabelStatus.Text = Resources.Pages.Financial.Donate_StatusInitial;
            this.SuppressStatisticHeaders = true;

            if (this.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot == null)
            {
                this.PanelDisabled.Visible = true;
                this.PanelEnabled.Visible = false;
            }

            HotBitcoinAddress address = HotBitcoinAddress.Create (this.CurrentOrganization, BitcoinChain.Cash,
                BitcoinUtility.BitcoinDonationsIndex, this.CurrentUser.Identity);

            this.BitcoinCashAddressUsed = address.Address;
            string guid = Guid.NewGuid().ToString ("N");
            GuidCache.Set (guid, address.Address);
            this.TransactionGuid = guid;

            // Calculate conversion rate (satoshi-cents to unit-cents, so we're good, even if the conversion rate
            // is calculated on microbitcoin to whole units)

            this.ConversionRateSatoshisToCents = Currency.BitcoinCash.GetConversionRate(CurrentOrganization.Currency);

            // Add subscription to address
            /*    --- RETIRED CODE -- THIS WAS NOT RELIABLE -- DONE CLIENT SIDE INSTEAD
            using (
                WebSocket socket =
                    new WebSocket("ws://localhost:" + SystemSettings.WebsocketPortFrontend + "/Front?Auth=" +
                                  Uri.EscapeDataString(this.CurrentAuthority.ToEncryptedXml())))
            {
                socket.Connect();

                JObject data = new JObject();
                data ["ServerRequest"] = "AddBitcoinAddress";
                data["Address"] = address.Address;
                socket.Send(data.ToString());
                socket.Ping(); // wait a little little while for send to work
                socket.Close();
            }*/

            this.BoxTitle.Text = Resources.Pages.Financial.Donate_PageTitle;
            this.LabelExplainBitcoinDonation.Text = String.Format (Resources.Pages.Financial.Donate_Explain,
                CurrentOrganization.Name, address.Address);
            this.LabelReceivedFunds.Text = String.Format(Resources.Pages.Financial.Donate_FundsReceivedLabel,
                CurrentOrganization.Currency.DisplayCode);

            this.ImageBitcoinQr.ImageUrl =
                "https://chart.googleapis.com/chart?cht=qr&chs=400x400&chl=bitcoincash:" +
                HttpUtility.UrlEncode (address.Address + "?label=" +
                                       Uri.EscapeDataString (String.Format (Resources.Pages.Financial.Donate_TxLabel,
                                           CurrentOrganization.Name))); // URI scheme doesn't like &, =
        }

        public string BitcoinCashAddressUsed { get; private set; }
        public string TransactionGuid { get; private set; }
        public double ConversionRateSatoshisToCents { get; private set; }

        [WebMethod]
        static public AjaxCallResult ProcessTransactionReceived (string guid, string txHash)
        {
            BitcoinChain chain = BitcoinChain.Cash;

            AuthenticationData authData = GetAuthenticationDataAndCulture(); // just to make sure we're called properly

            string bitcoinAddress = (string) GuidCache.Get (guid);
            if (BitcoinUtility.TestUnspents (chain, bitcoinAddress))
            {
                HotBitcoinAddressUnspents unspents = HotBitcoinAddress.FromAddress (chain, bitcoinAddress).Unspents;

                // TODO: Update the HotBitcoinAddress with the new amount?

                HotBitcoinAddressUnspent unspent = null;
                Int64 satoshisReceived = 0;

                foreach (HotBitcoinAddressUnspent potentialUnspent in unspents)
                {
                    if (potentialUnspent.TransactionHash == txHash)
                    {
                        satoshisReceived = potentialUnspent.AmountSatoshis;
                        unspent = potentialUnspent;
                    }
                }

                if (unspent == null)  // Supplied transaction hash was not found in collection
                {
                    Debugger.Break();   // TODO: Something else than break the debugger
                }

                Swarmops.Logic.Financial.Money moneyReceived = new Swarmops.Logic.Financial.Money(satoshisReceived,
                    Currency.BitcoinCash);

                // Make sure that the hotwallet native currency is bitcoin cash
                authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot.ForeignCurrency = Currency.BitcoinCash;

                // Create success message and ledger transaction
                string successMessage = string.Empty;

                FinancialTransaction testTransaction = null;
                try
                {
                    testTransaction = FinancialTransaction.FromBlockchainHash (authData.CurrentOrganization, txHash);

                    // We've already seen this donation! Something is seriously bogus here
                    Debugger.Break();
                    return new AjaxCallResult() { DisplayMessage = successMessage, Success = true };
                }
                catch (ArgumentException)
                {
                    // This exception is expected - the transaction should not yet exist
                }

                if (authData.CurrentOrganization.Currency.IsBitcoinCash)
                {
                    // The ledger is native bitcoin cash, so units are Satoshis 

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

                    long orgNativeCents = moneyReceived.ToCurrency (authData.CurrentOrganization.Currency).Cents;
                    FinancialTransaction ledgerTx = FinancialTransaction.Create(authData.CurrentOrganization,
                        DateTime.UtcNow, "Donation (bitcoin to hotwallet)");
                    ledgerTx.AddRow(authData.CurrentOrganization.FinancialAccounts.IncomeDonations, -orgNativeCents, authData.CurrentUser);
                    ledgerTx.AddRow(authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot, orgNativeCents, authData.CurrentUser).AmountForeignCents = new Swarmops.Logic.Financial.Money(satoshisReceived, Currency.BitcoinCash);
                    ledgerTx.BlockchainHash = txHash;

                    successMessage = string.Format (Resources.Pages.Financial.Donate_FundsReceived,
                        authData.CurrentOrganization.Currency.DisplayCode, orgNativeCents/100.0, satoshisReceived/100.0);
                }

                return new AjaxCallResult() {DisplayMessage = successMessage, Success = true};

                // TODO: Ack donation via mail?
                // TODO: Notify CFO/etc of donation?
            }

            return new AjaxCallResult() {Success = false};
        }
    }
}