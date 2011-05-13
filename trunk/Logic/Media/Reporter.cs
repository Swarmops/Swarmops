using System;

using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Media
{
    public class Reporter : BasicReporter, IComparable
    {
        private MediaCategories mediaCategories;

        private Reporter (BasicReporter original) : base (original)
        {
        }

        public static Reporter Create (string name, string email, string[] categories)
        {
            Reporter reporter = FromIdentity(PirateDb.GetDatabase().CreateReporter(name, email));

            foreach (string category in categories)
            {
                PirateDb.GetDatabase().CreateReporterMediaCategory(reporter.Identity, category);
            }

            return reporter;
        }


        public void Delete()
        {
            PirateDb.GetDatabase().DeleteReporter(this.Identity);

            // After this operation, the object is no longer valid.
        }


        public MediaCategories MediaCategories
        {
            get
            {
                if (MediaCategoryIds == null)
                {
                    MediaCategoryIds = PirateDb.GetDatabase().GetReporterMediaCategories (Identity);
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
            return Reporter.FromBasic(PirateDb.GetDatabase().GetReporter(identity));
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