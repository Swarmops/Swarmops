using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Diagnostics;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Pirates;
using System.Threading;
using Swarmops.Basic;
using Swarmops.Logic.Media;
using Swarmops.Logic.Support;
using Swarmops.Utility.Mail;

namespace Swarmops.Utility.BotCode
{
    public class MailProcessor
    {
        #region  Exception declarations
        public class RemoveRecipientException : Exception
        {
            public RemoveRecipientException (string message, Exception innerException)
                : base(message, innerException)
            { }
        }

        public class IgnoreRecipientException : Exception
        {
            public IgnoreRecipientException (string message, Exception innerException)
                : base(message, innerException)
            { }
        }

        public class InvalidRecipientException : IgnoreRecipientException
        {
            public InvalidRecipientException (string message, Exception innerException)
                : base(message, innerException)
            { }
        }

        public class RetryRecipientException : Exception
        {
            public RetryRecipientException (string message, Exception innerException)
                : base(message, innerException)
            { }
        }
        public class ReportAndRetryRecipientException : RetryRecipientException
        {
            public ReportAndRetryRecipientException (string message, Exception innerException)
                : base(message, innerException)
            { }
        }

        public class InvalidSenderException : Exception
        {
            public InvalidSenderException (string message, Exception innerException)
                : base(message, innerException)
            { }
        }
        #endregion

        static QuotedPrintable qpUTF8 = new QuotedPrintable(Encoding.UTF8);
        static QuotedPrintable qp8859 = new QuotedPrintable(Encoding.GetEncoding("ISO-8859-1"));
        static Dictionary<Encoding, QuotedPrintable> QuotedPrintableEncoder = InitQpDictionary();

        #region Static initialisation
        static private Dictionary<Encoding, QuotedPrintable> InitQpDictionary ()
        {
            Dictionary<Encoding, QuotedPrintable> qp = new Dictionary<Encoding, QuotedPrintable>();
            qp[Encoding.UTF8] = qpUTF8;
            qp[Encoding.GetEncoding("ISO-8859-1")] = qp8859;
            return qp;
        }

        #endregion

        public static void Run ()
        {
            // If there is mail in the outbound mail queue, do not add more. (This is a primitive
            // protection against dupes. Better will come that doesn't force idle time like this.)

            lock (lockObject)
            {
                if (mailQueueSize > 0)
                {
                    // return;
                }
            }

            // Look for a mail to process, or keep processing.
            OutboundMails mails = OutboundMails.GetTopUnprocessed(100);
            mailQueueSize = 0;

            if (mails.Count == 0)
            {
                // no unprocessed mail, everybody's happy
                return;
            }

            int maxBatchSize = 20; // Total number of recipients to process across all mails

            foreach (OutboundMail mail in mails)
            {

                // If we have already processed past our limit, return for now.
                if (maxBatchSize < 1)
                {
                    return;
                }

                // We are continuing to process, and starting with a fresh outbound mail.

                // Is this the first time we touch this mail?
                if (mail.StartProcessDateTime.Year < 2000)
                {
                    // Yes! Mark it as started and mail our author.
                    mail.StartProcessDateTime = DateTime.Now;

                    if (mail.MailType == (int)TypedMailTemplate.TemplateType.MemberMail
                        || mail.MailType == (int)TypedMailTemplate.TemplateType.OfficerMail) // TODO: Set special flag
                    {
                        string mailBody = string.Empty;

                        new MailTransmitter(
                            Strings.MailSenderName, Strings.MailSenderAddress, "Mail transmission begins: " + mail.Title,
                            mailBody, Person.FromIdentity(mail.AuthorPersonId), true).Send();
                    }
                }

                int batchSize = maxBatchSize;

                // If we are debugging, and stepping through this program, avoid the misery of many simultaneous threads.

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    batchSize = 1; // Do NOT multithread while debugging
                }

                while (true)
                {
                    if (!System.Diagnostics.Debugger.IsAttached)
                    {
                        HeartBeater.Instance.Beat();
                            //Tick the heartbeat to stop exernal restart if this takes a lot of time, but only if not debugging.
                    }

                    OutboundMailRecipients recipients = mail.GetNextRecipientBatch(batchSize);

                    maxBatchSize -= recipients.Count; // Decrement number of recipients to process on next mail, if any

                    if (recipients.Count == 0)
                    {
                        // This mail is complete!

                        mail.SetProcessed();

                        if (mail.MailType == (int)TypedMailTemplate.TemplateType.MemberMail
                            || mail.MailType == (int)TypedMailTemplate.TemplateType.OfficerMail) // TODO: Set special flag
                        {
                            string body = "Your mail has completed transmitting to " + mail.RecipientCount.ToString("#,##0") +
                                          " intended recipients. Out of these, " + mail.RecipientsFail.ToString("#,##0") +
                                          " failed because of invalid, empty, or otherwise bad e-mail addresses. These people have not received your message.\r\n";

                            new Mail.MailTransmitter(
                                Strings.MailSenderName, Strings.MailSenderAddress,
                                "Mail transmission completed: " + mail.Title, body, Person.FromIdentity(mail.AuthorPersonId),
                                true).Send();
                        }

                        if (maxBatchSize < 1)
                        {
                            return;
                        }

                        break; //the while loop
                    }
                    else
                    {
                        List<IAsyncResult> sendInProgress = new List<IAsyncResult>();
                        List<WaitHandle> waitHandlesList = new List<WaitHandle>();
                        foreach (OutboundMailRecipient recipient in recipients)
                        {
                            // Skip known invalid mail addresses
                            if (recipient.Person != null
                                && (recipient.Person.EMailIsInvalid || recipient.Person.MailUnreachable))
                            {
                                lock (lockObject)
                                {
                                    recipient.Delete();
                                    recipient.OutboundMail.IncrementFailures();
                                }

                                continue;
                            }

                            // Start the transmission process, asynchronously

                            lock (lockObject)
                            {
                                MailTransmissionDelegate asyncTransmitter = new MailTransmissionDelegate(TransmitOneMail);
                                MailTransmissionAsyncState asyncState = new MailTransmissionAsyncState();
                                asyncState.dlgt = asyncTransmitter;
                                asyncState.recipient = recipient;
                                IAsyncResult asyncResult = asyncTransmitter.BeginInvoke(recipient, new AsyncCallback(MailSent), asyncState);
                                sendInProgress.Add(asyncResult);
                                waitHandlesList.Add(asyncResult.AsyncWaitHandle);
                                mailQueueSize++;
                            }

                            System.Threading.Thread.Sleep(25); // Allow some time
                        }

                        // now wait for them to finish;
                        int numberStillExecuting = sendInProgress.Count;
                        int numberExecutingLast = numberStillExecuting + 1;
                        DateTime lastProgress = DateTime.Now;


                        while (numberStillExecuting > 0)
                        {
                            WaitHandle.WaitAny(waitHandlesList.ToArray(), 100, true);
                            lock (lockObject)
                            {
                                numberStillExecuting = 0;
                                waitHandlesList = new List<WaitHandle>();

                                for (int i = 0; i < sendInProgress.Count; ++i)
                                {
                                    IAsyncResult iares = sendInProgress[i];
                                    MailTransmissionAsyncState asyncState = (MailTransmissionAsyncState)iares.AsyncState;

                                    if (asyncState.dlgt != null)
                                    {
                                        if (!asyncState.callbackCompleted)
                                        {
                                            waitHandlesList.Add(iares.AsyncWaitHandle);
                                            numberStillExecuting++;
                                        }
                                        else
                                        {
                                            //Just finalised
                                            if (asyncState.exception != null)
                                            {
                                                if (asyncState.exception is RetryRecipientException)
                                                {
                                                    //Failed in sending due to some reason that can clear up by itself.
                                                    asyncState.dlgt = null;
                                                }
                                                else
                                                {
                                                    //Make sure recipient is deleted
                                                    try
                                                    {
                                                        asyncState.recipient.Delete();

                                                        // if RemoveRecipientException everything went ok except for the removal
                                                        if (asyncState.exception is RemoveRecipientException)
                                                            asyncState.recipient.OutboundMail.IncrementSuccesses();

                                                        asyncState.dlgt = null; // mark as done;
                                                        mailQueueSize--;

                                                    }
                                                    catch
                                                    {   //Keep looping until recipient removed
                                                        numberStillExecuting++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (numberExecutingLast != numberStillExecuting)
                                lastProgress = DateTime.Now;

                            numberExecutingLast = numberStillExecuting;

                            if (lastProgress.AddSeconds(60 * 2) < DateTime.Now)
                            {
                                // since last change, something must have hanged
                                lock (lockObject)
                                {
                                    mailQueueSize = -1000;
                                }
                                throw new Exception("Timeout in MailProcessor");
                            }
                        }
                    }
                }
            }
        }

        internal class MailTransmissionAsyncState
        {
            internal MailTransmissionDelegate dlgt = null;
            internal Exception exception = null;
            internal OutboundMailRecipient recipient = null;
            internal bool callbackCompleted = false;
        }

        internal delegate OutboundMailRecipient MailTransmissionDelegate (OutboundMailRecipient recipient);


        internal static OutboundMailRecipient TransmitOneMail (OutboundMailRecipient recipient)
        {
            try
            {
                // If the mail address in illegal format, do not try to send anything:
                if (!Formatting.ValidateEmailFormat(recipient.EmailPerson.Email.Trim()))
                {
                    string msg = "Invalid email address:\r\nEmailPerson [" + recipient.EmailPerson.Identity.ToString() + "], mail [" +
                        recipient.EmailPerson.Email + "]\r\nwill not send mail:" + recipient.OutboundMail.Title;
                    throw new InvalidRecipientException(msg, null);
                }

                // If the mail address is marked as unreachable, do not try to send anything
                if (recipient.Person != null && recipient.Person.MailUnreachable)
                {
                    string msg = "MailUnreachable email address:\r\nEmailPerson [" + recipient.EmailPerson.Identity.ToString() + "], mail [" +
                        recipient.EmailPerson.Email + "]\r\nwill not send mail:" + recipient.OutboundMail.Title;
                    throw new InvalidRecipientException(msg, null);
                }

                // If the mail address is marked as unreachable, do not try to send anything
                if (recipient.Person != null && recipient.Person.NeverMail)
                {
                    string msg = "NeverMail email address:\r\nEmailPerson [" + recipient.EmailPerson.Identity.ToString() + "], mail [" +
                        recipient.EmailPerson.Email + "]\r\nwill not send mail:" + recipient.OutboundMail.Title;
                    throw new IgnoreRecipientException(msg, null);
                }



                // Otherwise, let's start processing

                OutboundMail mail = recipient.OutboundMail;

                bool limitToLatin1 = false;
                bool limitToText = false;
                Encoding currentEncoding = Encoding.UTF8;

                string email = recipient.EmailPerson.Email.ToLower();


                if (mail.MailType == 0 || mail.TemplateName.EndsWith("Plain"))
                    limitToText = true;

                // TEST: Does this user require the use of a text-only message (as opposed to multipart/alternative)?
                if (recipient.Person != null && recipient.Person.LimitMailToText)
                {
                    limitToText = true;
                }

                // This is supposedly not a problem anymore
                //if (email.EndsWith("@hotmail.com") || email.EndsWith("@msn.com"))
                //{
                //    limitToLatin1 = true;
                //}

                // TEST: Does this user require the limited use of the Latin-1 charset (as opposed to Unicode?)
                if (recipient.Person != null && recipient.Person.LimitMailToLatin1)
                {
                    limitToLatin1 = true;
                }

                // Capability tests end here

                if (limitToLatin1)
                    currentEncoding = Encoding.GetEncoding("ISO-8859-1");
                else
                    currentEncoding = Encoding.UTF8;

                QuotedPrintable qp = QuotedPrintableEncoder[currentEncoding];


                MailMessage message = new MailMessage();

                if (mail.AuthorType == MailAuthorType.Person)
                {
                    try
                    {
                        message.From = new MailAddress(mail.Author.PartyEmail,
                                                       qp.EncodeMailHeaderString(mail.Author.Name + " (" + mail.Organization.MailPrefixInherited + ")"),
                                                       currentEncoding);

                        if (mail.Author.Identity == 1)
                        {
                            //TODO: Create alternative party mail optional data field, or organization chairman (based on roles) differently
                            // Ugly hack
                            message.From = new MailAddress("rick.falkvinge@piratpartiet.se",
                                                           qp.EncodeMailHeaderString(mail.Author.Name + " (" + mail.Organization.MailPrefixInherited + ")"),
                                                           currentEncoding);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSenderException("Invalid author address in MailProcessor.TransmitOneMail:" + (mail.AuthorPersonId).ToString() + ";" + mail.Author.PartyEmail, ex);
                    }

                }
                else
                {
                    try
                    {
                        FunctionalMail.AddressItem aItem = mail.Organization.GetFunctionalMailAddressInh(mail.AuthorType);
                        message.From = new MailAddress(aItem.Email,  qp.EncodeMailHeaderString(aItem.Name),
                                                        currentEncoding);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidSenderException("Unknown MailAuthorType in MailProcessor.TransmitOneMail:" + ((int)mail.AuthorType).ToString(), ex);
                    }
                }


                if (recipient.AsOfficer && recipient.Person != null)
                {
                    try
                    {
                        message.To.Add(new MailAddress(recipient.Person.PartyEmail,
                                            qp.EncodeMailHeaderString(recipient.Person.Name + " (" + mail.Organization.MailPrefixInherited + ")"),
                                            currentEncoding));
                    }
                    catch (FormatException e)
                    {
                        string msg = "Invalid officer email address:\r\nperson [" + recipient.Person.Identity.ToString() + "], mail [" +
                            recipient.Person.PartyEmail + "]\r\nwill not send mail:" + recipient.OutboundMail.Title;
                        throw new InvalidRecipientException(msg, e);
                    }
                }
                else
                {
                    try
                    {
                        message.To.Add(new MailAddress(recipient.EmailPerson.Email,
                                            qp.EncodeMailHeaderString(recipient.EmailPerson.Name),
                                            currentEncoding));
                    }
                    catch (FormatException e)
                    {
                        string msg = "Invalid email address:\r\nEmailPerson [" + recipient.EmailPerson.Identity.ToString() + "], mail [" +
                            recipient.EmailPerson.Email + "]\r\nwill not send mail:" + recipient.OutboundMail.Title;
                        throw new InvalidRecipientException(msg, e);
                    }
                }

                string culture = mail.Organization.DefaultCountry.Culture;

                // UGLY UGLY UGLY HACK, NEEDS TO CHANGE ASAP:
                // We need to determine the culture of the recipient in order to use the right template. However, this is also dependent on the text body, which needs to be
                // in the same culture. At this point, we don't have the mail/recipient cultures in the schema. This would be the correct solution.

                // The INCORRECT but working solution is to do as we do here and check if a) it's a reporter and b) the reporter has International/English as a category. If so,
                // we change the culture to en-US. It's an ugly as all hell hack but it should work as a temporary stopgap.

                if (recipient.Reporter != null)
                {
                    MediaCategories categories = recipient.Reporter.MediaCategories;

                    foreach (MediaCategory category in categories)
                    {
                        if (category.Name == "International/English")
                        {
                            culture = Strings.InternationalCultureCode;
                            break;
                        }
                    }
                }

                if (limitToText)
                {
                    // if just text, then just add a plaintext body;
                    string text = "";

                    //Cant really see any reson the HtmlAgilityPack shouldn't be thread safe, but what the heck, just in case..
                    lock (lockObject)
                    {
                        try
                        {
                            text = mail.RenderText(recipient.EmailPerson, culture);
                        }
                        catch (Exception ex)
                        {
                            throw new RemoveRecipientException("TextRendering failed for " + mail.Title + " to " + recipient.EmailPerson.Email + " will not retry.\n", ex);
                        }
                    }
                    message.BodyEncoding = currentEncoding;
                    message.Body = text;
                }
                else
                {
                    // otherwise, add a multipart/alternative with text and HTML
                    string text = "";
                    string html = "";

                    //Cant really see any reson the HtmlAgilityPack shouldn't be thread safe, but what the heck, just in case..
                    Exception ex = null;
                    lock (lockObject)
                    {
                        try
                        {
                            text = mail.RenderText(recipient.EmailPerson, culture);
                            html = mail.RenderHtml(recipient.EmailPerson, culture);
                        }
                        catch (Exception e)
                        {
                            ex = e;
                        }
                    }
                    if (text == "")
                        throw new RemoveRecipientException("Rendering (text) failed for " + mail.Title + " to " + recipient.EmailPerson.Email + " will not retry.\n", ex);
                    else if (html == "" || ex != null)
                        throw new RemoveRecipientException("Rendering (html) failed for " + mail.Title + " to " + recipient.EmailPerson.Email + " will not retry.\n", ex);

                    ContentType textContentType = new ContentType(MediaTypeNames.Text.Plain);
                    textContentType.CharSet = currentEncoding.BodyName;

                    ContentType htmlContentType = new ContentType(MediaTypeNames.Text.Html);
                    htmlContentType.CharSet = currentEncoding.BodyName;

                    AlternateView textView = null;
                    AlternateView htmlView = null;


                    if (limitToLatin1)
                    {
                        textView = new AlternateView(new MemoryStream(currentEncoding.GetBytes(text)), textContentType);
                        htmlView = new AlternateView(new MemoryStream(currentEncoding.GetBytes(text)), htmlContentType);
                    }
                    else
                    {
                        textView = AlternateView.CreateAlternateViewFromString(text, textContentType);
                        htmlView = AlternateView.CreateAlternateViewFromString(html, htmlContentType);
                    }

                    // A fucking stupid Mono bug forces us to transfer-encode in base64: it can't encode qp properly
                    // (the "=" is not encoded to "=3D")

                    htmlView.TransferEncoding = TransferEncoding.Base64;
                    textView.TransferEncoding = TransferEncoding.Base64;

                    // Add the views in increasing order of preference

                    message.AlternateViews.Add(textView);
                    message.AlternateViews.Add(htmlView);
                }

                if (mail.AuthorType == MailAuthorType.PirateWeb)
                {
                    message.Subject = mail.Title;
                }
                else if (mail.MailType == 0)
                {
                    message.Subject = mail.Organization.MailPrefixInherited + ": " + mail.Title;
                }
                else
                {
                    //Title is set up in template processing in OutboundMail rendering.
                    message.Subject = mail.Title;
                }

                message.SubjectEncoding = currentEncoding;

                string smtpServer = ConfigurationManager.AppSettings["SmtpServer"];

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debug.WriteLine("sending " + message.Subject + " to " + recipient.EmailPerson.Email);
                    System.Threading.Thread.Sleep(200); //simulate delay
                }


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
                        if (e.ToString().StartsWith("System.Net.Mail.SmtpException: 4"))
                        {
                            // Temporary error (SMTP 4xx). Try again.

                            System.Threading.Thread.Sleep(2000); // Allow 2 seconds pause to wait for smtp-server to become available
                            throw new ReportAndRetryRecipientException("Temporary smtp error, will retry.", e);
                        }

                        // Otherwise, bad recipient (assume so). Have the mail removed from the queue.

                        List<string> recipients = new List<string>();
                        foreach (MailAddress address in message.To)
                        {
                            recipients.Add(address.Address);
                        }

                        ExceptionMail.Send(new ArgumentException("Bad Recipients when sending to " + recipient.EmailPerson.Email + ": " + String.Join(", ", recipients.ToArray()), e));

                        if (mail.AuthorType == MailAuthorType.Person)
                        {
                            try
                            {
                                mail.Author.SendOfficerNotice("Failed recipient(s): " + String.Join(", ", recipients.ToArray()),
                                                              "Some recipients failed inexplicably in a mail from you.", 1);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Failed to SendOfficerNotice to :" + mail.AuthorPersonId.ToString(), ex);
                            }
                        }
                    }
                }
                return recipient; // To pass this object onto the we're-done callback
            }
            catch (InvalidRecipientException ex)
            {

                throw ex;
            }
            catch (RetryRecipientException ex)
            {
                System.Threading.Thread.Sleep(2000); // Allow 2 seconds pause to avoid flooding the errorlog too fast in case of a permanent failure
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        internal static void MailSent (IAsyncResult result)
        {
            lock (lockObject)
            {

                MailTransmissionAsyncState asyncState = (MailTransmissionAsyncState)result.AsyncState;
                MailTransmissionDelegate asyncTransmitter = asyncState.dlgt;
                OutboundMailRecipient recipient = asyncState.recipient;
                try
                {
                    asyncTransmitter.EndInvoke(result);

                    try
                    {
                        recipient.Delete();
                        recipient.OutboundMail.IncrementSuccesses();
                    }
                    catch (Exception ex)
                    {
                        // whatever.. The mail is sent, but we couldn't mark it as such, probably a database failure.
                        asyncState.exception = new RemoveRecipientException("Couldn't remove mail recipient.", ex);
                        ExceptionMail.Send(asyncState.exception, true);
                    }
                }
                catch (ReportAndRetryRecipientException e)
                {
                    asyncState.exception = e;
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    ExceptionMail.Send(e, true);
                }
                catch (RetryRecipientException e)
                {
                    // No action - handled in async delegate - retry this mail 
                    asyncState.exception = e;
                }
                catch (Exception e)
                {
                    asyncState.exception = e;

                    try
                    {
                        recipient.Delete();
                        recipient.OutboundMail.IncrementFailures();

                        if (e is InvalidRecipientException)
                        {
                            if (recipient.Person != null && recipient.Person.EMailIsInvalid == false)
                            {
                                //Mark address as invalid.
                                recipient.Person.EMailIsInvalid = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Return this exception in asyncState since it is important to stop flooding.
                        asyncState.exception = new RemoveRecipientException("Couldn't remove mail recipient after exception.", ex);
                        ExceptionMail.Send(asyncState.exception, true); //Report the secondary exception
                    }



                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    ExceptionMail.Send(e, true);
                }

                asyncState.callbackCompleted = true;
            }
        }

        private static int mailQueueSize;
        private static Object lockObject = new Object();
    }
}