using System;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class FinancialTransactions :
        PluralBase<FinancialTransactions, FinancialTransaction, BasicFinancialTransaction>
    {
        public static FinancialTransactions GetIncomplete (Organization organization)
        {
            return GetIncomplete (organization.Identity);
        }

        public static FinancialTransactions GetUnbalanced (Organization organization)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetUnbalancedFinancialTransactions (organization.Identity));
        }


        public static FinancialTransactions GetIncomplete (int organizationId)
        {
            FinancialTransactions unbalanced =
                FromArray (SwarmDb.GetDatabaseForReading().GetUnbalancedFinancialTransactions (organizationId));
            FinancialTransactions undocumented =
                FromArray (SwarmDb.GetDatabaseForReading().GetUndocumentedFinancialTransactions (organizationId));

            FinancialTransactions allIncomplete = LogicalOr (unbalanced, undocumented);

            // filter by year, too
            // HACK: get book-close date from Org object
            // TODO: make org independent and get from Org object

            FinancialTransactions result = new FinancialTransactions();
            foreach (FinancialTransaction transaction in allIncomplete)
            {
                if (transaction.DateTime.Year > 2009)
                {
                    result.Add (transaction);
                }
            }

            return result;
        }

        public static FinancialTransactions ForDependentObject (IHasIdentity foreignObject)
        {
            return
                FromArray (
                    SwarmDb.GetDatabaseForReading()
                        .GetDependentFinancialTransactions (GetDependencyType (foreignObject),
                            foreignObject.Identity));
        }


        private static FinancialDependencyType GetDependencyType (IHasIdentity foreignObject)
        {
            if (foreignObject is ExpenseClaim)
            {
                return FinancialDependencyType.ExpenseClaim;
            }
            if (foreignObject is InboundInvoice)
            {
                return FinancialDependencyType.InboundInvoice;
            }

            throw new NotImplementedException ("Unimplemented dependency type:" + foreignObject.GetType());
        }

        // This needs only be run once. It populates the OrganizationSequenceId field.

        public static void FixAllUnsequenced()
        {
            SwarmDb db = SwarmDb.GetDatabaseForReading();
            FinancialTransactions txs = FromArray(db.GetAllFinancialTransactionsWithoutSequenceNumber());

            foreach (FinancialTransaction tx in txs)
            {
                tx.SetOrganizationSequenceId();
            }
        }
    }
}