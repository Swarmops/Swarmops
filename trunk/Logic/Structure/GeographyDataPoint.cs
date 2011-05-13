using System;

namespace Activizr.Logic.Structure
{
    [Serializable]
    public class GeographyDataPoint
    {
        public int GeographyId;
        public string GeographyName;
        public GeographyOrganizationDataPoint[] OrganizationData;
        public int VoterCount;

        public GeographyDataPoint()
        {
            // HACK

            this.OrganizationData = new GeographyOrganizationDataPoint[2];
            this.OrganizationData[0] = new GeographyOrganizationDataPoint();
            this.OrganizationData[1] = new GeographyOrganizationDataPoint();

            this.OrganizationData[0].OrganizationId = 1;
            this.OrganizationData[0].OrganizationNameShort = "Piratpartiet SE";
            this.OrganizationData[1].OrganizationId = 2;
            this.OrganizationData[1].OrganizationNameShort = "Ung Pirat SE";
        }
    }

    [Serializable]
    public class GeographyOrganizationDataPoint
    {
        public int ActivistCount;
        public int[] BirthYearBracketMemberCounts; // Intervals of five years, starting 1900
        public int FemaleMemberCount;
        public int MaleMemberCount;
        public int OrganizationId;
        public string OrganizationNameShort;

        public GeographyOrganizationDataPoint()
        {
            this.BirthYearBracketMemberCounts = new int[30];
        }

        public int MemberCount
        {
            get { return this.MaleMemberCount + this.FemaleMemberCount; }
            set
            {
                // ignore
            }
        }
    }
}