using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
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
            return
                FromIdentityAggressive(SwarmDb.GetDatabaseForWriting().CreateCashAdvance(forPerson.Identity,
                                                                                          createdByPerson.Identity,
                                                                                          organization.Identity,
                                                                                          budget.Identity, amountCents,
                                                                                          description));
        }

        #endregion



        #region Implementation of IAttestable

        public void Attest(Person attester)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Attestation,
                FinancialDependencyType.CashAdvance, this.Identity, DateTime.Now, attester.Identity, this.AmountCents / 100.0);
            SwarmDb.GetDatabaseForWriting().SetCashAdvanceAttested(this.Identity, true, attester.Identity);
        }

        public void Deattest(Person deattester)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Deattestation,
                FinancialDependencyType.CashAdvance, this.Identity, DateTime.Now, deattester.Identity, this.AmountCents / 100.0);
            SwarmDb.GetDatabaseForWriting().SetCashAdvanceAttested(this.Identity, false, Person.NobodyId);
        }

        #endregion


        public Person Person 
        {
            get { return Person.FromIdentity(this.PersonId); }
        }

        public FinancialAccount FinancialAccount
        {
            get { return FinancialAccount.FromIdentity(this.FinancialAccountId); }
        }
    }
}
