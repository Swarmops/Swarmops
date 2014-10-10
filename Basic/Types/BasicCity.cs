using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    [Serializable]
    public class BasicCity : IHasIdentity
    {
        public BasicCity (int cityId, string name, int countryId, int geographyId)
        {
            this.CityId = cityId;
            this.Name = name;
            this.CountryId = countryId;
            this.GeographyId = geographyId;
        }

        public BasicCity (BasicCity original)
            : this(original.CityId, original.Name, original.CountryId, original.GeographyId)
        {
        }

        [Obsolete ("Do not call this ctor directly. It exists to enable serialization.", true)]
        public BasicCity()
        {
            // this ctor should never be called from code
        }

        public int CityId { get; private set; }
        public string Name { get; protected set; }
        public int CountryId { get; protected set; }
        public int GeographyId { get; protected set; }


        #region IHasIdentity Members

        public int Identity
        {
            get { return this.CityId; }
        }

        #endregion
    }
}