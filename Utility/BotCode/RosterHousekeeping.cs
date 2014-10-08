using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Swarmops.Basic;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;
using Swarmops.Utility.Mail;

namespace Swarmops.Utility.BotCode
{
    public class RosterHousekeeping
    {
        // This function should basically not be called. Any time it is called means a security
        // risk.

        public static string ExportMemberList (int organizationIdRoot, DateTime membersAfter, DateTime membersBefore)
        {
            Organizations orgs = Organization.FromIdentity(organizationIdRoot).GetTree();
            StringBuilder result = new StringBuilder();

            result.Append(
                String.Format("All members joining tree of " + orgs[0].Name + " between " +
                              membersAfter.ToString("yyyy-MMM-dd") + " and " + membersBefore.ToString("yyyy-MMM-dd") +
                              "\r\n\r\n"));

            membersAfter = membersAfter.Date;
            membersBefore = membersBefore.AddDays(1).Date;

            foreach (Organization org in orgs)
            {
                if (org.AcceptsMembers)
                {
                    Memberships memberships = org.GetMemberships();

                    string orgSummary = org.Name + "\r\n";

                    // Iterate over membership roster and filter by date

                    List<int> relevantPersonIds = new List<int>();
                    foreach (Membership membership in memberships)
                    {
                        if (membership.MemberSince > membersAfter && membership.MemberSince < membersBefore)
                        {
                            relevantPersonIds.Add(membership.PersonId);
                        }
                    }

                    People listMembers = People.FromIdentities(relevantPersonIds.ToArray());

                    List<string> csvResultList = new List<string>();

                    foreach (Person person in listMembers)
                    {
                        csvResultList.Add(
                            String.Format("{0},{1},{2},{3:yyyy-MMM-dd},{4}",
                                          person.Name, person.Street, person.PostalCodeAndCity, person.Birthdate,
                                          person.IsMale ? "Male" : "Female"));
                    }

                    string[] csvResults = csvResultList.ToArray();
                    Array.Sort(csvResults);

                    orgSummary += String.Join("\r\n", csvResults) + "\r\n\r\n";

                    result.Append(orgSummary);
                }
            }

            return result.ToString();
        }


        // This should run daily.

        public static void RemindAllExpiries ()
        {
            RemindExpiriesMail(); // Send on ALL days, but only if DayOfWeek == OrganizationId % 7. This scatters reminder mails.

            //RemindExpiriesSms(DateTime.Now.AddDays(7).Date,
            //                  "Piratpartiet: Ditt medlemskap går ut om bara ett par dagar. Svara på detta SMS med texten \"PP IGEN\" för att förnya (5 kr).");
            RemindExpiriesSms(DateTime.Now.AddDays(1).Date,
                              "Piratpartiet: Ditt medlemskap går ut vid midnatt ikväll. Svara på detta SMS med texten \"PP IGEN\" för att förnya.");
        }

        public static void RemindExpiriesMail ()
        {
            // Get expiring

            Console.WriteLine("Inside RemindExpiriesMail()");

            Organizations orgs = Organization.Root.GetTree();

            Dictionary<int, bool> personLookup = new Dictionary<int, bool>();
            Dictionary<string, int> dateLookup = new Dictionary<string, int>();

            DateTime lowerBound = DateTime.Today;
            DateTime upperBound = lowerBound.AddDays(31);
            List<string> failedReminders = new List<string>();

            int weekDayInteger = (int) DateTime.Today.DayOfWeek;

            foreach (Organization org in orgs)
            {
                Memberships memberships = Memberships.GetExpiring(org, lowerBound, upperBound);

                foreach (Membership membership in memberships)
                {
                    if (membership.OrganizationId % 7 != weekDayInteger)
                    {
                        continue;
                    }

                    try
                    {
                        Console.Write("Reminding " + membership.Person.Canonical + " about " +
                                      membership.Organization.Name + ".");
                        SendReminderMail(membership);
                        Console.Write(".");
                        PWLog.Write(PWLogItem.Person, membership.PersonId,
                                        PWLogAction.MembershipRenewReminder,
                                        "Mail was sent to " + membership.Person.Mail +
                                        " reminding to renew membership in " + membership.Organization.Name + ".", string.Empty);

                        Console.Write(".");

                        string dateString = membership.Expires.ToString("yyyy-MM-dd");
                        if (!dateLookup.ContainsKey(dateString))
                        {
                            dateLookup[dateString] = 0;
                        }
                        dateLookup[dateString]++;

                        Console.WriteLine(" done.");
                    }
                    catch (Exception x)
                    {
                        string logText = "FAILED sending mail to " + membership.Person.Mail +
                                        " for reminder of pending renewal in " + membership.Organization.Name + ".";
                        failedReminders.Add(membership.Person.Canonical);
                        PWLog.Write(PWLogItem.Person, membership.PersonId,
                                        PWLogAction.MembershipRenewReminder,
                                        logText, string.Empty);
                        ExceptionMail.Send(new Exception(logText, x));
                    }
                }
            }

            string notifyBody = String.Format("Sending renewal reminders to {0} people:\r\n\r\n", personLookup.Count);
            Console.WriteLine("Sending renewal reminders to {0} people", personLookup.Count);

            List<string> dateSummary = new List<string>();
            int total = 0;
            foreach (string dateString in dateLookup.Keys)
            {
                dateSummary.Add(string.Format("{0}: {1,5}", dateString, dateLookup[dateString]));
                total += dateLookup[dateString];
            }

            dateSummary.Sort();

            foreach (string dateString in dateSummary)
            {
                notifyBody += dateString + "\r\n";
                Console.WriteLine(dateString);
            }

            notifyBody += string.Format("Total sent: {0,5}\r\n\r\n", total);
            Console.WriteLine("Total sent: {0,5}\r\n\r\n", total);

            notifyBody += "FAILED reminders:\r\n";
            Console.WriteLine("FAILED reminders:");

            foreach (string failed in failedReminders)
            {
                notifyBody += failed + "\r\n";
                Console.WriteLine(failed);
            }

            if (failedReminders.Count == 0)
            {
                notifyBody += "none.\r\n";
                Console.WriteLine("none.");
            }

            Person.FromIdentity(1).SendOfficerNotice("Reminders sent today", notifyBody, 1);
        }


        // This should run daily, suggested right after midnight.

        public static void ChurnExpiredMembers ()
        {
            // For the time being, use org 1 as master.

            int[] organizationIds = new int[] { Organization.PPSEid };

            foreach (int organizationId in organizationIds)
            {
                Memberships memberships = Memberships.GetExpired(Organization.FromIdentity(organizationId));
                // Mail each expiring member

                foreach (Membership membership in memberships)
                {
                    //only remove expired memberships
                    if (membership.Expires > DateTime.Now.Date)
                        continue;

                    Person person = membership.Person;

                    // Remove all roles and responsibilities for this person in the org

                    Authority authority = person.GetAuthority();

                    foreach (BasicPersonRole basicRole in authority.LocalPersonRoles)
                    {
                        PersonRole personRole = PersonRole.FromBasic(basicRole);
                        if (personRole.OrganizationId == membership.OrganizationId)
                        {
                            PWEvents.CreateEvent(EventSource.PirateBot, EventType.DeletedRole, person.Identity,
                                               personRole.OrganizationId, personRole.GeographyId, person.Identity, (int)personRole.Type,
                                               string.Empty);
                            personRole.Delete();
                        }
                    }

                    // Mail

                    Memberships personMemberships = person.GetMemberships();
                    Memberships membershipsToDelete = new Memberships();
                    foreach (Membership personMembership in personMemberships)
                    {
                        if (personMembership.Expires <= DateTime.Now.Date)
                        {
                            membershipsToDelete.Add(personMembership);
                        }
                    }


                    ExpiredMail expiredmail = new ExpiredMail();
                    string membershipsIds = "";

                    if (membershipsToDelete.Count > 1)
                    {
                        foreach (Membership personMembership in membershipsToDelete)
                        {
                            membershipsIds += "," + personMembership.MembershipId.ToString();
                        }
                        membershipsIds = membershipsIds.Substring(1);
                        string expiredMemberships = "";
                        foreach (Membership personMembership in membershipsToDelete)
                        {
                            if (personMembership.OrganizationId != organizationId)
                            {
                                expiredMemberships += ", " + personMembership.Organization.Name;
                            }

                        }
                        expiredMemberships += ".  ";
                        expiredmail.pMemberships = expiredMemberships.Substring(2).Trim();
                    }

                    //TODO: URL for renewal, recieving end of this is NOT yet implemented...
                    // intended to recreate the memberships in MID
                    string tokenBase = person.PasswordHash + "-" + membership.Expires.Year.ToString();
                    string stdLink = "https://pirateweb.net/Pages/Public/SE/People/MemberRenew.aspx?MemberId=" +
                                     person.Identity.ToString() +
                                     "&SecHash=" + SHA1.Hash(tokenBase).Replace(" ", "").Substring(0, 8) +
                                     "&MID=" + membershipsIds;

                    expiredmail.pStdRenewLink = stdLink;
                    expiredmail.pOrgName = Organization.FromIdentity(organizationId).MailPrefixInherited;

                    person.SendNotice(expiredmail, organizationId);

                    person.DeleteSubscriptionData();

                    string orgIdString = string.Empty;

                    foreach (Membership personMembership in membershipsToDelete)
                    {
                        if (personMembership.Active)
                        {
                            orgIdString += " " + personMembership.OrganizationId;

                            personMembership.Terminate(EventSource.PirateBot, null, "Member churned in housekeeping.");
                        }
                    }
                }
            }
        }


        internal static void RemindExpiries (DateTime dateExpiry)
        {
            Organizations orgs = Organization.Root.GetTree();

            foreach (Organization org in orgs)
            {
                Memberships memberships = Memberships.GetExpiring(org, dateExpiry);

                // Mail each expiring member

                foreach (Membership membership in memberships)
                {
                    try
                    {
                        SendReminderMail(membership);
                        PWLog.Write(PWLogItem.Person, membership.PersonId,
                                        PWLogAction.MembershipRenewReminder,
                                        "Mail was sent to " + membership.Person.Mail +
                                        " reminding to renew membership in " + membership.Organization.Name + ".", string.Empty);
                    }
                    catch (Exception ex)
                    {
                        ExceptionMail.Send(new Exception("Failed to create reminder mail for person " + membership.PersonId, ex));
                    }
                }
            }
        }

        internal static void RemindExpiriesSms (DateTime dateExpiry, string message)
        {
            // For the time being, only remind for org 1.

            int[] organizationIds = new int[] { Organization.PPSEid };

            foreach (int organizationId in organizationIds)
            {
                Memberships memberships = Memberships.GetExpiring(Organization.FromIdentity(organizationId), dateExpiry);

                // Mail each expiring member

                foreach (Membership membership in memberships)
                {
                    Person person = membership.Person;

                    if (People.FromPhoneNumber("SE", person.Phone).Count == 1)
                    {
                        if (SendReminderSms(person, message))
                        {
                            PWLog.Write(PWLogItem.Person, membership.PersonId,
                                            PWLogAction.MembershipRenewReminder,
                                            "SMS was sent to " + membership.Person.Phone +
                                            " reminding to renew membership.", string.Empty);
                        }
                        else
                        {
                            PWLog.Write(PWLogItem.Person, membership.PersonId,
                                            PWLogAction.MembershipRenewReminder,
                                            "Unable to send SMS to " + membership.Person.Phone +
                                            "; tried to remind about renewing membership.", string.Empty);
                        }
                    }
                }
            }
        }


        internal static bool SendReminderSms (Person person, string message)
        {
            bool result = false;

            try
            {
                person.SendPhoneMessage(message);
                result = true;
            }
            // ignore exceptions
            catch (Exception)
            {
            }

            return result;
        }

        public static void SendReminderMail (Membership membership)
        {
            // First, determine the organization template to use. Prioritize a long ancestry.

            // This is a hack for the Swedish structure.

            ReminderMail remindermail = new ReminderMail();

            // NEW December 2010: Organizations are separated as per common agreement, there are no common reminder mails. Every membership renews on its own.

            Organization lowOrg = membership.Organization;
            DateTime currentExpiry = membership.Expires;
            Person person = membership.Person;


            DateTime newExpiry = currentExpiry;

            //do not mess with lifetime memberships (100 years)
            if (newExpiry < DateTime.Today.AddYears(10))
            {
                newExpiry = newExpiry.AddYears(1);
            }

            remindermail.pPreamble = "<p> Nu är det dags att <strong>förnya ditt medlemskap</strong> i " + membership.Organization.Name + ".";

            remindermail.pExpirationDate = currentExpiry;
            remindermail.pNextDate = newExpiry;
            remindermail.pOrgName = membership.Organization.MailPrefixInherited;


            string tokenBase = person.PasswordHash + "-" + membership.Identity.ToString() + "-" + currentExpiry.Year.ToString();

            // REMOVED DEC2010: suggestion that people older than 25 may leave UP, as renewal mails are separated

            // HACK for UPSE:

            Organization expectedLowOrg = Organizations.GetMostLocalOrganization(person.GeographyId, Organization.UPSEid);

            if (expectedLowOrg != null && lowOrg.Inherits(Organization.UPSEid) && lowOrg.Identity != expectedLowOrg.Identity)
            {
                // Is this person in the wrong locale?

                remindermail.pCurrentOrg = lowOrg.Name;
                remindermail.pOtherOrg = expectedLowOrg.Name;
                remindermail.pGeographyName = person.Geography.Name;
                //mailBody += "Du är medlem i " + lowOrg.Name + ", men när du bor i [b]" + person.Geography.Name +
                //             "[/b] så rekommenderar " +
                //             "vi att du byter till din lokala organisation, [b]" + expectedLowOrg.Name +
                //             "[/b]. Klicka här för att göra det:\r\n\r\n";

                string link = "https://pirateweb.net/Pages/Public/SE/People/MemberRenew.aspx?PersonId=" +
                                person.Identity.ToString() + "&Transfer=" + lowOrg.Identity.ToString() + "," +
                                expectedLowOrg.Identity.ToString() +
                                "&MembershipId=" + membership.Identity.ToString() +
                                "&SecHash=" + SHA1.Hash(tokenBase + "-Transfer" + lowOrg.Identity.ToString() + "/" +
                                                      expectedLowOrg.Identity.ToString()).Replace(" ", "").Substring(0, 8);
                remindermail.pOtherRenewLink = link;
                remindermail.pTooOldForYouthOrgSpan = " "; //clear the other span

                //mailBody += "[a href=\"" + link + "\"]" + link + "[/a]\r\n\r\n" +
                //            "Det är naturligtvis inget krav, utan du kan fortsätta precis som förut om du vill. " +
                //            "För att fortsätta i dina befintliga föreningar, klicka här:\r\n\r\n";
            }

            else
            {
                remindermail.pTooOldForYouthOrgSpan = " "; //clear the other span
                remindermail.pWrongOrgSpan = " "; //clear the other span
                //mailBody += "Klicka på den här länken för att förnya för ett år till:\r\n\r\n";
            }

            string stdLink = "https://pirateweb.net/Pages/Public/SE/People/MemberRenew.aspx?PersonId=" +
                             person.Identity.ToString() +
                             "&MembershipId=" + membership.Identity.ToString() +
                             "&SecHash=" + SHA1.Hash(tokenBase).Replace(" ", "").Substring(0, 8);

            remindermail.pStdRenewLink = stdLink;
            tokenBase = person.PasswordHash + "-" + membership.Identity.ToString();
            string terminateLink = "https://pirateweb.net/Pages/Public/SE/People/MemberTerminate.aspx?MemberId=" +
                             person.Identity.ToString() +
                             "&SecHash=" + SHA1.Hash(tokenBase).Replace(" ", "").Substring(0, 8) +
                             "&MID=" + membership.Identity.ToString();

            remindermail.pTerminateLink = terminateLink;

            //mailBody += "[a href=\"" + stdLink + "\"]" + stdLink + "[/a]\r\n\r\n" +
            //            "[br]Välkommen att vara med oss i [b]ett år till![/b]\r\n\r\n" +
            //            "Hälsningar,[br]Medlemsservice\r\n\r\n"; // +
            /*"PS: [b]Du fick ett likadant mail alldeles nyss. Om du har å, ä eller ö i ditt namn fungerade " +
                "inte länken i det mailet. Tack till alla som hörde av sig om det; felet är fixat nu och länken ovan ska fungera.[/b]\r\n\r\n";*/

            //OutboundMail mail = remindermail.CreateOutboundMail(sender, OutboundMail.PriorityNormal, topOrg, Geography.Root);
            OutboundMail mail = remindermail.CreateFunctionalOutboundMail(MailAuthorType.MemberService, OutboundMail.PriorityNormal, membership.Organization, Geography.Root);
            if (mail.Body.Trim() == "")
            {
                throw new InvalidOperationException("Failed to create a mailBody");
            }
            else
            {
                mail.AddRecipient(person.Identity, false);
                mail.SetRecipientCount(2);
                mail.SetResolved();
                mail.SetReadyForPickup();
            }
        }

        public static void TimeoutVolunteers ()
        {
            Volunteers volunteers = Volunteers.GetOpen();
            DateTime threshold = DateTime.Today.AddDays(-30);

            foreach (Volunteer volunteer in volunteers)
            {
                if (volunteer.OpenedDateTime < threshold)
                {
                    // timed out

                    OfficerChain officers = OfficerChain.FromOrganizationAndGeography(Organization.PPSE,
                                                                                      volunteer.Geography);

                    new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                                               "Volunteer Timed Out: [" + volunteer.Geography.Name + "]",
                                                               String.Empty, officers, true).Send();

                    volunteer.Close("Timed out");
                }
            }
        }


        /*
         * Handle UP special: Monthly reminder to members that are in wrong  organisation
         * (member in an org that no longer accepts members) or member of an org where a "better fit" exists geographically
         * 
         */
        private class ChangeOrgReport
        {
            public Organization FromOrg = null;
            public Organization ToOrg = null;
        }

        public static void RemindChangeOrg ()
        {
            HeartBeater.Instance.Beat();

            // Get expiring
            DateTime starttime = DateTime.Now;
            Organizations orgs = Organization.FromIdentity(Organization.UPSEid).GetTree();
            Dictionary<int, Person> personLookup = new Dictionary<int, Person>();
            Dictionary<int, Memberships> personMembershipsLookup = new Dictionary<int, Memberships>();
            Dictionary<int, Organization> orgLookup = new Dictionary<int, Organization>();
            Dictionary<int, Organization> mostLocalOrganizationCache = new Dictionary<int, Organization>();
            List<ChangeOrgReport> report = new List<ChangeOrgReport>();

            foreach (Organization org in orgs)
            {
                orgLookup[org.OrganizationId] = org;
            }

            Memberships allMemberships = Memberships.ForOrganizations(orgs);
            foreach (Membership ms in allMemberships)
            {
                //Handle defunct Memberships.ForOrganizations
                if (orgLookup.ContainsKey(ms.OrganizationId))
                {
                    if (!personLookup.ContainsKey(ms.PersonId))
                    {
                        personLookup[ms.PersonId] = null;
                        personMembershipsLookup[ms.PersonId] = new Memberships();

                    }
                    personMembershipsLookup[ms.PersonId].Add(ms);
                }
            }
            allMemberships = null;

            People peeps = People.FromIdentities((new List<int>(personLookup.Keys)).ToArray());
            foreach (Person p in peeps)
            {
                personLookup[p.PersonId] = p;
            }


            if (Debugger.IsAttached)
                Console.WriteLine("Found " + personLookup.Count + " people");

            int processedCounter = 0;
            int sentCounter = 0;
            int failCounter = 0;

            DateTime lastDisplay = DateTime.Now;

            foreach (Person person in personLookup.Values)
            {
                ++processedCounter;
                if ((processedCounter % 50) == 0)
                {
                    HeartBeater.Instance.Beat();
                    if (Debugger.IsAttached)
                        Console.WriteLine("Processed " + processedCounter + " t=" + DateTime.Now.Subtract(lastDisplay).TotalSeconds);
                    lastDisplay = DateTime.Now;
                }

                int geoid = person.GeographyId;

                //check for error, geography broken.
                if (person.GeographyId == 0)
                {
                    geoid = person.Geography.GeographyId; //Will force resolve Geography
                    if (geoid != 0)
                        person.Geography = person.Geography; ; //repair person.
                }

                if (geoid == 0)
                    continue; //give up on that...

                Organization expectedLowOrg = null;

                if (mostLocalOrganizationCache.ContainsKey(geoid))
                {
                    expectedLowOrg = mostLocalOrganizationCache[geoid];
                }
                else
                {
                    expectedLowOrg = Organizations.GetMostLocalOrganization(geoid, Organization.UPSEid);
                    mostLocalOrganizationCache[geoid] = expectedLowOrg;
                }

                bool found = false;
                Dictionary<int, Membership> personMS = new Dictionary<int, Membership>();

                foreach (Membership ms in personMembershipsLookup[person.PersonId])
                {
                    if (orgLookup.ContainsKey(ms.OrganizationId))
                    {   //Its an UP org
                        personMS[ms.OrganizationId] = ms;
                    }
                    if (ms.OrganizationId == expectedLowOrg.Identity)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    //OK we didnt find the recommended org. Find out why.

                    // loop thru the persons up orgs to find an inactive
                    List<Organization> foundInactiveOrg = new List<Organization>();
                    List<Organization> foundActiveOrg = new List<Organization>();
                    Membership membership = null;
                    try
                    {
                        Organization org = null;

                        foreach (Membership ms in personMS.Values)
                        {
                            org = orgLookup[ms.OrganizationId];
                            if (org.AcceptsMembers == false)
                            {
                                foundInactiveOrg.Add(org);
                            }
                            else if (org.AutoAssignNewMembers == true)
                            {
                                foundActiveOrg.Add(org);
                            }
                        }

                        if (foundInactiveOrg.Count > 0)
                        {
                            //inactive
                            membership = personMS[foundInactiveOrg[0].Identity];

                        }
                        else if (foundActiveOrg.Count > 0)
                        {
                            //change
                            membership = personMS[foundActiveOrg[0].Identity];

                        }
                        else
                        {
                            //already a member but not of an autoassign org
                            if (Debugger.IsAttached)
                                Console.WriteLine("Debug:nochange " + person.Name + ";" + person.Geography.Name + ";" + (org != null ? org.Name : "UnknownOrg") + ";" + expectedLowOrg.Name);
                            continue;
                        }
                        DateTime lastReminder = PWLog.CheckLatest(PWLogItem.Person, membership.PersonId, PWLogAction.MembershipRenewReminder);

                        if (DateTime.Now.Subtract(lastReminder).TotalDays > 25)
                        {
                            if (Debugger.IsAttached)
                            {
                                Console.Write("Debug:" + person.Name + ";" + person.Geography.Name + ";" + membership.Organization.Name + ";" + expectedLowOrg.Name);
                                foreach (var o in foundActiveOrg)
                                    Console.Write(";" + o.Name);
                                Console.WriteLine("");
                            }

                            SendChangeOrgMail(person, membership, expectedLowOrg);
                            report.Add(new ChangeOrgReport { FromOrg = membership.Organization, ToOrg = expectedLowOrg });
                            ++sentCounter;
                            PWLog.Write(PWLogItem.Person, membership.PersonId,
                                            PWLogAction.MembershipRenewReminder,
                                            "Mail was sent to " + membership.Person.Mail +
                                                " for recommendation of organisation change in " + membership.Organization.Name + ".",
                                            membership.Organization.Identity.ToString() + "/" + expectedLowOrg.Identity.ToString());
                        }
                    }
                    catch (Exception x)
                    {
                        ++failCounter;
                        string logText = "FAILED sending mail to " + membership.Person.Mail +
                                        " for recommendation of organisation change in " + membership.Organization.Name + ".";
                        PWLog.Write(PWLogItem.Person, membership.PersonId,
                                        PWLogAction.MembershipRenewReminder,
                                        logText, string.Empty);
                        ExceptionMail.Send(new Exception(logText, x));
                    }
                }
            }

            Dictionary<Organization, Dictionary<Organization, int>> fromdict = new Dictionary<Organization, Dictionary<Organization, int>>();
            StringBuilder fromOrgReport = new StringBuilder();
            report.ForEach(delegate(ChangeOrgReport r)
                {
                    if (!fromdict.ContainsKey(r.FromOrg))
                        fromdict[r.FromOrg] = new Dictionary<Organization, int>();
                    if (!fromdict[r.FromOrg].ContainsKey(r.ToOrg))
                        fromdict[r.FromOrg][r.ToOrg] = 0;
                    fromdict[r.FromOrg][r.ToOrg]++;
                });
            foreach (var fd in fromdict.Keys)
            {
                StringBuilder tmp = new StringBuilder();
                int cnt = 0;
                foreach (var td in fromdict[fd].Keys)
                {
                    tmp.Append(", " + td.Name);
                    cnt += fromdict[fd][td];
                }
                fromOrgReport.Append("\r\nFrån " + fd.Name + " (" + cnt + " st) till " + tmp.ToString().Substring(2));
            }

            Dictionary<Organization, Dictionary<Organization, int>> todict = new Dictionary<Organization, Dictionary<Organization, int>>();
            StringBuilder toOrgReport = new StringBuilder();
            report.ForEach(delegate(ChangeOrgReport r)
             {
                 if (!todict.ContainsKey(r.ToOrg))
                     todict[r.ToOrg] = new Dictionary<Organization, int>();
                 if (!todict[r.ToOrg].ContainsKey(r.FromOrg))
                     todict[r.ToOrg][r.FromOrg] = 0;
                 todict[r.ToOrg][r.FromOrg]++;
             });
            foreach (var td in todict.Keys)
            {
                StringBuilder tmp = new StringBuilder();
                int cnt = 0;
                foreach (var fd in todict[td].Keys)
                {
                    tmp.Append(", " + fd.Name);
                    cnt += todict[td][fd];
                }
                toOrgReport.Append("\r\nTill " + td.Name + " (" + cnt + " st) från " + tmp.ToString().Substring(2));
            }


            string reportMessage = string.Format(
                        "Result from running recommendation to change org mails:\r\n"
                       + "Time:    {0,10:#0.0} minutes. \r\n"
                       + "Checked: {1,10:g}\r\n"
                       + "Sent:    {2,10:g}\r\n"
                       + "Failed:  {3,10:g}\r\n",
                    DateTime.Now.Subtract(starttime).TotalMinutes, processedCounter, sentCounter, failCounter)
                 + fromOrgReport + "\r\n"
                 + toOrgReport;

            BasicPersonRole[] UPSecretary = SwarmDb.GetDatabaseForReading().GetPeopleWithRoleType(RoleType.OrganizationSecretary,
                                                                                        new int[] { Organization.UPSEid },
                                                                                        new int[] { });
            if (UPSecretary.Length > 0)
            {
                Person.FromIdentity(UPSecretary[0].PersonId).SendOfficerNotice("ChangeOrg Mails Job report", reportMessage, Organization.UPSEid);
            }

            Person.FromIdentity(7838).SendOfficerNotice("ChangeOrgMails run", reportMessage, Organization.UPSEid);//Debug
        }

        public static void SendChangeOrgMail (Person person, Membership membership, Organization newOrg)
        {
            // HACK for UPSE
            // This is a hack for the Swedish structure.

            ChangeOrgMail changeorgmail = new ChangeOrgMail();

            Organization topOrg = Organization.FromIdentity(Organization.UPSEid);
            Organization lowOrg = membership.Organization;
            DateTime currentExpiry = membership.Expires;


            DateTime newExpiry = DateTime.Today.AddYears(1);


            changeorgmail.pCurrentOrg = lowOrg.Name;
            changeorgmail.pCurrentGeo = person.Geography.Name;
            changeorgmail.pNextDate = newExpiry;

            string tokenBase = person.PasswordHash + "-" + membership.Identity.ToString() + "-" + currentExpiry.Year.ToString();

            if (newOrg.AnchorGeographyId == Geography.RootIdentity && newOrg.AutoAssignNewMembers)
            {//Fallback org
                changeorgmail.pNoLocalOrg = newOrg.Name;
                changeorgmail.pChangeOrg = "";
            }
            else
            {
                changeorgmail.pNoLocalOrg = "";
                changeorgmail.pChangeOrg = newOrg.Name;
            }

            if (lowOrg.AcceptsMembers)
            {
                changeorgmail.pInactiveOrg = "";
                changeorgmail.pInactiveEnding = " ";
            }
            else
            {
                changeorgmail.pInactiveOrg = newOrg.Name;
            }



            changeorgmail.pStdRenewLink = "https://pirateweb.net/Pages/Public/SE/People/MemberRenew.aspx?PersonId=" +
                              person.Identity.ToString() + "&Transfer=" + lowOrg.Identity.ToString() + "," +
                              newOrg.Identity.ToString() +
                              "&MembershipId=" + membership.Identity.ToString() +
                              "&SecHash=" + SHA1.Hash(tokenBase + "-Transfer" + lowOrg.Identity.ToString() + "/" +
                                                      newOrg.Identity.ToString()).Replace(" ", "").Substring(0, 8);

            OutboundMail mail = changeorgmail.CreateFunctionalOutboundMail(MailAuthorType.MemberService, OutboundMail.PriorityNormal, topOrg, Geography.Root);
            string test = mail.RenderHtml(person, person.PreferredCulture);
            test = mail.RenderText(person, person.PreferredCulture);

            if (mail.Body.Trim() == "")
            {
                throw new InvalidOperationException("Failed to create a mailBody");
            }
            else
            {
                //mail.AddRecipient(7838, true);
                mail.AddRecipient(person.Identity, false);
                mail.SetRecipientCount(1);
                mail.SetResolved();
                mail.SetReadyForPickup();
            }
        }


    }
}