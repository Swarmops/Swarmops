using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NBitcoin;
using NBitcoin.BouncyCastle.Asn1.Ocsp;
using Newtonsoft.Json.Linq;

namespace Swarmops.Logic.Financial
{
    public class BitcoinUtility
    {
        static public Coin[] GetSpendableCoin (BitcoinSecret secretKey) // we're using BitcoinSecret as arg just to reinforce caller must have privkey to spend funds
        {
            // This function queries the Blockchain API for the unspent coin.

            string addressString = secretKey.PubKey.GetAddress (Network.Main).ToString();
            var unspentJsonResult =
                JObject.Parse (new WebClient().DownloadString ("https://blockchain.info/unspent?active=" + addressString));

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

        public const int BitcoinWalletIndex = 1;
        public const int BitcoinDonationsIndex = 2;
        public const int BitcoinLoansIndex = 3;
        public const int BitcoinAccountsReceivableIndex = 4;

        public const string BitcoinTestAddress = "1JMpU3D6c5sruunMwzkt6p6PQzLcUYcL26";
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
