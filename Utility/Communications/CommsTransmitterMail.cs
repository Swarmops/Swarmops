using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Transmission;
using Swarmops.Logic.Swarm;

namespace Swarmops.Utility.Communications
{
    public class CommsTransmitterMail: ICommsTransmitter
    {
        // To be implemented

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

            mail.From = new MailAddress(comm[CommRenderPart.SenderMail], comm[CommRenderPart.SenderMail], Encoding.UTF8);
            mail.To.Add(new MailAddress(person.Email, person.Name));

            mail.Subject = comm[CommRenderPart.Subject];
            mail.Body = comm[CommRenderPart.Subject];
            mail.SubjectEncoding = Encoding.UTF8;
            mail.BodyEncoding = Encoding.UTF8;
            
            SmtpClient mailClient = new SmtpClient("192.168.80.204"); // Fix this - for development use only

            try
            {
                mailClient.Send(mail);
            }
            catch (Exception e)
            {
                throw new OutboundCommTransmitException("Cannot send mail to " + person.Email, e);  
            }
        }

    }
}
