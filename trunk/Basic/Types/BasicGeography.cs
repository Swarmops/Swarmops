using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    /// <summary>
    /// A geographical node.
    /// </summary>
    [Serializable]
    public class BasicGeography : IHasIdentity
    {
        public BasicGeography (int geographyId, int parentGeographyId, string name)
            : this(geographyId, parentGeographyId, name, 0)
        {
        }

        public BasicGeography (int geographyId, int parentGeographyId, string name, int generation)
        {
            this.GeographyId = geographyId;
            this.ParentGeographyId = parentGeographyId;
            this.Name = name;
            this.Generation = generation;
        }

        public BasicGeography (BasicGeography original)
            : this(original.GeographyId, original.ParentGeographyId, original.Name, original.Generation)
        {
        }

        [Obsolete("Do not use this ctor. It is provided to enable serializability.", true)]
        public BasicGeography()
        {
            // provided for serializability - does not initialize fields
        }

        /// <summary>
        /// The database id of this geography.
        /// </summary>
        public int GeographyId { get; private set; }

        /// <summary>
        /// The database id of the parent geography, for linking in a tree structure.
        /// </summary>
        public int ParentGeographyId { get; protected set; }

        /// <summary>
        /// The friendly name of the node (city, county, region, etc).
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The generation of this node. Used in tree-style listings.
        /// </summary>
        public int Generation { get; protected set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.GeographyId; }
        }

        #endregion
    }
}