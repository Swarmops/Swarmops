using System;
using System.Xml.Serialization;
using Activizr.Logic.Cache;
using Activizr.Logic.Interfaces;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Database;
using System.Collections.Generic;

namespace Activizr.Logic.Structure
{
    [Serializable]
    public class Geography : BasicGeography, ITreeNode, ITreeNodeObject
    {

        #region Construction and Creation

        private Geography ()
            : base(0, 0, string.Empty)
        {
            // private ctor prevents construction
        }

        private Geography (BasicGeography basic)
            : base(basic)
        {
            // construction from basic
        }

        public static Geography FromBasic (BasicGeography basic)
        {
            return new Geography(basic);
        }

        public static Geography FromIdentity (int geographyId)
        {
            return FromBasic(GeographyCache.GetGeography(geographyId));
            //return FromBasic(PirateDb.GetDatabaseForReading().GetGeography(geographyId));
        }

        public static Geography FromName (string geographyName)
        {
            // TODO: Possible dupes here, need resolution once we see the extent of the problem

            return FromBasic(GeographyCache.GetGeographyByName(geographyName));
            //return FromBasic(PirateDb.GetDatabaseForReading().GetGeographyByName(geographyName));
        }


        public static Geography FromOfficialDesignation (Country country, GeographyLevel level, string designation)
        {
            return FromOfficialDesignation(country.Identity, level, designation);
        }

        public static Geography FromOfficialDesignation (int countryId, GeographyLevel level, string designation)
        {
            return
                FromIdentity(PirateDb.GetDatabaseForReading().GetGeographyIdFromOfficialDesignation(countryId, level,
                                                                                          designation));
        }

        #endregion

        public Geographies Children
        {
            get
            {
                return Geographies.FromArray(GeographyCache.GetGeographyChildren(Identity));
            }
        }

        public int ChildrenCount
        {
            get
            {
                return GeographyCache.CountGeographyChildren(Identity);
            }
        }


        public static Geography Root
        {
            get { return FromIdentity(RootIdentity); }
        }

        public Geography Parent
        {
            get { return FromIdentity(ParentIdentity); }
        }


        public static readonly int RootIdentity = 1; // The identity of the root geography (i.e., "World")

        public static readonly int SwedenId = 30;
        public static readonly int DenmarkId = 357;
        public static readonly int FinlandId = 358;

        public static readonly int IgnoreGeography = -1; // Used as parameter to methods where geography is optional

        #region ITreeNode Members

        public int ParentIdentity
        {
            get { return ParentGeographyId; }
        }

        public int[] ChildrenIdentities
        {
            get { return Children.Identities; }
        }

        [XmlIgnore]  // interface cannot be serialized
        [SoapIgnore] 
        public ITreeNodeObject ParentObject
        {
            get { return this.Parent; }
        }

        [XmlIgnore] // interface cannot be serialized
        [SoapIgnore]    
        public List<ITreeNodeObject> ChildObjects
        {
            get
            {
                List<ITreeNodeObject> retVal = new List<ITreeNodeObject>();
                foreach (Geography child in Children)
                    retVal.Add(child);
                return retVal;
            }
        }

        #endregion

        public Geographies GetTree ()
        {
            return Geographies.FromArray(GeographyCache.GetGeographyTree(Identity));
            // return Geographies.FromArray(PirateDb.GetDatabaseForReading().GetGeographyTree(Identity));
        }

        public Geographies GetLine ()
        {
            return Geographies.FromArray(GeographyCache.GetGeographyLine(Identity));
            //return Geographies.FromArray(PirateDb.GetDatabaseForReading().GetGeographyLine(Identity));
        }

        public bool Inherits (Geography prospectiveParent)
        {
            return Inherits(prospectiveParent.Identity);
        }

        public bool Inherits (int prospectiveParentGeographyId)
        {
            // Returns true if prospectiveParent is a parent of ours.

            Geographies line = GetLine();

            for (int index = 0; index < line.Count - 1; index++)
            {
                if (line[index].Identity == prospectiveParentGeographyId)
                {
                    return true;
                }
            }

            return false;
        }


        public bool AtLevel (GeographyLevel level)
        {
            GeographyLevel[] levels = PirateDb.GetDatabaseForReading().GetGeographyLevelsAtGeographyId(Identity);

            foreach (GeographyLevel potentialMatch in levels)
            {
                if (level == potentialMatch)
                {
                    return true;
                }
            }

            return false;
        }

        public BasicGeographyDesignation[] GetGeographyDesignations ()
        {
            return PirateDb.GetDatabaseForReading().GetGeographyDesignationsForGeographyId(Identity);
        }
    }
}