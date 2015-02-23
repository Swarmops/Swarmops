using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class PayrollAdjustments : PluralBase<PayrollAdjustments, PayrollAdjustment, BasicPayrollAdjustment>
    {
        public static PayrollAdjustments ForPayrollItem (PayrollItem payrollItem)
        {
            return
                FromArray (SwarmDb.GetDatabaseForReading()
                    .GetPayrollAdjustments (payrollItem, DatabaseCondition.OpenTrue));
        }

        public static PayrollAdjustments ForSalary (Salary salary)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetPayrollAdjustments (salary));
        }
    }
}