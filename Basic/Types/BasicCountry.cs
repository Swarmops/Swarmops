using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    [Serializable]
    public class BasicCountry : IHasIdentity
    {
        public BasicCountry (int countryId, string name, string code)
        {
            this.CountryId = countryId;
            this.Name = name;
            this.Code = code;
        }

        public BasicCountry (int countryId, string name, string code, string defaultCulture)
            : this(countryId, name, code)
        {
            this.Culture = defaultCulture;
        }

        public BasicCountry (int countryId, string name, string code, string defaultCulture, int geographyId, int postalCodeLength)
            : this(countryId, name, code, defaultCulture)
        {
            this.GeographyId = geographyId;
            this.PostalCodeLength = postalCodeLength;
        }

        public BasicCountry (BasicCountry original)
            : this(original.CountryId, original.Name, original.Code, original.Culture, original.GeographyId, original.PostalCodeLength )
        {
        }

        [Obsolete ("Do not call this function directly. It exists to enable serialization.", true)]
        public BasicCountry()
        {
            // this does not initialize the Basic Country, but exists to enable serialization.
        }

        public int CountryId { get; private set; }
        public string Name { get; protected set; }
        public string Code { get; protected set; }
        public string Culture { get; protected set; }
        public int GeographyId { get; protected set; }
        public int PostalCodeLength { get; protected set; }

        public int CurrencyId
        {
            get
            {
                if (this.Code == "SE")
                {
                    return 1;
                }

                throw new NotImplementedException("BasicCountry.CurrencyId");
            }
        }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.CountryId; }
        }

        #endregion
    }
}