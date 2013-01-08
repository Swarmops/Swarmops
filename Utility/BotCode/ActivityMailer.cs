using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;

namespace Swarmops.Utility.BotCode
{
    public class ActivityMailer
    {
        public static void Run()
        {
            // PPSE only, for now

            ExternalActivities activities = ExternalActivities.ForOrganization(Organization.PPSE);

            // TODO: HTML mailer for this (or TemplateMail)

            string body = string.Empty;

            body +=
                "<html><body>" +
                "<p>Veckans aktivism i Piratpartiet (ladda bilderna):</p>";

            DateTime cutoffDate = DateTime.Today.AddDays(-6);

            foreach (ExternalActivity activity in activities)
            {
                if (activity.DateTime > cutoffDate)
                {
                    body += "<img src=\"http://data.piratpartiet.se/Handlers/DisplayActivism.aspx?Id=" +
                            activity.Identity +
                            "\" width=\"600\" height=\"450\" Alt=\"Aktivism i " +
                            HttpUtility.HtmlEncode(activity.Geography.Name) + "\" /><br/><br/>";
                }
            }

            body +=
                "<p>F&ouml;r att logga aktivism d&auml;r du &auml;r funktion&auml;r, g&aring; till PirateWeb, Aktivism, <a href=\"https://pirateweb.net/Pages/v4/Activism/LogActivism.aspx\">Logga aktivism</a>. B&auml;sta valkretsen fram till valet vinner Fina Priser!</p>" +
                "<p>Denna sammanfattning skickas varje tisdag kl 10, s&aring; att alla hinner logga helgens aktivism.</p>" +
                "</body></html>";

            // foreach officer

            Roles roles = Roles.GetAll();
            Dictionary<int, bool> dupeCheck = new Dictionary<int, bool>();

            foreach (PersonRole role in roles)
            {
                if (role.OrganizationId == 1)
                {
                    Person person = role.Person;

                    if (person.MemberOf(Organization.PPSE) && person.PartyEmail.Length > 0 && !dupeCheck.ContainsKey(person.Identity))
                    {
                        dupeCheck[person.Identity] = true;

                        MailMessage message = new MailMessage();

                        message.To.Add(person.PartyEmail);
                        message.From = new MailAddress("noreply@pirateweb.net", "PirateWeb Activism");

                        message.IsBodyHtml = true;
                        message.Body = body;
                        message.Subject = "Veckans aktivism i Piratpartiet";

                        string smtpServer = ConfigurationManager.AppSettings["SmtpServer"];

                        if (smtpServer.ToLower() != "none")
                        {
                            if (smtpServer == null || smtpServer.Length < 2)
                            {
                                smtpServer = "localhost";
                            }

                            try
                            {
                                SmtpClient client = new SmtpClient(smtpServer, 25);
                                client.Send(message);
                            }
                            catch (SmtpException e)
                            {
                                // Basically ignore any exception

                                Console.WriteLine(e.Message);
                                
                            }
                        }
                    }
                }
            }

        }
    }
}
