using System;
using System.Collections.Generic;
using System.Text;

namespace Swarmops.Basic.Types
{
    [Serializable]
    public class BasicPostalCode
    {
        public BasicPostalCode(int postalCodeId, string postalCode, int cityId, int countryId)
        {
            this.PostalCodeId = postalCodeId;
            this.PostalCode = postalCode.Replace(" ", "");
            this.CityId = cityId;
            this.CountryId = countryId;
        }

        public BasicPostalCode(BasicPostalCode original)
            : this(original.PostalCodeId, original.PostalCode, original.CityId, original.CountryId)
        {
        }

        [Obsolete ("Do not call this ctor directly. It exists to enable serialization.", true)]
        public BasicPostalCode()
        {
            // this should never be called from code
        }

        public int PostalCodeId { get; private set; }
        public string PostalCode { get; private set; }  // any whitespace removed
        public int CityId { get; protected set; }
        public int CountryId { get; protected set; }
    }
}
