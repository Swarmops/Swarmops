using System;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
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
            return FinancialTransaction.GetFinancialDependencyType(foreignObject);
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