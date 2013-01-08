using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Pirates;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

namespace Swarmops.Interface
{
    public class PersonIcon
    {
        public class PersonIconSpec
        {
            public PersonIconSpec (string image, string altText)
            {
                this.Image = image;
                this.AltText = altText;

            }

            public string Image = "";
            public string AltText = "";
        }

        public static PersonIconSpec ForPerson (Person person, Organizations organizations)
        {
            // HACK: For now, cheat and assume only PPSE roles matter. A future expansion of this is needed.
            // HACK: Rehacked to use another org in case there is one and PPSE is not among selected
            Organization org = null;

            if (organizations.Contains(Organization.PPSE) || organizations.Count == 0)
            {
                org = Organization.PPSE;
            }
            else
            {
                org = organizations[0];
            }

            bool isMember = false;
            foreach (Organization org1 in organizations)
            {
                if (person.MemberOf(org1)) isMember = true;
            }

            if (!isMember)
            {
                // if he/she used to be a member, use exmember icons

                Memberships memberships = person.GetMemberships(true);

                foreach (Membership membership in memberships)
                {
                    if (membership.Organization.Identity == org.Identity)
                    {
                        return new PersonIconSpec("pwcustom/exmember-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Expired member, " + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                    }
                }

                // If activist, use activist icon (as sex is unknown, use silhouette)

                if (person.IsActivist)
                {
                    return new PersonIconSpec("pwcustom/activist-silhouette.png", "Activist");
                }

                // TODO: Add case for was-activist

                // otherwise, use unknown icon

                return new PersonIconSpec("user-silhouette.png", "Unknown");
            }

            Authority auth = person.GetAuthority();

            if (auth.SystemPersonRoles.Length == 0 && auth.LocalPersonRoles.Length == 0 && auth.OrganizationPersonRoles.Length == 0)
            {
                // This is a fairly regular joe.

                if (person.IsActivist)
                {
                    return new PersonIconSpec("pwcustom/activist-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Activist, " + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }
                else
                {
                    return new PersonIconSpec("pwcustom/member-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Member, " + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }
            }

            // HACK: Cheat and return "org leader" for ID 1

            if (person.Identity == 1)
            {
                return new PersonIconSpec("pwcustom/orglevel-3-male.png", "Org Lead");
            }
            bool foundOddRole = false;
            foreach (Organization org1 in organizations)
            {
                string orgName = org1.NameShort + ", ";

                if (org1 == Organization.PPSE)
                    orgName = "";


                // Chairman / equivalent

                if (auth.HasRoleAtOrganization(org1, RoleType.OrganizationChairman, Authorization.Flag.AnyGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/boardlevel-3-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Chairman, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }

                // Board member

                if (auth.HasRoleAtOrganization(org1, RoleType.OrganizationBoardMember, Authorization.Flag.AnyGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/boardlevel-2-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Board member, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }

                // Board deputy

                if (auth.HasRoleAtOrganization(org1, RoleType.OrganizationBoardDeputy, Authorization.Flag.AnyGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/boardlevel-1-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Board deputy, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }

                // Auditor

                if (auth.HasRoleAtOrganization(org1, RoleType.OrganizationAuditor, Authorization.Flag.AnyGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/boardlevel-1-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Autitor, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }

                // Org-level leader or deputy

                if (auth.HasLocalRoleAtOrganizationGeography(org1, org1.PrimaryGeography, RoleType.LocalLead, Authorization.Flag.ExactGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/orglevel-3-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Org Lead, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }
                if (auth.HasLocalRoleAtOrganizationGeography(org1, org1.PrimaryGeography, RoleType.LocalDeputy, Authorization.Flag.ExactGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/orglevel-3-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Org Deputy, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }

                // Org-level secretary

                if (auth.HasRoleAtOrganization(org1, RoleType.OrganizationSecretary, Authorization.Flag.AnyGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/orglevel-2-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Org secretary, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }

                // system roles

                if (auth.HasRoleType(RoleType.SystemAdmin))
                {
                    return new PersonIconSpec("user-worker.png", "Sys.adm., " + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }

                // Org-level admin

                if (auth.HasRoleAtOrganization(org1, RoleType.OrganizationAdmin, Authorization.Flag.AnyGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/orglevel-1-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Org admin, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }
                if (auth.HasLocalRoleAtOrganizationGeography(org1, org1.PrimaryGeography, RoleType.LocalAdmin, Authorization.Flag.ExactGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/orglevel-1-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Org admin, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }

                // TODO: ADD THE REST OF THE ROLES HERE SOMEWHERE

                // local roles

                if (auth.HasLocalRoleAtOrganizationGeography(org1, Geography.Root, RoleType.LocalLead, Authorization.Flag.AnyGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/officer-lead-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Local lead, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }

                if (auth.HasLocalRoleAtOrganizationGeography(org1, Geography.Root, RoleType.LocalDeputy, Authorization.Flag.AnyGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/officer-deputy-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Local deputy, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }

                if (auth.HasLocalRoleAtOrganizationGeography(org1, Geography.Root, RoleType.LocalAdmin, Authorization.Flag.AnyGeographyExactOrganization))
                {
                    return new PersonIconSpec("pwcustom/officer-admin-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Local admin, " + orgName + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }

                if (auth.HasRoleAtOrganization(org1, Authorization.Flag.AnyGeographyExactOrganization)
                 || auth.HasLocalRoleAtOrganizationGeography(org1, Geography.Root, Authorization.Flag.AnyGeographyExactOrganization))
                {
                    foundOddRole = true;
                }
            }

            if (!foundOddRole)
            {
                if (person.IsActivist)
                {
                    return new PersonIconSpec("pwcustom/activist-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Activist, " + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }
                else
                {
                    return new PersonIconSpec("pwcustom/member-" + (person.IsFemale ? "female" : person.IsMale ? "male" : "") + ".png", "Member, " + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
                }
            }
            else
            {
                // unhandled role type
                return new PersonIconSpec("user-silhouette-question.png", "unknown role, " + (person.IsFemale ? "female" : person.IsMale ? "male" : ""));
            }
        }
    }
}
