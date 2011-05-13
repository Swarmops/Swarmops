using System.Collections.Generic;

using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Structure
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
            return FromArray(PirateDb.GetDatabase().GetCitiesByCountryAndPostalCode(countryId, postalCode));
        }
        
        public static Cities FromName (string cityName, int countryId)
        {
            return FromArray(PirateDb.GetDatabase().GetCitiesByName(cityName, countryId));
        }

    }
}