using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Structure
{
    public class Cities : List<City>
    {
        public static Cities FromArray (BasicCity[] array)
        {
            var result = new Cities {Capacity = (array.Length*11/10)};

            foreach (BasicCity basic in array)
            {
                result.Add(City.FromBasic(basic));
            }

            return result;
        }


        public static Cities FromPostalCode (string postalCode, Country country)
        {
            return FromPostalCode(postalCode, country.Identity);
        }

        public static Cities FromPostalCode (string postalCode, string countryCode)
        {
            return FromPostalCode(postalCode, Country.FromCode(countryCode).Identity);
        }

        public static Cities FromPostalCode (string postalCode, int countryId)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetCitiesByCountryAndPostalCode(countryId, postalCode));
        }
        
        public static Cities FromName (string cityName, int countryId)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetCitiesByName(cityName, countryId));
        }

        public static Cities ForCountry (string countryCode)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetCitiesByCountry (countryCode));
        }
    }
}