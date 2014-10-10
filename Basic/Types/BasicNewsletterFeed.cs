namespace Swarmops.Basic.Types
{
    public class BasicNewsletterFeed
    {
        public BasicNewsletterFeed (int newsletterId, int organizationId, bool defaultSubscribed, string name)
        {
            this.NewsletterFeedId = newsletterId;
            this.OrganizationId = organizationId;
            this.DefaultSubscribed = defaultSubscribed;
            this.Name = name;
        }

        public BasicNewsletterFeed()
        {
            Name = string.Empty;
        }

        public BasicNewsletterFeed (BasicNewsletterFeed original)
            : this(original.NewsletterFeedId, original.OrganizationId, original.DefaultSubscribed, original.Name)
        {
        }

        /// <summary>
        /// The database identity of the newsletter feed.
        /// </summary>
        public readonly int NewsletterFeedId;

        /// <summary>
        /// The identity of the organization that owns this newsletter.
        /// </summary>
        public readonly int OrganizationId;

        /// <summary>
        /// If true, a new member in the owning organization will get this newsletter.
        /// </summary>
        public readonly bool DefaultSubscribed;

        /// <summary>
        /// A friendly name for the newsletter.
        /// </summary>
        public readonly string Name;
    }
}