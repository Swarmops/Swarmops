using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Support.LogEntries;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
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
            throw new NotImplementedException(); // Seems not to be needed

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
            DependentCashAdvancesPayout = new CashAdvances();
            DependentCashAdvancesPayback = new CashAdvances();

            BasicFinancialDependency[] dependencies = SwarmDb.GetDatabaseForReading().GetPayoutDependencies(this.Identity);

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

                    case FinancialDependencyType.CashAdvance:
                        DependentCashAdvancesPayout.Add(CashAdvance.FromIdentity(dependency.ForeignId));
                        break;

                    case FinancialDependencyType.CashAdvancePayback:
                        DependentCashAdvancesPayback.Add(CashAdvance.FromIdentity(dependency.ForeignId));
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
            return FromBasic(SwarmDb.GetDatabaseForReading().GetPayout(payoutId));
        }


        #endregion

        public InboundInvoices DependentInvoices;
        public ExpenseClaims DependentExpenseClaims;
        public Salaries DependentSalariesNet;
        public Salaries DependentSalariesTax;
        public CashAdvances DependentCashAdvancesPayout;
        public CashAdvances DependentCashAdvancesPayback;

        public decimal Amount
        {
            set
            {
                if (this.Identity != 0)
                {
                    SwarmDb.GetDatabaseForWriting().SetPayoutAmount(this.Identity, (Int64)(value * 100));
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
                    SwarmDb.GetDatabaseForWriting().SetPayoutAmount(this.Identity, value);
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
                    SwarmDb.GetDatabaseForWriting().SetPayoutReference(this.Identity, value);
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
                SwarmDb.GetDatabaseForWriting().SetPayoutOpen(this.Identity, value);
                base.Open = value;
            }
        }



        public static Payout CreateFromProtoIdentity (Person creator, string protoIdentity)
        {
            string[] components = protoIdentity.Split('|');
            int payoutId = 0;

            // The components can EITHER be a series of expense claims OR a single invoice.

            if (components.Length == 0)
            {
                // nothing to construct. Exception or return null?

                return null;
            }
            if (components [0][0] == 'A')
            {
                // Cash advance(s) to be paid out.

                string bank = string.Empty;
                string account = string.Empty;
                List<int> identityList = new List<int>();
                Int64 amountCents = 0;
                int organizationId = 0;

                foreach (string component in components)
                {
                    int advanceId = Int32.Parse(component.Substring(1));
                    CashAdvance advance = CashAdvance.FromIdentity(advanceId);
                    identityList.Add(advanceId);
                    organizationId = advance.OrganizationId;
                    Organization organization = Organization.FromIdentity(advance.OrganizationId);

                    if (bank.Length < 1)
                    {
                        Person asker = advance.Person;
                        bank = asker.BankName;
                        account = asker.BankAccount;
                    }

                    amountCents += advance.AmountCents;

                    advance.PaidOut = true;
                    //advance.Open = false;   // isn't this closed only when settling the debt? Serious bug here?

                    SwarmopsLogEntry.Create(creator,
                                            new PayoutCreatedLogEntry(creator, advance.Person, organization,
                                                                      organization.Currency, (double) amountCents/100.0,
                                                                      "Cash Advance Paid Out"),
                                            advance.Person, advance);

                }

                string referenceString = string.Empty;

                if (identityList.Count == 1)
                {
                    referenceString = "Cash Advance #" + identityList[0].ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    identityList.Sort();
                    referenceString = "Cash Advances " + Formatting.GenerateRangeString(identityList);
                }

                payoutId = SwarmDb.GetDatabaseForWriting().CreatePayout(organizationId, bank, account,
                                                                   referenceString, amountCents, DateTime.Today.AddDays(1),
                                                                   creator.Identity);

                foreach (int advanceId in identityList)
                {
                    SwarmDb.GetDatabaseForWriting().CreatePayoutDependency(payoutId, FinancialDependencyType.CashAdvance,
                                                                  advanceId);
                }
            }
            else if (components [0][0] == 'C')
            {
                // Expense claims, possibly followed up by cash advance paybacks

                Person beneficiaryPerson = null;
                Organization organization = null;
                string bank = string.Empty;
                string account = string.Empty;
                List<int> claimIdentityList = new List<int>();
                List<int> advancePaybackIdentityList = new List<int>();
                Int64 amountCents = 0;
                int organizationId = 0;

                foreach (string component in components)
                {
                    int foreignId = Int32.Parse(component.Substring(1));

                    if (component[0] == 'C')
                    {
                        ExpenseClaim claim = ExpenseClaim.FromIdentity(foreignId);
                        claimIdentityList.Add(foreignId);

                        if (bank.Length < 1)
                        {
                            Person claimer = claim.Claimer;
                            bank = claimer.BankName;
                            account = claimer.BankAccount;
                            organizationId = claim.OrganizationId;
                        }

                        beneficiaryPerson = claim.Claimer;
                        organization = claim.Organization;
                        amountCents += claim.AmountCents;

                        claim.Repaid = true;
                        claim.Close();
                    }
                    else if (component[0] == 'a')
                    {
                        CashAdvance advancePayback = CashAdvance.FromIdentity(foreignId);
                        advancePaybackIdentityList.Add(foreignId);

                        amountCents -= advancePayback.AmountCents;
                        advancePayback.Open = false;
                    }
                }

                string referenceString = string.Empty;

                if (claimIdentityList.Count == 1)
                {
                    referenceString = "Expense Claim #" + claimIdentityList[0].ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    claimIdentityList.Sort();
                    referenceString = "Expense Claims " + Formatting.GenerateRangeString(claimIdentityList);
                }

                SwarmopsLogEntry.Create(creator,
                                        new PayoutCreatedLogEntry(creator, beneficiaryPerson, organization,
                                                                  organization.Currency, amountCents/100.0,
                                                                  referenceString),
                                                                  beneficiaryPerson);

                payoutId = SwarmDb.GetDatabaseForWriting().CreatePayout(organizationId, bank, account,
                                                                   referenceString, amountCents, DateTime.Today.AddDays(1),
                                                                   creator.Identity);

                foreach (int claimId in claimIdentityList)
                {
                    SwarmDb.GetDatabaseForWriting().CreatePayoutDependency(payoutId, FinancialDependencyType.ExpenseClaim,
                                                                  claimId);
                }

                foreach (int advancePaybackId in advancePaybackIdentityList)
                {
                    SwarmDb.GetDatabaseForWriting().CreatePayoutDependency(payoutId, FinancialDependencyType.CashAdvancePayback,
                                                                  advancePaybackId);
                }
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

                payoutId = SwarmDb.GetDatabaseForWriting().CreatePayout(invoice.OrganizationId, string.Empty, invoice.PayToAccount,
                                                                   invoice.Ocr.Length > 0 ? "OCR " + invoice.Ocr : "Ref# " + invoice.InvoiceReference,
                                                                   invoice.AmountCents, expectedPayment,
                                                                   creator.Identity);

                SwarmDb.GetDatabaseForWriting().CreatePayoutDependency(payoutId, FinancialDependencyType.InboundInvoice,
                                                              invoice.Identity);

                invoice.Open = false;
            }
            else if (components [0][0] == 'S')
            {
                // Salary, net payment

                Salary salary = Salary.FromIdentity(Int32.Parse(components[0].Substring(1)));

                payoutId = SwarmDb.GetDatabaseForWriting().CreatePayout(salary.PayrollItem.OrganizationId, salary.PayrollItem.Person.BankName, salary.PayrollItem.Person.BankAccount,
                                                                   "Salary " + salary.PayoutDate.ToString("yyyy-MMM"),
                                                                   salary.NetSalaryCents, salary.PayoutDate,
                                                                   creator.Identity);

                SwarmDb.GetDatabaseForWriting().CreatePayoutDependency(payoutId, FinancialDependencyType.Salary,
                                                              salary.Identity);

                salary.NetPaid = true;
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

                payoutId = SwarmDb.GetDatabaseForWriting().CreatePayout(organization.Identity, "The Tax Man", organization.Parameters.TaxAccount, organization.Parameters.TaxOcr,
                                                                   amountCents, payDay, creator.Identity);

                foreach (int salaryId in identityList)
                {
                    SwarmDb.GetDatabaseForWriting().CreatePayoutDependency(payoutId, FinancialDependencyType.Salary,
                                                                  salaryId);
                }
            }
            else
            {
                throw new NotImplementedException(); 
            }

            return FromIdentity(payoutId);
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

                // ReSharper disable LoopCanBeConvertedToQuery

                foreach (InboundInvoice invoice in DependentInvoices)
                {
                    result += "|I" + invoice.Identity.ToString(CultureInfo.InvariantCulture);
                }

                foreach (ExpenseClaim claim in DependentExpenseClaims)
                {
                    result += "|C" + claim.Identity.ToString(CultureInfo.InvariantCulture);
                }

                foreach (CashAdvance advancePayback in DependentCashAdvancesPayback)
                {
                    result += "|a" + advancePayback.Identity.ToString(CultureInfo.InvariantCulture);
                }

                foreach (CashAdvance advance in DependentCashAdvancesPayout)
                {
                    result += "|A" + advance.Identity.ToString(CultureInfo.InvariantCulture);
                }

                foreach (Salary salary in DependentSalariesNet)
                {
                    result += "|S" + salary.Identity.ToString(CultureInfo.InvariantCulture);
                }

                foreach (Salary salary in DependentSalariesTax)
                {
                    result += "|T" + salary.Identity.ToString(CultureInfo.InvariantCulture);
                }

                // ReSharper restore LoopCanBeConvertedToQuery

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
                else if (this.DependentCashAdvancesPayout.Count > 0)
                {
                    return this.DependentCashAdvancesPayout[0].Person.Canonical;
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
            accountDebitLookup[organization.FinancialAccounts.AssetsOutstandingCashAdvances.Identity] = 0;

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
            if (this.DependentCashAdvancesPayout.Count > 0)
            {
                accountDebitLookup[organization.FinancialAccounts.AssetsOutstandingCashAdvances.Identity] +=
                    this.DependentCashAdvancesPayout.TotalAmountCents;
            }
            if (this.DependentCashAdvancesPayback.Count > 0)
            {
                accountDebitLookup[organization.FinancialAccounts.AssetsOutstandingCashAdvances.Identity] -=  // observe the minus
                    this.DependentCashAdvancesPayback.TotalAmountCents;
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


        public FinancialTransaction FinancialTransaction
        {
            get { return FinancialTransaction.FromDependency(this); }
        }


        public void ReloadDependencies()
        {
            LoadDependencies();
        }


        public void MigrateDependenciesTo (Payout migrationTarget)
        {
            SwarmDb.GetDatabaseForWriting().MovePayoutDependencies(this.Identity, migrationTarget.Identity);
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
            // This only works on still-open payouts

            if (!this.Open)
            {
                throw new InvalidOperationException("Payout is already closed - cannot undo");
            }

            foreach (ExpenseClaim claim in this.DependentExpenseClaims)
            {
                claim.Repaid = false;
                claim.Open = true;
            }

            foreach (CashAdvance advance in this.DependentCashAdvancesPayout)
            {
                advance.PaidOut = false;
                advance.Open = true;
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

            SwarmDb.GetDatabaseForWriting().ClearPayoutDependencies(this.Identity);
            RecalculateAmount();
            this.Open = false;
        }

    }
}
