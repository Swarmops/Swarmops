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
    }

    public class BitcoinTransactionInputs: List<BitcoinTransactionInput>
    {
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
                            new BitcoinAddress (input.BitcoinAddress, Network.Main))));
                }

                return coinList.ToArray();
            }
        }

        public Int64 AmountSatoshisTotal
        {
            get { return this.Sum (input => input.AmountSatoshis); }
        }
    }
}
