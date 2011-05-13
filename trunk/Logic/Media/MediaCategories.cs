using System.Collections.Generic;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Media
{
    public class MediaCategories : List<MediaCategory>
    {
        public int[] Identities
        {
            get { return LogicServices.ObjectsToIdentifiers (ToArray()); }
        }

        public static MediaCategories FromArray (BasicMediaCategory[] basicArray)
        {
            var result = new MediaCategories();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicMediaCategory basic in basicArray)
            {
                result.Add (MediaCategory.FromBasic (basic));
            }

            return result;
        }

        public static MediaCategories FromIdentities (int[] identities)
        {
            return FromArray (PirateDb.GetDatabase().GetMediaCategories (identities));
        }

        public static MediaCategories GetAll()
        {
            return FromArray(PirateDb.GetDatabase().GetMediaCategories());
        }
    }
}