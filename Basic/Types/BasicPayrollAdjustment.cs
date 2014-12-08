using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicPayrollAdjustment : IHasIdentity
    {
        public BasicPayrollAdjustment (int payrollAdjustmentId, int payrollItemId, PayrollAdjustmentType type,
            Int64 amountCents,
            string description, bool open, int salaryId)
        {
            PayrollAdjustmentId = payrollAdjustmentId;
            PayrollItemId = payrollItemId;
            Type = type;
            AmountCents = amountCents;
            Description = description;
            Open = open;
            SalaryId = salaryId;
        }


        public BasicPayrollAdjustment (BasicPayrollAdjustment original)
            : this (
                original.PayrollAdjustmentId, original.PayrollItemId, original.Type, original.AmountCents,
                original.Description, original.Open, original.SalaryId)
        {
            // empty copy ctor
        }


        public int PayrollAdjustmentId { get; private set; }
        public int PayrollItemId { get; private set; }
        public PayrollAdjustmentType Type { get; private set; }
        public Int64 AmountCents { get; private set; }
        public string Description { get; private set; }
        public bool Open { get; protected set; }
        public int SalaryId { get; protected set; }

        public int Identity
        {
            get { return PayrollAdjustmentId; }
        }
    }


    public enum PayrollAdjustmentType
    {
        /// <summary>
        ///     Undefined value
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     Adjustment to salary BEFORE tax (sick leave, etc)
        /// </summary>
        GrossAdjustment = 1,

        /// <summary>
        ///     Adjustment to salary AFTER tax (expenses, etc)
        /// </summary>
        NetAdjustment = 2
    }
}