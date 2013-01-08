using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicMailTemplate : IHasIdentity
    {
        /// <summary>
        /// Basic constructor.
        /// </summary>
        public BasicMailTemplate (int templateId,
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
        /// Copy constructor.
        /// </summary>
        public BasicMailTemplate (BasicMailTemplate original)
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

        public int Identity
        {
            get { return this.templateId; }
        }


        public string TemplateName
        {
            get
            {
                return templateName;
            }
        }
        public string LanguageCode
        {
            get
            {
                return languageCode;
            }
        }
        public string CountryCode
        {
            get
            {
                return countryCode;
            }
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

        private int templateId;
        private string templateName;
        private string languageCode;
        private string countryCode;
        private int organizationId;
        private string templateBody;
    }
}