using System;
using System.Collections.Generic;
using System.Text;
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

        public BasicCountry (int countryId, string name, string code, string defaultCulture, int geographyId)
            : this(countryId, name, code, defaultCulture)
        {
            this.GeographyId = geographyId;
        }

        public BasicCountry (BasicCountry original)
            : this(original.CountryId, original.Name, original.Code, original.Culture, original.GeographyId )
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
        public int PostalCodeLength
        {
            get
            {
                if (this.Identity == 5)
                {
                    return 4;
                }
                else if (this.Identity == 1)
                {
                    return 5;
                }
                else
                {
                    return 0;
                }
            }
        }


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