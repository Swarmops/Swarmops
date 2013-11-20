using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Transmission;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Utility.Communications
{
    public class CommsTransmitterMail: ICommsTransmitter
    {
        // To be implemented

        private static string _smtpServerCache = string.Empty;
        private static DateTime _cacheReloadTime = DateTime.MinValue;

        public void Transmit(PayloadEnvelope envelope, Person person)
        {
            if (envelope.PayloadClass != "Swarmops.Logic.Communications.Transmission.NotificationPayload")
            {
                throw new NotImplementedException();
            }

            ICommsRenderer renderer = NotificationPayload.FromXml(envelope.PayloadXml);

            RenderedComm comm = renderer.RenderComm(person);

            MailMessage mail = new MailMessage();

            // This is a rather simple mail (no images or stuff like that)

            mail.From = new MailAddress(comm[CommRenderPart.SenderMail], comm[CommRenderPart.SenderName], Encoding.UTF8);
            mail.To.Add(new MailAddress(person.Mail, person.Name));

            mail.Subject = comm[CommRenderPart.Subject];
            mail.Body = comm[CommRenderPart.BodyText];
            mail.SubjectEncoding = Encoding.UTF8;
            mail.BodyEncoding = Encoding.UTF8;

            string smtpServer = _smtpServerCache;

            DateTime now = DateTime.Now;

            if (now > _cacheReloadTime)
            {
                smtpServer = _smtpServerCache = Persistence.Key ["SmtpServer"];
                _cacheReloadTime = now.AddMinutes(5);
            }

            if (string.IsNullOrEmpty(smtpServer))
            {
                smtpServer = "192.168.80.204"; // For development use only - invalidate cache instead of this, forcing re-reload
            }

            SmtpClient mailClient = new SmtpClient(smtpServer);

            // TODO: SMTP Server login credentials

            try
            {
                mailClient.Send(mail);
            }
            catch (Exception e)
            {
                _cacheReloadTime = DateTime.MinValue;
                throw new OutboundCommTransmitException("Cannot send mail to " + person.Mail, e);
            }
        }

    }
}
