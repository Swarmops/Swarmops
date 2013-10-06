using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicFinancialTransactionTagSetType: IHasIdentity
    {
        public BasicFinancialTransactionTagSetType (int financialTransactionTagSetTypeId, string resourceName)
        {
            this.FinancialTransactionTagSetTypeId = financialTransactionTagSetTypeId;
            this.ResourceName = resourceName;
        }

        public BasicFinancialTransactionTagSetType (BasicFinancialTransactionTagSetType original):
            this (original.FinancialTransactionTagSetTypeId, original.ResourceName)
        {
            // copy ctor
        }

        public int FinancialTransactionTagSetTypeId { get; private set; }
        public string ResourceName { get; private set; }


        #region Implementation of IHasIdentity

        public int Identity { get { return this.FinancialTransactionTagSetTypeId; } }

        #endregion
    }
}
