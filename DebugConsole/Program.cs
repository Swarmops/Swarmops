using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.BackendServices;


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

            Organization organization = Organization.FromIdentity(8);

            // Debug debug: Try to load a CSV file for import

            Person currentUser = testPerson;
            FinancialAccount account = FinancialAccount.FromIdentity(150);

            Currency accountCurrency = account.ForeignCurrency;
            Currency presentationCurrency = organization.Currency;

            ExternalBankData externalData = new ExternalBankData();
            externalData.Profile = account.ExternalBankDataProfile;

            using (StreamReader reader = new StreamReader(@"E:\Bank Files\debug.csv", externalData.Profile.Encoding == "UTF8" ? Encoding.UTF8 : Encoding.GetEncoding(1252)))
            {
                externalData.LoadData(reader, organization, accountCurrency);
            }
            // TODO:


            /*
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
                             BitcoinUtility.BitcoinEchoTestIndex, 1);*/

            // 1) Check unspents on hotwallet addresses

            /*
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
                    Console.WriteLine(" - Unspent count was corrected: was {0}, changed to {1}", beforeCheckCount, correctedCount);
                }

                if (
                    address.DerivationPath.StartsWith(BitcoinUtility.BitcoinEchoTestIndex.ToString() + " "))
                {
                    // This is an address that should be echoed back, all of it

                    foreach (HotBitcoinAddressUnspent unspent in unspents2)
                    {
                        ReturnBitcoinEchoUtxoOrder backendOrder = new ReturnBitcoinEchoUtxoOrder(unspent);
                        backendOrder.Create(organization, testPerson);
                    }
                }
            }*/

            // 2) Check cold storage accounts, make sure there are corresponding Cash accounts for all Core accounts

            /*
            FinancialAccount coldStorageRoot = organization.FinancialAccounts.AssetsBitcoinCold;

            if (coldStorageRoot != null)
            {
                FinancialAccounts accounts = coldStorageRoot.ThisAndBelow();

                foreach (FinancialAccount account in accounts)
                {
                    string bitcoinAddress = account.BitcoinAddress;

                    if (!String.IsNullOrEmpty(bitcoinAddress))
                    {
                        Currency accountCurrency = organization.Currency;
                        if (account.ForeignCurrency != null)
                        {
                            accountCurrency = account.ForeignCurrency;
                        }

                        if (accountCurrency.IsBitcoinCore)
                        {
                            // Assert there's a corresponding Bitcoin Cash account

                            bool bitcoinCashExists = false;

                            FinancialAccounts accountsMatchingAddress =
                                FinancialAccounts.FromBitcoinAddress(bitcoinAddress);

                            foreach (FinancialAccount accountMatchingAddress in accountsMatchingAddress)
                            {
                                if (accountMatchingAddress.Identity == account.Identity)
                                {
                                    continue; // this is the outer loop account we've found in the inner loop
                                }

                                if (accountMatchingAddress.OrganizationId != account.OrganizationId)
                                {
                                    // This is not supposed to happen, ever, since it implies that two
                                    // different organizations share the same bitcoin address. Nevertheless
                                    // it's a theoretically valid case and so we check for it

                                    continue; // not the right organization
                                }

                                if (accountMatchingAddress.ForeignCurrency.IsBitcoinCash)
                                {
                                    // We have a match for organization, currency, and address

                                    bitcoinCashExists = true;
                                }
                            }

                            if (!bitcoinCashExists)
                            {
                                // Need to create a new Bitcoin Cash address and populate it with transactions,
                                // starting on 2017-Dec-30

                                if (!account.Name.StartsWith("[Core] "))
                                {
                                    account.Name = "[Core] " + account.Name;
                                }

                                FinancialAccount correspondingCashAccount = FinancialAccount.Create(
                                    account.Organization, "[Cash] " + account.Name.Substring(7), account.AccountType,
                                    account.Parent);

                                correspondingCashAccount.BitcoinAddress = account.BitcoinAddress;

                                if (!organization.Currency.IsBitcoinCash)
                                {
                                    correspondingCashAccount.ForeignCurrency = Currency.BitcoinCash;
                                }
                            }
                        }
                    }
                }
            }

            // 2½ - TODO) Shapeshift all Core into Cash

            // 2 3/4 -- Check cold storage for the new Cash accounts
            
            BitcoinUtility.CheckColdStorageForOrganization(organization); 

            // 3) Adjust balances of foreign cents on hotwallet

            // 4) Adjust value of hotwallet
            */
        }
    }
}
