using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicFinancialTransactionTagSet: IHasIdentity
    {
        public BasicFinancialTransactionTagSet (int financialTransactionTagSetId, int financialTransactionTagSetTypeId, int organizationId, int order, bool allowUntagged, int profitLossType)
        {
            this.FinancialTransactionTagSetId = financialTransactionTagSetId;
            this.FinancialTransactionTagSetTypeId = financialTransactionTagSetTypeId;
            this.OrganizationId = organizationId;
            this.Order = order;
            this.AllowUntagged = allowUntagged;
            this.ProfitLossType = profitLossType;
        }

        public BasicFinancialTransactionTagSet (BasicFinancialTransactionTagSet original):
            this (original.FinancialTransactionTagSetId, original.FinancialTransactionTagSetTypeId, original.OrganizationId, original.Order, original.AllowUntagged, original.ProfitLossType)
        {
            // copy ctor
        }

        public int FinancialTransactionTagSetId { get; private set; }
        public int FinancialTransactionTagSetTypeId { get; private set; }
        public int OrganizationId { get; private set; }
        public int Order { get; protected set; }
        public bool AllowUntagged { get; protected set; }
        public int ProfitLossType { get; protected set; }

        #region Implementation of IHasIdentity

        public int Identity { get { return this.FinancialTransactionTagSetId; } }

        #endregion
    }
}
