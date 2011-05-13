using System;
using System.Collections;
using System.Web.Mail;

using PirateWeb.Logic.Objects;
using PirateWeb.Database;

namespace PirateWeb
{
	/// <summary>
	/// Summary description for ForumHousekeeping.
	/// </summary>
	public class ForumHousekeeping
	{
		public ForumHousekeeping()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		

		public static void Run()
		{
			Database database = new Database (Globals.DatabaseConnectString);
			ForumDatabase forumDatabase = new ForumDatabase (Globals.ForumDatabaseConnectString);

			Hashtable memberAccounts = new Hashtable();
				
			Member[] members = database.GetAllMembers();

			foreach (Member member in members)
			{
				try
				{
					if (member.ForumAccountId != 0)
					{
						if (!forumDatabase.IsPartyMember (member.ForumAccountId))
						{
							// This guy is not listed as a party member, but should be.

							forumDatabase.SetPartyMember (member.ForumAccountId);
						}
						memberAccounts [member.ForumAccountId] = true;
					}
				}
				catch (Exception)
				{
					// The forum account probably doesn't exist. Just remove it from the profile.

					try
					{
						database.SetMemberForumAccountId (member.MemberId, 0);
					}
					catch (Exception e2)
					{
						ExceptionMail.Send (e2);
					}
				}
			}

			// Now that we have flagged all member accounts as member accounts, flag the rest as
			// non-party members.

			int[] accountIds = forumDatabase.GetAccountList();

			foreach (int accountId in accountIds)
			{
				if (!memberAccounts.ContainsKey (accountId))
				{
					if (forumDatabase.IsPartyMember (accountId))
					{
						forumDatabase.SetPartyNonmember (accountId);
					}
				}
			}
		}

	}
}
