using System;
using Swarmops.Logic.Financial;

namespace Swarmops.Logic.DashboardTasks
{
    public class TaskPayout : TaskBase
    {
        public TaskPayout(Payout payout)
            : base(payout.Identity, "Payout #" + payout.Identity, DateTime.Now, payout.ExpectedTransactionDate)
        {
        }
    }
}