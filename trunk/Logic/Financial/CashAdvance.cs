using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return FromBasic(PirateDb.GetDatabaseForReading().GetCashAdvance(cashAdvanceId));
        }

        public static CashAdvance FromIdentityAggressive (int cashAdvanceId)
        {
            return FromBasic(PirateDb.GetDatabaseForWriting().GetCashAdvance(cashAdvanceId));
        }

        public static CashAdvance Create(Organization organization, Person forPerson, Person createdByPerson, Int64 amountCents, FinancialAccount budget, string description)
        {
            return
                FromIdentityAggressive(PirateDb.GetDatabaseForWriting().CreateCashAdvance(forPerson.Identity,
                                                                                          createdByPerson.Identity,
                                                                                          organization.Identity,
                                                                                          budget.Identity, amountCents,
                                                                                          description));
        }

        #endregion



        #region Implementation of IAttestable

        public void Attest(Person attester)
        {
            throw new NotImplementedException();
        }

        public void Deattest(Person deattester)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
