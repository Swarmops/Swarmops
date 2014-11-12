using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Security
{
    public class Access
    {
        public Access (AccessAspect aspect, AccessType type)
        {
            this.Aspect = aspect;
            this.Type = type;
        }

        public Access(Organization organization, AccessAspect aspect, AccessType type)
        {
            this.Organization = organization;
            this.Aspect = aspect;
            this.Type = type;
        }

        public Access(Organization organization, Geography geography, AccessAspect aspect, AccessType type)
        {
            this.Organization = organization;
            this.Geography = geography;
            this.Aspect = aspect;
            this.Type = type;
        }

        public readonly Organization Organization;
        public readonly Geography Geography;
        public readonly AccessAspect Aspect;
        public readonly AccessType Type;
    }


    public enum AccessAspect
    {
        Unknown = 0,
        /// <summary>
        /// Access to underlying ledgers
        /// </summary>
        Bookkeeping,
        /// <summary>
        /// Access to regular accounting
        /// </summary>
        Financials,
        /// <summary>
        /// Access to personal details about people in org
        /// </summary>
        PersonData,
        /// <summary>
        /// Access to change organization's fundamental operating parameters
        /// </summary>
        Administration
    }

    public enum AccessType
    {
        Unknown = 0,
        Read,
        Write
    }
}
