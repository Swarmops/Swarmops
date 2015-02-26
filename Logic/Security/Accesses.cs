using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Security
{
    public class Access
    {
        public readonly AccessAspect Aspect;
        public readonly Geography Geography;
        public readonly Organization Organization;
        public readonly AccessType Type;

        public Access (AccessAspect aspect, AccessType type)
        {
            this.Aspect = aspect;
            this.Type = type;
        }

        public Access (Organization organization, AccessAspect aspect, AccessType type)
        {
            this.Organization = organization;
            this.Aspect = aspect;
            this.Type = type;
        }

        public Access (Organization organization, Geography geography, AccessAspect aspect, AccessType type)
        {
            this.Organization = organization;
            this.Geography = geography;
            this.Aspect = aspect;
            this.Type = type;
        }
    }


    public enum AccessAspect
    {
        Unknown = 0,

        /// <summary>
        ///     Basic access to underlying ledgers
        /// </summary>
        Bookkeeping,

        /// <summary>
        ///     Detailed access to underlying ledgers (documents, descriptions)
        /// </summary>
        BookkeepingDetails,

        /// <summary>
        ///     Access to auditing working material (redflagged transactions, etc)
        /// </summary>
        Auditing,

        /// <summary>
        ///     Access to regular accounting
        /// </summary>
        Financials,

        /// <summary>
        ///     Access to send various correspondence, w/o necessarily knowing the recipients
        /// </summary>
        Correspondence,

        /// <summary>
        ///     Access to personal details about people in an organization
        /// </summary>
        PersonalData,

        /// <summary>
        ///     Access to change organization's fundamental operating parameters
        /// </summary>
        Administration,

        /// <summary>
        ///     Access to this Swarmops installation and its operating parameters
        /// </summary>
        System

    }

    public enum AccessType
    {
        Unknown = 0,
        Read,
        Write
    }
}