using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;
using Activizr.Logic.Support;
using Activizr.Basic.Enums;
using Activizr.Basic.Interfaces;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
{
    public class FinancialValidations: PluralBase<FinancialValidations,FinancialValidation,BasicFinancialValidation>
    {
        public static void Create (FinancialValidationType validationType, IHasIdentity foreignObject, Person actingPerson)
        {
            Create(validationType, foreignObject, actingPerson, 0.0);
        }

        public static void Create (FinancialValidationType validationType, IHasIdentity foreignObject, Person actingPerson, double amount)
        {
            PirateDb.GetDatabaseForWriting().CreateFinancialValidation(validationType, GetDependencyType(foreignObject), foreignObject.Identity,
                DateTime.Now, actingPerson.Identity, amount);
        }

        static FinancialDependencyType GetDependencyType (IHasIdentity foreignObject)
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

            throw new NotImplementedException("Unknown dependency: " + foreignObject.GetType().ToString());
        }


        public static FinancialValidations ForObject (IHasIdentity financialDependency)
        {
            return
                FromArray(PirateDb.GetDatabaseForReading().GetFinancialValidations(GetDependencyType(financialDependency),
                                                                         financialDependency.Identity));
        }
    }
}
