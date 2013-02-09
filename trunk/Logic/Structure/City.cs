using System;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Structure
{
    [Serializable]
    public class City : BasicCity
    {
        private City (BasicCity basic)
            : base(basic)
        {
        }

        // public ctor needed for serialization
        [Obsolete ("Do not call this function directly. It is intended only for use in serialization.", true)]
        public City(): base (0, string.Empty, 0, 0)
        {
            // this instance is NOT initalized, and intended to be used only in serialization.
        }

        public static City FromIdentity (int cityId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetCity(cityId));
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
            return FromBasic(SwarmDb.GetDatabaseForReading().GetCityByName(cityName, countryId));
        }
        
        public static City FromName (string cityName, string countryCode)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetCityByName(cityName, countryCode));
        }

        public static City Create (string cityName, int countryId, int geographyId)
        {
            return City.FromIdentity(SwarmDb.GetDatabaseForWriting().CreateCity(cityName, countryId, geographyId));
        }
    }
}