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

        public static HotBitcoinAddress FromIdentity(int hotBitcoinAddressId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetHotBitcoinAddress(hotBitcoinAddressId));
        }

        internal static HotBitcoinAddress FromIdentityAggressive(int hotBitcoinAddressId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetHotBitcoinAddress(hotBitcoinAddressId)); // "Writing" is intentional
        }

        public static HotBitcoinAddress Create(Organization organization, string derivationPath)
        {


/*
            string bitcoinAddress =
                FinancialAccounts.BitcoinHotPublicRoot
                    .Derive((uint)this.CurrentOrganization.Identity)
                    .Derive(BitcoinUtility.BitcoinDonationsIndex)
                    .Derive((uint)this.CurrentUser.Identity)
                    .PubKey.GetAddress(Network.Main)
                    .ToString();*/

            throw new NotImplementedException();
        }

        public static HotBitcoinAddress Create (Organization organization, params int[] derivationPath)
        {
            ExtPubKey extPubKey = FinancialAccounts.BitcoinHotPublicRoot;
            extPubKey = extPubKey.Derive ((uint) organization.Identity);
            string derivationPathString = string.Empty;

            foreach (int derivation in derivationPath) // requires that order is consistent from 0 to n-1
            {
                extPubKey = extPubKey.Derive ((uint) derivation);
                derivationPathString += " " + derivation.ToString (CultureInfo.InvariantCulture);
            }

            derivationPathString = derivationPathString.TrimStart();
            string bitcoinAddress = extPubKey.PubKey.GetAddress (Network.Main).ToString();

            int hotBitcoinAddressId =
                SwarmDb.GetDatabaseForWriting()
                    .CreateHotBitcoinAddressConditional (organization.Identity, derivationPathString, bitcoinAddress);

            return FromIdentityAggressive (hotBitcoinAddressId);
        }

        public static HotBitcoinAddress FromAddress (string bitcoinAddress)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetHotBitcoinAddress(bitcoinAddress));
        }

        public HotBitcoinAddressUnspents Unspents
        {
            get { return HotBitcoinAddressUnspents.ForAddress (this); }
        }
    }
}
