using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
{
    public class Payout: BasicPayout
    {
        #region Creation and Construction

        public static void Create ()
        {
            // is this even needed?

            throw new NotImplementedException();
        }

        public static Payout Create (Payout payout)
        {
            // This is quite unique in PW4 -- this is the only class which is pre-constructed and
            // can take an instance of its own kind as parameter in creation

            // TODO: Create in database

            return payout;
        }

        private Payout (BasicPayout basic):
            base (basic)
        {
            LoadDependencies();
        }

        private void LoadDependencies()
        {
            DependentExpenseClaims = new ExpenseClaims();
            DependentInvoices = new InboundInvoices();
            DependentSalariesNet = new Salaries();
            DependentSalariesTax = new Salaries();

            BasicFinancialDependency[] dependencies = PirateDb.GetDatabase().GetPayoutDependencies(this.Identity);

            foreach (BasicFinancialDependency dependency in dependencies)
            {
                switch (dependency.DependencyType)
                {
                    case FinancialDependencyType.ExpenseClaim:
                        DependentExpenseClaims.Add(ExpenseClaim.FromIdentity(dependency.ForeignId));
                        break;
                    case FinancialDependencyType.InboundInvoice:
                        DependentInvoices.Add(InboundInvoice.FromIdentity(dependency.ForeignId));
                        break;
                    case FinancialDependencyType.Salary:
                        Salary salary = Salary.FromIdentity(dependency.ForeignId);
                        if (salary.NetSalaryCents == this.AmountCents)  // HACK: Assumes that tax total is not identical
                        {
                            DependentSalariesNet.Add(salary);
                        }
                        else
                        {
                            DependentSalariesTax.Add(salary);
                        }
                        break;

                    default:
                        throw new NotImplementedException("Unknown financial dependency type in Payout.LoadDependencies(): " + dependency.ToString());
                }
            }
        }

        public static Payout FromBasic (BasicPayout basic)
        {
            return new Payout(basic);
        }

        public static Payout FromIdentity (int payoutId)
        {
            return FromBasic(PirateDb.GetDatabase().GetPayout(payoutId));
        }


        #endregion

        public InboundInvoices DependentInvoices;
        public ExpenseClaims DependentExpenseClaims;
        public Salaries DependentSalariesNet;
        public Salaries DependentSalariesTax;

        public new decimal Amount
        {
            set
            {
                if (this.Identity != 0)
                {
                    PirateDb.GetDatabase().SetPayoutAmount(this.Identity, (Int64)(value * 100));
                }
                base.AmountCents = (Int64)value * 100;
            }
            get
            {
                return base.AmountCents / 100.0m;
            }
        }

        public new Int64 AmountCents
        {
            set
            {
                if (this.Identity != 0)
                {
                    PirateDb.GetDatabase().SetPayoutAmount(this.Identity, value);
                }
                base.AmountCents = value;
            }
            get
            {
                return base.AmountCents;
            }
        }

        public new string Reference
        {
            set
            {
                if (this.Identity != 0)
                {
                    PirateDb.GetDatabase().SetPayoutReference(this.Identity, value);
                }
                base.Reference = value;
            }
            get
            {
                return base.Reference;
            }
        }


        public new bool Open
        {
            get
            {
                return base.Open;
            }
            set
            {
                PirateDb.GetDatabase().SetPayoutOpen(this.Identity, value);
                base.Open = value;
            }
        }



        public static Payout CreateFromProtoIdentity (Person creator, string protoIdentity)
        {
            string[] components = protoIdentity.Split('|');

            // The components can EITHER be a series of expense claims OR a single invoice.

            if (components.Length == 0)
            {
                // nothing to construct. Exception or return null?

                return null;
            }

            if (components [0][0] == 'C')
            {
                // Expense claims.

                string bank = string.Empty;
                string account = string.Empty;
                List<int> identityList = new List<int>();
                Int64 amountCents = 0;
                int organizationId = 0;

                foreach (string component in components)
                {
                    int claimId = Int32.Parse(component.Substring(1));
                    ExpenseClaim claim = ExpenseClaim.FromIdentity(claimId);
                    identityList.Add(claimId);

                    if (bank.Length < 1)
                    {
                        Person claimer = claim.Claimer;
                        bank = claimer.BankName;
                        account = claimer.BankAccount;
                        organizationId = claim.OrganizationId;
                    }

                    amountCents += claim.AmountCents;

                    claim.Repaid = true;
                    claim.Close();
                }

                string referenceString = string.Empty;

                if (identityList.Count == 1)
                {
                    referenceString = "Expense Claim #" + identityList[0].ToString();
                }
                else
                {
                    identityList.Sort();
                    referenceString = "Expense Claims " + Formatting.GenerateRangeString(identityList);
                }

                int payoutId = PirateDb.GetDatabase().CreatePayout(organizationId, bank, account,
                                                                   referenceString, amountCents, DateTime.Today.AddDays(1),
                                                                   creator.Identity);

                foreach (int claimId in identityList)
                {
                    PirateDb.GetDatabase().CreatePayoutDependency(payoutId, FinancialDependencyType.ExpenseClaim,
                                                                  claimId);
                }

                return FromIdentity(payoutId);
            }
            else if (components[0][0] == 'I')
            {
                // There is just one invoice per payout

                InboundInvoice invoice = InboundInvoice.FromIdentity(Int32.Parse(components[0].Substring(1)));

                DateTime expectedPayment = invoice.DueDate;

                if (expectedPayment < DateTime.Today)
                {
                    expectedPayment = DateTime.Today;
                }

                int payoutId = PirateDb.GetDatabase().CreatePayout(invoice.OrganizationId, string.Empty, invoice.PayToAccount,
                                                                   invoice.Ocr.Length > 0 ? "OCR " + invoice.Ocr : "Ref# " + invoice.InvoiceReference,
                                                                   invoice.AmountCents, expectedPayment,
                                                                   creator.Identity);

                PirateDb.GetDatabase().CreatePayoutDependency(payoutId, FinancialDependencyType.InboundInvoice,
                                                              invoice.Identity);

                invoice.Open = false;

                return FromIdentity(payoutId);
            }
            else if (components [0][0] == 'S')
            {
                // Salary, net payment

                Salary salary = Salary.FromIdentity(Int32.Parse(components[0].Substring(1)));

                int payoutId = PirateDb.GetDatabase().CreatePayout(salary.PayrollItem.OrganizationId, salary.PayrollItem.Person.BankName, salary.PayrollItem.Person.BankAccount,
                                                                   "Salary " + salary.PayoutDate.ToString("yyyy-MMM"),
                                                                   salary.NetSalaryCents, salary.PayoutDate,
                                                                   creator.Identity);

                PirateDb.GetDatabase().CreatePayoutDependency(payoutId, FinancialDependencyType.Salary,
                                                              salary.Identity);

                salary.NetPaid = true;

                return FromIdentity(payoutId);
            }
            else if (components [0][0] == 'T')
            {
                // Tax payment for multiple salaries.

                List<int> identityList = new List<int>();
                Int64 amountCents = 0;
                int organizationId = 0;
                DateTime payDay = DateTime.Today.AddDays(1);

                foreach (string component in components)
                {
                    int salaryId = Int32.Parse(component.Substring(1));
                    Salary salary = Salary.FromIdentity(salaryId);
                    identityList.Add(salaryId);

                    if (organizationId == 0)
                    {
                        organizationId = salary.PayrollItem.OrganizationId;
                        payDay = salary.PayoutDate;
                    }

                    amountCents += salary.TaxTotalCents;

                    salary.TaxPaid = true;
                }

                string referenceString = string.Empty;
                Organization organization = Organization.FromIdentity(organizationId);

                if (identityList.Count == 1)
                {
                    referenceString = "Tax for salary #" + identityList[0].ToString();
                }
                else
                {
                    identityList.Sort();
                    referenceString = "Tax for salaries " + Formatting.GenerateRangeString(identityList);
                }

                int payoutId = PirateDb.GetDatabase().CreatePayout(organization.Identity, "The Tax Man", organization.Parameters.TaxAccount, organization.Parameters.TaxOcr,
                                                                   amountCents, payDay, creator.Identity);

                foreach (int salaryId in identityList)
                {
                    PirateDb.GetDatabase().CreatePayoutDependency(payoutId, FinancialDependencyType.Salary,
                                                                  salaryId);
                }

                return FromIdentity(payoutId);
            }
            else
            {
                throw new NotImplementedException(); 
            }
        }



        public string ProtoIdentity
        {
            get
            {
                if (this.Identity != 0)
                {
                    throw new InvalidOperationException("Should never use ProtoIdentity after Identity has been established");
                }

                string result = string.Empty;
                foreach (InboundInvoice invoice in DependentInvoices)
                {
                    result += "|I" + invoice.Identity.ToString();
                }

                foreach (ExpenseClaim claim in DependentExpenseClaims)
                {
                    result += "|C" + claim.Identity.ToString();
                }

                foreach (Salary salary in DependentSalariesNet)
                {
                    result += "|S" + salary.Identity.ToString();
                }

                foreach (Salary salary in DependentSalariesTax)
                {
                    result += "|T" + salary.Identity.ToString();
                }

                if (result.Length > 0)
                {
                    return result.Substring(1);
                }

                return string.Empty;
            }
        }

        public string Recipient
        {
            get
            {
                if (this.DependentExpenseClaims == null || this.DependentInvoices == null)
                {
                    // TODO: read from db instead

                    throw new InvalidOperationException("Dependencies not initialized");
                }

                if (this.DependentInvoices.Count > 0)
                {
                    return this.DependentInvoices[0].Supplier;
                }
                else if (this.DependentExpenseClaims.Count > 0)
                {
                    return this.DependentExpenseClaims[0].ClaimerCanonical;
                }
                else if (this.DependentSalariesNet.Count > 0)
                {
                    return this.DependentSalariesNet[0].PayrollItem.PersonCanonical;
                }
                else if (this.DependentSalariesTax.Count > 0)
                {
                    return "The Tax Man";
                }
                else
                {
                    throw new InvalidOperationException("No dependencies for payout #" + this.Identity);
                }

            }
        }

        public Organization Organization
        {
            get { return Organization.FromIdentity(this.OrganizationId); }
        }


        public void BindToTransactionAndClose (FinancialTransaction transaction, Person bindingPerson)
        {
            Dictionary<int, Int64> accountDebitLookup = new Dictionary<int, Int64>();
            Organization organization = this.Organization;

            accountDebitLookup[organization.FinancialAccounts.DebtsExpenseClaims.Identity] = 0;
            accountDebitLookup[organization.FinancialAccounts.DebtsInboundInvoices.Identity] = 0;
            accountDebitLookup[organization.FinancialAccounts.DebtsSalary.Identity] = 0;
            accountDebitLookup[organization.FinancialAccounts.DebtsTax.Identity] = 0;

            if (this.DependentExpenseClaims.Count > 0)
            {
                accountDebitLookup[organization.FinancialAccounts.DebtsExpenseClaims.Identity] +=
                    this.DependentExpenseClaims.TotalAmountCents;
            }
            if (this.DependentInvoices.Count > 0)
            {
                accountDebitLookup[organization.FinancialAccounts.DebtsInboundInvoices.Identity] +=
                    this.DependentInvoices.TotalAmountCents;
            }
            if (this.DependentSalariesNet.Count > 0)
            {
                accountDebitLookup[organization.FinancialAccounts.DebtsSalary.Identity] +=
                    this.DependentSalariesNet.TotalAmountCentsNet;
            }
            if (this.DependentSalariesTax.Count > 0)
            {
                accountDebitLookup[organization.FinancialAccounts.DebtsTax.Identity] +=
                    this.DependentSalariesTax.TotalAmountCentsTax;
            }

            foreach (int financialAccountId in accountDebitLookup.Keys)
            {
                if (accountDebitLookup[financialAccountId] != 0)
                {
                    transaction.AddRow(FinancialAccount.FromIdentity(financialAccountId),
                                       accountDebitLookup[financialAccountId], bindingPerson);
                }
            }

            transaction.Dependency = this;
            this.Open = false;
        }


        public void ReloadDependencies()
        {
            LoadDependencies();
        }


        public void MigrateDependenciesTo (Payout migrationTarget)
        {
            PirateDb.GetDatabase().MovePayoutDependencies(this.Identity, migrationTarget.Identity);
            migrationTarget.RecalculateAmount();
            this.RecalculateAmount();
        }


        private void RecalculateAmount()
        {
            Int64 newAmountCents = 0;

            LoadDependencies();

            newAmountCents += this.DependentExpenseClaims.TotalAmountCents;
            newAmountCents += this.DependentInvoices.TotalAmountCents;
            newAmountCents += this.DependentSalariesNet.TotalAmountCentsNet;
            newAmountCents += this.DependentSalariesTax.TotalAmountCentsTax;

            this.AmountCents = newAmountCents;
        }



        public void UndoPayout()
        {
            foreach (ExpenseClaim claim in this.DependentExpenseClaims)
            {
                claim.Repaid = false;
                claim.Open = true;
            }

            foreach (InboundInvoice invoice in this.DependentInvoices)
            {
                invoice.Open = true;
            }

            foreach (Salary salary in this.DependentSalariesNet)
            {
                salary.NetPaid = false;
            }

            foreach (Salary salary in this.DependentSalariesTax)
            {
                salary.TaxPaid = false;
            }

            PirateDb.GetDatabase().ClearPayoutDependencies(this.Identity);
            RecalculateAmount();
            this.Open = false;
        }

    }
}
