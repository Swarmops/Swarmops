using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Swarmops.Basic.Types;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Support;

namespace Swarmops.Utility.Mail
{
    public class NewsletterTransmitter2
    {
        static QuotedPrintable qpUTF8 = new QuotedPrintable(Encoding.UTF8);
        static QuotedPrintable qp8859 = new QuotedPrintable(Encoding.GetEncoding("ISO-8859-1"));

        public NewsletterTransmitter2 (string title, string htmlTemplateFile, string textTemplateFile, People sendList)
        {
            this.title = title;
            this.htmlTemplateFile = htmlTemplateFile;
            this.textTemplateFile = textTemplateFile;
            this.sendList = sendList;
        }

        public NewsletterTransmitter2()
        {
            // This constructor will basically make a nonfunctioning instance, since the fields are readonly
        }


        public void Send()
        {
            int indexLastBackslash = htmlTemplateFile.LastIndexOf('\\');

            string directory = htmlTemplateFile.Substring(0, indexLastBackslash);
            string nakedFileName = htmlTemplateFile.Substring(indexLastBackslash + 1);

            string htmlTemplate = "Failed to read HTML mail template.";
            string textTemplate = "Failed to read plaintext mail template.";
            string intro = "Failed to read intro.";
            string mailBody = "Failed to read mail body.";

            using (StreamReader reader = new StreamReader(htmlTemplateFile, System.Text.Encoding.Default))
            {
                htmlTemplate = reader.ReadToEnd();
            }

            using (StreamReader reader = new StreamReader(textTemplateFile, System.Text.Encoding.Default))
            {
                textTemplate = reader.ReadToEnd();
            }

            using (StreamReader reader = new StreamReader(directory + "\\" + "intro.txt", System.Text.Encoding.Default))
            {
                intro = reader.ReadToEnd();
            }

            using (StreamReader reader = new StreamReader(directory + "\\" + "body.txt", System.Text.Encoding.Default))
            {
                mailBody = reader.ReadToEnd();
            }


            // PREPARE HTML VIEW:

            // Write in the title, intro, and body

            htmlTemplate = htmlTemplate.
                Replace("%TITLE%", HttpUtility.HtmlEncode(title.ToUpper())).
                Replace("%title%", HttpUtility.HtmlEncode(title)).
                Replace("%intro%", HtmlEncode(intro)).
                Replace("%body%", HtmlEncode(mailBody));

            // Embed any inline images directly into the message

            string newsletterIdentifier = DateTime.Now.ToString("yyyyMMddhhMMssfff") + "@piratpartiet.se";

            htmlTemplate = htmlTemplate.Replace("Header-PP-Logo.png", "cid:pplogo" + newsletterIdentifier);


            /*
			htmlTemplate = htmlTemplate.Replace("TopRight.png", "cid:topright" + newsletterIdentifier);
			htmlTemplate = htmlTemplate.Replace("MiddleRight.png", "cid:middleright" + newsletterIdentifier);
			htmlTemplate = htmlTemplate.Replace("BottomRight.png", "cid:bottomright" + newsletterIdentifier);
			htmlTemplate = htmlTemplate.Replace("TopLeft.png", "cid:topleft" + newsletterIdentifier);
			htmlTemplate = htmlTemplate.Replace("MiddleLeft.png", "cid:middleleft" + newsletterIdentifier);
			htmlTemplate = htmlTemplate.Replace("BottomLeft.png", "cid:bottomleft" + newsletterIdentifier);
			htmlTemplate = htmlTemplate.Replace("TopMiddle.png", "cid:topmiddle" + newsletterIdentifier);
			htmlTemplate = htmlTemplate.Replace("BottomMiddle.png", "cid:bottommiddle" + newsletterIdentifier);
			htmlTemplate = htmlTemplate.Replace("PPShield.png", "cid:ppshield" + newsletterIdentifier);
			htmlTemplate = htmlTemplate.Replace("Rick.png", "cid:rick" + newsletterIdentifier);
			htmlTemplate = htmlTemplate.Replace("RFe-signature.gif", "cid:rick-signature" + newsletterIdentifier);*/


            MemoryStream memStream = new MemoryStream();

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlTemplate,
                                                                                 new ContentType(
                                                                                     MediaTypeNames.Text.Html));

            LinkedResource image = new LinkedResource(directory + "\\" + "Header-PP-Logo.png");
            image.ContentId = "pplogo" + newsletterIdentifier;
            htmlView.LinkedResources.Add(image);

            /*
			image = new LinkedResource(directory + "\\" + "MiddleRight.png");
			image.ContentId = "middleright" + newsletterIdentifier;
			htmlView.LinkedResources.Add(image);

			image = new LinkedResource(directory + "\\" + "BottomRight.png");
			image.ContentId = "bottomright" + newsletterIdentifier;
			htmlView.LinkedResources.Add(image);

			image = new LinkedResource(directory + "\\" + "TopLeft.png");
			image.ContentId = "topleft" + newsletterIdentifier;
			htmlView.LinkedResources.Add(image);

			image = new LinkedResource(directory + "\\" + "MiddleLeft.png");
			image.ContentId = "middleleft" + newsletterIdentifier;
			htmlView.LinkedResources.Add(image);

			image = new LinkedResource(directory + "\\" + "BottomLeft.png");
			image.ContentId = "bottomleft" + newsletterIdentifier;
			htmlView.LinkedResources.Add(image);

			image = new LinkedResource(directory + "\\" + "TopMiddle.png");
			image.ContentId = "topmiddle" + newsletterIdentifier;
			htmlView.LinkedResources.Add(image);

			image = new LinkedResource(directory + "\\" + "BottomMiddle.png");
			image.ContentId = "bottommiddle" + newsletterIdentifier;
			htmlView.LinkedResources.Add(image);

			image = new LinkedResource(directory + "\\" + "PPShield.png");
			image.ContentId = "ppshield" + newsletterIdentifier;
			htmlView.LinkedResources.Add(image);

			image = new LinkedResource(directory + "\\" + "Rick.png");
			image.ContentId = "rick" + newsletterIdentifier;
			htmlView.LinkedResources.Add(image);

			image = new LinkedResource(directory + "\\" + "RFe-Signature.gif");
			image.ContentId = "rick-signature" + newsletterIdentifier;
			htmlView.LinkedResources.Add(image);*/

            // PREPARE FORUM FILE:

            string forumTemplate = textTemplate.
                Replace("%TITLE%", "[h1]" + title.ToUpper() + "[/h1]").
                Replace("%title%", "[h1]" + title + "[/h1]").
                Replace("%intro%", intro).
                Replace("%body%", mailBody);

            // Replace "<a href="http://link">Text</a>" with "Text (http://link)"

            Regex regexLinksForum =
                new Regex("(?s)\\[a\\s+href=\\\"(?<link>[^\\\"]+)\\\"\\](?<description>[^\\[]+)\\[/a\\]",
                          RegexOptions.Multiline);

            forumTemplate = regexLinksForum.Replace(forumTemplate,
                                                    new MatchEvaluator(NewsletterTransmitter2.RewriteUrlsInForum));

            using (
                StreamWriter writer = new StreamWriter(directory + "\\" + "forum.txt", false,
                                                       System.Text.Encoding.Default))
            {
                writer.WriteLine(forumTemplate);
            }


            // PREPARE TEXT VIEW:

            // Write in the title, intro, and body

            textTemplate = textTemplate.
                Replace("%TITLE%", title.ToUpper()).
                Replace("%title%", title).
                Replace("%intro%", intro).
                Replace("%body%", mailBody);

            // Replace "<a href="http://link">Text</a>" with "Text (http://link)"

            Regex regexLinks = new Regex(
                "(?s)\\[a\\s+href=\\\"(?<link>[^\\\"]+)\\\"\\](?<description>[^\\[]+)\\[/a\\]", RegexOptions.Multiline);

            textTemplate = regexLinks.Replace(textTemplate, new MatchEvaluator(NewsletterTransmitter2.RewriteUrlsInText));

            Regex regexHtmlCodes = new Regex("(?s)\\[[^\\[]+\\]", RegexOptions.Multiline);

            textTemplate = regexHtmlCodes.Replace(textTemplate, string.Empty);

            ContentType typeUnicode = new ContentType(MediaTypeNames.Text.Plain);
            typeUnicode.CharSet = "utf-8";

            AlternateView textUnicodeView = AlternateView.CreateAlternateViewFromString(textTemplate, typeUnicode);

            ContentType typeIsoLatin1 = new ContentType(MediaTypeNames.Text.Plain);
            typeIsoLatin1.CharSet = "iso-8859-1";

            AlternateView textHotmailView = new AlternateView(
                new MemoryStream(Encoding.Default.GetBytes(textTemplate)), typeIsoLatin1);

            // Finally, send the views to all recipients.

            SendViewsToAllRecipients(htmlView, textUnicodeView, textHotmailView);
        }


        private static string RewriteUrlsInText (Match match)
        {
            return match.Groups["description"].Value + " (" + match.Groups["link"].Value + ")";
        }

        private static string RewriteUrlsInForum (Match match)
        {
            return "[url=" + match.Groups["link"].Value + "]" + match.Groups["description"].Value + "[/url]";
        }

        private void SendViewsToAllRecipients (AlternateView htmlView, AlternateView textUnicodeView,
                                               AlternateView textHotmailView)
        {
            int countTotal = sendList.Count;
            int count = 0;

            foreach (Person person in sendList)
            {
                count++;


                if (person.PersonId != 1)
                {
                    //continue;
                }

                Console.Write("{0:D4}/{1:D4} - ", count, countTotal);

                if (person.MailUnreachable)
                {
                    Console.WriteLine("UNREACHABLE");
                    continue;
                }

                if (person.Email.Length < 2)
                {
                    Console.WriteLine("NO EMAIL");
                    continue;
                }


                Console.WriteLine(person.Name);


                SmtpClient client = new SmtpClient("sparrow", 587);

                MailMessage message = new MailMessage(
                    new MailAddress("rick.falkvinge@piratpartiet.se", "Rick Falkvinge (Piratpartiet)"),
                    new MailAddress(person.Email, person.Name));

                message.Subject = "Piratpartiet: " + title;

                message.AlternateViews.Add(htmlView);

                string personEmail = person.Email.Trim().ToLower();

                string identifier = String.Format("{0:X4}", person.Identity);
                char[] array = identifier.ToCharArray();
                Array.Reverse(array);

                identifier = " [PP" + new string(array) + "]";

                message.Subject += identifier;

                if (personEmail.EndsWith("hotmail.com") || personEmail.EndsWith("msn.com"))
                {
                    message.AlternateViews.Add(textHotmailView);
                    message.Subject += " (anpassat till Hotmail)";
                    message.SubjectEncoding = Encoding.Default;
                }
                else
                {
                    message.AlternateViews.Add(textUnicodeView);
                }

                // EDIT THE BELOW LINE TO EDIT THE ATTACHMENT

                message.Attachments.Add(
                    new Attachment(@"C:\Documents and Settings\rick\Desktop\Piratpartiet Nyhetsbrev 2008-04-25.pdf"));


                client.Send(message);
            }
        }


        private string HtmlEncode (string input)
        {
            return HttpUtility.HtmlEncode(input).
                Replace("\r\n\r\n", "</p>\r\n\r\n<p>").
                Replace("[", "<").
                Replace("]", ">").
                Replace("&quot;", "\"").
                Replace("&amp;", "&");
        }

        private string title;

        private string htmlTemplateFile;
        private string textTemplateFile;
        private People sendList;
    }
}