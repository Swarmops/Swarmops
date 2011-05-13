using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Enums;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicFinancialAccount : IHasIdentity
    {
        public BasicFinancialAccount (int financialAccountId, string name, FinancialAccountType accountType,
                                      int organizationId, int parentFinancialAccountId, int ownerPersonId)
        {
            this.financialAccountId = financialAccountId;
            this.name = name;
            this.accountType = accountType;
            this.organizationId = organizationId;
            this.ownerPersonId = ownerPersonId;
            this.parentFinancialAccountId = parentFinancialAccountId;
        }

        public BasicFinancialAccount (BasicFinancialAccount original) :
            this(original.Identity, original.name, original.accountType, original.organizationId, original.parentFinancialAccountId, original.ownerPersonId)
        {
            // Empty copy constructor
        }


        private int financialAccountId;
        private string name;
        private int organizationId;
        private FinancialAccountType accountType;
        private int ownerPersonId;
        private int parentFinancialAccountId;

        public int FinancialAccountId
        {
            get { return this.financialAccountId; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public int OrganizationId
        {
            get { return this.organizationId; }
        }

        public FinancialAccountType AccountType
        {
            get { return this.accountType; }
        }

        public int OwnerPersonId
        {
            get { return this.ownerPersonId; }
        }

        public int ParentFinancialAccountId
        {
            get { return this.parentFinancialAccountId; }
        }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.financialAccountId; }
        }

        #endregion
    }
}