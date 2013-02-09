using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class PayrollAdjustments: PluralBase<PayrollAdjustments,PayrollAdjustment,BasicPayrollAdjustment>
    {
        static public PayrollAdjustments ForPayrollItem (PayrollItem payrollItem)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetPayrollAdjustments(payrollItem, DatabaseCondition.OpenTrue));
        }

        static public PayrollAdjustments ForSalary (Salary salary)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetPayrollAdjustments(salary));
        }
    }
}
