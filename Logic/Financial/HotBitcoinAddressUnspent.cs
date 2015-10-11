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
