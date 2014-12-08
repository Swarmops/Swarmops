namespace Swarmops.Basic.Types
{
    public class BasicUptakeGeography
    {
        private int geoId;
        private int orgId;

        public BasicUptakeGeography (int pOrgId, int pGeoId)
        {
            this.orgId = pOrgId;
            this.geoId = pGeoId;
        }

        public BasicUptakeGeography()
        {
        }

        public BasicUptakeGeography (BasicUptakeGeography other)
        {
            this.orgId = other.orgId;
            this.geoId = other.geoId;
        }

        public int OrgId
        {
            get { return this.orgId; }
            set { this.orgId = value; }
        }

        public int GeoId
        {
            get { return this.geoId; }
            set { this.geoId = value; }
        }
    }
}