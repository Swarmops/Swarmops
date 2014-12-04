using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Special.Sweden
{
    public class SupportDatabase
    {
        private static MySqlConnection GetConnection()
        {
            string connect = Persistence.Key["SupportDatabaseConnect"];
            return new MySqlConnection(connect);
        }


        private static SupportCase[] GetUndeliverableCases()
        {
            return GetUndeliverableCases(0, 99999);
        }


        public static SupportCaseDelta[] GetCaseDeltas(int fromDeltaId)
        {
            List<SupportCaseDelta> result = new List<SupportCaseDelta>();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                MySqlCommand command =
                    new MySqlCommand(
                        "SELECT ixBugEvent,ixBug,ixPerson,dt,sVerb,sChanges FROM BugEvent WHERE ixBugEvent > " +
                        fromDeltaId + "  ORDER BY ixBugEvent",
                        connection);
                command.CommandTimeout = 600;

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(1)) // Safeguard -- some ixBug are NULL due to data corruption
                        {
                            result.Add(ReadSupportCaseDeltaFromReader(reader));
                        }
                    }
                }
            }

            return result.ToArray();
        }


        public static SupportCase[] GetOpenCases()
        {
            List<SupportCase> result = new List<SupportCase>();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                MySqlCommand command =
                    new MySqlCommand(
                        "SELECT ixBug,sTitle,sCustomerEmail FROM Bug WHERE fOpen=1", connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            result.Add(ReadFullSupportCaseFromReader(reader));
                        }
                        catch (SqlNullValueException)
                        {
                            Console.WriteLine("Exception on bug {0}", reader.GetInt32(0));
                        }
                    }
                }
            }

            return result.ToArray();
        }


        public static SupportCase GetCase(int supportCaseId)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                MySqlCommand command =
                    new MySqlCommand(
                        "SELECT ixBug,sTitle,sCustomerEmail FROM Bug WHERE ixBug=" + supportCaseId, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadFullSupportCaseFromReader(reader);
                    }

                    throw new ArgumentException("No such Support Case Id: " + supportCaseId);
                }
            }
        }


        private static SupportCase[] GetUndeliverableCases(int startBugId, int limit)
        {
            List<SupportCase> result = new List<SupportCase>();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                MySqlCommand command =
                    new MySqlCommand(
                        "SELECT ixBug,sTitle FROM Bug WHERE (sTitle LIKE 'Undeliver%' OR sTitle = 'failure notice' OR sTitle LIKE 'Mail delivery failed: returning message to sender' OR sTitle LIKE 'Delivery Status Notification%' OR sTitle LIKE 'Olevererbart:%' OR sTitle LIKE 'Returned mail:%') AND fOpen=1 and ixBug > " +
                        startBugId + " order by ixBug LIMIT " + limit,
                        connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadSupportCaseFromReader(reader));
                    }
                }
            }

            return result.ToArray();
        }

        private static SupportCase[] GetDelayWarnings()
        {
            List<SupportCase> result = new List<SupportCase>();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                MySqlCommand command =
                    new MySqlCommand(
                        "SELECT ixBug,sTitle FROM Bug WHERE (sTitle = 'Delivery Status Notification (Delay)' OR sTitle LIKE 'Warning: could not send message for %' OR sTitle LIKE 'AUTO:%' OR sTitle LIKE 'Out of Office%') AND fOpen=1",
                        connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadSupportCaseFromReader(reader));
                    }
                }
            }

            return result.ToArray();
        }


        public static bool IsCaseOpen(int caseId)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                MySqlCommand command =
                    new MySqlCommand(
                        "SELECT fOpen FROM Bug WHERE ixBug=" + caseId,
                        connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader.GetInt32(0) == 1)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
        }


        public static DateTime? GetCaseCloseDateTime(int caseId)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                MySqlCommand command =
                    new MySqlCommand(
                        "SELECT dtClosed FROM Bug WHERE ixBug=" + caseId,
                        connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            return reader.GetDateTime(0);
                        }
                    }

                    return null;
                }
            }
        }


        public static void CloseDelayWarnings()
        {
            SupportCase[] cases = GetDelayWarnings();

            foreach (SupportCase @case in cases)
            {
                CloseWithComment(@case.Identity, "No action taken on delay warnings. Case closed.");
            }
        }


        public static SupportEmail[] GetRecentOutgoingEmails(int sinceEventId)
        {
            List<SupportEmail> result = new List<SupportEmail>();

            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                MySqlCommand command =
                    new MySqlCommand(
                        "SELECT BugEvent.ixBugEvent AS BugEventId,BugEvent.ixBug AS BugId, Bug.sTitle as BugTitle, Bug.sCustomerEmail AS Recipient, BugEvent.s AS Body,Person.sFullName AS Sender FROM BugEvent,Bug,Person WHERE BugEvent.sVerb='Replied' AND BugEvent.fEmail=1 AND Person.ixPerson=BugEvent.ixPerson AND Bug.ixBug = BugEvent.ixBug AND BugEvent.ixBugEvent > " +
                        sinceEventId, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(ReadSupportEmailFromReader(reader));
                    }
                }
            }

            return result.ToArray();
        }


        public static string GetFirstEventText(int bugId)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                MySqlCommand command =
                    new MySqlCommand(
                        "SELECT s FROM BugEvent WHERE ixBug = " + bugId + " ORDER BY ixBugEvent LIMIT 1",
                        connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (string) reader[0];
                    }

                    return string.Empty;
                }
            }
        }


        private static SupportEmail ReadSupportEmailFromReader(MySqlDataReader reader)
        {
            SupportEmail email = new SupportEmail();

            //  BugEvent.ixBugEvent AS BugEventId,BugEvent.ixBug AS BugId, Bug.sTitle as BugTitle, Bug.sCustomerEmail AS Recipient, BugEvent.s AS Body,Person.sFullName AS Sender

            email.CaseId = reader.GetInt32(1);
            email.Body = Encoding.UTF8.GetString(Encoding.GetEncoding(1252).GetBytes(reader.GetString(4)));
            email.EventId = reader.GetInt32(0);
            email.From = reader.GetString(5);
            email.To = reader.GetString(3);
            email.CaseTitle = reader.GetString(2);

            return email;
        }


        private static SupportCase ReadFullSupportCaseFromReader(MySqlDataReader reader)
        {
            // ixBug,sTitle,sCustomerEmail

            int identity = reader.GetInt32(0);
            string title = reader.GetString(1);
            string email = string.Empty;

            if (!reader.IsDBNull(2))
            {
                email = reader.GetString(2);
            }

            return new SupportCase(identity, title, email);
        }


        private static SupportCase ReadSupportCaseFromReader(MySqlDataReader reader)
        {
            // ixBug,sTitle

            int identity = reader.GetInt32(0);
            string title = reader.GetString(1);

            return new SupportCase(identity, title);
        }


        private static SupportCaseDelta ReadSupportCaseDeltaFromReader(MySqlDataReader reader)
        {
            // ixBug,sTitle

            int deltaId = reader.GetInt32(0);
            int caseId = reader.GetInt32(1);
            int personId = reader.GetInt32(2);
            DateTime dateTime = reader.GetDateTime(3);
            string verb = reader.GetString(4);
            string changes = reader.GetString(5);


            return new SupportCaseDelta(deltaId, caseId, personId, dateTime, verb, changes);
        }


        internal static void CloseWithComment(int bugId, string comment)
        {
            using (MySqlConnection connection = GetConnection())
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("CloseWithComment", connection);
                command.Parameters.AddWithValue("bugId", bugId);
                command.Parameters.AddWithValue("comment", comment);
                command.Parameters.AddWithValue("dateTime", DateTime.Now);

                command.CommandType = CommandType.StoredProcedure;

                command.ExecuteNonQuery();
            }
        }


        public static void NotifyBouncingEmails()
        {
            Regex regex = new Regex(@"(?<email>[a-z_0-9\.\+\-]+\@[a-z_0-9\.\-]+)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string SMSBouncingEmail = ("" + Persistence.Key["SMSBouncingEmail"]).ToUpper().Trim();
            Regex reIsNo = new Regex(@"(NO)|(N)|(NEJ)", RegexOptions.IgnoreCase);
            Boolean dontSendSMS = reIsNo.IsMatch(SMSBouncingEmail);
            Boolean holdSMS = SMSBouncingEmail == "HOLD";
            int lastCaseHandled = 0;

            if (PhoneMessageTransmitter.CheckServiceStatus() == false)
                dontSendSMS = true;

            SupportCase[] cases = null;
            while (lastCaseHandled == 0 || (cases != null && cases.Length > 0))
            {
                cases = GetUndeliverableCases(lastCaseHandled, 100);

                if (cases.Length == 0) // very very important bugfix
                {
                    break;
                }

                foreach (SupportCase @case in cases)
                {
                    lastCaseHandled = @case.Identity;
                    bool attemptRecovery = true;

                    if (@case.Title.ToLower().Contains("ditt medlemskap har"))
                    {
                        // "Ditt medlemskap har gått ut"

                        attemptRecovery = false;
                    }

                    if (@case.Title.ToLower().Contains("press"))
                    {
                        // Press release

                        attemptRecovery = false;
                    }

                    if (attemptRecovery)
                    {
                        string body = GetFirstEventText(@case.Identity);

                        // Strip the mail header

                        int bodySeparator = body.IndexOf("\r\n\r\n");

                        if (bodySeparator > 0)
                        {
                            body = body.Substring(bodySeparator + 4);
                        }
                        else
                        {
                            body = string.Empty;
                        }

                        Match match = regex.Match(body);
                        if (match.Success)
                        {
                            string email = match.Groups["email"].Value;
                            People people = People.FromEmail(email);

                            if (people.Count > 2)
                            {
                                CloseWithComment(@case.Identity,
                                    "Bounced message closed. More than one person matches the email address. Both marked unreachable.");

                                foreach (Person person in people)
                                {
                                    person.MailUnreachable = true;
                                }
                            }

                            else if (people.Count < 1)
                            {
                                CloseWithComment(@case.Identity,
                                    "Bounced message closed. No person in the database matches the email address.");
                            }

                            else
                            {
                                // When we get here, there is exactly one person matching in the database

                                Person person = people[0];

                                bool hasActiveMemberships = false;

                                Memberships memberships = person.GetMemberships();

                                foreach (Membership membership in memberships)
                                {
                                    if (membership.Active)
                                    {
                                        hasActiveMemberships = true;
                                    }
                                }

                                if (hasActiveMemberships)
                                {
                                    // Attempt to contact by SMS.
                                    if (!holdSMS)
                                    {
                                        if (person.Phone.Trim().Length > 2)
                                        {
                                            if (dontSendSMS)
                                            {
                                                CloseWithComment(@case.Identity,
                                                    "The person at phone# " + person.Phone + ", " + person.Name +
                                                    " (#" + person.Identity +
                                                    "), was not contacted due to that SMS notification is currently turned off. Bounced message closed.");
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    person.SendPhoneMessage(
                                                        "Piratpartiet: den mailadress vi har till dig (" +
                                                        person.Mail +
                                                        ") studsar. Kontakta medlemsservice@piratpartiet.se med ny adress.");
                                                    CloseWithComment(@case.Identity,
                                                        "Successfully notified the member at phone# " + person.Phone +
                                                        ", " + person.Name + " (#" + person.Identity +
                                                        "), about the bounced email using an SMS message. Case closed.");
                                                }
                                                catch (Exception)
                                                {
                                                    CloseWithComment(@case.Identity,
                                                        "The person at phone# " + person.Phone + ", " + person.Name +
                                                        " (#" + person.Identity +
                                                        "), could not be reached over SMS. This member has been marked unreachable. Bounced message closed.");
                                                    person.MailUnreachable = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            CloseWithComment(@case.Identity,
                                                person.Name + " (#" + person.Identity +
                                                ") does not have a listed phone number. This member has been marked unreachable. Bounced message closed.");
                                            person.MailUnreachable = true;
                                        }
                                    }
                                }
                                else
                                {
                                    CloseWithComment(@case.Identity,
                                        "The person at phone# " + person.Phone + ", " + person.Name + " (#" +
                                        person.Identity +
                                        ") has no active memberships. Bounced message closed.");
                                }
                            }
                        }
                        else
                        {
                            CloseWithComment(@case.Identity,
                                "Bounced message closed. No email address could be located.");
                        }
                    }
                    else
                    {
                        CloseWithComment(@case.Identity, "Bounced message closed without attempt of recovery.");
                    }
                }
            }
        }
    }
}