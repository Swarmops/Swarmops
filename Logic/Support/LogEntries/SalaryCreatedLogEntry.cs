using System;
using Swarmops.Logic.Financial;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class SalaryCreatedLogEntry : FinancialActionBase
    {
        [Obsolete ("Do not call this constructor directly.", true)]
        public SalaryCreatedLogEntry()
        {
            // empty ctor needed for serializability; should not be called
        }

        public SalaryCreatedLogEntry (Salary salary)
        {
            ActingPersonId = 0;
            BeneficiaryPersonId = salary.PayrollItem.PersonId;
            Amount = salary.NetSalaryCents/100.0;
            CurrencyCode = salary.Budget.Organization.Currency.Code;
            DateTime = DateTime.UtcNow;
            FinancialAccountId = salary.Budget.Identity;
            FinancialAccountName = salary.Budget.Name;
            OrganizationId = salary.PayrollItem.OrganizationId;
            OwnerPersonId = salary.PayrollItem.ReportsToPersonId;
            OwnerPersonName = salary.PayrollItem.ReportsToPersonId != 0? salary.PayrollItem.ReportsToPerson.Canonical: "Nobody";
        }
    }
}