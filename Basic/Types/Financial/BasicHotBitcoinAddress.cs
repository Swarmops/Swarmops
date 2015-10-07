using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicHotBitcoinAddress: IHasIdentity
    {
        public BasicHotBitcoinAddress (int hotBitcoinAddressId, int organizationId, string derivationPath,
            string address, Int64 balanceSatoshis, Int64 throughputSatoshis)
        {
            this.HotBitcoinAddressId = hotBitcoinAddressId;
            this.OrganizationId = organizationId;
            this.DerivationPath = derivationPath;
            this.Address = address;
            this.BalanceSatoshis = balanceSatoshis;
            this.ThroughputSatoshis = throughputSatoshis;
        }

        public BasicHotBitcoinAddress (BasicHotBitcoinAddress original)
            : this (
                original.HotBitcoinAddressId, original.OrganizationId, original.DerivationPath, original.Address, original.BalanceSatoshis,
                original.ThroughputSatoshis)
        {
            // copy ctor
        }

        public int HotBitcoinAddressId { get; private set; }
        public int OrganizationId { get; private set; }
        public string DerivationPath { get; private set; }
        public string Address { get; private set; }
        public Int64 BalanceSatoshis { get; protected set; }
        public Int64 ThroughputSatoshis { get; protected set; }

        public int Identity { get { return HotBitcoinAddressId; }}
    }
}
