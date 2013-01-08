using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Interface.Collections;
using Swarmops.Interface.Objects;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using Swarmops.Basic.Types;

namespace Swarmops.Interface.DataObjects
{
#if !__MonoCS__
    [DataObject(true)]
#endif

    public class OrganizationRolesDataObject
    {
        public class Role : BasicPersonRole
        {
            private string organisationName;
            private string geographyName;
            private string personName;
            private string roleName;
            private int level;
            private int geoLevel;

            public Role ()
                : base()
            {
            }

            public Role (BasicPersonRole org)
                : base(org)
            {
                organisationName = Organization.FromIdentity(this.OrganizationId).Name;
                personName = Person.FromIdentity(this.PersonId).Name;
                roleName = Enum.GetName(this.Type.GetType(), this.Type);
                geographyName = "";
                if (RoleTypes.ClassOfRole(this.Type) == RoleClass.Local)
                {
                    if (GeographyId > 0)
                    {
                        geographyName = Geography.FromIdentity(GeographyId).Name;
                    }
                }
            }

            public string GeographyName
            {
                get
                {
                    return geographyName;
                }
                set
                {
                    geographyName = value;
                }
            }
            public int GeoLevel
            {
                get
                {
                    return geoLevel;
                }
                set
                {
                    geoLevel = value;
                }
            }
            public int Level
            {
                get
                {
                    return level;
                }
                set
                {
                    level = value;
                }
            }
            public string OrganisationName
            {
                get
                {
                    return organisationName;
                }
                set
                {
                    organisationName = value;
                }
            }
            public string PersonName
            {
                get
                {
                    return personName;
                }
                set
                {
                    personName = value;
                }
            }
            public string RoleName
            {
                get
                {
                    return roleName;
                }
                set
                {
                    roleName = value;
                }
            }

        }

        static private object dataLock = new object();
        static private Dictionary<int, Geographies.HierSortOrder> hierarchicalSortOrder = new Dictionary<int, Geographies.HierSortOrder>();



#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        static public OrganizationRolesDataObject.Role[] SelectAllByOrganization (int organizationId)
        {
            try
            {
                BasicPersonRole[] personRoles = Roles.GetAllDownwardRolesRoles(organizationId, Geography.IgnoreGeography);
                Dictionary<int, List<Role>> result = new Dictionary<int, List<Role>>();
                foreach (BasicPersonRole br in personRoles)
                {
                    if (!result.ContainsKey(br.OrganizationId))
                        result[br.OrganizationId] = new List<Role>();
                    result[br.OrganizationId].Add(new Role(br));
                }
                List<Role> result2 = new List<Role>();
                lock (dataLock)
                {
                    hierarchicalSortOrder = Geographies.GetHierarchicalSortOrder();
                    RecursiveAddInTreeOrder(result2, result, organizationId, 0);
                }
                return result2.ToArray();
            }
            catch
            {
                return new OrganizationRolesDataObject.Role[0];
            }
        }

        static private void RecursiveAddInTreeOrder (List<Role> resultList, Dictionary<int, List<Role>> roleDictionary, int currentOrg, int level)
        {
            if (roleDictionary.ContainsKey(currentOrg))
            {
                roleDictionary[currentOrg].Sort(
                    delegate(Role r1, Role r2)
                    {
                        int res = 0;
                        try
                        {
                            res = ((int)RoleTypes.ClassOfRole(r1.Type)).CompareTo((int)RoleTypes.ClassOfRole(r2.Type));
                        }
                        catch { }
                        try
                        {
                            if (res == 0)
                                res = hierarchicalSortOrder[r1.GeographyId].Order.CompareTo(hierarchicalSortOrder[r2.GeographyId].Order);
                        }
                        catch { }
                        if (res == 0)
                            res = r1.Type.CompareTo(r2.Type);
                        return res;

                    });
                foreach (Role role in roleDictionary[currentOrg])
                {
                    role.Level = level;
                    role.GeoLevel = hierarchicalSortOrder[role.GeographyId].Level;
                    resultList.Add(role);
                }
            }
            Organizations children = Organization.FromIdentity(currentOrg).Children;
            foreach (Organization org in children)
                RecursiveAddInTreeOrder(resultList, roleDictionary, org.OrganizationId, level + 1);
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        static public OrganizationRolesDataObject.Role[] SelectSubByOrganization (int organizationId)
        {
            try
            {
                BasicPersonRole[] personRoles = Roles.GetAllDownwardRolesRoles(organizationId, Geography.IgnoreGeography);
                Dictionary<int, List<Role>> result = new Dictionary<int, List<Role>>();
                foreach (BasicPersonRole br in personRoles)
                {
                    if (br.OrganizationId != organizationId)
                    {
                        if (!result.ContainsKey(br.OrganizationId))
                            result[br.OrganizationId] = new List<Role>();
                        result[br.OrganizationId].Add(new Role(br));
                    }
                }
                List<Role> result2 = new List<Role>();
                lock (dataLock)
                {
                    hierarchicalSortOrder = Geographies.GetHierarchicalSortOrder();
                    RecursiveAddInTreeOrder(result2, result, organizationId, 0);
                }
                return result2.ToArray();
            }
            catch
            {
                return new OrganizationRolesDataObject.Role[0];
            }
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        static public OrganizationRolesDataObject.Role[] SelectThisByOrganization (int organizationId)
        {
            try
            {
                BasicPersonRole[] personRoles = Roles.GetAllDownwardRolesRoles(organizationId, Geography.IgnoreGeography);
                Dictionary<int, List<Role>> result = new Dictionary<int, List<Role>>();
                foreach (BasicPersonRole br in personRoles)
                {
                    if (br.OrganizationId == organizationId)
                    {
                        if (!result.ContainsKey(br.OrganizationId))
                            result[br.OrganizationId] = new List<Role>();
                        result[br.OrganizationId].Add(new Role(br));
                    }
                }
                List<Role> result2 = new List<Role>();
                lock (dataLock)
                {
                    hierarchicalSortOrder = Geographies.GetHierarchicalSortOrder();
                    RecursiveAddInTreeOrder(result2, result, organizationId, 0);
                }
                return result2.ToArray();
            }
            catch
            {
                return new OrganizationRolesDataObject.Role[0];
            }
        }
    }
}
