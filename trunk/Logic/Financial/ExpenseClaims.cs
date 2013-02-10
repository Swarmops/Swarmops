using System;
using System.Collections.Generic;
using System.Linq;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class ExpenseClaims : PluralBase<ExpenseClaims,ExpenseClaim,BasicExpenseClaim>
    {
        public ExpenseClaims WhereOpen
        {
            get
            {
                var result = new ExpenseClaims();
                foreach (ExpenseClaim expense in this)
                {
                    if (expense.Open)
                    {
                        result.Add (expense);
                    }
                }

                return result;
            }
        }

        public ExpenseClaims WhereApproved
        {
            get
            {
                var result = new ExpenseClaims();
                foreach (ExpenseClaim expense in this)
                {
                    if (expense.Approved)
                    {
                        result.Add (expense);
                    }
                }

                return result;
            }
        }

        public ExpenseClaims WhereUnapproved
        {
            get
            {
                var result = new ExpenseClaims();
                foreach (ExpenseClaim expense in this)
                {
                    if (!expense.Approved)
                    {
                        result.Add (expense);
                    }
                }

                return result;
            }
        }


        public ExpenseClaims WhereUnattested
        {
            get
            {
                ExpenseClaims result = new ExpenseClaims();
                result.AddRange(this.Where(expenseClaim => expenseClaim.Attested == false));

                return result;
            }
        }



        public static ExpenseClaims FromClaimingPersonAndOrganization (Person person, Organization organization)
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
                decimal result = 0.0m;

                foreach (ExpenseClaim claim in this)
                {
                    result += claim.Amount;
                }

                return result;
            }
        }

        public Int64 TotalAmountCents
        {
            get 
            { 
                Int64 result = 0;

                foreach (ExpenseClaim claim in this)
                {
                    result += claim.AmountCents;
                }

                return result;
            }
        }
    }
}