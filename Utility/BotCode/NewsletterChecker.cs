using System;
using System.Collections.Generic;
using NRss;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Special.Sweden;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Utility.BotCode
{
    public class NewsletterChecker
    {
        const int ForumIdTestPost = 262;//vBulletin,  old forum was  326;
        const int ForumIdNewsletter = 342; //vBulletin,  old forum was 69;


        public class ReaderException : Exception
        {
            public ReaderException (string message, Exception innerException)
                : base(message, innerException)
            { }
        }
        static Dictionary<string, DateTime> feedErrorSignaled = new Dictionary<string, DateTime>();
        public static void Run ()
        {
            //TODO: This list should reside in database/by organisation and possibly geography.

            CheckOneBlog("http://rickfalkvinge.se/feed", 1);
            CheckOneBlog("http://christianengstrom.wordpress.com/feed", 5);
            CheckOneBlog("http://opassande.se/index.php/feed/", 367);
            CheckOneBlog("http://www.annatroberg.com/feed/", 11443);
        }

        private static void CheckOneBlog (string readerFeedUrl, int personId)
        {
            try
            {

                Person sender = Person.FromIdentity(personId);

                string senderName = sender.Name + " (Piratpartiet)";
                string senderAddress = sender.PartyEmail;


                if (personId == 5)
                {
                    senderName = "=?utf-8?Q?Christian_Engstr=C3=B6m_(Piratpartiet)?="; // compensate for Mono bug
                }
                if (personId == 1)
                {
                    senderAddress = "rick.falkvinge@piratpartiet.se";
                }

                RssReader reader = new RssReader(readerFeedUrl);
                People recipients = null;

                DateTime highWaterMark = DateTime.MinValue;
                string persistenceKey = "Newsletter-Highwater-" + personId.ToString();

                string highWaterMarkString = Persistence.Key[persistenceKey];

                try
                {
                    highWaterMark = DateTime.Parse(highWaterMarkString);
                }
                catch (FormatException)
                {
                    highWaterMark = DateTime.MinValue;
                }
                catch (Exception e)
                {
                    throw new ReaderException("feed:" + readerFeedUrl, e);
                }

                try
                {
                    Rss rss = reader.Read();

                    // TODO: Read the high water mark from db

                    foreach (RssChannelItem item in rss.Channel.Items)
                    {
                        // Ignore any items older than the highwater mark.

                        if (item.PubDate < highWaterMark)
                        {
                            continue;
                        }

                        // For each item, look for the "Nyhetsbrev" category.

                        bool publish = false;

                        foreach (RssCategory category in item.Categories)
                        {
                            if (category.Name.ToLowerInvariant() == "nyhetsbrev")
                            {
                                publish = true;
                            }
                        }


                        if (publish)
                        {
                            // Set highwater datetime mark. We do this first as a defense against mail floods, should something go wrong.

                            Persistence.Key[persistenceKey] = item.PubDate.AddMinutes(5).ToString();

                            // Verify that it was written correctly to database. This is defensive programming to avoid a mail flood.

                            if (DateTime.Parse(Persistence.Key[persistenceKey]) < item.PubDate)
                            {
                                throw new Exception(
                                    "Unable to commit new highwater mark to database in NewsletterChecker.Run()");
                            }

                            bool testMode = false;

                            if (item.Title.ToLower().Contains("test"))
                            {
                                // Newsletter blog entry contains "test" in title -> testmode
                                testMode = true;
                            }

                            // Post to forum

                            string forumText = Blog2Forum(item.Content);
                            string mailText = Blog2Mail(item.Content);
                            int forumPostId = 0;
                            try
                            {
                                forumPostId = SwedishForumDatabase.GetDatabase().CreateNewPost(
                                                                testMode ? ForumIdTestPost : ForumIdNewsletter, sender,
                                                                "Nyhetsbrev " + DateTime.Today.ToString("yyyy-MM-dd"),
                                                                item.Title, forumText);
                            }
                            catch (Exception ex)
                            {
                                if (!System.Diagnostics.Debugger.IsAttached)
                                {   // ignore when debugging
                                    throw ex;
                                }
                            }


                            // Establish people to send to, if not already done.

                            if (recipients == null || recipients.Count == 1)
                            {
                                recipients = People.FromNewsletterFeed(NewsletterFeed.TypeID.ChairmanBlog);
                            }

                            /*                            
                             *  Disabled sending to activists -- this was done leading up to the election in 2009
                             */
                            // Add activists (HACK)
                            // Should probably be better to select by organization, not geography.

                            /*
                            People activists = Activists.FromGeography(Country.FromCode("SE").Geography).People;

                            recipients = recipients.LogicalOr(activists);*/


                            // OVERRIDE: If this is a TEST newsletter, send ONLY to the originator

                            if (testMode)
                            {
                                recipients = People.FromSingle(sender);
                                item.Title += " [TEST MODE]";
                            }

                            //TODO: hardcoded Org & geo ... using PP & World
                            Organization org = Organization.PPSE;
                            Geography geo = Geography.Root;


                            NewsletterMail newslettermail = new NewsletterMail();

                            newslettermail.pSubject = item.Title;
                            newslettermail.pDate = DateTime.Now;
                            newslettermail.pForumPostUrl =
                                String.Format("http://vbulletin.piratpartiet.se/showthread.php?t={0}", forumPostId);
                            newslettermail.pBodyContent = Blog2Mail(item.Content);
                            newslettermail.pOrgName = org.MailPrefixInherited;


                            OutboundMail newMail = newslettermail.CreateOutboundMail(sender, OutboundMail.PriorityLow, org, geo);

                            int recipientCount = 0;
                            foreach (Person recipient in recipients)
                            {
                                if (!Formatting.ValidateEmailFormat(recipient.Mail)
                                    || recipient.MailUnreachable
                                    || recipient.NeverMail)
                                {
                                    continue;
                                }
                                ++recipientCount;
                                newMail.AddRecipient(recipient, false);
                            }

                            newMail.SetRecipientCount(recipientCount);
                            newMail.SetResolved();
                            newMail.SetReadyForPickup();
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                lock (feedErrorSignaled)
                {
                    if (!feedErrorSignaled.ContainsKey(readerFeedUrl) || feedErrorSignaled[readerFeedUrl].AddMinutes(60) < DateTime.Now)
                    {
                        feedErrorSignaled[readerFeedUrl] = DateTime.Now;
                        throw new ReaderException("NewsletterChecker got error " + ex.Message + "\n when checking feed:" + readerFeedUrl + "\n(feed will be continually checked, bu will not signal error again for an hour)", ex);
                    }
                }
            }
        }

        private static string Blog2Forum (string text)
        {
            return text.
                Replace("<h3>", "<h2>").
                Replace("</h3>", "</h2>").
                Replace("<blockquote>", "[quote]").
                Replace("</blockquote>", "[/quote]");
        }

        private static string Blog2Mail (string text)
        {
            return text.
                Replace("<h3>", "<h2>").
                Replace("</h3>", "</h2>").
                Replace("&#8220;", "\"").
                Replace("&#8221;", "\"").
                Replace("&#8216;", "'").
                Replace("&#8217;", "'").
                Replace("&#8212;", "--").
                Replace("&#8230;", "...");
        }
    }
}