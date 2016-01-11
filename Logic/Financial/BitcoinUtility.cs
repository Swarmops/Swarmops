using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using NBitcoin;
using NBitcoin.BouncyCastle.Asn1.Ocsp;
using Newtonsoft.Json.Linq;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using NUnit.Framework;
using Satoshis = NBitcoin.Money;


namespace Swarmops.Logic.Financial
{
    public class BitcoinUtility
    {
        public static void VerifyBitcoinHotWallet()
        {
            // This must only be run from the backend

            if (HttpContext.Current != null)
            {
                throw new InvalidOperationException ("Checking root keys cannot be done from the frontend");
            }

            // Make sure there's always a private hotwallet root, regardless of whether it's used or not

            if (!File.Exists (SystemSettings.EtcFolder + Path.DirectorySeparatorChar + "hotwallet"))
            {
                ExtKey privateRoot = new ExtKey();
                File.WriteAllText (SystemSettings.EtcFolder + Path.DirectorySeparatorChar + "hotwallet",
                    privateRoot.GetWif (Network.Main).ToWif(), Encoding.ASCII);
                File.WriteAllText (
                    SystemSettings.EtcFolder + Path.DirectorySeparatorChar + "hotwallet-created-" +
                    DateTime.UtcNow.ToString ("yyyy-MM-dd--HH-mm-ss--fff.backup"),
                    privateRoot.GetWif (Network.Main).ToWif(), Encoding.ASCII); // an extra backup

                if (String.IsNullOrEmpty (Persistence.Key["BitcoinHotPublicRoot"]))
                {
                    Persistence.Key["BitcoinHotPublicRoot"] = privateRoot.Neuter().GetWif (Network.Main).ToWif();
                }
            }
            else
            {
                // The file exists. Does the database have the hotwallet public root?

                if (Persistence.Key["BitcoinHotPublicRoot"].Length < 3)
                {
                    // No, it has disappeared, which can happen for a few bad reasons

                    Persistence.Key["BitcoinHotPublicRoot"] =
                        BitcoinHotPrivateRoot.Neuter().GetWif (Network.Main).ToWif();
                    if (!PilotInstallationIds.IsPilot (PilotInstallationIds.DevelopmentSandbox))
                    {
                        // TODO: Log some sort of exception (the sandbox db is reset every night, so it's ok to lose the public key from there)
                    }
                }

                // Is the hotwallet public root equal to the private root, while in production environment?

                // ReSharper disable once RedundantCheckBeforeAssignment
                if (Persistence.Key["BitcoinHotPublicRoot"] !=
                    BitcoinHotPrivateRoot.Neuter().GetWif (Network.Main).ToWif() && !Debugger.IsAttached)
                {
                    // SERIOUS CONDITION - the public root key did not match the private root key. This needs to be logged somewhere.
                    OutboundComm.CreateNotification (NotificationResource.System_PublicRootReset);

                    // Reset it
                    Persistence.Key["BitcoinHotPublicRoot"] =
                        BitcoinHotPrivateRoot.Neuter().GetWif (Network.Main).ToWif();
                }
            }
        }

        public static ExtKey BitcoinHotPrivateRoot
        {
            get
            {
                return
                    ExtKey.Parse (File.ReadAllText (SystemSettings.EtcFolder + Path.DirectorySeparatorChar + "hotwallet"));
                    // will throw if hotwallet file does not exist - intentional
            }
        }

        public static ExtPubKey BitcoinHotPublicRoot
        {
            get { return ExtPubKey.Parse ((string) Persistence.Key["BitcoinHotPublicRoot"]); }
        }

        public static string[] GetInputAddressesForTransaction (string txHash)
        {
            List<string> inputAddresses = new List<string>();
            var jsonResult =
                JObject.Parse (
                    new WebClient().DownloadString ("https://blockchain.info/rawtx/" + txHash + "?format=json&api_key=" +
                                                    SystemSettings.BlockchainSwarmopsApiKey));

            foreach (var input in jsonResult["inputs"])
            {
                inputAddresses.Add ((string) (input["prev_out"]["addr"]));
            }

            return inputAddresses.ToArray();
        }


        public static Coin[] GetSpendableCoin (BitcoinSecret secretKey)
            // we're using BitcoinSecret as arg just to reinforce caller must have privkey to spend funds
        {
            // This function queries the Blockchain API for the unspent coin.

            string addressString = secretKey.PubKey.GetAddress (Network.Main).ToString();
            var unspentJsonResult =
                JObject.Parse (
                    new WebClient().DownloadString ("https://blockchain.info/unspent?active=" + addressString +
                                                    "&api_key=" + SystemSettings.BlockchainSwarmopsApiKey));

            List<Coin> coinList = new List<Coin>();

            foreach (var unspentJson in unspentJsonResult["unspent_outputs"])
            {
                BitcoinUnspentTransactionOutput txUnspent = new BitcoinUnspentTransactionOutput()
                {
                    BitcoinAddress = addressString,
                    ConfirmationCount = (UInt32) unspentJson["confirmations"],
                    Satoshis = (UInt64) unspentJson["value"],
                    TransactionHash = (string) unspentJson["tx_hash_big_endian"],
                    TransactionOutputIndex = (UInt32) unspentJson["tx_output_n"]
                };

                coinList.Add (txUnspent); // invokes implicit conversion to NBitcoin.Coin
            }

            return coinList.ToArray();
        }

        // TODO: Condense TestUnspents into ONE call for MULTIPLE addresses (separated by | for Unspents according to API docs)

        public static bool TestUnspents (string address)
        {
            // This function queries the Blockchain API for the unspent coin.
            bool result = false;
            HotBitcoinAddress hotAddress = null;

            var addressInfoResult =
                JObject.Parse (
                    new WebClient().DownloadString (
                        "https://blockchain.info/address/" + address + "?format=json&api_key=" +
                        SystemSettings.BlockchainSwarmopsApiKey));

            if ((int) addressInfoResult["final_balance"] == 0)
            {
                return false; // no funds on address at all at this time
            }

            var unspentJsonResult =
                JObject.Parse (
                    new WebClient().DownloadString ("https://blockchain.info/unspent?active=" + address + "&api_key=" +
                                                    SystemSettings.BlockchainSwarmopsApiKey));
                // warn: will 500 if no unspent outpoints

            foreach (var unspentJson in unspentJsonResult["unspent_outputs"])
            {
                BitcoinUnspentTransactionOutput txUnspent = new BitcoinUnspentTransactionOutput()
                {
                    BitcoinAddress = address,
                    ConfirmationCount = (UInt32) unspentJson["confirmations"],
                    Satoshis = (UInt64) unspentJson["value"],
                    TransactionHash = (string) unspentJson["tx_hash_big_endian"],
                    TransactionOutputIndex = (UInt32) unspentJson["tx_output_n"]
                };

                if (txUnspent.ConfirmationCount < 2)
                {
                    // Fresh transactions, return true
                    result = true;
                }

                // Add unspent to database

                if (hotAddress == null)
                {
                    hotAddress = HotBitcoinAddress.FromAddress (address);
                }

                SwarmDb.GetDatabaseForWriting()
                    .CreateHotBitcoinAddressUnspentConditional (hotAddress.Identity, txUnspent.TransactionHash,
                        (int) txUnspent.TransactionOutputIndex, (Int64) txUnspent.Satoshis,
                        (int) txUnspent.ConfirmationCount);
            }

            return result;
        }


        public static void BroadcastTransaction (Transaction transaction)
        {
            using (WebClient client = new WebClient())
            {
                byte[] response = client.UploadValues ("https://blockchain.info/pushtx?cors=true",
                    new NameValueCollection()
                    {
                        {"tx", transaction.ToHex()}
                    });

                // TODO: Exception handling
            }
        }

        public static BitcoinTransactionInputs GetInputsForAmount (Organization organization, Int64 satoshis)
        {
            // TODO: Verify inputs up to date?

            // TODO: What's the most efficient way to do this?

            HotBitcoinAddresses addresses = HotBitcoinAddresses.ForOrganization (organization);
            HotBitcoinAddressUnspents unspents = addresses.Unspents;

            BitcoinTransactionInputs result = new BitcoinTransactionInputs();

            // Lazy checking: if there's one single input that covers the entire amount, use it

            BitcoinTransactionInput lowestSingleSufficientInput = null;

            foreach (HotBitcoinAddressUnspent unspent in unspents)
            {
                if (unspent.AmountSatoshis >= satoshis)
                {
                    if (lowestSingleSufficientInput == null ||
                        lowestSingleSufficientInput.AmountSatoshis > unspent.AmountSatoshis)
                    {
                        lowestSingleSufficientInput = unspent.AsInput;
                    }
                }
            }

            if (lowestSingleSufficientInput != null)
            {
                // There is a single input that will cover the entire amount, so return it

                return BitcoinTransactionInputs.FromSingle (lowestSingleSufficientInput);
            }

            throw new NotEnoughFundsException();

            // Slightly more complex checking (TODO)
        }


        public static FinancialTransaction GetSwarmopsTransactionFromBlockchainId (string transactionId)
        {
            return null;
        }


        public static void CheckColdStorageForOrganization (Organization organization)
        {
            FinancialAccount coldRoot = organization.FinancialAccounts.AssetsBitcoinCold;

            if (coldRoot == null)
            {
                return; // no cold assets to check
            }
        }

        private static void CheckColdStorageRecurse (FinancialAccount parent)
        {
            foreach (FinancialAccount child in parent.Children)
            {
                CheckColdStorageRecurse (child);
            }

            // After recursing, get all transactions for this account and verify against our records

            // is the account name a valid bitcoin address on the main network?

            string address = parent.Name.Trim();

            try
            {
                BitcoinAddress addressTest = new BitcoinAddress (BitcoinTestAddress, Network.Main);
            }
            catch (Exception)
            {
                // the name wasn't an address, so return

                return;
            }

            var addressData =
                JObject.Parse (
                    new WebClient().DownloadString ("https://blockchain.info/address/" + address +
                                                    "?format=json&api_key=" +
                                                    SystemSettings.BlockchainSwarmopsApiKey));
            // warn: will 500 if no unspent outpoints

            int transactionCount = (int) (addressData["n_tx"]);

            foreach (var tx in addressData["txs"])
            {
                string blockchainTransactionId = (string) tx["hash"];
                FinancialTransaction ourTx = null;

                try
                {
                    ourTx = FinancialTransaction.FromBlockchainHash (parent.Organization, blockchainTransactionId);
                    // If the transaction was fetched fine, we have already seen this transaction, but need to re-check it
                }
                catch (ArgumentException)
                {
                    // We didn't have this transaction, so we need to create it

                    // ourTx = FinancialTransaction.Create (parent.Organization, DateTime, Description)
                    // Did we lose or gain money?

                    // Find all in- and outpoints, determine which are ours (hot and cold wallet) and which aren't
                }
            }

            // TODO: PARSE (awaiting functions for native currency to be added to FinancialTransactionRows)
            // TODO: CONTINUE HERE
        }

        public const int BitcoinWalletIndex = 1;
        public const int BitcoinDonationsIndex = 2;
        public const int BitcoinLoansIndex = 3;
        public const int BitcoinAccountsReceivableIndex = 4;

        public const string BitcoinTestAddress = "1JMpU3D6c5sruunMwzkt6p6PQzLcUYcL26";

        public const long FeeSatoshisPerThousandBytes = 20000;
            // This is twice the standardized amount - needed until blocksize debacle sorts out



        public static bool IsValidBitcoinAddress (string address)
        {
            try
            {
                BitcoinAddress singleAddress = new BitcoinAddress (address, Network.Main);
                return true;
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
                // ignore for now; move on to next possible case
            }

            try
            {
                BitcoinScriptAddress multiSigAddress = new BitcoinScriptAddress (address, Network.Main);
                return true;
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }

            return false; // not a valid address, neither single nor multisig
        }



        public static void TestMultisigPayout()
        {
            throw new InvalidOperationException("This function is only for testing purposes. It pays real money. Don't use except for dev/test.");

            // disable "code unreachable" warning for this code
            // ReSharper disable once CSharpWarnings::CS0162
            #pragma warning disable 162
            Organization organization = Organization.Sandbox; // a few testing cents here in test environment

            string bitcoinTestAddress = "3KS6AuQbZARSvqnaHoHfL1Xhm3bTLFAzoK";

            // Make a small test payment to a multisig address

            TransactionBuilder txBuilder = new TransactionBuilder();
            Int64 satoshis = new Money(100, Currency.FromCode ("SEK")).ToCurrency (Currency.Bitcoin).Cents;
            BitcoinTransactionInputs inputs = null;
            Int64 satoshisMaximumAnticipatedFees = BitcoinUtility.FeeSatoshisPerThousandBytes * 20; // assume max 20k transaction size

            try
            {
                inputs = BitcoinUtility.GetInputsForAmount(organization, satoshis + satoshisMaximumAnticipatedFees);
            }
            catch (NotEnoughFundsException)
            {
                Debugger.Break();
            }

            // If we arrive at this point, the previous function didn't throw, and we have enough money. Add the inputs to the transaction.

            txBuilder = txBuilder.AddCoins(inputs.Coins);
            txBuilder = txBuilder.AddKeys(inputs.PrivateKeys);
            Int64 satoshisInput = inputs.AmountSatoshisTotal;

            // Add outputs and prepare notifications

            Int64 satoshisUsed = 0;
            Dictionary<int, List<string>> notificationSpecLookup = new Dictionary<int, List<string>>();
            Dictionary<int, List<Int64>> notificationAmountLookup = new Dictionary<int, List<Int64>>();
            Payout masterPayoutPrototype = Payout.Empty;
            HotBitcoinAddress changeAddress = HotBitcoinAddress.OrganizationWalletZero(organization);

            // Add the test payment

            if (bitcoinTestAddress.StartsWith("1")) // regular address
            {
                txBuilder = txBuilder.Send(new BitcoinAddress(bitcoinTestAddress),
                    new Satoshis(satoshis));
            }
            else if (bitcoinTestAddress.StartsWith("3")) // multisig
            {
                txBuilder = txBuilder.Send(new BitcoinScriptAddress(bitcoinTestAddress, Network.Main),
                    new Satoshis(satoshis));
            }
            else
            {
                throw new InvalidOperationException("Unhandled address case");
            }
            satoshisUsed += satoshis;

            // Set change address to wallet slush

            txBuilder.SetChange(new BitcoinAddress(changeAddress.Address));

            // Add fee

            int transactionSizeBytes = txBuilder.EstimateSize(txBuilder.BuildTransaction(false)) + inputs.Count;
            // +inputs.Count for size variability

            Int64 feeSatoshis = (transactionSizeBytes / 1000 + 1) * BitcoinUtility.FeeSatoshisPerThousandBytes;

            txBuilder = txBuilder.SendFees(new Satoshis(feeSatoshis));
            satoshisUsed += feeSatoshis;

            // Sign transaction - ready to execute

            Transaction txReady = txBuilder.BuildTransaction(true);

            // Verify that transaction is ready

            if (!txBuilder.Verify(txReady))
            {
                // Transaction was not signed with the correct keys. This is a serious condition.

                NotificationStrings primaryStrings = new NotificationStrings();
                primaryStrings[NotificationString.OrganizationName] = organization.Name;

                OutboundComm.CreateNotification(organization, NotificationResource.Bitcoin_PrivateKeyError,
                    primaryStrings);

                throw new InvalidOperationException("Transaction is not signed enough");
            }

            // Broadcast transaction

            BitcoinUtility.BroadcastTransaction(txReady);

            // Note the transaction hash

            string transactionHash = txReady.GetHash().ToString();

            // Delete all old inputs, adjust balance for addresses (re-register unused inputs)

            inputs.AsUnspents.DeleteAll();

            // Log the new unspent created by change (if there is any)

            if (satoshisInput - satoshisUsed > 0)
            {
                SwarmDb.GetDatabaseForWriting()
                    .CreateHotBitcoinAddressUnspentConditional(changeAddress.Identity, transactionHash,
                        + /* the change address seems to always get index 0? is this a safe assumption? */ 0, satoshisInput - satoshisUsed, /* confirmation count*/ 0);
            }

            // Restore "code unreachable" warnings
            #pragma warning restore 162

            // This puts the ledger out of sync, so only do this on Sandbox for various small-change (cents) testing
        }


        internal class BitcoinUnspentTransactionOutput
        {
            public UInt64 Satoshis { get; set; }
            public string TransactionHash { get; set; }
            public UInt32 TransactionOutputIndex { get; set; }
            public string BitcoinAddress { get; set; }
            public UInt32 ConfirmationCount { get; set; }

            public static implicit operator NBitcoin.Coin (BitcoinUnspentTransactionOutput source)
            {
                return
                    new Coin (
                        new OutPoint()
                        {
                            Hash = uint256.Parse (source.TransactionHash),
                            N = source.TransactionOutputIndex
                        },
                        new TxOut (new NBitcoin.Money (source.Satoshis),
                            new BitcoinAddress (source.BitcoinAddress, Network.Main)));
            }
        }
    }
}
