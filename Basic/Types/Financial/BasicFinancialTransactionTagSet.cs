using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicFinancialTransactionTagSet: IHasIdentity
    {
        public BasicFinancialTransactionTagSet (int financialTransactionTagSetId, int financialTransactionTagSetTypeId, int organizationId, int order, bool allowUntagged, int visibilityLevel, int profitLossType)
        {
            this.FinancialTransactionTagSetId = financialTransactionTagSetId;
            this.FinancialTransactionTagSetTypeId = financialTransactionTagSetTypeId;
            this.OrganizationId = organizationId;
            this.Order = order;
            this.AllowUntagged = allowUntagged;
            this.VisibilityLevel = visibilityLevel;
            this.ProfitLossType = profitLossType;
        }

        public BasicFinancialTransactionTagSet (BasicFinancialTransactionTagSet original):
            this (original.FinancialTransactionTagSetId, original.FinancialTransactionTagSetTypeId, original.OrganizationId, original.Order, original.AllowUntagged, original.VisibilityLevel, original.ProfitLossType)
        {
            // copy ctor
        }

        public int FinancialTransactionTagSetId { get; private set; }
        public int FinancialTransactionTagSetTypeId { get; private set; }
        public int OrganizationId { get; private set; }
        public int Order { get; protected set; }
        public bool AllowUntagged { get; protected set; }
        public int ProfitLossType { get; protected set; }
        public int VisibilityLevel { get; protected set; }

        #region Implementation of IHasIdentity

        public int Identity { get { return this.FinancialTransactionTagSetId; } }

        #endregion
    }
}
