using System;
using System.Net.Mail;
using System.Text;
using Swarmops.Basic;
using Swarmops.Logic.Special.Sweden;
using Swarmops.Logic.Support;

namespace Swarmops.Utility.Special.Sweden
{
    public class SupportMailReview
    {
        public static void Run ()
        {
            string lastIndexString = Persistence.Key["LastSupportMailIndex"];

            int lastIndex = 110000;

            if (!String.IsNullOrEmpty(lastIndexString))
            {

                if (!Int32.TryParse(lastIndexString, out lastIndex))
                {
                    throw new Exception("Failed to read LastSupportMailIndex:" + lastIndexString);
                }
            }

            SupportEmail[] emails = SupportDatabase.GetRecentOutgoingEmails(lastIndex);

            int highIndex = lastIndex;

            foreach (SupportEmail email in emails)
            {
                if (email.EventId > highIndex)
                {
                    highIndex = email.EventId;
                }
            }

            if (highIndex > lastIndex)
            {
                Persistence.Key["LastSupportMailIndex"] = highIndex.ToString();

                // Verify that it was written correctly to database. This is defensive programming to avoid a mail flood.

                if (Persistence.Key["LastSupportMailIndex"] != highIndex.ToString())
                {
                    throw new Exception("Unable to commit new highwater mark to database in SupportMailReview.Run()");
                }
            }

            foreach (SupportEmail email in emails)
            {
                // add extra words to avoid arrayindex exceptions
                string[] stringTokens = (email.From + " unknown unknown").Split(' ');

                string subject = stringTokens[0].Substring(0, 1) + stringTokens[1].Substring(0, 1) + ", case " +
                                 email.CaseId.ToString() + ": " + email.CaseTitle;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(Strings.MailSenderAddress, Strings.MailSenderName);
                message.To.Add(new MailAddress("pp.se.mail@lists.pirateweb.net"));
                message.Subject = subject;
                message.Body = "Mail skickat av " + email.From + " till " + email.To + ":\r\n\r\n";
                message.BodyEncoding = Encoding.UTF8;

                message.Body += email.Body;

                SmtpClient client = new SmtpClient(Config.SmtpHost, Config.SmtpPort);
                client.Credentials = null;
                client.Send(message);
            }
        }
    }
}