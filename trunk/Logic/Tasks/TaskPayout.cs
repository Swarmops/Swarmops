using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Financial;

namespace Activizr.Logic.Tasks
{
    public class TaskPayout: TaskBase
    {
        public TaskPayout (Payout payout)
            : base (payout.Identity, "Payout #" + payout.Identity, DateTime.Now, payout.ExpectedTransactionDate)
        {
            
        }
    }
}
