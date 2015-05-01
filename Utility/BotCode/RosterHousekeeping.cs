using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Utility.BotCode
{
    public class RosterHousekeeping
    {
        // This function should basically not be called. Any time it is called means a security
        // risk.

        public static string ExportMemberList (int organizationIdRoot, DateTime membersAfter, DateTime membersBefore)
        {
            Organizations orgs = Organization.FromIdentity (organizationIdRoot).GetAllBelow();
            StringBuilder result = new StringBuilder();

            result.Append (
                String.Format ("All members joining tree of " + orgs[0].Name + " between " +
                               membersAfter.ToString ("yyyy-MMM-dd") + " and " + membersBefore.ToString ("yyyy-MMM-dd") +
                               "\r\n\r\n"));

            membersAfter = membersAfter.Date;
            membersBefore = membersBefore.AddDays (1).Date;

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
                            relevantPersonIds.Add (membership.PersonId);
                        }
                    }

                    People listMembers = People.FromIdentities (relevantPersonIds.ToArray());

                    List<string> csvResultList = new List<string>();

                    foreach (Person person in listMembers)
                    {
                        csvResultList.Add (
                            String.Format ("{0},{1},{2},{3:yyyy-MMM-dd},{4}",
                                person.Name, person.Street, person.PostalCodeAndCity, person.Birthdate,
                                person.IsMale ? "Male" : "Female"));
                    }

                    string[] csvResults = csvResultList.ToArray();
                    Array.Sort (csvResults);

                    orgSummary += String.Join ("\r\n", csvResults) + "\r\n\r\n";

                    result.Append (orgSummary);
                }
            }

            return result.ToString();
        }


        // This should run daily.

        public static void RemindAllExpiries()
        {
            throw new NotImplementedException();

            // needs generalization and localization.

            /*
            RemindExpiriesMail(); // Send on ALL days, but only if DayOfWeek == OrganizationId % 7. This scatters reminder mails.

            RemindExpiriesSms(DateTime.Now.AddDays(7).Date,
                              "Piratpartiet: Ditt medlemskap går ut om bara ett par dagar. Svara på detta SMS med texten \"PP IGEN\" för att förnya (5 kr).");
            RemindExpiriesSms(DateTime.Now.AddDays(1).Date,
                              "Piratpartiet: Ditt medlemskap går ut vid midnatt ikväll. Svara på detta SMS med texten \"PP IGEN\" för att förnya.");*/
        }


        [Obsolete ("Generalize and localize this function.", true)]
        public static void RemindExpiriesMail()
        {
            // Get expiring

            Console.WriteLine ("Inside RemindExpiriesMail()");

            Organizations orgs = Organizations.GetAll();

            Dictionary<int, bool> personLookup = new Dictionary<int, bool>();
            Dictionary<string, int> dateLookup = new Dictionary<string, int>();

            DateTime lowerBound = DateTime.Today;
            DateTime upperBound = lowerBound.AddDays (31);
            List<string> failedReminders = new List<string>();

            int weekDayInteger = (int) DateTime.Today.DayOfWeek;

            foreach (Organization org in orgs)
            {
                Memberships memberships = Memberships.GetExpiring (org, lowerBound, upperBound);

                foreach (Membership membership in memberships)
                {
                    if (membership.OrganizationId%7 != weekDayInteger)
                    {
                        continue;
                    }

                    try
                    {
                        Console.Write ("Reminding " + membership.Person.Canonical + " about " +
                                       membership.Organization.Name + ".");
                        SendReminderMail (membership);
                        Console.Write (".");
                        PWLog.Write (PWLogItem.Person, membership.PersonId,
                            PWLogAction.MembershipRenewReminder,
                            "Mail was sent to " + membership.Person.Mail +
                            " reminding to renew membership in " + membership.Organization.Name + ".", string.Empty);

                        Console.Write (".");

                        string dateString = membership.Expires.ToString ("yyyy-MM-dd");
                        if (!dateLookup.ContainsKey (dateString))
                        {
                            dateLookup[dateString] = 0;
                        }
                        dateLookup[dateString]++;

                        Console.WriteLine (" done.");
                    }
                    catch (Exception x)
                    {
                        string logText = "FAILED sending mail to " + membership.Person.Mail +
                                         " for reminder of pending renewal in " + membership.Organization.Name + ".";
                        failedReminders.Add (membership.Person.Canonical);
                        PWLog.Write (PWLogItem.Person, membership.PersonId,
                            PWLogAction.MembershipRenewReminder,
                            logText, string.Empty);
                        ExceptionMail.Send (new Exception (logText, x));
                    }
                }
            }

            string notifyBody = String.Format ("Sending renewal reminders to {0} people:\r\n\r\n", personLookup.Count);
            Console.WriteLine ("Sending renewal reminders to {0} people", personLookup.Count);

            List<string> dateSummary = new List<string>();
            int total = 0;
            foreach (string dateString in dateLookup.Keys)
            {
                dateSummary.Add (string.Format ("{0}: {1,5}", dateString, dateLookup[dateString]));
                total += dateLookup[dateString];
            }

            dateSummary.Sort();

            foreach (string dateString in dateSummary)
            {
                notifyBody += dateString + "\r\n";
                Console.WriteLine (dateString);
            }

            notifyBody += string.Format ("Total sent: {0,5}\r\n\r\n", total);
            Console.WriteLine ("Total sent: {0,5}\r\n\r\n", total);

            notifyBody += "FAILED reminders:\r\n";
            Console.WriteLine ("FAILED reminders:");

            foreach (string failed in failedReminders)
            {
                notifyBody += failed + "\r\n";
                Console.WriteLine (failed);
            }

            if (failedReminders.Count == 0)
            {
                notifyBody += "none.\r\n";
                Console.WriteLine ("none.");
            }

            /* no. just no. we should do a global search for "FromIdentity(1)"
            Person.FromIdentity(1).SendOfficerNotice("Reminders sent today", notifyBody, 1);  */
        }


        // This should run daily, suggested right after midnight.

        public static void ChurnExpiredMembers()
        {
            Organizations organizations = Organizations.GetAll();

            foreach (Organization organization in organizations)
            {
                Memberships memberships = Memberships.GetExpired (organization);
                // Mail each expiring member

                foreach (Membership membership in memberships)
                {
                    //only remove expired memberships
                    if (membership.Expires > DateTime.Now.Date)
                        continue;

                    Person person = membership.Person;

                    // TODO: Check for positions that expire with membership


                    // Mail

                    Memberships personMemberships = person.GetMemberships();
                    Memberships membershipsToDelete = new Memberships();
                    foreach (Membership personMembership in personMemberships)
                    {
                        if (personMembership.Expires <= DateTime.Now.Date)
                        {
                            membershipsToDelete.Add (personMembership);
                        }
                    }


                    ExpiredMail expiredmail = new ExpiredMail();
                    string membershipsIds = "";

                    if (membershipsToDelete.Count > 1)
                    {
                        foreach (Membership personMembership in membershipsToDelete)
                        {
                            membershipsIds += "," + personMembership.MembershipId;
                        }
                        membershipsIds = membershipsIds.Substring (1);
                        string expiredMemberships = "";
                        foreach (Membership personMembership in membershipsToDelete)
                        {
                            if (personMembership.OrganizationId != organization.Identity)
                            {
                                expiredMemberships += ", " + personMembership.Organization.Name;
                            }
                        }
                        expiredMemberships += ".  ";
                        expiredmail.pMemberships = expiredMemberships.Substring (2).Trim();
                    }

                    //TODO: URL for renewal, recieving end of this is NOT yet implemented...
                    // intended to recreate the memberships in MID
                    string tokenBase = person.PasswordHash + "-" + membership.Expires.Year;
                    string stdLink = "https://pirateweb.net/Pages/Public/SE/People/MemberRenew.aspx?MemberId=" +
                                     person.Identity +
                                     "&SecHash=" + SHA1.Hash (tokenBase).Replace (" ", "").Substring (0, 8) +
                                     "&MID=" + membershipsIds;

                    expiredmail.pStdRenewLink = stdLink;
                    expiredmail.pOrgName = organization.MailPrefixInherited;

                    person.SendNotice (expiredmail, organization.Identity);

                    person.DeleteSubscriptionData();

                    string orgIdString = string.Empty;

                    foreach (Membership personMembership in membershipsToDelete)
                    {
                        if (personMembership.Active)
                        {
                            orgIdString += " " + personMembership.OrganizationId;

                            personMembership.Terminate (EventSource.PirateBot, null, "Member churned in housekeeping.");
                        }
                    }
                }
            }
        }

        [Obsolete ("Generalize and localize this functionality.", true)]
        internal static void RemindExpiries (DateTime dateExpiry)
        {
            Organizations orgs = Organizations.GetAll();

            foreach (Organization org in orgs)
            {
                Memberships memberships = Memberships.GetExpiring (org, dateExpiry);

                // Mail each expiring member

                foreach (Membership membership in memberships)
                {
                    try
                    {
                        SendReminderMail (membership);
                        PWLog.Write (PWLogItem.Person, membership.PersonId,
                            PWLogAction.MembershipRenewReminder,
                            "Mail was sent to " + membership.Person.Mail +
                            " reminding to renew membership in " + membership.Organization.Name + ".", string.Empty);
                    }
                    catch (Exception ex)
                    {
                        ExceptionMail.Send (
                            new Exception ("Failed to create reminder mail for person " + membership.PersonId, ex));
                    }
                }
            }
        }

        [Obsolete ("Requires move to plugin or similar, or at least rearch into a hook", true)]
        internal static void RemindExpiriesSms (DateTime dateExpiry, string message)
        {
            throw new NotImplementedException();

            /*

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
            }*/
        }


        internal static bool SendReminderSms (Person person, string message)
        {
            bool result = false;

            try
            {
                person.SendPhoneMessage (message);
                result = true;
            }
                // ignore exceptions
            catch (Exception)
            {
            }

            return result;
        }

        [Obsolete ("Generalize and localize this function.", true)]
        public static void SendReminderMail (Membership membership)
        {
            /*
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
            }*/
        }

        public static void TimeoutVolunteers()
        {
            Volunteers volunteers = Volunteers.GetOpen();
            DateTime threshold = DateTime.Today.AddDays (-30);

            foreach (Volunteer volunteer in volunteers)
            {
                if (volunteer.OpenedDateTime < threshold)
                {
                    // timed out

                    /* -- wtf, Volunteer doesn't have an Org component? Well, will change anyway with Swarmops role structure

                    OfficerChain officers = OfficerChain.FromOrganizationAndGeography(volunteer.,
                                                                                      volunteer.Geography);

                    new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                                               "Volunteer Timed Out: [" + volunteer.Geography.Name + "]",
                                                               String.Empty, officers, true).Send(); */

                    volunteer.Close ("Timed out");
                }
            }
        }


        // Deleted a ton of special cases here, see file history if you need to copy code for reminding people to change org
    }
}