using System;
using System.Collections.Generic;
using System.Globalization;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Transmission;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Support.LogEntries;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class Payout : BasicPayout, ISummable
    {
        #region Creation and Construction

        private Payout (BasicPayout basic) :
            base (basic)
        {
            LoadDependencies();
        }

        public static void Create()
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

            // return payout;
        }

        public static Payout Empty
        {
            get { return new Payout (new BasicPayout (0, 0, string.Empty, string.Empty, string.Empty, 0, DateTime.UtcNow, true, DateTime.UtcNow, 0)); }
        }

        private void LoadDependencies()
        {
            if (this.DependentCashAdvancesPayback != null && Identity == 0)
            {
                return; // if inited and identity zero, return
            }

            this.DependentExpenseClaims = new ExpenseClaims();
            this.DependentInvoices = new InboundInvoices();
            this.DependentSalariesNet = new Salaries();
            this.DependentSalariesTax = new Salaries();
            this.DependentCashAdvancesPayout = new CashAdvances();
            this.DependentCashAdvancesPayback = new CashAdvances();

            if (Identity == 0)
            {
                return; // never progress past here if identity zero
            }

            BasicFinancialDependency[] dependencies = SwarmDb.GetDatabaseForReading().GetPayoutDependencies (Identity);

            foreach (BasicFinancialDependency dependency in dependencies)
            {
                switch (dependency.DependencyType)
                {
                    case FinancialDependencyType.ExpenseClaim:
                        this.DependentExpenseClaims.Add (ExpenseClaim.FromIdentity (dependency.ForeignId));
                        break;
                    case FinancialDependencyType.InboundInvoice:
                        this.DependentInvoices.Add (InboundInvoice.FromIdentity (dependency.ForeignId));
                        break;
                    case FinancialDependencyType.Salary:
                        Salary salary = Salary.FromIdentity (dependency.ForeignId);
                        if (salary.NetSalaryCents == AmountCents) // HACK/LEGACY: Assumes that tax total is not identical
                        {
                            this.DependentSalariesNet.Add (salary);
                        }
                        else
                        {
                            this.DependentSalariesTax.Add (salary);
                        }
                        break;

                    case FinancialDependencyType.SalaryTax:
                        Salary salaryTax = Salary.FromIdentity (dependency.ForeignId);
                        this.DependentSalariesTax.Add (salaryTax);
                        break;

                    case FinancialDependencyType.CashAdvance:
                        this.DependentCashAdvancesPayout.Add (CashAdvance.FromIdentity (dependency.ForeignId));
                        break;

                    case FinancialDependencyType.CashAdvancePayback:
                        this.DependentCashAdvancesPayback.Add (CashAdvance.FromIdentity (dependency.ForeignId));
                        break;

                    default:
                        throw new NotImplementedException (
                            "Unknown financial dependency type in Payout.LoadDependencies(): " + dependency);
                }
            }
        }

        public static Payout FromBasic (BasicPayout basic)
        {
            return new Payout (basic);
        }

        public static Payout FromIdentity (int payoutId)
        {
            try
            {
                return FromBasic (SwarmDb.GetDatabaseForReading().GetPayout (payoutId));
            }
            catch (ArgumentException e)
            {
                throw new InvalidOperationException (String.Format ("Error loading Payout #{0}", payoutId), e);
            }
        }

        #endregion

        public CashAdvances DependentCashAdvancesPayback;
        public CashAdvances DependentCashAdvancesPayout;
        public ExpenseClaims DependentExpenseClaims;
        public InboundInvoices DependentInvoices;
        public Salaries DependentSalariesNet;
        public Salaries DependentSalariesTax;

        public Person RecipientPerson { get; set; }

        public decimal Amount
        {
            set
            {
                if (Identity != 0)
                {
                    SwarmDb.GetDatabaseForWriting().SetPayoutAmount (Identity, (Int64) (value*100));
                }
                base.AmountCents = (Int64) value*100;
            }
            get { return base.AmountCents/100.0m; }
        }

        public new Int64 AmountCents
        {
            set
            {
                if (Identity != 0)
                {
                    SwarmDb.GetDatabaseForWriting().SetPayoutAmount (Identity, value);
                }
                base.AmountCents = value;
            }
            get { return base.AmountCents; }
        }

        public new string Reference
        {
            set
            {
                if (Identity != 0)
                {
                    SwarmDb.GetDatabaseForWriting().SetPayoutReference (Identity, value);
                }
                base.Reference = value;
            }
            get { return base.Reference; }
        }


        public new bool Open
        {
            get { return base.Open; }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetPayoutOpen (Identity, value);
                base.Open = value;
            }
        }


        public string ProtoIdentity
        {
            get
            {
                string result = string.Empty;

                // ReSharper disable LoopCanBeConvertedToQuery

                foreach (InboundInvoice invoice in this.DependentInvoices)
                {
                    result += "|I" + invoice.Identity.ToString (CultureInfo.InvariantCulture);
                }

                foreach (ExpenseClaim claim in this.DependentExpenseClaims)
                {
                    result += "|C" + claim.Identity.ToString (CultureInfo.InvariantCulture);
                }

                foreach (CashAdvance advancePayback in this.DependentCashAdvancesPayback)
                {
                    result += "|a" + advancePayback.Identity.ToString (CultureInfo.InvariantCulture);
                }

                foreach (CashAdvance advance in this.DependentCashAdvancesPayout)
                {
                    result += "|A" + advance.Identity.ToString (CultureInfo.InvariantCulture);
                }

                foreach (Salary salary in this.DependentSalariesNet)
                {
                    result += "|S" + salary.Identity.ToString (CultureInfo.InvariantCulture);
                }

                foreach (Salary salary in this.DependentSalariesTax)
                {
                    result += "|T" + salary.Identity.ToString (CultureInfo.InvariantCulture);
                }

                // ReSharper restore LoopCanBeConvertedToQuery

                if (result.Length > 0)
                {
                    return result.Substring (1);
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

                    throw new InvalidOperationException ("Dependencies not initialized");
                }

                if (this.DependentInvoices.Count > 0)
                {
                    return this.DependentInvoices[0].Supplier;
                }
                if (this.DependentExpenseClaims.Count > 0)
                {
                    return this.DependentExpenseClaims[0].ClaimerCanonical;
                }
                if (this.DependentCashAdvancesPayout.Count > 0)
                {
                    return this.DependentCashAdvancesPayout[0].Person.Canonical;
                }
                if (this.DependentSalariesNet.Count > 0)
                {
                    return this.DependentSalariesNet[0].PayrollItem.PersonCanonical;
                }
                if (this.DependentSalariesTax.Count > 0)
                {
                    return "The Tax Man";
                }
                throw new InvalidOperationException ("No dependencies for payout #" + Identity);
            }
        }

        public Organization Organization
        {
            get { return Organization.FromIdentity (OrganizationId); }
        }


        public Person CreatedByPerson
        {
            get
            {
                if (base.CreatedByPersonId > 0)
                {
                    return Person.FromIdentity (base.CreatedByPersonId);
                }

                return null;
            }
        }


        public FinancialTransaction FinancialTransaction
        {
            get { return FinancialTransaction.FromDependency (this); }
        }

        public static Payout CreateBitcoinPayoutFromPrototype (Organization organization, Payout prototype, string transactionHash)
        {
            string[] components = prototype.ProtoIdentity.Split('|');
            int payoutId = 0;

            // This function is made for complex bitcoin payouts and will typically take many different types of payouts to many people at once.

            if (components.Length == 0)
            {
                // nothing to construct. Exception or return null?

                return null;
            }

            payoutId = SwarmDb.GetDatabaseForWriting().CreatePayout(organization.Identity, "Bitcoin network", "Multiple",
                transactionHash, prototype.AmountCents, DateTime.UtcNow, 0);

            foreach (string component in components)
            {
                int foreignId = Int32.Parse(component.Substring(1));

                switch (component[0])
                {
                    case 'A':
                        // Cash advance
                        CashAdvance advance = CashAdvance.FromIdentity (foreignId);
                        advance.PaidOut = true;

                        SwarmopsLogEntry.Create (null,
                            new PayoutCreatedLogEntry (null, advance.Person, organization,
                                organization.Currency, advance.AmountCents/100.0,
                                "Cash Advance Paid Out"),
                            advance.Person, advance);

                        OutboundComm.CreateNotificationOfFinancialValidation (advance.Budget, advance.Person,
                            advance.AmountCents/100.0, advance.Description, NotificationResource.CashAdvance_PaidOut);
                        SwarmDb.GetDatabaseForWriting()
                            .CreatePayoutDependency (payoutId, FinancialDependencyType.CashAdvance,
                                foreignId);
                        break;

                    case 'a':
                        // This is a negative record - payback of cash advance
                        CashAdvance advancePayback = CashAdvance.FromIdentity (foreignId);
                        advancePayback.Open = false;

                        SwarmDb.GetDatabaseForWriting()
                            .CreatePayoutDependency(payoutId, FinancialDependencyType.CashAdvancePayback,
                                foreignId);

                        break;

                    case 'C':
                        // Expense claim
                        ExpenseClaim claim = ExpenseClaim.FromIdentity (foreignId);
                        claim.Repaid = true;
                        claim.Close();

                        OutboundComm.CreateNotificationOfFinancialValidation (claim.Budget, claim.Claimer,
                            claim.AmountCents/100.0, claim.Description, NotificationResource.ExpenseClaim_PaidOut);
                        SwarmDb.GetDatabaseForWriting()
                            .CreatePayoutDependency(payoutId, FinancialDependencyType.ExpenseClaim,
                                foreignId);

                        break;

                    case 'I':
                        // Invoice
                        InboundInvoice invoice = InboundInvoice.FromIdentity (foreignId);
                        DateTime expectedPayment = invoice.DueDate;

                        if (expectedPayment < DateTime.Today)
                        {
                            expectedPayment = DateTime.Today;
                        }

                        SwarmDb.GetDatabaseForWriting()
                            .CreatePayoutDependency (payoutId, FinancialDependencyType.InboundInvoice,
                                invoice.Identity);

                        // TODO: NOTIFY PAID?

                        invoice.Open = false;
                        break;

                    case 'S':
                        // Salary net

                        Salary salaryNet = Salary.FromIdentity (foreignId);
                        SwarmDb.GetDatabaseForWriting().CreatePayoutDependency (payoutId, FinancialDependencyType.Salary,
                            salaryNet.Identity);
                        salaryNet.NetPaid = true;
                        break;

                    case 'T':
                        // Tax payout, typically for multiple salaries

                        Salary salaryTax = Salary.FromIdentity (foreignId);
                        SwarmDb.GetDatabaseForWriting().CreatePayoutDependency (payoutId, FinancialDependencyType.SalaryTax,
                            salaryTax.Identity);
                        salaryTax.TaxPaid = true;

                        break;
                    default:
                        throw new NotImplementedException();

                }
            }

            // Return the new object by reloading it from database

            return Payout.FromIdentity (payoutId);
        }

        public static Payout CreateFromProtoIdentity (Person creator, string protoIdentity)
        {
            string[] components = protoIdentity.Split ('|');
            int payoutId = 0;

            // The components can EITHER be a series of expense claims OR a single invoice.

            if (components.Length == 0)
            {
                // nothing to construct. Exception or return null?

                return null;
            }
            if (components[0][0] == 'A')
            {
                // Cash advance(s) to be paid out.

                string bank = string.Empty;
                string account = string.Empty;
                List<int> identityList = new List<int>();
                Int64 amountCents = 0;
                int organizationId = 0;

                foreach (string component in components)
                {
                    int advanceId = Int32.Parse (component.Substring (1));
                    CashAdvance advance = CashAdvance.FromIdentity (advanceId);
                    identityList.Add (advanceId);
                    organizationId = advance.OrganizationId;
                    Organization organization = Organization.FromIdentity (advance.OrganizationId);

                    if (bank.Length < 1)
                    {
                        Person asker = advance.Person;
                        bank = asker.BankName;
                        account = asker.BankAccount;
                    }

                    amountCents += advance.AmountCents;

                    advance.PaidOut = true;
                    // advance.Open remains true until the advance is repaid

                    SwarmopsLogEntry.Create (creator,
                        new PayoutCreatedLogEntry (creator, advance.Person, organization,
                            organization.Currency, amountCents/100.0,
                            "Cash Advance Paid Out"),
                        advance.Person, advance);


                    OutboundComm.CreateNotificationOfFinancialValidation (advance.Budget, advance.Person,
                        advance.AmountCents/100.0, advance.Description, NotificationResource.CashAdvance_PaidOut);
                }

                string referenceString = string.Empty;

                if (identityList.Count == 1)
                {
                    referenceString = "Cash Advance #" + identityList[0].ToString (CultureInfo.InvariantCulture);
                }
                else
                {
                    identityList.Sort();
                    referenceString = "Cash Advances " + Formatting.GenerateRangeString (identityList);
                }

                payoutId = SwarmDb.GetDatabaseForWriting().CreatePayout (organizationId, bank, account,
                    referenceString, amountCents, DateTime.Today.AddDays (1),
                    creator.Identity);

                foreach (int advanceId in identityList)
                {
                    SwarmDb.GetDatabaseForWriting()
                        .CreatePayoutDependency (payoutId, FinancialDependencyType.CashAdvance,
                            advanceId);
                }
            }
            else if (components[0][0] == 'C')
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
                    int foreignId = Int32.Parse (component.Substring (1));

                    if (component[0] == 'C')
                    {
                        ExpenseClaim claim = ExpenseClaim.FromIdentity (foreignId);
                        claimIdentityList.Add (foreignId);

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

                        OutboundComm.CreateNotificationOfFinancialValidation (claim.Budget, claim.Claimer,
                            claim.AmountCents/100.0, claim.Description, NotificationResource.ExpenseClaim_PaidOut);
                    }
                    else if (component[0] == 'a')
                    {
                        CashAdvance advancePayback = CashAdvance.FromIdentity (foreignId);
                        advancePaybackIdentityList.Add (foreignId);

                        amountCents -= advancePayback.AmountCents;
                        advancePayback.Open = false;
                    }
                }

                string referenceString = string.Empty;

                if (claimIdentityList.Count == 1)
                {
                    referenceString = "Expense Claim #" + claimIdentityList[0].ToString (CultureInfo.InvariantCulture);
                }
                else
                {
                    claimIdentityList.Sort();
                    referenceString = "Expense Claims " + Formatting.GenerateRangeString (claimIdentityList);
                }

                SwarmopsLogEntry.Create (creator,
                    new PayoutCreatedLogEntry (creator, beneficiaryPerson, organization,
                        organization.Currency, amountCents/100.0,
                        referenceString),
                    beneficiaryPerson);

                payoutId = SwarmDb.GetDatabaseForWriting().CreatePayout (organizationId, bank, account,
                    referenceString, amountCents, DateTime.Today.AddDays (1),
                    creator.Identity);

                foreach (int claimId in claimIdentityList)
                {
                    SwarmDb.GetDatabaseForWriting()
                        .CreatePayoutDependency (payoutId, FinancialDependencyType.ExpenseClaim,
                            claimId);
                }

                foreach (int advancePaybackId in advancePaybackIdentityList)
                {
                    SwarmDb.GetDatabaseForWriting()
                        .CreatePayoutDependency (payoutId, FinancialDependencyType.CashAdvancePayback,
                            advancePaybackId);
                }
            }
            else if (components[0][0] == 'I')
            {
                // There is just one invoice per payout

                InboundInvoice invoice = InboundInvoice.FromIdentity (Int32.Parse (components[0].Substring (1)));

                DateTime expectedPayment = invoice.DueDate;

                if (expectedPayment < DateTime.Today)
                {
                    expectedPayment = DateTime.Today;
                }

                payoutId = SwarmDb.GetDatabaseForWriting()
                    .CreatePayout (invoice.OrganizationId, string.Empty, invoice.PayToAccount,
                        invoice.Ocr.Length > 0 ? "OCR " + invoice.Ocr : "Ref# " + invoice.InvoiceReference,
                        invoice.AmountCents, expectedPayment,
                        creator.Identity);

                SwarmDb.GetDatabaseForWriting()
                    .CreatePayoutDependency (payoutId, FinancialDependencyType.InboundInvoice,
                        invoice.Identity);

                invoice.Open = false;
            }
            else if (components[0][0] == 'S')
            {
                // Salary, net payment

                Salary salary = Salary.FromIdentity (Int32.Parse (components[0].Substring (1)));

                payoutId = SwarmDb.GetDatabaseForWriting()
                    .CreatePayout (salary.PayrollItem.OrganizationId, salary.PayrollItem.Person.BankName,
                        salary.PayrollItem.Person.BankAccount,
                        "Salary " + salary.PayoutDate.ToString ("yyyy-MMM"),
                        salary.NetSalaryCents, salary.PayoutDate,
                        creator.Identity);

                SwarmDb.GetDatabaseForWriting().CreatePayoutDependency (payoutId, FinancialDependencyType.Salary,
                    salary.Identity);

                salary.NetPaid = true;
            }
            else if (components[0][0] == 'T')
            {
                // Tax payment for multiple salaries.

                List<int> identityList = new List<int>();
                Int64 amountCents = 0;
                int organizationId = 0;
                DateTime payDay = DateTime.Today.AddDays (1);

                foreach (string component in components)
                {
                    int salaryId = Int32.Parse (component.Substring (1));
                    Salary salary = Salary.FromIdentity (salaryId);
                    identityList.Add (salaryId);

                    if (organizationId == 0)
                    {
                        organizationId = salary.PayrollItem.OrganizationId;
                        payDay = salary.PayoutDate;
                    }

                    amountCents += salary.TaxTotalCents;

                    salary.TaxPaid = true;
                }

                string referenceString = string.Empty;
                Organization organization = Organization.FromIdentity (organizationId);

                if (identityList.Count == 1)
                {
                    referenceString = "Tax for salary #" + identityList[0];
                }
                else
                {
                    identityList.Sort();
                    referenceString = "Tax for salaries " + Formatting.GenerateRangeString (identityList);
                }

                payoutId = SwarmDb.GetDatabaseForWriting()
                    .CreatePayout (organization.Identity, "The Tax Man", organization.Parameters.TaxAccount,
                        organization.Parameters.TaxOcr,
                        amountCents, payDay, creator.Identity);

                foreach (int salaryId in identityList)
                {
                    SwarmDb.GetDatabaseForWriting().CreatePayoutDependency (payoutId, FinancialDependencyType.SalaryTax,
                        salaryId);
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return FromIdentity (payoutId);
        }

        public void BindToTransactionAndClose (FinancialTransaction transaction, Person bindingPerson)
        {
            Dictionary<int, Int64> accountDebitLookup = new Dictionary<int, Int64>();
            Organization organization = Organization;

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
                accountDebitLookup[organization.FinancialAccounts.AssetsOutstandingCashAdvances.Identity] -=
                    // observe the minus
                    this.DependentCashAdvancesPayback.TotalAmountCents;
            }

            foreach (int financialAccountId in accountDebitLookup.Keys)
            {
                if (accountDebitLookup[financialAccountId] != 0)
                {
                    transaction.AddRow (FinancialAccount.FromIdentity (financialAccountId),
                        accountDebitLookup[financialAccountId], bindingPerson);
                }
            }

            transaction.Dependency = this;
            Open = false;
        }


        public void ReloadDependencies()
        {
            LoadDependencies();
        }


        public void MigrateDependenciesTo (Payout migrationTarget)
        {
            if (Identity > 0 && migrationTarget.Identity > 0)
            {
                // Persisted payout migration

                SwarmDb.GetDatabaseForWriting().MovePayoutDependencies (Identity, migrationTarget.Identity);
            }
            else
            {
                // In-memory migration: this payout isn't in database yet

                this.DependentCashAdvancesPayback.ForEach(item => migrationTarget.DependentCashAdvancesPayback.Add (item));
                this.DependentCashAdvancesPayout.ForEach (item => migrationTarget.DependentCashAdvancesPayout.Add (item));
                this.DependentExpenseClaims.ForEach(item => migrationTarget.DependentExpenseClaims.Add(item));
                this.DependentInvoices.ForEach(item => migrationTarget.DependentInvoices.Add(item));
                this.DependentSalariesNet.ForEach(item => migrationTarget.DependentSalariesNet.Add(item));
                this.DependentSalariesTax.ForEach(item => migrationTarget.DependentSalariesTax.Add(item));

                this.DependentCashAdvancesPayback = new CashAdvances();
                this.DependentCashAdvancesPayout = new CashAdvances();
                this.DependentExpenseClaims = new ExpenseClaims();
                this.DependentInvoices = new InboundInvoices();
                this.DependentSalariesNet = new Salaries();
                this.DependentSalariesTax = new Salaries();
            }
            migrationTarget.RecalculateAmount();
            RecalculateAmount();
        }


        private void RecalculateAmount()
        {
            Int64 newAmountCents = 0;

            LoadDependencies();

            newAmountCents += this.DependentExpenseClaims.TotalAmountCents;
            newAmountCents += this.DependentInvoices.TotalAmountCents;
            newAmountCents += this.DependentSalariesNet.TotalAmountCentsNet;
            newAmountCents += this.DependentSalariesTax.TotalAmountCentsTax;

            AmountCents = newAmountCents;
        }


        public void UndoPayout()
        {
            // This only works on still-open payouts

            if (!Open)
            {
                throw new InvalidOperationException ("Payout is already closed - cannot undo");
            }

            foreach (ExpenseClaim claim in this.DependentExpenseClaims)
            {
                claim.Repaid = false;
                claim.Open = true;
            }

            foreach (CashAdvance advance in this.DependentCashAdvancesPayout)
            {
                advance.PaidOut = false;
                advance.Open = true; // BUG: Why is this here? Isn't the cash advance open until repaid?
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

            SwarmDb.GetDatabaseForWriting().ClearPayoutDependencies (Identity);
            RecalculateAmount();
            Open = false;
        }


        /// <summary>
        /// Gets a (localized) specification of what this payout is for. Will return a type and identity or identities. Does not include the recipient name.
        /// </summary>
        public string Specification
        {
            get
            {
                if (this.DependentCashAdvancesPayout.Count > 0)
                {
                    if (this.DependentCashAdvancesPayout.Count > 1)
                    {
                        return String.Format (Resources.Logic_Financial_Payout.Payout_CashAdvanceIdentities,
                            Formatting.GenerateRangeString (this.DependentCashAdvancesPayout.Identities));
                    }

                    return String.Format(Resources.Logic_Financial_Payout.Payout_CashAdvanceIdentity,
                        this.DependentCashAdvancesPayout [0].Identity);
                }

                if (this.DependentExpenseClaims.Count > 0)
                {
                    if (this.DependentExpenseClaims.Count > 1)
                    {
                        if (this.DependentCashAdvancesPayback.Count > 0)
                        {
                            if (this.DependentCashAdvancesPayback.Count > 1)
                            {
                                // multiple expenses, multiple paybacks

                                return String.Format(Resources.Logic_Financial_Payout.Payout_ExpenseClaimIdentitiesLessAdvanceIdentities,
                                     Formatting.GenerateRangeString(this.DependentExpenseClaims.Identities),
                                     Formatting.GenerateRangeString(this.DependentCashAdvancesPayback.Identities));
                            }

                            // multiple expenses, single payback

                            return String.Format(Resources.Logic_Financial_Payout.Payout_ExpenseClaimIdentitiesLessAdvanceIdentities,
                                Formatting.GenerateRangeString(this.DependentExpenseClaims.Identities),
                                this.DependentCashAdvancesPayback[0].Identity);
                        }

                        // multiple expenses, no payback

                        return String.Format (Resources.Logic_Financial_Payout.Payout_ExpenseClaimIdentities,
                            Formatting.GenerateRangeString (this.DependentExpenseClaims.Identities));
                    }

                    // single expense, no payback

                    return String.Format (Resources.Logic_Financial_Payout.Payout_ExpenseClaimIdentity,
                        this.DependentExpenseClaims[0].Identity);
                }

                if (this.DependentInvoices.Count > 0)
                {
                    if (this.DependentInvoices.Count > 1)
                    {
                        return @"NO SUPPORT FOR MULTIPLE INVOICES - FIX IN PAYOUT.SPECIFICATION";
                    }

                    return String.Format (Resources.Logic_Financial_Payout.Payout_InboundInvoiceIdentity,
                        this.DependentInvoices[0].Identity);
                }

                if (this.DependentSalariesNet.Count > 0)
                {
                    if (this.DependentSalariesNet.Count > 1)
                    {
                        return @"NO SUPPORT FOR MULTIPLE NET SALARIES - FIX IN PAYOUT.SPECIFICATION";
                    }

                    return String.Format (Resources.Logic_Financial_Payout.Payout_SalaryIdentityMonth,
                        this.DependentSalariesNet[0].Identity,
                        this.DependentSalariesNet[0].PayoutDate);
                }

                if (this.DependentSalariesTax.Count > 0)
                {
                    if (this.DependentSalariesTax.Count > 1)
                    {
                        // tax for multiple salaries

                        return String.Format (Resources.Logic_Financial_Payout.Payout_SalaryTaxIdentitiesMonth,
                            Formatting.GenerateRangeString (this.DependentSalariesTax.Identities),
                            this.DependentSalariesTax[0].PayoutDate); // assumes all salaries on same date
                    }
                }

                return @"UNSUPPORTED CASE - FIX IN PAYOUT.SPECIFICATION";

            }
        }

        #region Implementation of ISummable

        public long SumCents
        {
            get { return AmountCents; }
        }

        public OrganizationFinancialAccountType CounterAccountType
        {
            get { return OrganizationFinancialAccountType.Unknown; }
        }

        #endregion
    }
}