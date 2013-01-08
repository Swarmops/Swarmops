using System;
using System.Globalization;
using Swarmops.Database;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Interfaces;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Cache;
using Swarmops.Basic.Enums;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Swarmops.Basic.Types;

namespace Swarmops.Logic.Structure
{
    public class UptakeGeography : BasicUptakeGeography
    {
        #region Creation and Construction

        // ReSharper disable UnusedPrivateMember
        protected UptakeGeography ()
        // ReSharper restore UnusedPrivateMember
        {
        } // disallow public direct construction

        private UptakeGeography (BasicUptakeGeography basic)
            : base(basic)
        {
        }

        protected UptakeGeography (UptakeGeography other)
            : base(other)
        {
        }


        public static UptakeGeography FromBasic (BasicUptakeGeography basic)
        {
            return new UptakeGeography(basic);
        }

        #endregion

        #region Public methods


        #endregion

        #region Public properties


        public string GeoName
        {
            get
            {
                return this.Geography.Name;
            }
        }

        public string OrgName
        {
            get
            {
                return this.Organization.Name;
            }
        }

        public Geography Geography
        {
            get
            {
                if (this.geography == null)
                    this.geography = Geography.FromIdentity(this.GeoId);
                return geography;
            }
            protected set
            {
                if (this.Organization != null)
                    this.Organization.DeleteUptakeGeography(this.GeoId);

                if (value != null && this.Organization != null)
                    this.Organization.AddUptakeGeography(value.GeographyId);

                geography = value;
            }
        }

        public Organization Organization
        {
            get
            {
                if (this.organization == null)
                    this.organization = Organization.FromIdentity(this.OrgId);

                return organization;
            }
            protected set
            {
                if (this.Organization != null)
                    this.Organization.DeleteUptakeGeography(this.GeoId);
                if (value != null)
                    value.AddUptakeGeography(this.GeoId);
                organization = value;
            }
        }
        #endregion

        private Geography geography;
        private Organization organization;


   }
}