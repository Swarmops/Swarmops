using Swarmops.Basic.Enums;
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
        Bookkeeping,
        Financials
    }

    public enum AccessType
    {
        Unknown = 0,
        Read,
        Write
    }
}
