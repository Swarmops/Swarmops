using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using NBitcoin;
using Newtonsoft.Json.Linq;
using Swarmops.Common;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Satoshis = NBitcoin.Money;


namespace Swarmops.Logic.Financial
{
    public class BitcoinUtility
    {
        static BitcoinUtility()
        {
            // Necessary applicatin-wide initialization

            NBitcoin.RandomUtils.Random = new NBitcoinSecureRandomizer();
        }


        /// <summary>
        /// Always an even number. If an odd number, the upgrade to the next even number
        /// is currently in progress, and all blockchain operations are halted.
        /// </summary>
        public static int ExpectedBlockchainCodeVersion => 2;

        public static bool CurrentlyUpgrading
        {
            get { return SystemSettings.BlockchainCodeVersion%2 == 1 ? true : false; }
        }

        public static void Upgrade(int fromVersion)
        {
            BitcoinBlockchainUpgrade.Upgrade(fromVersion);
        }


        public static void CheckHotwalletAddresses(Organization organization)
        {
            HotBitcoinAddresses addresses = HotBitcoinAddresses.ForOrganization(organization);

            foreach (HotBitcoinAddress address in addresses)
            {
                if (address.Chain == BitcoinChain.Core)
                {
                    // These shouldn't exist much, so make sure that we have the equivalent Cash address registered
                    try
                    {
                        HotBitcoinAddress.FromAddress(BitcoinChain.Cash, address.ProtocolLevelAddress);
                    }
                    catch (ArgumentException)
                    {
                        // We didn't have it, so create it
                        BitcoinUtility.TestUnspents(BitcoinChain.Cash, address.ProtocolLevelAddress);
                    }
                }
            }
        }


        public static Dictionary<BitcoinChain, Int64> GetHotwalletSatoshisPerChain(Organization organization)
        {
            Dictionary<BitcoinChain, Int64> satoshisTotalLookup = new Dictionary<BitcoinChain, long>();

            HotBitcoinAddresses addresses = HotBitcoinAddresses.ForOrganization(organization);

            foreach (HotBitcoinAddress address in addresses)
            {
                HotBitcoinAddressUnspents unspents = HotBitcoinAddressUnspents.ForAddress(address);
                Int64 satoshisUnspentAddress = 0;

                foreach (HotBitcoinAddressUnspent unspent in unspents)
                {
                    satoshisUnspentAddress += unspent.AmountSatoshis;
                }

                if (satoshisUnspentAddress > 0)
                {
                    if (!satoshisTotalLookup.ContainsKey(address.Chain))
                    {
                        satoshisTotalLookup[address.Chain] = 0;
                    }

                    satoshisTotalLookup[address.Chain] += satoshisUnspentAddress;
                }
            }

            return satoshisTotalLookup;
        }


        public static Money GetHotwalletValue (Organization organization)
        {
            Currency presentationCurrency = organization.Currency;

            Dictionary<BitcoinChain, double> conversionRateLookup = new Dictionary<BitcoinChain, double>();
            Dictionary<BitcoinChain, Int64> satoshisTotalLookup = new Dictionary<BitcoinChain, long>();

            long fiatCentsPerCoreCoin =
                new Money(BitcoinUtility.SatoshisPerBitcoin, Currency.BitcoinCore).ToCurrency(
                    presentationCurrency).Cents;
            long fiatCentsPerCashCoin =
                new Money(BitcoinUtility.SatoshisPerBitcoin, Currency.BitcoinCash).ToCurrency(
                    presentationCurrency).Cents;

            conversionRateLookup[BitcoinChain.Cash] = fiatCentsPerCashCoin/1.0/BitcoinUtility.SatoshisPerBitcoin;
                // the "/1.0" converts to double implicitly
            conversionRateLookup[BitcoinChain.Core] = fiatCentsPerCoreCoin/1.0/BitcoinUtility.SatoshisPerBitcoin;

            satoshisTotalLookup = GetHotwalletSatoshisPerChain(organization);

            Int64 presentationCurrencyCents = 0;

            foreach (BitcoinChain chain in satoshisTotalLookup.Keys)
            {
                presentationCurrencyCents += (Int64) (satoshisTotalLookup[chain]*conversionRateLookup[chain]); // some precision loss unavoidable here
            }

            return new Money(presentationCurrencyCents, presentationCurrency);
        }


        public static void CheckHotwalletAccountBalance(FinancialAccount hotWallet)
        {
            if (hotWallet == null)
            {
                return;
            }

            Dictionary<BitcoinChain, Int64> satoshisPerChain = GetHotwalletSatoshisPerChain(hotWallet.Organization);

            Int64 satoshisCash = 0;

            if (satoshisPerChain.ContainsKey(BitcoinChain.Core))
            {
                // There should not be any. Use Shapeshift to convert to Cash.

                // TODO

                return; // do not repeat not process at this time
            }
            else
            {
                if (satoshisPerChain.ContainsKey(BitcoinChain.Cash))
                {
                    satoshisCash = satoshisPerChain[BitcoinChain.Cash];
                }
            }

        }


        public static void CheckHotwalletForexProfitLoss(FinancialAccount hotWallet)
        {
            if (hotWallet != null)
            {
                hotWallet.LogForexProfitLoss(GetHotwalletValue(hotWallet.Organization));
            }
        }



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
                if (Debugger.IsAttached)
                {
                    // REPLACE BELOW WITH YOUR FAVORITE PRIVATE KEY FOR DEBUGGING

                    return
                        ExtKey.Parse(
                            "Write your favorite root private key here");
                }

                return
                    ExtKey.Parse (File.ReadAllText (SystemSettings.EtcFolder + Path.DirectorySeparatorChar + "hotwallet"));
                    // will throw if hotwallet file does not exist - intentional
            }
        }

        public static ExtPubKey BitcoinHotPublicRoot
        {
            get { return ExtPubKey.Parse ((string) Persistence.Key["BitcoinHotPublicRoot"]); }
        }

        public static string[] GetInputAddressesForTransaction (BitcoinChain chain, string txHash)
        {
            List<string> inputAddresses = new List<string>();

            JObject jsonResult = new JObject();

            switch (chain)
            {
                case BitcoinChain.Core:
                    jsonResult =
                        JObject.Parse(
                            new WebClient().DownloadString("https://blockchain.info/rawtx/" + txHash + "?format=json&api_key=" +
                                                            SystemSettings.BlockchainSwarmopsApiKey));
                    foreach (var input in jsonResult["inputs"])
                    {
                        inputAddresses.Add((string)(input["prev_out"]["addr"]));
                    }

                    break;

                case BitcoinChain.Cash:
                    jsonResult =
                        JObject.Parse(
                            new WebClient().DownloadString("https://bch-insight.bitpay.com/api/tx/" + txHash));
                    foreach (var input in jsonResult["vin"])
                    {
                        string address = ((string) (input["addr"]));
                        address = BitcoinUtility.EnsureLegacyAddress(address);

                        inputAddresses.Add(address);
                    }

                    break;
                default:
                    throw new NotImplementedException("Unimplemented chain, no Explorer: " + chain);
            }


            return inputAddresses.ToArray();
        }


        public static Coin[] GetSpendableCoin (BitcoinChain chain, BitcoinSecret secretKey)
            // we're using BitcoinSecret as arg just to reinforce caller must have privkey to spend funds
        {
            List<Coin> coinList = new List<Coin>();

            switch (chain)
            {
                case BitcoinChain.Core:

                    // For the Core net, we query the Blockchain API for the unspent coin.

                    string addressString = secretKey.PubKey.GetAddress(Network.Main).ToString();
                    var unspentJsonResult =
                        JObject.Parse(
                            new WebClient().DownloadString("https://blockchain.info/unspent?active=" + addressString +
                                                            "&api_key=" + SystemSettings.BlockchainSwarmopsApiKey));

                    foreach (var unspentJson in unspentJsonResult["unspent_outputs"])
                    {
                        BitcoinUnspentTransactionOutput txUnspent = new BitcoinUnspentTransactionOutput()
                        {
                            BitcoinAddress = addressString,
                            ConfirmationCount = (UInt32)unspentJson["confirmations"],
                            Satoshis = (UInt64)unspentJson["value"],
                            TransactionHash = (string)unspentJson["tx_hash_big_endian"],
                            TransactionOutputIndex = (UInt32)unspentJson["tx_output_n"]
                        };

                        coinList.Add(txUnspent); // invokes implicit conversion to NBitcoin.Coin
                    }

                    break;


                case BitcoinChain.Cash:

                    JArray unspentArray;

                    // TODO: SELECT BLOCK EXPLORER AND ITS ACCOMPANYING ADDRESS FORMAT

                    unspentArray = JArray.Parse(
                        new WebClient().DownloadString("https://bch-insight.bitpay.com/api/addr/" + EnsureCashAddressWithoutPrefix(secretKey.PubKey.GetAddress(Network.Main).ToString()) + "/utxo"));

                    foreach (JObject unspentJson in unspentArray.Children())
                    {


                        BitcoinUnspentTransactionOutput txUnspent = new BitcoinUnspentTransactionOutput()
                        {
                            BitcoinAddress = secretKey.PubKey.GetAddress(Network.Main).ToString(),
                            ConfirmationCount = (UInt32)unspentJson["confirmations"],
                            Satoshis = (UInt64)unspentJson["satoshis"],
                            TransactionHash = (string)unspentJson["txid"],
                            TransactionOutputIndex = (UInt32)unspentJson["vout"]
                        };

                        coinList.Add(txUnspent); // invokes implicit conversion to NBitcoin.Coin

                    }

                    break;

                default:
                    throw new NotImplementedException("Unimplemented chain identity: " + chain);
            }

            return coinList.ToArray();

        }

        // TODO: Condense TestUnspents into ONE call for MULTIPLE addresses (separated by | for Unspents according to API docs)

        // TODO: Enable backend to call a running bitcoin node for all this instead of callign third party services



        public static bool TestUnspents(BitcoinChain chain, string address)
        {
            // This function queries the Blockchain API for the unspent coin.
            bool result = false;
            HotBitcoinAddress hotAddress = null;
            JObject addressInfoResult;
            JObject unspentJsonResult;

            switch (chain)
            {
                case BitcoinChain.Core:

                    addressInfoResult =
                        JObject.Parse(
                            new WebClient().DownloadString(
                                "https://blockchain.info/address/" + address + "?format=json&api_key=" +
                                SystemSettings.BlockchainSwarmopsApiKey));

                    if ((int)addressInfoResult["final_balance"] == 0)
                    {
                        return false; // no funds on address at all at this time
                    }

                    try
                    {
                        unspentJsonResult = JObject.Parse(
                            new WebClient().DownloadString("https://blockchain.info/unspent?active=" + address + "&api_key=" +
                                                            SystemSettings.BlockchainSwarmopsApiKey));

                    }
                    catch (WebException webException)
                    {
                        // A 500 on the above _may_ mean that there's no unspent outpoints. It can also mean a data
                        // retrieval or network error, in which case the exception must absolutely not be interpreted
                        // as valid data of zero unspent outpoints.

                        try
                        {
                            if (webException.Response == null)
                            {
                                throw; // if there's no response at all, we can't do shit
                            }

                            string errorResponseContent =
                                new StreamReader(webException.Response.GetResponseStream()).ReadToEnd();

                            if (errorResponseContent.Trim().StartsWith("No free outputs to spend"))
                            {
                                // all is okay network-wise, there just aren't any UTXOs so we're getting an error code for that

                                return false; // no further processing and there are no fresh transactions
                            }

                            throw; // otherwise throw upward
                        }
                        catch (WebException)
                        {
                            // Ok, we tried, but there's apparently a network error so we need to abort this whole thing
                            throw;
                        }

                    }

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
                            hotAddress = HotBitcoinAddress.FromAddress(chain, address);
                        }

                        SwarmDb.GetDatabaseForWriting()
                            .CreateHotBitcoinAddressUnspentConditional(hotAddress.Identity, txUnspent.TransactionHash,
                                (int)txUnspent.TransactionOutputIndex, (Int64)txUnspent.Satoshis,
                                (int)txUnspent.ConfirmationCount);
                    }

                    // Update hotaddress totals

                    HotBitcoinAddresses.UpdateAllUnspentTotals();

                    return result;



                case BitcoinChain.Cash:

                    // TODO: SELECTION OF BLOCK EXPLORER, ADDRESS STRING FORMAT TO GO WITH IT

                    string cashAddress = EnsureCashAddressWithoutPrefix(address);

                    addressInfoResult =
                        JObject.Parse(
                            new WebClient().DownloadString(
                                "https://bch-insight.bitpay.com/api/addr/" + cashAddress));

                    JArray unspentArray;

                    if ((int)addressInfoResult["balanceSat"] == 0 && (int)addressInfoResult["unconfirmedBalanceSat"] == 0)
                    {
                        return false; // no funds on address at all at this time
                    }

                    try
                    {
                        unspentArray = JArray.Parse(
                            new WebClient().DownloadString("https://bch-insight.bitpay.com/api/addr/" + cashAddress + "/utxo"));

                    }
                    catch (WebException webException)
                    {
                        // A 500 on the above _may_ mean that there's no unspent outpoints. It can also mean a data
                        // retrieval or network error, in which case the exception must absolutely not be interpreted
                        // as valid data of zero unspent outpoints.

                        try
                        {
                            if (webException.Response == null)
                            {
                                throw; // if there's no response at all, we can't do shit
                            }

                            string errorResponseContent =
                                new StreamReader(webException.Response.GetResponseStream()).ReadToEnd();

                            if (errorResponseContent.Trim().StartsWith("No free outputs to spend"))
                            {
                                // all is okay network-wise, there just aren't any UTXOs so we're getting an error code for that

                                return false; // no further processing and there are no fresh transactions
                            }

                            throw; // otherwise throw upward
                        }
                        catch (WebException)
                        {
                            // Ok, we tried, but there's apparently a network error so we need to abort this whole thing
                            throw;
                        }

                    }

                    foreach (JObject unspentJson in unspentArray.Children())
                    {
                        BitcoinUnspentTransactionOutput txUnspent = new BitcoinUnspentTransactionOutput()
                        {
                            BitcoinAddress = address,
                            ConfirmationCount = (UInt32)unspentJson["confirmations"],
                            Satoshis = (UInt64)unspentJson["satoshis"],
                            TransactionHash = (string)unspentJson["txid"],
                            TransactionOutputIndex = (UInt32)unspentJson["vout"]
                        };

                        if (txUnspent.ConfirmationCount < 2)
                        {
                            // Fresh transactions, return true
                            result = true;
                        }

                        // Add unspent to database

                        if (hotAddress == null)
                        {
                            hotAddress = HotBitcoinAddress.GetAddressOrForkCore(chain, address);
                        }

                        SwarmDb.GetDatabaseForWriting()
                            .CreateHotBitcoinAddressUnspentConditional(hotAddress.Identity, txUnspent.TransactionHash,
                                (int)txUnspent.TransactionOutputIndex, (Int64)txUnspent.Satoshis,
                                (int)txUnspent.ConfirmationCount);
                    }

                    // Update hotaddress totals

                    HotBitcoinAddresses.UpdateAllUnspentTotals();

                    return result;

                default:
                    throw new NotImplementedException("Unimplemented bitcoin chain: " + chain);
            }

        }


        public static void BroadcastTransaction (Transaction transaction, BitcoinChain chain)
        {
            using (WebClient client = new WebClient())
            {
                switch (chain)
                {
                    case BitcoinChain.Core:

                        client.UploadValues("https://blockchain.info/pushtx?cors=true",
                            new NameValueCollection()
                            {
                                {"tx", transaction.ToHex()}
                            });
                        break;  // no txid given

                    case BitcoinChain.Cash:

                        client.UploadValues("https://bitcoincash.blockexplorer.com/api/tx/send",
                            new NameValueCollection()
                            {
                               {"rawtx", transaction.ToHex()}
                            });
                        break;

                    default:
                        throw new NotImplementedException("Unknown chain: " + chain);
                }
                // TODO: Exception handling
            }
        }

        public static BitcoinTransactionInputs GetInputsForAmount (Organization organization, BitcoinChain chain, Int64 satoshis)
        {
            // TODO: Verify inputs up to date?

            // TODO: What's the most efficient way to do this?

            HotBitcoinAddresses addresses = HotBitcoinAddresses.ForOrganization (organization, BitcoinChain.Cash);
            HotBitcoinAddressUnspents unspents = addresses.Unspents;

            // Lazy checking: if there's one single input that covers the entire amount, use it

            BitcoinTransactionInput lowestSingleSufficientInput = null;

            foreach (HotBitcoinAddressUnspent unspent in unspents)
            {
                if (unspent.Address.Chain == BitcoinChain.Cash)
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
            }

            if (lowestSingleSufficientInput != null)
            {
                // There is a single input that will cover the entire amount, so return it

                return BitcoinTransactionInputs.FromSingle (lowestSingleSufficientInput);
            }

            throw new NotEnoughFundsException("Insufficient funds", "group argument", new Satoshis(satoshis));

            // Slightly more complex checking (TODO)
        }


        public static FinancialTransaction GetSwarmopsTransactionFromBlockchainId (string transactionId)
        {
            return null;
        }


        public static string EnsureLegacyAddress(string address)
        {
            bool dummy1, dummy2;

            try
            {
                string legacyAddress = BitcoinCashAddressConversion.CashAddressToLegacyAddresss(address, out dummy1,
                    out dummy2);

                // If this doesn't throw, we have a valid legacy address

                return legacyAddress;
            }
            catch (Exception)
            {
                // This wasn't a valid cash address, so assume it's a legacy address
            }

            return address;
        }

    


        public static string EnsureCashAddressWithoutPrefix (string address)
        {
            bool dummy1, dummy2;

            try
            {
                BitcoinCashAddressConversion.CashAddressToLegacyAddresss(address, out dummy1, out dummy2);

                // If this doesn't throw, we already have a valid bitcoincash address
                // Just strip the prefix if there is one

                return EnsureNoCashAddressPrefix(address);
            }
            catch (Exception)
            {
                // This wasn't a valid cash address, so assume it's a legacy address
            }

            // Convert a legacy address to a cash address. This may throw if the address was invalid and that's
            // exactly what we want in this case.

            string cashAddress = BitcoinCashAddressConversion.LegacyAddressToCashAddress(address, out dummy1, out dummy2);

            return EnsureNoCashAddressPrefix(cashAddress);
        }

        public static string EnsureCashAddressWithPrefix(string address)
        {
            return BitcoinCashPrefix + EnsureCashAddressWithoutPrefix(address);
        }



        private const string BitcoinCashPrefix = "bitcoincash:";


        public static string EnsureNoCashAddressPrefix(string address)
        {
            if (address.ToLowerInvariant().StartsWith(BitcoinCashPrefix))
            {
                return address.Substring(BitcoinCashPrefix.Length);
            }

            return address;  // does not start with prefix
        }

        public static string EnsureCashAddressPrefix(string address)
        {
            if (!address.ToLowerInvariant().StartsWith(BitcoinCashPrefix))
            {
                return BitcoinCashPrefix + address;
            }

            return address;  // already has prefix
        }


        public static void CheckColdStorageForOrganization (Organization organization)
        {
            FinancialAccount coldRoot = organization.FinancialAccounts.AssetsBitcoinCold;

            if (coldRoot == null)
            {
                return; // no cold assets to check
            }

            // Construct the address-to-account map only once for the entire recursion chain

            Dictionary<string,int> addressAccountLookup = GetAddressAccountLookup (organization);

            // Recurse

            CheckColdStorageRecurse (coldRoot, addressAccountLookup);
        }

        private static void CheckColdStorageRecurse (FinancialAccount parent, Dictionary<string, int> addressAccountLookup)
        {
            foreach (FinancialAccount child in parent.Children)
            {
                CheckColdStorageRecurse (child, addressAccountLookup);
            }

            // After recursing, get all transactions for this account and verify against our records

            // is the account name a valid bitcoin address on the main network?

            string address = parent.BitcoinAddress;
            if (string.IsNullOrEmpty (address))
            {
                if (BitcoinUtility.IsValidBitcoinAddress (parent.Name.Trim()))
                {
                    parent.BitcoinAddress = address = parent.Name.Trim();
                }
                else
                {
                    return; // not a bitcoin address but something else; do not process
                }
            }

            Organization organization = parent.Organization;
            bool organizationLedgerUsesBitcoin = organization.Currency.IsBitcoinCore;

            JObject addressData = JObject.Parse(
                        new WebClient().DownloadString("https://blockchain.info/address/" + address +
                                                        "?format=json&api_key=" +
                                                        SystemSettings.BlockchainSwarmopsApiKey));
            //int transactionCount = (int) (addressData["n_tx"]);

            foreach (JObject txJson in addressData["txs"])
            {
                FinancialTransaction ourTx = null;
                Dictionary<Int64, Int64> satoshisLookup = new Dictionary<long, long>(); // map from ledgercents to satoshis
                BlockchainTransaction blockchainTx = BlockchainTransaction.FromBlockchainInfoJson (txJson);

                try
                {
                    ourTx = FinancialTransaction.FromBlockchainHash (parent.Organization, blockchainTx.TransactionHash);
                    // If the transaction was fetched fine, we have already seen this transaction, but need to re-check it

                }
                catch (ArgumentException)
                {
                    // We didn't have this transaction, so we need to create it

                    ourTx = FinancialTransaction.Create (parent.Organization, blockchainTx.TransactionDateTimeUtc, "Blockchain tx");
                    ourTx.BlockchainHash = blockchainTx.TransactionHash;

                    // Did we lose or gain money?
                    // Find all in- and outpoints, determine which are ours (hot and cold wallet) and which aren't
                }

                Dictionary<int,long> transactionReconstructedRows = new Dictionary<int, long>();

                // Note the non-blockchain rows in this tx, keep them for reconstruction

                foreach (FinancialTransactionRow row in ourTx.Rows)
                {
                    if (!addressAccountLookup.ContainsValue (row.FinancialAccountId)) // not optimal but n is small
                    {
                        // This is not a bitcoin address account, so note it for reconstruction

                        if (!transactionReconstructedRows.ContainsKey (row.FinancialAccountId))
                        {
                            transactionReconstructedRows[row.FinancialAccountId] = 0; // init
                        }

                        transactionReconstructedRows[row.FinancialAccountId] += row.AmountCents;
                    }
                    else
                    {
                        // this is a known blockchain row, note its ledgered value in satoshis
                        if (!organizationLedgerUsesBitcoin)
                        {
                            Money nativeMoney = row.AmountForeignCents;
                            if (nativeMoney != null && nativeMoney.Currency.IsBitcoinCore) // it damn well should be, but just checking
                            {
                                satoshisLookup[row.AmountCents] = row.AmountForeignCents.Cents;
                            }
                        }
                    }
                }

                // Reconstruct the blockchain rows: input, output, fees, in that order

                // -- inputs

                foreach (BlockchainTransactionRow inputRow in blockchainTx.Inputs)
                {
                    if (addressAccountLookup.ContainsKey (inputRow.Address))
                    {
                        // this input is ours

                        int financialAccountId = addressAccountLookup[inputRow.Address];

                        if (!transactionReconstructedRows.ContainsKey (financialAccountId))
                        {
                            transactionReconstructedRows[financialAccountId] = 0; // initialize
                        }

                        if (organizationLedgerUsesBitcoin)
                        {
                            transactionReconstructedRows[financialAccountId] +=
                                -inputRow.ValueSatoshis; // note the negation!
                        }
                        else
                        {
                            Int64 ledgerCents =
                                new Money (inputRow.ValueSatoshis, Currency.BitcoinCore, ourTx.DateTime).ToCurrency (
                                    organization.Currency).Cents;
                            transactionReconstructedRows[financialAccountId] +=
                                -ledgerCents; // note the negation!
                            satoshisLookup[ledgerCents] = inputRow.ValueSatoshis;
                        }
                    }
                }

                // -- outputs

                foreach (BlockchainTransactionRow outputRow in blockchainTx.Outputs)
                {
                    if (addressAccountLookup.ContainsKey(outputRow.Address))
                    {
                        // this output is ours

                        int financialAccountId = addressAccountLookup[outputRow.Address];

                        if (!transactionReconstructedRows.ContainsKey (financialAccountId))
                        {
                            transactionReconstructedRows[financialAccountId] = 0; // initialize
                        }

                        if (organizationLedgerUsesBitcoin)
                        {
                            transactionReconstructedRows[financialAccountId] +=
                                outputRow.ValueSatoshis;
                        }
                        else
                        {
                            Int64 ledgerCents =
                                new Money(outputRow.ValueSatoshis, Currency.BitcoinCore, ourTx.DateTime).ToCurrency(
                                    organization.Currency).Cents;
                            transactionReconstructedRows[financialAccountId] +=
                                ledgerCents;
                            satoshisLookup[ledgerCents] = outputRow.ValueSatoshis;
                        }
                    }
                }

                // -- fees

                if (addressAccountLookup.ContainsKey (blockchainTx.Inputs[0].Address))
                {
                    // if the first input is ours, we're paying the fee (is there any case where this is not true?)

                    if (organizationLedgerUsesBitcoin)
                    {
                        transactionReconstructedRows[organization.FinancialAccounts.CostsBitcoinFees.Identity] =
                            blockchainTx.FeeSatoshis;
                    }
                    else
                    {
                        Int64 feeLedgerCents =
                            new Money (blockchainTx.FeeSatoshis, Currency.BitcoinCore,
                                blockchainTx.TransactionDateTimeUtc).ToCurrency (organization.Currency).Cents;
                        transactionReconstructedRows[organization.FinancialAccounts.CostsBitcoinFees.Identity] =
                            feeLedgerCents;
                    }
                }


                // Rewrite the transaction (called always, but the function won't do anything if everything matches)

                ourTx.RecalculateTransaction (transactionReconstructedRows, /* loggingPerson*/ null);

                // Finally, add foreign cents, if any

                if (!organizationLedgerUsesBitcoin)
                {
                    foreach (FinancialTransactionRow row in ourTx.Rows)
                    {
                        if (addressAccountLookup.ContainsValue (row.Account.Identity)) // "ContainsValue" is bad, but n is low
                        {
                            // Do we have a foreign amount for this row already?

                            Money foreignMoney = row.AmountForeignCents;
                            if (foreignMoney == null || foreignMoney.Cents == 0)
                            {
                                // no we didn't; create one

                                if (satoshisLookup.ContainsKey (row.AmountCents))
                                {
                                    row.AmountForeignCents = new Money(satoshisLookup[row.AmountCents], Currency.BitcoinCore, ourTx.DateTime);
                                }
                                else if (satoshisLookup.ContainsKey (-row.AmountCents)) // the negative counterpart
                                {
                                    row.AmountForeignCents = new Money(-satoshisLookup[-row.AmountCents], Currency.BitcoinCore, ourTx.DateTime);
                                }
                                else
                                {
                                    // There's a last case which may happen if the row is an addition to a previous row; if so, calculate
                                    row.AmountForeignCents = new Money(row.AmountCents, organization.Currency, ourTx.DateTime).ToCurrency (Currency.BitcoinCore);
                                }
                            }
                        }
                    }
                }
            }

        }


        private static Dictionary<string, int> GetAddressAccountLookup (Organization organization)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            // Add all cold addresses

            GetAddressAccountLookupRecurse (organization.FinancialAccounts.AssetsBitcoinCold, result);

            // Add all hot addresses

            HotBitcoinAddresses hotAddresses = HotBitcoinAddresses.ForOrganization (organization);
            foreach (HotBitcoinAddress hotAddress in hotAddresses)
            {
                result[hotAddress.ProtocolLevelAddress] = organization.FinancialAccounts.AssetsBitcoinHot.Identity;
            }

            return result;
        }

        private static void GetAddressAccountLookupRecurse (FinancialAccount account, Dictionary<string, int> result)
        {
            if (account == null)
            {
                return;
            }

            foreach (FinancialAccount child in account.Children)
            {
                GetAddressAccountLookupRecurse (child, result);
            }

            if (!string.IsNullOrEmpty(account.BitcoinAddress) || IsValidBitcoinAddress (account.Name)) // TODO: Add a special property for the address instead of using name
            {
                result [account.Name] = account.Identity;
            }
        }

        public const int BitcoinWalletIndex = 1;
        public const int BitcoinDonationsIndex = 2;
        public const int BitcoinLoansIndex = 3;
        public const int BitcoinAccountsReceivableIndex = 4;
        public const int BitcoinEchoTestIndex = 127;

        public const int EchoFeeSatoshis = 200; // translates to just over 100 sats / kilobyte: tx has one input, one output

        public const string BitcoinTestAddress = "1JMpU3D6c5sruunMwzkt6p6PQzLcUYcL26";

        
        public static bool IsValidBitcoinAddress (string address)
        {
            try
            {
                #pragma warning disable 219
                // ReSharper disable once UnusedVariable

                BitcoinAddress singleAddress = new BitcoinPubKeyAddress (address, Network.Main);
                return true;

            }

            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
                // ignore for now; move on to next possible case
            }

            try
            {
                // ReSharper disable once UnusedVariable

                BitcoinScriptAddress multiSigAddress = new BitcoinScriptAddress (address, Network.Main);
                return true;
                #pragma warning restore 219
            }

            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }

            return false; // not a valid address, neither single nor multisig
        }


        public static Int64 GetRecommendedFeePerThousandBytesSatoshis (BitcoinChain chain, int blocksWait = 2)
        {
            if (_lastFeeRefreshLookup == null)
            {
                _lastFeeRefreshLookup = new Dictionary<BitcoinChain, DateTime>();
                _lastFeeSatoshisLookup = new Dictionary<BitcoinChain, long>();
            }

            DateTime utcNow = DateTime.UtcNow;
            if (_lastFeeRefreshLookup.ContainsKey(chain) && utcNow < _lastFeeRefreshLookup[chain].AddHours (3))
            {
                return _lastFeeSatoshisLookup[chain]; // cache fee estimate for three hours
            }

            try
            {
                Int64 satoshisPerThousandBytes = 1010; // default for Bitcoin Cash

                if (chain == BitcoinChain.Core)
                {
                    // But Core has way way WAAAAAAY higher fees

                    JObject feeData = JObject.Parse(
                                new WebClient().DownloadString("https://blockexplorer.com/api/utils/estimatefee?nbBlocks=" + blocksWait.ToString(CultureInfo.InvariantCulture)));
                    double feeWholeCoins = Double.Parse((string)feeData[blocksWait.ToString(CultureInfo.InvariantCulture)], NumberStyles.AllowDecimalPoint);  // rounding errors are okay, don't use Formatting fn
                    satoshisPerThousandBytes = Convert.ToInt64(feeWholeCoins * _satoshisPerBitcoin);
                }

                _lastFeeSatoshisLookup[chain] = satoshisPerThousandBytes;
                _lastFeeRefreshLookup[chain] = utcNow;
                return satoshisPerThousandBytes;
            }
            catch (Exception)
            {
                // TODO: Check if _lastFeeRefresh is older than a day

                if (_lastFeeSatoshisLookup.ContainsKey(chain))
                {
                    return _lastFeeSatoshisLookup[chain];
                }

                if (chain == BitcoinChain.Cash)
                {
                    return 1012; // a low standard: one sat per byte, plus some random extra
                }

                return 1000*200; // a not so particularly low standard: 200 sat per byte
            }
        }

        private static Dictionary<BitcoinChain,DateTime> _lastFeeRefreshLookup;
        private static Dictionary<BitcoinChain,Int64> _lastFeeSatoshisLookup;

        private const Int64 _satoshisPerBitcoin = 100 * 1000 * 1000; // written this way to improve readability - important constants

        

        public static Currency GetCurrencyFromChain(BitcoinChain chain)
        {
            switch (chain)
            {
                case BitcoinChain.Core:
                    return Currency.BitcoinCore;
                case BitcoinChain.Cash:
                    return Currency.BitcoinCash;
                default:
                    throw new NotImplementedException("Unimplemented BitcoinChain: " + chain);
            }
        }

        public static Int64 SatoshisPerBitcoin
        {
            get { return _satoshisPerBitcoin; }
        }

        public static void TestMultisigPayout()
        {
            throw new InvalidOperationException("This function is only for testing purposes. It pays real money. Don't use except for dev/test.");

            // disable "code unreachable" warning for this code
            // ReSharper disable once CSharpWarnings::CS0162
            #pragma warning disable 162,219
            Organization organization = Organization.Sandbox; // a few testing cents here in test environment

            string bitcoinTestAddress = "3KS6AuQbZARSvqnaHoHfL1Xhm3bTLFAzoK";

            // Make a small test payment to a multisig address

            TransactionBuilder txBuilder = null; // TODO TODO TODO TODO  new TransactionBuilder();
            Int64 satoshis = new Money(100, Currency.FromCode ("SEK")).ToCurrency (Currency.BitcoinCash).Cents;
            BitcoinTransactionInputs inputs = null;
            Int64 satoshisMaximumAnticipatedFees = BitcoinUtility.GetRecommendedFeePerThousandBytesSatoshis(BitcoinChain.Cash) * 20; // assume max 20k transaction size

            try
            {
                inputs = BitcoinUtility.GetInputsForAmount(organization, BitcoinChain.Cash, satoshis + satoshisMaximumAnticipatedFees);
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
            //Dictionary<int, List<string>> notificationSpecLookup = new Dictionary<int, List<string>>();
            //Dictionary<int, List<Int64>> notificationAmountLookup = new Dictionary<int, List<Int64>>();
            Payout masterPayoutPrototype = Payout.Empty;
            HotBitcoinAddress changeAddress = HotBitcoinAddress.OrganizationWalletZero(organization, BitcoinChain.Core);

            // Add the test payment

            if (bitcoinTestAddress.StartsWith("1")) // regular address
            {
                txBuilder = txBuilder.Send(new BitcoinPubKeyAddress(bitcoinTestAddress),
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

            txBuilder.SetChange(new BitcoinPubKeyAddress(changeAddress.ProtocolLevelAddress));

            // Add fee

            int transactionSizeBytes = txBuilder.EstimateSize(txBuilder.BuildTransaction(false)) + inputs.Count;
            // +inputs.Count for size variability

            Int64 feeSatoshis = (transactionSizeBytes/1000 + 1)*
                                BitcoinUtility.GetRecommendedFeePerThousandBytesSatoshis(BitcoinChain.Cash);

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

            BitcoinUtility.BroadcastTransaction(txReady, BitcoinChain.Cash);

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

            // Restore "code unreachable", "unsued var" warnings
            #pragma warning restore 162,219

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
                            new BitcoinPubKeyAddress (source.BitcoinAddress, Network.Main)));
            }
        }

        public class NBitcoinSecureRandomizer: NBitcoin.IRandom 
        {
            public void GetBytes(byte[] buffer)
            {
                int byteCount = buffer.Length;
                byte[] randomBuffer = SupportFunctions.GenerateSecureRandomBytes(byteCount);
                Array.Copy(randomBuffer, buffer, byteCount);
            }
        }
    }
}
