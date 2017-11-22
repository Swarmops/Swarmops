using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Satoshis=NBitcoin.Money;

namespace Swarmops.Logic.Financial
{
    public class BitcoinTransactionInput
    {
        public BitcoinSecret PrivateKey { get; set; }
        public Int64 AmountSatoshis { get; set; }
        public string TransactionHash { get; set; }
        public Int32 TransactionOutputIndex { get; set; }
        public string BitcoinAddress { get; set; }
        public int HotBitcoinAddressUnspentId { get; set; }

        public HotBitcoinAddressUnspent AsUnspent
        {
            get { return HotBitcoinAddressUnspent.FromIdentity (this.HotBitcoinAddressUnspentId); }
        }
    }

    public class BitcoinTransactionInputs: List<BitcoinTransactionInput>
    {
        public static BitcoinTransactionInputs FromSingle (BitcoinTransactionInput single)
        {
            BitcoinTransactionInputs result = new BitcoinTransactionInputs();
            result.Add (single);
            return result;
        }

        public BitcoinSecret[] PrivateKeys
        {
            get
            {
                List<BitcoinSecret> keyList = new List<BitcoinSecret>();

                foreach (BitcoinTransactionInput input in this)
                {
                    keyList.Add (input.PrivateKey);
                }

                return keyList.ToArray();
            }
        }

        public Coin[] Coins
        {
            get
            {
                Console.WriteLine("WTF");

                List<Coin> coinList = new List<Coin>();

                foreach (BitcoinTransactionInput input in this)
                {
                    coinList.Add (new Coin (
                        new OutPoint()
                        {
                            Hash = uint256.Parse (input.TransactionHash),
                            N = (uint) input.TransactionOutputIndex
                        },
                        new TxOut (new Satoshis (input.AmountSatoshis),
                            new BitcoinPubKeyAddress (input.BitcoinAddress, Network.Main))));
                }

                return coinList.ToArray();
            }
        }

        public Int64 AmountSatoshisTotal
        {
            get { return this.Sum (input => input.AmountSatoshis); }
        }

        public HotBitcoinAddressUnspents AsUnspents
        {
            get
            {
                HotBitcoinAddressUnspents unspents = new HotBitcoinAddressUnspents();
                this.ForEach (item => unspents.Add (item.AsUnspent));
                return unspents;
            }
        }
    }
}
