using System;
using System.Collections.Generic;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class FinancialTransaction : BasicFinancialTransaction
    {
        #region Construction and Creation

        private FinancialTransaction (BasicFinancialTransaction basic)
            : base (basic)
        {
        }

        public static FinancialTransaction FromBasic (BasicFinancialTransaction basic)
        {
            return new FinancialTransaction (basic);
        }

        public static FinancialTransaction FromIdentity (int financialTransactionId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetFinancialTransaction (financialTransactionId));
        }

        public static FinancialTransaction FromIdentityAggressive (int financialTransactionId)
        {
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetFinancialTransaction (financialTransactionId));
            // ForWriting intentional - bypass replication lag
        }

        public static FinancialTransaction FromDependency (IHasIdentity dependency)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetFinancialTransactionFromDependency (dependency));
        }


        public static FinancialTransaction FromImportKey (Organization organization, string importKey)
        {
            return
                FromBasic (SwarmDb.GetDatabaseForReading()
                    .GetFinancialTransactionFromImportKey (organization.Identity, importKey));
        }


        public static FinancialTransaction ImportWithStub (int organizationId, DateTime dateTime, int financialAccountId,
            Int64 amountCents, string description, string importHash, int personId)
        {
            int transactionId = SwarmDb.GetDatabaseForWriting()
                .CreateFinancialTransactionStub (organizationId, dateTime,
                    financialAccountId, amountCents,
                    description, importHash, personId);

            if (transactionId == 0)
            {
                return null; // This was a dupe -- already imported, as determined by ImportHash
            }

            return FromIdentityAggressive (transactionId);
        }


        public static FinancialTransaction Create (int organizationId, DateTime dateTime, string description)
        {
            int transactionId = SwarmDb.GetDatabaseForWriting()
                .CreateFinancialTransaction (organizationId, dateTime, description);
            return FromIdentityAggressive (transactionId);
        }

        #endregion

        public FinancialTransactionRows Rows
        {
            get
            {
                return
                    FinancialTransactionRows.FromArray (
                        SwarmDb.GetDatabaseForReading().GetFinancialTransactionRows (Identity));
            }
        }

        public Documents Documents
        {
            get { return Documents.ForObject (this); }
        }

        public Int64 this [FinancialAccount account]
        {
            get
            {
                Int64 result = 0;

                FinancialTransactionRows rows = Rows;

                foreach (FinancialTransactionRow row in rows)
                {
                    if (row.FinancialAccountId == account.Identity)
                    {
                        result += row.AmountCents;
                    }
                }

                return result;
            }
        }

        public new string Description
        {
            get { return base.Description; }
            set
            {
                base.Description = value;
                SwarmDb.GetDatabaseForWriting().SetFinancialTransactionDescription (Identity, value);
            }
        }

        private FinancialTransaction ContinuedTransaction
        {
            get
            {
                try
                {
                    return FromDependency (this);
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }
        }

        public IHasIdentity Dependency
        {
            set
            {
                SwarmDb.GetDatabaseForWriting().SetFinancialTransactionDependency (
                    Identity, GetFinancialDependencyType (value), value.Identity);
            }
            get
            {
                // This uses OUT parameters, which goes against .Net Guidelines. The proper way
                // is to create a new type for it, BasicFinancialDependency. Do this in some
                // semi-near future.

                FinancialDependencyType dependencyType;
                int foreignId;

                SwarmDb.GetDatabaseForReading().GetFinancialTransactionDependency (Identity, out dependencyType,
                    out foreignId);

                if (foreignId == 0)
                {
                    return null;
                }

                if (dependencyType == FinancialDependencyType.ExpenseClaim)
                {
                    return ExpenseClaim.FromIdentity (foreignId);
                }

                if (dependencyType == FinancialDependencyType.InboundInvoice)
                {
                    return InboundInvoice.FromIdentity (foreignId);
                }

                if (dependencyType == FinancialDependencyType.OutboundInvoice)
                {
                    return OutboundInvoice.FromIdentity (foreignId);
                }

                if (dependencyType == FinancialDependencyType.Salary)
                {
                    return Salary.FromIdentity (foreignId);
                }

                if (dependencyType == FinancialDependencyType.Payout)
                {
                    return Payout.FromIdentity (foreignId);
                }

                if (dependencyType == FinancialDependencyType.PaymentGroup)
                {
                    return PaymentGroup.FromIdentity (foreignId);
                }

                if (dependencyType == FinancialDependencyType.FinancialTransaction)
                {
                    return FromIdentity (foreignId);
                }

                throw new NotImplementedException ("Unimplemented dependency type: " + dependencyType);
            }
        }

        [Obsolete ("Do not use double-precision methods. They leak cents. Use AddRow (FinancialAccount, Int64, Person).",
            true)]
        public void AddRow (FinancialAccount account, double amount, Person person)
        {
            AddRow (account, (Int64) (amount*100), person);
        }

        public void AddRow (FinancialAccount account, Int64 amountCents, Person person)
        {
            AddRow (account.Identity, amountCents, person != null ? person.Identity : 0);
        }

        private void AddRow (int financialAccountId, Int64 amountCents, int personId)
        {
            if (DateTime.Year <=
                FinancialAccount.FromIdentity (financialAccountId).Organization.Parameters.FiscalBooksClosedUntilYear)
            {
                // Recurse down into continuation transactions to write row in first nonclosed year

                FinancialTransaction transactionContinued = ContinuedTransaction;

                if (transactionContinued == null)
                {
                    // No continuation; create one

                    transactionContinued = Create (OrganizationId, DateTime.Now,
                        "Continued Tx #" + Identity);
                    transactionContinued.AddRow (financialAccountId, amountCents, personId);
                    transactionContinued.Dependency = this;
                }
                else
                {
                    // Recurse

                    transactionContinued.AddRow (financialAccountId, amountCents, personId);
                }

                return;
            }

            SwarmDb.GetDatabaseForWriting()
                .CreateFinancialTransactionRow (Identity, financialAccountId, amountCents, personId);
        }

        public void AddDocument (string serverFileName, string originalFileName, Int64 fileSize, string description,
            Person uploader)
        {
            // Determine a new client file name

            int indexOfLastPeriod = originalFileName.LastIndexOf ('.');
            string extension = originalFileName.Substring (indexOfLastPeriod).ToLower();
            string newClientFileName = "transaction_" + Identity + "_document_" +
                                       DateTime.Now.ToString ("yyyyMMddHHmmss") + extension;

            // Create the document

            Document.Create (serverFileName, newClientFileName, fileSize, description, this, uploader);
        }


        private static FinancialDependencyType GetFinancialDependencyType (IHasIdentity foreignObject)
        {
            if (foreignObject is ExpenseClaim)
            {
                return FinancialDependencyType.ExpenseClaim;
            }
            if (foreignObject is InboundInvoice)
            {
                return FinancialDependencyType.InboundInvoice;
            }
            if (foreignObject is OutboundInvoice)
            {
                return FinancialDependencyType.OutboundInvoice;
            }
            if (foreignObject is Salary)
            {
                return FinancialDependencyType.Salary;
            }
            if (foreignObject is Payout)
            {
                return FinancialDependencyType.Payout;
            }
            if (foreignObject is PaymentGroup)
            {
                return FinancialDependencyType.PaymentGroup;
            }
            if (foreignObject is FinancialTransaction)
            {
                return FinancialDependencyType.FinancialTransaction;
            }

            throw new NotImplementedException ("Unidentified dependency encountered in GetFinancialDependencyType:" +
                                               foreignObject.GetType());
        }


        public void CreateTag (FinancialTransactionTagType tagType, Person creatingPerson)
        {
            // TODO: Verify that there isn't already a tag in this set in the transaction, and if so, delete it first

            SwarmDb.GetDatabaseForWriting().CreateFinancialTransactionTag (Identity, tagType.Identity);

            // TODO: Log that the tag was added (and by whom?)
        }

        public FinancialTransactionTagType GetTag (FinancialTransactionTagSet tagSet)
        {
            // We're lazy: we're getting ALL tags and picking out the correct one. It's a minimum of overhead given that
            // there should be at most five records in the most advanced scenarios, and this is cheaper than doing it
            // database-side.

            // Possible todo: cache tags (it's likely there are several calls in a row).

            FinancialTransactionTagTypes tagTypes = FinancialTransactionTagTypes.ForTransaction (this);

            foreach (FinancialTransactionTagType tagType in tagTypes)
            {
                if (tagType.FinancialTransactionTagSetId == tagSet.Identity)
                {
                    return tagType;
                }
            }

            // None found, so return null

            return null;
        }


        public Dictionary<int, Int64> GetRecalculationBase()
        {
            Dictionary<int, Int64> currentTransaction = new Dictionary<int, Int64>();

            foreach (FinancialTransactionRow row in Rows)
            {
                if (!currentTransaction.ContainsKey(row.FinancialAccountId))
                {
                    currentTransaction[row.FinancialAccountId] = 0;
                }

                currentTransaction[row.FinancialAccountId] += row.AmountCents;
            }

            return currentTransaction;
        }


        public bool RecalculateTransaction (Dictionary<int, Int64> nominalTransaction, Person loggingPerson)
        {
            bool changedTransaction = false;

            // We need to create a delta. This is... somewhat complicated.

            // 1) Iterate over the rows to build a "current" transaction record.
            // 2) Create a "should-look-like" transaction record. (done in calling routine, already).
            // 3) Apply the delta, in two steps.

            Dictionary<int, Int64> currentTransaction = GetRecalculationBase();

            FinancialTransaction continuedTransaction = ContinuedTransaction;

            if (continuedTransaction != null)
            {
                continuedTransaction.AddContinuedTransactionsToLookup (currentTransaction);
                // Recurses to all continued transactions
            }

            // Step 2: create an image of what the transaction SHOULD look like with changes.
            //         now done in calling routine.

            // Step 3a: For all accounts existing in Current but not in Nominal, set them to 0, and
            // vice versa.

            foreach (int accountId in currentTransaction.Keys)
            {
                if (!nominalTransaction.ContainsKey (accountId))
                {
                    nominalTransaction[accountId] = 0;
                }
            }

            foreach (int accountId in nominalTransaction.Keys)
            {
                if (!currentTransaction.ContainsKey (accountId))
                {
                    currentTransaction[accountId] = 0;
                }
            }

            // Step 3b: Iterate over all accounts in the two sets -- which now has the same keys --
            // and apply the delta to the transaction.

            foreach (int accountId in currentTransaction.Keys)
            {
                if (currentTransaction[accountId] != nominalTransaction[accountId])
                {
                    AddRow (accountId, nominalTransaction[accountId] - currentTransaction[accountId],
                        loggingPerson == null ? 0 : loggingPerson.Identity);
                    changedTransaction = true;
                }
            }

            return changedTransaction;
        }

        private void AddContinuedTransactionsToLookup (Dictionary<int, Int64> currentTransactionData)
        {
            FinancialTransaction continuedTransaction = ContinuedTransaction;

            if (continuedTransaction != null)
            {
                continuedTransaction.AddContinuedTransactionsToLookup (currentTransactionData);
            }

            foreach (FinancialTransactionRow row in Rows)
            {
                if (!currentTransactionData.ContainsKey (row.FinancialAccountId))
                {
                    currentTransactionData[row.FinancialAccountId] = 0;
                }

                currentTransactionData[row.FinancialAccountId] += row.AmountCents;
            }
        }
    }
}