using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Security
{
    public class Access
    {
        public readonly AccessAspect Aspect;
        public readonly Geography Geography;
        public readonly Organization Organization;
        public readonly AccessType Type;

        public Access (AccessAspect aspect, AccessType type = AccessType.Write)  // Default to demanding r/w access unless r/o specified
        {
            this.Aspect = aspect;
            this.Type = type;
        }

        public Access (Organization organization, AccessAspect aspect, AccessType type = AccessType.Write)
        {
            this.Organization = organization;
            this.Aspect = aspect;
            this.Type = type;
        }

        public Access(Organization organization, Geography geography, AccessAspect aspect, AccessType type = AccessType.Write)
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
        ///     Access to the mere existence (photo, name) about people in an organization
        /// </summary>
        Participation,

        /// <summary>
        ///     Access to change organization's fundamental operating parameters
        /// </summary>
        Administration,

        /// <summary>
        ///     Access to this Swarmops installation and its operating parameters
        /// </summary>
        System,

        /// <summary>
        ///     Null security: disable security checks entirely, like for Dashboard where each component rolls their own
        /// </summary>
        Null,

        /// <summary>
        ///     Uniquely identified person (logged on) in a specific org. Write access means they can submit expense reports on their own behalf &c.
        /// </summary>
        Participant,

        /// <summary>
        ///     Can enter received invoices, paper letters, etc.
        /// </summary>
        Secretarial,

        /// <summary>
        ///     Access to personalized salary data (implies financial read access)
        /// </summary>
        Payroll
    }

    public enum AccessType
    {
        Unknown = 0,
        Read,
        Write
    }
}