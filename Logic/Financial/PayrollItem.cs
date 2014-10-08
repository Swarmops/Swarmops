using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class PayrollItem: BasicPayrollItem
    {
        private PayrollItem (BasicPayrollItem basic): base (basic)
        {
            // private constructor
        }

        public static PayrollItem FromBasic (BasicPayrollItem basic)
        {
            return new PayrollItem(basic); // invoke private ctor
        }

        public static PayrollItem FromIdentity (int payrollItemId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetPayrollItem(payrollItemId));
        }


        private new int PersonId
        {
            get
            {
                return base.PersonId;  // hides base's personId
            }
        }

        public Person Person
        {
            get
            {
                return Person.FromIdentity(PersonId);
            }
        }

        public Person ReportsToPerson
        {
            get
            {
                return Person.FromIdentity(ReportsToPersonId);
            }
        }

        public string PersonCanonical
        {
            get
            {
                return this.Person.Canonical;
            }
        }

        public FinancialAccount Budget
        {
            get
            {
                return FinancialAccount.FromIdentity(BudgetId);
            }
        }

        public Country Country
        {
            get
            {
                return Country.FromIdentity(CountryId);
            }
        }

        public Organization Organization
        {
            get
            {
                return Structure.Organization.FromIdentity(this.OrganizationId);
            }
        }
    }
}
