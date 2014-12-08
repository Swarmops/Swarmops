using Swarmops.Basic.Types.Financial;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class FinancialValidation : BasicFinancialValidation
    {
        private FinancialValidation (BasicFinancialValidation basic)
            : base (basic)
        {
        }

        public Person Person
        {
            get { return Person.FromIdentity (PersonId); }
        }

        public static FinancialValidation FromBasic (BasicFinancialValidation basic)
        {
            return new FinancialValidation (basic);
        }
    }
}