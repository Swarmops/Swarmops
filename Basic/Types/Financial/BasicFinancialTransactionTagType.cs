using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicFinancialTransactionTagType : IHasIdentity
    {
        public BasicFinancialTransactionTagType (int financialTransactionTagTypeId,
            int parentFinancialTransactionTagTypeId, int financialTransactionTagSetId, string name, bool active,
            int openedYear, int closedYear)
        {
            FinancialTransactionTagTypeId = financialTransactionTagTypeId;
            ParentFinancialTransactionTagTypeId = parentFinancialTransactionTagTypeId;
            FinancialTransactionTagSetId = financialTransactionTagSetId;
            Name = name;
            Active = active;
            OpenedYear = openedYear;
            ClosedYear = closedYear;
        }

        public BasicFinancialTransactionTagType (BasicFinancialTransactionTagType original) :
            this (
            original.FinancialTransactionTagTypeId, original.ParentFinancialTransactionTagTypeId,
            original.FinancialTransactionTagSetId, original.Name, original.Active, original.OpenedYear,
            original.ClosedYear)
        {
            // copy ctor 
        }

        public int FinancialTransactionTagTypeId { get; private set; }
        public int ParentFinancialTransactionTagTypeId { get; private set; }
        public int FinancialTransactionTagSetId { get; private set; }
        public string Name { get; protected set; }
        public bool Active { get; protected set; }
        public int OpenedYear { get; private set; }
        public int ClosedYear { get; protected set; }

        #region Implementation of IHasIdentity

        public int Identity
        {
            get { return FinancialTransactionTagTypeId; }
        }

        #endregion
    }
}