using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    /// <summary>
    /// This class encapsulates a single newsletter.
    /// </summary>
    public class BasicNewsletter : IHasIdentity
    {
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
                this(newsletterId, title, introduction, body, authorId, templateId, feedId)
        {
            this.organizationId = organizationId;
            this.geographyId = geographyId;
        }

        public int NewsletterId
        {
            get { return newsletterId; }
        }

        public string Title
        {
            get { return title; }
        }

        public string Introduction
        {
            get { return introduction; }
        }

        public string Body
        {
            get { return body; }
        }

        public int AuthorId
        {
            get { return authorId; }
        }

        public int NewsletterFeedId
        {
            get { return newsletterFeedId; }
        }

        public int NewsletterTemplateId
        {
            get { return newsletterTemplateId; }
        }

        public int OrganizationId
        {
            get { return organizationId; }
        }

        public int GeographyId
        {
            get { return geographyId; }
        }

        #region IHasIdentity Members

        int IHasIdentity.Identity
        {
            get { return NewsletterId; }
        }

        #endregion

        private int newsletterId;
        private string title;
        private string introduction;
        private string body;
        private int authorId;
        private int newsletterFeedId;
        private int newsletterTemplateId;
        private int geographyId;
        private int organizationId;
    }
}