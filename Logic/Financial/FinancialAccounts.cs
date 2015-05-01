using System;
using System.Collections.Generic;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class FinancialAccounts : PluralBase<FinancialAccounts, FinancialAccount, BasicFinancialAccount>
    {
        public static FinancialAccounts ForOrganization (Organization organization)
        {
            return
                FromArray (SwarmDb.GetDatabaseForReading()
                    .GetFinancialAccountTreeForOrganization (organization.Identity));
        }

        public static FinancialAccounts ForOwner (Person person)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetFinancialAccountsOwnedByPerson (person.Identity));
        }


        public static FinancialAccounts ForOrganization (Organization organization, FinancialAccountType accountType)
        {
            FinancialAccounts allAccounts = ForOrganization (organization);

            if (accountType == FinancialAccountType.All)
            {
                return allAccounts;
            }

            FinancialAccounts result = new FinancialAccounts();

            Dictionary<FinancialAccountType, bool> lookup = new Dictionary<FinancialAccountType, bool>();
            lookup[accountType] = true;

            if (accountType == FinancialAccountType.Balance || accountType == FinancialAccountType.Unknown)
            {
                lookup[FinancialAccountType.Asset] = true;
                lookup[FinancialAccountType.Debt] = true;
            }

            if (accountType == FinancialAccountType.Result || accountType == FinancialAccountType.Unknown)
            {
                lookup[FinancialAccountType.Income] = true;
                lookup[FinancialAccountType.Cost] = true;
            }

            foreach (FinancialAccount account in allAccounts)
            {
                if (lookup.ContainsKey (account.AccountType))
                {
                    result.Add (account);
                }
            }

            return result;
        }

        public static FinancialAccounts FromBankTransactionTag (string tag)
        {
            int[] accountIdentities =
                SwarmDb.GetDatabaseForReading().GetObjectsByOptionalData (ObjectType.FinancialAccount,
                    ObjectOptionalDataType.
                        BankTransactionTag,
                    tag.ToLower());

            return FromIdentities (accountIdentities);
        }

        public static FinancialAccounts FromIdentities (int[] financialAccountIds)
        {
            if (financialAccountIds.Length == 0)
            {
                return new FinancialAccounts();
            }

            return FromArray (SwarmDb.GetDatabaseForReading().GetFinancialAccounts (financialAccountIds));
        }

        public FinancialAccountRows GetRows (DateTime start, DateTime end)
        {
            BasicFinancialAccountRow[] basicRows = SwarmDb.GetDatabaseForReading()
                .GetFinancialAccountRows (Identities, start, end, false);
            return FinancialAccountRows.FromArray (basicRows);
        }

        public FinancialAccountRows GetRowsFar (DateTime start, DateTime end)
        {
            BasicFinancialAccountRow[] basicRows = SwarmDb.GetDatabaseForReading()
                .GetFinancialAccountRows (Identities, start, end, true);
            return FinancialAccountRows.FromArray (basicRows);
        }

        [Obsolete ("This function uses double-point variables for money. Use GetBudgetSumCents instead.")]
        public double GetBudgetSum (int year)
        {
            return SwarmDb.GetDatabaseForReading().GetFinancialAccountsBudget (Identities, year);
        }


        public Int64 GetBudgetSumCents (int year)
        {
            // TODO: Add support in database
            return (Int64) (SwarmDb.GetDatabaseForReading().GetFinancialAccountsBudget (Identities, year)*100);
        }


        public Int64 GetDeltaCents (DateTime start, DateTime end)
        {
            return SwarmDb.GetDatabaseForReading().GetFinancialAccountBalanceDeltaCents (Identities, start, end);
        }


        public static FinancialAccounts GetAllBelow (FinancialAccount rootAccount)
        {
            Dictionary<int, FinancialAccounts> nodes = GetHashedAccounts (rootAccount.Organization);

            return GetAllBelow (nodes, rootAccount.Identity, 0);
        }


        /*
        public Dictionary<int, BasicGeography> GetGeographyHashtable(int startGeographyId)
        {
            BasicGeography[] nodes = GetTree(startGeographyId);

            Dictionary<int, BasicGeography> result = new Dictionary<int, BasicGeography>();

            foreach (BasicGeography node in nodes)
            {
                result[node.GeographyId] = node;
            }

            return result;
        }*/


        private static FinancialAccounts GetAllBelow (Dictionary<int, FinancialAccounts> accounts, int startNodeId,
            int generation)
        {
            FinancialAccounts result = new FinancialAccounts();

            FinancialAccounts thisList = accounts[startNodeId];

            foreach (FinancialAccount account in thisList)
            {
                if (account.Identity != startNodeId)
                {
                    result.Add (account);

                    // Add recursively

                    FinancialAccounts children = GetAllBelow (accounts, account.Identity, generation + 1);

                    if (children.Count > 0)
                    {
                        foreach (FinancialAccount child in children)
                        {
                            result.Add (child);
                        }
                    }
                }
                else if (generation == 0)
                {
                    // The top parent is special and should be added; the others shouldn't

                    result.Add (account);
                }
            }

            return result;
        }

        public static Dictionary<int, FinancialAccounts> GetHashedAccounts (Organization organization)
        {
            // This generates a Dictionary <int,List<FinancialAccount>>.
            // 
            // Keys are integers corresponding to NodeIds. At each key n,
            // the value is an List<Node> starting with the node n followed by
            // its children.
            //
            // (Later reflection:) O(n) complexity, instead of recursion. Nice!

            Dictionary<int, FinancialAccounts> result = new Dictionary<int, FinancialAccounts>();

            FinancialAccounts allAccounts =
                FromArray (SwarmDb.GetDatabaseForReading().GetFinancialAccountsForOrganization (organization.Identity));

            // Add the nodes.

            foreach (FinancialAccount account in allAccounts)
            {
                FinancialAccounts accounts = new FinancialAccounts();
                accounts.Add (account);

                result[account.Identity] = accounts;
            }

            // Add the children.

            result[0] = new FinancialAccounts(); // for the root-level accounts

            foreach (FinancialAccount account in allAccounts)
            {
                result[account.ParentIdentity].Add (account);
            }

            return result;
        }
    }
}