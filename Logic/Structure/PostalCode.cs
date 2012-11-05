using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Activizr.Basic.Types;

namespace Activizr.Logic.Structure
{
    [Serializable]
    public class PostalCode: BasicPostalCode
    {
        public PostalCode (BasicPostalCode basic):
                base (basic)
        {
            // constructor
        }

        static public PostalCode FromBasic (BasicPostalCode basic)
        {
            return new PostalCode(basic);
        }

        public Country Country
        {
            get { return Country.FromCode(this.CountryCode); }
        }

        public City City
        {
            get { return City.FromName(this.CityName, this.CountryCode); }
        }

    }
}
