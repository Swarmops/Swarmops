using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;

namespace Swarmops.Basic.Types.Common
{
    [Serializable]
    public class BlockchainHash
    {
        [Obsolete("Do not call the parameterless ctor", true)]
        public BlockchainHash()
        {
            // do not call the parameterless ctor; it is there for serialization to work
        }

        public BlockchainHash(BitcoinChain chain, string hash)
        {
            this.Chain = chain;
            this.Hash = hash;
        }

        public BitcoinChain Chain { get; private set; }
        public string Hash { get; private set; }

        public static BlockchainHash Parse(string input)
        {
            int indexOfDash = input.IndexOf('-');
            if (indexOfDash < 0)
            {
                throw new ArgumentException("Invalid chain hash");
            }

            string chainPart = input.Substring(0, indexOfDash);
            string hashPart = input.Substring(indexOfDash + 1);
            BitcoinChain chain = (BitcoinChain) Enum.Parse(typeof (BitcoinChain), chainPart);

            return new BlockchainHash(chain, hashPart);
        }

        public new string ToString()
        {
            return this.Chain.ToString() + "-" + this.Hash;
        }
    }
}
