using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Utility.BotCode;
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Special.Sweden;
using Activizr.Logic.Structure;

namespace Activizr.Utility.Special.Sweden
{
    public class SwedishForumMemberCheck
    {
        public static void Run()
        {
            BotLog.Write(1, "SeForumCheck", "Entering");

            Memberships memberships = Memberships.ForOrganization(Organization.PPSE);
            IForumDatabase forumDatabase = SwedishForumDatabase.GetDatabase();
            int[] accountIds;
            try
            {
                accountIds = forumDatabase.GetAccountList();
            }
            catch (Exception e)
            {
                ExceptionMail.Send(new Exception("Failed to connect to vBulletin",e),true);
                BotLog.Write(1, "SeForumCheck", "Failed to connect -- exiting");

                return;
            }

            BotLog.Write(1, "SeForumCheck", "Primed db - entering promotion cycle");

            Dictionary<int, bool> memberAccountLookup = new Dictionary<int, bool>();

            // This is kind of suboptimal, but hey, it only runs once a night in the wolf hour.

            Person currentMember = null;

            foreach (Membership membership in memberships)
            {
                if (!membership.Active)
                {
                    continue;
                }

                currentMember = membership.Person;

                try
                {
                    if (currentMember.SwedishForumAccountId != 0)
                    {
                        if (!forumDatabase.IsPartyMember(currentMember.SwedishForumAccountId))
                        {
                            // This guy is not listed as a party member, but should be.

                            BotLog.Write(2, "SeForumCheck", "Promoting " + currentMember.Name);
                            forumDatabase.SetPartyMember(currentMember.SwedishForumAccountId);
                        }

                        memberAccountLookup[currentMember.SwedishForumAccountId] = true;
                    }
                }
                catch (Exception e)
                {
                    // The forum account probably doesn't exist. Just remove it from the profile.

                    BotLog.Write(2, "SeForumCheck", "Exception reading " + currentMember.Name + ": " + e.ToString());

                    try
                    {
                        currentMember.SwedishForumAccountId = 0;
                    }
                    catch (Exception e2)
                    {
                        string logMessage="Exception removing " + currentMember.Name + "'s forum account: " + e2.ToString();
                        BotLog.Write(2, "SeForumCheck", logMessage);
                        ExceptionMail.Send(new Exception(logMessage,e2));
                    }
                }
            }

            // Now that we have flagged all member accounts as member accounts, flag the rest as
            // non-party members.

            BotLog.Write(1, "SeForumCheck", "Promotion cycle done - entering demotion cycle");

            foreach (int accountId in accountIds)
            {
                if (!memberAccountLookup.ContainsKey(accountId))
                {
                    if (forumDatabase.IsPartyMember(accountId))
                    {
                        BotLog.Write(2, "SeForumCheck", "Demoting forum account " + forumDatabase.GetAccountName(accountId));
                        forumDatabase.SetPartyNonmember(accountId);
                    }
                }
            }
            BotLog.Write(1, "SeForumCheck", "Demotion cycle complete -- exiting");
        }
    }
}