using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Activizr.Basic.Types.Financial;
using Activizr.Database;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

namespace Activizr.Logic.Financial
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
