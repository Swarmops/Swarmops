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
    public class Parley: BasicParley, IAttestable
    {
        private Parley (BasicParley basic): base (basic)
        {
            // empty pvt ctor
        }

        public static Parley FromBasic (BasicParley basic)
        {
            return new Parley(basic);
        }

        public static Parley FromIdentity (int parleyId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetParley(parleyId));
        }

        public static Parley Create (Organization organization, Person person, FinancialAccount budgetInitial, string name, Geography geography, string description, string informationUrl, DateTime startDate, DateTime endDate, Int64 budgetCents, Int64 guaranteeCents, Int64 attendanceFeeCents)
        {
            Parley newParley = 
                FromIdentity(SwarmDb.GetDatabaseForWriting().CreateParley(organization.Identity, person.Identity,
                                                                 -(budgetInitial.Identity), name, geography.Identity,
                                                                 description, informationUrl, startDate, endDate,
                                                                 budgetCents, guaranteeCents, attendanceFeeCents));

            PWEvents.CreateEvent(EventSource.PirateWeb, EventType.ParleyCreated, person.Identity, organization.Identity, 0, 0, newParley.Identity, string.Empty); 

            return newParley;
        }

        public ParleyOption CreateOption (string description, Int64 amountCents)
        {
            return ParleyOption.Create(this, description, amountCents);
        }

        public ParleyOptions Options
        {
            get { return ParleyOptions.ForParley(this); }
        }

        public ParleyAttendees Attendees
        {
            get { return ParleyAttendees.ForParley(this); }
        }

        public ParleyAttendee CreateAttendee (string firstName, string lastName, string email, bool asGuest)
        {
            Person newAttendee = Person.Create(firstName + "|" + lastName, email, string.Empty, string.Empty,
                                               string.Empty, string.Empty, string.Empty,
                                               this.Organization.DefaultCountry.Code, DateTime.Now, PersonGender.Unknown);

            return ParleyAttendee.Create(this, newAttendee, asGuest);
        }

        public Organization Organization
        {
            get { return Structure.Organization.FromIdentity(this.OrganizationId); }
        }

        public Person Person
        {
            get { return Person.FromIdentity(this.PersonId); }
        }

        public FinancialAccount Budget
        {
            get { return FinancialAccount.FromIdentity(this.BudgetId); }
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
            get
            {
                return base.Open;
            }
            set
            {
                if (base.Open != value)
                {
                    base.Open = value;
                    SwarmDb.GetDatabaseForWriting().SetParleyOpen(this.Identity, value);
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

        #region Implementation of IAttestable
         
        public void Attest(Person attester)
        {
            if (this.Attested)
            {
                return;
            }

            // If needed, create new account for parley

            FinancialAccount ourBudget;
            FinancialAccount parentBudget;
            int year = DateTime.Today.Year;

            if (base.BudgetId < 0) // no account created yet
            {
                ourBudget = FinancialAccount.Create(this.Budget.Organization,
                                                    "Conf: " + this.Name,
                                                    FinancialAccountType.Cost,
                                                    this.Budget);

                parentBudget = this.Budget;

                base.BudgetId = ourBudget.Identity;
                ourBudget.Owner = this.Person;
                SwarmDb.GetDatabaseForWriting().SetParleyBudget(this.Identity, ourBudget.Identity);
            }
            else
            {
                // The budget has been created already - we should already be initialized. Verify this
                // by checking that we were already attested once.

                if (!this.AttestedOnce)
                {
                    throw new InvalidOperationException(
                        "Budget exists despite parley not having been attested. This should not be possible.");
                }

                ourBudget = this.Budget;
                parentBudget = this.ParentBudget;
            }

            ourBudget.SetBudgetCents(DateTime.Today.Year, -this.BudgetCents);
            parentBudget.SetBudgetCents(DateTime.Today.Year,
                                    parentBudget.GetBudgetCents(year) + this.BudgetCents);  // cost budgets are negative

            // Reserve the guarantee money

            FinancialTransaction guaranteeFundsTx = FinancialTransaction.Create(this.OrganizationId, DateTime.Now,
                                                                                "Conference #" +
                                                                                this.Identity.ToString() + " Guarantee");
            guaranteeFundsTx.AddRow(this.Budget, -this.GuaranteeCents, attester);
            guaranteeFundsTx.AddRow(this.Budget.Parent, this.GuaranteeCents, attester);

            // Finally, set as attested

            PWEvents.CreateEvent(
                EventSource.PirateWeb, EventType.ParleyAttested, attester.Identity,
                this.OrganizationId, 0, 0, this.Identity, string.Empty);

            base.Attested = true;
            SwarmDb.GetDatabaseForWriting().SetParleyAttested(this.Identity, true);
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Attestation,
                                                             FinancialDependencyType.Parley, this.Identity,
                                                             DateTime.Now, attester.Identity, (double)(this.GuaranteeDecimal));


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


        public FinancialAccount ParentBudget
        {
            get
            {
                // If the financial account for this parley is uninitialized, the BudgetId of base is negative to indicate that.

                if (base.BudgetId < 0)
                {
                    return FinancialAccount.FromIdentity(-base.BudgetId);
                }
                else
                {
                    return Budget.Parent;
                }
            }
        }


        #endregion

        public void CloseBudget(Person closingPerson)
        {
            Int64 remainingFunds = -this.Budget.GetDeltaCents(this.CreatedDateTime, DateTime.Now);

            FinancialTransaction transaction = FinancialTransaction.Create(this.OrganizationId,
                                                                           DateTime.Now,
                                                                           "Closing conference #" + this.Identity);
            transaction.AddRow(this.Budget, remainingFunds, closingPerson);
            transaction.AddRow(this.Budget.Parent, -remainingFunds, closingPerson);
        }

        public void CancelBudget()
        {
            int year = DateTime.Today.Year;

            // Adjust budgets. (NB: Cost budgets are negative, but the bookkeeping cost funds are positive. Some sign reversal is necessary.)

            if (this.BudgetId > 0)  // Budget attested and allocated
            {
                this.Budget.Parent.SetBudgetCents(year,
                                       this.Budget.Parent.GetBudgetCents(year) - this.BudgetCents);  // cost budgets are negative
                this.Budget.SetBudgetCents(year, 0);
            }
        }

        private bool AttestedOnce
        {
            get
            {
                BasicFinancialValidation[] validations =
                    SwarmDb.GetDatabaseForReading().GetFinancialValidations(FinancialDependencyType.Parley,
                                                                            this.Identity);

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
    }
}
