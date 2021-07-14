using System.Runtime.CompilerServices;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Financial
{
    /// <summary>
    ///     Accessors for special accounts for a particular organization.
    /// </summary>
    public class OrganizationFinancialAccounts
    {
        private readonly int _organizationId;

        public OrganizationFinancialAccounts (int organizationId)
        {
            this._organizationId = organizationId;
        }

        public OrganizationFinancialAccounts (Organization organization)
            : this (organization.Identity)
        {
        }


        public FinancialAccount this [OrganizationFinancialAccountType accountType]
        {
            get
            {
                int accountId = SwarmDb.GetDatabaseForReading().GetOrganizationFinancialAccountId (this._organizationId,
                    accountType);

                if (accountId == 0)
                {
                    return null; // not set
                }

                return FinancialAccount.FromIdentity (accountId);
            }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetOrganizationFinancialAccountId (this._organizationId, accountType,
                    value.Identity);
            }
        }


        // -- SHORTCUTS TO GET ACCOUNTS, FOR CODE READABILITY --

        public FinancialAccount AssetsBankAccountMain
        {
            get { return this[OrganizationFinancialAccountType.AssetsBankAccountMain]; }
        }

        public FinancialAccount AssetsBitcoinCold
        {
            get { return this[OrganizationFinancialAccountType.AssetsBitcoinCold]; }
            set { this[OrganizationFinancialAccountType.AssetsBitcoinCold] = value; }
        }

        public FinancialAccount AssetsBitcoinHot
        {
            get { return this[OrganizationFinancialAccountType.AssetsBitcoinHot]; }
            set { this[OrganizationFinancialAccountType.AssetsBitcoinHot] = value; }
        }

        public FinancialAccount AssetsOutboundInvoices
        {
            get { return this[OrganizationFinancialAccountType.AssetsOutboundInvoices]; }
        }

        public FinancialAccount AssetsOutstandingCashAdvances
        {
            get { return this[OrganizationFinancialAccountType.AssetsOutstandingCashAdvances]; }
        }


        public FinancialAccount AssetsShortTerm
        {
            get { return this[OrganizationFinancialAccountType.AssetsShortTerm]; }
        }

        public FinancialAccount AssetsTaxAdvances
        {
            get { return this[OrganizationFinancialAccountType.AssetsTaxAdvances]; }
        }

        public FinancialAccount AssetsTransfersInProgress
        {
            get { return this[OrganizationFinancialAccountType.AssetsTransfersInProgress]; }
        }

        public FinancialAccount AssetsPaypal
        {
            get { return this[OrganizationFinancialAccountType.AssetsPaypal]; }
            set { this[OrganizationFinancialAccountType.AssetsPaypal] = value; }
        }

        public FinancialAccount AssetsPrivateWithdrawals
        {
            get { return this[OrganizationFinancialAccountType.AssetsPrivateWithdrawals]; }
            set { this[OrganizationFinancialAccountType.AssetsPrivateWithdrawals] = value; }
        }

        public FinancialAccount AssetsVatInbound
        {
            get { return this[OrganizationFinancialAccountType.AssetsVatInbound]; }
            set { this[OrganizationFinancialAccountType.AssetsVatInbound] = value; }
        }

        public FinancialAccount AssetsVatInboundReported
        {
            get { return this[OrganizationFinancialAccountType.AssetsVatInboundReported]; }
            set { this[OrganizationFinancialAccountType.AssetsVatInboundReported] = value; }
        }

        public FinancialAccount AssetsVatInboundUnreported
        {
            get { return this[OrganizationFinancialAccountType.AssetsVatInboundUnreported]; }
            set { this[OrganizationFinancialAccountType.AssetsVatInboundUnreported] = value; }
        }

        public FinancialAccount CostsBankFees
        {
            get { return this[OrganizationFinancialAccountType.CostsBankFees]; }
        }

        public FinancialAccount CostsBitcoinFees
        {
            get { return this[OrganizationFinancialAccountType.CostsBitcoinFees]; }
            set { this[OrganizationFinancialAccountType.CostsBitcoinFees] = value; }
        }

        public FinancialAccount CostsAllocatedFunds
        {
            get { return this[OrganizationFinancialAccountType.CostsAllocatedFunds]; }
        }

        public FinancialAccount CostsCurrencyFluctuations
        {
            get { return this[OrganizationFinancialAccountType.CostsCurrencyFluctuations]; }
            set { this[OrganizationFinancialAccountType.CostsCurrencyFluctuations] = value; }
        }

        public FinancialAccount CostsInfrastructure
        {
            get { return this[OrganizationFinancialAccountType.CostsInfrastructure]; }
        }

        public FinancialAccount CostsLocalDonationTransfers
        {
            get { return this[OrganizationFinancialAccountType.CostsLocalDonationTransfers]; }
        }

        public FinancialAccount CostsYearlyResult
        {
            get { return this[OrganizationFinancialAccountType.CostsYearlyResult]; }
        }

        public FinancialAccount DebtsEquity
        {
            get { return this[OrganizationFinancialAccountType.DebtsEquity]; }
        }

        public FinancialAccount DebtsExpenseClaims
        {
            get { return this[OrganizationFinancialAccountType.DebtsExpenseClaims]; }
        }

        public FinancialAccount DebtsInboundInvoices
        {
            get { return this[OrganizationFinancialAccountType.DebtsInboundInvoices]; }
        }

        public FinancialAccount DebtsPrivateDeposits
        {
            get { return this[OrganizationFinancialAccountType.DebtsPrivateDeposits]; }
            set { this[OrganizationFinancialAccountType.DebtsPrivateDeposits] = value; }
        }

        public FinancialAccount DebtsSalary
        {
            get { return this[OrganizationFinancialAccountType.DebtsSalary]; }
        }

        public FinancialAccount DebtsTax
        {
            get { return this[OrganizationFinancialAccountType.DebtsTax]; }
        }

        public FinancialAccount DebtsVatOutbound
        {
            get { return this[OrganizationFinancialAccountType.DebtsVatOutbound]; }
            set { this[OrganizationFinancialAccountType.DebtsVatOutbound] = value; }
        }

        public FinancialAccount DebtsVatOutboundReported
        {
            get { return this[OrganizationFinancialAccountType.DebtsVatOutboundReported]; }
            set { this[OrganizationFinancialAccountType.DebtsVatOutboundReported] = value; }
        }

        public FinancialAccount DebtsVatOutboundUnreported
        {
            get { return this[OrganizationFinancialAccountType.DebtsVatOutboundUnreported]; }
            set { this[OrganizationFinancialAccountType.DebtsVatOutboundUnreported] = value; }
        }

        public FinancialAccount DebtsEarmarkedVirtualBanking
        {
            get { return this[OrganizationFinancialAccountType.DebtsEarmarkedVirtualBanking]; }
        }

        public FinancialAccount DebtsEarmarkedOtherAssets
        {
            get { return this[OrganizationFinancialAccountType.DebtsEarmarkedOtherAssets]; }
        }

        public FinancialAccount DebtsOther
        {
            get { return this[OrganizationFinancialAccountType.DebtsOther]; }
        }

        public FinancialAccount IncomeCurrencyFluctuations
        {
            get { return this[OrganizationFinancialAccountType.IncomeCurrencyFluctuations]; }
            set { this[OrganizationFinancialAccountType.IncomeCurrencyFluctuations] = value; }
        }

        public FinancialAccount IncomeDonations
        {
            get { return this[OrganizationFinancialAccountType.IncomeDonations]; }
        }

        public FinancialAccount IncomeSales
        {
            get { return this[OrganizationFinancialAccountType.IncomeSales]; }
        }

        public FinancialAccount IncomeMembershipDues
        {
            get { return this[OrganizationFinancialAccountType.IncomeMembershipDues]; }
        }

        public FinancialAccount CostsMiscalculations
        {
            get { return this[OrganizationFinancialAccountType.CostsMiscalculations]; }
        }

        public FinancialAccounts CostsConferences
        {
            get
            {
                FinancialAccounts result = new FinancialAccounts();

                // THIS SERIOUSLY NEEDS OPTIMIZATION, MMMKAY?

                FinancialAccounts costAccounts =
                    FinancialAccounts.ForOrganization (Organization.FromIdentity (this._organizationId),
                        FinancialAccountType.Cost);

                foreach (FinancialAccount account in costAccounts)
                {
                    if (account.IsConferenceParent)
                    {
                        result.Add (account);
                    }
                }

                return result;
            }
        }

        public FinancialAccounts ExpensableBudgets
        {
            // TODO: This needs to return a tree, not a flat list.
            get
            {
                FinancialAccounts result = new FinancialAccounts();

                int yearlyResultId = CostsYearlyResult.Identity;

                FinancialAccounts costAccounts =
                    FinancialAccounts.ForOrganization(Organization.FromIdentity(this._organizationId),
                        FinancialAccountType.Cost);

                foreach (FinancialAccount account in costAccounts)
                {
                    if (account.Identity != yearlyResultId && account.Expensable)
                    // really should be redundant, but still...
                    {
                        result.Add(account);
                    }
                }

                return result;
            }
        }

        public FinancialAccounts InvoiceableBudgetsInbound
        {
            // TODO: This needs to return a tree, not a flat list.
            get
            {
                FinancialAccounts result = new FinancialAccounts();

                int yearlyResultId = CostsYearlyResult.Identity;

                FinancialAccounts costAccounts =
                    FinancialAccounts.ForOrganization(Organization.FromIdentity(this._organizationId),
                        FinancialAccountType.Cost);

                foreach (FinancialAccount account in costAccounts)
                {
                    if (account.Identity != yearlyResultId)
                    // really should be redundant, but still...
                    {
                        result.Add(account);
                    }
                }

                return result;
            }
        }

        public FinancialAccounts InvoiceableBudgetsOutbound
        {
            // TODO: This needs to return a tree, not a flat list.
            get
            {
                FinancialAccounts result = new FinancialAccounts();

                int yearlyResultId = CostsYearlyResult.Identity;

                FinancialAccounts revenueAccounts =
                    FinancialAccounts.ForOrganization (Organization.FromIdentity (this._organizationId),
                        FinancialAccountType.Income);

                foreach (FinancialAccount account in revenueAccounts)
                {
                    result.Add (account);
                }

                return result;
            }
        }

    }
}