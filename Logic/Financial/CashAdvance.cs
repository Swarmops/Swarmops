using System;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.LogEntries;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    [Serializable]
    public class CashAdvance : BasicCashAdvance, IPayable
    {
        #region Construction and creation

        private CashAdvance (BasicCashAdvance basic) : base (basic)
        {
        }

        public static CashAdvance FromBasic (BasicCashAdvance basic)
        {
            return new CashAdvance (basic);
        }

        public static CashAdvance FromIdentity (int cashAdvanceId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetCashAdvance (cashAdvanceId));
        }

        public static CashAdvance FromIdentityAggressive (int cashAdvanceId)
        {
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetCashAdvance (cashAdvanceId));
        }

        public static CashAdvance Create (Organization organization, Person forPerson, Person createdByPerson,
            Int64 amountCents, FinancialAccount budget, string description)
        {
            CashAdvance newAdvance =
                FromIdentityAggressive (SwarmDb.GetDatabaseForWriting().CreateCashAdvance (forPerson.Identity,
                    createdByPerson.Identity,
                    organization.Identity,
                    budget.Identity, amountCents,
                    description));

            OutboundComm.CreateNotificationApprovalNeeded (budget, forPerson, string.Empty, amountCents/100.0,
                description, NotificationResource.CashAdvance_Requested);
            // Slightly misplaced logic, but failsafer here
            SwarmopsLogEntry.Create (forPerson,
                new CashAdvanceRequestedLogEntry (createdByPerson, forPerson, amountCents/100.0, budget, description),
                newAdvance);

            // Clear a cache
            FinancialAccount.ClearApprovalAdjustmentsCache (organization);

            return newAdvance;
        }

        #endregion

        #region Implementation of IApprovable

        public void Approve (Person approvingPerson)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.Approval,
                FinancialDependencyType.CashAdvance, Identity, DateTime.UtcNow, approvingPerson.Identity, AmountCents/100.0);
            SwarmDb.GetDatabaseForWriting().SetCashAdvanceAttested (Identity, true, approvingPerson.Identity);

            OutboundComm.CreateNotificationOfFinancialValidation (Budget, Person, AmountCents/100.0, Description,
                NotificationResource.CashAdvance_Approved);
        }

        public void RetractApproval (Person retractingPerson)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.UndoApproval,
                FinancialDependencyType.CashAdvance, Identity, DateTime.UtcNow, retractingPerson.Identity, AmountCents/100.0);
            SwarmDb.GetDatabaseForWriting().SetCashAdvanceAttested (Identity, false, Person.NobodyId);

            OutboundComm.CreateNotificationOfFinancialValidation (Budget, Person, AmountCents/100.0, Description,
                NotificationResource.CashAdvance_ApprovalRetracted);
        }

        public void DenyApproval (Person denyingPerson, string reason)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Kill,
                FinancialDependencyType.CashAdvance, Identity, DateTime.UtcNow, denyingPerson.Identity, AmountCents / 100.0);

            OutboundComm.CreateNotificationOfFinancialValidation(Budget, Person, AmountCents / 100.0, Description,
                NotificationResource.CashAdvance_Denied, reason);
            Attested = false;
            Open = false;
        }


        #endregion



        public new int OrganizationSequenceId
        {
            get
            {
                if (base.OrganizationSequenceId == 0)
                {
                    // This case is for legacy installations before DbVersion 66, when
                    // OrganizationSequenceId was added for each new cash advance

                    SwarmDb db = SwarmDb.GetDatabaseForWriting();
                    base.OrganizationSequenceId = db.SetCashAdvanceSequence(this.Identity);
                    return base.OrganizationSequenceId;
                }

                return base.OrganizationSequenceId;
            }
        }

        public Person Person
        {
            get { return Person.FromIdentity (PersonId); }
        }

        public new bool Open
        {
            get { return base.Open; }
            set
            {
                if (base.Open == value)
                {
                    return;
                }

                SwarmDb.GetDatabaseForWriting().SetCashAdvanceOpen (Identity, value);
                base.Open = value;
            }
        }

        public new bool PaidOut
        {
            get { return base.PaidOut; }
            set
            {
                if (base.PaidOut == value)
                {
                    return;
                }

                SwarmDb.GetDatabaseForWriting().SetCashAdvancePaidOut (Identity, value);
                base.PaidOut = value;
            }
        }


        public Payout PayoutOut
        {
            get
            {
                if (!PaidOut)
                {
                    return null; // or throw?
                }

                int payoutId =
                    SwarmDb.GetDatabaseForReading()
                        .GetPayoutIdFromDependency (this, FinancialDependencyType.CashAdvance);

                if (payoutId == 0)
                {
                    return null; // or throw? When expense claim system was being phased in, payouts did not exist
                }

                return Payout.FromIdentity (payoutId);
            }
        }

        public Payout PayoutBack
        {
            get
            {
                if (!PaidOut)
                {
                    return null; // or throw?
                }

                int payoutId =
                    SwarmDb.GetDatabaseForReading()
                        .GetPayoutIdFromDependency (this, FinancialDependencyType.CashAdvancePayback);

                if (payoutId == 0)
                {
                    return null; // or throw? When expense claim system was being phased in, payouts did not exist
                }

                return Payout.FromIdentity (payoutId);
            }
        }

        public FinancialAccount Budget
        {
            get { return FinancialAccount.FromIdentity (BudgetId); }
            set
            {
                if (BudgetId != value.Identity)
                {
                    if (this.Open) // requirement
                    {
                        SwarmDb.GetDatabaseForWriting().SetCashAdvanceBudget (this.Identity, value.Identity);
                        base.BudgetId = value.Identity;
                    }
                }

            }
        }

        public Organization Organization
        {
            get { return Organization.FromIdentity(OrganizationId); }
        }

        public FinancialValidations Validations
        {
            get { return FinancialValidations.ForObject(this); }
        }



        public void SetBudget(FinancialAccount newBudget, Person settingPerson)
        {
            this.Budget = newBudget; // ignore settingPerson
        }

        public void SetAmountCents(Int64 newAmount, Person settingPerson)
        {
            SwarmDb.GetDatabaseForWriting().SetCashAdvanceAmountCents (this.Identity, newAmount);
            base.AmountCents = newAmount;
        }
    }
}