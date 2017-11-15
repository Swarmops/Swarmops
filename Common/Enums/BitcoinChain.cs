using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Common.Enums
{
    public enum BitcoinChain
    {
        /// <summary>
        /// Undefined chain -- should never happen, but could probably be resolved by asking block explorers
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The "Core" chain, referring to the Blockstream-hijacked chain of 2014
        /// </summary>
        Core = 1,

        /// <summary>
        /// The "Cash" chain, referring to the fork of August 1 2017 following the original Bitcoin whitepaper
        /// </summary>
        Cash = 2
    }
}
