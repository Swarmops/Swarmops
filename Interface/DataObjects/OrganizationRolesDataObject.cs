using System;
using System.Collections.Generic;
using System.ComponentModel;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Interface.DataObjects
{
#if !__MonoCS__
    [DataObject(true)]
#endif
    public class OrganizationRolesDataObject
    {
        public class Role : BasicPersonRole
        {
            public Role()
            {
            }

            public Role(BasicPersonRole org)
                : base(org)
            {
                this.OrganisationName = Organization.FromIdentity(OrganizationId).Name;
                this.PersonName = Person.FromIdentity(PersonId).Name;
                this.RoleName = Enum.GetName(Type.GetType(), Type);
                this.GeographyName = "";
                if (RoleTypes.ClassOfRole(Type) == RoleClass.Local)
                {
                    if (GeographyId > 0)
                    {
                        this.GeographyName = Geography.FromIdentity(GeographyId).Name;
                    }
                }
            }

            public string GeographyName { get; set; }

            public int GeoLevel { get; set; }

            public int Level { get; set; }

            public string OrganisationName { get; set; }

            public string PersonName { get; set; }

            public string RoleName { get; set; }
        }

        private static readonly object dataLock = new object();

        private static Dictionary<int, Geographies.HierSortOrder> hierarchicalSortOrder =
            new Dictionary<int, Geographies.HierSortOrder>();


#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static Role[] SelectAllByOrganization(int organizationId)
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
                return new Role[0];
            }
        }

        private static void RecursiveAddInTreeOrder(List<Role> resultList, Dictionary<int, List<Role>> roleDictionary,
            int currentOrg, int level)
        {
            if (roleDictionary.ContainsKey(currentOrg))
            {
                roleDictionary[currentOrg].Sort(
                    delegate(Role r1, Role r2)
                    {
                        int res = 0;
                        try
                        {
                            res = ((int) RoleTypes.ClassOfRole(r1.Type)).CompareTo((int) RoleTypes.ClassOfRole(r2.Type));
                        }
                        catch
                        {
                        }
                        try
                        {
                            if (res == 0)
                                res =
                                    hierarchicalSortOrder[r1.GeographyId].Order.CompareTo(
                                        hierarchicalSortOrder[r2.GeographyId].Order);
                        }
                        catch
                        {
                        }
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
        public static Role[] SelectSubByOrganization(int organizationId)
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
                return new Role[0];
            }
        }

#if !__MonoCS__
        [DataObjectMethod(DataObjectMethodType.Select)]
#endif
        public static Role[] SelectThisByOrganization(int organizationId)
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
                return new Role[0];
            }
        }
    }
}