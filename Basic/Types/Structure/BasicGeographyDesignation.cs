using Swarmops.Common.Enums;

namespace Swarmops.Basic.Types.Structure
{
    public class BasicGeographyDesignation
    {
        public BasicGeographyDesignation (int geographyId, int countryId,
            string designation, GeographyLevel geoLevelId)
        {
            this.GeographyId = geographyId;
            this.CountryId = countryId;
            this.Designation = designation;
            this.GeographyLevel = geoLevelId;
        }

        public BasicGeographyDesignation (BasicGeographyDesignation original) :
            this (original.GeographyId, original.CountryId, original.Designation, original.GeographyLevel)
        {
            // empty copy ctor
        }

        public int GeographyId { get; private set; }
        public int CountryId { get; private set; }
        public string Designation { get; private set; }
        public GeographyLevel GeographyLevel { get; private set; }
    }
}