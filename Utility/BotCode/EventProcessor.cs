using System;
using System.IO;

namespace Swarmops.Utility.BotCode
{
    public class EventProcessor
    {
        // This entire class could use a tune-up. More event types will come, too.

        [Obsolete("Deprecated for Swarmops. Way too much hardcoded. Rewrite, generalize, or scrap.", true)]
        public static void Run()
        {
            /*
            BasicPWEvent[] events = PWEvents.GetTopUnprocessedEvents();

            foreach (BasicPWEvent newEvent in events)
            {
                try
                {
                    switch (newEvent.EventType)
                    {
                        // This code is in SERIOUS need of refactoring.
                        // Idea: use delegates and a Dictionary<>.

                        case EventType.AddedRole:
                            ProcessAddedRole(newEvent);
                            break;

                        case EventType.DeletedRole:
                            ProcessDeletedRole(newEvent);
                            break;

                        case EventType.AddedMember:
                            ProcessAddedMember(newEvent);
                            break;

                        case EventType.LostMember:
                            ProcessLostMember(newEvent);
                            break;

                        case EventType.AddedMembership:
                            ProcessAddedMembership(newEvent);
                            break;

                        case EventType.ExtendedMembership:
                            ProcessExtendedMembership(newEvent);
                            break;

                        case EventType.TransferredMembership:
                            ProcessTransferredMembership(newEvent);
                            break;

                        //case EventType.TerminatedMembership: //Obsolete
                        //   ProcessTerminatedMembership(newEvent);
                        //   break;

                        case EventType.ReceivedMembershipPayment:
                            ProcessReceivedPayment(newEvent);
                            break;

                        case EventType.PaperLetterReceived:
                            ProcessPaperLetterReceived(newEvent);
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

                        case EventType.EmailAccountRequested:
                            ProcessCreateEmailAccount(newEvent);
                            break;

                        case EventType.RefreshEmailAccount:
                            ProcessRefreshEmailAccount(newEvent);
                            break;

                        case EventType.NewActivist:
                            ProcessNewActivist(newEvent);
                            break;

                        case EventType.LostActivist:
                            ProcessLostActivist(newEvent);
                            break;

                        case EventType.NewVolunteer:
                            ProcessNewVolunteer(newEvent);
                            break;

                        case EventType.SalaryCreated:
                            ProcessSalaryCreated(newEvent);
                            break;

                        case EventType.LocalDonationReceived:
                            ProcessLocalDonationReceived(newEvent);
                            break;

                        case EventType.PhoneMessagesCreated:
                            ProcessPhoneMessagesCreated(newEvent);
                            break;

                        case EventType.InboundInvoiceReceived:
                            ProcessInboundInvoiceReceived(newEvent);
                            break;

                        case EventType.OutboundInvoiceCreated:
                            ProcessOutboundInvoiceCreated(newEvent);
                            break;

                        case EventType.CandidateDocumentationReceived:
                            ProcessCandidateDocumentationReceived(newEvent);
                            break;

                        case EventType.CryptoKeyRequested:
                            ProcessCryptoKeyRequested(newEvent);
                            break;

                        case EventType.OutboundInvoicePaid:
                            ProcessOutboundInvoicePaid(newEvent);
                            break;

                        case EventType.ActivistMailsCreated:
                            ProcessActivistMailsCreated(newEvent);
                            break;

                        case EventType.ParleyAttested:
                            ProcessParleyAttested(newEvent);
                            break;

                        case EventType.ParleyAttendeeCreated:
                            ProcessParleyAttendeeCreated(newEvent);
                            break;

                        case EventType.ParleyAttendeeConfirmed:
                            ProcessParleyAttendeeConfirmed(newEvent);
                            break;

                        case EventType.FinancialDataUploaded:
                            if (newEvent.ParameterInt == 1)
                            {
                                PaypalImporter.Run(Encoding.ASCII.GetString(Convert.FromBase64String(newEvent.ParameterText)), Organization.FromIdentity(newEvent.OrganizationId), Person.FromIdentity(newEvent.ActingPersonId));
                            }
                            else if (newEvent.ParameterInt == 2)
                            {
                                PaysonImporter.Run(Encoding.ASCII.GetString(Convert.FromBase64String(newEvent.ParameterText)), Organization.FromIdentity(newEvent.OrganizationId), Person.FromIdentity(newEvent.ActingPersonId));
                            }
                            break;

                        case EventType.RefundCreated:
                            ProcessRefundCreated(newEvent);
                            break;

                        case EventType.ParleyCancelled:
                            ProcessParleyCancelled(newEvent);
                            break;

                        case EventType.ParleyClosed:
                            ProcessParleyClosed(newEvent);
                            break;

                        case EventType.ActivismLogged:
                        case EventType.PayoutCreated:
                        case EventType.InboundInvoiceAttested:
                        case EventType.ExpenseAttested:
                        case EventType.ExpenseValidated:
                            break;

                        default:
                            throw new InvalidOperationException("Unknown EventType: " + newEvent.EventType);
                    }
                }
                catch (Exception ex)
                {

                    PWEvents.SetEventProcessed(newEvent.EventId);
                    ExceptionMail.Send(new Exception("Eventprocessor failed for EventType:" + newEvent.EventType + ", id=" + newEvent.EventId + ". Event was ignored.", ex));
                    // just return, this eventprocessing loop will be re-run every 10 seconds but there are others 
                    // in the outer loop that needs to be run that is run more seldom.
                    return;

                }
                PWEvents.SetEventProcessed(newEvent.EventId);
            }*/
        }


        internal static bool RunningOnLinux()
        {
            if (Path.DirectorySeparatorChar == '/')
            {
                return true;
            }
            return false;
        }

        /*
        internal static void ProcessAddedRole (BasicPWEvent newPwEvent)
        {
            // This function handles the case when a new role has been added to a point in the organization.

            Person victim = null;
            Person perpetrator = null;
            Organization organization = null;
            Geography geography = null;
            RoleType roleType = RoleType.Unknown;


            bool showName = true;
            try
            {
                victim = Person.FromIdentity(newPwEvent.AffectedPersonId);
                perpetrator = Person.FromIdentity(newPwEvent.ActingPersonId);
                organization = Organization.FromIdentity(newPwEvent.OrganizationId);
                geography = Geography.FromIdentity(newPwEvent.GeographyId);
                roleType = (RoleType)newPwEvent.ParameterInt;
                if (organization.ShowNamesInNotificationsInh == false)
                    showName = false;
            }
            catch (Exception)
            {
                // if any of these fail, one of the necessary components (for example, the role) was already deleted.

                return;
            }

            // Create a mail address
            if (organization.DefaultCountry.Code.ToUpper() != "FI")
            {
                PWEvents.CreateEvent(EventSource.PirateBot, EventType.EmailAccountRequested, newPwEvent.ActingPersonId, Organization.PPSEid,
                                   victim.GeographyId, victim.Identity, 0, string.Empty);
            }
            // Notify concerned people

            int[] concernedPeopleId = Roles.GetAllUpwardRoles(newPwEvent.OrganizationId, newPwEvent.GeographyId);

            // TODO HERE: Filter to only get the interested people in this event

            People concernedPeople = People.FromIdentities(concernedPeopleId);
            concernedPeople.Remove(victim);
            // I wonder if this works... very sceptical. What's the identity op? Object or comparison level?

            new MailTransmitter(Strings.MailSenderNameHumanResources, Strings.MailSenderAddress,
                                                       "New Role: [" + (showName ? victim.Name : victim.Initials) + "] - [" + organization.NameShort +
                                                       "], [" + geography.Name + "], [" + roleType.ToString() + "]",
                                                       "A new role was assigned on PirateWeb within your area of authority.\r\n\r\n" +
                                                       "Person:       " + (showName ? victim.Name : victim.Initials) + "\r\n" +
                                                       "Organization: " + organization.Name + "\r\n" +
                                                       "Geography:    " + geography.Name + "\r\n" +
                                                       "Role Name:    " + roleType.ToString() + "\r\n\r\n" +
                                                       "This role was assigned by " + (showName ? perpetrator.Name : perpetrator.Initials) + ".\r\n",
                                                       concernedPeople, true).
                Send();
        }


        internal static void ProcessDeletedRole (BasicPWEvent newPwEvent)
        {
            // This function handles the case when a new role has been deleted from a point in the organization.

            Person victim = null;
            Person perpetrator = null;
            Organization organization = null;
            Geography geography = null;
            RoleType roleType = RoleType.Unknown;

            bool showName = true;
            try
            {
                victim = Person.FromIdentity(newPwEvent.AffectedPersonId);
                perpetrator = Person.FromIdentity(newPwEvent.ActingPersonId);
                organization = Organization.FromIdentity(newPwEvent.OrganizationId);
                geography = Geography.FromIdentity(newPwEvent.GeographyId);
                roleType = (RoleType)newPwEvent.ParameterInt;
                if (organization.ShowNamesInNotificationsInh == false)
                    showName = false;

            }
            catch (Exception)
            {
                // if any of these fail, one of the necessary components (for example, the role) was already deleted.

                return;
            }

            int[] concernedPeopleId = Roles.GetAllUpwardRoles(newPwEvent.OrganizationId, newPwEvent.GeographyId);

            // TODO HERE: Filter to only get the interested people in this event

            People concernedPeople = People.FromIdentities(concernedPeopleId);
            concernedPeople.Remove(victim);
            // I wonder if this works... very sceptical. What's the identity op? Object or comparison level?

            new MailTransmitter(Strings.MailSenderNameHumanResources, Strings.MailSenderAddress,
                                                       "Deleted Role: [" + (showName ? victim.Name : victim.Initials) + "] - [" +
                                                       organization.NameShort + "], [" + geography.Name + "], [" +
                                                       roleType.ToString() + "]",
                                                       "A role was deleted on PirateWeb within your area of authority.\r\n\r\n" +
                                                       "Person:       " + (showName ? victim.Name : victim.Initials) + "\r\n" +
                                                       "Organization: " + organization.Name + "\r\n" +
                                                       "Geography:    " + geography.Name + "\r\n" +
                                                       "Role Name:    " + roleType.ToString() + "\r\n\r\n" +
                                                       "This role was deleted by " + (showName ? perpetrator.Name : perpetrator.Initials) + ".\r\n",
                                                       concernedPeople, true).
                Send();
        }


        internal static void ProcessAddedMember (BasicPWEvent newPwEvent)
        {
            // This function handles the case when a new member has been added. Several things are hardcoded
            // for UP at this point.

            Person victim = Person.FromIdentity(newPwEvent.AffectedPersonId);
            Person perpetrator = Person.FromIdentity(newPwEvent.ActingPersonId);
            Organizations orgs = new Organizations();
            Geography geography = Geography.FromIdentity(newPwEvent.GeographyId);
            string referrerString = string.Empty;

            if (newPwEvent.ParameterText.Contains(","))
            {
                string[] eventParts = newPwEvent.ParameterText.Split(',');

                string[] orgStrings = eventParts[0].Split(' ');

                foreach (string orgString in orgStrings)
                {
                    int orgId = 0;
                    if (int.TryParse(orgString, out orgId))
                    {
                        orgs.Add(Organization.FromIdentity(orgId));
                    }
                }

                if (eventParts.Length > 2)
                {
                    referrerString = eventParts[2];
                }
            }
            else
            {
                orgs.Add(Organization.FromIdentity(newPwEvent.OrganizationId));
            }

            People concernedPeople = new People();

            bool showName = true;

            foreach (Organization org in orgs)
            {
                concernedPeople =
                    concernedPeople.LogicalOr(
                        People.FromIdentities(Roles.GetAllUpwardRoles(org.Identity, newPwEvent.GeographyId)));

                //Only show name if all orgs allow it
                if (org.ShowNamesInNotificationsInh == false)
                    showName = false;
            }

            //Filter to only get the interested people in this event
            concernedPeople = ApplySubscription(concernedPeople, NewsletterFeed.TypeID.OfficerNewMembers);

            string body = "A new member has appeared within your area of authority.\r\n\r\n";

            if (showName)
            {
                body += "Person:       " + victim.Name + "\r\n";
            }

            foreach (Organization org in orgs)
            {
                body += "Organization: " + org.Name + "\r\n";
            }

            body +=
                "Geography:    " + geography.Name + "\r\n\r\n" +
                "Event source: " + newPwEvent.EventSource.ToString() + "\r\n";

            if (newPwEvent.EventSource == EventSource.SignupPage && referrerString.Length > 0)
            {
                body += "Referrer:     " + referrerString + "\r\n";
            }

            body += "\r\n";

            if (newPwEvent.EventSource == EventSource.PirateWeb)
            {
                body += "This member was added manually";
                body += " by " + perpetrator.Name + ".\r\n\r\n";
            }

            // Send welcoming mails

            string mailsSent = MailResolver.CreateWelcomeMail(victim, Organization.FromIdentity(newPwEvent.OrganizationId));
            // HACK - should be for all orgs

            body += "Welcoming automails sent:\r\n" + mailsSent +
                    "\r\nTo add an automatic welcome mail for your organization and geography, " +
                    "go to PirateWeb, Communications, Triggered Automails, Automail type \"Welcome\".\r\n\r\n";


            string orgSubjectLine = string.Empty;

            foreach (Organization org in orgs)
            {
                orgSubjectLine += ", " + org.NameShort;
            }

            orgSubjectLine = orgSubjectLine.Substring(2);

            new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                                       "New Member: " + (showName ? "[" + victim.Name + "] - " : "") + "[" + orgSubjectLine + "], [" +
                                                       geography.Name + "]",
                                                       body, concernedPeople, true).Send();
        }


        internal static void ProcessLostMember (BasicPWEvent newPwEvent)
        {
            // This function handles the case when a member was purged from the database. Several things are hardcoded
            // for UP at this point.

            Person victim = Person.FromIdentity(newPwEvent.AffectedPersonId);

            Organizations orgs = new Organizations();
            Geography geography = Geography.FromIdentity(newPwEvent.GeographyId);

            string[] orgStrings = newPwEvent.ParameterText.Split(' ');

            foreach (string orgString in orgStrings)
            {
                int orgId = Int32.Parse(orgString);
                orgs.Add(Organization.FromIdentity(orgId));
            }

            People concernedPeople = new People();

            bool showName = true;
            foreach (Organization org in orgs)
            {
                concernedPeople =
                    concernedPeople.LogicalOr(
                        People.FromIdentities(Roles.GetAllUpwardRoles(org.Identity, newPwEvent.GeographyId)));
                //Only show name if all orgs allow it
                if (org.ShowNamesInNotificationsInh == false)
                    showName = false;
            }

            concernedPeople = ApplySubscription(concernedPeople, NewsletterFeed.TypeID.OfficerNewMembers);

            string body = "A member was lost within your area of authority.\r\n\r\n";
            if (showName)
            {
                body += "Person:       " + victim.Name + "\r\n";
            }

            foreach (Organization org in orgs)
            {
                body += "Organization: " + org.Name + "\r\n";
            }

            body +=
                "Geography:    " + geography.Name + "\r\n\r\n" +
                "Event source: " + newPwEvent.EventSource.ToString() + "\r\n\r\n";

            string orgSubjectLine = string.Empty;

            foreach (Organization org in orgs)
            {
                orgSubjectLine += ", " + org.NameShort;
            }

            orgSubjectLine = orgSubjectLine.Substring(2);

            new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                                       "Member Lost: " + (showName ? "[" + victim.Name + "] - " : "") + "[" + orgSubjectLine +
                                                       "], [" + geography.Name + "]",
                                                       body, concernedPeople, true).Send();
        }


        internal static void ProcessAddedMembership (BasicPWEvent newPwEvent)
        {
            // This function handles the case when a new membership has been added. Several things are hardcoded
            // for UP at this point.

            Person victim = Person.FromIdentity(newPwEvent.AffectedPersonId);
            Person perpetrator = Person.FromIdentity(newPwEvent.ActingPersonId);
            Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);
            Geography node = Geography.FromIdentity(newPwEvent.GeographyId);
            bool showName = true;
            if (organization.ShowNamesInNotificationsInh == false)
                showName = false;


            int[] concernedPeopleId = Roles.GetAllUpwardRoles(newPwEvent.OrganizationId, newPwEvent.GeographyId);

            string body =
                "A new membership (for an existing member) has appeared within your area of authority.\r\n\r\n" +
                "Person:       " + (showName ? victim.Name : victim.Initials) + "\r\n" +
                "Organization: " + organization.Name + "\r\n" +
                "Geography:    " + node.Name + "\r\n\r\n" +
                "Event source: " + newPwEvent.EventSource.ToString() + "\r\n\r\n";

            // Add some hardcoded things for UP

            if (organization.Inherits(Organization.UPSEid))
            {
                int membersTotal = Organization.FromIdentity(Organization.UPSEid).GetTree().GetMemberCount();
                int membersHere = organization.GetMemberCount();

                body += "Member count for Ung Pirat SE: " + membersTotal.ToString("#,##0") + "\r\n" +
                        "Member count for " + organization.Name + ": " + membersHere.ToString("#,##0") + "\r\n";
            }


            People concernedPeople = People.FromIdentities(concernedPeopleId);

            concernedPeople = ApplySubscription(concernedPeople, NewsletterFeed.TypeID.OfficerNewMembers);

            new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                                       "New Membership: " + (showName ? "[" + victim.Name + "] - " : "") + "[" +
                                                       organization.NameShort + "], [" + node.Name + "]",
                                                       body, concernedPeople, true).Send();
        }


        internal static void ProcessExtendedMembership (BasicPWEvent newPwEvent)
        {
            // This function handles the case when a membership has been extended for a year.


            BasicPerson victim = Person.FromIdentity(newPwEvent.AffectedPersonId);
            BasicPerson perpetrator = Person.FromIdentity(newPwEvent.ActingPersonId);
            Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);
            Geography node = Geography.FromIdentity(newPwEvent.GeographyId);

            int[] concernedPeopleId = Roles.GetAllUpwardRoles(newPwEvent.OrganizationId, newPwEvent.GeographyId);

            Organizations orgs = Organizations.FromSingle(organization);

            bool showName = true;
            if (organization.ShowNamesInNotificationsInh == false)
                showName = false;

            if (newPwEvent.ParameterText.Contains(" "))
            {
                // 2nd-gen: several orgs can be passed

                orgs = new Organizations();

                string[] orgStrings = newPwEvent.ParameterText.Split(' ');
                foreach (string orgString in orgStrings)
                {
                    if ((!orgString.Contains(".")) && (!orgString.Contains("/"))) // IP address or SMS
                    {
                        if (orgString.Trim().Length > 0)
                        {
                            orgs.Add(Organization.FromIdentity(Int32.Parse(orgString)));
                        }
                    }
                }
            }

            string orgNames = string.Empty;

            string body =
                "A membership was extended within your area of authority.\r\n\r\n" +
                (showName ? "Person:       " + victim.Name + "\r\n" : "");

            foreach (Organization org in orgs)
            {
                body +=
                    "Organization: " + org.Name + "\r\n";

                orgNames += ", " + org.NameShort;
            }

            orgNames = orgNames.Substring(2);


            body +=
                "Geography:    " + node.Name + "\r\n\r\n" +
                "Event source: " + newPwEvent.EventSource.ToString() + "\r\n";

            People concernedPeople = People.FromIdentities(concernedPeopleId);

            concernedPeople = ApplySubscription(concernedPeople, NewsletterFeed.TypeID.OfficerNewMembers);

            new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                                       "Extended Membership: " + (showName ? "[" + victim.Name + "] - " : "") + "[" + orgNames +
                                                       "], [" + node.Name + "]",
                                                       body, concernedPeople, true).Send();
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


        internal static void ProcessTransferredMembership (BasicPWEvent newPwEvent)
        {
            // Moving from one org to another.

            Person victim = Person.FromIdentity(newPwEvent.AffectedPersonId);
            Person perpetrator = Person.FromIdentity(newPwEvent.ActingPersonId);
            Geography geo = Geography.FromIdentity(newPwEvent.GeographyId);

            string[] orgStrings = newPwEvent.ParameterText.Split(' ');
            string ip = orgStrings[0];
            string oldOrgString = orgStrings[1];
            string newOrgString = orgStrings[2];

            Organization oldOrganization = Organization.FromIdentity(Int32.Parse(oldOrgString));
            Organization newOrganization = Organization.FromIdentity(Int32.Parse(newOrgString));

            bool showName = true;
            if (newOrganization.ShowNamesInNotificationsInh == false)
                showName = false;


            int[] concernedPeopleId1 = Roles.GetAllUpwardRoles(oldOrganization.Identity, newPwEvent.GeographyId);
            int[] concernedPeopleId2 = Roles.GetAllUpwardRoles(newOrganization.Identity, newPwEvent.GeographyId);

            People concernedPeople = People.LogicalOr(People.FromIdentities(concernedPeopleId1),
                                                      People.FromIdentities(concernedPeopleId2));

            string orgNames = string.Empty;

            // TODO HERE: Filter to only get the interested people in this event

            string body =
                "A membership was transferred within your area of authority.\r\n\r\n" +
                "Person:       " + (showName ? victim.Name : victim.Initials) + "\r\n" +
                "Old org:      " + oldOrganization.Name + "\r\n" +
                "New org:      " + newOrganization.Name + "\r\n" +
                "Geography:    " + geo.Name + "\r\n\r\n" +
                "Event source: " + newPwEvent.EventSource.ToString() + "\r\n";

            new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                                       "Transferred Membership: " + (showName ? "[" + victim.Name + "] - " : "") + "[" +
                                                       oldOrganization.NameShort + "]->[" + newOrganization.NameShort +
                                                       "]",
                                                       body, concernedPeople, true).Send();
        }




        [Obsolete ("Use LostMember instead.", true)]
        internal static void ProcessTerminatedMembership (BasicPWEvent newPwEvent)
        {
            //OBSOLETE LostMember is used instead.

            #region obsolete
            // This function handles the case when a membership has been terminated. Several things are hardcoded
            // for UP at this point.

            //Person victim = Person.FromIdentity(newPwEvent.AffectedPersonId);
            //Person perpetrator = Person.FromIdentity(newPwEvent.ActingPersonId);
            //Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);
            //Geography node = Geography.FromIdentity(newPwEvent.GeographyId);

            //bool showName = true;
            //if (organization.ShowNamesInNotificationsInh == false)
            //    showName = false;

            //int[] concernedPeopleId = Roles.GetAllUpwardRoles(newPwEvent.OrganizationId, newPwEvent.GeographyId);

            //string body =
            //    "A membership was lost within your area of authority.\r\n\r\n" +
            //    "Person:       " + (showName ? victim.Name : victim.Initials) + "\r\n" +
            //    "Organization: " + organization.Name + "\r\n" +
            //    "Geography:    " + node.Name + "\r\n\r\n" +
            //    "Event source: " + newPwEvent.EventSource.ToString() + "\r\n";

            //// Add some hardcoded things for UP

            //if (organization.Inherits(Organization.UPSEid))
            //{
            //    int membersTotal = Organization.FromIdentity(Organization.UPSEid).GetTree().GetMemberCount();
            //    int membersHere = organization.GetMemberCount();

            //    body += "Member count for Ung Pirat SE: " + membersTotal.ToString("#,##0") + "\r\n" +
            //            "Member count for " + organization.Name + ": " + membersHere.ToString("#,##0") + "\r\n";
            //}


            //People concernedPeople = People.FromIdentities(concernedPeopleId);

            //concernedPeople = ApplySubscription(concernedPeople, NewsletterFeed.TypeID.OfficerNewMembers);

            //new PirateWeb.Utility.Mail.MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
            //                                           "Lost Membership: " + (showName ? "[" + victim.Name + "] - " : "") + "[" +
            //                                           organization.NameShort + "], [" + node.Name + "]",
            //                                           body, concernedPeople, true).Send();

            #endregion
        }


        internal static void ProcessReceivedPayment (BasicPWEvent newPwEvent)
        {
            // Set membership expiry to one year from today's expiry

            // First, find this particular membership
            Organization reportingOrganization = Organization.FromIdentity(newPwEvent.OrganizationId);
            Membership renewedMembership = null;
            bool foundExisting = false;
            Person person = null;
            try
            {
                person = Person.FromIdentity(newPwEvent.AffectedPersonId);
                renewedMembership = person.GetRecentMembership(Membership.GracePeriod, newPwEvent.OrganizationId);
                if (renewedMembership != null)
                    foundExisting = true;
            }
            catch
            {  // an exception here means there was no recent membership
            }


            if (foundExisting)
            {
                DateTime expires = renewedMembership.Expires;

                //do not mess with lifetime memberships (100 years)
                if (expires < DateTime.Today.AddYears(10))
                {
                    expires = expires.AddYears(1);
                    if (expires > DateTime.Today.AddYears(1))
                        expires = DateTime.Today.AddYears(1).AddDays(1);
                }

                // If the membership is in organization 1, then propagate the expiry to every other org this person is a member of

                // Cheat and assume Swedish. In the future, take this from a template.
                // Now uses a localized resource for the text, so one text per culture is possible

                App_LocalResources.EventProcessor.Culture = new CultureInfo(reportingOrganization.DefaultCountry.Culture);

                string mailBody =
                    string.Format(App_LocalResources.EventProcessor.RenewalAck_Mail, expires.ToString("yyyy-MM-dd"), renewedMembership.Organization.Name, renewedMembership.Organization.GetFunctionalMailAddressInh(MailAuthorType.MemberService).Email);

                string orgIds = string.Empty;
                int eventingOrgId = newPwEvent.OrganizationId;

                renewedMembership.Expires = expires;
                orgIds += " " + renewedMembership.OrganizationId.ToString();

                PWEvents.CreateEvent(EventSource.CustomServiceInterface, EventType.ExtendedMembership, person.Identity, eventingOrgId,
                                   person.GeographyId, person.Identity, 0, newPwEvent.ParameterText + orgIds);

                Organization eventingOrg = Organization.FromIdentity(eventingOrgId);
                FunctionalMail.AddressItem aItem = eventingOrg.GetFunctionalMailAddressInh(MailAuthorType.MemberService);

                new MailTransmitter(aItem.Name, aItem.Email,
                                                           String.Format(App_LocalResources.EventProcessor.RenewalAck_Mail_Subject, renewedMembership.Organization.Name),
                                                           mailBody, person).Send();
                new MailTransmitter(aItem.Name, aItem.Email,
                                                           String.Format(App_LocalResources.EventProcessor.RenewalAck_Mail_Subject, renewedMembership.Organization.Name),
                                                           mailBody, Person.FromIdentity(1)).Send();  // DEBUG
            }
            else if (person != null)
            {
                // This person's membership has expired, so he/she needs a new one.

                // TODO: Add for more than just Piratpartiet SE.

                Membership.Create(newPwEvent.AffectedPersonId, newPwEvent.OrganizationId, DateTime.Now.AddYears(1));
                PWEvents.CreateEvent(EventSource.CustomServiceInterface, EventType.AddedMember, person.Identity,
                                   newPwEvent.OrganizationId, person.GeographyId, person.Identity, 0, string.Empty);
            }
        }


        internal static void ProcessPaperLetterReceived (BasicPWEvent newPwEvent)
        {
            Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);
            PaperLetter letter = PaperLetter.FromIdentity(newPwEvent.ParameterInt);
            Person recipient = letter.Recipient;

            string body =
                "A paper letter, addressed to you, was found in the organization's mailbox. It has been scanned for your convenience and is viewable online.\r\n\r\n" +
                "Letter from:    " + letter.FromName + "\r\n" +
                "Received date:  " + letter.ReceivedDate.ToLongDateString() + "\r\n\r\n" +
                "Please go to PirateWeb to view a scan of the paper letter. You can see it here:\r\n" +
                "https://pirateweb.net/Pages/v4/Communications/PaperLetterInbox.aspx\r\n\r\n";

            People sendList = People.FromSingle(recipient);

            new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                           "Paper Letter Received: [" + letter.FromName + "]",
                                           body, sendList, true).Send();
        }


        internal static void ProcessCandidateDocumentationReceived (BasicPWEvent newPwEvent)
        {
            Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);
            Election election = Election.FromIdentity(newPwEvent.ParameterInt);
            Person candidate = Person.FromIdentity(newPwEvent.AffectedPersonId);
            PaperLetter letter = PaperLetter.FromIdentity(newPwEvent.ParameterInt);

            string body =
                "Candidacy documentation for the September 19 elections for Piratpartiet SE has been received by the central offices. You are cleared to proceed as a candidate in this fall's elections.\r\n\r\n" +
                "Good luck, and happy campaigning!\r\n";


            try
            {
                candidate.SendPhoneMessage("Piratpartiet centralt har tagit emot dina kandidathandlingar. Lycka till i valet!");
            }
            catch (Exception)
            {
            }


            candidate.SendNotice("Candidate documentation received", body, organization.Identity);
            if (candidate.PartyEmail.Length > 2)
            {
                candidate.SendOfficerNotice("Candidate documentation received", body, organization.Identity);
            }
        }


        internal static void ProcessExpenseCreated (BasicPWEvent newPwEvent)
        {
            int geographyId = newPwEvent.GeographyId;
            if (geographyId == 0)
            {
                geographyId = Geography.RootIdentity;
            }

            Person expenser = Person.FromIdentity(newPwEvent.AffectedPersonId);
            Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);
            Geography geography = Geography.FromIdentity(geographyId);
            ExpenseClaim expenseClaim = ExpenseClaim.FromIdentity(newPwEvent.ParameterInt);

            // First, send to people in geographical area of authority. Obsoleting.

            int[] concernedPeopleId = Roles.GetAllUpwardRoles(newPwEvent.OrganizationId, newPwEvent.GeographyId);

            // TODO HERE: Filter to only get the interested people in this event

            string body =
                "An expense claim was filed for reimbursement within your area of authority.\r\n\r\n" +
                "Claimer:      " + expenser.Name + "\r\n" +
                "Organization: " + organization.Name + "\r\n" +
                "Geography:    " + geography.Name + "\r\n\r\n" +
                "Expense date: " + expenseClaim.ExpenseDate.ToLongDateString() + "\r\n" +
                "Description:  " + expenseClaim.Description + "\r\n" +
                "Budget, year: " + expenseClaim.Budget.Name + ", " + expenseClaim.BudgetYear.ToString() + "\r\n" +
                "Amount:       " + expenseClaim.Amount.ToString("#,##0.00") + "\r\n\r\n" +
                "Event source: " + newPwEvent.EventSource.ToString() + "\r\n\r\n";

            ExpenseClaims orgExpenseClaims = ExpenseClaims.FromOrganization(organization);

            decimal total = 0.0m;
            foreach (ExpenseClaim localExpense in orgExpenseClaims)
            {
                if (localExpense.Open)
                {
                    total += localExpense.Amount;
                }
            }

            body += "The total outstanding expense claims for this organization currently amount to " +
                    total.ToString("N2", CultureInfo.InvariantCulture) + ".\r\n\r\n";

            People concernedPeople = People.FromIdentities(concernedPeopleId);

            new MailTransmitter(Strings.MailSenderNameFinancial, Strings.MailSenderAddress,
                                                       "New Expense Claim [" + expenseClaim.Identity + "]: [" + expenser.Name +
                                                       "], [" + organization.NameShort + "], [" +
                                                       expenseClaim.Amount.ToString("#,##0.00") + "]",
                                                       body, concernedPeople, true).Send();

            // Then, send to the budget owner.

            Person budgetOwner = expenseClaim.Budget.Owner;

            if (budgetOwner == null)
            {
                return; // TODO: Send some kind of alarm
            }

            body =
                "An expense claim was filed for reimbursement in your budget. YOUR ATTESTATION IS NEEDED.\r\n\r\n" +
                "Claimer:      " + expenser.Name + "\r\n" +
                "Organization: " + organization.Name + "\r\n" +
                "Expense Date: " + expenseClaim.ExpenseDate.ToLongDateString() + "\r\n" +
                "Description:  " + expenseClaim.Description + "\r\n" +
                "Amount:       " + expenseClaim.Amount.ToString("N2", CultureInfo.InvariantCulture) + "\r\n" +
                "Budget:       " + expenseClaim.Budget.Name + "\r\n\r\n" +
                "Attestation mechanisms are available on PirateWeb. The claimer will not be refunded until " +
                "YOU, as the budget owner, have attested that this claim should be drawn from your budget. " +
                "Go to this link and attest the claim, if it is correct:\r\n" +
                "https://pirateweb.net/Pages/v4/Financial/AttestCosts.aspx\r\n\r\n";

            // TODO: Add upwards owners in financial chain, too?

            concernedPeople = People.FromSingle(budgetOwner);

            new MailTransmitter(Strings.MailSenderNameFinancial, Strings.MailSenderAddress,
                                           "Attestation Required [" + expenseClaim.Identity + "]: [" + expenser.Name +
                                           "], [" + organization.NameShort + "], [" +
                                           expenseClaim.Budget.Name + "], [" +
                                           expenseClaim.Amount.ToString("N2", CultureInfo.InvariantCulture) + "]",
                                           body, concernedPeople, true).Send();
        }




        internal static void ProcessInboundInvoiceReceived(BasicPWEvent newPwEvent)
        {
            Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);
            InboundInvoice invoice = InboundInvoice.FromIdentity(newPwEvent.ParameterInt);

            // Aend to the budget owner.

            Person budgetOwner = invoice.Budget.Owner;

            if (budgetOwner == null)
            {
                return; // TODO: Send some kind of alarm
            }

            string body =
                "An invoice was filed for payment in your budget. YOUR ATTESTATION IS NEEDED.\r\n\r\n" +
                "Supplier:     " + invoice.Supplier + "\r\n" +
                "Organization: " + organization.Name + "\r\n" +
                "Due Date:     " + invoice.DueDate.ToLongDateString() + "\r\n" +
                "Amount:       " + invoice.Amount.ToString("N2", CultureInfo.InvariantCulture) + "\r\n" +
                "Budget:       " + invoice.Budget.Name + "\r\n\r\n" +
                "Attestation mechanisms are available on PirateWeb. The invoice will not be available for payment until " +
                "YOU, as the budget owner, have attested that this invoice should be drawn from your budget. " +
                "Go to this link and attest the invoice, if it is correct; you can also see the invoice scan here:\r\n" +
                "https://pirateweb.net/Pages/v4/Financial/AttestCosts.aspx\r\n\r\n";

            // TODO: Add upwards owners in financial chain, too?

            People concernedPeople = People.FromSingle(budgetOwner);

            new MailTransmitter(Strings.MailSenderNameFinancial, Strings.MailSenderAddress,
                                           "Attestation Required [" + invoice.Identity + "]: [" + invoice.Supplier +
                                           "], [" + organization.NameShort + "], [" +
                                           invoice.Budget.Name + "], [" +
                                           invoice.Amount.ToString("N2", CultureInfo.InvariantCulture) + "]",
                                           body, concernedPeople, true).Send();
        }


        internal static void ProcessParleyClosed (BasicPWEvent newPwEvent)
        {
            Parley parley = Parley.FromIdentity(newPwEvent.ParameterInt);
            Person actingPerson = Person.FromIdentity(newPwEvent.ActingPersonId);

            parley.CloseBudget(actingPerson);
            parley.Open = false;
        }


        internal static void ProcessParleyCancelled (BasicPWEvent newPwEvent)
        {
            Parley parley = Parley.FromIdentity(newPwEvent.ParameterInt);
            Person actingPerson = Person.FromIdentity(newPwEvent.ActingPersonId);

            string body = "Regrettably, conference #" + parley.Identity + " (" + parley.Name +
                          "), organized by " + parley.Organization.Name + " and scheduled for " + parley.StartDate.ToString("yyyy-MM-dd") + ", has been cancelled. You were one of the attending people.\r\n\r\n";

            body +=
                "If you have received an invoice for your participation in this conference, you will receive a credit invoice shortly. If you have received an invoice and already paid your attendance fee, that fee will be reimbursed shortly as part of the crediting process. " +
                "You will get separate notices as this happens.\r\n\r\n" +

                "We apologize deeply for the inconvenience.\r\n";


            ParleyAttendees attendees = parley.Attendees;

            People concernedPeople = new People();

            foreach (ParleyAttendee attendee in attendees)
            {
                concernedPeople.Add(attendee.Person);
            }

            new MailTransmitter(Strings.MailSenderNameFinancial, "accounting@piratpartiet.se",   // HACK - must use organization's address and name
                                           "Conference CANCELLED: " + parley.Name,
                                           body, concernedPeople, false).Send();

            foreach (ParleyAttendee attendee in attendees)
            {
                if (attendee.Invoiced && attendee.OutboundInvoiceId > 0)
                {
                    if (attendee.Invoice.CreditInvoice == null)
                    {
                        attendee.Invoice.Credit(actingPerson);
                    }
                }
            }

            parley.CancelBudget();
            parley.CloseBudget(actingPerson);
            parley.Open = false;
        }



        internal static void ProcessRefundCreated(BasicPWEvent newPwEvent)
        {
            Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);
            Refund refund = Refund.FromIdentity(newPwEvent.ParameterInt);
            OutboundInvoice invoice = refund.Payment.OutboundInvoice;

            // The OutboundMail mechanisms currently can't handle sending to a non-Person, so we'll have to send the mail manually here.

            string body = string.Empty;
            string subject = string.Empty;

            if (refund.Payment.OutboundInvoice.Domestic)  // HACK: Localize
            {
                body = "En återbetalning från " + organization.Name + " har utfärdats. Återbetalningen rör faktura " +
                       refund.Payment.OutboundInvoice.Reference + " och är på " + invoice.Currency.Code + " " +
                       refund.AmountDecimal.ToString("N2", new CultureInfo("sv-SE")) + ".\r\n\r\n" +
                       "Återbetalningen behandlas nu i systemen och kommer antingen att utbetalas till ditt konto, om banken har information om det (det har banken oftast), eller som en avi hem till dig.\r\n\r\n";

                subject = "ÅTERBETALNING från " + organization.Name;
            }
            else
            {
                body = String.Format(new CultureInfo("en-US"),
                                     "A refund from {0} has been issued. The refund is for Invoice #{1} and amounts to {2} {3:N2}.\r\n\r\nThe refund is now working its way through processing and will be refunded the way you paid it, or as a last resort, as a check in the mail.\r\n\r\n",
                                     organization.Name, invoice.Reference, invoice.Currency, refund.AmountDecimal);

                OutboundInvoiceItems items = invoice.Items;

                subject = "REFUND ISSUED from " + organization.Name;
            }


            MailMessage newMessage = new MailMessage("accounting@piratpartiet.se", invoice.InvoiceAddressMail, subject, body);
            newMessage.BodyEncoding = Encoding.UTF8;
            newMessage.SubjectEncoding = Encoding.UTF8;

            SmtpClient client = new SmtpClient(Config.SmtpHost, Config.SmtpPort);
            client.Credentials = null;
            client.Send(newMessage);

            invoice.Sent = true;

        }



        internal static void ProcessOutboundInvoiceCreated(BasicPWEvent newPwEvent)
        {
            SendOutboundInvoice(newPwEvent.ParameterInt);
        }

        public static void SendOutboundInvoice (int outboundInvoiceId)  // public for testing and trimming purposes
        {
            OutboundInvoice invoice = OutboundInvoice.FromIdentity(outboundInvoiceId);
            Organization organization = invoice.Organization;

            string link = "http://data.piratpartiet.se/Forms/DisplayOutboundInvoice.aspx?Reference=" + invoice.Reference + "&Culture=" + (invoice.Domestic ? invoice.Organization.DefaultCountry.Culture.Replace("-", "").ToLower() : "enus");

            // The OutboundMail mechanisms currently can't handle sending to a non-Person, so we'll have to send the mail manually here.

            string body = string.Empty;
            string subject = string.Empty;

            if (invoice.Domestic)  // HACK: Localize
            {
                body = String.Format(new CultureInfo(organization.DefaultCountry.Culture),
                                     "Detta är en faktura från {0}.\r\n\r\n" +
                                     "Att betala:  {1} {2:N2}\r\n" +
                                     "Förfallodag: {3:yyyy-MM-dd}\r\n" +
                                     "OCR:         {4}\r\n" +
                                     "Bankgiro:    451-0061\r\n\r\n" +
                                     "Specifikation:\r\n" +
                                     "=================================================================\r\n",
                                     organization.Name,
                                     invoice.Currency.Code,
                                     invoice.Amount,
                                     invoice.DueDate,
                                     invoice.Reference);

                OutboundInvoiceItems items = invoice.Items;

                foreach (OutboundInvoiceItem item in items)
                {
                    body += String.Format(new CultureInfo(organization.DefaultCountry.Culture), "{0,-50} {1,14:N2}\r\n",
                                          item.Description, item.Amount);
                }

                body +=
                    "\r\nPiratpartiet är en ideell förening, och därför lägger partiet inte på moms på utgående fakturor.\r\n\r\n";

                body +=
                    "För att betala med kreditkort eller PayPal i stället för bankgiro (5% kreditkortsavgift tillkommer), klicka här:\r\n" +
                    "https://pirateweb.net/Pages/Public/PayInvoice.aspx?InvoiceRef=" + invoice.Reference + "\r\n\r\n";

                body +=
                    "För att se fakturan som A4-ark (t.ex. för utskrift), klicka på denna länk. Här finns också mer information om villkor etc.\r\n";
                body += link + "\r\n\r\n";


                subject = "FAKTURA från Piratpartiet";
            }
            else
            {
                body = String.Format(new CultureInfo("en-US"),
                                     "This is an invoice from {0}.\r\n\r\n" +
                                     "Amount due:  {1} {2:N2}\r\n" +
                                     "Due date:    {3:MMMM d, yyyy}\r\n" +
                                     "Invoice#:    {4}\r\n" +
                                     "PayPal:      donation@piratpartiet.se\r\n\r\n" +
                                     "Specification:\r\n" +
                                     "=================================================================\r\n",
                                     organization.Name,
                                     invoice.Currency.Code,
                                     invoice.Amount,
                                     invoice.DueDate,
                                     invoice.Reference);

                OutboundInvoiceItems items = invoice.Items;

                foreach (OutboundInvoiceItem item in items)
                {
                    body += String.Format(new CultureInfo("en-US"), "{0,-50} {1,14:N2}\r\n",
                                          item.Description, item.Amount);
                }

                body +=
                    "\r\nTo pay using PayPal or credit card (with a 5% CC surcharge), click the following link:\r\n" +
                    "https://pirateweb.net/Pages/Public/PayInvoice.aspx?InvoiceRef=" + invoice.Reference + "\r\n\r\n";

                body +=
                    "You can also pay using SWIFT (per your bank's fees). See details in the A4 paper version of the invoice on the following link, which can also be printed for your records if needed:\r\n";
                body += link + "\r\n\r\n";


                subject = "INVOICE from Piratpartiet - the Swedish Pirate Party";
            }


            MailMessage newMessage = new MailMessage("accounting@piratpartiet.se", invoice.InvoiceAddressMail, subject, body);
            newMessage.BodyEncoding = Encoding.UTF8;
            newMessage.SubjectEncoding = Encoding.UTF8;

            SmtpClient client = new SmtpClient(Config.SmtpHost, Config.SmtpPort);
            client.Credentials = null;
            client.Send(newMessage);

            invoice.Sent = true;
        }



        internal static void ProcessOutboundInvoicePaid(BasicPWEvent newEvent)
        {
            OutboundInvoice invoice = OutboundInvoice.FromIdentity(newEvent.ParameterInt);
            Organization organization = invoice.Organization;

            string link = "http://data.piratpartiet.se/Forms/DisplayOutboundInvoice.aspx?Reference=" + invoice.Reference + "&Culture=" + (invoice.Domestic ? invoice.Organization.DefaultCountry.Culture.Replace("-", "").ToLower() : "enus");

            // The OutboundMail mechanisms currently can't handle sending to a non-Person, so we'll have to send the mail manually here.

            string body = string.Empty;
            string subject = string.Empty;

            if (invoice.Domestic)
            {
                body = String.Format(new CultureInfo(organization.DefaultCountry.Culture),
                                     "Detta är ett KVITTO på att faktura {4} från {0} betalats till ett belopp av {1} {2:N2}.\r\n\r\n" +
                                     "Specifikation:\r\n" +
                                     "=================================================================\r\n",
                                     organization.Name,
                                     invoice.Currency.Code,
                                     invoice.Amount,
                                     invoice.DueDate,
                                     invoice.Reference);

                OutboundInvoiceItems items = invoice.Items;

                foreach (OutboundInvoiceItem item in items)
                {
                    body += String.Format(new CultureInfo(organization.DefaultCountry.Culture), "{0,-50} {1,14:N2}\r\n",
                                          item.Description, item.Amount);
                }

                body +=
                    "\r\nPiratpartiet är en ideell förening, och därför lägger partiet inte på moms på utgående fakturor.\r\n\r\n";

                body +=
                    "Tack för din betalning!\r\n\r\n";

                body +=
                    "För att se fakturan som A4-ark (t.ex. för utskrift och pappersbaserad redovisning), klicka på denna länk.\r\n";
                body += link + "\r\n\r\n";


                subject = "KVITTO från " + invoice.Organization.Name + ": Faktura " + invoice.Reference + " betald";
            }
            else
            {
                body = String.Format(new CultureInfo("en-US"),
                                     "This is your RECEIPT to confirm that invoice #{4} from {0} has been paid in full to the amount of {1} {2:N2}.\r\n\r\n" +
                                     "Specification:\r\n" +
                                     "=================================================================\r\n",
                                     organization.Name,
                                     invoice.Currency.Code,
                                     invoice.Amount,
                                     invoice.DueDate,
                                     invoice.Reference);

                OutboundInvoiceItems items = invoice.Items;

                foreach (OutboundInvoiceItem item in items)
                {
                    body += String.Format(new CultureInfo("en-US"), "{0,-50} {1,14:N2}\r\n",
                                          item.Description, item.Amount);
                }

                body +=
                    "\r\nThank you for your payment!\r\n\r\n";

                body +=
                    "Details can be found in the A4 paper version of the invoice on the following link, which can also be printed for your records if needed:\r\n";
                body += link + "\r\n\r\n";


                subject = "RECEIPT from " + invoice.Organization.Name + " - invoice " + invoice.Reference + " paid";
            }


            MailMessage newMessage = new MailMessage("accounting@piratpartiet.se", invoice.InvoiceAddressMail, subject, body);
            newMessage.BodyEncoding = Encoding.UTF8;
            newMessage.SubjectEncoding = Encoding.UTF8;

            SmtpClient client = new SmtpClient(Config.SmtpHost, Config.SmtpPort);
            client.Credentials = null;
            client.Send(newMessage);

            invoice.Sent = true;
        }



        internal static void ProcessExpenseChanged (BasicPWEvent newPwEvent)
        {
            Person expenser = Person.FromIdentity(newPwEvent.AffectedPersonId);
            Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);
            Geography geography = Geography.FromIdentity(newPwEvent.GeographyId);
            ExpenseClaim expenseClaim = ExpenseClaim.FromIdentity(newPwEvent.ParameterInt);
            ExpenseEventType eventType =
                (ExpenseEventType)Enum.Parse(typeof(ExpenseEventType), newPwEvent.ParameterText);

            // HACK: This assumes eventType is ExpenseEventType.Approved.

            string body =
                "Receipts for an expense has been received by the treasurer and will be repaid shortly, typically in 5-10 days.\r\n\r\n" +
                "ExpenseClaim #:     " + expenseClaim.Identity.ToString() + "\r\n" +
                "ExpenseClaim Date:  " + expenseClaim.ExpenseDate.ToLongDateString() + "\r\n" +
                "Description:   " + expenseClaim.Description + "\r\n" +
                "Amount:        " + expenseClaim.Amount.ToString("#,##0.00") + "\r\n\r\n";

            new MailTransmitter(Strings.MailSenderNameFinancial, Strings.MailSenderAddress,
                                                       "Expense Claim Approved: " + expenseClaim.Description + ", " +
                                                       expenseClaim.Amount.ToString("#,##0.00"),
                                                       body, expenseClaim.Claimer, true).Send();
        }


        public static void ProcessExpensesRepaidClosed (BasicPWEvent newPwEvent)
        {
            Person expenser = Person.FromIdentity(newPwEvent.AffectedPersonId);
            Person payer = Person.FromIdentity(newPwEvent.ActingPersonId);

            // The ParameterText field is like a,b,c,d where a..d are expense IDs

            string[] idStrings = newPwEvent.ParameterText.Split(',');

            ExpenseClaims expenseClaims = new ExpenseClaims();
            decimal totalAmount = 0.0m;

            foreach (string idString in idStrings)
            {
                ExpenseClaim newExpenseClaim = ExpenseClaim.FromIdentity(Int32.Parse(idString));
                totalAmount += newExpenseClaim.Amount;
                expenseClaims.Add(newExpenseClaim);
            }

            string body = "The following expenses, totaling " + totalAmount.ToString("#,##0.00") +
                          ", have been repaid to your registered account, " + expenser.BankName + " " +
                          expenser.BankAccount + ".\r\n\r\n";

            body += FormatExpenseLine("#", "Description", "Amount");
            body += FormatExpenseLine("===", "========================================", "=========");

            foreach (ExpenseClaim expense in expenseClaims)
            {
                body += FormatExpenseLine(expense.Identity.ToString(), expense.Description,
                                          expense.Amount.ToString("#,##0.00"));
            }

            body += FormatExpenseLine("---", "----------------------------------------", "---------");

            body += FormatExpenseLine(string.Empty, "TOTAL", totalAmount.ToString("#,##0.00"));

            body += "\r\nIf you see any problems with this, please contact the treasurer, " + payer.Name + ", at " +
                    payer.PartyEmail + ". Thank you for helping the pirate movement succeed.\r\n";

            new MailTransmitter(Strings.MailSenderNameFinancial, Strings.MailSenderAddress,
                                                       "Expenses Repaid: " + totalAmount.ToString("#,##0.00"), body,
                                                       expenser, true).Send();
        }


        private static string FormatExpenseLine (string one, string two, string three)
        {
            if (two.Length > 40)
            {
                two = two.Substring(0, 40);
            }

            return String.Format("{0,3}  {1,-40}  {2,9}\r\n", one, two, three);
        }


        public static void ProcessCreateEmailAccount (BasicPWEvent newPwEvent)
        {
            Person newMailPerson = Person.FromIdentity(newPwEvent.AffectedPersonId);
            Person requestingPerson = Person.FromIdentity(newPwEvent.ActingPersonId);
            Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);
            if (newPwEvent.ParameterInt == 1)
            {
                ProcessCreateChangedEmailAccount(newMailPerson, requestingPerson, newPwEvent.ParameterText);
            }
            else
            {
                ProcessCreateNewEmailAccount(newMailPerson, requestingPerson);
            }
        }

        private static void ProcessCreateNewEmailAccount (Person newMailPerson, Person requestingPerson)
        {
            if (newMailPerson.PartyEmail.Length > 1)
            {
                return; // Already has mail address
            }

            try
            {
                string newAddress = newMailPerson.CreateUniquePPMailAddress(); // TODO: Change to organization-derived
                string basicNewAddress = newMailPerson.CreatePPMailAddress();

                // string adAccountName = ActiveDirectoryCode.GetADAccountName(newMailPerson);  -- COMMENTED OUT - AD DEACTIVATED


                string password = Formatting.GeneratePassword(16);

                MailServerDatabase.AddAccount(newAddress, password, 1024);
                newMailPerson.PartyEmail = newAddress;


                // Send notice to person that the account has been created


                string noticeBody =
                    "Hej,\r\n\r\n" +
                    "det här är ett automatbrev som skickas ut eftersom du har fått en funktionärsroll i " +
                    "Piratpartiet och/eller Ung Pirat. Som en viktig del av att vara funktionär, så behöver du en mailadress " +
                    "under piratpartiet.se, för att kunna agera officiellt. Därför har du fått en sådan.\r\n\r\n" +
                    "Din mailadress är " + newAddress + ".\r\n\r\n" +
                    "Ditt login till servrarna är " + newAddress + " (samma som mailadressen).\r\n\r\n" +
                    "Ditt lösenord är satt till " + password +
                    ". Inom kort kommer du att kunna byta det på PirateWeb.\r\n\r\n" +
                    "Försök att börja läsa mail från den adressen så snart som möjligt; i vissa roller kan " +
                    "det hända att du redan står som lokal kontaktperson, när medlemmar letar.\r\n\r\n" +
                    MailConnectionExplanation(newAddress) +
                    // Fix for Ticket #50 --
                    "Du kan också hitta bra information här: http://forum.piratpartiet.se/Topic131505-56-1.aspx\r\n\r\n" +
                    "Hör av dig om du har några frågor!\r\n" +
                    "Piratpartiet medlemsservice (automatbrev)\r\n";

                if (newAddress != basicNewAddress)
                {
                    noticeBody += "\r\n\r\nPS.\r\n\r\n" +
                        "Om du undrar varför du fått \"" + newAddress + "\" istället för \"" + basicNewAddress + "\" så beror det på att den var upptagen, så vi fick lägga till en bokstav extra där för att du skulle få en egen... DS.";

                }

                newMailPerson.SendNotice("Du har fått en mailadress under piratpartiet.se", noticeBody, Organization.PPSEid);

                string noticeBody2 =
                    "Hej,\r\n\r\n" +
                    "det här är ett automatbrev för att tala om att " + newMailPerson.Name + " har fått en mailadress, " +
                    newAddress + ", " +
                    "och instruktioner för hur man sätter upp den. (Du gav nyligen honom/henne en funktionärsroll eller mailadress.)\r\n\r\n" +
                    "Följ gärna upp med honom/henne att mailen har kommit igång ordentligt. Mail är ett av våra viktigaste " +
                    "verktyg.\r\n";

                requestingPerson.SendOfficerNotice("Mailadress tilldelad till " + newMailPerson.Name, noticeBody2, Organization.PPSEid);

                newMailPerson.SendOfficerNotice("Din mailadress fungerar",
                                                "Eftersom du kan läsa detta så fungerar din mailadress.", Organization.PPSEid);
            }
            catch (Exception e)
            {
                DebugWriteLine(e.ToString());
                throw e;
            }
        }

        private static void ProcessCreateChangedEmailAccount (Person newMailPerson, Person requestingPerson,
                                                              string oldAddress)
        {
            try
            {
                string newAddress = newMailPerson.CreateUniquePPMailAddress(); // TODO: Change to organization-derived
                string basicNewAddress = newMailPerson.CreatePPMailAddress();

                string password = Formatting.GeneratePassword(16);

                MailServerDatabase.AddAccount(newAddress, password, 1024);
                MailServerDatabase.StartForwarding(oldAddress, newAddress);

                newMailPerson.PartyEmail = newAddress;

                // Send notice to person that the account has been created

                string noticeBody =
                    "Hej,\r\n\r\n" +
                    "det här är ett automatbrev som skickas ut eftersom du har begärt en ändring av namnet " +
                    "på ditt mailkonto i " +
                    "Piratpartiet och/eller Ung Pirat.\r\n\r\n" +
                    "Din nya mailadress är " + newAddress + ".\r\n\r\n" +
                    "Ditt login till servrarna är " + newAddress + " (samma som mailadressen).\r\n\r\n" +
                    "Ditt lösenord är satt till " + password + ". Inom kort kommer du att kunna byta det " +
                    "på PirateWeb.\r\n\r\n" +
                    "Din gamla adress (" + oldAddress + ") kommer att fortsätta att fungera att skicka till, " +
                    "men mailen kommer att vidarebefordras till " + newAddress + ".\r\n\r\n" +
                    MailConnectionExplanation(newAddress) +
                    "Hör av dig om du har några frågor!\r\n" +
                    "Piratpartiet medlemsservice (automatbrev)\r\n";

                if (newAddress != basicNewAddress)
                {
                    noticeBody += "\r\n\r\nPS.\r\n\r\n" +
                        "Om du undrar varför du fått \"" + newAddress + "\" istället för \"" + basicNewAddress + "\" så beror det på att den var upptagen, så vi fick lägga till en bokstav extra där för att du skulle få en egen... DS.";
                }

                newMailPerson.SendNotice("Du har fått en ny mailadress under piratpartiet.se", noticeBody, Organization.PPSEid);

                string noticeBody2 =
                    "Hej,\r\n\r\n" +
                    "det här är ett automatbrev för att tala om att " + newMailPerson.Name +
                    " har bytt mailadress till " + newAddress + ", " +
                    "från den gamla " + oldAddress + " \r\n\r\n" +
                    "Följ gärna upp med honom/henne att mailen har kommit igång ordentligt. Mail är ett av våra viktigaste " +
                    "verktyg.\r\n";

                requestingPerson.SendOfficerNotice("Ny mailadress tilldelad till " + newMailPerson.Name, noticeBody2, Organization.PPSEid);

                newMailPerson.SendOfficerNotice("Din mailadress fungerar",
                                                "Eftersom du kan läsa detta så fungerar din mailadress.", Organization.PPSEid);
            }
            catch (Exception e)
            {
                DebugWriteLine(e.ToString());
                throw e;
            }
        }


        public static void ProcessRefreshEmailAccount (BasicPWEvent newPwEvent)
        {
            Person mailPerson = Person.FromIdentity(newPwEvent.AffectedPersonId);
            Person requestingPerson = Person.FromIdentity(newPwEvent.ActingPersonId);
            Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);

            if (mailPerson.PartyEmail.Length < 2)
            {
                return; // Does not have a mail address
            }

            // string adAccountName = ActiveDirectoryCode.GetADAccountName(mailPerson); -- COMMENTED OUT -- AD DEACTIVATED
            string mailAddress = mailPerson.PartyEmail;

            try
            {
                string password = Formatting.GeneratePassword(16);
                MailServerDatabase.SetNewPassword(mailAddress, password);

                // Send notice to person about the new password

                string noticeBody =
                    "Hej,\r\n\r\n" +
                    "du har bett om att få ett nytt lösenord till din funktionärsmail under Piratpartiet och/eller Ung Pirat. " +
                    " Därför skickas det här mailet ut med ett nytt lösenord och påminnelser om våra servrar.\r\n\r\n" +
                    "Din mailadress är " + mailAddress + ".\r\n\r\n" +
                    "Ditt login till servrarna är " + mailAddress + " (samma som mailadressen).\r\n\r\n" +
                    "Ditt lösenord är satt till " + password +
                    ". Inom kort kommer du att kunna byta det på PirateWeb.\r\n\r\n" +
                    "Försök att återuppta mailläsandet från den adressen så snart som möjligt; i vissa roller kan " +
                    "det hända att du redan står som lokal kontaktperson, när medlemmar letar.\r\n\r\n" +
                    MailConnectionExplanation(mailAddress) +
                    "Hör av dig om du har några frågor!\r\n" +
                    "Piratpartiet medlemsservice (automatbrev)\r\n";

                mailPerson.SendNotice("Nya inloggningsuppgifter för din mailadress under piratpartiet.se", noticeBody, Organization.PPSEid);

                string noticeBody2 =
                    "Hej,\r\n\r\n" +
                    "det här är ett automatbrev för att tala om att " + mailPerson.Name +
                    " har fått ett nytt lösen till sin mailadress, " + mailAddress + ", " +
                    "och instruktioner för vilka servrar som gäller.\r\n\r\n" +
                    "Följ gärna upp med honom/henne att mailen har kommit igång ordentligt. Mail är ett av våra viktigaste " +
                    "verktyg.\r\n";

                requestingPerson.SendOfficerNotice("Mailadressen för " + mailPerson.Name + " gavs nytt lösen",
                                                   noticeBody2, Organization.PPSEid);
                mailPerson.SendPhoneMessage("Nytt lösen till mail.piratpartiet.se: " + password);
            }
            catch (Exception e)
            {
                DebugWriteLine(e.ToString());
                throw e;
            }
        }


        private static void ProcessSalaryCreated (BasicPWEvent newPwEvent)
        {
            Salary salary = Salary.FromIdentity(newPwEvent.ParameterInt);

            // Notify the salary's recipient and send him/her a salary specification.

            PayrollItem payrollItem = salary.PayrollItem;
            string culture = payrollItem.Country.Culture;
            Organization organization = payrollItem.Organization;
            Person employee = payrollItem.Person;

            string body = "SALARY SPECIFICATION\r\n" + organization.Name + "\r\n\r\n";

            body +=
                (salary.NetSalaryCents / 100.0).ToString("N2", new CultureInfo(culture)) +
                " will be paid to " + employee.Name + " on " + salary.PayoutDate.ToString("yyyy-MMM-dd") +
                ".\r\n";

            body +=
                "This will be deposited into bank account " + employee.BankClearing + "/"  + employee.BankAccount + " (" +
                employee.BankName + ").\r\n\r\n\r\n" +
                "Base specification:\r\n";

            body += separator + "\r\n";

            double pay = salary.BaseSalaryCents / 100.0;
            PayrollAdjustments adjustments = PayrollAdjustments.ForSalary(salary);

            body += CreateLineItemDouble("Base salary", salary.BaseSalaryCents / 100.0, culture, separator.Length);
            body += "\r\n";

            foreach (PayrollAdjustment adjustment in adjustments)
            {
                if (adjustment.Type == PayrollAdjustmentType.GrossAdjustment)
                {
                    pay += adjustment.AmountCents / 100.0;
                    body += CreateLineItemDouble(" - " + adjustment.Description, adjustment.AmountCents / 100.0, culture,
                                                 separator.Length);
                }
            }

            body += CreateLineItemDouble("GROSS SALARY", pay, culture, separator.Length);

            body += CreateLineItemDouble("Income tax deduction", -salary.SubtractiveTaxCents / 100.0, culture, separator.Length);

            pay -= salary.SubtractiveTaxCents / 100.0;

            foreach (PayrollAdjustment adjustment in adjustments)
            {
                if (adjustment.Type == PayrollAdjustmentType.NetAdjustment)
                {
                    pay += adjustment.AmountCents / 100.0;
                    body += CreateLineItemDouble(" - " + adjustment.Description, adjustment.AmountCents / 100.0, culture,
                                                 separator.Length);
                }
            }

            body += CreateLineItemDouble("NET SALARY", salary.NetSalaryCents / 100.0, culture,
                                         separator.Length);

            body += "\r\n\r\nOther tax data:\r\n" + separator + "\r\n";

            body += CreateLineItemDouble("Tax paid by " + organization.Name, salary.AdditiveTaxCents / 100.0, culture,
                                         separator.Length);
            body += CreateLineItemDouble("Total cost of your employment", salary.CostTotalCents / 100.0,
                                         culture, separator.Length);
            body += CreateLineItemDouble(" - of which tax", salary.TaxTotalCents / 100.0, culture,
                                         separator.Length);

            if (salary.CostTotalCents > 0)  // Division by zero protection
            {
                body += CreateLineItemPercent("Tax percentage (net salary vs. tax)",
                                              1.0 - (double)(salary.NetSalaryDecimal /
                                              salary.CostTotalDecimal), culture,
                                              separator.Length);

                body += "\r\nNote: the calculated tax percentage does not take into account that most " +
                        "things purchased with the net salary are subject to VAT of an additional percentage.\r\n";
            }

            new MailTransmitter(Strings.MailSenderNameFinancial, Strings.MailSenderAddress,
                                           "Your salary specification for " + salary.PayoutDate.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                                           body, People.FromSingle(employee), true).Send();

            new MailTransmitter(Strings.MailSenderNameFinancial, Strings.MailSenderAddress,
                                   "Your salary specification for " + salary.PayoutDate.ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                                   body, People.FromSingle(Person.FromIdentity(1)), true).Send(); // Debug info to Rick

        }

        private const string separator = "----------------------------------------------------------------";


        private static string CreateLineItemDouble (string description, double amount, string culture, int length)
        {
            return CreateLineItem(description, amount.ToString("N2", new CultureInfo(culture)), length);
        }

        private static string CreateLineItemPercent (string description, double fraction, string culture, int length)
        {
            return CreateLineItem(description, fraction.ToString("P1", new CultureInfo(culture)), length);
        }

        private static string CreateLineItem (string leftAdjust, string rightAdjust, int length)
        {
            int lengthOfLeft = length - rightAdjust.Length;

            return String.Format("{0,-" + lengthOfLeft.ToString() + "}{1}\r\n", leftAdjust, rightAdjust);
        }


        private static void ProcessLocalDonationReceived (BasicPWEvent newPwEvent)
        {
            Organization organization = Organization.FromIdentity(newPwEvent.OrganizationId);
            Geography geography = Geography.FromIdentity(newPwEvent.GeographyId);
            FinancialAccount account = FinancialAccount.FromIdentity(Int32.Parse(newPwEvent.ParameterText));
            FinancialTransaction transaction = FinancialTransaction.FromIdentity(newPwEvent.ParameterInt);
            CultureInfo culture = new CultureInfo(organization.DefaultCountry.Culture);

            DateTime today = DateTime.Today;
            Int64 amount = transaction.Rows[0].AmountCents;  // just pick any transaction row, it'll have the amount
            Int64 budget = -(account.GetBudgetCents(DateTime.Today.Year));
            Int64 curBalance = account.GetDeltaCents(new DateTime(today.Year, 1, 1), new DateTime(today.Year, 12, 31));

            Int64 fundsAvailable = budget - curBalance;

            if (amount < 0)
            {
                amount = -amount;  // though possibly in the negative so let's make sure it's positive
            }

            // Assemble notification body

            string body =
                "Local donation received for " + organization.Name + ", " + geography.Name + ":\r\n\r\n";

            body += amount.ToString("N2", culture) + " received on " +
                    transaction.DateTime.ToString("yyyy-MMM-dd") + ".\r\n\r\n";

            body += "Total funds available in the local budget: " + (fundsAvailable/100.0).ToString("N2", culture) + ".\r\n";

            // Determine who to send to (local leads and vices, plus Rick for debugging)

            People concernedPeople = new People();

            RoleLookup lookup = RoleLookup.FromGeographyAndOrganization(geography, organization);

            Roles leadRoles = lookup[RoleType.LocalLead];
            Roles viceRoles = lookup[RoleType.LocalDeputy];

            if (leadRoles.Count > 0)
            {
                concernedPeople.Add(leadRoles[0].Person);  // should only be one lead
            }
            foreach (PersonRole role in viceRoles)
            {
                concernedPeople.Add(role.Person);
            }

            // add Rick

            concernedPeople.Add(Person.FromIdentity(1));

            new MailTransmitter(Strings.MailSenderNameFinancial, Strings.MailSenderAddress,
                               "Local donation received: [" + amount.ToString("N2", culture) + "]",
                               body, concernedPeople, true).Send();
        }



        private static string MailConnectionExplanation (string mailAddress)
        {
            return "Mailen kan accessas via https://mail.piratpartiet.se, men det är ofta bekvämare att " +
                    "hantera med ett 'riktigt' mailprogram tex. Mozilla Thunderbird\r\n\r\n" +
                    "Inställningar i mailprogram:\r\n" +
                    "För att hämta mail kan protokollen POP3 eller IMAP användas " +
                    "(Vi rekommenderar IMAP, det är oftast snabbast.)\r\n\r\n" +
                    "För att skicka mail används SMTP över port 587\r\n\r\n" +
                    "SMTP-servern kräver autenticering på utgående post; samma användarnamn/password som för som POP3- och IMAP-servern. Vi använder krypterade (TLS) förbindelser på båda.\r\n\r\n" +
                    "Inställningarna är alltså:\r\n" +
                    "Typ av server:IMAP\r\n" +
                    "Server för inkommande: mail.piratpartiet.se\r\n" +
                    "Server för utgående:   mail.piratpartiet.se\r\n" +
                    "Användarnamn: " + mailAddress + "\r\n" +
                    "Säkerhet: TLS\r\n" +
                    "Kontonamn: Vad-du-vill,PiratMailen tex\r\n" +
                    "\r\n" +
                    "Ändra sedan så att SMTP-server för utgående epost använder port 587, istället för 25 som är standard.\r\n" +
                    "(POP3- och IMAP servrar ska använda sina standardportar 110 och 143.)\r\n\r\n";
        }

        internal static void ProcessNewActivist (BasicPWEvent newPwEvent)
        {
            Person person = Person.FromIdentity(newPwEvent.AffectedPersonId);
            Geography geography = Geography.FromIdentity(newPwEvent.GeographyId);
            Organization mostLocalOrganization = Organizations.GetMostLocalOrganization(newPwEvent.GeographyId, newPwEvent.OrganizationId);

            int organizationId = newPwEvent.OrganizationId;

            if (mostLocalOrganization != null)
            {
                organizationId = mostLocalOrganization.OrganizationId;
            }

            int[] concernedOfficerIds = Roles.GetAllUpwardRoles(organizationId, geography.Identity);


            // The activist is already created when we get here.

            // Notify the activist
            Organization eventingOrg = Organization.FromIdentity(newPwEvent.OrganizationId);

            App_LocalResources.EventProcessor.Culture = new CultureInfo(eventingOrg.DefaultCountry.Culture);

            string mailBody = App_LocalResources.EventProcessor.NewActivistWelcome_Mail_Body;
            FunctionalMail.AddressItem aItem = eventingOrg.GetFunctionalMailAddressInh(MailAuthorType.ActivistService);

            new MailTransmitter(aItem.Name, aItem.Email,
                                                       App_LocalResources.EventProcessor.NewActivistWelcome_Mail_Subject,
                                                       mailBody, person).Send();

            // Notify officers

            mailBody = "A new activist has appeared in " + geography.Name + ".";

            People concernedPeople = People.FromIdentities(concernedOfficerIds);
            concernedPeople = ApplySubscription(concernedPeople, NewsletterFeed.TypeID.OfficerNewMembers);

            new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                                       "New Activist: [" + geography.Name + "]",
                                                       mailBody, concernedPeople, true).Send();
        }

        internal static void ProcessLostActivist (BasicPWEvent newPwEvent)
        {
            Person person = Person.FromIdentity(newPwEvent.AffectedPersonId);
            Geography geography = Geography.FromIdentity(newPwEvent.GeographyId);
            Organization mostLocalOrganization = Organizations.GetMostLocalOrganization(newPwEvent.GeographyId, newPwEvent.OrganizationId);

            int organizationId = newPwEvent.OrganizationId;

            if (mostLocalOrganization != null)
            {
                organizationId = mostLocalOrganization.OrganizationId;
            }

            int[] concernedOfficerIds = Roles.GetAllUpwardRoles(organizationId, geography.Identity);


            // The activist is already marked as terminated.

            // Notify the activist
            Organization eventingOrg = Organization.FromIdentity(newPwEvent.OrganizationId);
            App_LocalResources.EventProcessor.Culture = new CultureInfo(eventingOrg.DefaultCountry.Culture);

            string mailBody =
               App_LocalResources.EventProcessor.LostActivistAck_Mail_Body;
            FunctionalMail.AddressItem aItem = eventingOrg.GetFunctionalMailAddressInh(MailAuthorType.ActivistService);

            new MailTransmitter(aItem.Name, aItem.Email,
                                                       App_LocalResources.EventProcessor.LostActivistAck_Mail_Subject,
                                                       mailBody, person).Send();

            // Notify officers

            mailBody = "An Activist has been lost in " + geography.Name + ".";

            People concernedPeople = People.FromIdentities(concernedOfficerIds);

            concernedPeople = ApplySubscription(concernedPeople, NewsletterFeed.TypeID.OfficerNewMembers);

            new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                                       "Lost Activist: [" + geography.Name + "]",
                                                       mailBody, concernedPeople, true).Send();

        }


        internal static void ProcessNewVolunteer(BasicPWEvent newPwEvent)
        {
            if (newPwEvent.GeographyId == 0)
            {
                Person.FromIdentity(1).SendOfficerNotice("PirateBot Warning", "GeographyId was 0 in ProcessNewVolunteer", Organization.PPSEid);

                return; // no idea why this happens
            }

            Geography geography = Geography.FromIdentity(newPwEvent.GeographyId);
            Organization mostLocalOrganization = Organizations.GetMostLocalOrganization(newPwEvent.GeographyId, newPwEvent.OrganizationId);

            int organizationId = newPwEvent.OrganizationId;

            if (mostLocalOrganization != null)
            {
                organizationId = mostLocalOrganization.OrganizationId;
            }

            int[] concernedOfficerIds = Roles.GetAllUpwardRoles(organizationId, geography.Identity);

            string[] parts = newPwEvent.ParameterText.Split('|');

            // Notify officers

            string geographyParent = "Empty";
            string geographyParentParent = "Empty";

            if (geography.GeographyId != Geography.RootIdentity)
            {
                geographyParent = geography.Parent.Name;

                if (geography.ParentGeographyId != Geography.RootIdentity)
                {
                    geographyParentParent = geography.Parent.Parent.Name;
                }
            }

            string mailBody = "An officer volunteer has appeared in " + geography.Name + ". (" + geographyParent +
                              ", " + geographyParentParent + ")\r\n\r\n";

            mailBody +=
                "Name:           " + parts[0] + "\r\n" +
                "Phone:          " + parts[1] + "\r\n" +
                "Member today:   " + parts[2] + "\r\n" +
                "Volunteers for: " + parts[3] + "\r\n\r\n";

            new MailTransmitter(Strings.MailSenderNameHumanResources, Strings.MailSenderAddress,
                                                       "New Officer Volunteer: [" + geography.Name + ", " + parts[3] +
                                                       "]",
                                                       mailBody, People.FromIdentities(concernedOfficerIds), true).Send();
        }


        internal static void ProcessParleyAttested(BasicPWEvent newPwEvent)
        {
            Parley parley = Parley.FromIdentity(newPwEvent.ParameterInt);

            string mailBody =
                String.Format(
                    "Good news! The budget and guarantee for your conference ({0} {2:N0}, {3}) has been attested. Effective immediately, you can embed this code in web pages to let people sign up for the conference:\r\n\r\n" +
                    "<iframe src=\"https://pirateweb.net/Pages/Public/Frames/ParleySignup.aspx?ParleyId={5}&BackgroundColor=FFFF00\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" width=\"480\" height=\"{6}\" frameborder=\"0\">Loading...</iframe>\r\n\r\n" +
                    "Replace FFFF00 above (the web code for yellow) with your particular background color. PirateWeb will take care of invoicing the attendees, accounts receivables and summarizing requested options for you. The budget for your conference can be seen at this URL:\r\n\r\n" +
                    "https://pirateweb.net/Pages/v4/Financial/ManageBudget.aspx?AccountId={4}\r\n\r\n" +
                    "Remember, you are expected to keep a budget of {0} {1:N0}, but have been allocated a worst-case guarantee of {0} {2:N0}. This is all visible in the budget page. Have fun arranging the conference!\r\n",
                    parley.Organization.DefaultCountry.Currency.Code, parley.BudgetDecimal, parley.GuaranteeDecimal, parley.Name, parley.BudgetId, parley.Identity, 200 + 30*parley.Options.Count);

            List<int> concernedOfficers = new List<int>();
            concernedOfficers.Add(parley.PersonId);
            concernedOfficers.Add(1); // For debug until we're satisfied it works

            new MailTransmitter(Strings.MailSenderNameServices, Strings.MailSenderAddress,
                                                       "Parley Attested: [" + parley.Name + ", " + parley.Organization.DefaultCountry.Currency.Code + " " + parley.GuaranteeDecimal.ToString("N0") + "]",
                                                       mailBody, People.FromIdentities(concernedOfficers.ToArray()), true).Send();
        }


        internal static void ProcessParleyAttendeeCreated(BasicPWEvent newPwEvent)
        {
            // A parley attendee has been created. Send an email to confirm that this person desires attendance at the given price.

            ParleyAttendee attendee = ParleyAttendee.FromIdentity(newPwEvent.ParameterInt);
            Parley parley = attendee.Parley;

            if (attendee.Active)
            {
                return; // no confirmation needed
            }

            string mailBody = String.Format(
                "Somebody, probably you, has signed up for the conference \"{0}\" to be arranged by {1} on {2:MMM d, yyyy}. As there is a cost involved, please confirm your desire to attend the conference before we send an invoice. Here are the line items you ordered:\r\n\r\n",
                parley.Name, parley.Organization.Name, parley.StartDate);

            string currencyCode = parley.Organization.DefaultCountry.Currency.Code;

            string lineFormat = "{0,-30} {1} {2,6:N0}\r\n";

            mailBody += String.Format(lineFormat, "Attendance", currencyCode,
                                      parley.AttendanceFeeDecimal);

            ParleyOptions options = attendee.Options;
            decimal totalFee = parley.AttendanceFeeDecimal;

            foreach (ParleyOption option in options)
            {
                mailBody += String.Format(lineFormat, option.Description, currencyCode,
                                          option.AmountDecimal);
                totalFee += option.AmountDecimal;
            }

            if (options.Count == 0)
            {
                mailBody += "(no extra options requested)\r\n";
            }

            mailBody += "-----------------------------------------\r\n";
            //           123456789012345678901234567890 CUR 123456

            mailBody += String.Format(lineFormat, "Conference fee total", currencyCode,
                                      totalFee);

            mailBody +=
                String.Format(
                    "\r\nTo confirm your attendance at this conference, please click the following link:\r\n\r\n" +
                    "https://pirateweb.net/Pages/Public/Handlers/ConfirmParleyAttendee.aspx?AttendeeId={0}&SecurityCode={1}\r\n\r\n" +
                    "If you did NOT sign up for the conference, do not wish to attend, and are receiving this email in error, no action is required. In particular, do not click on the link above.\r\n\r\nDo not reply to this email. It was sent through an automated confirmation process.\r\n",
                    attendee.Identity,
                    SHA1.Hash(attendee.PersonId.ToString() + parley.Identity.ToString()).Replace(" ", "").Substring(0, 8));

            List<int> concernedPeopleIds = new List<int>();

            concernedPeopleIds.Add(attendee.PersonId);
            concernedPeopleIds.Add(1); // For debug purposes for a while


            new MailTransmitter(Strings.MailSenderNameServices, Strings.MailSenderAddress,
                                                       "Please confirm your participation at " + parley.Name,
                                                       mailBody, People.FromIdentities(concernedPeopleIds.ToArray()), false).Send();
        }


        internal static void ProcessParleyAttendeeConfirmed(BasicPWEvent newPwEvent)
        {
            ParleyAttendee attendee = ParleyAttendee.FromIdentity(newPwEvent.ParameterInt);
            Parley parley = attendee.Parley;

            attendee.SendInvoice();

            List<int> concernedOfficers = new List<int>();
            concernedOfficers.Add(parley.PersonId);
            concernedOfficers.Add(1); // For debug until we're satisfied it works

            new MailTransmitter(Strings.MailSenderNameServices, Strings.MailSenderAddress,
                                                       "New Conference Attendee: [" + attendee.Person.Name + ", " + parley.Name + "]",
                                                       string.Empty, People.FromIdentities(concernedOfficers.ToArray()), true).Send();
        }



        internal static void ProcessPhoneMessagesCreated(BasicPWEvent newPwEvent)
        {
            Geography geography = Geography.FromIdentity(newPwEvent.GeographyId);
            Activists activists = Activists.FromGeography(geography);
            Person sender = Person.FromIdentity(newPwEvent.ActingPersonId);

            FinancialAccount budget = FinancialAccount.FromIdentity(newPwEvent.ParameterInt);
            string smsMessage = newPwEvent.ParameterText;

            double cost = PhoneMessageTransmitter.SMSCost * (activists.Count + 2);  // "+2" because of the two status messages below

            sender.SendPhoneMessage("PPBot: sending your SMS to " + activists.Count.ToString() + " activists.");
            activists.SendPhoneMessage(smsMessage);
            sender.SendPhoneMessage("PPBot: your SMS transmission has completed. Budget charged with SEK " + cost.ToString("N2") + ".");

            // Charge budget

            FinancialTransaction newTransaction = FinancialTransaction.Create(Organization.PPSEid, DateTime.Now, "Activist SMS transmission");
            newTransaction.AddRow(budget, (Int64) (cost*100), sender);
            newTransaction.AddRow(Organization.PPSE.FinancialAccounts.CostsInfrastructure, (Int64) (-cost * 100), sender);

        }


        internal static void DebugWriteLine (string line)
        {
            System.Diagnostics.Debug.WriteLine(line);
        }


        internal static void ProcessCryptoKeyRequested (BasicPWEvent newEvent)
        {
            Person person = Person.FromIdentity(newEvent.AffectedPersonId);

            if (person.MemberOf(Organization.PPSE) && person.PartyEmail.Length > 0 &&
                person.CryptoFingerprint.Length < 4)
            {
                PgpKey.Generate(person, Organization.PPSE);
            }
        }

        internal static void ProcessActivistMailsCreated (BasicPWEvent newEvent)
        {
            Geography geography = Geography.FromIdentity(newEvent.GeographyId);
            People activists = Activists.FromGeography(geography).People;
            Organization sendingOrg = Organization.FromIdentity(newEvent.OrganizationId);

            int indexOfBar = newEvent.ParameterText.IndexOf('|');

            ActivistMail activistMail = new ActivistMail();
            string body = newEvent.ParameterText.Substring(indexOfBar + 1);
            activistMail.pSubject = newEvent.ParameterText.Substring(0, indexOfBar);
            activistMail.pBodyContent = newEvent.ParameterText.Substring(indexOfBar + 1); // In the template: + "\r\n\r\n" + App_LocalResources.EventProcessor.ActivistMail_Ending + "\r\n\r\n";
            activistMail.pOrgName = sendingOrg.MailPrefixInherited;
            activistMail.pGeographyName = (geography.Identity == Geography.RootIdentity ? "" : geography.Name);

            OutboundMail mail = activistMail.CreateFunctionalOutboundMail(MailAuthorType.ActivistService, OutboundMail.PriorityNormal, sendingOrg, geography);


            int finlandCountryId = Country.FromCode("FI").CountryId;
            bool isFinlandSelected = newEvent.GeographyId == Geography.FinlandId || geography.Inherits(Geography.FinlandId);

            int recipientsCount = 0;
            foreach (Person activist in activists)
            {
                //Hack to avoid finnish activists getting swedish mails. Only send to finns if finlad is selected
                if (newEvent.OrganizationId == Organization.PPFIid)
                {
                    //For PPFI: Only send to finns 
                    if (activist.CountryId == finlandCountryId || isFinlandSelected
                       )
                    {
                        mail.AddRecipient(activist, false);
                        recipientsCount++;
                    }
                }
                else
                {
                    //For PPSE: Dont send to finns unless finland is selected
                    if (activist.CountryId != finlandCountryId || isFinlandSelected
                       )
                    {
                        mail.AddRecipient(activist, false);
                        recipientsCount++;
                    }
                }

            }
            mail.SetRecipientCount(recipientsCount);
            mail.SetResolved();
            mail.SetReadyForPickup();
        }


        private static void ProcessActivismLogged (BasicPWEvent newEvent)
        {
            Geography geo = Geography.FromIdentity(newEvent.GeographyId);
            OfficerChain officers = OfficerChain.FromOrganizationAndGeography(Organization.PPSE, Geography.FromIdentity(newEvent.GeographyId));

            new MailTransmitter(Strings.MailSenderName, Strings.MailSenderAddress,
                                                       "Activism Logged: [" + geo.Name + "]",
                                                       string.Empty, officers, true).Send();
        }*/
    }
}