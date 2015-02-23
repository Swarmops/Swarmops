using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Structure
{
    public class BasicOrganization : IHasIdentity, IHasParentIdentity
    {
        private string nameShort;

        #region Interface Members

        public int Identity { get { return this.OrganizationId; } }
        public int ParentIdentity { get { return this.ParentOrganizationId; } }

        #endregion

        public BasicOrganization()
        {
        }

        public BasicOrganization (int organizationId, string name, string nameInternational)
        {
            this.OrganizationId = organizationId;
            this.Name = name;
            this.NameInternational = nameInternational;
        }

        public BasicOrganization (int organizationId, int parentOrganizationId, string name, string nameInternational) :
            this (organizationId, name, nameInternational)
        {
            this.ParentOrganizationId = parentOrganizationId;
        }


        public BasicOrganization (int organizationId, int parentOrganizationId, string name, string nameInternational,
            string nameShort, string domain, string mailPrefix, int anchorGeographyId,
            bool acceptsMembers, bool autoAssignNewMembers,
            int defaultCountryId)
            : this (organizationId, parentOrganizationId, name, nameInternational)
        {
            this.nameShort = nameShort;
            this.Domain = domain;
            this.MailPrefix = mailPrefix;
            this.AnchorGeographyId = anchorGeographyId;
            this.AcceptsMembers = acceptsMembers;
            this.AutoAssignNewMembers = autoAssignNewMembers;
            this.DefaultCountryId = defaultCountryId;
        }

        public BasicOrganization (BasicOrganization original)
            : this (
                original.Identity, original.ParentOrganizationId, original.Name, original.NameInternational,
                original.NameShort, original.Domain, original.MailPrefix, original.AnchorGeographyId,
                original.AcceptsMembers, original.AutoAssignNewMembers, original.DefaultCountryId)
        {
        }

        public int DefaultCountryId { get; protected set; }

        public string Name { get; protected set; }

        public string NameInternational { get; protected set; }

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
            set { this.nameShort = value; }
        }

        public string Domain { get; protected set; }

        public string MailPrefix { get; protected set; }

        public int AnchorGeographyId { get; protected set; }

        public bool AcceptsMembers { get; protected set; }

        public int OrganizationId { get; protected set; }

        public int ParentOrganizationId { get; protected set; }

        public bool AutoAssignNewMembers { get; protected set; }
    }
}