using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;


namespace Activizr.Basic.Types
{
    [Serializable]
    public class BasicCountry : IHasIdentity
    {
        public BasicCountry (int countryId, string name, string code)
        {
            this.countryId = countryId;
            this.name = name;
            this.code = code;
        }

        public BasicCountry (int countryId, string name, string code, string defaultCulture)
            : this(countryId, name, code)
        {
            this.culture = defaultCulture;
        }

        public BasicCountry (int countryId, string name, string code, string defaultCulture, int geoId)
            : this(countryId, name, code, defaultCulture)
        {
            this.geographyId = geoId;
        }

        public BasicCountry (BasicCountry original)
            : this(original.CountryId, original.Name, original.Code, original.Culture, original.GeographyId )
        {
        }

        public int CountryId
        {
            get { return countryId; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string Code
        {
            get { return this.code; }
        }

        public string Culture
        {
            get { return this.culture; }
        }

        public int GeographyId
        {
            get { return this.geographyId; }
        }


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


        private int countryId;
        private string name;
        private string code;
        private string culture;
        private int geographyId;

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