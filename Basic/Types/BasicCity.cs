using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    [Serializable]
    public class BasicCity : IHasIdentity
    {
        public BasicCity (int cityId, string name, int countryId, int geographyId)
        {
            this.cityId = cityId;
            this.name = name;
            this.countryId = countryId;
            this.geographyId = geographyId;
        }

        public BasicCity (BasicCity original)
            : this(original.cityId, original.name, original.countryId, original.geographyId)
        {
        }

        [Obsolete ("Do not call this ctor directly. It exists to enable serialization.", true)]
        public BasicCity()
        {
            // this ctor should never be called from code
        }

        private int cityId;
        private string name;
        private int countryId;
        private int geographyId;

        public int CityId
        {
            get { return this.cityId; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public int CountryId
        {
            get { return this.countryId; }
        }

        public int GeographyId
        {
            get { return this.geographyId; }
        }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.cityId; }
        }

        #endregion
    }
}