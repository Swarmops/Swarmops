using System;
using System.Collections.Generic;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Pirates
{
    public class Memberships : List<Membership>
    {
        public static Memberships FromArray (BasicMembership[] basicArray)
        {
            var result = new Memberships();

            result.Capacity = basicArray.Length*11/10;
            foreach (BasicMembership basic in basicArray)
            {
                result.Add(Membership.FromBasic(basic));
            }

            return result;
        }
        
        public int[] Identities
        {
            get { return LogicServices.ObjectsToIdentifiers(ToArray()); }
        }


        /*
		public static BasicMembership[] GetMembershipsForPerson (int personId)
		{
			return LogicServices.GetDatabase().GetMembershipsForPerson(personId);
		}*/

        public static int GetMemberCountForOrganization (Organization organization)
        {
            return PirateDb.GetDatabase().GetMembershipCountForOrganizations(new[] {organization.Identity});
        }

        public static int GetMemberCountForOrganizations (int[] organizationIds)
        {
            return PirateDb.GetDatabase().GetMembershipCountForOrganizations(organizationIds);
        }

        public static int GetMemberCountForOrganizationSince (Organization org, DateTime sinceDateTime)
        {
            return GetMemberCountForOrganizationSince(org.Identity, sinceDateTime);
        }

        public static int GetMemberCountForOrganizationSince (int orgId, DateTime sinceDateTime)
        {
            return PirateDb.GetDatabase().GetMembershipCountForOrganizationsSince(new[] {orgId}, sinceDateTime);
        }

        public static int GetMemberCountForOrganizations (Organizations organizations)
        {
            var idArray = new List<int>();

            foreach (Organization organization in organizations)
            {
                idArray.Add(organization.Identity);
            }

            return GetMemberCountForOrganizations(idArray.ToArray());
        }

        public Memberships RemoveToMatchAuthority (Geography personGeography, Authority authority)
        {
            return Authorization.FilterMembershipsToMatchAuthority(this, personGeography, authority);
        }

        public static Memberships ForOrganization (Organization organization, bool includeTerminated)
        {
            if (!includeTerminated)
            {
                return ForOrganization(organization);
            }

            return FromArray(PirateDb.GetDatabase().GetMemberships(organization));
        }

        public static Memberships ForOrganization (Organization organization)
        {
            return FromArray(PirateDb.GetDatabase().GetMemberships(organization, DatabaseCondition.ActiveTrue));
        }

        public static Memberships ForOrganizations (Organizations organizations, bool includeTerminated)
        {
            if (!includeTerminated)
            {
                return ForOrganizations(organizations);
            }

            return FromArray(PirateDb.GetDatabase().GetMemberships(organizations));
        }

        public static Memberships ForOrganizations (Organizations organizations)
        {
            return FromArray(PirateDb.GetDatabase().GetMemberships(organizations, DatabaseCondition.ActiveTrue));
        }

        public static Memberships GetExpiring (Organization organization, DateTime dateExpiry)
        {
            return GetExpiring(organization, dateExpiry, dateExpiry);
        }

        public static Memberships GetExpired (Organization organization)
        {
            return
                FromArray(PirateDb.GetDatabase().GetExpiringMemberships(organization, DateTime.MinValue,
                                                                        DateTime.Now));
        }
        public static Memberships GetExpired (Organization organization, DateTime lowerBound, DateTime upperBound)
        {
            return FromArray(PirateDb.GetDatabase().GetExpiringMemberships(organization, lowerBound, upperBound, DatabaseCondition.None));
        }

        public static Memberships GetExpiring (Organization organization, DateTime lowerBound, DateTime upperBound)
        {
            return FromArray(PirateDb.GetDatabase().GetExpiringMemberships(organization, lowerBound, upperBound));
        }


        public static Dictionary<int, List<BasicMembership>> GetMembershipsForPeople (int[] personIds, int gracePeriod)
        {
            return PirateDb.GetDatabase().GetMembershipsForPeople(personIds, gracePeriod);
        }

    }
}