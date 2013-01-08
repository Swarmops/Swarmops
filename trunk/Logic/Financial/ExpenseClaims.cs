using System;
using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Pirates;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class ExpenseClaims : PluralBase<ExpenseClaims,ExpenseClaim,BasicExpenseClaim>
    {
        public ExpenseClaims AllOpen
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

        public ExpenseClaims AllApproved
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

        public ExpenseClaims AllUnapproved
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

        public static ExpenseClaims FromClaimingPersonAndOrganization (Person person, Organization organization)
        {
            return
                FromArray(PirateDb.GetDatabaseForReading().GetExpenseClaimsByClaimerAndOrganization(person.Identity,
                                                                                          organization.Identity));
        }


        public static ExpenseClaims FromClaimingPerson (Person person)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetExpenseClaimsByClaimer(person.Identity));
        }

        public static ExpenseClaims FromOrganization (Organization org)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetExpenseClaimsByOrganization(org.Identity));
        }


        public static ExpenseClaims ForOrganization (Organization org)
        {
            return ForOrganization(org, false);
        }

        public static ExpenseClaims ForOrganization (Organization org, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray(PirateDb.GetDatabaseForReading().GetExpenseClaims(org));
            }
            else
            {
                return FromArray(PirateDb.GetDatabaseForReading().GetExpenseClaims(org, DatabaseCondition.OpenTrue));
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