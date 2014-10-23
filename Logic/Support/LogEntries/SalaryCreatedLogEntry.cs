using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Logic.Financial;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class SalaryCreatedLogEntry: FinancialActionBase
    {
        [Obsolete("Do not call this constructor directly.", true)]
        public SalaryCreatedLogEntry()
        {
            // empty ctor needed for serializability; should not be called
        }

        public SalaryCreatedLogEntry(Salary salary)
        {
            this.ActingPersonId = 0;
            this.BeneficiaryPersonId = salary.PayrollItem.PersonId;
            this.Amount = salary.NetSalaryCents/100.0;
            this.Currency = salary.Budget.Organization.Currency.Code;
            this.DateTime = DateTime.UtcNow;
            this.FinancialAccountId = salary.Budget.Identity;
            this.FinancialAccountName = salary.Budget.Name;
            this.OrganizationId = salary.PayrollItem.OrganizationId;
            this.OwnerPersonId = salary.PayrollItem.ReportsToPersonId;
            this.OwnerPersonName = salary.PayrollItem.ReportsToPerson.Canonical;
        }
    }
}
