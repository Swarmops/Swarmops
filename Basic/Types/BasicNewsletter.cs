using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    /// <summary>
    ///     This class encapsulates a single newsletter.
    /// </summary>
    public class BasicNewsletter : IHasIdentity
    {
        private readonly int authorId;
        private readonly string body;
        private readonly int geographyId;
        private readonly string introduction;
        private readonly int newsletterFeedId;
        private readonly int newsletterId;
        private readonly int newsletterTemplateId;
        private readonly int organizationId;
        private readonly string title;

        public BasicNewsletter (int newsletterId, string title, string introduction, string body,
            int authorId, int templateId, int feedId)
        {
            this.newsletterId = newsletterId;
            this.newsletterFeedId = feedId;
            this.title = title;
            this.introduction = introduction;
            this.body = body;
            this.authorId = authorId;
            this.newsletterTemplateId = templateId;
            this.newsletterFeedId = feedId;
        }

        public BasicNewsletter (int newsletterId, string title, string introduction, string body,
            int authorId, int templateId, int feedId, int organizationId, int geographyId)
            :
                this (newsletterId, title, introduction, body, authorId, templateId, feedId)
        {
            this.organizationId = organizationId;
            this.geographyId = geographyId;
        }

        public int NewsletterId
        {
            get { return this.newsletterId; }
        }

        public string Title
        {
            get { return this.title; }
        }

        public string Introduction
        {
            get { return this.introduction; }
        }

        public string Body
        {
            get { return this.body; }
        }

        public int AuthorId
        {
            get { return this.authorId; }
        }

        public int NewsletterFeedId
        {
            get { return this.newsletterFeedId; }
        }

        public int NewsletterTemplateId
        {
            get { return this.newsletterTemplateId; }
        }

        public int OrganizationId
        {
            get { return this.organizationId; }
        }

        public int GeographyId
        {
            get { return this.geographyId; }
        }

        #region IHasIdentity Members

        int IHasIdentity.Identity
        {
            get { return NewsletterId; }
        }

        #endregion
    }
}