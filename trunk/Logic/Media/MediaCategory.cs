using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Media
{
    public class MediaCategory : BasicMediaCategory
    {
        private MediaCategory (BasicMediaCategory original) : base (original)
        {
        }

        public static MediaCategory FromName (string name)
        {
            return new MediaCategory(PirateDb.GetDatabaseForReading().GetMediaCategoryByName(name));
        }

        public static MediaCategory FromBasic (BasicMediaCategory basic)
        {
            return new MediaCategory (basic);
        }
    }
}