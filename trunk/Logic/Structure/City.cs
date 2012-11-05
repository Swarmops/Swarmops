using System;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Structure
{
    [Serializable]
    public class City : BasicCity
    {
        private City (BasicCity basic)
            : base(basic)
        {
        }


        public Geography Geography
        {
            get { return Geography.FromIdentity(GeographyId); } // TODO: Cache
        }

        public static City FromBasic (BasicCity basic)
        {
            return new City(basic);
        }

        public static City FromName (string cityName, int countryId)
        {
            return FromBasic(PirateDb.GetDatabase().GetCityByName(cityName, countryId));
        }
        
        public static City FromName (string cityName, string countryCode)
        {
            return FromBasic(PirateDb.GetDatabase().GetCityByName(cityName, countryCode));
        }
    }
}