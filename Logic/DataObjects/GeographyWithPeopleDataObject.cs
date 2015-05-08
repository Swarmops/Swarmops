using System.Collections.Generic;
using System.ComponentModel;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.DataObjects
{
#if !__MonoCS__
    [DataObject (true)]
#endif
    public class GeographyWithPeopleDataObject
    {
#if !__MonoCS__
        [DataObjectMethod (DataObjectMethodType.Select)]
#endif
        public static GeographyWithPeople[] SelectSortedStatic (int orgId)
        {
            Organizations orgs = Organization.FromIdentity (orgId).ThisAndBelow();

            List<GeographyWithPeople> resList = new List<GeographyWithPeople>();
            Geographies tree = Geography.Root.ThisAndBelow();
            foreach (Geography geo in tree)
            {
                GeographyWithPeople row = new GeographyWithPeople();
                row.GeographyID = geo.Identity;
                row.Name = geo.Name;
                row.ActivistCount = 0;
                row.MemberCount = 0;
                row.LeadContent = "";
                row.SecondsContent = "";
                row.OrgId = orgId;
                Geographies gTree = geo.ThisAndBelow();
                int[] members = SwarmDb.GetDatabaseForReading()
                    .GetParticipantsForOrganizationsAndGeographies (orgs.Identities, gTree.Identities);
            }


            return resList.ToArray();
        }
    }
}