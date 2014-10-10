using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicSalary: IHasIdentity
    {
        public BasicSalary (int salaryId, int payrollItemId, DateTime payoutDate, Int64 baseSalaryCents, Int64 netSalaryCents,
            Int64 subtractiveTaxCents, Int64 additiveTaxCents, bool attested, bool netPaid, bool taxPaid, bool open)
        {
            this.SalaryId = salaryId;
            this.PayrollItemId = payrollItemId;
            this.PayoutDate = payoutDate;
            this.BaseSalaryCents = baseSalaryCents;
            this.NetSalaryCents = netSalaryCents;
            this.SubtractiveTaxCents = subtractiveTaxCents;
            this.AdditiveTaxCents = additiveTaxCents;
            this.Attested = attested;
            this.NetPaid = netPaid;
            this.TaxPaid = taxPaid;
            this.Open = open;
        }

        public BasicSalary (BasicSalary original)
            :this (original.SalaryId, original.PayrollItemId, original.PayoutDate, original.BaseSalaryCents, original.NetSalaryCents,
            original.SubtractiveTaxCents, original.AdditiveTaxCents, original.Attested, original.NetPaid, original.TaxPaid,
            original.Open)
        {
            // empty copy constructor
        }

        public int SalaryId { get; private set; }
        public int PayrollItemId { get; private set; }
        public DateTime PayoutDate { get; protected set; }
        public Int64 BaseSalaryCents { get; protected set; }
        public Int64 NetSalaryCents { get; protected set; }
        public Int64 SubtractiveTaxCents { get; protected set; }
        public Int64 AdditiveTaxCents { get; protected set; }
        public bool Attested { get; protected set; }
        public bool TaxPaid { get; protected set; }
        public bool NetPaid { get; protected set; }
        public bool Open { get; protected set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.SalaryId; }
        }

        #endregion
    }
}
