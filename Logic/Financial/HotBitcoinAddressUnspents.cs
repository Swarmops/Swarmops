using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class HotBitcoinAddressUnspents: PluralBase<HotBitcoinAddressUnspents, HotBitcoinAddressUnspent, BasicHotBitcoinAddressUnspent>
    {
        static public HotBitcoinAddressUnspents ForAddress (HotBitcoinAddress address)
        {
            return FromArray (SwarmDb.GetDatabaseForWriting().GetHotBitcoinAddressUnspents (address)); // "ForWriting" is intentional here
        }

        public Int64 AmountSatoshisTotal
        {
            get { return this.Sum (item => item.AmountSatoshis); }
        }

        public BitcoinTransactionInputs AsInputs
        {
            get
            {
                BitcoinTransactionInputs result = new BitcoinTransactionInputs();
                foreach (HotBitcoinAddressUnspent unspent in this)
                {
                    HotBitcoinAddress address = unspent.Address;

                    BitcoinTransactionInput input = new BitcoinTransactionInput()
                    {
                        AmountSatoshis = unspent.AmountSatoshis,
                        BitcoinAddress = address.Address,
                        PrivateKey = address.PrivateKey,
                        TransactionHash = unspent.TransactionHash,
                        TransactionOutputIndex = unspent.TransactionOutputIndex
                    };

                    result.Add (input);
                }

                return result;
            }
        }

        public void DeleteAll()
        {
            Dictionary<int, bool> addressIdLookup = new Dictionary<int, bool>();
            this.ForEach (item => addressIdLookup[item.HotBitcoinAddressId] = true);
            SwarmDb db = SwarmDb.GetDatabaseForWriting();

            // Delete the unspents in this collection

            this.ForEach (item => db.DeleteHotBitcoinAddressUnspent (item.Identity));

            // Recalculate the amount remaining in the addresses

            foreach (int addressId in addressIdLookup.Keys)
            {
                db.SetHotBitcoinAddressBalance (addressId,
                    HotBitcoinAddress.FromIdentity (addressId).Unspents.AmountSatoshisTotal);
            }

        }
    }
}
