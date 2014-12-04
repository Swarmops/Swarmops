using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Threading;
using Swarmops.Basic;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Utility.Mail
{
    public class MailTransmitter
    {
        private static readonly QuotedPrintable qpUTF8 = new QuotedPrintable(Encoding.UTF8);
        private static readonly QuotedPrintable qp8859 = new QuotedPrintable(Encoding.GetEncoding("ISO-8859-1"));
        private static int outstandingTransmissions;
        private static readonly object lockObject = new object();
        private readonly string bodyPlain;
        private readonly string fromAddress;
        private readonly string fromName;
        private readonly People recipients;
        private readonly string subject;
        private readonly bool toOfficers;

        static MailTransmitter()
        {
            outstandingTransmissions = 0;
        }

        public MailTransmitter(string fromName, string fromAddress, string subject, string bodyPlain, People recipients,
            bool toOfficers)
        {
            this.fromName = fromName;
            this.fromAddress = fromAddress;
            this.subject = subject;
            this.bodyPlain = bodyPlain;
            this.recipients = recipients;
            this.toOfficers = toOfficers;
        }

        public MailTransmitter(string fromName, string fromAddress, string subject, string bodyPlain, Person recipient)
            : this(fromName, fromAddress, subject, bodyPlain, People.FromArray(new[] {recipient}), false)
        {
        }

        public MailTransmitter(string fromName, string fromAddress, string subject, string bodyPlain, Person recipient,
            bool toOfficers)
            : this(fromName, fromAddress, subject, bodyPlain, People.FromArray(new[] {recipient}), toOfficers)
        {
        }

        public static bool CanExit
        {
            get
            {
                lock (lockObject)
                {
                    return outstandingTransmissions < 1;
                }
            }
        }

        public void Send()
        {
            //Send method now sending syncronously.

            //List to keep track of started async send's
            List<IAsyncResult> sendInProgress = new List<IAsyncResult>();

            foreach (Person recipient in this.recipients)
            {
                try
                {
                    MailMessage message = null;

                    if (this.toOfficers && recipient.Country.Identity == 1) // HACK until PirateWeb Exchange server up
                    {
                        if (!Formatting.ValidateEmailFormat(recipient.PartyEmail.Trim()))
                        {
                            if (recipient.Identity == 13354)
                            {
                                // HACK - UP auditor

                                continue;
                            }

                            Person.FromIdentity(1)
                                .SendOfficerNotice("PirateBot Warning",
                                    String.Format("The officer {0} (#{1}) does not have a party email.", recipient.Name,
                                        recipient.Identity), 1);

                            continue;
                        }

                        message =
                            new MailMessage(
                                new MailAddress(this.fromAddress, qp8859.EncodeMailHeaderString(this.fromName),
                                    Encoding.Default),
                                new MailAddress(recipient.PartyEmail,
                                    qp8859.EncodeMailHeaderString(recipient.Name + " (Piratpartiet)"),
                                    Encoding.Default));
                    }
                    else
                    {
                        if (!Formatting.ValidateEmailFormat(recipient.Mail.Trim()))
                        {
                            continue;
                        }

                        message =
                            new MailMessage(
                                new MailAddress(this.fromAddress, qpUTF8.EncodeMailHeaderString(this.fromName),
                                    Encoding.UTF8),
                                new MailAddress(recipient.Mail, qpUTF8.EncodeMailHeaderString(recipient.Name),
                                    Encoding.UTF8));
                    }

                    if (message == null)
                        continue;

                    message.Subject = this.subject;
                    message.Body = this.bodyPlain;

                    message.SubjectEncoding = Encoding.UTF8;
                    message.BodyEncoding = Encoding.UTF8;


                    // Start the transmission process, synchronously

                    #region Commented out asynchronous sending

                    //lock (lockObject)
                    //{
                    //    MailTransmissionDelegate asyncTransmitter = new MailTransmissionDelegate(TransmitOneMessage);
                    //    outstandingTransmissions++;
                    //    sendInProgress.Add(asyncTransmitter.BeginInvoke(message, null, asyncTransmitter));
                    //}
                    //System.Threading.Thread.Sleep(25); // Allow some time

                    #endregion

                    TransmitOneMessage(message); // Sending synchronosly
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                    ExceptionMail.Send(new Exception("Excepton in Mailtransmitter.Send:", e), true);
                }
            }

            #region Commented out handling of wait for asyncronous completion (Wich isn't even good...)

            //// now wait for them to finish;
            //int numberStillExecuting = sendInProgress.Count;
            //int numberExecutingLast = numberStillExecuting + 1;
            //DateTime lastProgress = DateTime.Now;

            //while (numberStillExecuting > 0)
            //{
            //    System.Threading.Thread.Sleep(25); // Allow some time

            //    numberStillExecuting = 0;
            //    for (int i = 0; i < sendInProgress.Count; ++i)
            //    {
            //        IAsyncResult iares = sendInProgress[i];
            //        if (iares != null)
            //        {
            //            if (!iares.IsCompleted)
            //            {
            //                numberStillExecuting++;
            //            }
            //            else
            //            {
            //                MessageSent(iares);
            //                sendInProgress[i] = null;
            //            }
            //        }
            //    }

            //    if (numberExecutingLast != numberStillExecuting)
            //        lastProgress = DateTime.Now;

            //    numberExecutingLast = numberStillExecuting;

            //    if (lastProgress.AddSeconds(60) < DateTime.Now)
            //    {
            //        //60 seconds since last change, something must have hanged
            //        lock (lockObject)
            //        {
            //            outstandingTransmissions = -1000;
            //        }
            //        throw new Exception("Timeout in MailTransmitter");
            //    }

            //}

            #endregion
        }

        internal void TransmitOneMessage(MailMessage message)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine("sending " + message.Subject);
                Thread.Sleep(500); //simulate delay
            }
            else
            {
                try
                {
                    SmtpClient client = new SmtpClient(Config.SmtpHost, Config.SmtpPort);
                    client.Credentials = null;
                    client.Send(message);
                }
                catch (SmtpFailedRecipientsException e)
                {
                    List<string> recipients = new List<string>();
                    foreach (MailAddress address in message.To)
                    {
                        recipients.Add(address.Address);
                    }

                    throw new ArgumentException(
                        "Bad Recipients: " + String.Join(", ", recipients.ToArray()) + " for mail with subject:" +
                        message.Subject, e);
                }
                catch (SmtpFailedRecipientException e)
                {
                    throw new ArgumentException(
                        "Bad Recipient: " + message.To[0].Address + " for mail with subject:" + message.Subject, e);
                }
                catch (SmtpException e)
                {
                    string msg = "Other smtp error:\r\n";
                    try
                    {
                        msg += string.Format(
                            "Subject:{0}\r\nSender:{1}"
                            , this.subject, this.fromAddress);
                    }
                    catch (Exception)
                    {
                    }
                    throw new ArgumentException(msg, e);
                }
            }
        }

        /// <summary>
        ///     Asynchronous callback method. Not in use.
        /// </summary>
        /// <param name="result"></param>
        internal static void MessageSent(IAsyncResult result)
        {
            MailTransmissionDelegate asyncTransmitter = (MailTransmissionDelegate) result.AsyncState;

            try
            {
                asyncTransmitter.EndInvoke(result);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                ExceptionMail.Send(e);
            }

            lock (lockObject)
            {
                outstandingTransmissions--;
            }
        }

        internal delegate void MailTransmissionDelegate(MailMessage message);
    }
}