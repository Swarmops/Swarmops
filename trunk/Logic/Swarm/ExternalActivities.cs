using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class ExternalActivities : PluralBase<ExternalActivities, ExternalActivity, BasicExternalActivity>
    {
        public static ExternalActivities ForOrganization (Organization organization)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetExternalActivities(organization));
        }

        public static ExternalActivities ForOrganization (Organization organization, SortOrder sort, int count)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetExternalActivitiesSorted(sort.ToString(), count, organization));
        }

        public enum SortOrder
        {
            Unknown = 0,
            /// <summary>
            /// By date/time of the activism, newest first
            /// </summary>
            DateDescending,
            /// <summary>
            /// By date/time of the upload, newest first
            /// </summary>
            CreationDateDescending
        }

        public void Sort (SortOrder order)
        {
            switch (order)
            {
                case SortOrder.DateDescending:
                    this.Sort(ExternalActivity.CompareDateDescending);
                    break;
                case SortOrder.CreationDateDescending:
                    this.Sort(ExternalActivity.CompareCreationDateDescending);
                    break;
                default:
                    throw new NotImplementedException("Unimplemented sort order");
            }

        }
    }
}
