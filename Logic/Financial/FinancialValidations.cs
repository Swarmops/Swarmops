using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class FinancialValidations : PluralBase<FinancialValidations, FinancialValidation, BasicFinancialValidation>
    {
        public static void Create (FinancialValidationType validationType, IHasIdentity foreignObject,
            Person actingPerson)
        {
            Create (validationType, foreignObject, actingPerson, 0.0);
        }

        public static void Create (FinancialValidationType validationType, IHasIdentity foreignObject,
            Person actingPerson, double amount)
        {
            SwarmDb.GetDatabaseForWriting()
                .CreateFinancialValidation (validationType, GetDependencyType (foreignObject), foreignObject.Identity,
                    DateTime.Now, actingPerson.Identity, amount);
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
            if (foreignObject is Salary)
            {
                return FinancialDependencyType.Salary;
            }

            throw new NotImplementedException ("Unknown dependency: " + foreignObject.GetType());
        }


        public static FinancialValidations ForObject (IHasIdentity financialDependency)
        {
            return
                FromArray (SwarmDb.GetDatabaseForReading()
                    .GetFinancialValidations (GetDependencyType (financialDependency),
                        financialDependency.Identity));
        }
    }
}