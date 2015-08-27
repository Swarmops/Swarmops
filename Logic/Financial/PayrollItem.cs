using System;
using System.IO;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class PayrollItem : BasicPayrollItem
    {
        private PayrollItem (BasicPayrollItem basic) : base (basic)
        {
            // private constructor
        }

        public static PayrollItem FromPersonAndOrganization (Person person, Organization organization)
        {
            Payroll payrollItems = Payroll.FromArray(SwarmDb.GetDatabaseForReading().GetPayroll (person, organization));

            if (payrollItems.Count > 1)
            {
                throw new InvalidDataException("More than one payroll item for a person/organization combination is not allowed");
            }
            if (payrollItems.Count == 0)
            {
                return null; // no combo. Throw instead? This has not been consistently coded
            }

            return payrollItems[0];
        }

        public static PayrollItem Create (Person person, Organization organization, DateTime employedDate,
            Person reportsToPerson, Country country, Int64 baseSalaryCents, FinancialAccount budget,
            double additiveTaxLevel, int subtractiveTaxLevelId, bool isContractor)
        {
            int payrollItemId = SwarmDb.GetDatabaseForWriting()
                .CreatePayrollItem (person.Identity, organization.Identity, employedDate, reportsToPerson.Identity,
                    country.Identity, baseSalaryCents, budget.Identity, additiveTaxLevel, subtractiveTaxLevelId,
                    isContractor);

            return FromIdentityAggressive (payrollItemId);
        }


        public void CreateAdjustment (PayrollAdjustmentType type, double amount, string description)
        {
            PayrollAdjustment.Create (this, type, amount, description);
        }


        private new int PersonId
        {
            get { return base.PersonId; // hides base's personId
            }
        }

        public Person Person
        {
            get { return Person.FromIdentity (PersonId); }
        }

        public Person ReportsToPerson
        {
            get { return Person.FromIdentity (ReportsToPersonId); }
        }

        public string PersonCanonical
        {
            get { return Person.Canonical; }
        }

        public FinancialAccount Budget
        {
            get { return FinancialAccount.FromIdentity (BudgetId); }
        }

        public Country Country
        {
            get { return Country.FromIdentity (CountryId); }
        }

        public Organization Organization
        {
            get { return Organization.FromIdentity (OrganizationId); }
        }

        public static PayrollItem FromBasic (BasicPayrollItem basic)
        {
            return new PayrollItem (basic); // invoke private ctor
        }

        public static PayrollItem FromIdentity(int payrollItemId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetPayrollItem(payrollItemId));
        }

        private static PayrollItem FromIdentityAggressive(int payrollItemId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetPayrollItem(payrollItemId));
        }
    }
}