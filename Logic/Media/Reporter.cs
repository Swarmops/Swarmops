using System;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Media
{
    public class Reporter : BasicReporter, IComparable
    {
        private MediaCategories mediaCategories;

        private Reporter (BasicReporter original) : base (original)
        {
        }

        public static Reporter Create (string name, string email, string[] categories)
        {
            Reporter reporter = FromIdentity(PirateDb.GetDatabaseForWriting().CreateReporter(name, email));

            foreach (string category in categories)
            {
                PirateDb.GetDatabaseForWriting().CreateReporterMediaCategory(reporter.Identity, category);
            }

            return reporter;
        }


        public void Delete()
        {
            PirateDb.GetDatabaseForWriting().DeleteReporter(this.Identity);

            // After this operation, the object is no longer valid.
        }


        public MediaCategories MediaCategories
        {
            get
            {
                if (MediaCategoryIds == null)
                {
                    MediaCategoryIds = PirateDb.GetDatabaseForReading().GetReporterMediaCategories (Identity);
                }

                if (this.mediaCategories == null)
                {
                    if (MediaCategoryIds.Length > 0)
                    {
                        this.mediaCategories = MediaCategories.FromIdentities(MediaCategoryIds);
                    }
                    else
                    {
                        this.mediaCategories = new MediaCategories();
                    }

                }

                return this.mediaCategories;
            }
        }

        public static Reporter FromBasic (BasicReporter basic)
        {
            return new Reporter (basic);
        }

        public static Reporter FromIdentity (int identity)
        {
            return Reporter.FromBasic(PirateDb.GetDatabaseForReading().GetReporter(identity));
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            Reporter otherReporter = (Reporter) obj;

            return this.Name.CompareTo(otherReporter.Name);
        }

        #endregion
    }
}