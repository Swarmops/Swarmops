using System;
using System.Net;
using NRss;
using Swarmops.Logic.Media;

namespace Swarmops.Utility.BotCode
{
    public class BlogWatcher
    {
        public static void Run()
        {
            // Get keywords. Read Knuff. Parse. Store.

            string[] keywords = MediaEntries.GetBlogKeywords();

            foreach (string keyword in keywords)
            {
                string rssUrl = "http://knuff.se/rss/q/" + keyword;

                // Read the RSS URL into memory, then feed RssReader from a MemoryStream

                HttpWebRequest request = (HttpWebRequest) WebRequest.Create (rssUrl);
                request.UserAgent = "Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9b5) Gecko/2008050509 Firefox/3.0b5";
                HttpWebResponse resp = (HttpWebResponse) request.GetResponse();
                RssReader reader = new RssReader (resp.GetResponseStream());

                try
                {
                    Rss rss = reader.Read();

                    foreach (RssChannelItem item in rss.Channel.Items)
                    {
                        // We want the title, link and pubdate.

                        string url = item.Link;
                        DateTime dateTime = item.PubDate;

                        int dividerIndex = item.Title.LastIndexOf ('(');

                        if (item.Title.EndsWith ("))"))
                        {
                            dividerIndex = item.Title.Substring (0, item.Title.Length - 7).LastIndexOf ('(');
                        }

                        string title = item.Title.Substring (0, dividerIndex).Trim();
                        string blogName = item.Title.Substring (dividerIndex + 1, item.Title.Length - dividerIndex - 2);

                        bool newEntry = MediaEntry.CreateFromKeyword (keyword, blogName, true, url, title, dateTime);

                        if (newEntry)
                        {
                            try
                            {
                                PingCreeper (url);
                            }
                            catch (Exception)
                            {
                                // Ignore exceptions here, move on to the next entry
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ReaderException ("feed:" + rssUrl + ", Status=" + resp.StatusCode, e);
                }

                finally
                {
                    request.GetResponse().GetResponseStream().Close();
                }
            }
        }

        private static void PingCreeper (string referringUrl)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create ("http://gnuheter.com/creeper/image");
            request.UserAgent = "Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9b5) Gecko/2008050509 Firefox/3.0b5";
            request.Referer = referringUrl;

            request.GetResponse().GetResponseStream().Close();
        }

        public class ReaderException : Exception
        {
            public ReaderException (string message, Exception innerException)
                : base (message, innerException)
            {
            }
        }
    }
}