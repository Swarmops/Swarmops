using System;
using Activizr.Logic.Financial;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Structure
{
    [Serializable]
    public class Country : BasicCountry
    {
        private Country (BasicCountry basic)
            : base(basic)
        {
        }

        [Obsolete ("Do not call this function directly. It is only intended for use in serialization.", true)]
        public Country(): base (0, string.Empty, string.Empty)
        {
            // this ctor does NOT initialize the instance, but is provided to allow for serialization
        }

        public static Country FromBasic (BasicCountry basic)
        {
            return new Country(basic);
        }

        // TODO: Cache these two functions, country info doesn't change

        public static Country FromIdentity (int identity)
        {
            return FromBasic(PirateDb.GetDatabase().GetCountry(identity));
        }

        public static Country FromCode (string countryCode)
        {
            return FromBasic(PirateDb.GetDatabase().GetCountry(countryCode));
        }

        public Geography Geography
        {
            get { return Geography.FromIdentity(this.GeographyId); }
        }

        public Currency Currency
        {
            get { return Financial.Currency.FromIdentity(this.CurrencyId); }
        }

    }
}