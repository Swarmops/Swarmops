using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Basic.Interfaces;


namespace Swarmops.Interface.Objects
{
	public class ExpensePayout: IHasIdentity
	{
		internal ExpensePayout(Person person, ExpenseClaims expenseClaims)
		{
			this.person = person;
			this.expenseClaims = expenseClaims;

			decimal total = 0.0m;

			foreach (ExpenseClaim expense in expenseClaims)
			{
				total += expense.Amount;
			}

			this.amount = total;
		}

		private readonly Person person;

		public Person Person
		{
			get { return person; }
		}

		private readonly decimal amount;

		public decimal Amount
		{
			get { return amount; }
		}

		private readonly ExpenseClaims expenseClaims;

		public ExpenseClaims ExpenseClaims
		{
			get { return expenseClaims; }
		}

		public static ExpensePayout FromOrganizationAndPerson (int organizationId, int personId)
		{
			ExpenseClaims allExpenseClaims = ExpenseClaims.FromOrganization(Organization.FromIdentity(organizationId));
			ExpenseClaims expenseClaims = new ExpenseClaims();

			// Aggregate per person

			foreach (ExpenseClaim expense in allExpenseClaims)
			{
				if (expense.ClaimingPersonId == personId)
				{
					if (expense.Approved && expense.Open)
					{
						// Ready for payout

						expenseClaims.Add(expense);
					}
				}
			}

			// Create ExpensePayout object

			return new ExpensePayout(Person.FromIdentity(personId), expenseClaims);
		}


		public string ReceiptIdentitiesString // this could be calculated on construction
		{
			get
			{
				List<string> identities = new List<string>();

				foreach (ExpenseClaim expense in expenseClaims)
				{
					identities.Add(expense.Identity.ToString());
				}

				return String.Join(",", identities.ToArray());
			}
		}

		public string ReceiptList  // this could be calculated on construction
		{
			get
			{
				List<string> spans = new List<string>();
				List<int> expenseIdList = new List<int>();

				foreach (ExpenseClaim expense in expenseClaims)
				{
					expenseIdList.Add(expense.Identity);
				}

				int[] expenseIds = expenseIdList.ToArray();
				Array.Sort(expenseIds);

				// iterate over the sorted array: group spans of ids together

				int spanStart = expenseIds[0];
				int spanLength = 0;
				int currentIndex = 1;

				while (currentIndex < expenseIds.Length)
				{
					if (spanStart + spanLength + 1 == expenseIds[currentIndex])
					{
						// the span continues

						spanLength++;
						currentIndex++;
					}
					else
					{
						// the span does NOT continue: add

						if (spanLength == 0)
						{
							spans.Add(spanStart.ToString());
						}
						else if (spanLength == 1) // really means 2, is zero based
						{
							spans.Add(spanStart.ToString());
							spans.Add((spanStart + 1).ToString());
						}
						else
						{
							spans.Add (String.Format ("{0}-{1}", spanStart, spanStart + spanLength));
						}

						spanStart = expenseIds[currentIndex];
						spanLength = 0;

						currentIndex++;

					}
				}

				// Add the last span

				if (spanLength == 0)
				{
					spans.Add(spanStart.ToString());
				}
				else if (spanLength == 1) // really means 2, is zero based
				{
					spans.Add(spanStart.ToString());
					spans.Add((spanStart + 1).ToString());
				}
				else if (spanLength > 1)
				{
					spans.Add (String.Format ("{0}-{1}", spanStart, spanStart + spanLength));
				}

				// Join spans together with ", " between them

				return String.Join (", ", spans.ToArray());

			}
		}


		#region IHasIdentity Members

		public int Identity
		{
			// We're borrowing the person's identity, as that is what we iterate across.

			get { return person.Identity; }
		}

		#endregion
	}
}
