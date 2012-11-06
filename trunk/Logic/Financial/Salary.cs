using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Activizr.Logic.Pirates;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Database;
using Activizr.Logic.Structure;

namespace Activizr.Logic.Financial
{
    public class Salary: BasicSalary, IAttestable
    {
        internal Salary (BasicSalary basic): base (basic)
        {
            // private ctor
        }

        static public Salary FromBasic (BasicSalary basic)
        {
            return new Salary(basic);
        }

        static public Salary FromIdentity (int salaryId)
        {
            return FromBasic(PirateDb.GetDatabaseForReading().GetSalary(salaryId));
        }

        static public Salary Create (int payrollItemId, DateTime payoutDate)
        {
            return Create(PayrollItem.FromIdentity(payrollItemId), payoutDate);
        }

        static public Salary Create (PayrollItem payrollItem, DateTime payoutDate)
        {
            // Load the existing adjustments.

            PayrollAdjustments adjustments = PayrollAdjustments.ForPayrollItem(payrollItem);

            Int64 payCents = payrollItem.BaseSalaryCents;

            // Apply all before-tax adjustments

            foreach (PayrollAdjustment adjustment in adjustments)
            {
                if (adjustment.Type == PayrollAdjustmentType.GrossAdjustment)
                {
                    payCents += adjustment.AmountCents;
                }
            }

            // calculate tax

            double subtractiveTax = TaxLevels.GetTax (payrollItem.Country, payrollItem.SubtractiveTaxLevelId, payCents / 100.0);
            
            if (subtractiveTax < 1.0)
            {
                // this is a percentage and not an absolute number

                subtractiveTax = payCents * subtractiveTax;
            }
            Int64 subtractiveTaxCents = (Int64) (subtractiveTax*100);

            Int64 additiveTaxCents = (Int64) (payCents * payrollItem.AdditiveTaxLevel);

            payCents -= subtractiveTaxCents;

            // Apply all after-tax adjustments

            foreach (PayrollAdjustment adjustment in adjustments)
            {
                if (adjustment.Type == PayrollAdjustmentType.NetAdjustment)
                {
                    payCents += adjustment.AmountCents;
                }
            }

            // Create salary, close adjustments

            Salary salary = Salary.Create(payrollItem, payoutDate, payCents, subtractiveTaxCents, additiveTaxCents);

            // For each adjustment, close and bind to salary

            foreach (PayrollAdjustment adjustment in adjustments)
            {
                adjustment.Close(salary);
            }

            // If net is negative, create rollover adjustment

            if (payCents < 0)
            {
                PayrollAdjustment rollover1 = PayrollAdjustment.Create(payrollItem, PayrollAdjustmentType.NetAdjustment, -payCents,
                                         "Deficit rolls over to next salary");

                rollover1.Close(salary);

                PayrollAdjustment rollover2 = PayrollAdjustment.Create(payrollItem, PayrollAdjustmentType.NetAdjustment,
                                                                       -payCents, "Deficit rolled over from " +
                                                                       payoutDate.ToString("yyyy-MM-dd"));

                // keep rollover2 open, so the deficit from this salary is carried to the next

                salary.NetSalaryCents = 0;
            }

            // Add the financial transaction

            FinancialTransaction transaction =
                FinancialTransaction.Create(payrollItem.OrganizationId, DateTime.Now,
                                "Salary #" + salary.Identity + ": " + payrollItem.PersonCanonical +
                                " " +
                                salary.PayoutDate.ToString("yyyy-MMM", CultureInfo.InvariantCulture));
            transaction.AddRow(payrollItem.Budget, salary.CostTotalCents, null);
            transaction.AddRow(payrollItem.Organization.FinancialAccounts.DebtsSalary, -salary.NetSalaryCents, null);
            transaction.AddRow(payrollItem.Organization.FinancialAccounts.DebtsTax, -salary.TaxTotalCents, null);
            transaction.Dependency = salary;

            // Finally, check if net and/or tax are zero, and if so, mark them as already-paid (i.e. not due for payment)

            if (salary.NetSalaryCents == 0)
            {
                salary.NetPaid = true;
            }

            if (salary.TaxTotalCents == 0)
            {
                salary.TaxPaid = true;
            }

            return salary;
        }

        static private Salary Create (PayrollItem payrollItem, DateTime payoutDate, Int64 netSalaryCents, Int64 subtractiveTaxCents, Int64 additiveTaxCents)
        {
            return
                FromIdentity(PirateDb.GetDatabaseForWriting().CreateSalary(payrollItem.Identity, payoutDate, payrollItem.BaseSalaryCents, netSalaryCents,
                                                                 subtractiveTaxCents, additiveTaxCents));
        }


        public PayrollItem PayrollItem
        {
            get
            {
                return PayrollItem.FromIdentity(this.PayrollItemId);
            }
        }


        public PayrollAdjustments Adjustments
        {
            get { return PayrollAdjustments.ForSalary(this); }
        }


        public decimal BaseSalaryDecimal
        {
            get { return base.BaseSalaryCents/100.0m; }
        }


        public decimal SubtractiveTaxDecimal
        {
            get { return base.SubtractiveTaxCents/100.0m; }
        }

        public decimal AdditiveTaxDecimal
        {
            get { return base.AdditiveTaxCents/100.0m; }
        }


        public decimal NetSalaryDecimal
        {
            get { return base.NetSalaryCents/100.0m; }
        }


        public new Int64 NetSalaryCents
        {
            get { return base.NetSalaryCents; }  // TODO: CONVERT TO MAKE CENTS-BASED CALCULATIONS THE MASTER
            set
            {
                if (value != base.NetSalaryCents)
                {
                    PirateDb.GetDatabaseForWriting().SetSalaryNetSalary(this.Identity, value);
                    base.NetSalaryCents = value;
                }
            }
        }



        public Int64 GrossSalaryCents
        {
            get
            {

                Int64 payCents = this.BaseSalaryCents;

                // Apply all before-tax adjustments

                foreach (PayrollAdjustment adjustment in this.Adjustments)
                {
                    if (adjustment.Type == PayrollAdjustmentType.GrossAdjustment)
                    {
                        payCents += adjustment.AmountCents;
                    }
                }

                return payCents;
            }
        }

        public Int64 CostTotalCents
        {
            get { return NetSalaryCents + TaxTotalCents; }
        }

        public decimal CostTotalDecimal
        {
            get { return CostTotalCents / 100.0m; }
        }

        public decimal TaxTotalDecimal
        {
            get { return TaxTotalCents / 100.0m; }
        }

        public Int64 TaxTotalCents
        {
            get { return AdditiveTaxCents + SubtractiveTaxCents;  }
        }

        public new bool NetPaid
        {
            get { return base.NetPaid; }
            set
            {
                PirateDb.GetDatabaseForWriting().SetSalaryNetPaid(this.Identity, value);
                base.NetPaid = value;
                base.Open = !(base.NetPaid && base.TaxPaid);
            }
        }

        public new bool TaxPaid
        {
            get { return base.TaxPaid; }
            set
            {
                PirateDb.GetDatabaseForWriting().SetSalaryTaxPaid(this.Identity, value);
                base.NetPaid = value;
                base.Open = !(base.NetPaid && base.TaxPaid);
            }
        }


        public FinancialValidations Validations
        {
            get { return FinancialValidations.ForObject(this); }
        }

        //  dummy comment to force a build -- remove on sight

        public Person AttestationExpectedBy
        {
            get
            {
                // Usually a salary is attested by the budget owner -- except when that person is the target of the payout

                Person budgetOwner = this.PayrollItem.Budget.Owner;

                if (this.PayrollItem.Person.Identity == budgetOwner.Identity)
                {
                    return this.PayrollItem.ReportsToPerson;
                }

                return budgetOwner;
            }
        }



        #region IAttestable Members

        public void Attest(Person attester)
        {
            PirateDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Attestation,
                FinancialDependencyType.Salary, this.Identity, DateTime.Now, attester.Identity, this.NetSalaryCents / 100.0);
            PirateDb.GetDatabaseForWriting().SetSalaryAttested(this.Identity, true);
        }

        public void Deattest(Person deattester)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
