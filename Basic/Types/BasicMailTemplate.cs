using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicMailTemplate : IHasIdentity
    {
        private readonly string countryCode;
        private readonly string languageCode;
        private readonly int organizationId;
        private readonly int templateId;
        private readonly string templateName;
        private string templateBody;

        /// <summary>
        ///     Basic constructor.
        /// </summary>
        public BasicMailTemplate(int templateId,
            string templateName,
            string languageCode,
            string countryCode,
            int organizationId,
            string templateBody)
        {
            this.templateId = templateId;
            this.templateName = templateName;
            this.languageCode = languageCode;
            this.countryCode = countryCode;
            this.organizationId = organizationId;
            this.templateBody = templateBody;
        }


        /// <summary>
        ///     Copy constructor.
        /// </summary>
        public BasicMailTemplate(BasicMailTemplate original)
            : this(original.templateId,
                original.templateName,
                original.languageCode,
                original.countryCode,
                original.organizationId,
                original.templateBody)
        {
        }

        public int TemplateId
        {
            get { return this.templateId; }
        }


        public string TemplateName
        {
            get { return this.templateName; }
        }

        public string LanguageCode
        {
            get { return this.languageCode; }
        }

        public string CountryCode
        {
            get { return this.countryCode; }
        }

        public int OrganizationId
        {
            get { return this.organizationId; }
        }


        public string TemplateBody
        {
            get { return this.templateBody; }
            set { this.templateBody = value; }
        }

        public int Identity
        {
            get { return this.templateId; }
        }
    }
}