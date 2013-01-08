using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Communications
{
    public class BasicAutoMail : IHasIdentity
    {
        /// <summary>
        /// Basic constructor.
        /// </summary>
        public BasicAutoMail (int autoMailId, AutoMailType type, int organizationId, int geographyId,
                              int authorPersonId, string title, string body)
        {
            this.autoMailId = autoMailId;
            this.type = type;
            this.organizationId = organizationId;
            this.geographyId = geographyId;
            this.authorPersonId = authorPersonId;
            this.title = title;
            this.body = body;
        }


        /// <summary>
        /// Copy constructor.
        /// </summary>
        public BasicAutoMail (BasicAutoMail original)
            : this(original.autoMailId, original.type, original.organizationId, original.geographyId,
                   original.authorPersonId, original.title, original.body)
        {
        }

        public int AutoMailId
        {
            get { return this.autoMailId; }
        }

        public int Identity
        {
            get { return this.AutoMailId; }
        }

        public AutoMailType Type
        {
            get { return this.type; }
        }

        public int OrganizationId
        {
            get { return this.organizationId; }
        }

        public int GeographyId
        {
            get { return this.geographyId; }
        }

        public int AuthorPersonId
        {
            get { return this.autoMailId; }
        }

        public string Title
        {
            get { return this.title; }
        }

        public string Body
        {
            get { return this.body; }
            set { this.body = value; }
        }

        private int autoMailId;
        private AutoMailType type;
        private int organizationId;
        private int geographyId;
        private int authorPersonId;
        private string title;
        private string body;
    }
}