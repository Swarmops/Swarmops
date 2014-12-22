using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Swarmops.Basic.Types.Structure
{
    [Serializable]
    public class BasicGeographyUpdate
    {
        [Obsolete ("Do not call this ctor directly. It is intended for serialization.", true)]
        public BasicGeographyUpdate()
        {
            
        }

        public BasicGeographyUpdate (int geographyUpdateId, string updateType, string updateSource, string guid,
            string countryCode, string changeDataXml, DateTime createdDateTime, DateTime effectiveDateTime,
            bool processed)
        {
            this.GeographyUpdateId = geographyUpdateId;
            this.UpdateType = updateType;
            this.UpdateSource = updateSource;
            this.Guid = guid;
            this.CountryCode = countryCode;
            this.ChangeDataXml = changeDataXml;
            this.CreatedDateTime = createdDateTime;
            this.EffectiveDateTime = effectiveDateTime;
            this.Processed = processed;
        }

        public BasicGeographyUpdate (BasicGeographyUpdate original)
            : this (
                original.GeographyUpdateId, original.UpdateType, original.UpdateSource, original.Guid,
                original.CountryCode, original.ChangeDataXml, original.CreatedDateTime, original.EffectiveDateTime,
                original.Processed)
        {
            
        }


        public int GeographyUpdateId { get; private set; }
        public string UpdateType { get; private set; }
        public string UpdateSource { get; private set; }
        public string Guid { get; private set; }
        public string CountryCode { get; private set; }
        public string ChangeDataXml { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public DateTime EffectiveDateTime { get; private set; }
        public bool Processed { get; protected set; }
    }
}
