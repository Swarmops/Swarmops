using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

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
        // typedef the name
    }
}
