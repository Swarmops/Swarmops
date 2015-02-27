using System;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Transmission;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.LogEntries;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    [Serializable]
    public class CashAdvance : BasicCashAdvance, IAttestable
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

            OutboundComm.CreateNotificationAttestationNeeded (budget, forPerson, string.Empty, amountCents/100.0,
                description, NotificationResource.CashAdvance_Requested);
            // Slightly misplaced logic, but failsafer here
            SwarmopsLogEntry.Create (forPerson,
                new CashAdvanceRequestedLogEntry (createdByPerson, forPerson, amountCents/100.0, budget, description),
                newAdvance);

            return newAdvance;
        }

        #endregion

        #region Implementation of IAttestable

        public void Attest (Person attester)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.Attestation,
                FinancialDependencyType.CashAdvance, Identity, DateTime.Now, attester.Identity, AmountCents/100.0);
            SwarmDb.GetDatabaseForWriting().SetCashAdvanceAttested (Identity, true, attester.Identity);

            OutboundComm.CreateNotificationOfFinancialValidation (Budget, Person, AmountCents/100.0, Description,
                NotificationResource.CashAdvance_Attested);
        }

        public void Deattest (Person deattester)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.Deattestation,
                FinancialDependencyType.CashAdvance, Identity, DateTime.Now, deattester.Identity, AmountCents/100.0);
            SwarmDb.GetDatabaseForWriting().SetCashAdvanceAttested (Identity, false, Person.NobodyId);

            OutboundComm.CreateNotificationOfFinancialValidation (Budget, Person, AmountCents/100.0, Description,
                NotificationResource.CashAdvance_Deattested);
        }

        #endregion

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
        }

        public Organization Organization
        {
            get { return Organization.FromIdentity(OrganizationId); }
        }

        public FinancialValidations Validations
        {
            get { return FinancialValidations.ForObject(this); }
        }

    }
}