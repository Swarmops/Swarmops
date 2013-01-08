using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicOrganization : IHasIdentity
    {
        public BasicOrganization()
        {
        }

        public BasicOrganization (int organizationId, string name, string nameInternational)
        {
            this.organizationId = organizationId;
            this.name = name;
            this.nameInternational = nameInternational;
        }

        public BasicOrganization (int organizationId, int parentOrganizationId, string name, string nameInternational) :
            this(organizationId, name, nameInternational)
        {
            this.parentOrganizationId = parentOrganizationId;
        }


        public BasicOrganization (int organizationId, int parentOrganizationId, string name, string nameInternational,
                                  string nameShort, string domain, string mailPrefix, int anchorGeographyId,
                                  bool acceptsMembers, bool autoAssignNewMembers,
                                  int defaultCountryId)
            : this(organizationId, parentOrganizationId, name, nameInternational)
        {
            this.nameShort = nameShort;
            this.domain = domain;
            this.mailPrefix = mailPrefix;
            this.anchorGeographyId = anchorGeographyId;
            this.acceptsMembers = acceptsMembers;
            this.autoAssignNewMembers = autoAssignNewMembers;
            this.defaultCountryId = defaultCountryId;
        }

        public BasicOrganization (BasicOrganization original)
            : this(
                original.Identity, original.ParentOrganizationId, original.Name, original.NameInternational,
                original.NameShort, original.Domain, original.MailPrefix, original.AnchorGeographyId,
                original.AcceptsMembers, original.AutoAssignNewMembers, original.DefaultCountryId)
        {
        }

        public int DefaultCountryId
        {
            get { return this.defaultCountryId; }
            protected set { this.defaultCountryId = value; }
        }

        public string Name 
        {
            get { return this.name; }
            protected set { this.name = value; }
        }

        public string NameInternational
        {
            get { return this.nameInternational; }
            protected set { this.nameInternational = value; }
        }

        public string NameShort
        {
            get
            {
                if (this.nameShort.Length > 0)
                {
                    return this.nameShort;
                }

                return this.Name;
            }
            set
            {
                this.nameShort = value;
            }
        }

        public string Domain
        {
            get { return this.domain; }
            protected set { this.domain = value; }
        }

        public string MailPrefix
        {
            get { return this.mailPrefix; }
            protected set { this.mailPrefix = value; }
        }

        public int AnchorGeographyId
        {
            get { return this.anchorGeographyId; }
            protected set { this.anchorGeographyId = value; }
        }

        public bool AcceptsMembers
        {
            get { return this.acceptsMembers; }
            protected set {  this.acceptsMembers=value; }
        }

        public int OrganizationId
        {
            get { return this.organizationId; }
            protected set { this.organizationId = value; }
        }

        public int ParentOrganizationId
        {
            get { return this.parentOrganizationId; }
            protected set { this.parentOrganizationId = value; }
        }

        public bool AutoAssignNewMembers
        {
            get { return this.autoAssignNewMembers; }
            protected set { this.autoAssignNewMembers = value; }
        }

        private string name;
        private string nameInternational;
        private string nameShort;
        private string domain;
        private string mailPrefix;
        private int anchorGeographyId;
        private bool acceptsMembers;
        private bool autoAssignNewMembers;
        private int organizationId;
        private int parentOrganizationId;
        private int defaultCountryId;

        #region IHasIdentity Members

        public int Identity
        {
            get { return OrganizationId; }
        }

        #endregion
    }
}