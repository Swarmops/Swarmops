using System;
using System.Collections.Generic;
using System.Globalization;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class Salary : BasicSalary, IAttestable
    {
        internal Salary (BasicSalary basic) : base (basic)
        {
            // private ctor
        }


        public PayrollItem PayrollItem
        {
            get { return PayrollItem.FromIdentity (PayrollItemId); }
        }


        public PayrollAdjustments Adjustments
        {
            get { return PayrollAdjustments.ForSalary (this); }
        }


        public new Int64 NetSalaryCents
        {
            get { return base.NetSalaryCents; }
        }


        public Int64 GrossSalaryCents
        {
            get
            {
                Int64 payCents = BaseSalaryCents;

                // Apply all before-tax adjustments

                foreach (PayrollAdjustment adjustment in Adjustments)
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
            get { return CostTotalCents/100.0m; }
        }

        public decimal TaxTotalDecimal
        {
            get { return TaxTotalCents/100.0m; }
        }

        public Int64 TaxTotalCents
        {
            get { return AdditiveTaxCents + SubtractiveTaxCents; }
        }

        public new bool NetPaid
        {
            get { return base.NetPaid; }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetSalaryNetPaid (Identity, value);
                base.NetPaid = value;
                base.Open = !(base.NetPaid && base.TaxPaid);
            }
        }

        public new bool TaxPaid
        {
            get { return base.TaxPaid; }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetSalaryTaxPaid (Identity, value);
                base.TaxPaid = value;
                base.Open = !(base.NetPaid && base.TaxPaid);
            }
        }


        public FinancialValidations Validations
        {
            get { return FinancialValidations.ForObject (this); }
        }

        //  dummy comment to force a build -- remove on sight

        public Person AttestationExpectedBy
        {
            get
            {
                // Usually a salary is attested by the budget owner -- except when that person is the target of the payout

                Person budgetOwner = PayrollItem.Budget.Owner;

                if (PayrollItem.Person.Identity == budgetOwner.Identity)
                {
                    return PayrollItem.ReportsToPerson;
                }

                return budgetOwner;
            }
        }


        public new bool Open
        {
            get { return base.Open; }
            set
            {
                if (base.Open != value)
                {
                    SwarmDb.GetDatabaseForWriting().SetSalaryOpen(Identity, value);
                    base.Open = value;
                }
            }
        }


        public new bool Attested 
        {
            get { return base.Attested; }
            set
            {
                if (base.Attested != value)
                {
                    SwarmDb.GetDatabaseForWriting().SetSalaryAttested(Identity, value);
                    base.Attested = value;
                }
            }
        }


        #region IAttestable Members

        public void Attest (Person attester)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.Attestation,
                FinancialDependencyType.Salary, Identity, DateTime.Now, attester.Identity, CostTotalCents/100.0);
            this.Attested = true;
        }

        public void Deattest (Person deattester)
        {
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.Deattestation,
                FinancialDependencyType.Salary, Identity, DateTime.Now, deattester.Identity, CostTotalCents/100.0);
            this.Attested = false;
        }

        public void DenyAttestation (Person denyingPerson, string reason)
        {
            this.Attested = false;
            this.Open = false;

            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation(FinancialValidationType.Kill,
                FinancialDependencyType.ExpenseClaim, Identity,
                DateTime.UtcNow, denyingPerson.Identity, this.CostTotalCents);

            OutboundComm.CreateNotificationOfFinancialValidation(Budget, this.PayrollItem.Person, NetSalaryCents / 100.0, this.PayoutDate.ToString("MMMM yyyy"),
                NotificationResource.Salary_Denied, reason);

            FinancialTransaction transaction = FinancialTransaction.FromDependency(this);
            transaction.RecalculateTransaction(new Dictionary<int, long>(), denyingPerson); // zeroes out the tx
        }

        #endregion

        public FinancialAccount Budget // needed for IAttestable
        {
            get { return PayrollItem.Budget; }
        }

        public static Salary FromBasic (BasicSalary basic)
        {
            return new Salary (basic);
        }

        public static Salary FromIdentity (int salaryId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetSalary (salaryId));
        }

        public static Salary Create (int payrollItemId, DateTime payoutDate)
        {
            return Create (PayrollItem.FromIdentity (payrollItemId), payoutDate);
        }

        public static Salary Create (PayrollItem payrollItem, DateTime payoutDate)
        {
            // Load the existing adjustments.

            PayrollAdjustments adjustments = PayrollAdjustments.ForPayrollItem (payrollItem);

            Int64 payCents = payrollItem.BaseSalaryCents;

            // Apply all before-tax adjustments

            foreach (PayrollAdjustment adjustment in adjustments)
            {
                if (adjustment.Type == PayrollAdjustmentType.GrossAdjustment)
                {
                    payCents += adjustment.AmountCents;
                }
            }

            Int64 subtractiveTaxCents = 0;
            Int64 additiveTaxCents = 0;

            if (!payrollItem.IsContractor)
            {

                // calculate tax

                Money grossInOrgCurrency = new Money
                {
                    Cents = payCents,
                    Currency = payrollItem.Organization.Currency,
                    ValuationDateTime = DateTime.UtcNow
                };

                Money grossInTaxCurrency = grossInOrgCurrency.ToCurrency (payrollItem.Country.Currency);

                Money subtractiveTax = TaxLevels.GetTax (payrollItem.Country, payrollItem.SubtractiveTaxLevelId,
                    grossInTaxCurrency);

                Money subtractiveTaxInOrgCurrency = subtractiveTax.ToCurrency (payrollItem.Organization.Currency);

                subtractiveTaxCents = (Int64) (subtractiveTaxInOrgCurrency.Cents);

                additiveTaxCents = (Int64) (payCents*payrollItem.AdditiveTaxLevel);

                payCents -= subtractiveTaxCents;
            }

            // Apply all after-tax adjustments

            foreach (PayrollAdjustment adjustment in adjustments)
            {
                if (adjustment.Type == PayrollAdjustmentType.NetAdjustment)
                {
                    payCents += adjustment.AmountCents;
                }
            }

            // If net is negative, create rollover adjustment

            PayrollAdjustment rolloverAdjustment = null;

            if (payCents < 0)
            {
                rolloverAdjustment = PayrollAdjustment.Create(payrollItem, PayrollAdjustmentType.NetAdjustment,
                    -payCents,
                    "Deficit rolls over to next salary");

                PayrollAdjustment.Create(payrollItem, PayrollAdjustmentType.NetAdjustment,
                    payCents, "Deficit rolled over from " +
                              payoutDate.ToString("yyyy-MM-dd"));

                // keep second rollover open, so the deficit from this salary is carried to the next

                payCents = 0;
            }

            // Create salary, close adjustments

            Salary salary = Create (payrollItem, payoutDate, payCents, subtractiveTaxCents, additiveTaxCents);

            // For each adjustment, close and bind to salary

            foreach (PayrollAdjustment adjustment in adjustments)
            {
                adjustment.Close (salary);
            }

            if (rolloverAdjustment != null)
            {
                rolloverAdjustment.Close (salary);
            }

            // Add the financial transaction

            FinancialTransaction transaction =
                FinancialTransaction.Create (payrollItem.OrganizationId, DateTime.UtcNow,
                    "Salary #" + salary.Identity + ": " + payrollItem.PersonCanonical +
                    " " +
                    salary.PayoutDate.ToString ("yyyy-MMM", CultureInfo.InvariantCulture));
            transaction.AddRow (payrollItem.Budget, salary.CostTotalCents, null);
            transaction.AddRow (payrollItem.Organization.FinancialAccounts.DebtsSalary, -salary.NetSalaryCents, null);
            if (salary.TaxTotalCents != 0)
            {
                transaction.AddRow (payrollItem.Organization.FinancialAccounts.DebtsTax, -salary.TaxTotalCents, null);
            }
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

            // Clear a cache
            FinancialAccount.ClearAttestationAdjustmentsCache(payrollItem.Organization);



            return salary;
        }

        private static Salary Create (PayrollItem payrollItem, DateTime payoutDate, Int64 netSalaryCents,
            Int64 subtractiveTaxCents, Int64 additiveTaxCents)
        {
            return
                FromIdentity (SwarmDb.GetDatabaseForWriting()
                    .CreateSalary (payrollItem.Identity, payoutDate, payrollItem.BaseSalaryCents, netSalaryCents,
                        subtractiveTaxCents, additiveTaxCents));
        }
    }
}