using System;
using System.Globalization;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Payload;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Utility.Communications
{
    public class CommsTransmitterMail : ICommsTransmitter
    {
        // To be implemented better

        private static string _smtpServerCache = string.Empty;
        private static int _smtpPortCache = 25;
        private static string _smtpUserCache = string.Empty;
        private static string _smtpPasswordCache = string.Empty;
        private static DateTime _cacheReloadTime = DateTime.MinValue;

        public void Transmit (PayloadEnvelope envelope, Person person)
        {
            // Create the renderer via reflection of the static FromXml method

            Assembly assembly = typeof(PayloadEnvelope).Assembly;

            Type payloadType = assembly.GetType(envelope.PayloadClass);
            if (payloadType == null)
            {
                NotificationCustomStrings customStrings = new NotificationCustomStrings();
                customStrings["UnrecognizedPayloadType"] = envelope.PayloadClass;
                OutboundComm.CreateNotification (null, NotificationResource.System_UnrecognizedPayload, customStrings);

                throw new OutboundCommTransmitException("Unrecognized or uninstantiable payload type: " + envelope.PayloadClass);
            }

            var methodInfo = payloadType.GetMethod("FromXml", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            ICommsRenderer renderer = (ICommsRenderer)(methodInfo.Invoke(null, new object[] { envelope.PayloadXml }));
            RenderedComm comm = renderer.RenderComm (person);

            MailMessage mail = new MailMessage();

            // This is a rather simple mail (no images or stuff like that)

            mail.Subject = (string)comm[CommRenderPart.Subject];
            mail.SubjectEncoding = Encoding.UTF8;

            try
            {
                mail.From = new MailAddress ((string) comm[CommRenderPart.SenderMail], (string) comm[CommRenderPart.SenderName],
                    Encoding.UTF8);

                // SPECIAL CASE for sandbox mails -- ugly code but wtf
                if (person.Identity == 1 && PilotInstallationIds.DevelopmentSandbox == SystemSettings.InstallationId && mail.Subject.Contains("|"))
                {
                    string[] separated = mail.Subject.Split('|');
                    mail.Subject = separated[1];
                    mail.To.Add(new MailAddress("test@falkvinge.net", "Swarmops Sandbox Administrator"));
                }
                else // regular case to be used... like everywhere else except for the sandbox test
                {
                    if (string.IsNullOrWhiteSpace(person.Mail))
                    {
                        throw new OutboundCommTransmitException("No valid mail address for " + person.Canonical);
                    }

                    mail.To.Add(new MailAddress(person.Mail, person.Name));
                }
            }
            catch (FormatException e)
            {
                // Address failure -- either sender or recipient

                _cacheReloadTime = DateTime.MinValue;
                throw new OutboundCommTransmitException ("Cannot send mail to " + person.Mail, e);
            }

            string mailBodyText = (string)comm[CommRenderPart.BodyText];
            mailBodyText = mailBodyText.Replace("[Addressee]", person.Canonical);

            string mailBodyHtml = comm.ContainsKey(CommRenderPart.BodyHtml)? (string)comm[CommRenderPart.BodyHtml]: string.Empty;
            mailBodyHtml = mailBodyHtml.Replace("[Addressee]", person.Canonical);

            mail.Body = mailBodyText;
            mail.BodyEncoding = Encoding.UTF8;

            string smtpServer = _smtpServerCache;
            int smtpPort = _smtpPortCache;
            string smtpUser = _smtpUserCache;
            string smtpPassword = _smtpPasswordCache;

            DateTime now = DateTime.Now;

            if (now > _cacheReloadTime)
            {
                smtpServer = _smtpServerCache = SystemSettings.SmtpHost;
                smtpPort = _smtpPortCache = SystemSettings.SmtpPort;
                smtpUser = _smtpUserCache = SystemSettings.SmtpUser;
                smtpPassword = _smtpPasswordCache = SystemSettings.SmtpPassword;
                
                _cacheReloadTime = now.AddMinutes (5);
            }

            if (string.IsNullOrEmpty (smtpServer))
            {
                smtpServer = "localhost";
                smtpPort = 25;
                // For development use only - invalidate cache instead of this, forcing re-reload
                _cacheReloadTime = DateTime.MinValue;
            }

            SmtpClient mailClient = new SmtpClient (smtpServer, smtpPort);
            if (!string.IsNullOrEmpty(smtpUser))
            {
                mailClient.Credentials = new System.Net.NetworkCredential(smtpUser, smtpPassword);
            }

            try
            {
                mailClient.Send (mail);
            }
            catch (Exception e)
            {
                _cacheReloadTime = DateTime.MinValue;
                throw new OutboundCommTransmitException ("Cannot send mail to " + person.Mail, e);
            }
        }
    }
}