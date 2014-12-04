using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicPayrollItem : IHasIdentity
    {
        public BasicPayrollItem(int payrollItemId, int personId, int organizationId, int countryId,
            DateTime employedDate,
            int reportsToPersonId, Int64 baseSalaryCents, int budgetId, bool open, DateTime terminatedDate,
            int subtractiveTaxLevelId, double additiveTaxLevel)
        {
            PayrollItemId = payrollItemId;
            PersonId = personId;
            OrganizationId = organizationId;
            CountryId = countryId;
            EmployedDate = employedDate;
            ReportsToPersonId = reportsToPersonId;
            BaseSalaryCents = baseSalaryCents;
            BudgetId = budgetId;
            Open = open;
            TerminatedDate = terminatedDate;
            SubtractiveTaxLevelId = subtractiveTaxLevelId;
            AdditiveTaxLevel = additiveTaxLevel;
        }

        public BasicPayrollItem(BasicPayrollItem original)
            : this(original.PayrollItemId, original.PersonId, original.OrganizationId, original.CountryId,
                original.EmployedDate, original.ReportsToPersonId, original.BaseSalaryCents, original.BudgetId,
                original.Open, original.TerminatedDate, original.SubtractiveTaxLevelId, original.AdditiveTaxLevel)
        {
            // empty copy constructor
        }


        /// <summary>
        ///     Gets the identity of the payroll item. Also accessible through Identity.
        /// </summary>
        public int PayrollItemId { get; private set; }

        /// <summary>
        ///     The employed person Id.
        /// </summary>
        public int PersonId { get; private set; }

        /// <summary>
        ///     The employing organization.
        /// </summary>
        public int OrganizationId { get; private set; }

        /// <summary>
        ///     The employment country (for tax laws and such).
        /// </summary>
        public int CountryId { get; private set; }

        /// <summary>
        ///     The date of employment.
        /// </summary>
        public DateTime EmployedDate { get; private set; }

        /// <summary>
        ///     The person above this person in the org chart.
        /// </summary>
        public int ReportsToPersonId { get; protected set; }

        /// <summary>
        ///     The base gross montly salary in the local currency.
        /// </summary>
        public Int64 BaseSalaryCents { get; protected set; }

        /// <summary>
        ///     The budget charged with the costs of this employment.
        /// </summary>
        public int BudgetId { get; protected set; }

        /// <summary>
        ///     True if this employment is still active.
        /// </summary>
        public bool Open { get; protected set; }

        /// <summary>
        ///     If closed, this field contains the date the employment ended.
        /// </summary>
        public DateTime TerminatedDate { get; protected set; }

        /// <summary>
        ///     The tax level code of the country of employment. Subtracted from gross salary to get net salary.
        /// </summary>
        public int SubtractiveTaxLevelId { get; protected set; }

        /// <summary>
        ///     The tax level, as a fraction, paid by the organization on top of the gross salary.
        /// </summary>
        public double AdditiveTaxLevel { get; protected set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return PayrollItemId; }
        }

        #endregion
    }
}