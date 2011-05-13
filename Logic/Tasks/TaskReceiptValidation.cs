using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Financial;

namespace Activizr.Logic.Tasks
{
    public class TaskReceiptValidation: TaskBase
    {
        public TaskReceiptValidation (ExpenseClaim claim): base (claim.Identity, "Expense Claim #" + claim.Identity, claim.CreatedDateTime, claim.CreatedDateTime.AddDays(14))
        {
            // empty ctor
        }
    }
}
