using System.ComponentModel;
using Activizr.Logic.Structure;
using Activizr.Logic.Pirates;
using System;
using System.Collections.Generic;
using Activizr.Database;

namespace Activizr.Logic.DataObjects
{
#if !__MonoCS__
    [DataObject(true)]
#endif
    public class GeographyWithPeopleDataObject
    {

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static GeographyWithPeople[] SelectSortedStatic (int orgId)
        {
            Organizations orgs = Organization.FromIdentity(orgId).GetTree();

            List<GeographyWithPeople> resList = new List<GeographyWithPeople>();
            Geographies tree = Geography.Root.GetTree();
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
                Geographies gTree = geo.GetTree();
                int[] members = Activizr.Database.PirateDb.GetDatabase().GetMembersForOrganizationsAndGeographies(orgs.Identities, gTree.Identities);


            }


            return resList.ToArray();

        }

    }
}