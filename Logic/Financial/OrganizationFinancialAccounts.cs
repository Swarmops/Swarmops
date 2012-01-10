using System;
using System.Collections.Generic;
using System.Text;

namespace Activizr.Logic.Financial
{
    /// <summary>
    /// Accessors for special accounts for a particular organization.
    /// </summary>
    public class OrganizationFinancialAccounts
    {
        static OrganizationFinancialAccounts()
        {
            organizationAccounts = new Dictionary<OrganizationFinancialAccountType, int>();

            // HACK HACK HACK

            organizationAccounts[OrganizationFinancialAccountType.CostsBankFees] = 26;
            organizationAccounts[OrganizationFinancialAccountType.AssetsBankAccountMain] = 1;
            organizationAccounts[OrganizationFinancialAccountType.IncomeDonations] = 4;
            organizationAccounts[OrganizationFinancialAccountType.CostsInfrastructure] = 21;
            organizationAccounts[OrganizationFinancialAccountType.CostsLocalDonationTransfers] = 88;
            organizationAccounts[OrganizationFinancialAccountType.CostsAllocatedFunds] = 124;
            organizationAccounts[OrganizationFinancialAccountType.DebtsExpenseClaims] = 3;
            organizationAccounts[OrganizationFinancialAccountType.DebtsSalary] = 25;
            organizationAccounts[OrganizationFinancialAccountType.DebtsTax] = 86;
            organizationAccounts[OrganizationFinancialAccountType.DebtsInboundInvoices] = 25;
            organizationAccounts[OrganizationFinancialAccountType.DebtsOther] = 25;
            organizationAccounts[OrganizationFinancialAccountType.AssetsOutboundInvoices] = 28;
            organizationAccounts[OrganizationFinancialAccountType.DebtsCapital] = 96;
            organizationAccounts[OrganizationFinancialAccountType.AssetsPaypal] = 2;
            organizationAccounts[OrganizationFinancialAccountType.CostsYearResult] = 97;
        }

        public OrganizationFinancialAccounts (int organizationId)
        {
            if (organizationId != 1)
            {
                throw new NotImplementedException();

                // TODO: Move to database. For now, only org 1 is supported.
            }

            this.organizationId = organizationId;
        }

        private readonly static Dictionary<OrganizationFinancialAccountType, int> organizationAccounts;

        private int organizationId;

        public FinancialAccount AssetsBankAccountMain
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.AssetsBankAccountMain]); }
        }

        public FinancialAccount AssetsPaypal
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.AssetsPaypal]); }
        }

        public FinancialAccount AssetsOutboundInvoices
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.AssetsOutboundInvoices]); }
        }

        public FinancialAccount DebtsExpenseClaims
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.DebtsExpenseClaims]); }
        }

        public FinancialAccount DebtsInboundInvoices
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.DebtsInboundInvoices]); }
        }

        public FinancialAccount DebtsSalary
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.DebtsSalary]); }
        }

        public FinancialAccount DebtsTax
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.DebtsTax]); }
        }

        public FinancialAccount DebtsCapital
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.DebtsCapital]); }
        }

        public FinancialAccount DebtsOther
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.DebtsOther]); }
        }

        public FinancialAccount IncomeDonations
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.IncomeDonations]); }
        }

        public FinancialAccount CostsAllocatedFunds
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.CostsAllocatedFunds]); }
        }

        public FinancialAccount CostsInfrastructure
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.CostsInfrastructure]); }
        }

        public FinancialAccount CostsBankFees
        {
            get { return FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.CostsBankFees]); }
        }

        public FinancialAccount CostsLocalDonationTransfers
        {
            get {
                return
                    FinancialAccount.FromIdentity(
                        organizationAccounts[OrganizationFinancialAccountType.CostsLocalDonationTransfers]); }
        }

        public FinancialAccount CostsYearResult
        {
            get
            {
                return
                    FinancialAccount.FromIdentity(organizationAccounts[OrganizationFinancialAccountType.CostsYearResult]);
            }
        }

        public FinancialAccounts CostsConferences
        {
            get
            {
                if (this.organizationId != 1)
                {
                    throw new NotImplementedException();
                }

                return FinancialAccounts.FromIdentities(new int[] {80, 27, 15, 16, 17, 18, 19});
            }
        }
    }


    //TODO: Move to database
    public enum OrganizationFinancialAccountType
    {
        Unknown,
        AssetsBankAccountMain,
        AssetsOutboundInvoices,
        AssetsPaypal,
        DebtsExpenseClaims,
        DebtsInboundInvoices,
        DebtsSalary,
        DebtsTax,
        DebtsCapital,
        DebtsOther,
        IncomeDonations,
        CostsBankFees,
        CostsInfrastructure,
        CostsLocalDonationTransfers,
        CostsAllocatedFunds,
        CostsConferences,
        CostsYearResult // multi-account type
    }
}
