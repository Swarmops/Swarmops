using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Logic.Structure;

namespace Activizr.Logic.Financial
{
    [Serializable]
    public class YearlyReport
    {
        public static YearlyReport Create (Organization organization, int year, FinancialAccountType accountType)
        {
            YearlyReport report = new YearlyReport();
            report.Organization = organization;
            report.Year = year;
            report._accountType = accountType;

            // Get accounts

            FinancialAccounts accounts = FinancialAccounts.ForOrganization(organization, accountType);

            // Remove unwanted accounts

            FinancialAccount resultsAccount = organization.FinancialAccounts.CostsYearlyResult;

            foreach (FinancialAccount account in accounts)
            {
                // For now, just remove the "results" account. TODO: Remove inactive accounts, too.

                if (account.Identity == resultsAccount.Identity)
                {
                    accounts.Remove(account);
                    break;
                }
            }

            // Build tree (there should be a template for this)

            report._treeMap = new Dictionary<int, List<FinancialAccount>>();

            foreach (FinancialAccount account in accounts)
            {
                if (!report._treeMap.ContainsKey(account.ParentIdentity))
                {
                    report._treeMap[account.ParentIdentity] = new List<FinancialAccount>();
                }

                report._treeMap[account.ParentIdentity].Add(account);
            }


            if (report._treeMap[0].Count > 3)
            {
                // regroup list

                report.AggregateAccounts();
            }

            FinancialAccounts orderedList = new FinancialAccounts(); // This list is guaranteed to have parents before children

            report.PopulateOrderedList(orderedList, 0);  // recursively add nodes parents-first
            report.PopulateLookups(orderedList);         // populate the lookup tables for results per account
            report.PopulateTotals();

            report.ReportLines = new List<YearlyReportLine>();
            report.RecurseAddLines(report.ReportLines, 0);

            return report;
        }


        private FinancialAccountType _accountType;


        private void AggregateAccounts()
        {
            const int assetIdentity = 1000000001;
            const int debtIdentity = 1000000002;
            const int incomeIdentity = 1000000003;
            const int costIdentity = 1000000004;

            Dictionary<FinancialAccountType, int> remapLookup = new Dictionary<FinancialAccountType, int>();

            remapLookup[FinancialAccountType.Asset] = assetIdentity;
            remapLookup[FinancialAccountType.Debt] = debtIdentity;
            remapLookup[FinancialAccountType.Income] = incomeIdentity;
            remapLookup[FinancialAccountType.Cost] = costIdentity;

            List<FinancialAccount> newRootLevel = new List<FinancialAccount>();

            int equityIdentity = _treeMap[0][0].Organization.FinancialAccounts.DebtsEquity.Identity;

            if (_accountType == FinancialAccountType.Balance)
            {
                newRootLevel.Add(FinancialAccount.FromBasic(new BasicFinancialAccount(assetIdentity, "%ASSET_ACCOUNTGROUP%", FinancialAccountType.Asset, 0, 0, 0)));
                newRootLevel.Add(FinancialAccount.FromBasic(new BasicFinancialAccount(debtIdentity, "%DEBT_ACCOUNTGROUP%", FinancialAccountType.Debt, 0, 0, 0)));
            }
            else if (_accountType == FinancialAccountType.Result)
            {
                newRootLevel.Add(FinancialAccount.FromBasic(new BasicFinancialAccount(incomeIdentity, "%INCOME_ACCOUNTGROUP%", FinancialAccountType.Income, 0, 0, 0)));
                newRootLevel.Add(FinancialAccount.FromBasic(new BasicFinancialAccount(costIdentity, "%COST_ACCOUNTGROUP%", FinancialAccountType.Cost, 0, 0, 0)));
            }
            else
            {
                throw new InvalidOperationException("AccountType other than Balance or Result passed to YearlyReport.AggregateAccounts()");
            }

            foreach (FinancialAccount account in _treeMap[0])
            {
                if (account.Identity == equityIdentity)
                {
                    newRootLevel.Add(account);
                }
                else
                {
                    if (!_treeMap.ContainsKey(remapLookup[account.AccountType]))
                    {
                        _treeMap[remapLookup[account.AccountType]] = new List<FinancialAccount>();
                    }

                    _treeMap[remapLookup[account.AccountType]].Add(account);
                }
            }

            _treeMap[0] = newRootLevel;
        }


        private void PopulateTotals()
        {
            this.Totals = new YearlyReportNode();

            this.Totals.PreviousYear = PopulateOneTotal(_singleLookups[0]);
            this.Totals.ThisYear = PopulateOneTotal(_singleLookups[5]);

            for (int quarter = 0; quarter < 4; quarter++)
            {
                this.Totals.Quarters[quarter] = PopulateOneTotal(_singleLookups[quarter + 1]);
            }
        }

        private Int64 PopulateOneTotal (Dictionary<int, Int64> lookup)
        {
            Int64[] allValues = lookup.Values.ToArray();

            return allValues.Sum();
        }


        private void RecurseAddLines (List<YearlyReportLine> list, int renderNodeId)
        {
            foreach (FinancialAccount account in _treeMap[renderNodeId])
            {
                YearlyReportLine newLine = new YearlyReportLine();
                newLine.AccountId = account.Identity;
                newLine.AccountName = account.Name;
                newLine.AccountValues = CreateYearlyReportNode(account.Identity, _singleLookups);
                newLine.AccountTreeValues = CreateYearlyReportNode(account.Identity, _treeLookups);

                if (_treeMap.ContainsKey(account.Identity))
                {
                    RecurseAddLines(newLine.Children, account.Identity);
                }

                list.Add(newLine);
            }
        }


        private YearlyReportNode CreateYearlyReportNode (int accountId, Dictionary<int, Int64>[] lookup)
        {
            YearlyReportNode node = new YearlyReportNode();
            node.PreviousYear = lookup[0][accountId];
            
            for (int quarter = 1; quarter <= 4; quarter++)
            {
                node.Quarters[quarter - 1] = lookup[quarter][accountId];
            }

            node.ThisYear = lookup[5][accountId];
            return node;
        }


        private void PopulateOrderedList(FinancialAccounts orderedList, int renderNodeId)
        {
            foreach (FinancialAccount account in _treeMap[renderNodeId])
            {
                orderedList.Add(account);

                if (_treeMap.ContainsKey(account.Identity))
                {
                    PopulateOrderedList(orderedList, account.Identity); // recursive call
                }
            }
        }

        private Dictionary<int, List<FinancialAccount>> _treeMap;
        private Dictionary<int, Int64>[] _singleLookups;
        private Dictionary<int, Int64>[] _treeLookups;


        private void PopulateLookups(FinancialAccounts accounts)
        {
            _singleLookups = new Dictionary<int, Int64>[6];
            _treeLookups = new Dictionary<int, Int64>[6];

            for (int index = 0; index < 6; index++)
            {
                _treeLookups[index] = new Dictionary<int, Int64>();
                _singleLookups[index] = new Dictionary<int, Int64>();
            }

            DateTime[] quarterBoundaries =
            {
                new DateTime(this.Year, 1, 1), new DateTime(this.Year, 3, 1), new DateTime(this.Year, 6, 1),
                new DateTime(this.Year, 9, 1), new DateTime(this.Year + 1, 1, 1)
            };

            // 1) Actually, the accounts are already sorted. Or are supposed to be, anyway,
            // since FinancialAccounts.ForOrganization gets the _tree_ rather than the flat list.

            // 2) Add all values to the accounts.

            foreach (FinancialAccount account in accounts)
            {
                // Find this year's inbound

                if (_accountType == FinancialAccountType.Result)
                {
                    _singleLookups[0][account.Identity] = account.GetDeltaCents(new DateTime(this.Year - 1, 1, 1),
                                                                                new DateTime(this.Year, 1, 1));
                }
                else if (_accountType == FinancialAccountType.Balance)
                {
                    _singleLookups[0][account.Identity] = account.GetDeltaCents(new DateTime(1900, 1, 1),
                                                                                new DateTime(this.Year, 1, 1));
                }
                else
                {
                    throw new InvalidOperationException("Can only calculate yearly reports for balance or P&L statements");
                }

                // Find quarter diffs

                for (int quarter = 0; quarter < 4; quarter++)
                {
                    _singleLookups[quarter + 1][account.Identity] = account.GetDeltaCents(quarterBoundaries[quarter],
                                                                                    quarterBoundaries[quarter + 1]);
                }

                // Find outbound

                if (_accountType == FinancialAccountType.Result)
                {
                    _singleLookups[5][account.Identity] = account.GetDeltaCents(new DateTime(this.Year, 1, 1),
                                                                                new DateTime(this.Year + 1, 1, 1));
                }
                else if (_accountType == FinancialAccountType.Balance)
                {
                    _singleLookups[5][account.Identity] = account.GetDeltaCents(new DateTime(1900, 1, 1),
                                                                                new DateTime(this.Year + 1, 1, 1));
                }
                else
                {
                    throw new InvalidOperationException("Can only calculate yearly reports for balance or P&L statements");
                }

                // copy to treeLookups

                for (int index = 0; index < 6; index++)
                {
                    _treeLookups[index][account.Identity] = _singleLookups[index][account.Identity];
                }
            }

            // 3) Add all children's values to parents

            for (int index = 0; index < 6; index++)
            {
                AddChildrenValuesToParents(_treeLookups[index], accounts);
            }

            // Done.
        }


        private void AddChildrenValuesToParents(Dictionary<int, Int64> lookup, FinancialAccounts accounts)
        {
            // Iterate backwards and add any value to its parent's value, as they are sorted in tree order.

            for (int index = accounts.Count - 1; index >= 0; index--)
            {
                int parentFinancialAccountId = accounts[index].ParentFinancialAccountId;
                int accountId = accounts[index].Identity;

                if (parentFinancialAccountId != 0)
                {
                    lookup[parentFinancialAccountId] += lookup[accountId];
                }
            }
        }




        public List<YearlyReportLine> ReportLines;
        public YearlyReportNode Totals;
        public Organization Organization;
        public int Year;
    }


    [Serializable]
    public class YearlyReportLine
    {
        public YearlyReportLine()
        {
            Children = new List<YearlyReportLine>();
        }

        public int AccountId;
        public string AccountName;
        public YearlyReportNode AccountValues;
        public YearlyReportNode AccountTreeValues;

        public List<YearlyReportLine> Children;
    }

    [Serializable]
    public class YearlyReportNode
    {
        public YearlyReportNode()
        {
            Quarters = new Int64[4];
        }

        public Int64 PreviousYear;
        public Int64[] Quarters;
        public Int64 ThisYear;
    }
}
