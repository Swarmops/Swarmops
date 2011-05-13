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

        /// <summary>
        /// The database node id.
        /// </summary>
        public readonly int GeographyId;

        /// <summary>
        /// The database node id of the parent node.
        /// </summary>
        public readonly int ParentGeographyId;

        /// <summary>
        /// The friendly name of the node (city, county, region, etc).
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The generation of this node. Used in tree-style listings.
        /// </summary>
        public readonly int Generation;

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.GeographyId; }
        }

        #endregion
    }
}