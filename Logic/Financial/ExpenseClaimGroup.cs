using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Financial
{
    public class ExpenseClaimGroup: BasicExpenseClaimGroup
    {
        private ExpenseClaimGroup(BasicExpenseClaimGroup basic)
            : base (basic)
        {
            // private ctor
        }

        public static ExpenseClaimGroup FromBasic(BasicExpenseClaimGroup basic)
        {
            return new ExpenseClaimGroup(basic);
        }

        public static ExpenseClaimGroup FromIdentity(int expenseClaimGroupId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetExpenseClaimGroup(expenseClaimGroupId));
        }

        internal static ExpenseClaimGroup FromIdentityAggressive(int expenseClaimGroupId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetExpenseClaimGroup(expenseClaimGroupId)); // "writing" intentional
        }

        public static ExpenseClaimGroup Create(Organization organization, Person creatingPerson, ExpenseClaimGroupType groupType,
            string rawGroupData)
        {
            return FromIdentityAggressive(SwarmDb.GetDatabaseForWriting().CreateExpenseClaimGroup(organization.Identity, creatingPerson.Identity, groupType, rawGroupData));
        }

        public new bool Open
        {
            get { return base.Open; }
            set
            {
                if (value != base.Open)
                {
                    // change of boolean value

                    if (!base.Open)
                    {
                        // only one direction permitted
                    }

                    SwarmDb.GetDatabaseForWriting().SetExpenseClaimGroupClosed(this.Identity);
                    base.Open = false;
                }
            }
        }
    }
}
