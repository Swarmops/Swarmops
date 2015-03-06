using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Structure
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

        public BasicCountry (int countryId, string name, string code, string defaultCulture, string currencyCode,
            int geographyId, int postalCodeLength, string postalCodeFormat)
            : this (countryId, name, code)
        {
            this.Culture = defaultCulture;
            this.CurrencyCode = currencyCode;
            this.GeographyId = geographyId;
            this.PostalCodeLength = postalCodeLength;
            this.PostalCodeFormat = postalCodeFormat;
        }

        public BasicCountry (BasicCountry original)
            : this (
                original.CountryId, original.Name, original.Code, original.Culture, original.CurrencyCode,
                original.GeographyId, original.PostalCodeLength, original.PostalCodeFormat)
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
        public string Culture { get; protected set; }  // may be overridden at geography level (multilinguality etc.)
        public string CurrencyCode { get; protected set; }
        public int GeographyId { get; protected set; }
        public int PostalCodeLength { get; protected set; }
        public string PostalCodeFormat { get; protected set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.CountryId; }
        }

        #endregion
    }
}