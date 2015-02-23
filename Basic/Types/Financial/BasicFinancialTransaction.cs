using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicFinancialTransaction : IHasIdentity
    {
        private readonly DateTime dateTime;
        private readonly int financialTransactionId;
        private readonly string importHash;
        private readonly int organizationId;
        private string description;

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
            this (
            original.financialTransactionId, original.organizationId, original.dateTime, original.description,
            original.importHash)
        {
        }

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