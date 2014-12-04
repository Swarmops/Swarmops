using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Media
{
    public class MediaCategory : BasicMediaCategory
    {
        private MediaCategory(BasicMediaCategory original) : base(original)
        {
        }

        public static MediaCategory FromName(string name)
        {
            return new MediaCategory(SwarmDb.GetDatabaseForReading().GetMediaCategoryByName(name));
        }

        public static MediaCategory FromBasic(BasicMediaCategory basic)
        {
            return new MediaCategory(basic);
        }
    }
}