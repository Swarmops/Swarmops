using System;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Structure;

namespace Swarmops.Logic.Structure
{
    [Serializable]
    public class PostalCode : BasicPostalCode
    {
        private PostalCode (BasicPostalCode basic) :
            base (basic)
        {
            // constructor
        }

        [Obsolete ("Do not call this constructor directly. It is intended only for serialization.", true)]
        public PostalCode() : base (0, string.Empty, 0, 0)
        {
            // this ctor does NOT initialize the instance. It is provided to allow for serialization.
        }

        public Country Country
        {
            get { return Country.FromIdentity (CountryId); }
        }

        public City City
        {
            get { return City.FromIdentity (CityId); }
        }

        public string CityName
        {
            get { return City.Name; }
        }

        public static PostalCode FromBasic (BasicPostalCode basic)
        {
            return new PostalCode (basic);
        }
    }
}