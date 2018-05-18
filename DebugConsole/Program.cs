using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;


// This is a console environment for debugging the logic and database layers.
// Nothing in this project is deployed to production environments.


namespace Swarmops.DebugConsole
{
    class Program
    {
        static void Main(/*string[] args*/)
        {
            // Establish that we have a database connection

            Person testPerson = Person.FromIdentity(1);
            testPerson.GetSecureAvatarLink(64); // suppress "is never used" warning

            Organization organization = Organization.FromIdentity(7);

            // TODO:

            // 0) Check ForeignCurrency of hotwallet account

            FinancialAccount hotWallet = organization.FinancialAccounts.AssetsBitcoinHot;

            if (hotWallet != null)
            {
                Console.WriteLine("Organization has Hotwallet");

                if (hotWallet.ForeignCurrency != null)
                {
                    if (!hotWallet.ForeignCurrency.IsBitcoinCash)
                    {
                        Console.WriteLine(" - Foreign Currency was incorrect; setting to BCH");

                        hotWallet.ForeignCurrency = Currency.BitcoinCash;
                    }
                }
                else
                {
                    // ForeignCurrency of hotwallet was Null
                    hotWallet.ForeignCurrency = Currency.BitcoinCash;

                    Console.WriteLine(" - Foreign Currency was not set; setting to BCH");
                }
            }

            // 0.5) Re-register the initial Sandbox Echo address

            HotBitcoinAddress.Create(organization, BitcoinChain.Cash,
                             BitcoinUtility.BitcoinEchoTestIndex, 1);

            // 1) Check unspents on hotwallet addresses

            HotBitcoinAddresses addresses = HotBitcoinAddresses.ForOrganization(organization);

            foreach (HotBitcoinAddress address in addresses)
            {
                HotBitcoinAddressUnspents unspents1 = HotBitcoinAddressUnspents.ForAddress(address);
                int beforeCheckCount = unspents1.Count();

                BitcoinUtility.TestUnspents(BitcoinChain.Core, address.ProtocolLevelAddress);
                BitcoinUtility.TestUnspents(BitcoinChain.Cash, address.ProtocolLevelAddress);

                HotBitcoinAddressUnspents unspents2 = HotBitcoinAddressUnspents.ForAddress(address);
                int correctedCount = unspents2.Count();

                if (beforeCheckCount != correctedCount)
                {
                    Console.WriteLine(" - Unspent count was corrected: ");
                }
            }

            // 2) Shapeshift all Core into Cash

            // 3) Adjust balances of foreign cents on hotwallet

            // 4) Adjust value of hotwallet

        }
    }
}
