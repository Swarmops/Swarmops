using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Communications
{
    public class BasicAutoMail : IHasIdentity
    {
        private readonly int authorPersonId;
        private readonly int autoMailId;
        private readonly int geographyId;
        private readonly int organizationId;
        private readonly string title;
        private readonly AutoMailType type;
        private string body;

        /// <summary>
        ///     Basic constructor.
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
        ///     Copy constructor.
        /// </summary>
        public BasicAutoMail (BasicAutoMail original)
            : this (original.autoMailId, original.type, original.organizationId, original.geographyId,
                original.authorPersonId, original.title, original.body)
        {
        }

        public int AutoMailId
        {
            get { return this.autoMailId; }
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

        public int Identity
        {
            get { return AutoMailId; }
        }
    }
}