using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;

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
    }
}
