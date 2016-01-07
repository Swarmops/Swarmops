using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class HotBitcoinAddresses: PluralBase<HotBitcoinAddresses, HotBitcoinAddress, BasicHotBitcoinAddress>
    {
        public static HotBitcoinAddresses ForOrganization (Organization organization)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetHotBitcoinAddresses (organization));
        }

        public HotBitcoinAddresses InMainWallet
        {
            get
            {
                HotBitcoinAddresses subset = new HotBitcoinAddresses();
                string startMatch = BitcoinUtility.BitcoinWalletIndex.ToString (CultureInfo.InvariantCulture) + " ";
                foreach (HotBitcoinAddress address in this)
                {
                    if (address.DerivationPath.StartsWith (startMatch))
                    {
                        subset.Add (address);
                    }
                }

                return subset;
            }
        }

        public BitcoinTransactionInputs FindAmount (Int64 satoshisRequired)
        {
            // Sort addresses by known balance, richest first

            HotBitcoinAddresses workCopy = this;

            workCopy.Sort((a,b) => Math.Sign(b.BalanceSatoshis - a.BalanceSatoshis)); // Descending order

            // Do linear search from top for last address with sufficient money on its own. TODO: Binary search.

            int lastSufficientIndex = -1;
            for (int index = 0; index < workCopy.Count; index++)
            {
                if (workCopy[index].BalanceSatoshis >= satoshisRequired)
                {
                    lastSufficientIndex = index;
                }
            }

            if (lastSufficientIndex >= 0)
            {
                // There exists at least one single input which is sufficient to meet the requirement. 
                // The lowest input fulfilling this criterion has been chosen as return value.

                return workCopy[lastSufficientIndex].Unspents.AsInputs;
            }

            throw new NotEnoughFundsException(); // Serving as a placeholder for now, also testing the notification

            throw new NotImplementedException("This fundfinding path is not completed"); // TODO
        }

        public Int64 BalanceSatoshisTotal
        {
            get { return this.Sum (item => item.BalanceSatoshis); }
        }

        public HotBitcoinAddressUnspents Unspents
        {
            get
            {
                HotBitcoinAddressUnspents unspents = new HotBitcoinAddressUnspents();
                foreach (HotBitcoinAddress address in this)
                {
                    unspents.AddRange (address.Unspents);
                }

                return unspents;
            }
        }
    }
}
