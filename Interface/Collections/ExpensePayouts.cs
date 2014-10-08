using System.Collections.Generic;
using Swarmops.Interface.Objects;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Interface.Collections
{
	class ExpensePayouts: List<ExpensePayout>
	{
		private ExpensePayouts() // prevent public construction
		{
		}

		public static ExpensePayouts FromOrganization(Organization org)
		{
			return FromOrganization(org.Identity);
		}

		public static ExpensePayouts FromOrganization(int organizationId)
		{
			// This aggregates a list of all pending expense payouts per person,
			// and returns an ExpensePayouts object.

			// It uses a Dictionary<int, List<ExpenseClaim>> to build the data, iterating over
			// all expenses, where the int key is the person identity.

			ExpenseClaims allExpenseClaims = ExpenseClaims.FromOrganization(Organization.FromIdentity(organizationId));
			Dictionary<int, ExpenseClaims> aggregation = new Dictionary<int, ExpenseClaims>();
			ExpensePayouts result = new ExpensePayouts();

			// Aggregate per person

			foreach (ExpenseClaim expense in allExpenseClaims)
			{
				if (expense.Approved && expense.Open)
				{
					// Ready for payout

					if (!aggregation.ContainsKey(expense.ClaimingPersonId))
					{
						aggregation[expense.ClaimingPersonId] = new ExpenseClaims();
					}

					aggregation[expense.ClaimingPersonId].Add(expense);
				}
			}

			// Create ExpensePayout objects

			foreach (int personId in aggregation.Keys)
			{
				result.Add(new ExpensePayout(Person.FromIdentity(personId), aggregation[personId]));
			}

			return result;
		}

	}
}
