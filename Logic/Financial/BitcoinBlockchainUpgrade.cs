using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    internal class BitcoinBlockchainUpgrade
    {
        static internal void Upgrade(int fromVersion)
        {
            fromVersion -= fromVersion%2;
            int toVersion = BitcoinUtility.ExpectedBlockchainCodeVersion;

            while (fromVersion < toVersion)
            {
                SystemSettings.BlockchainCodeVersion = fromVersion + 1;  // Indicates upgrade in progress

                switch (fromVersion)
                {
                    case 0:
                        SplitCashFromCore();
                        break;
                    default:
                        throw new NotImplementedException("Unimplemented upgrade path");
                }


                fromVersion += 2;
            }

            SystemSettings.BlockchainCodeVersion = fromVersion;
        }

        static internal void SplitCashFromCore()
        {
            
        }
    }
}
