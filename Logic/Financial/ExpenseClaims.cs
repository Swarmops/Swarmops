using System;
using System.Linq;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    public class ExpenseClaims : PluralBase<ExpenseClaims,ExpenseClaim,BasicExpenseClaim>
    {
        public ExpenseClaims WhereOpen
        {
            get
            {
                var result = new ExpenseClaims();
                result.AddRange(this.Where(expense => expense.Open));

                return result;
            }
        }

        public ExpenseClaims WhereApproved
        {
            get
            {
                var result = new ExpenseClaims();
                result.AddRange(this.Where(expense => expense.Approved));

                return result;
            }
        }

        public ExpenseClaims WhereUnapproved
        {
            get
            {
                var result = new ExpenseClaims();
                result.AddRange(this.Where(expense => !expense.Approved));

                return result;
            }
        }


        public ExpenseClaims WhereUnattested
        {
            get
            {
                ExpenseClaims result = new ExpenseClaims();
                result.AddRange(this.Where(expenseClaim => !expenseClaim.Attested));

                return result;
            }
        }



        public ExpenseClaims WhereUnvalidated
        {
            get
            {
                ExpenseClaims result = new ExpenseClaims();
                result.AddRange(this.Where(expenseClaim => !expenseClaim.Validated));

                return result;
            }
        }



        public static ExpenseClaims FromClaimingPersonAndOrganization(Person person, Organization organization)
        {
            return
                FromArray(SwarmDb.GetDatabaseForReading().GetExpenseClaimsByClaimerAndOrganization(person.Identity,
                                                                                          organization.Identity));
        }


        public static ExpenseClaims FromClaimingPerson (Person person)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetExpenseClaimsByClaimer(person.Identity));
        }

        public static ExpenseClaims FromOrganization (Organization org)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetExpenseClaimsByOrganization(org.Identity));
        }


        public static ExpenseClaims ForOrganization (Organization org)
        {
            return ForOrganization(org, false);
        }

        public static ExpenseClaims ForOrganization (Organization org, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray(SwarmDb.GetDatabaseForReading().GetExpenseClaims(org));
            }
            else
            {
                return FromArray(SwarmDb.GetDatabaseForReading().GetExpenseClaims(org, DatabaseCondition.OpenTrue));
            }
        }



        public decimal TotalAmount
        {
            get
            {
                return this.Sum(claim => claim.Amount);
            }
        }

        public Int64 TotalAmountCents
        {
            get
            {
                return this.Sum(claim => claim.AmountCents);
            }
        }
    }
}