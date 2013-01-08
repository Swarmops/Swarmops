using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

using System.IO;
using System.Data.Common;

using Swarmops.Logic;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Special.Mail
{
    // Have moved this Class from Utility to Logic to avoid circular dependency between assemblies /JL
    public class MailServerDatabase
    {
        /// <summary>
        /// class MailAccount is used for transport of results only
        /// </summary>
        public class MailAccount
        {
            public string account { get; set; }
            public List<string> forwardedTo = new List<string>();
            public List<string> forwardedFrom = new List<string>();

        }


        static MailServerDatabase ()
        {
            connectionString = Persistence.Key["MailServerDatabaseConnect"];
        }

        public static void AddAccount (string email, string initialPassword, int quotaMegabytes)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            "INSERT INTO users (email, password, quota) VALUES ('" + email.Replace("'", "''") +
                            "', ENCRYPT('" + initialPassword.Replace("'", "''") + "'), " +
                            (1048576 * quotaMegabytes).ToString() + ");", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<MailAccount> SearchAccount (string email)
        {
            //temporary test

            string sql = @"SELECT 
                           users.email,
                           forwardings.destination,
                           forwardings_1.source
                      FROM ( users
                            LEFT OUTER JOIN  forwardings
                               ON (users.email = forwardings.source))
                           LEFT OUTER JOIN forwardings forwardings_1
                              ON (forwardings_1.destination = users.email)
                            where email LIKE '" + email.Replace("'", "''") + "%'";

            Dictionary<string, MailAccount> foundAccounts = new Dictionary<string, MailAccount>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!foundAccounts.ContainsKey(reader["email"].ToString().ToLower()))
                                foundAccounts.Add(reader["email"].ToString().ToLower(), new MailAccount());

                            MailAccount curr = foundAccounts[reader["email"].ToString().ToLower()];

                            curr.account = reader["email"].ToString();
                            if (reader["destination"] != DBNull.Value
                                && !curr.forwardedTo.Contains(reader["destination"].ToString()))
                                curr.forwardedTo.Add(reader["destination"].ToString());
                            if (reader["source"] != DBNull.Value
                                && !curr.forwardedFrom.Contains(reader["source"].ToString()))
                                curr.forwardedFrom.Add(reader["source"].ToString());
                        }
                    }
                }
            }
            return new List<MailAccount>(foundAccounts.Values);
        }

        public static string FindFreeAccount (string email)
        {

            email = email.Trim();
            if (email == null || email.IndexOf("@") < 2)
            {
                throw new ArgumentOutOfRangeException("email too short");
            }

            email = email.Replace("'", "''");

            // First, try to find the exact address (Because I'm suspicious about performance of regexp searches in mysql.../JL)
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand("select * from users where email = '" + email + "'", connection))
                {
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {   //Address not found, it's OK to use
                            return email;
                        }
                    }
                }
                string[] splitDom = email.Split(new char[] { '@' });

                string[] splitArr = splitDom[0].Split(new char[] { '.' });

                string firstPart = splitArr[0];
                splitArr[0] = "";

                string secondPart = string.Join(".", splitArr, 1, splitArr.Length - 1);
                string secondPartRegEx = secondPart.Replace(".", "[.period.]");
                string domainRegex = splitDom[1].Replace(".", "[.period.]");


                // fetch all addresses that start the same as the one we are checking.
                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            "select * from users where email RLIKE " +
                            "'^" + firstPart + "([.period.][^[.period.]]+){0,1}[.period.]" + secondPartRegEx + "@" + domainRegex + "'",
                            connection))
                {
                    List<string> foundAddresses = new List<string>(); ;
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foundAddresses.Add(((string)reader["email"]).Trim().ToLowerInvariant());
                        }
                    }
                    string suggestMail = email;
                    char suggestChar = 'a';
                    char suggestChar2 = (char)('a' - 1);
                    while (foundAddresses.Contains(suggestMail.Trim().ToLowerInvariant()))
                    {
                        string suggestString = "" + suggestChar;
                        if (suggestChar > 'z')
                        {
                            ++suggestChar2;
                            suggestChar = 'a';
                        }
                        suggestString = "" + suggestChar;
                        if (suggestChar2 >= 'a')
                            suggestString += suggestChar2;

                        suggestMail = firstPart + "." + suggestString + "." + secondPart + "@" + splitDom[1];
                        ++suggestChar;

                    }
                    return suggestMail;
                }
            }
        }


        public static void SetNewPassword (string email, string newPassword)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            "UPDATE users SET password=ENCRYPT('" + newPassword.Replace("'", "''") +
                            "') WHERE email='" + email.Replace("'", "''") + "';", connection))
                {
                    command.ExecuteNonQuery();
                }

            }
        }

        public static void StartForwarding (string fromEmail, string toEmail)
        {
            fromEmail = fromEmail.Trim().Replace("'", "''");
            toEmail = toEmail.Trim().Replace("'", "''");

            if (fromEmail == null || fromEmail.IndexOf("@") < 2)
            {
                throw new ArgumentOutOfRangeException("fromEmail too short");
            }

            if (toEmail == null || toEmail.IndexOf("@") < 2)
            {
                throw new ArgumentOutOfRangeException("toEmail too short");
            }


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                //follow forwarding chain until end or until loop found
                Dictionary<string, string> loopDict = new Dictionary<string, string>();

                // this will be created, if no loop is found, 
                // so test for it when finding loop
                loopDict.Add(fromEmail, toEmail);

                string loopEmail = toEmail;

                while (loopEmail != "" && !loopDict.ContainsKey(loopEmail))
                {
                    using (MySqlCommand command = new MySqlCommand(
                        "select * from forwardings where source = '" + loopEmail + "'", connection))
                    {
                        using (DbDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string tmpDest = ((string)reader["destination"]).Trim().ToLower();
                                loopDict.Add(loopEmail, tmpDest);
                                loopEmail = tmpDest;
                            }
                            else
                            {   //Address not found
                                loopEmail = "";
                            }
                        }
                    }
                }

                if (loopEmail != "")
                {
                    //Found forwarding loop, throw exception msg
                    string msg = "Mail forwarding loop found:";
                    string lastadr = "";
                    foreach (string adr in loopDict.Keys)
                    {
                        lastadr = adr;
                        msg += " -> " + adr;
                    }
                    msg += " -> " + loopDict[lastadr];
                    throw new InvalidOperationException(msg);
                }


                using (MySqlCommand command = new MySqlCommand(
                    "DELETE FROM forwardings WHERE source='" + fromEmail + "';"
                    + "INSERT INTO forwardings (source, destination) VALUES ('" + fromEmail + "', '" + toEmail + "');",
                    connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void StopSpecificForwarding (string fromEmail, string toEmail)
        {
            fromEmail = fromEmail.Trim().Replace("'", "''");
            if (fromEmail == null || fromEmail.IndexOf("@") < 2)
            {
                throw new ArgumentOutOfRangeException("fromEmail too short");
            }

            toEmail = toEmail.Trim().Replace("'", "''");
            if (toEmail == null || toEmail.IndexOf("@") < 2)
            {
                throw new ArgumentOutOfRangeException("toEmail too short");
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            "DELETE FROM forwardings WHERE source='" + fromEmail + "' AND destination='" + toEmail +
                            "';", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void StopForwarding (string fromEmail)
        {
            fromEmail = fromEmail.Trim().Replace("'", "''");
            if (fromEmail == null || fromEmail.IndexOf("@") < 2)
            {
                throw new ArgumentOutOfRangeException("fromEmail too short");
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            "DELETE FROM forwardings WHERE source='" + fromEmail + "';", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteAccount (string fromEmail)
        {
            fromEmail = fromEmail.Trim().Replace("'", "''");
            if (fromEmail == null || fromEmail.IndexOf("@") < 2)
            {
                throw new ArgumentOutOfRangeException("Email too short");
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                    // Shortcut past the account that is to be removed
                           @"Update forwardings
                               INNER JOIN
                                  (SELECT DISTINCT
                                          forwardings.source,
                                          forwardings_1.destination AS newDestination,
                                          forwardings.destination AS orgDestination
                                     FROM   forwardings
                                          INNER JOIN
                                             forwardings forwardings_1
                                          ON (forwardings.destination = forwardings_1.source))
                                  Subquery
                               ON (forwardings.source = Subquery.source)
                               set destination=Subquery.newDestination
                               where Subquery.orgDestination='" + fromEmail + "';" +
                    // Delete where the removed account is source of forwarding
                            "DELETE FROM forwardings WHERE source='" + fromEmail + "';" +
                    // Delete where the removed account is target of forwarding
                            "DELETE FROM forwardings WHERE destination='" + fromEmail + "';" +
                    // Delete the user
                            "DELETE FROM users WHERE email='" + fromEmail + "';"
                            , connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static string connectionString = "";
    }
}