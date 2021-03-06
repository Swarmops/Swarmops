using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Structure;
using Swarmops.Common.Enums;
using Swarmops.Common.Generics;
using Swarmops.Database;
using Swarmops.Logic.Cache;

namespace Swarmops.Logic.Structure
{
    [Serializable]
    public class Geography : BasicGeography
    {
        #region Construction and Creation

        private Geography()
            : base (0, 0, string.Empty)
        {
            // private ctor prevents construction
        }

        private Geography (BasicGeography basic)
            : base (basic)
        {
            // construction from basic
        }

        public static Geography FromBasic (BasicGeography basic)
        {
            return new Geography (basic);
        }

        public static Geography FromIdentity (int geographyId)
        {
            return FromBasic (GeographyCache.GetGeography (geographyId));
            //return FromBasic(SwarmDb.GetDatabaseForReading().GetGeography(geographyId));
        }

        public static Geography FromName (string geographyName)
        {
            // TODO: Possible dupes here, need resolution once we see the extent of the problem

            return FromBasic (GeographyCache.GetGeographyByName (geographyName));
            //return FromBasic(SwarmDb.GetDatabaseForReading().GetGeographyByName(geographyName));
        }


        public static Geography FromOfficialDesignation (Country country, GeographyLevel level, string designation)
        {
            return FromOfficialDesignation (country.Identity, level, designation);
        }

        public static Geography FromOfficialDesignation (int countryId, GeographyLevel level, string designation)
        {
            return
                FromIdentity (SwarmDb.GetDatabaseForReading().GetGeographyIdFromOfficialDesignation (countryId, level,
                    designation));
        }

        #endregion

        public static readonly int RootIdentity = 1; // The identity of the root geography (i.e., "World")

        public static readonly int IgnoreGeography = -1; // Used as parameter to methods where geography is optional

        public Tree<Geography> Tree
        {
            get
            {
                Geographies geographies = this.ThisAndBelow();
                return Tree<Geography>.FromCollection (geographies, this);
            }
        }

        public Geographies Children
        {
            get { return Geographies.FromArray (GeographyCache.GetGeographyChildren (Identity)); }
        }

        public int ChildrenCount
        {
            get { return GeographyCache.CountGeographyChildren (Identity); }
        }


        public static Geography Root
        {
            get { return FromIdentity (RootIdentity); }
        }

        public Geography Parent
        {
            get { return FromIdentity (ParentIdentity); }
        }

        public bool IsOrInherits(Geography prospectiveParent)
        {
            if (Identity == prospectiveParent.Identity)
                return true;
            return Inherits(prospectiveParent.Identity);
        }

        public bool Inherits(Geography prospectiveParent)
        {
            return Inherits(prospectiveParent.Identity);
        }

        private bool IsOrInherits(int prospectiveParentGeographyId)
        {
            if (Identity == prospectiveParentGeographyId)
                return true;
            return Inherits(prospectiveParentGeographyId);
        }

        private bool Inherits(int prospectiveParentGeographyId)
        {
            // Returns true if prospectiveParent is a parent of ours.

            Geographies line = GetRootLineage();

            for (int index = 0; index < line.Count - 1; index++)
            {
                if (line[index].Identity == prospectiveParentGeographyId)
                {
                    return true;
                }
            }

            return false;
        }

        public Country Country
        {
            get
            {
                Countries countries = Countries.All;
                Dictionary<int, Country> geoLookup = new Dictionary<int, Country>();

                foreach (Country country in countries)
                {
                    if (country.GeographyId != Geography.RootIdentity && country.GeographyId != 0)
                    {
                        geoLookup[country.GeographyId] = country;
                    }
                }

                Geography iterator = this;
                while (!geoLookup.ContainsKey (iterator.Identity) && iterator.ParentGeographyId != 0)
                {
                    iterator = iterator.Parent;
                }

                if (geoLookup.ContainsKey (iterator.Identity))
                {
                    return geoLookup[iterator.Identity];
                }

                return null; // no country - we're probably a geography above the country line
            }
        }

        public Geographies ThisAndBelow()
        {
            return Geographies.FromArray (GeographyCache.GetGeographyTree (Identity));
            // return Geographies.FromArray(SwarmDb.GetDatabaseForReading().GetGeographyTree(Identity));
        }

        public Geographies GetRootLineage()
        {
            return Geographies.FromArray (GeographyCache.GetGeographyLine (Identity));
            //return Geographies.FromArray(SwarmDb.GetDatabaseForReading().GetGeographyLine(Identity));
        }



        public bool AtLevel (GeographyLevel level)
        {
            GeographyLevel[] levels = SwarmDb.GetDatabaseForReading().GetGeographyLevelsAtGeographyId (Identity);

            foreach (GeographyLevel potentialMatch in levels)
            {
                if (level == potentialMatch)
                {
                    return true;
                }
            }

            return false;
        }

        public string Localized
        {
            get
            {
                if (Name.StartsWith("[LOC]"))
                {
                    return Resources.Logic_Structure_Geography.ResourceManager.GetString(Name.Substring(5));
                }
                return Name;
            }
        }

        public BasicGeographyDesignation[] GetGeographyDesignations()
        {
            return SwarmDb.GetDatabaseForReading().GetGeographyDesignationsForGeographyId (Identity);
        }

    }
}