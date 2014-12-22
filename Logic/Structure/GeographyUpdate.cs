using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Structure;
using Swarmops.Database;

namespace Swarmops.Logic.Structure
{
    [Serializable]
    public class GeographyUpdate : BasicGeographyUpdate
    {
        [Obsolete("Do not call this ctor directly. It is intended for serialization use only.", true)]
        public GeographyUpdate()
        {
            
        }

        private GeographyUpdate (BasicGeographyUpdate basic)
            : base (basic)
        {
            // private ctor
        }

        public static GeographyUpdate FromBasic(BasicGeographyUpdate basic)
        {
            return new GeographyUpdate(basic);
        }

        public Country Country
        {
            get
            {
                if (string.IsNullOrEmpty (base.CountryCode))
                {
                    return null;
                }

                return Country.FromCode (base.CountryCode);
            }
        }

        public static GeographyUpdate Create (string updateType, string updateSource, string guid, string countryCode, string changeDataXml, DateTime effectiveDateTime)
        {
            int identity = SwarmDb.GetDatabaseForWriting()
                .CreateGeographyUpdate (updateType, updateSource, guid, countryCode, changeDataXml, DateTime.UtcNow,
                    effectiveDateTime);
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetGeographyUpdate (identity));  // "ForWriting" intentional to avoid db replication race conditions
        }

    }
}
