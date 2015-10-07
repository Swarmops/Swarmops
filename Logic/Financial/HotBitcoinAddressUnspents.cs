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
            return FromArray (SwarmDb.GetDatabaseForReading().GetHotBitcoinAddressUnspents (address));
        }
    }
}
