using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Activizr.Basic;
using Activizr.Basic.Types;
using Activizr.Logic.Media;


namespace Activizr.Utility.Mail
{

    /// <summary>
    /// This class i deprecated. Pressreleases are sent using the Typed template mechanism.
    /// </summary>
    internal class PressReleaseTransmitter
    {
        public static void Send (string title, bool sendToAll, MediaCategories categories, string mailText,
                                 Reporters reporters)
        {
            string directory = "content" + Path.DirectorySeparatorChar + "pressreleasetemplate-1";

            string htmlTemplate = "Failed to read HTML mail template.";

            using (
                StreamReader reader = new StreamReader(directory + Path.DirectorySeparatorChar + "template.html",
                                                       System.Text.Encoding.GetEncoding(1252)))
            {
                htmlTemplate = reader.ReadToEnd();
            }

            /*
            using (StreamReader reader = new StreamReader(textTemplateFile, System.Text.Encoding.Default))
            {
                textTemplate = reader.ReadToEnd();
            }*/

            // PREPARE DATE

            // assume Swedish

            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("sv-SE");
            string date = DateTime.Today.ToString("d MMMM yyyy", culture);
            string time = DateTime.Now.ToString("HH:mm");

            string categoriesSentTo = "Alla kategorier";

            if (!sendToAll)
            {
                categoriesSentTo = GetConcatenatedCategoryString(categories);
            }

            // PREPARE HTML VIEW:

            // Write in the title, intro, and body

            htmlTemplate = htmlTemplate.
                Replace("%TITLE%", HttpUtility.HtmlEncode(title.ToUpper())).
                Replace("%title%", HttpUtility.HtmlEncode(title)).
                Replace("%date%", HttpUtility.HtmlEncode(date)).
                Replace("%DATE%", HttpUtility.HtmlEncode(date.ToUpper())).
                Replace("%time%", HttpUtility.HtmlEncode(time)).
                Replace("%postedtocategories%", HttpUtility.HtmlEncode(categoriesSentTo));

            string body = mailText;

            body = body.
                Replace("<", "({[(").
                Replace(">", ")}])").
                Replace("\"", "quotQUOTquot");

            body = HttpUtility.HtmlEncode(body);

            body = body.
                Replace("({[(", "<").
                Replace(")}])", ">").
                Replace("quotQUOTquot", "\"").
                Replace("&amp;#", "&#");

            htmlTemplate = htmlTemplate.Replace("%body%", body);

            // Embed any inline images directly into the message

            string newsletterIdentifier = DateTime.Now.ToString("yyyyMMddhhMMssfff") + "@piratpartiet.se";

            // TODO: Read the replacements from a config file

            htmlTemplate = htmlTemplate.Replace("header-pp-logo.png", "cid:pplogo" + newsletterIdentifier);


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


            // MemoryStream memStream = new MemoryStream();

            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlTemplate,
                                                                                 new ContentType(
                                                                                     MediaTypeNames.Text.Html));
            htmlView.TransferEncoding = TransferEncoding.Base64;

            /*
            LinkedResource image = new LinkedResource(directory + Path.DirectorySeparatorChar + "header-pp-logo.png");
            image.ContentId = "pplogo" + newsletterIdentifier;
            htmlView.LinkedResources.Add(image);*/


            /*
             * -- DISABLED -- code writing a forum-style text file
             * 

            // PREPARE FORUM FILE:

            string forumTemplate = textTemplate.
                Replace("%TITLE%", "[h1]" + title.ToUpper() + "[/h1]").
                Replace("%title%", "[h1]" + title + "[/h1]").
                Replace("%intro%", intro).
                Replace("%body%", mailBody);

            // Replace "<a href="http://link">Text</a>" with "Text (http://link)"

            Regex regexLinksForum = new Regex("(?s)\\[a\\s+href=\\\"(?<link>[^\\\"]+)\\\"\\](?<description>[^\\[]+)\\[/a\\]", RegexOptions.Multiline);

            forumTemplate = regexLinksForum.Replace(forumTemplate, new MatchEvaluator(NewsletterTransmitter2.RewriteUrlsInForum));

            using (StreamWriter writer = new StreamWriter(directory + "\\" + "forum.txt", false, System.Text.Encoding.Default))
            {
                writer.WriteLine(forumTemplate);
            }*/


            /*
             *  -- DISABLED -- no text view
             * 
             * 

            // PREPARE TEXT VIEW:

            // Write in the title, intro, and body

            textTemplate = textTemplate.
                Replace("%TITLE%", title.ToUpper()).
                Replace("%title%", title).
                Replace("%intro%", intro).
                Replace("%body%", mailBody);

            // Replace "<a href="http://link">Text</a>" with "Text (http://link)"

            Regex regexLinks = new Regex("(?s)\\[a\\s+href=\\\"(?<link>[^\\\"]+)\\\"\\](?<description>[^\\[]+)\\[/a\\]", RegexOptions.Multiline);

            textTemplate = regexLinks.Replace(textTemplate, new MatchEvaluator(NewsletterTransmitter2.RewriteUrlsInText));

            Regex regexHtmlCodes = new Regex("(?s)\\[[^\\[]+\\]", RegexOptions.Multiline);

            textTemplate = regexHtmlCodes.Replace(textTemplate, string.Empty);

            ContentType typeUnicode = new ContentType(MediaTypeNames.Text.Plain);
            typeUnicode.CharSet = "utf-8";

            AlternateView textUnicodeView = AlternateView.CreateAlternateViewFromString(textTemplate, typeUnicode);

            ContentType typeIsoLatin1 = new ContentType(MediaTypeNames.Text.Plain);
            typeIsoLatin1.CharSet = "iso-8859-1";

            AlternateView textHotmailView = new AlternateView(new MemoryStream(Encoding.Default.GetBytes(textTemplate)), typeIsoLatin1);
             * 
             * */

            foreach (Reporter reporter in reporters)
            {
                //Console.Write (recipient.Name + "(#" + recipient.Identity.ToString() + ")... ");

                try
                {
                    SmtpClient client = new SmtpClient(Config.SmtpHost, Config.SmtpPort);
                    client.Credentials = null;

                    MailMessage message = new MailMessage(
                        new MailAddress("press@piratpartiet.se", "Piratpartiet Press"),
                        new MailAddress(reporter.Email, reporter.Name, Encoding.UTF8));

                    message.Subject = "PP-Press: " + title;

                    string individualBody = htmlTemplate;

                    individualBody = individualBody.
                        Replace("%reportername%", HttpUtility.HtmlEncode(reporter.Name)).
                        Replace("%reporteremail%", HttpUtility.HtmlEncode(reporter.Email)).
                        Replace("%reportercategories%",
                                HttpUtility.HtmlEncode(GetConcatenatedCategoryString(reporter.MediaCategories)));

                    message.Body = individualBody;
                    message.BodyEncoding = Encoding.ASCII;
                    message.IsBodyHtml = true;

                    // COMPENSATE FOR MONO BUG -- put logo online instead of attached

                    message.Body = message.Body.Replace("cid:pplogo" + newsletterIdentifier,
                                                        "http://docs.piratpartiet.se/banners/newsletter-banner-pp-logo.png");

                    /*
                    Attachment attachment = new Attachment(directory + Path.DirectorySeparatorChar + "header-pp-logo.png", "image/png");
                    attachment.ContentId = "pplogo" + newsletterIdentifier;
                    attachment.ContentDisposition.Inline = true;
                    message.Attachments.Add(attachment);*/

                    bool successOrPermanentFail = false;

                    while (!successOrPermanentFail)
                    {
                        try
                        {
                            client.Send(message);
                            successOrPermanentFail = true;
                        }
                        catch (SmtpException e)
                        {
                            if (!(e.ToString().StartsWith("System.Net.Mail.SmtpException: 4")))
                            {
                                // This is NOT a temporary error (SMTP 4xx). Fail.

                                successOrPermanentFail = true;
                                throw e;
                            }

                            // Otherwise, sleep for a while and try again.

                            System.Threading.Thread.Sleep(1000);
                        }
                    }

                    //Console.WriteLine("ok");

                    //Console.WriteLine("ok");
                }
                catch (Exception e)
                {
                    //Console.WriteLine("FAIL! <" + recipient.Email + ">");
                    ExceptionMail.Send(
                        new Exception("Error sending press release to " + reporter.Name + " <" + reporter.Email + ">:",
                                      e),true);
                }
            }
        }

        private static string GetConcatenatedCategoryString (MediaCategories categories)
        {
            string result = categories[0].Name;

            for (int index = 1; index < categories.Count - 1; index++)
            {
                result += ", " + categories[index].Name;
            }

            if (categories.Count > 1)
            {
                result += " och " + categories[categories.Count - 1].Name;
            }

            return result;
        }
    }
}