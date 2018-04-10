using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
{
    public class HotBitcoinAddressUnspent: BasicHotBitcoinAddressUnspent
    {
        private HotBitcoinAddressUnspent (BasicHotBitcoinAddressUnspent basic): base (basic)
        {
            // private ctor
        }

        public static HotBitcoinAddressUnspent FromBasic (BasicHotBitcoinAddressUnspent basic)
        {
            return new HotBitcoinAddressUnspent (basic);
        }

        public static HotBitcoinAddressUnspent FromIdentity (int hotBitcoinAddressUnspentId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetHotBitcoinAddressUnspent (hotBitcoinAddressUnspentId));
        }

        public BitcoinTransactionInput AsInput
        {
            get
            {
                HotBitcoinAddress address = this.Address;

                return new BitcoinTransactionInput()
                {
                    AmountSatoshis = this.AmountSatoshis,
                    BitcoinAddress = address.ProtocolLevelAddress,
                    PrivateKey = address.PrivateKey,
                    TransactionHash = this.TransactionHash,
                    TransactionOutputIndex = this.TransactionOutputIndex,
                    HotBitcoinAddressUnspentId = this.Identity
                };
            }
        }

        public BitcoinTransactionInputs AsInputs
        {
            get { return BitcoinTransactionInputs.FromSingle(AsInput); }
        }

        public HotBitcoinAddress Address
        {
            get { return HotBitcoinAddress.FromIdentity (base.HotBitcoinAddressId); }
        }

        public void Delete()
        {
            SwarmDb.GetDatabaseForWriting().DeleteHotBitcoinAddressUnspent (this.Identity);
            SwarmDb.GetDatabaseForWriting().SetHotBitcoinAddressBalance (base.HotBitcoinAddressId, this.Address.Unspents.AmountSatoshisTotal); // re-fetches from db
        }
    }
}
