using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicHotBitcoinAddress: IHasIdentity
    {
        public BasicHotBitcoinAddress (int hotBitcoinAddressId, int organizationId, BitcoinChain chain, string derivationPath,
            int uniqueDerive, string address, Int64 balanceSatoshis, Int64 throughputSatoshis)
        {
            this.HotBitcoinAddressId = hotBitcoinAddressId;
            this.OrganizationId = organizationId;
            this.Chain = chain;
            this.DerivationPath = derivationPath;
            this.UniqueDerive = uniqueDerive;
            this.MachineAddress = address;
            this.BalanceSatoshis = balanceSatoshis;
            this.ThroughputSatoshis = throughputSatoshis;
        }

        public BasicHotBitcoinAddress (BasicHotBitcoinAddress original)
            : this (
                original.HotBitcoinAddressId, original.OrganizationId, original.Chain, original.DerivationPath, 
                original.UniqueDerive, original.MachineAddress, original.BalanceSatoshis, original.ThroughputSatoshis)
        {
            // copy ctor
        }

        public int HotBitcoinAddressId { get; private set; }
        public int OrganizationId { get; private set; }
        public BitcoinChain Chain { get; private set; }
        public string DerivationPath { get; private set; }
        public int UniqueDerive { get; private set; }
        public string MachineAddress { get; private set; }
        public Int64 BalanceSatoshis { get; protected set; }
        public Int64 ThroughputSatoshis { get; protected set; }

        public int Identity { get { return HotBitcoinAddressId; }}
    }
}
