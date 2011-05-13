using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Enums;
using Activizr.Logic.Communications;
using Activizr.Utility.Mail;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

namespace Activizr.Utility.BotCode
{
    public class MailResolver
    {
        public static void Run ()
        {
            OutboundMail mail = OutboundMail.GetFirstUnresolved();

            if (mail == null)
            {
                return; // No unresolved -- nothing to do
            }

            // Use a dictionary to add people

            int downwardCount = 0;
            int upwardCount = 0;

            Dictionary<int, bool> officerIds = new Dictionary<int, bool>();
            Dictionary<int, bool> personIds = new Dictionary<int, bool>();

            // Add the affected people

            switch (mail.MailType)
            {
                case (int)TypedMailTemplate.TemplateType.MemberMail: 
                    // All members at this org and geography

                    People people = People.FromOrganizationAndGeography(mail.OrganizationId, mail.GeographyId);
                    downwardCount = people.Count;
                    foreach (Person person in people)
                    {
                        personIds[person.Identity] = true;
                    }
                    break;

                case (int)TypedMailTemplate.TemplateType.OfficerMail: 
                    // All officers at this org and geography
                    int[] officers = Roles.GetAllDownwardRoles(mail.OrganizationId, mail.GeographyId);
                    downwardCount = officers.Length;
                    foreach (int personId in officers)
                    {
                        officerIds[personId] = true;
                    }
                    break;

                default:
                    throw new InvalidOperationException("Unhandled mail mode; can't resolve mail id " +
                                                        mail.Identity.ToString());
            }

            int[] upwardIds = Roles.GetAllUpwardRoles(mail.OrganizationId, mail.GeographyId);
            upwardCount = upwardIds.Length;

            foreach (int personId in upwardIds)
            {
                // Filter for subscription of copies to local mail. Inefficient filter, room for optimization.

                if (Person.FromIdentity(personId).IsSubscribing(NewsletterFeed.TypeID.OfficerUpwardCopies))
                {
                    officerIds[personId] = true;
                }
            }

            // Assemble arrays

            List<int> officerIdList = new List<int>();
            foreach (int officerId in officerIds.Keys)
            {
                officerIdList.Add(officerId);
                if (personIds.ContainsKey(officerId))
                {
                    // If somebody is going to recieve this mail both due to area coverage and
                    // chain of command, send as chain of command only

                    personIds.Remove(officerId);
                }
            }

            List<int> personIdList = new List<int>();
            foreach (int personId in personIds.Keys)
            {
                personIdList.Add(personId);
            }

            // Check for people who have declined local mail

            Dictionary<int, bool> decliners = new Dictionary<int, bool>();

            //HACK: invitation to member meeting should be sent out to ALL members for PPSE
            if (mail.Author.PartyEmail.ToLower() != "motespresidiet@piratpartiet.se"
                || mail.GeographyId != Geography.RootIdentity
                || mail.OrganizationId != Organization.PPSEid)
            {
                decliners = Optimizations.GetPeopleWhoDeclineLocalMail(personIdList.ToArray());
            }

            int declineCount = decliners.Count;

            if (declineCount > 0)
            {
                foreach (int personId in decliners.Keys)
                {
                    personIds.Remove(personId);
                }

                // Rebuild list after decliners gone

                personIdList = new List<int>();
                foreach (int personId in personIds.Keys)
                {
                    personIdList.Add(personId);
                }
            }

            // TODO: Get queue count to give a time estimate until transmission


            // Create recipients

            mail.AddRecipients(personIdList.ToArray(), false);
            mail.AddRecipients(officerIdList.ToArray(), true);

            int countTotal = personIdList.Count + officerIdList.Count;

            // Mark as ready

            mail.SetRecipientCount(countTotal);
            mail.SetResolved();

            // TODO: Set recipient count of mail

            // Create a mail to the author

            string mailBody = "Your outbound mail with the title \"" + mail.Title +
                              "\" has been resolved for recipients and " +
                              "will be sent to " + countTotal.ToString("#,##0") + " people.\r\n\r\nOut of these " +
                              countTotal.ToString("#,##0") + ", " + (downwardCount - declineCount).ToString("#,##0") +
                              " are people within the requested " +
                              "organization/geography (" + Organization.FromIdentity(mail.OrganizationId).Name + "/" +
                              Geography.FromIdentity(mail.GeographyId).Name + "), and " +
                              upwardCount.ToString("#,##0") + " are people in the chain of command who are " +
                              "copied on the mail to know what's happening in the organization. There is normally some overlap between these two groups.\r\n\r\n";

            if (declineCount > 1)
            {
                mailBody += declineCount.ToString("#,##0") +
                            " people will not receive the message because they have declined local mail.\r\n\r\n";
            }
            else if (declineCount == 1)
            {
                mailBody += "One person will not receive the message because he or she has declined local mail.\r\n\r\n";
            }

            mailBody +=
                "Transmissions will begin " + (mail.ReleaseDateTime < DateTime.Now
                                                   ? "immediately"
                                                   :
                                                       "in " + (mail.ReleaseDateTime - DateTime.Now).Minutes.ToString() +
                                                       " minutes") + ".\r\n";

            new MailTransmitter(
                "PirateWeb", "noreply@pirateweb.net",
                "Mail resolved: " + mail.Title + " (" + countTotal.ToString() + " recipients)", mailBody,
                Person.FromIdentity(mail.AuthorPersonId), true).Send();
        }


        static private People ApplySubscription (People input, int feedId)
        {
            People output = new People();

            foreach (Person person in input)
            {
                if (person.IsSubscribing(feedId))
                {
                    output.Add(person);
                }
            }

            return output;
        }


        public static string CreateWelcomeMail (Person person, Organization organization)
        {
            // for this person, iterate over all applicable geographies and organizations

            Organizations orgLine = organization.GetLine();
            Geographies geoLine = person.Geography.GetLine();

            orgLine.Reverse(); // Start at the most local org

            Dictionary<int,bool> orgMailedLookup = new Dictionary<int, bool>();

            int delay = 0;
            string result = string.Empty;
            Random random = new Random();

            foreach (Organization org in orgLine)
            {
                foreach (Geography geo in geoLine) // but at the top geography
                {
                    AutoMail autoMail = AutoMail.FromTypeOrganizationAndGeography(AutoMailType.Welcome, org, geo);

                    if (autoMail == null)
                    {
                        continue;
                    }

                    Person lead = null;
                    string geoName = geo.Name;

                    try
                    {
                        lead = Roles.GetLocalLead(org, geo);
                        orgMailedLookup[org.Identity] = true; // Make sure that the chairman doesn't mail at a lower level
                    }
                    catch (ArgumentException)
                    {
                    }

                    if (lead == null && !orgMailedLookup.ContainsKey(org.Identity))
                    {
                        // If we get here, there is a mail template at the highest possible geo for this org, but no local lead.
                        // That's usually the case with board-centric organizations rather than executive-centric.
                        // Try to mail from chairman rather than the local lead.
                       
                        try
                        {
                            orgMailedLookup[org.Identity] = true;
                            lead = Roles.GetChairman(org);
                            geoName = "Chairman";
                        }
                        catch (ArgumentException)
                        {
                        }
                    }

                    if (lead == null)
                    {
                        // ok, give up if there isn't a chairman either or if we've already mailed this org from the chairman
                        continue;
                    }

                    // TODO: fetch lead from roles
                    WelcomeMail welcomemail = new WelcomeMail();

                    welcomemail.pOrgName = org.MailPrefixInherited;

                    welcomemail.pGeographyName = "";
                    if (geo.Identity != Geography.RootIdentity)
                    {
                        welcomemail.pGeographyName = geo.Name;
                    }

                    welcomemail.pBodyContent = autoMail.Body;
                    welcomemail.pSubject = autoMail.Title;
                    
                    OutboundMail newMail = welcomemail.CreateOutboundMail (lead, OutboundMail.PriorityNormal,
                                                                            org, geo, DateTime.Now.AddMinutes(delay));
                    newMail.AddRecipient(person.Identity, false);
                    newMail.SetRecipientCount(1);
                    newMail.SetResolved();
                    newMail.SetReadyForPickup();
                    
                    result += String.Format(" - {0}/{1} by {2} (",
                                            org.NameShort, geoName, lead.Canonical);
                    if (delay == 0)
                    {
                        result += "sent now";
                        delay += 37;
                    }
                    else
                    {
                        result += "sending at " + DateTime.Now.AddMinutes(delay).ToString("HH:mm");
                        delay += 31 + random.Next(52);
                    }
                    result += ")\r\n";
                }
            }

            if (result.Length < 4)
            {
                result = "none\r\n";
            }

            return result;
        }
    }
}