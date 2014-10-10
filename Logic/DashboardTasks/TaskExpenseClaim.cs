using Swarmops.Logic.Financial;

namespace Swarmops.Logic.DashboardTasks
{
    public class TaskExpenseClaim: TaskBase
    {
        public TaskExpenseClaim(ExpenseClaim claim)
            : base(
                claim.Identity, "Expense Claim #" + claim.Identity.ToString(), claim.CreatedDateTime,
                claim.CreatedDateTime.AddDays(14))
        {
            // empty ctor
        }
    }
}
