using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Transmission;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.LogEntries;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Financial
{
    [Serializable]
    public class CashAdvance: BasicCashAdvance, IAttestable
    {
        #region Construction and creation

        private CashAdvance(BasicCashAdvance basic): base(basic)
        {
            
        }

        public static CashAdvance FromBasic (BasicCashAdvance basic)
        {
            return new CashAdvance(basic);
        }

        public static CashAdvance FromIdentity (int cashAdvanceId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetCashAdvance(cashAdvanceId));
        }

        public static CashAdvance FromIdentityAggressive (int cashAdvanceId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetCashAdvance(cashAdvanceId));
        }

        public static CashAdvance Create(Organization organization, Person forPerson, Person createdByPerson, Int64 amountCents, FinancialAccount budget, string description)
        {
            CashAdvance newAdvance = FromIdentityAggressive(SwarmDb.GetDatabaseForWriting().CreateCashAdvance(forPerson.Identity,
                                                                                          createdByPerson.Identity,
                                                                                          organization.Identity,
                                                                                          budget.Identity, amountCents,
                                                                                          description));

            OutboundComm.CreateNotificationAttestationNeeded(budget, forPerson, string.Empty, (double) amountCents/100.0, description, NotificationResource.CashAdvance_Requested); // Slightly misplaced logic, but failsafer here
            SwarmopsLogEntry.Create(forPerson,
                                    new CashAdvanceRequestedLogEntry(createdByPerson, forPerson, (double) amountCents/100.0, budget, description),
                                    newAdvance);

            return newAdvance;
        }

        #endregion



        #region Implementation of IAttestable

        public void Attest(Person attester)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Attestation,
                FinancialDependencyType.CashAdvance, this.Identity, DateTime.Now, attester.Identity, this.AmountCents / 100.0);
            SwarmDb.GetDatabaseForWriting().SetCashAdvanceAttested(this.Identity, true, attester.Identity);

            OutboundComm.CreateNotificationOfFinancialValidation(this.Budget, this.Person, (double)this.AmountCents / 100.0, this.Description, NotificationResource.CashAdvance_Attested);
        }

        public void Deattest(Person deattester)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Deattestation,
                FinancialDependencyType.CashAdvance, this.Identity, DateTime.Now, deattester.Identity, this.AmountCents / 100.0);
            SwarmDb.GetDatabaseForWriting().SetCashAdvanceAttested(this.Identity, false, Person.NobodyId);

            OutboundComm.CreateNotificationOfFinancialValidation(this.Budget, this.Person, (double)this.AmountCents / 100.0, this.Description, NotificationResource.CashAdvance_Deattested);
        }

        #endregion


        public Person Person 
        {
            get { return Person.FromIdentity(this.PersonId); }
        }

        public FinancialAccount Budget
        {
            get { return FinancialAccount.FromIdentity(this.BudgetId); }
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

                SwarmDb.GetDatabaseForWriting().SetCashAdvanceOpen(this.Identity, value);
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

                SwarmDb.GetDatabaseForWriting().SetCashAdvancePaidOut(this.Identity, value);
                base.PaidOut = value;
            }
        }


        public Payout PayoutOut
        {
            get
            {
                if (!this.PaidOut)
                {
                    return null; // or throw?
                }

                int payoutId =
                    SwarmDb.GetDatabaseForReading().GetPayoutIdFromDependency(this, FinancialDependencyType.CashAdvance);

                if (payoutId == 0)
                {
                    return null; // or throw? When expense claim system was being phased in, payouts did not exist
                }

                return Payout.FromIdentity(payoutId);
            }
        }

        public Payout PayoutBack
        {
            get
            {
                if (!this.PaidOut)
                {
                    return null; // or throw?
                }

                int payoutId =
                    SwarmDb.GetDatabaseForReading().GetPayoutIdFromDependency(this, FinancialDependencyType.CashAdvancePayback);

                if (payoutId == 0)
                {
                    return null; // or throw? When expense claim system was being phased in, payouts did not exist
                }

                return Payout.FromIdentity(payoutId);
            }
        }
    }
}
