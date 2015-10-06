using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;

namespace Swarmops.Logic.Financial
{
    public class HotBitcoinAddress: BasicHotBitcoinAddress
    {
        private HotBitcoinAddress(BasicHotBitcoinAddress basic): base (basic)
        {
            // private ctor
        }

        public static HotBitcoinAddress FromBasic (BasicHotBitcoinAddress basic)
        {
            return new HotBitcoinAddress (basic); // invoke private ctor
        }


    }
}
