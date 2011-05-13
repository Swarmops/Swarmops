using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicFinancialTransaction : IHasIdentity
    {
        public BasicFinancialTransaction (int financialTransactionId, int organizationId, DateTime dateTime,
                                          string description, string importHash)
        {
            this.financialTransactionId = financialTransactionId;
            this.organizationId = organizationId;
            this.dateTime = dateTime;
            this.description = description;
            this.importHash = importHash;
        }

        public BasicFinancialTransaction (BasicFinancialTransaction original) :
            this(
            original.financialTransactionId, original.organizationId, original.dateTime, original.description,
            original.importHash)
        {
        }

        private int organizationId;
        private int financialTransactionId;
        private DateTime dateTime;
        private string description;
        private string importHash;

        public int OrganizationId
        {
            get { return this.organizationId; }
        }

        public int FinancialTransactionId
        {
            get { return this.financialTransactionId; }
        }

        public DateTime DateTime
        {
            get { return this.dateTime; }
        }

        public string Description
        {
            get { return this.description; }
            protected set { this.description = value; }
        }

        public string ImportHash
        {
            get { return this.importHash; }
        }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.financialTransactionId; }
        }

        #endregion
    }
}