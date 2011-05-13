using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Enums;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicFinancialValidation: IHasIdentity
    {
        public BasicFinancialValidation (int financialValidationId, FinancialValidationType validationType, int personId, DateTime dateTime, FinancialDependencyType dependencyType, int foreignId)
        {
            this.FinancialValidationId = financialValidationId;
            this.ValidationType = validationType;
            this.PersonId = personId;
            this.DateTime = dateTime;
            this.DependencyType = dependencyType;
            this.ForeignId = foreignId;
        }

        public BasicFinancialValidation (BasicFinancialValidation original)
            :this (original.Identity, original.ValidationType, original.PersonId, original.DateTime, original.DependencyType, original.ForeignId)
        {
            // empty copy ctor
        }

        public int FinancialValidationId { get; private set; }
        public FinancialValidationType ValidationType { get; private set; }
        public int PersonId { get; private set; }
        public DateTime DateTime { get; private set; }
        public FinancialDependencyType DependencyType { get; private set; }
        public int ForeignId { get; private set; }
        public int Identity
        {
            get { return this.FinancialValidationId; }
        }
    }
}
