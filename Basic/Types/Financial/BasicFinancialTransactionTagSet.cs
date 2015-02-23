using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicFinancialTransactionTagSet : IHasIdentity
    {
        public BasicFinancialTransactionTagSet (int financialTransactionTagSetId, int financialTransactionTagSetTypeId,
            int organizationId, int order, bool allowUntagged, int visibilityLevel, int profitLossType)
        {
            FinancialTransactionTagSetId = financialTransactionTagSetId;
            FinancialTransactionTagSetTypeId = financialTransactionTagSetTypeId;
            OrganizationId = organizationId;
            Order = order;
            AllowUntagged = allowUntagged;
            VisibilityLevel = visibilityLevel;
            ProfitLossType = profitLossType;
        }

        public BasicFinancialTransactionTagSet (BasicFinancialTransactionTagSet original) :
            this (
            original.FinancialTransactionTagSetId, original.FinancialTransactionTagSetTypeId, original.OrganizationId,
            original.Order, original.AllowUntagged, original.VisibilityLevel, original.ProfitLossType)
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

        public int Identity
        {
            get { return FinancialTransactionTagSetId; }
        }

        #endregion
    }
}