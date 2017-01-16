using System;
using System.Collections.Generic;
using NRss;
using Swarmops.Common;
using Swarmops.Common.Enums;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Media;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Utility.BotCode
{
    public class PressReleaseChecker
    {
        [Obsolete ("Do not call this function until it's been generalized. Don't delete, though - generalize it.", true)
        ]
        public static void Run()
        {
            throw new NotImplementedException();

            //TODO: This list should reside in database/by organisation and possibly geography.

            /*
            CheckOneFeed("http://press.piratpartiet.se/feed", "PPSE", Organization.PPSEid);
            CheckOneFeed("http://presscenter.ungpirat.se/feed", "UPSE", Organization.UPSEid);
             */
        }

        private static void CheckOneFeed (string readerUrl, string persistAsKey, int orgIdForTemplate)
        {
            string persistenceKey = String.Format ("Pressrelease-Highwater-{0}", persistAsKey);

            DateTime highWaterMark = Constants.DateTimeLow;

            RssReader reader = null;

            try
            {
                string highWaterMarkString = Persistence.Key[persistenceKey];

                if (string.IsNullOrEmpty (highWaterMarkString))
                {
                    //Initialize highwatermark if never used
                    highWaterMark = DateTime.Now;
                    Persistence.Key[persistenceKey] = DateTime.Now.ToString();
                }
                else
                {
                    try
                    {
                        highWaterMark = DateTime.Parse (highWaterMarkString);
                    }
                    catch (Exception ex)
                    {
                        HeartBeater.Instance.SuggestRestart();
                        throw new Exception (
                            "Triggered restart. Unable to read/parse old highwater mark from database in PressReleaseChecker.Run(), from key:" +
                            persistenceKey + ", loaded string was '" + highWaterMarkString + "' expected format is " +
                            DateTime.Now, ex);
                    }
                }
                DateTime storedHighWaterMark = highWaterMark;
                reader = new RssReader (readerUrl);
                Rss rss = reader.Read();

                foreach (RssChannelItem item in rss.Channel.Items)
                {
                    // Ignore any items older than the highwater mark.
                    // Also ignore if older than two days

                    if (item.PubDate < highWaterMark || item.PubDate < DateTime.Now.AddDays (-2))
                    {
                        continue;
                    }

                    // This is an item we should publish.

                    // Set highwater datetime mark. We do this first, BEFORE processing, as a defense against mail floods,
                    // if should something go wrong and unexpected exceptions happen.

                    // We used to add 70 minutes as a defense against mistakes on DST switch in spring and fall (yes, it has happened), but have reduced to two.

                    if (item.PubDate > storedHighWaterMark)
                    {
                        Persistence.Key[persistenceKey] = item.PubDate.AddMinutes (2).ToString();
                        storedHighWaterMark = item.PubDate.AddMinutes (2);

                        // Verify that it was written correctly to database. This is defensive programming to avoid a mail flood,
                        // in case we can't write to the database for some reason.
                        string newStoredHighWaterString = "";
                        try
                        {
                            newStoredHighWaterString = Persistence.Key[persistenceKey];
                            DateTime temp = DateTime.Parse (newStoredHighWaterString);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception (
                                "Unable to commit/parse new highwater mark to database in PressReleaseChecker.Run(), loaded string was '" +
                                newStoredHighWaterString + "'", ex);
                        }

                        if (DateTime.Parse (Persistence.Key[persistenceKey]) < item.PubDate)
                        {
                            throw new Exception (
                                "Unable to commit new highwater mark to database in PressReleaseChecker.Run()");
                        }
                    }

                    bool allReporters = false;
                    bool international = false;
                    MediaCategories categories = new MediaCategories();

                    foreach (RssCategory category in item.Categories)
                    {
                        if (category.Name == "Alla")
                        {
                            allReporters = true;
                        }
                        else if (category.Name == "Uncategorized")
                        {
                        }
                        else
                        {
                            try
                            {
                                MediaCategory mediaCategory = MediaCategory.FromName (category.Name);
                                categories.Add (mediaCategory);

                                if (category.Name.StartsWith ("International"))
                                {
                                    international = true;
                                }
                            }
                            catch (Exception)
                            {
                                ExceptionMail.Send (
                                    new Exception ("Unrecognized media category in press release: " + category.Name));
                            }
                        }
                    }

                    string mailText = Blog2Mail (item.Content);

                    // Create recipient list of relevant reporters

                    Reporters reporters = null;

                    if (allReporters)
                    {
                        reporters = Reporters.GetAll();
                    }
                    else
                    {
                        reporters = Reporters.FromMediaCategories (categories);
                    }

                    // Add officers if not int'l

                    People officers = new People();
                    Dictionary<int, bool> officerLookup = new Dictionary<int, bool>();

                    if (!international)
                    {
                        int[] officerIds = Roles.GetAllDownwardRoles (1, 1);
                        foreach (int officerId in officerIds)
                        {
                            officerLookup[officerId] = true;
                        }
                    }
                    else
                    {
                        officerLookup[1] = true;
                    }


                    // Send press release

                    //TODO: hardcoded  geo ... using  World
                    Organization org = Organization.FromIdentity (orgIdForTemplate);
                    Geography geo = Geography.Root;
                    PressReleaseMail pressreleasemail = new PressReleaseMail();

                    pressreleasemail.pSubject = item.Title;
                    pressreleasemail.pDate = DateTime.Now;
                    pressreleasemail.pBodyContent = Blog2Mail (item.Content);
                    pressreleasemail.pOrgName = org.MailPrefixInherited;
                    if (allReporters)
                    {
                        pressreleasemail.pPostedToCategories = "Alla"; // TODO: TRANSLATE
                    }
                    else if (international)
                    {
                        pressreleasemail.pPostedToCategories = "International/English"; // TODO: THIS IS HARDCODED
                    }
                    else
                    {
                        pressreleasemail.pPostedToCategories =
                            PressReleaseMail.GetConcatenatedCategoryString (categories);
                    }

                    OutboundMail newMail = pressreleasemail.CreateFunctionalOutboundMail (MailAuthorType.PressService,
                        OutboundMail.PriorityHighest, org, geo);

                    int recipientCount = 0;
                    foreach (Reporter recipient in reporters)
                    {
                        if (!Formatting.ValidateEmailFormat (recipient.Email))
                        {
                            continue;
                        }
                        ++recipientCount;
                        newMail.AddRecipient (recipient);
                    }
                    foreach (int key in officerLookup.Keys)
                    {
                        Person recipient = Person.FromIdentity (key);
                        if (!Formatting.ValidateEmailFormat (recipient.Mail))
                        {
                            continue;
                        }
                        ++recipientCount;
                        newMail.AddRecipient (recipient, true);
                    }

                    newMail.SetRecipientCount (recipientCount);
                    newMail.SetResolved();
                    newMail.SetReadyForPickup();
                }
            }
            catch (Exception ex)
            {
                ExceptionMail.Send (
                    new Exception ("PressReleaseChecker failed:" + ex.Message + "\r\nwhen checking " + readerUrl, ex));
            }
            finally
            {
                reader.Close();
            }
        }

        private static string Blog2Mail (string text)
        {
            return text.
                Replace ("<h3>", "<h2>").
                Replace ("</h3>", "</h2>").
                Replace ("&#8220;", "\"").
                Replace ("&#8221;", "\"").
                Replace ("&#8216;", "'").
                Replace ("&#8217;", "'").
                Replace ("&#8212;", "--").
                Replace ("&#8230;", "...");
        }
    }
}