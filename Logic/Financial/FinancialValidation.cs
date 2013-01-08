using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Logic.Pirates;

namespace Swarmops.Logic.Financial
{
    public class FinancialValidation: BasicFinancialValidation
    {
        private FinancialValidation (BasicFinancialValidation basic)
            : base (basic)
        {
        }

        public static FinancialValidation FromBasic (BasicFinancialValidation basic)
        {
            return new FinancialValidation(basic);
        }

        public Person Person
        {
            get { return Person.FromIdentity(this.PersonId); }
        }
    }
}
