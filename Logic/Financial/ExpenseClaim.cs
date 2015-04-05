using System;
using System.Collections.Generic;
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
    public class ExpenseClaim : BasicExpenseClaim, IValidatable, IAttestable, IPayable
    {
        #region Construction and Creation

        private ExpenseClaim() : base (null)
        {
        } // Private constructor prevents wanton creation

        private ExpenseClaim (BasicExpenseClaim basic) : base (basic)
        {
        } // Used by FromBasic()

        public static ExpenseClaim FromBasic (BasicExpenseClaim basic)
        {
            return new ExpenseClaim (basic);
        }

        public static ExpenseClaim FromIdentity (int expenseId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetExpenseClaim (expenseId));
        }

        public static ExpenseClaim FromIdentityAggressive (int expenseId)
        {
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetExpenseClaim (expenseId));
            // ForWriting is intentional - bypass replication lag
        }

        public static ExpenseClaim Create (Person claimer, Organization organization, FinancialAccount budget,
            DateTime expenseDate, string description, Int64 amountCents)
        {
            ExpenseClaim newClaim =
                FromIdentityAggressive (SwarmDb.GetDatabaseForWriting()
                    .CreateExpenseClaim (claimer.Identity, organization.Identity,
                        budget.Identity, expenseDate, description, amountCents));
            // Create the financial transaction with rows

            string transactionDescription = "Expense #" + newClaim.Identity + ": " + description; // TODO: Localize

            if (transactionDescription.Length > 64)
            {
                transactionDescription = transactionDescription.Substring (0, 61) + "...";
            }

            FinancialTransaction transaction =
                FinancialTransaction.Create (organization.Identity, DateTime.Now,
                    transactionDescription);

            transaction.AddRow (organization.FinancialAccounts.DebtsExpenseClaims, -amountCents, claimer);
            transaction.AddRow (budget, amountCents, claimer);

            // Make the transaction dependent on the expense claim

            transaction.Dependency = newClaim;

            // Create notifications

            OutboundComm.CreateNotificationAttestationNeeded (budget, claimer, string.Empty, amountCents/100.0,
                description, NotificationResource.ExpenseClaim_Created); // Slightly misplaced logic, but failsafer here
            OutboundComm.CreateNotificationFinancialValidationNeeded (organization, amountCents/100.0,
                NotificationResource.Receipts_Filed);
            SwarmopsLogEntry.Create (claimer,
                new ExpenseClaimFiledLogEntry (claimer /*filing person*/, claimer /*beneficiary*/, amountCents/100.0,
                    budget, description), newClaim);

            return newClaim;
        }

        #endregion

        public Person Claimer
        {
            get { return Person.FromIdentity (base.ClaimingPersonId); }
        }

        public bool Approved
        {
            get { return Validated && Attested; }
        }


        public string ClaimerCanonical
        {
            get
            {
                try
                {
                    return Claimer.Canonical;
                }
                catch (ArgumentException)
                {
                    return "NOT-IN-DATABASE"; // For development purposes only!
                }
            }
        }

        public Organization Organization
        {
            get { return Organization.FromIdentity (OrganizationId); }
        }

        public FinancialTransaction FinancialTransaction
        {
            get
            {
                FinancialTransactions transactions = FinancialTransactions.ForDependentObject (this);

                if (transactions.Count == 0)
                {
                    return null; // not possible from July 26 onwards, but some grandfather work
                }

                if (transactions.Count == 1)
                {
                    return transactions[0];
                }

                throw new InvalidOperationException ("It appears expense claim #" + Identity +
                                                     " has multiple dependent financial transactions. This is an invalid state.");
            }
        }

        public new bool Attested
        {
            get { return base.Attested; }
            set
            {
                if (base.Attested != value)
                {
                    SwarmDb.GetDatabaseForWriting().SetExpenseClaimAttested (Identity, value);
                    base.Attested = value;
                }
            }
        }

        public new bool Validated
        {
            get { return base.Validated; }
            set
            {
                if (base.Validated != value)
                {
                    SwarmDb.GetDatabaseForWriting().SetExpenseClaimValidated (Identity, value);
                    base.Validated = value;
                }
            }
        }

        public new bool Open
        {
            get { return base.Open; }
            set
            {
                if (base.Open != value)
                {
                    base.Open = value;
                    SwarmDb.GetDatabaseForWriting().SetExpenseClaimOpen (Identity, value);
                }
            }
        }

        public new bool Claimed
        {
            get { return base.Claimed; }
            set
            {
                if (base.Claimed != value)
                {
                    base.Claimed = value;
                    SwarmDb.GetDatabaseForWriting().SetExpenseClaimClaimed (Identity, value);
                    UpdateFinancialTransaction (Claimer);
                }
            }
        }

        public new bool Repaid
        {
            get { return base.Repaid; }
            set
            {
                if (base.Repaid != value)
                {
                    base.Repaid = value;
                    SwarmDb.GetDatabaseForWriting().SetExpenseClaimRepaid (Identity, value);
                }
            }
        }

        public new bool KeepSeparate
        {
            get { return base.KeepSeparate; }
            set
            {
                if (base.KeepSeparate != value)
                {
                    base.KeepSeparate = value;
                    SwarmDb.GetDatabaseForWriting().SetExpenseClaimKeepSeparate (Identity, value);
                }
            }
        }

        public new string Description
        {
            get { return base.Description; }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetExpenseClaimDescription (Identity, value);
                base.Description = value;
            }
        }

        public int BudgetYear
        {
            get { return CreatedDateTime.Year; }
            set
            {
                // ignore
            }
        }

        public decimal Amount
        {
            get { return base.AmountCents/100.0m; }
        }

        public new Int64 AmountCents
        {
            get { return base.AmountCents; }
        }

        public new DateTime ExpenseDate
        {
            get { return base.ExpenseDate; }
            set
            {
                // TODO
            }
        }


        public Documents Documents
        {
            get { return Documents.ForObject (this); }
        }


        public FinancialValidations Validations
        {
            get { return FinancialValidations.ForObject (this); }
        }

        public Payout Payout
        {
            get
            {
                if (Open)
                {
                    return null; // or throw?
                }

                int payoutId =
                    SwarmDb.GetDatabaseForReading().GetPayoutIdFromDependency (this);

                if (payoutId == 0)
                {
                    return null; // or throw? When expense claim system was being phased in, payouts did not exist
                }

                return Payout.FromIdentity (payoutId);
            }
        }

        #region IValidatable Members

        public void Validate (Person validator)
        {
            SwarmDb.GetDatabaseForWriting().SetExpenseClaimValidated (Identity, true);
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.Validation,
                FinancialDependencyType.ExpenseClaim, Identity,
                DateTime.Now, validator.Identity, (double) Amount);
            base.Validated = true;

            OutboundComm.CreateNotificationOfFinancialValidation (Budget, Claimer, AmountCents/100.0, Description,
                NotificationResource.ExpenseClaim_Validated);
        }

        public void Devalidate (Person devalidator)
        {
            SwarmDb.GetDatabaseForWriting().SetExpenseClaimValidated (Identity, false);
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.Devalidation,
                FinancialDependencyType.ExpenseClaim, Identity,
                DateTime.Now, devalidator.Identity, (double) Amount);
            base.Validated = false;

            OutboundComm.CreateNotificationOfFinancialValidation (Budget, Claimer, AmountCents/100.0, Description,
                NotificationResource.ExpenseClaim_Devalidated);
        }

        #endregion

        #region IAttestable Members

        public void Attest (Person attester)
        {
            SwarmDb.GetDatabaseForWriting().SetExpenseClaimAttested (Identity, true);
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.Attestation,
                FinancialDependencyType.ExpenseClaim, Identity,
                DateTime.Now, attester.Identity, (double) Amount);
            base.Attested = true;

            OutboundComm.CreateNotificationOfFinancialValidation (Budget, Claimer, AmountCents/100.0, Description,
                NotificationResource.ExpenseClaim_Attested);
        }

        public void Deattest (Person deattester)
        {
            SwarmDb.GetDatabaseForWriting().SetExpenseClaimAttested (Identity, false);
            SwarmDb.GetDatabaseForWriting().CreateFinancialValidation (FinancialValidationType.Deattestation,
                FinancialDependencyType.ExpenseClaim, Identity,
                DateTime.Now, deattester.Identity, (double) Amount);
            base.Attested = false;

            OutboundComm.CreateNotificationOfFinancialValidation (Budget, Claimer, AmountCents/100.0, Description,
                NotificationResource.ExpenseClaim_Deattested);
        }

        #endregion

        public FinancialAccount Budget
        {
            get { return FinancialAccount.FromIdentity (base.BudgetId); }
        }

        [Obsolete ("Obsolete", true)]
        public void CreateEvent (ExpenseEventType eventType, Person person)
        {
            // OBSOLETE

            // CreateEvent (eventType, person.Identity);
        }

        public void CreateEvent (ExpenseEventType eventType, int personId)
        {
            // OBSOLETE

            // SwarmDb.GetDatabaseForWriting().CreateExpenseEvent (Identity, eventType, personId);

            // TODO: Repopulate Events property, when created
        }

        public void Close()
        {
            Open = false;
        }

        public void SetAmountCents (Int64 amountCents, Person settingPerson)
        {
            if (base.AmountCents == amountCents)
            {
                return;
            }

            base.AmountCents = amountCents;
            SwarmDb.GetDatabaseForWriting().SetExpenseClaimAmount (Identity, amountCents);
            UpdateFinancialTransaction (settingPerson);
        }


        public void SetBudget (FinancialAccount budget, Person settingPerson)
        {
            SwarmDb.GetDatabaseForWriting().SetExpenseClaimBudget (Identity, budget.Identity);
            base.BudgetId = budget.Identity;
            UpdateFinancialTransaction (settingPerson);
        }


        public void Kill (Person killingPerson)
        {
            // Set the state to Closed, Unvalidated, Unattested

            Attested = false;
            Validated = false;
            Open = false;

            UpdateFinancialTransaction (killingPerson);
            // will zero out transaction since both Validated and Open are false

            // Mark transaction as invalid in description

            FinancialTransaction.Description = "Expense Claim #" + Identity + " (killed/zeroed)";
        }


        public void Recalibrate()
        {
            UpdateFinancialTransaction (null); // only to be used for fix-bookkeeping scripts
        }


        private void UpdateFinancialTransaction (Person updatingPerson)
        {
            Dictionary<int, Int64> nominalTransaction = new Dictionary<int, Int64>();

            int debtAccountId = Organization.FinancialAccounts.DebtsExpenseClaims.Identity;

            if (!Claimed)
            {
                debtAccountId = Organization.FinancialAccounts.CostsAllocatedFunds.Identity;
            }

            if (Validated || Open)
            {
                // ...only holds values if not closed as invalid...

                nominalTransaction[debtAccountId] = -AmountCents;
                nominalTransaction[BudgetId] = AmountCents;
            }

            FinancialTransaction.RecalculateTransaction (nominalTransaction, updatingPerson);
        }

        public bool PaidOut // IPayable naming convention
        {
            get { return this.Repaid; }
            set { this.Repaid = value;  }
        }
    }
}