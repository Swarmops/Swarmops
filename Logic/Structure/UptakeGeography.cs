using Swarmops.Basic.Types;

namespace Swarmops.Logic.Structure
{
    public class UptakeGeography : BasicUptakeGeography
    {
        #region Creation and Construction

        // ReSharper disable UnusedPrivateMember
        protected UptakeGeography()
            // ReSharper restore UnusedPrivateMember
        {
        } // disallow public direct construction

        private UptakeGeography (BasicUptakeGeography basic)
            : base (basic)
        {
        }

        protected UptakeGeography (UptakeGeography other)
            : base (other)
        {
        }


        public static UptakeGeography FromBasic (BasicUptakeGeography basic)
        {
            return new UptakeGeography (basic);
        }

        #endregion

        #region Public methods

        #endregion

        #region Public properties

        public string GeoName
        {
            get { return Geography.Name; }
        }

        public string OrgName
        {
            get { return Organization.Name; }
        }

        public Geography Geography
        {
            get
            {
                if (this.geography == null)
                    this.geography = Geography.FromIdentity (GeoId);
                return this.geography;
            }
            protected set
            {
                if (Organization != null)
                    Organization.DeleteUptakeGeography (GeoId);

                if (value != null && Organization != null)
                    Organization.AddUptakeGeography (value.GeographyId);

                this.geography = value;
            }
        }

        public Organization Organization
        {
            get
            {
                if (this.organization == null)
                    this.organization = Organization.FromIdentity (OrgId);

                return this.organization;
            }
            protected set
            {
                if (Organization != null)
                    Organization.DeleteUptakeGeography (GeoId);
                if (value != null)
                    value.AddUptakeGeography (GeoId);
                this.organization = value;
            }
        }

        #endregion

        private Geography geography;
        private Organization organization;
    }
}