using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Communications;

namespace Swarmops.Logic.Communications
{
    public class NewsletterFeeds : List<NewsletterFeed>
    {
        public static NewsletterFeeds FromArray (BasicNewsletterFeed[] basicArray)
        {
            NewsletterFeeds result = new NewsletterFeeds();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicNewsletterFeed basic in basicArray)
            {
                result.Add (NewsletterFeed.FromBasic (basic));
            }

            return result;
        }

        public NewsletterFeed Find (int newsletterFeedId)
        {
            NewsletterFeed result = null;
            foreach (NewsletterFeed feed in this)
            {
                if (feed.NewsletterFeedId == newsletterFeedId)
                {
                    result = feed;
                    break;
                }
            }
            return result;
        }
    }
}