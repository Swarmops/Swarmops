using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Structure
{
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
        
    }
}