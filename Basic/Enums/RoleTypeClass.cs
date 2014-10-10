using System;
using System.Collections.Generic;

namespace Swarmops.Basic.Enums
{
    /// <summary>
    /// RoleTypes. 
    /// Important that the number corresponds 
    /// to RoleTypeId in database since roles are stored referencing 
    /// id rather than name.
    /// 
    /// Add new roles at the end, and create corresponding record in db.
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// Undefined
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The primary contact for a geographical node in an organization
        /// </summary>
        LocalLead = 1,
        /// <summary>
        /// The secondary contact for a geographical node in an organization
        /// </summary>
        LocalDeputy = 2,
        /// <summary>
        /// OBSOLETE - DO NOT USE
        /// </summary>
        LocalActive = 3,
        /// <summary>
        /// Non-public role - full access to organization's node
        /// </summary>
        LocalAdmin = 4,
        /// <summary>
        /// Public role - PoC for org and can perform any task on the org in the system
        /// </summary>
        OrganizationChairman = 5,
        /// <summary>
        /// Treasurers in an org can approve and manage expenses
        /// </summary>
        OrganizationTreasurer = 6,
        /// <summary>
        /// Public role - for display / PoC purposes only
        /// </summary>
        OrganizationBoardMember = 7,
        /// <summary>
        /// Public role - for display / PoC purposes only
        /// </summary>
        OrganizationSecretary = 8,
        /// <summary>
        /// Non-public role for people who do administrative work
        /// </summary>
        OrganizationAdmin = 9,
        /// <summary>
        /// Non-public role - full access to entire PirateWeb
        /// </summary>
        SystemAdmin = 10,
        /// <summary>
        /// Non-public role - editor of the geography tree (at a certain geo and below)
        /// </summary>
        SystemGeographyEditor = 11,
        /// <summary>
        /// Non-public role - a member/nonmember who has full access to the organization
        /// </summary>
        OrganizationAuditor = 12,
        /// <summary>
        /// A deputy board member of an org
        /// </summary>
        OrganizationBoardDeputy = 13,
        /// <summary>
        /// First vice chairman
        /// </summary>
        OrganizationVice1 = 14,
        /// <summary>
        /// Second vice chairman
        /// </summary>
        OrganizationVice2 = 15,
        /// <summary>
        /// This Role is assigned to Memberservice administrators
        /// </summary>
        OrganizationMemberService = 17,
        /// <summary>
        /// This Role is assigned to Memberservice administrators, extra permissions
        /// </summary>
        OrganizationMemberServiceLead = 18,
        /// <summary>
        /// Role assigned to elected representatives, mostly for admin purposes. No intrinsic rights.
        /// </summary>
        OrganizationElectedRepresentative = 19,
        /// <summary>
        /// A role that everybody has, although it is not set in the database on every person. 
        /// Letters may be sent to people in this role, in which case they are private. No intrinsic rights.
        /// </summary>
        PrivateIndividual = 20,
        /// <summary>
        /// A person who can upload paper documents, like letters/invoices, but not access members.
        /// </summary>
        OrganizationClerk = 21,
        /// <summary>
        /// A person who can see and manage all of the economy, but not member registry.
        /// </summary>
        OrganizationEconomyAssistant = 22,
        /// <summary>
        /// An elected meeting official, to be assigned temporarily during a meeting
        /// </summary>
        OrganizationMeetingOfficial = 23,
        /// <summary>
        /// The board's secretary. Not the same as the org's secretary.
        /// </summary>
        OrganizationBoardSecretary = 24,
        /// <summary>
        /// Role for non officers helping with ballot distribution etc
        /// </summary>
        LocalElectionSupport = 25,
        /// <summary>
        /// Temporary - Jörgen added this; the whole enum is deprecated anyway; added for compatibility with v4
        /// </summary>
        OrganizationMailSender
    }


    public enum RoleClass
    {
        /// <summary>
        /// Undefined
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// System-wide privileges and responsibilities
        /// </summary>
        System,
        /// <summary>
        /// Roles for an entire organization
        /// </summary>
        Organization,
        /// <summary>
        /// Roles and responsibilities to a node in an organization
        /// </summary>
        Local,
        /// <summary>
        /// Roles that are org-agnostic but are tied to a geo node
        /// </summary>
        Geographic
    }

    public class RoleTypes
    {
        static public readonly RoleType[] AllLocalRoleTypes = new RoleType[] 
                    { RoleType.LocalLead, 
                    RoleType.LocalDeputy, 
                    RoleType.LocalActive, 
                    RoleType.LocalAdmin,
                    RoleType.LocalElectionSupport };

        static public readonly RoleType[] AllOrganizationalRoleTypes = new RoleType[] 
                { RoleType.OrganizationChairman,
                RoleType.OrganizationVice1,
                RoleType.OrganizationVice2, 
                RoleType.OrganizationTreasurer,
                RoleType.OrganizationSecretary,
                RoleType.OrganizationBoardMember, 
                RoleType.OrganizationBoardDeputy, 
                RoleType.OrganizationAdmin,
                RoleType.OrganizationClerk,
                RoleType.OrganizationAuditor,
                RoleType.OrganizationMemberService,
                RoleType.OrganizationMemberServiceLead,
                RoleType.OrganizationElectedRepresentative,
                RoleType.OrganizationEconomyAssistant,
                RoleType.OrganizationMeetingOfficial,
                RoleType.OrganizationBoardSecretary };

        static public readonly RoleType[] AllSystemRoleTypes = new RoleType[] 
                { RoleType.SystemAdmin, 
                RoleType.SystemGeographyEditor };

        static public readonly RoleType[] AllRoleTypes = CreateAllRoleTypes();

        private static RoleType[] CreateAllRoleTypes ()
        {
            List<RoleType> retList=new List<RoleType>();
            retList.AddRange(AllOrganizationalRoleTypes);
            retList.AddRange(AllSystemRoleTypes);
            retList.AddRange(AllLocalRoleTypes);
            foreach(RoleType rt in Enum.GetValues(typeof(RoleType)))
            {
                if (!retList.Contains(rt))
                    retList.Add(rt);
            }

            return retList.ToArray();
        }

        static public RoleClass ClassOfRole (RoleType r)
        {
            if (Array.IndexOf(AllLocalRoleTypes, r) > -1)
                return RoleClass.Local;
            if (Array.IndexOf(AllOrganizationalRoleTypes, r) > -1)
                return RoleClass.Organization;
            if (Array.IndexOf(AllSystemRoleTypes, r) > -1)
                return RoleClass.System;
            return RoleClass.Undefined;
        }
    }
}