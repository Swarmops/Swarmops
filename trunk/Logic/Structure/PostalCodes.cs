using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Structure
{
    [Serializable]
    public class PostalCodes: List<PostalCode>
    {
        public static PostalCodes FromArray(BasicPostalCode[] array)
        {
            var result = new PostalCodes { Capacity = (array.Length * 11 / 10) };

            foreach (BasicPostalCode basic in array)
            {
                result.Add(PostalCode.FromBasic(basic));
            }

            return result;
        }

        public static PostalCodes ForCountry (string countryCode)
        {
            return ForCountry(Country.FromCode(countryCode).Identity);
        }

        public static PostalCodes ForCountry (int countryId)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetPostalCodesForCountry(countryId));
        }
    }
}
