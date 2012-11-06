using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
{
    public class PayrollAdjustment: BasicPayrollAdjustment
    {
        private PayrollAdjustment (BasicPayrollAdjustment basic): base (basic)
        {
            // private constructor
        }

        public static PayrollAdjustment FromBasic (BasicPayrollAdjustment basic)
        {
            return new PayrollAdjustment(basic); // invoke private ctor
        }

        public static PayrollAdjustment FromIdentity (int payrollAdjustmentId)
        {
            return FromBasic (PirateDb.GetDatabaseForReading().GetPayrollAdjustment(payrollAdjustmentId));
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


        public void Close (Salary salary)
        {
            if (!Open)
            {
                throw new InvalidOperationException("Payroll adjustment #" + this.Identity.ToString() + " cannot be closed; is already closed");
            }

            PirateDb.GetDatabaseForWriting().ClosePayrollAdjustment(this.Identity, salary.Identity);
            base.Open = false;
            base.SalaryId = salary.Identity;
        }


        public static PayrollAdjustment Create (PayrollItem item, PayrollAdjustmentType type, double amount, string description)
        {
            int id = PirateDb.GetDatabaseForWriting().CreatePayrollAdjustment(item.Identity, type, amount, description);
            return PayrollAdjustment.FromIdentity(id);
        }

        public decimal AmountDecimal
        {
            get { return base.AmountCents/100.0m; }
        }
    }
}
