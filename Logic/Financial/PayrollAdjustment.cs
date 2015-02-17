using System;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
{
    public class PayrollAdjustment : BasicPayrollAdjustment
    {
        private PayrollAdjustment (BasicPayrollAdjustment basic) : base (basic)
        {
            // private constructor
        }


        public new bool Open
        {
            get { return base.Open; }
            // Hide setter
        }


        public new int SalaryId
        {
            get { return base.SalaryId; }
            // Hide setter
        }

        public decimal AmountDecimal
        {
            get { return base.AmountCents/100.0m; }
        }

        public static PayrollAdjustment FromBasic (BasicPayrollAdjustment basic)
        {
            return new PayrollAdjustment (basic); // invoke private ctor
        }

        public static PayrollAdjustment FromIdentity (int payrollAdjustmentId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetPayrollAdjustment (payrollAdjustmentId));
        }


        public void Close (Salary salary)
        {
            if (!Open)
            {
                throw new InvalidOperationException ("Payroll adjustment #" + Identity +
                                                     " cannot be closed; is already closed");
            }

            SwarmDb.GetDatabaseForWriting().ClosePayrollAdjustment (Identity, salary.Identity);
            base.Open = false;
            base.SalaryId = salary.Identity;
        }


        public static PayrollAdjustment Create (PayrollItem item, PayrollAdjustmentType type, double amount,
            string description)
        {
            int id = SwarmDb.GetDatabaseForWriting().CreatePayrollAdjustment (item.Identity, type, amount, description);
            return FromIdentity (id);
        }
    }
}