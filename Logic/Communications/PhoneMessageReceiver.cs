using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Database;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Communications
{
    internal class PhoneMessage
    {
        private string errorMessage = "";
        private string fromNumber = "";
        private string message = "";

        private Dictionary<int, BasicPerson> people = new Dictionary<int, BasicPerson>();
        //people matching the set number

        private string replyMessage = "";
        private DateTime sentAt;

        public PhoneMessage (string fromNumber, string message, DateTime sentAt)
        {
            FromNumber = fromNumber;
            Message = message;
            SentAt = sentAt;
        }

        public Dictionary<int, BasicPerson> People
        {
            get { return this.people; }
        }

        public string ReplyMessage
        {
            get { return this.replyMessage; }
            set { this.replyMessage = value; }
        }

        public string ErrorMessage
        {
            get { return this.errorMessage; }
            set { this.errorMessage = value; }
        }

        public string FromNumber
        {
            get { return this.fromNumber; }
            //setting the number loads the list of matching people
            protected set
            {
                this.fromNumber = value;

                // The phone number should come from SMS host as +46<num>
                // But it can be saved in different formats in the DB, so try the most common
                // ways to format the number.
                string[] phoneNumbers = PhoneMessageTransmitter.DeNormalizedPhoneNumber (FromNumber);
                this.people = PhoneMessageReceiver.GetPeopleFromPhoneNumbers (phoneNumbers);
            }
        }

        public string Message
        {
            get { return this.message; }
            protected set { this.message = value; }
        }

        public DateTime SentAt
        {
            get { return this.sentAt; }
            protected set { this.sentAt = value; }
        }
    }

    internal interface IPhoneMessageHandler
    {
        bool Filter (PhoneMessage msg);
        bool Handle (PhoneMessage msg);
    }


    internal abstract class PhoneMessageHandler : IPhoneMessageHandler
    {
        public virtual bool Filter (PhoneMessage message)
        {
            return (message.People.Count > 0);
        }

        public virtual bool Handle (PhoneMessage message)
        {
            return false;
        }

        protected virtual void ForwardMessage (string from, string to, string subject, string body)
        {
            PhoneMessageReceiver.SendMail (from, to, subject, body);
        }
    }

    internal class DefaultMessageHandler : PhoneMessageHandler
    {
        public override bool Filter (PhoneMessage message)
        {
            return true; //Catch all
        }

        public override bool Handle (PhoneMessage msg)
        {
            string from = "sms-noreply@piratpartiet.se";
            string to = "smsreplies@piratpartiet.se";
            string subject = "SMS från '" + msg.FromNumber + "'";
            string body = "Inkommet från telefonnummer: " + msg.FromNumber + "\r\n" +
                          "Meddelande: " + msg.Message + "\r\n" +
                          "Skickades: " + msg.SentAt.ToShortDateString() + " " + msg.SentAt.ToShortTimeString() +
                          "\r\n\r\n";

            if (msg.People.Count > 0)
            {
                foreach (BasicPerson bp in msg.People.Values)
                {
                    body += "Det telefonnumret tillhör medlem/aktivistnummer: ";
                    Person person = Person.FromBasic (bp);
                    body += person.Identity + " (";
                    if (!person.IsActivist)
                        body += "ej ";
                    body += "aktivist) \r\n";
                }
                string numbers = "";
                foreach (BasicPerson bp in msg.People.Values)
                {
                    numbers += "," + bp.PersonId;
                }
                body += "\r\n\r\n" + numbers.Substring (1);
            }
            else
                body += "Hittade ingen medlem/aktivist i databasen med det numret.\r\n\r\n";

            if (msg.ErrorMessage != "")
            {
                body += "\r\nComment from SMS processing: " + msg.ErrorMessage;
            }

            ForwardMessage (from, to, subject, body);
            msg.ReplyMessage +=
                "\r\nDitt meddelande har vidarebefordrats till medlemsservice@piratpartiet.se. Kontakta oss om du undrar över något.";
            PWLog.Write (PWLogItem.None, 0, PWLogAction.SMSHandled, "Forwarded SMS from " + msg.FromNumber + " to " + to,
                msg.Message + "\r\n" + msg.ErrorMessage);

            return true;
        }
    }

    internal class UnregisterActivistHandler : PhoneMessageHandler
    {
        public override bool Filter (PhoneMessage msg)
        {
            string[] smsParts = msg.Message.ToLower().Trim().Split (new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if ((smsParts[0] == "stopp" || smsParts[0] == "stop") &&
                (smsParts[1] == "aktivist" || smsParts[1] == "activist"))
            {
                return base.Filter (msg);
            }
            return false;
        }

        public override bool Handle (PhoneMessage msg)
        {
            //TODO: Decide if more than one hit is OK
            if (msg.People.Count > 0)
            {
                foreach (BasicPerson bPerson in msg.People.Values)
                {
                    Person person = Person.FromBasic (bPerson);
                    if (person.IsActivist)
                    {
                        ActivistEvents.TerminateActivistWithLogging (person, EventSource.SMS);
                        return true;
                    }
                }
            }
            msg.ReplyMessage = "Misslyckades utföra åtgärden: Vi kunde inte hitta ditt telefonnummer i vårt register.";
            msg.ErrorMessage = "\r\nRequest to stop being an aktivist.";
            msg.ErrorMessage = "\r\nDid not find the phone number in the database.\r\n\r\nHave replied:" +
                               msg.ReplyMessage;
            return false;
        }
    }

    internal class RegisterActivistHandler : PhoneMessageHandler
    {
        public override bool Filter (PhoneMessage msg)
        {
            string[] smsParts = msg.Message.ToLower().Trim().Split (new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (smsParts[0] == "start" && (smsParts[1] == "aktivist" || smsParts[1] == "activist"))
            {
                return true;
            }
            return false;
        }


        public override bool Handle (PhoneMessage msg)
        {
            Trace.WriteLine (msg); // use parameter to suppress warning

            /* -- all of the phone message handling need to move to app or plugin

            // register new activist
            // check for words: activist <postal code> <e-mail>
            msg.ErrorMessage = "\r\nRequest to register as activist.";
            if (msg.People.Count > 0)
            {
                foreach (BasicPerson bPerson in msg.People.Values)
                {
                    Person person = Person.FromBasic(bPerson);
                    if (person.IsActivist)
                    {
                        if (msg.People.Count == 1)
                        {
                            // Only one and is already activist.
                            msg.ErrorMessage = "\r\nRequest to register as activist.";
                            msg.ErrorMessage += "\r\nThis person IS already an activist.";
                        }
                    }
                    else if (msg.People.Count == 1)
                    {
                        int partyOrgId = person.NationalPartyOrg(false);
                        ActivistEvents.CreateActivistWithLogging(person.Geography, person, "Registered by SMS", EventSource.SMS, true, true, partyOrgId);
                        msg.ReplyMessage = "Välkommen som aktivist i Piratpartiet!";

                        return true; // Handled;
                    }
                }
                if (msg.People.Count > 1)
                {
                    msg.ReplyMessage = "Misslyckades utföra åtgärden.";
                    msg.ErrorMessage = "\r\nRequest to renew membership.";
                    msg.ErrorMessage = "\r\nMultiple hits.\r\n\r\nHave replied:" + msg.ReplyMessage;
                }
            }
            else
            {
                // Handle further info in the message 
                // check for words: activist <postal code> <e-mail>

            }-- */
            return false;
        }
    }

    internal class RenewMembershipHandler : PhoneMessageHandler
    {
        public override bool Filter (PhoneMessage msg)
        {
            string[] smsParts = msg.Message.ToLower().Trim().Split (new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            if (smsParts[0] == "pp" && smsParts[1] == "igen")
            {
                return true;
            }
            return false;
        }


        public override bool Handle (PhoneMessage msg)
        {
            /* This needs to move to a plugin. */

            /*
            // renew memberships
            // if there is a third parameter it is the person#
            string[] smsParts = msg.Message.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            int personId = 0;

            if (smsParts.Length > 2)
            {
                // personid is specified
                try
                {
                    personId = Int32.Parse(smsParts[2]);
                    PWEvents.CreateEvent(EventSource.CustomServiceInterface, EventType.ReceivedMembershipPayment,
                                                                personId, Organization.PPSEid, 0, personId, 0,
                                                                msg.Message.Replace(" ", "_") + "/" + msg.FromNumber);
                    msg.ReplyMessage = "Det kommer strax ett e-mail som bekräftar förnyelsen.";

                    return true; // Handled;
                }
                catch (Exception)
                {
                    msg.ReplyMessage += "Kunde inte tolka tredje parametern (medlemsnummer).";
                    msg.ErrorMessage += "Person# parameter not valid.\r\nHave replied: " + msg.ReplyMessage;
                }
            }
            else
            {
                // personid is NOT specified, base on phone number
                if (msg.People.Count > 0)
                {
                    int countOfMembers = 0;
                    Person person = null;
                    foreach (BasicPerson bPerson in msg.People.Values)
                    {
                        person = Person.FromBasic(bPerson);
                        if (person.GetRecentMemberships(Membership.GracePeriod).Count > 0)
                        {
                            countOfMembers++;
                        }
                    }
                    if (countOfMembers == 1)
                    {
                        PWEvents.CreateEvent(EventSource.CustomServiceInterface, EventType.ReceivedMembershipPayment,
                                                                    person.PersonId, Organization.PPSEid, 0, personId, 0,
                                                                    msg.Message.Replace(" ", "_") + "/" + msg.FromNumber);
                        msg.ReplyMessage = "Det kommer strax ett e-mail som bekräftar förnyelsen.";

                        return true; // Handled;
                    }
                    else
                    {
                        msg.ReplyMessage = "Misslyckades utföra åtgärden.";
                        msg.ErrorMessage = "\r\nRequest to renew membership.";
                        msg.ErrorMessage = "\r\nMultiple hits.\r\n\r\nHave replied:" + msg.ReplyMessage;
                    }
                }
                else
                {
                    msg.ReplyMessage = "Misslyckades utföra åtgärden: Vi kunde inte hitta ditt telefonnummer i vårt register.";
                    msg.ErrorMessage = "\r\nRequest to renew membership.";
                    msg.ErrorMessage = "\r\nDid not find the phone number in the database.\r\n\r\nHave replied:" + msg.ReplyMessage;
                }
            }


            return false; */


            // use parameters to suppress warnings
            Trace.WriteLine (msg);
            return false;
        }
    }


    public class PhoneMessageReceiver
    {
        private static readonly List<IPhoneMessageHandler> _handlers = new List<IPhoneMessageHandler>();

        private static List<IPhoneMessageHandler> Handlers
        {
            get
            {
                if (_handlers.Count == 0)
                {
                    //set up list of handlers
                    _handlers.Add (new UnregisterActivistHandler());
                    _handlers.Add (new RegisterActivistHandler());
                    _handlers.Add (new DefaultMessageHandler()); // Always last
                }

                return _handlers;
            }
        }


        /// <summary>
        ///     Handle one SMS message according to the registered handlers
        /// </summary>
        /// <param name="fromNumber"></param>
        /// <param name="message"></param>
        /// <param name="sentAtStr"></param>
        public static void Handle (string fromNumber, string message, DateTime sentAt)
        {
            // create a message object 
            PhoneMessage msg = new PhoneMessage (fromNumber, message, sentAt);

            //Ask all handlers if they want to handle the message object
            foreach (IPhoneMessageHandler handler in Handlers)
            {
                try
                {
                    // Want to handle this message ?
                    if (handler.Filter (msg))
                    {
                        // if Handle() returns true do not call the following handlers
                        if (handler.Handle (msg))
                            break;
                    }
                }
                catch
                {
                }
            }
            if (msg.ReplyMessage != "")
            {
                PhoneMessageTransmitter.Send (msg.FromNumber, msg.ReplyMessage);
            }
        }


        /// <summary>
        ///     Find persons with a phone number in one of the formats supplied
        /// </summary>
        /// i
        /// <param name="phoneNumbers"></param>
        /// <returns></returns>
        public static Dictionary<int, BasicPerson> GetPeopleFromPhoneNumbers (string[] phoneNumbers)
        {
            Dictionary<int, BasicPerson> peopleDict = new Dictionary<int, BasicPerson>();

            try
            {
                for (int i = 0; i < phoneNumbers.Length; i++)
                {
                    BasicPerson[] people = SwarmDb.GetDatabaseForReading()
                        .GetPeopleFromPhoneNumber ("SE", phoneNumbers[i]);
                    foreach (BasicPerson bp in people)
                    {
                        //Only add if not seen.
                        if (!peopleDict.ContainsKey (bp.PersonId))
                        {
                            peopleDict[bp.PersonId] = bp;
                        }
                    }
                }
            }
            catch
            {
            }

            return peopleDict;
        }

        internal static void SendMail (string from, string to, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = false;
            mail.Subject = subject;
            mail.Body = body;
            mail.From = new MailAddress (from, from, Encoding.UTF8);
            mail.To.Add (to);

            SmtpClient mailserver = new SmtpClient ("mail.piratpartiet.se", 587);
            try
            {
                mailserver.Send (mail);
            }
            catch (Exception e)
            {
                throw new Exception (e.Message + "<br><hr>" + mail.Body);
            }
        }
    }
}