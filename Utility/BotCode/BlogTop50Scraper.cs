using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using Activizr.Database;
using Activizr.Logic;

namespace Activizr.Utility.BotCode
{
    public class BlogTop50Scraper
    {
        public static void Run()
        {
            // Every hour of the day, we check that this has been recorded for the day. This is just in case
            // the bot isn't running at midnight, to make sure that we get one entry per day.

            if (PirateDb.GetDatabaseForReading().GetBlogTopList(DateTime.Today) == null)
            {
                ScrapeAndStore();
            }
        }

        private static void ScrapeAndStore()
        {
            string scrapeData = string.Empty;

            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create("http://knuff.se/topp50/knuffpoaeng/");
            request.UserAgent = "Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9b5) Gecko/2008050509 Firefox/3.0b5";

            using (Stream stream = request.GetResponse().GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(1252)))
                {
                    scrapeData = reader.ReadToEnd();
                }
            }

            Regex regex = new Regex("<h4 .*?><a href.*?>(?<blogname>.*?)</a>", RegexOptions.None);

            Match match = regex.Match(scrapeData);

            List<string> rankingList = new List<string>();
            while (match.Success)
            {
                string blogName = match.Groups["blogname"].Value;
                rankingList.Add(blogName);
                match = match.NextMatch();
            }

            PirateDb.GetDatabase().StoreBlogTopList(DateTime.Today, rankingList.ToArray());
        }
    }
}