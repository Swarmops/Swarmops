using System;
using System.Xml;

// Class to write out RSS
// Expected Usage:
//  rss = new RSSWriter(...);
//  rss.WriteHeader(...);
//    rss.WriteItem(..);
//    rss.WriteItem(..);
//    rss.WriteItem(..);
//  rss.Close();

// Validate feeds by URL: http://feedvalidator.org, or http://validator.w3.org/feed
// Code for RSS writer from http://blogs.msdn.com/jmstall  

namespace Swarmops.Interface
{
    public class RssWriter
    {
        private readonly XmlWriter m_writer;

        public RssWriter(XmlWriter tw)
        {
            this.m_writer = tw;
        }

        // Write header for RSS Feed
        // Parameters:
        // title - Pretty title of the RSS feed. Eg "My Pictures"
        // link - optional URL to link RSS feed back to a web page.
        // description - more verbose human-readable description of this feed.
        // generator - optional string for 'generator' tag.
        public void WriteHeader(string title, string link, string description, string generator)
        {
            this.m_writer.WriteStartElement("rss");
            this.m_writer.WriteAttributeString("version", "2.0");
            this.m_writer.WriteStartElement("channel");
            this.m_writer.WriteElementString("title", title);

            if (link != null)
            {
                this.m_writer.WriteElementString("link", link); // link to generated report.
            }
            this.m_writer.WriteElementString("description", description);

            if (generator != null)
            {
                this.m_writer.WriteElementString("generator", generator);
            }

            this.m_writer.WriteElementString("lastBuildDate", ConvertDate(new DateTime()));
        }

        // Write out an item.
        // title - title of the blog entry
        // content - main body of the blog entry
        // link - link the blog entry back to a webpage.
        // time - date for the blog entry.
        public void WriteItem(string title, string content, Uri link, DateTime time)
        {
            this.m_writer.WriteStartElement("item");
            WriteItemBody(this.m_writer, title, content, link, time);
            this.m_writer.WriteEndElement(); // item
        }

        // Write just the body (InnerXml) of a new item
        public static void WriteItemBody(XmlWriter w, string title, string content, Uri link, DateTime time)
        {
            w.WriteElementString("title", title);
            if (link != null)
            {
                w.WriteElementString("link", link.ToString());
            }
            w.WriteElementString("description", content); // this will escape
            w.WriteElementString("pubDate", ConvertDate(time));
        }

        // Close out the RSS stream.
        // this does not close the underlying XML writer.
        public void Close()
        {
            this.m_writer.WriteEndElement(); // channel
            this.m_writer.WriteEndElement(); // rss
        }

        // Convert a DateTime into the format needed for RSS (from RFC 822).
        // This looks like: "Wed, 04 Jan 2006 16:03:00 GMT"
        private static string ConvertDate(DateTime t)
        {
            // See this for help on format string
            // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpguide/html/cpconcustomdatetimeformatstrings.asp 
            DateTime t2 = t.ToUniversalTime();
            return t2.ToString(@"ddd, dd MMM yyyy HH:mm:ss G\MT");

            // unnecessary - t.ToUniversalTime().ToString ("r") would have done it /RF
        }
    }
}