using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Financial;

namespace Swarmops.Logic.Tasks
{
    public class TaskSalary: TaskBase
    {
        public TaskSalary (Salary salary): base (salary.Identity, salary.PayoutDate.ToString("yyyy-MMM") + " salary for " + salary.PayrollItem.PersonCanonical, salary.PayoutDate.AddDays(-(salary.PayoutDate.Day-1)), salary.PayoutDate.AddDays(-7))
        {
            // empty ctor
        }
    }
}
