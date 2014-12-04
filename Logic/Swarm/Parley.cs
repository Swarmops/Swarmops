using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class Parley : BasicParley, IAttestable
    {
        private Parley(BasicParley basic) : base(basic)
        {
            // empty pvt ctor
        }

        public ParleyOptions Options
        {
            get { return ParleyOptions.ForParley(this); }
        }

        public ParleyAttendees Attendees
        {
            get { return ParleyAttendees.ForParley(this); }
        }

        public Organization Organization
        {
            get { return Organization.FromIdentity(OrganizationId); }
        }

        public Person Person
        {
            get { return Person.FromIdentity(PersonId); }
        }

        public decimal BudgetDecimal
        {
            get { return BudgetCents/100.0m; }
        }

        public decimal GuaranteeDecimal
        {
            get { return GuaranteeCents/100.0m; }
        }

        public decimal AttendanceFeeDecimal
        {
            get { return AttendanceFeeCents/100.0m; }
        }

        public new bool Open
        {
            get { return base.Open; }
            set
            {
                if (base.Open != value)
                {
                    base.Open = value;
                    SwarmDb.GetDatabaseForWriting().SetParleyOpen(Identity, value);
                }
            }
        }

        public new int BudgetId
        {
            get
            {
                if (base.BudgetId < 0)
                {
                    return -base.BudgetId;
                }

                return base.BudgetId;
            }
        }

        private bool AttestedOnce
        {
            get
            {
                BasicFinancialValidation[] validations =
                    SwarmDb.GetDatabaseForReading().GetFinancialValidations(FinancialDependencyType.Parley,
                        Identity);

                foreach (BasicFinancialValidation validation in validations)
                {
                    if (validation.ValidationType == FinancialValidationType.Attestation)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        #region Implementation of IAttestable

        public FinancialAccount ParentBudget
        {
            get
            {
                // If the financial account for this parley is uninitialized, the BudgetId of base is negative to indicate that.

                if (base.BudgetId < 0)
                {
                    return FinancialAccount.FromIdentity(-base.BudgetId);
                }
                return this.Budget.Parent;
            }
        }

        public void Attest(Person attester)
        {
            if (Attested)
            {
                return;
            }

            // If needed, create new account for parley

            FinancialAccount ourBudget;
            FinancialAccount parentBudget;
            int year = DateTime.Today.Year;

            if (base.BudgetId < 0) // no account created yet
            {
                ourBudget = FinancialAccount.Create(Budget.Organization,
                    "Conf: " + Name,
                    FinancialAccountType.Cost,
                    Budget);

                parentBudget = Budget;

                base.BudgetId = ourBudget.Identity;
                ourBudget.Owner = Person;
                SwarmDb.GetDatabaseForWriting().SetParleyBudget(Identity, ourBudget.Identity);
            }
            else
            {
                // The budget has been created already - we should already be initialized. Verify this
                // by checking that we were already attested once.

                if (!AttestedOnce)
                {
                    throw new InvalidOperationException(
                        "Budget exists despite parley not having been attested. This should not be possible.");
                }

                ourBudget = Budget;
                parentBudget = ParentBudget;
            }

            ourBudget.SetBudgetCents(DateTime.Today.Year, -BudgetCents);
            parentBudget.SetBudgetCents(DateTime.Today.Year,
                parentBudget.GetBudgetCents(year) + BudgetCents); // cost budgets are negative

            // Reserve the guarantee money

            FinancialTransaction guaranteeFundsTx = FinancialTransaction.Create(OrganizationId, DateTime.Now,
                "Conference #" +
                Identity + " Guarantee");
            guaranteeFundsTx.AddRow(Budget, -GuaranteeCents, attester);
            guaranteeFundsTx.AddRow(Budget.Parent, GuaranteeCents, attester);

            // Finally, set as attested

            PWEvents.CreateEvent(
                EventSource.PirateWeb, EventType.ParleyAttested, attester.Identity,
                OrganizationId, 0, 0, Identity, string.Empty);

            base.Attested = true;
            SwarmDb.GetDatabaseForWriting().SetParleyAttested(Identity, true);
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Attestation,
                FinancialDependencyType.Parley, Identity,
                DateTime.Now, attester.Identity, (double) (GuaranteeDecimal));
        }

        public void Deattest(Person deattester)
        {
            throw new NotImplementedException();
            /*
            base.Attested = false;
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Deattestation,
                                                             FinancialDependencyType.Parley, this.Identity,
                                                             DateTime.Now, deattester.Identity, (double)(this.GuaranteeDecimal+this.BudgetDecimal));*/

            // TODO: Remove budget, remove financial account, set unattested

            // this.BudgetId = this.Budget.ParentIdentity;
        }

        #endregion

        public FinancialAccount Budget
        {
            get { return FinancialAccount.FromIdentity(BudgetId); }
        }

        public static Parley FromBasic(BasicParley basic)
        {
            return new Parley(basic);
        }

        public static Parley FromIdentity(int parleyId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetParley(parleyId));
        }

        public static Parley Create(Organization organization, Person person, FinancialAccount budgetInitial,
            string name, Geography geography, string description, string informationUrl, DateTime startDate,
            DateTime endDate, Int64 budgetCents, Int64 guaranteeCents, Int64 attendanceFeeCents)
        {
            Parley newParley =
                FromIdentity(SwarmDb.GetDatabaseForWriting().CreateParley(organization.Identity, person.Identity,
                    -(budgetInitial.Identity), name, geography.Identity,
                    description, informationUrl, startDate, endDate,
                    budgetCents, guaranteeCents, attendanceFeeCents));

            PWEvents.CreateEvent(EventSource.PirateWeb, EventType.ParleyCreated, person.Identity, organization.Identity,
                0, 0, newParley.Identity, string.Empty);

            return newParley;
        }

        public ParleyOption CreateOption(string description, Int64 amountCents)
        {
            return ParleyOption.Create(this, description, amountCents);
        }

        public ParleyAttendee CreateAttendee(string firstName, string lastName, string email, bool asGuest)
        {
            Person newAttendee = Person.Create(firstName + "|" + lastName, email, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty,
                Organization.DefaultCountry.Code, DateTime.Now, PersonGender.Unknown);

            return ParleyAttendee.Create(this, newAttendee, asGuest);
        }

        public void CloseBudget(Person closingPerson)
        {
            Int64 remainingFunds = -Budget.GetDeltaCents(CreatedDateTime, DateTime.Now);

            FinancialTransaction transaction = FinancialTransaction.Create(OrganizationId,
                DateTime.Now,
                "Closing conference #" + Identity);
            transaction.AddRow(Budget, remainingFunds, closingPerson);
            transaction.AddRow(Budget.Parent, -remainingFunds, closingPerson);
        }

        public void CancelBudget()
        {
            int year = DateTime.Today.Year;

            // Adjust budgets. (NB: Cost budgets are negative, but the bookkeeping cost funds are positive. Some sign reversal is necessary.)

            if (BudgetId > 0) // Budget attested and allocated
            {
                Budget.Parent.SetBudgetCents(year,
                    Budget.Parent.GetBudgetCents(year) - BudgetCents); // cost budgets are negative
                Budget.SetBudgetCents(year, 0);
            }
        }
    }
}