using System;
using System.Collections.Generic;
using System.Text;

using PirateWeb.BasicTypes;
using PirateWeb.BasicTypes.Enums;
using PirateWeb.Logic.Collections;
using PirateWeb.Logic.Objects;

namespace PirateWeb.Utility.BotCode
{
	public class EventProcessor
	{
		public static void Run()
		{
			BasicEvent[] events = PirateWeb.Logic.Collections.Events.GetTopUnprocessedEvents();

			foreach (BasicEvent newEvent in events)
			{
				switch (newEvent.EventType)
				{
					// This code is in serious need of refactoring.

					case EventType.AddedRole:
						ProcessAddedRole(newEvent);
						break;

					case EventType.DeletedRole:
						ProcessDeletedRole(newEvent);
						break;

					case EventType.AddedMember:
						ProcessAddedMember(newEvent);
						break;

					case EventType.AddedMembership:
						ProcessAddedMembership(newEvent);
						break;

					case EventType.ExtendedMembership:
						ProcessExtendedMembership(newEvent);
						break;

					case EventType.TerminatedMembership:
						ProcessTerminatedMembership(newEvent);
						break;

					case EventType.ReceivedMembershipPayment:
						ProcessReceivedPayment(newEvent);
						break;

					case EventType.ExpenseCreated:
						ProcessExpenseCreated(newEvent);
						break;

					case EventType.ExpenseChanged:
						ProcessExpenseChanged(newEvent);
						break;

					case EventType.ExpensesRepaidClosed:
						ProcessExpensesRepaidClosed(newEvent);
						break;

					default:
						throw new InvalidOperationException("Unknown EventType: " + newEvent.EventType);
				}

				PirateWeb.Logic.Collections.Events.SetEventProcessed(newEvent.EventId);
			}
		}



		internal static void ProcessAddedRole (BasicEvent newEvent)
		{
			// This function handles the case when a new role has been added to a point in the organization.

			Person victim = null;
			Person perpetrator = null;
			Organization organization = null;
			Geography geography = null;
			RoleType roleType = RoleType.Unknown;

			try
			{
				victim = Person.FromIdentity(newEvent.VictimPersonId);
				perpetrator = Person.FromIdentity(newEvent.PerpetratorPersonId);
				organization = Organization.FromIdentity(newEvent.OrganizationId);
				geography = Geography.FromIdentity(newEvent.GeographyId);
				roleType = (RoleType) newEvent.ParameterInt;
			}
			catch (Exception)
			{
				// if any of these fail, one of the necessary components (for example, the role) was already deleted.

				return;
			}

			int[] concernedPeopleId = PirateWeb.Logic.Collections.Roles.GetAllUpwardRoleHolders(newEvent.OrganizationId, newEvent.GeographyId);

			// TODO HERE: Filter to only get the interested people in this event

			People concernedPeople = PirateWeb.Logic.Collections.People.FromIdentities(concernedPeopleId);

			new PirateWeb.Utility.Mail.MailTransmitter("PirateWeb", "noreply@pirateweb.net",
				"New Role: [" + victim.Name + "] - [" + organization.NameShort + "], [" + geography.Name + "], [" + roleType.ToString() + "]",
				"A new role was assigned on PirateWeb within your area of authority.\r\n\r\n" +
				"Person:       " + victim.Name + "\r\n" + 
				"Organization: " + organization.Name + "\r\n" + 
				"Geography:    " + geography.Name + "\r\n" +
				"Role Name:    " + roleType.ToString() + "\r\n\r\n" +
				"This role was assigned by " + perpetrator.Name + ".\r\n", concernedPeople, true).
				Send();
		}



		internal static void ProcessDeletedRole(BasicEvent newEvent)
		{
			// This function handles the case when a new role has been deleted from a point in the organization.

			Person victim = null;
			Person perpetrator = null;
			Organization organization = null;
			Geography geography = null;
			RoleType roleType = RoleType.Unknown;

			try
			{
				victim = Person.FromIdentity(newEvent.VictimPersonId);
				perpetrator = Person.FromIdentity(newEvent.PerpetratorPersonId);
				organization = Organization.FromIdentity(newEvent.OrganizationId);
				geography = Geography.FromIdentity(newEvent.GeographyId);
				roleType = (RoleType)newEvent.ParameterInt;
			}
			catch (Exception)
			{
				// if any of these fail, one of the necessary components (for example, the role) was already deleted.

				return;
			}

			int[] concernedPeopleId = PirateWeb.Logic.Collections.Roles.GetAllUpwardRoleHolders(newEvent.OrganizationId, newEvent.GeographyId);

			// TODO HERE: Filter to only get the interested people in this event

			People concernedPeople = PirateWeb.Logic.Collections.People.FromIdentities(concernedPeopleId);

			new PirateWeb.Utility.Mail.MailTransmitter("PirateWeb", "noreply@pirateweb.net",
				"Deleted Role: [" + victim.Name + "] - [" + organization.NameShort + "], [" + geography.Name + "], [" + roleType.ToString() + "]",
				"A role was deleted on PirateWeb within your area of authority.\r\n\r\n" +
				"Person:       " + victim.Name + "\r\n" +
				"Organization: " + organization.Name + "\r\n" +
				"Geography:    " + geography.Name + "\r\n" +
				"Role Name:    " + roleType.ToString() + "\r\n\r\n" +
				"This role was deleted by " + perpetrator.Name + ".\r\n", concernedPeople, true).
				Send();


		}





		internal static void ProcessAddedMember (BasicEvent newEvent)
		{
			// This function handles the case when a new member has been added. Several things are hardcoded
			// for UP at this point.

			Person victim = Person.FromIdentity(newEvent.VictimPersonId);
			Person perpetrator = Person.FromIdentity(newEvent.PerpetratorPersonId);
			Organization organization = Organization.FromIdentity(newEvent.OrganizationId);
			Geography geography = Geography.FromIdentity(newEvent.GeographyId);

			int[] concernedPeopleId = PirateWeb.Logic.Collections.Roles.GetAllUpwardRoleHolders(newEvent.OrganizationId, newEvent.GeographyId);

			// TODO HERE: Filter to only get the interested people in this event

			string body =
				"A new member has appeared within your area of authority.\r\n\r\n" +
				"Person:       " + victim.Name + "\r\n" +
				"Organization: " + organization.Name + "\r\n" +
				"Geography:    " + geography.Name + "\r\n\r\n" +
				"Event source: " + newEvent.EventSource.ToString() + "\r\n\r\n";

			if (newEvent.EventSource == EventSource.PirateWeb)
			{
				body += "This member was added manually by " + perpetrator.Name + ".\r\n\r\n";
			}

			// Send welcoming mails

			string mailsSent = MailResolver.CreateWelcomeMail (victim, organization);

			body += "Welcoming automails sent:\r\n" + mailsSent + 
				"\r\nTo add an automatic welcome mail for your organization and geography, " +
				"go to PirateWeb, Communications, Triggered Automails, Automail type \"Welcome\".\r\n\r\n";

			// Add some hardcoded things for UP

			if (organization.Inherits (2))
			{
				int membersTotal = Organization.FromIdentity (2).GetTree().GetMemberCount();
				int membersHere = organization.GetMemberCount();

				body += "Member count for Ung Pirat SE: " + membersTotal.ToString("#,##0") + "\r\n" +
					"Member count for " + organization.Name + ": " + membersHere.ToString("#,##0") + "\r\n";
			}


			People concernedPeople = PirateWeb.Logic.Collections.People.FromIdentities(concernedPeopleId);

			new PirateWeb.Utility.Mail.MailTransmitter("PirateWeb", "noreply@pirateweb.net",
				"New Member: [" + victim.Name + "] - [" + organization.NameShort + "], [" + geography.Name + "]",
				body, concernedPeople, true).Send();

		}



		internal static void ProcessAddedMembership(BasicEvent newEvent)
		{
			// This function handles the case when a new membership has been added. Several things are hardcoded
			// for UP at this point.

			Person victim = Person.FromIdentity(newEvent.VictimPersonId);
			Person perpetrator = Person.FromIdentity(newEvent.PerpetratorPersonId);
			Organization organization = Organization.FromIdentity(newEvent.OrganizationId);
			Geography node = Geography.FromIdentity(newEvent.GeographyId);

			int[] concernedPeopleId = PirateWeb.Logic.Collections.Roles.GetAllUpwardRoleHolders(newEvent.OrganizationId, newEvent.GeographyId);

			// TODO HERE: Filter to only get the interested people in this event

			string body =
				"A new membership (for an existing member) has appeared within your area of authority.\r\n\r\n" +
				"Person:       " + victim.Name + "\r\n" +
				"Organization: " + organization.Name + "\r\n" +
				"Geography:    " + node.Name + "\r\n\r\n" +
				"Event source: " + newEvent.EventSource.ToString() + "\r\n\r\n";

			// Add some hardcoded things for UP

			if (organization.Inherits(2))
			{
				int membersTotal = Organization.FromIdentity(2).GetTree().GetMemberCount();
				int membersHere = organization.GetMemberCount();

				body += "Member count for Ung Pirat SE: " + membersTotal.ToString("#,##0") + "\r\n" +
					"Member count for " + organization.Name + ": " + membersHere.ToString("#,##0") + "\r\n";
			}


			People concernedPeople = PirateWeb.Logic.Collections.People.FromIdentities(concernedPeopleId);

			new PirateWeb.Utility.Mail.MailTransmitter("PirateWeb", "noreply@pirateweb.net",
				"New Membership: [" + victim.Name + "] - [" + organization.NameShort + "], [" + node.Name + "]",
				body, concernedPeople, true).Send();

			
		}



		internal static void ProcessExtendedMembership(BasicEvent newEvent)
		{
			// This function handles the case when a membership has been extended for a year.

			BasicPerson victim = Person.FromIdentity(newEvent.VictimPersonId);
			BasicPerson perpetrator = Person.FromIdentity(newEvent.PerpetratorPersonId);
			Organization organization = Organization.FromIdentity(newEvent.OrganizationId);
			Geography node = Geography.FromIdentity(newEvent.GeographyId);

			int[] concernedPeopleId = PirateWeb.Logic.Collections.Roles.GetAllUpwardRoleHolders(newEvent.OrganizationId, newEvent.GeographyId);

			// TODO HERE: Filter to only get the interested people in this event

			string body =
				"A membership was extended within your area of authority.\r\n\r\n" +
				"Person:       " + victim.Name + "\r\n" +
				"Organization: " + organization.Name + "\r\n" +
				"Geography:    " + node.Name + "\r\n\r\n" +
				"Event source: " + newEvent.EventSource.ToString() + "\r\n";

			People concernedPeople = PirateWeb.Logic.Collections.People.FromIdentities(concernedPeopleId);

			new PirateWeb.Utility.Mail.MailTransmitter("PirateWeb", "noreply@pirateweb.net",
				"Extended Membership: [" + victim.Name + "] - [" + organization.NameShort + "], [" + node.Name + "]",
				body, concernedPeople, true).Send();
		}



		internal static void ProcessTerminatedMembership(BasicEvent newEvent)
		{
			// This function handles the case when a membership has been terminated. Several things are hardcoded
			// for UP at this point.

			Person victim = Person.FromIdentity(newEvent.VictimPersonId);
			Person perpetrator = Person.FromIdentity(newEvent.PerpetratorPersonId);
			Organization organization = Organization.FromIdentity (newEvent.OrganizationId);
			Geography node = Geography.FromIdentity(newEvent.GeographyId);

			int[] concernedPeopleId = PirateWeb.Logic.Collections.Roles.GetAllUpwardRoleHolders(newEvent.OrganizationId, newEvent.GeographyId);

			// TODO HERE: Filter to only get the interested people in this event

			string body =
				"A membership was lost within your area of authority.\r\n\r\n" +
				"Person:       " + victim.Name + "\r\n" +
				"Organization: " + organization.Name + "\r\n" +
				"Geography:    " + node.Name + "\r\n\r\n" +
				"Event source: " + newEvent.EventSource.ToString() + "\r\n";

			// Add some hardcoded things for UP

			if (organization.Inherits(2))
			{
				int membersTotal = Organization.FromIdentity(2).GetTree().GetMemberCount();
				int membersHere = organization.GetMemberCount();

				body += "Member count for Ung Pirat SE: " + membersTotal.ToString("#,##0") + "\r\n" +
					"Member count for " + organization.Name + ": " + membersHere.ToString("#,##0") + "\r\n";
			}


			People concernedPeople = PirateWeb.Logic.Collections.People.FromIdentities(concernedPeopleId);

			new PirateWeb.Utility.Mail.MailTransmitter("PirateWeb", "noreply@pirateweb.net",
				"Lost Membership: [" + victim.Name + "] - [" + organization.NameShort + "], [" + node.Name + "]",
				body, concernedPeople, true).Send();
		}


		internal static void ProcessReceivedPayment(BasicEvent newEvent)
		{
			// Set membership expiry to one year from today's expiry

			// First, find this particular membership
			
			Membership renewedMembership = null;
			bool foundExisting = false;
			try
			{
				renewedMembership = Membership.FromBasic(Memberships.GetMembership(newEvent.VictimPersonId, newEvent.OrganizationId));
				foundExisting = true;
			}
			catch (ArgumentException) { }  // an exception here means there was no active membership

			if (foundExisting)
			{
				Person person = renewedMembership.Person;
				DateTime expires = renewedMembership.Expires;

				ChurnData.LogRetention(person.Identity, renewedMembership.OrganizationId, expires);
				Events.CreateEvent(EventSource.CustomServiceInterface, EventType.ExtendedMembership, person.Identity, renewedMembership.OrganizationId, person.GeographyId, person.Identity, 0, string.Empty);

				expires = expires.AddYears(1);


				// If the membership is in organization 1, then propagate the expiry to every other org this person is a member of

				// Cheat and assume Swedish. In the future, take this from a template.

				string mailBody =
					"Tack för att du har valt att förnya ditt medlemskap och fortsätta vara en del av piratrörelsen i " +
					"Sverige! Ditt medlemskap går nu ut " + expires.ToString("yyyy-MM-dd") + ", och gäller följande föreningar:\r\n\r\n";

				Memberships memberships = person.GetMemberships();

				foreach (Membership membership in memberships)
				{
					if (membership.Organization.Inherits(1) || membership.OrganizationId == 1)
					{
						membership.Expires = expires;
						mailBody += membership.Organization.Name + "\r\n";
					}
				}

				mailBody += "\r\nOm du har några frågor, så kontakta gärna Medlemsservice på medlemsservice@piratpartiet.se. Återigen, " +
					"välkommen att vara med i vårt fortsatta arbete!\r\n";

				new PirateWeb.Utility.Mail.MailTransmitter("Piratpartiet Medlemsservice", "medlemsservice@piratpartiet.se", "Piratpartiet: Ditt medlemskap är förnyat",
					mailBody, person).Send();
			}
			else
			{
				// This person's membership has expired, so he/she needs a new one.

				// TODO

			}
		}


		internal static void ProcessExpenseCreated (BasicEvent newEvent)
		{
			Person expenser = Person.FromIdentity(newEvent.VictimPersonId);
			Organization organization = Organization.FromIdentity(newEvent.OrganizationId);
			Geography geography = Geography.FromIdentity(newEvent.GeographyId);
			Expense expense = Expense.FromIdentity(newEvent.ParameterInt);

			int[] concernedPeopleId = PirateWeb.Logic.Collections.Roles.GetAllUpwardRoleHolders(newEvent.OrganizationId, newEvent.GeographyId);

			// TODO HERE: Filter to only get the interested people in this event

			string body =
				"An expense was filed for reimbursement within your area of authority.\r\n\r\n" +
				"Claimer:      " + expenser.Name + "\r\n" +
				"Organization: " + organization.Name + "\r\n" +
				"Geography:    " + geography.Name + "\r\n\r\n" +
				"Expense Date: " + expense.ExpenseDate.ToLongDateString() + "\r\n" +
				"Description:  " + expense.Description + "\r\n" +
				"Amount:       " + expense.Amount.ToString ("#,##0.00") + "\r\n\r\n" +
				"Event source: " + newEvent.EventSource.ToString() + "\r\n\r\n";

			Expenses orgExpenses = Expenses.FromOrganization(organization);

			decimal total = 0.0m;
			foreach (Expense localExpense in orgExpenses)
			{
				if (localExpense.Open)
				{
					total += localExpense.Amount;
				}
			}

			body += "The total outstanding expense claims for this organization currently amount to " + total.ToString("#,##0.00") + ".\r\n\r\n";

			People concernedPeople = PirateWeb.Logic.Collections.People.FromIdentities(concernedPeopleId);

			new PirateWeb.Utility.Mail.MailTransmitter("PirateWeb", "noreply@pirateweb.net",
				"New Expense Claim: [" + expenser.Name + "], [" + organization.NameShort + "], [" + expense.Amount.ToString ("#,##0.00") + "]",
				body, concernedPeople, true).Send();

		}


		internal static void ProcessExpenseChanged(BasicEvent newEvent)
		{
			Person expenser = Person.FromIdentity(newEvent.VictimPersonId);
			Organization organization = Organization.FromIdentity(newEvent.OrganizationId);
			Geography geography = Geography.FromIdentity(newEvent.GeographyId);
			Expense expense = Expense.FromIdentity(newEvent.ParameterInt);
			ExpenseEventType eventType = (ExpenseEventType) Enum.Parse(typeof(ExpenseEventType), newEvent.ParameterText);

			// HACK: This assumes eventType is ExpenseEventType.Approved.

			string body =
				"Receipts for an expense has been received by the treasurer and will be repaid shortly, typically in 5-10 days.\r\n\r\n" +
				"Expense #:     " + expense.Identity.ToString() + "\r\n" +
				"Expense Date:  " + expense.ExpenseDate.ToLongDateString() + "\r\n" +
				"Description:   " + expense.Description + "\r\n" +
				"Amount:        " + expense.Amount.ToString("#,##0.00") + "\r\n\r\n";

			new PirateWeb.Utility.Mail.MailTransmitter("PirateWeb", "noreply@pirateweb.net",
				"Expense Approved: " + expense.Description + ", " + expense.Amount.ToString("#,##0.00"),
				body, expense.Claimer, true).Send();

		}


		public static void ProcessExpensesRepaidClosed (BasicEvent newEvent)
		{
			Person expenser = Person.FromIdentity(newEvent.VictimPersonId);
			Person payer = Person.FromIdentity(newEvent.PerpetratorPersonId);

			// The ParameterText field is like a,b,c,d where a..d are expense IDs

			string[] idStrings = newEvent.ParameterText.Split(',');

			Expenses expenses = new Expenses();
			decimal totalAmount = 0.0m;

			foreach (string idString in idStrings)
			{
				Expense newExpense = Expense.FromIdentity(Int32.Parse(idString));
				totalAmount += newExpense.Amount;
				expenses.Add (newExpense);
			}

			string body = "The following expenses, totaling " + totalAmount.ToString ("#,##0.00") + ", have been repaid to your registered account, " + expenser.BankName + " " + expenser.BankAccount + ".\r\n\r\n";

			body += FormatExpenseLine ("#", "Description", "Amount");
			body += FormatExpenseLine ("===", "========================================", "=========");

			foreach (Expense expense in expenses)
			{
				body += FormatExpenseLine (expense.Identity.ToString(), expense.Description, expense.Amount.ToString ("#,##0.00"));
			}

			body += FormatExpenseLine ("---", "----------------------------------------", "---------");

			body += FormatExpenseLine (string.Empty, "TOTAL", totalAmount.ToString ("#,##0.00"));

			body += "\r\nIf you see any problems with this, please contact the treasurer, " + payer.Name + ", at " + payer.PPMailAddress + ". Thank you for helping the pirate movement succeed.\r\n";

			new PirateWeb.Utility.Mail.MailTransmitter("PirateWeb", "noreply@pirateweb.net",
				"Expenses Repaid: " + totalAmount.ToString("#,##0.00"), body, expenser, true).Send();
		}

		private static string FormatExpenseLine (string one, string two, string three)
		{
			if (two.Length > 40)
			{
				two = two.Substring(0, 40);
			}

			return String.Format ("{0,3}  {1,-40}  {2,9}\r\n", one, two, three);
		}

	}
}
