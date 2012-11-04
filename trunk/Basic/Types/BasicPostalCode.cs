﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Activizr.Basic.Types
{
    [Serializable]
    public class BasicPostalCode
    {
        public BasicPostalCode(string postalCode, string cityName, string countryCode)
        {
            this.PostalCode = postalCode.Replace(" ", "");
            this.CityName = cityName;
            this.CountryCode = countryCode;
        }

        public BasicPostalCode(BasicPostalCode original)
            : this(original.PostalCode, original.CityName, original.CountryCode)
        {
        }

        public string PostalCode { get; private set; }  // any whitespace removed
        public string CityName { get; protected set; }
        public string CountryCode { get; protected set; }
    }
}
