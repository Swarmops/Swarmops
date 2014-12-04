using System;
using System.Net;
using NRss;
using Swarmops.Logic.Media;

namespace Swarmops.Utility.BotCode
{
    public class MediaWatcher
    {
        public static void Run()
        {
            // Get keywords. Read Frisim. Parse. Store.

            string[] keywords = MediaEntries.GetOldMediaKeywords();

            foreach (string keyword in keywords)
            {
                // string rssUrl = String.Format("http://sesam.se/search/?c=m&q={0}&&output=rss", keyword.ToLower());

                string rssUrl = String.Format("http://www.frisim.com/rss/?q={0}&k=nyheter", keyword.ToLower());

                // Read the RSS URL into memory, then feed RssReader from a MemoryStream

                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(rssUrl);
                request.UserAgent = "Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9b5) Gecko/2008050509 Firefox/3.0b5";

                HttpWebResponse resp = (HttpWebResponse) request.GetResponse();
                RssReader reader = new RssReader(resp.GetResponseStream());

                try
                {
                    Rss rss = reader.Read();

                    foreach (RssChannelItem item in rss.Channel.Items)
                    {
                        // We want the title, media name, link and pubdate.

                        string url = item.Link;
                        DateTime dateTime = item.PubDate;
                        string title = item.Title;
                        string mediaName = item.Categories[0].Name;

                        MediaEntry.CreateFromKeyword(keyword, mediaName, false, url, title, dateTime);
                    }
                }
                catch (Exception e)
                {
                    throw new ReaderException("feed:" + rssUrl + "Status=" + resp.StatusCode, e);
                }

                finally
                {
                    request.GetResponse().GetResponseStream().Close();
                }
            }
        }

        public class ReaderException : Exception
        {
            public ReaderException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }
    }
}