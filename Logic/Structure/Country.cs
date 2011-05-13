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