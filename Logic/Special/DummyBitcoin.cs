using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NBitcoin;

namespace Swarmops.Logic.Special
{
    /// <summary>
    /// The point of this class is to reference NBitcoin, so its assembly and dependencies get copied to the output dir,
    /// as the Site can't reference assemblies directly.
    /// </summary>
    public class DummyBitcoin
    {
        public static BitcoinAddress Address { get; set; }

        public DummyBitcoin()
        {
            Address = new BitcoinAddress("abcdefgh");
        }
    }
}
