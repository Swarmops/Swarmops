using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NBitcoin;
using NBitcoin.BouncyCastle.Asn1.Ocsp;
using Newtonsoft.Json.Linq;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class BitcoinUtility
    {
        public static void VerifyBitcoinHotWallet()
        {
            // This must only be run from the backend

            if (HttpContext.Current != null)
            {
                throw new InvalidOperationException("Checking root keys cannot be done from the frontend");
            }

            // Make sure there's always a private hotwallet root, regardless of whether it's used or not

            if (!File.Exists(SystemSettings.EtcFolder + Path.DirectorySeparatorChar + "hotwallet"))
            {
                ExtKey privateRoot = new ExtKey();
                File.WriteAllText(SystemSettings.EtcFolder + Path.DirectorySeparatorChar + "hotwallet", privateRoot.GetWif(Network.Main).ToWif(), Encoding.ASCII);
                File.WriteAllText(SystemSettings.EtcFolder + Path.DirectorySeparatorChar + "hotwallet-created-" + DateTime.UtcNow.ToString("yyyy-MM-dd--HH-mm-ss--fff.backup"), privateRoot.GetWif(Network.Main).ToWif(), Encoding.ASCII); // an extra backup
                Persistence.Key["BitcoinHotPublicRoot"] = privateRoot.Neuter().GetWif(Network.Main).ToWif();
            }
            else
            {
                // The file exists. Does the database have the hotwallet public root?

                if (Persistence.Key["BitcoinHotPublicRoot"].Length < 3)
                {
                    // No, it has disappeared, which can happen for a few bad reasons

                    Persistence.Key["BitcoinHotPublicRoot"] = BitcoinHotPrivateRoot.Neuter().GetWif(Network.Main).ToWif();
                    if (!PilotInstallationIds.IsPilot (PilotInstallationIds.DevelopmentSandbox))
                    {
                        // TODO: Log some sort of exception (the sandbox db is reset every night, so it's ok to lose the public key from there)
                    }
                }
            }
        }

        public static ExtKey BitcoinHotPrivateRoot
        {
            get
            {
                return
                    ExtKey.Parse(File.ReadAllText(SystemSettings.EtcFolder + Path.DirectorySeparatorChar + "hotwallet")); // will throw if hotwallet file does not exist - intentional
            }
        }

        public static ExtPubKey BitcoinHotPublicRoot
        {
            get
            {
                return ExtPubKey.Parse((string)Persistence.Key["BitcoinHotPublicRoot"]);
            }
        }

        static public Coin[] GetSpendableCoin (BitcoinSecret secretKey) // we're using BitcoinSecret as arg just to reinforce caller must have privkey to spend funds
        {
            // This function queries the Blockchain API for the unspent coin.

            string addressString = secretKey.PubKey.GetAddress (Network.Main).ToString();
            var unspentJsonResult =
                JObject.Parse(new WebClient().DownloadString("https://blockchain.info/unspent?active=" + addressString + "&api_key=" + SystemSettings.BlockchainSwarmopsApiKey));

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

        static public bool TestUnspents (string address)
        {
            // This function queries the Blockchain API for the unspent coin.
            bool result = false;
            HotBitcoinAddress hotAddress = null;

            var addressInfoResult =
                JObject.Parse (
                    new WebClient().DownloadString (
                        "https://blockchain.info/address/" + address + "?format=json&api_key=" + SystemSettings.BlockchainSwarmopsApiKey));

            if ((int) addressInfoResult["final_balance"] == 0)
            {
                return false; // no funds on address at all at this time
            }

            var unspentJsonResult =
                JObject.Parse (
                    new WebClient().DownloadString ("https://blockchain.info/unspent?active=" + address + "&api_key=" +
                                                    SystemSettings.BlockchainSwarmopsApiKey));   // warn: will 500 if no unspent outpoints

            foreach (var unspentJson in unspentJsonResult["unspent_outputs"])
            {
                BitcoinUnspentTransactionOutput txUnspent = new BitcoinUnspentTransactionOutput()
                {
                    BitcoinAddress = address,
                    ConfirmationCount = (UInt32)unspentJson["confirmations"],
                    Satoshis = (UInt64)unspentJson["value"],
                    TransactionHash = (string)unspentJson["tx_hash_big_endian"],
                    TransactionOutputIndex = (UInt32)unspentJson["tx_output_n"]
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
                        (int) txUnspent.TransactionOutputIndex, (Int64) txUnspent.Satoshis, (int) txUnspent.ConfirmationCount);
            }

            return result;
        }


        static public void BroadcastTransaction (Transaction transaction)
        {
            using (WebClient client = new WebClient())
            {
                byte[] response = client.UploadValues("https://blockchain.info/pushtx?cors=true", new NameValueCollection()
                {
                    { "tx", transaction.ToHex() }
                });

                // TODO: Exception handling
            }
        }

        static public BitcoinTransactionInputs GetInputsForAmount (Organization organization, Int64 satoshis)
        {
            // TODO: Verify inputs up to date?

            // TODO: What's the most efficient way to do this?

            HotBitcoinAddresses addresses = HotBitcoinAddresses.ForOrganization (organization);

            return addresses.FindAmount (satoshis); // Will throw on a number of circumstances
        }


        static public FinancialTransaction GetSwarmopsTransactionFromBlockchainId (string transactionId)
        {
            return null;
        }


        static public void CheckColdStorageForOrganization (Organization organization)
        {
            FinancialAccount coldRoot = organization.FinancialAccounts.AssetsBitcoinCold;

            if (coldRoot == null)
            {
                return; // no cold assets to check
            }
        }

        // https://blockchain.info/address/1Axd4QQhRSCMAuHe93uVKRjNSFj6WypcZa?format=json&api_key=I_am_testing_the_format_please_ignore

        static private void CheckColdStorageRecurse (FinancialAccount parent)
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
            catch (Exception e)
            {
                // the name wasn't an address, so return
            }

            var addressData =
                JObject.Parse(
                    new WebClient().DownloadString("https://blockchain.info/address/" + address + "?format=json&api_key=" +
                                                    SystemSettings.BlockchainSwarmopsApiKey));   // warn: will 500 if no unspent outpoints

            // TODO: PARSE (awaiting functions for native currency to be added to FinancialTransactionRows)

        }


        public const int BitcoinWalletIndex = 1;
        public const int BitcoinDonationsIndex = 2;
        public const int BitcoinLoansIndex = 3;
        public const int BitcoinAccountsReceivableIndex = 4;

        public const string BitcoinTestAddress = "1JMpU3D6c5sruunMwzkt6p6PQzLcUYcL26";

        public const long FeeSatoshisPerThousandBytes = 10000;
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
            return new Coin(new OutPoint() { Hash= uint256.Parse (source.TransactionHash), N=source.TransactionOutputIndex }, new TxOut(new NBitcoin.Money (source.Satoshis), new BitcoinAddress (source.BitcoinAddress, Network.Main) ));
        }
    }
}
