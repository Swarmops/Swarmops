using System;
using System.Collections.Generic;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Enums;
using Activizr.Basic.Interfaces;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
{
    public class FinancialTransactions : PluralBase<FinancialTransactions,FinancialTransaction,BasicFinancialTransaction>
    {
        public static FinancialTransactions GetIncomplete (Organization organization)
        {
            return GetIncomplete (organization.Identity);
        }

        public static FinancialTransactions GetUnbalanced (Organization organization)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetUnbalancedFinancialTransactions(organization.Identity));
        }


        public static FinancialTransactions GetIncomplete(int organizationId)
        {
            FinancialTransactions unbalanced =
                FromArray(PirateDb.GetDatabaseForReading().GetUnbalancedFinancialTransactions(organizationId));
            FinancialTransactions undocumented =
                FromArray(PirateDb.GetDatabaseForReading().GetUndocumentedFinancialTransactions(organizationId));

            FinancialTransactions allIncomplete = LogicalOr(unbalanced, undocumented);

            // filter by year, too
            // HACK: get book-close date from Org object
            // TODO: make org independent and get from Org object

            FinancialTransactions result = new FinancialTransactions();
            foreach (FinancialTransaction transaction in allIncomplete)
            {
                if (transaction.DateTime.Year > 2009)
                {
                    result.Add(transaction);
                }
            }

            return result;
        }

        public static FinancialTransactions ForDependentObject (IHasIdentity foreignObject)
        {
            return
                FromArray(PirateDb.GetDatabaseForReading().GetDependentFinancialTransactions(GetDependencyType(foreignObject),
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

            throw new NotImplementedException("Unimplemented dependency type:" + foreignObject.GetType().ToString());
        }
    }
}