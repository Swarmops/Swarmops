using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicFinancialTransactionTagSetType : IHasIdentity
    {
        public BasicFinancialTransactionTagSetType(int financialTransactionTagSetTypeId, string resourceName)
        {
            FinancialTransactionTagSetTypeId = financialTransactionTagSetTypeId;
            ResourceName = resourceName;
        }

        public BasicFinancialTransactionTagSetType(BasicFinancialTransactionTagSetType original) :
            this(original.FinancialTransactionTagSetTypeId, original.ResourceName)
        {
            // copy ctor
        }

        public int FinancialTransactionTagSetTypeId { get; private set; }
        public string ResourceName { get; private set; }

        #region Implementation of IHasIdentity

        public int Identity
        {
            get { return FinancialTransactionTagSetTypeId; }
        }

        #endregion
    }
}