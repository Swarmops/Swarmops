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
        private PostalCode (BasicPostalCode basic):
                base (basic)
        {
            // constructor
        }

        [Obsolete ("Do not call this constructor directly. It is intended only for serialization.", true)]
        public PostalCode(): base (string.Empty, string.Empty, string.Empty)
        {
            // this ctor does NOT initialize the instance. It is provided to allow for serialization.
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
