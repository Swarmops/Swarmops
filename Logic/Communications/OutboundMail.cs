using System;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Swarmops.Logic.Support;
using System.Collections.Generic;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Media;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Communications
{
    public class OutboundMail : BasicOutboundMail
    {
        #region Creation and Construction

        // =============================
        //   CREATION AND CONSTRUCTION
        // =============================


        /// <summary>
        /// Private constructor from basic object
        /// </summary>
        private OutboundMail (BasicOutboundMail basic)
            : base(basic)
        {
            // remember titel for templated mail, it is the name of the template.
            if (MailType > 0)
                templateName = basic.Title;
        }

        /// <summary>
        /// Creation from basic object - internal to PirateWeb.Logic
        /// </summary>
        internal static OutboundMail FromBasic (BasicOutboundMail basic)
        {
            if (basic == null)
            {
                return null;
            }

            return new OutboundMail(basic);
        }


        /// <summary>
        /// Retrieves the OutboundMail with a certain Identity
        /// </summary>
        /// <param name="outboundMailId">The Identity of the OutboundMail</param>
        /// <returns>An OutboundMail from the database</returns>
        public static OutboundMail FromIdentity (int outboundMailId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetOutboundMail(outboundMailId));
        }


        public static OutboundMail Create (Person author, string title,
                                           string body, int mailPriority, int mailType,
                                           Organization organization, Geography geography)
        {
            return Create(author, title, body, mailPriority, mailType, organization,
                           geography, DateTime.Now);
        }

        public static OutboundMail Create (Person author, string title,
                                           string body, int mailPriority, int mailType,
                                           Organization organization, Geography geography, DateTime releaseDateTime)
        {
            return
                FromIdentity(SwarmDb.GetDatabaseForWriting().CreateOutboundMail(MailAuthorType.Person, author.Identity, title,
                                                                         body, mailPriority, mailType,
                                                                         geography.Identity, organization.Identity,
                                                                         releaseDateTime));
        }


        /// <summary>
        /// Creates an unusable OutboundMail. Used for rendering previews - ONLY.
        /// (this version, with the ignored priority param is for backward compability)
        /// </summary>
        public static OutboundMail CreateFake (Person author, string title, string body, int ignoredPriority, int mailType,
                                               Organization organization, Geography geography)
        {
            return CreateFake(author, title, body, mailType, organization, geography);
        }


        /// <summary>
        /// Creates an unusable OutboundMail. Used for rendering previews - ONLY.
        /// </summary>
        public static OutboundMail CreateFake (Person author, string title,
                                               string body, int mailType,
                                               Organization organization, Geography geography)
        {
            return FromBasic(new BasicOutboundMail(0, MailAuthorType.Person, author.PersonId, title,
                                                     body, 99, mailType, organization.Identity,
                                                     geography.Identity, DateTime.Now,
                                                     DateTime.Now, false, false, false, DateTime.MinValue,
                                                     DateTime.MinValue, DateTime.MinValue, 0, 0, 0));
        }

        /// <summary>
        /// Creates an OutboundMail with a "functional" author. 
        /// </summary>
        public static OutboundMail CreateFunctional (MailAuthorType authorType, string title,
                                           string body, int mailPriority, int mailType,
                                           int organizationId, int geographyId, DateTime releaseDateTime)
        {

            int mailID = SwarmDb.GetDatabaseForWriting().CreateOutboundMail(authorType, 0, title,
                                           body, mailPriority, mailType, geographyId,
                                           organizationId, releaseDateTime);

            //Constructing instead of reading back from DB, saves time but..., Don't know if it is a good idea./JL
            return FromBasic(new BasicOutboundMail(mailID, authorType, 0, title,
                                                    body, mailPriority, mailType, organizationId,
                                                    geographyId, DateTime.Now,
                                                    releaseDateTime));
        }

        public List<OutboundMail> GetDuplicates (DateTime afterTime, int recipientId)
        {
            List<OutboundMail> retList = new List<OutboundMail>();
            BasicOutboundMail[] mails
                    = SwarmDb.GetDatabaseForReading().GetDuplicateOutboundMail(this.AuthorType, AuthorPersonId, Title,
                             Body, MailPriority, MailType, GeographyId, OrganizationId, afterTime, recipientId);
            foreach (BasicOutboundMail mail in mails)
            {
                retList.Add(FromBasic(mail));
            }

            return retList; ;
        }

        public static OutboundMail GetFirstUnresolved ()
        {
            BasicOutboundMail[] mails = SwarmDb.GetDatabaseForReading().GetTopUnresolvedOutboundMail(1);

            if (mails.Length > 0)
            {
                return FromBasic(mails[0]);
            }

            return null;
        }

        public static OutboundMail GetFirstUnprocessed ()
        {
            BasicOutboundMail[] mails = SwarmDb.GetDatabaseForReading().GetTopUnprocessedOutboundMail(1);

            if (mails.Length > 0)
            {
                return FromBasic(mails[0]);
            }

            return null;
        }

        #endregion

        #region Properties and Accessors

        public new DateTime StartProcessDateTime
        {
            get { return base.StartProcessDateTime; }
            set
            {
                SwarmDb.GetDatabaseForWriting().SetOutboundMailStartProcess(Identity);
                base.StartProcessDateTime = DateTime.Now;
            }
        }

        public Person Author
        {
            get
            {
                if (this.AuthorType != MailAuthorType.Person)
                {
                    throw new InvalidOperationException("OutboundMail.Author is not a valid property when OutboundMail.AuthorType isn't MailAuthorType.Person.");
                }

                if (this.author == null)
                {
                    this.author = Person.FromIdentity(AuthorPersonId);
                }

                return this.author;
            }
        }

        public Organization Organization
        {
            get
            {
                if (this.organization == null)
                {
                    this.organization = Organization.FromIdentity(OrganizationId);
                }

                return this.organization;
            }
        }

        public Geography Geography
        {
            get
            {
                if (this.geography == null)
                {
                    this.geography = Geography.FromIdentity(GeographyId);
                }

                return this.geography;
            }
        }

        public string TemplateName
        {
            get
            {
                return templateName;
            }
        }
        #endregion


        #region Recipient access and manipulation

        public OutboundMailRecipients GetNextRecipientBatch (int batchSize)
        {
            return
                OutboundMailRecipients.FromArray(
                    SwarmDb.GetDatabaseForReading().GetTopOutboundMailRecipients(Identity, batchSize), this);
        }

        public void AddRecipient (Person person, bool asOfficer)
        {
            AddRecipient(person.Identity, asOfficer);
        }

        public void AddRecipient (Reporter person)
        {
            AddReporterRecipient(person.Identity);
        }

        public void AddRecipients (int[] personIds, bool asOfficers)
        {
            foreach (int personId in personIds)
            {
                AddRecipient(personId, asOfficers);
            }
        }

        public void AddReporterRecipients (int[] personIds)
        {
            foreach (int personId in personIds)
            {
                AddReporterRecipient(personId);
            }
        }

        public void AddRecipient (int personId, bool asOfficer)
        {
            SwarmDb.GetDatabaseForWriting().CreateOutboundMailRecipient(Identity, personId, asOfficer, (int)OutboundMailRecipient.RecipientType.Person);
        }

        private void AddReporterRecipient (int identity)
        {
            SwarmDb.GetDatabaseForWriting().CreateOutboundMailRecipient(Identity, identity, false, (int)OutboundMailRecipient.RecipientType.Reporter);
        }
        #endregion

        #region Status changers

        public void SetReadyForPickup ()
        {
            SwarmDb.GetDatabaseForWriting().SetOutboundMailReadyForPickup(Identity);
            base.ReadyForPickup = true;
        }

        public void SetResolved ()
        {
            SwarmDb.GetDatabaseForWriting().SetOutboundMailResolved(Identity);
            base.Resolved = true;
        }

        public void SetProcessed ()
        {
            SwarmDb.GetDatabaseForWriting().SetOutboundMailProcessed(Identity);
            base.Processed = true;
        }

        public void SetRecipientCount (int recipientCount)
        {
            SwarmDb.GetDatabaseForWriting().SetOutboundMailRecipientCount(Identity, recipientCount);
            base.RecipientCount = recipientCount;
        }

        public void IncrementSuccesses ()
        {
            SwarmDb.GetDatabaseForWriting().IncrementOutboundMailSuccesses(Identity);
            base.RecipientsSuccess++;
        }

        public void IncrementFailures ()
        {
            SwarmDb.GetDatabaseForWriting().IncrementOutboundMailFailures(Identity);
            base.RecipientsFail++;
        }

        #endregion

        #region Methods and Manipulation

        /// <summary>
        /// Renders the mail for a specific recipient, in text.
        /// </summary>
        /// <param name="person">The person to render for.</param>
        /// <returns>Text with proper replacements done</returns>
        /// 
        public string RenderText (IEmailPerson recipient, string culture)
        {
            // First, check if there is a mail template AT ALL. If not, return the raw text.
            if (MailType == (int)TypedMailTemplate.TemplateType.None)
            {
                return Body;
            }

            // (If there is a template, the body contains the serialized placeholder values)
            return Render(recipient, culture, true).PlainText;
        }


        /// <summary>
        /// Renders the mail for a specific recipient, in HTML.
        /// </summary>
        /// <param name="recipient">The person to render for.</param>
        /// <returns>HTML with proper replacements done</returns>
        /// 
        public string RenderHtml (IEmailPerson recipient, string culture)
        {
            // First, check if there is a mail template AT ALL. If not, return the raw text,
            // within <PRE></PRE> if not already html

            if (MailType == (int)TypedMailTemplate.TemplateType.None)
            {
                Regex reHtmlType = new Regex(@"(</[a-z1-4]+>)", RegexOptions.IgnoreCase);

                if (reHtmlType.IsMatch(Body))
                    return Body;
                else
                    return "<PRE>" + Body + "</PRE>";
            }

            // (If there is a template, the body contains the serialized placeholder values)
            return Render(recipient, culture, false).Html;
        }

        /// <summary>
        /// Renders the mail for a specific recipient
        /// 
        /// </summary>
        /// <param name="recipient">The person to render for.</param>
        /// <param name="recipient">The culture to render for.</param>
        /// <param name="renderText">True if "Plain" template should be used (if available)</param>
        /// <returns>MailTemplate with proper replacements done</returns>
        /// 
        public MailTemplate Render (IEmailPerson recipient, string culture, bool renderText)
        {
            lock (lockTemplates)
            {
                //Locking is needed since this is used in a multithreading scenario and the template is cached for efficiency

                TypedMailTemplate template = PrepareTemplate(culture, renderText);   // Setup template and load serialized replacements
                template.FillIndividualPlaceholders(recipient);             // Add recipient data
                template.InsertAllPlaceHoldersToTemplate();                 // Make all replacements
                this.Title = template.Template.TemplateTitleText;           // Get the resulting title
                template.Template.TemplateBody = template.Template.TemplateBody; // Force it to save Html
                return new MailTemplate(template.Template);
            }
        }

        private TypedMailTemplate PrepareTemplate (string culture, bool renderText)
        {
            TypedMailTemplate template = null;
            if (renderText)
            {
                if (templatePlain == null)
                {
                    templatePlain = TypedMailTemplate.FromName(TypedMailTemplate.GetNameFromMailType( MailType) + "Plain");
                    templatePlain.Initialize(culture.Substring(3), Organization.Identity, this, (renderText ? "Plain" : ""));
                    templatePlain.PreparePlaceholders();
                }
                else
                {   //reuse the template, just reset the html to the saved version before replacements
                    templatePlain.Template.ResetContent();
                }
                template = templatePlain;
            }
            else
            {
                if (templateHtml == null)
                {
                    templateHtml = TypedMailTemplate.FromName(TypedMailTemplate.GetNameFromMailType(MailType));
                    templateHtml.Initialize(culture.Substring(3), Organization.Identity, this, (renderText ? "Plain" : ""));
                    templateHtml.PreparePlaceholders();
                }
                else
                {   //reuse the template, just reset the html to the saved version before replacements
                    templateHtml.Template.ResetContent();
                }
                template = templateHtml;
            }

            return template;
        }

        #endregion

        #region Dish and Dishonesty

        // Long live Black Adder.

        #endregion

        public static readonly int PriorityHighest = 10;
        public static readonly int PriorityHigh = 30;
        public static readonly int PriorityNormal = 50;
        public static readonly int PriorityLow = 70;
        public static readonly int PriorityLowest = 90;

        private Person author;
        private Organization organization;
        private Geography geography;

        //If used in a multithreaded scenario, the cached templates need to be locked while rendering.
        private static object lockTemplates = new object();
        private TypedMailTemplate templateHtml = null; //Cached template to use for plain rendering
        private TypedMailTemplate templatePlain = null;//Cached template to use for html rendering


        // keeps track of the Title of the outbound mail as it were when loaded from DB
        private string templateName = "";

    }
}