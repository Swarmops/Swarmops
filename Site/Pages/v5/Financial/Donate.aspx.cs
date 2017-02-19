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

            if (this.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot == null)
            {
                this.PanelDisabled.Visible = true;
                this.PanelEnabled.Visible = false;
            }

            HotBitcoinAddress address = HotBitcoinAddress.Create (this.CurrentOrganization,
                BitcoinUtility.BitcoinDonationsIndex, this.CurrentUser.Identity);

            this.LiteralBitcoinAddress.Text = address.Address;
            string guid = Guid.NewGuid().ToString ("N");
            GuidCache.Set (guid, address.Address);
            this.LiteralGuid.Text = guid;

            // Add subscription to address

            using (
                WebSocket socket =
                    new WebSocket("ws:/localhost:" + SystemSettings.WebsocketPortFrontend + "?Auth=" +
                                  Uri.EscapeDataString(this.CurrentAuthority.ToEncryptedXml())))
            {
                socket.Connect();

                JObject data = new JObject();
                data ["ServerRequest"] = "AddBitcoinAddress";
                data["Address"] = address.Address;

                socket.Send(data.ToString());
                socket.Close();
            }

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
        static public AjaxCallResult ProcessTransactionReceived (string guid, string txHash)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture(); // just to make sure we're called properly

            string bitcoinAddress = (string) GuidCache.Get (guid);
            if (BitcoinUtility.TestUnspents (bitcoinAddress))
            {
                HotBitcoinAddressUnspents unspents = HotBitcoinAddress.FromAddress (bitcoinAddress).Unspents;

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
                    Debugger.Break();
                }

                Swarmops.Logic.Financial.Money moneyReceived = new Swarmops.Logic.Financial.Money(satoshisReceived,
                    Currency.Bitcoin);

                // Make sure that the hotwallet native currency is bitcoin
                authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot.ForeignCurrency = Currency.Bitcoin;

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

                    long orgNativeCents = moneyReceived.ToCurrency (authData.CurrentOrganization.Currency).Cents;
                    FinancialTransaction ledgerTx = FinancialTransaction.Create(authData.CurrentOrganization,
                        DateTime.UtcNow, "Donation (bitcoin to hotwallet)");
                    ledgerTx.AddRow(authData.CurrentOrganization.FinancialAccounts.IncomeDonations, -orgNativeCents, authData.CurrentUser);
                    ledgerTx.AddRow(authData.CurrentOrganization.FinancialAccounts.AssetsBitcoinHot, orgNativeCents, authData.CurrentUser).AmountForeignCents = new Swarmops.Logic.Financial.Money(satoshisReceived, Currency.Bitcoin);
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