using System;
using System.Globalization;

namespace Swarmops.Logic.DashboardTasks
{
    public class TaskAdvanceDebt : TaskBase
    {
        public TaskAdvanceDebt (decimal debt)
            : base (
                0, debt.ToString ("N2", CultureInfo.InvariantCulture), DateTime.Now,
                DateTime.Now.AddDays (28))
        {
            // empty ctor
        }
    }
}