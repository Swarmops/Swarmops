namespace Swarmops.Basic.Types
{
  
        public class BasicUptakeGeography
        {
            int orgId;
            public int OrgId
            {
                get { return orgId; }
                set { orgId = value; }
            }
            int geoId;
            public int GeoId
            {
                get { return geoId; }
                set { geoId = value; }
            }

            public BasicUptakeGeography (int pOrgId, int pGeoId)
            {
                orgId = pOrgId;
                geoId = pGeoId;
            }

            public BasicUptakeGeography ()
            {
            }

            public BasicUptakeGeography (BasicUptakeGeography other)
            {
                this.orgId = other.orgId;
                this.geoId = other.geoId;
            }
        }
}

