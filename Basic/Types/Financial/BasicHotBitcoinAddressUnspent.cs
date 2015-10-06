using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicHotBitcoinAddressUnspent: IHasIdentity
    {
        public BasicHotBitcoinAddressUnspent (int hotBitcoinAddressUnspentId, string transactionHash,
            int transactionOutputIndex, Int64 amountSatoshis, int confirmationCount)
        {
            this.HotBitcoinAddressUnspentId = hotBitcoinAddressUnspentId;
            this.TransactionHash = transactionHash;
            this.TransactionOutputIndex = transactionOutputIndex;
            this.AmountSatoshis = amountSatoshis;
            this.ConfirmationCount = confirmationCount;
        }

        public BasicHotBitcoinAddressUnspent (BasicHotBitcoinAddressUnspent original)
            : this (
                original.HotBitcoinAddressUnspentId, original.TransactionHash, original.TransactionOutputIndex,
                original.AmountSatoshis, original.ConfirmationCount)
        {
            // copy ctor
        }

        public int HotBitcoinAddressUnspentId { get; private set; }
        public string TransactionHash { get; private set; }
        public int TransactionOutputIndex { get; private set; }
        public Int64 AmountSatoshis { get; private set; }
        public int ConfirmationCount { get; protected set; }
        
        public int Identity { get { return HotBitcoinAddressUnspentId; }}
    }
}
