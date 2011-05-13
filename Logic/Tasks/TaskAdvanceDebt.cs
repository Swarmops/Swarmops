using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Activizr.Logic.Tasks
{
    public class TaskAdvanceDebt: TaskBase
    {
        public TaskAdvanceDebt(decimal debt)
                    : base(
                0, debt.ToString("N2", CultureInfo.InvariantCulture), DateTime.Now,
                DateTime.Now.AddDays(28))
        {
            // empty ctor
        }
    }
}
