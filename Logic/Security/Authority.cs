using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Xml.Serialization;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Security;
using Swarmops.Basic.Types.Swarm;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Security
{
    public class Authority
    {
        public static Authority FromXml (string xml)
        {
            return new Authority (AuthorityData.FromXml (xml));
        }

        public static Authority FromEncryptedXml (string cryptXml)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(SystemSettings.InstallationId.Replace("-", ""));  // unique for this install
            byte[] cryptoBytes = Convert.FromBase64String(cryptXml);

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = keyBytes;
                aes.IV = new ArraySegment<byte> (cryptoBytes, 0, 16).ToArray();
                cryptoBytes = new ArraySegment<byte>(cryptoBytes, 16, cryptoBytes.Length - 16).ToArray();

                using (ICryptoTransform crypto = aes.CreateDecryptor())
                {
                    byte[] clearBytes = crypto.TransformFinalBlock(cryptoBytes, 0, cryptoBytes.Length);
                    return FromXml(Encoding.UTF8.GetString (clearBytes));
                }
            }
        }

        public string ToXml()
        {
            return _data.ToXml();
        }

        public string ToEncryptedXml()
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(SystemSettings.InstallationId.Replace ("-",""));  // unique for this install
            byte[] dataBytes = Encoding.UTF8.GetBytes (ToXml());

            using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
            {
                aes.Key = keyBytes;
                aes.GenerateIV(); // unique for every encryption

                if (aes.IV.Length != 16)
                {
                    throw new InvalidDataException("IV is not 16 bytes long");
                }

                using (ICryptoTransform crypto = aes.CreateEncryptor())
                {
                    byte[] cryptoBytes = crypto.TransformFinalBlock (dataBytes, 0, dataBytes.Length);
                    return Convert.ToBase64String (aes.IV.Concat (cryptoBytes).ToArray()); // joins two byte[] arrays
                }
            }
        }

        private Authority (AuthorityData data)
        {
            this._data = data;
        }

        public Person Person
        {
            get { return Person.FromIdentity (_data.PersonId); }
        }

        public Organization Organization
        {
            get { return Organization.FromIdentity (_data.OrganizationId); }
        }

        public void SetOrganization (Organization organization)
        {
            // This changes the active organization. There may be a Primary Position in this
            // organization; if so, activate that too. Otherwise, just null out the Position.

            // The reason this isn't an ordinary setter for the field is to minimize risk for
            // miscoding in the security framework.

            PositionAssignment primaryAssignment = Person.GetPrimaryAssignment (organization);

            _data.OrganizationId = organization.Identity;
            _data.PositionAssignmentId = (primaryAssignment == null ? 0 : primaryAssignment.Identity);
        }

        public void SetPosition (PositionAssignment assignment)
        {
            // This changes to a Position. The Organization will change along with it, but only
            // if it's not a system-level Position.

            // The reason this isn't an ordinary setter for the field is to minimize risk for
            // miscoding in the security framework.

            _data.PositionAssignmentId = (assignment == null ? 0 : assignment.Identity);
            if (assignment != null)
            {
                if (assignment.Position.PositionLevel != PositionLevel.SystemWide)
                {
                    _data.OrganizationId = assignment.Position.OrganizationId;
                }
            }
        }

        public PositionAssignment Assignment
        {
            get
            {
                if (_data.PositionAssignmentId == 0)
                {
                    return null;
                }

                return PositionAssignment.FromIdentity (_data.PositionAssignmentId);
            }
        }

        public Position Position
        {
            get { return Assignment != null? Assignment.Position: null; }
        }

        public static Authority FromLogin(Person person)
        {
            int lastOrgId = person.LastLogonOrganizationId;
            PositionAssignment assignment = null;

            if (lastOrgId != 0)
            {
                Organization organization = Organization.FromIdentity(lastOrgId);
                assignment = person.GetPrimaryAssignment(organization);
            }

            // TODO: Verify membership OR position OR volunteer

            return new Authority(new AuthorityData
            {
                CustomData = new Basic.Types.Common.SerializableDictionary<string, string>(),
                LoginDateTimeUtc = DateTime.UtcNow,
                OrganizationId = lastOrgId,
                PersonId = person.Identity,
                PositionAssignmentId = (assignment != null ? assignment.Identity : 0)
            });
        }

        public static Authority FromLogin(Person person, Organization organization)
        {
            PositionAssignment assignment = person.GetPrimaryAssignment(organization);

            // TODO: Verify membership OR position OR volunteer

            return new Authority(new AuthorityData
            {
                CustomData = new Basic.Types.Common.SerializableDictionary<string, string>(),
                LoginDateTimeUtc = DateTime.UtcNow,
                OrganizationId = organization.Identity,
                PersonId = person.Identity,
                PositionAssignmentId = (assignment != null ? assignment.Identity : 0)
            });
        }

        private readonly AuthorityData _data;



        /// <summary>
        /// Determines if this Authority has a particular Access.
        /// </summary>
        /// <param name="access">The access desired.</param>
        /// <returns>True if access can be granted.</returns>
        public bool HasAccess(Access access)
        {
            if (access == null)
            {
                throw new ArgumentNullException("access", @"Access cannot be null, but must always be explicitly specified. Specify AccessAspect.Null if null access is desired.");
            }

            if (access.Aspect == AccessAspect.Null)
            {
                // Null security (like Dashboard), so return true

                return true;
            }

            // Check for participant financials

            if (access.Aspect == AccessAspect.Financials && access.Type == AccessType.Read)
            {
                if (access.Organization.ParticipantFinancialsEnabled)
                {
                    // This organization has decided to open its financial reports to all participants. Reselect the access request to "participant" level.

                    access = new Access(access.Organization, AccessAspect.Participant);
                }
            }

            // Check for Participant access level

            if (access.Aspect == AccessAspect.Participant)
            {
                // Check that a membership (or whatever this org calls it) exists, for this org or a parentline org

                if (Person.MemberOfWithInherited(access.Organization))
                {
                    return true;
                }
            }

            // if Open Ledgers, return true

            if ((access.Aspect == AccessAspect.Bookkeeping || access.Aspect == AccessAspect.Financials) &&
                access.Type == AccessType.Read && this.Person.Identity == Swarm.Person.OpenLedgersIdentity)
            {
                return true;
            }

            // We're at the end of generic access control - now, check against position assignments

            // Check if the person is currently acting at sysadmin level

            if (HasSystemAccess (access.Type))
            {
                return true;
            }

            // If system-level access was requested and has not been granted at this point, deny it

            if (access.Organization == null)
            {
                return false;
            }

            // Organization-level or geography-level access requested

            if (Assignment == null)
            {
                // No assignment to ask, therefore, no access

                return false;
            }

            // Ask the current position assignment if it has the requested access

            return Assignment.Position.HasAccess(access);
        }


        public bool HasSystemAccess (AccessType accessType = AccessType.Write)
        {
            // TODO: CHECK POSITION ASSIGNMENT
            // for now, check if position assignment exists

            // This is not very efficient, but there shouldn't be many system-level positions or
            // assignments, so it should scale reasonably even in an 1M organization

            PositionAssignments assignments = Positions.ForSystem().Assignments;

            foreach (PositionAssignment assignment in assignments)
            {
                if (assignment.Active && assignment.PersonId == this._data.PersonId)
                {
                    if ((assignment.Position.PositionType == PositionType.System_SysadminMain ||
                        assignment.Position.PositionType == PositionType.System_SysadminReadWrite))
                        return true;

                    if (assignment.Position.PositionType == PositionType.System_SysadminAssistantReadOnly &&
                        accessType == AccessType.Read)
                    {
                        return true; // Read-only access
                    }
                }
            }

            return false;
        }


        public bool CanAccess (IHasIdentity identifiableObject, AccessType accessType = AccessType.Write)
        {
            // Tests if this Authority can access a certain object. Add new object types as needed by the logic.
            // This is a very general case of the CanSeePerson() function.

            PositionAssignment testAssignment = identifiableObject as PositionAssignment;
            if (testAssignment != null)
            {
                // shortcut, for now

                return HasSystemAccess (accessType);
            }

            throw new NotImplementedException("Authority.CanAccess is not implemented for type " + identifiableObject.GetType().FullName);
        }


        public People FilterPeople(People rawList, AccessAspect aspect = AccessAspect.Participation)
        {
            if (aspect != AccessAspect.Participation && aspect != AccessAspect.PersonalData)
            {
                throw new ArgumentException(@"AccessAspect needs to reflect visibility of people data", "aspect");
            }

            // Three cases:

            // 1) the current Position has system-level access.
            // 2) the current Position has org-level, but not system-level, access.
            // 3) the current Position has org-and-geo-level access.

            Dictionary<int, bool> orgLookup = new Dictionary<int, bool>();
            Dictionary<int, bool> geoLookup = new Dictionary<int, bool>();

            People result = new People();

            // Org lookup will always be needed. Geo lookup may be needed for case 3.

            Organizations orgStructure = this.Organization.GetTree();
            int[] orgIds = orgStructure.Identities;
            foreach (int orgId in orgIds)
            {
                orgLookup[orgId] = true;
            }
            orgLookup[Organization.Identity] = true;

            Dictionary<int, List<BasicMembership>> membershipLookup = null;

            if (HasSystemAccess(AccessType.Read) || HasAccess(new Access(Organization, aspect, AccessType.Read)))
            {
                // cases 1 and 2: systemwide access, return everybody at or under the current Organization,
                // or org-wide read access (at least) to participant/personal data at current Organization

                // Optimization: Get all memberships in advance, without instantiating logic objects
                membershipLookup = Memberships.GetMembershipsForPeople (rawList.Identities, 0);

                foreach (Person person in rawList)
                {
                    // For each person, we must test the list of active memberships to see if one of 
                    // them is visible to this Authority - if it's a membership in an org at or below the
                    // Authority object's organization

                    if (membershipLookup.ContainsKey (person.Identity))
                    {
                        List<BasicMembership> list = membershipLookup[person.Identity];

                        foreach (BasicMembership basicMembership in list)
                        {
                            if (orgLookup.ContainsKey (basicMembership.OrganizationId))
                            {
                                // hit - this person has an active membership that makes them visible to this Authority
                                result.Add (person);
                            }
                        }
                    }
                }

                return result;
            }

            // Case 3: Same as above but also check for Geography (in an AND pattern).

            if (this.Position == null)
            {
                // No access at all. That was an easy case!

                return new People(); // return empty list
            }

            if (this.Position.Geography == null)
            {
                // Org-level position, but one that doesn't have access to personal data, apparently.

                return new People(); // empty list again
            }

            if (!HasAccess (new Access (this.Organization, Position.Geography, aspect, AccessType.Read)))
            {
                // No people access for active position. Also a reasonably easy case.

                return new People(); // also return empty list
            }

            Geographies geoStructure = this.Position.Geography.GetTree();
            int[] geoIds = geoStructure.Identities;
            foreach (int geoId in geoIds)
            {
                geoLookup[geoId] = true;
            }
            geoLookup[Position.GeographyId] = true;

            // Optimization: Get all memberships in advance, without instantiating logic objects
            Dictionary<int, List<BasicMembership>> personLookup =
                Memberships.GetMembershipsForPeople(rawList.Identities, 0);

            foreach (Person person in rawList)
            {
                // For each person, we must test the list of active memberships to see if one of 
                // them is visible to this Authority - if it's a membership in an org at or below the
                // Authority object's organization - and also test the person's Geography against
                // the list (lookup) of visible Geographies. We do Geographies first, because that test is 
                // much cheaper.

                if (geoLookup[person.GeographyId])
                {
                    // Geography hit. Test Membership / Organization.

                    List<BasicMembership> list = personLookup[person.Identity];

                    foreach (BasicMembership basicMembership in list)
                    {
                        if (orgLookup.ContainsKey (basicMembership.OrganizationId))
                        {
                            // Organization hit - this person has an active membership that makes them visible to this Authority

                            result.Add (person);
                        }
                    }
                }
            }

            return result;

            
        }


        public bool CanSeePerson (Person person, AccessAspect aspect = AccessAspect.Participation)
        {
            if (aspect != AccessAspect.Participation && aspect != AccessAspect.PersonalData)
            {
                throw new ArgumentException(@"AccessAspect needs to reflect visibility of people data", "aspect");
            }

            // Three cases:

            // 1) the current Position has system-level access.
            // 2) the current Position has org-level, but not system-level, access.
            // 3) the current Position has org-and-geo-level access.

            if (HasSystemAccess (AccessType.Read))  // case 1
            {
                // Still filter to the current Organization, even though we have systemwide access

                if (person.MemberOfWithInherited(Organization))
                {
                    return true;
                }
            }

            // Is this Person a Participant of an org or sub-org where the current Authority
            // has organizationwide access? Case 2.

            if (
                HasAccess (new Access (Organization, aspect, AccessType.Read)))
            {
                if (person.MemberOfWithInherited (Organization))
                {
                    return true;
                }
            }

            // Finally, determine by geography AND organization.

            if (Position == null || Position.Geography == null)
            {
                return false;
            }

            if (
                HasAccess (new Access (Organization, Position.Geography, aspect, AccessType.Read)))
            {
                if (person.MemberOfWithInherited(Organization))
                {
                    if (person.GeographyId == Position.GeographyId || person.Geography.Inherits (Position.Geography))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        
    }

    [Serializable]
    public class AuthorityData
    {
        public int PersonId { get; set; }
        public int OrganizationId { get; set; }
        public int PositionAssignmentId { get; set; }
        public DateTime LoginDateTimeUtc { get; set; }
        public Basic.Types.Common.SerializableDictionary<string,string> CustomData { get; set; }

        internal string ToXml()
        {
            XmlSerializer serializer = new XmlSerializer(GetType());

            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, this);

            byte[] xmlBytes = stream.GetBuffer();
            return Encoding.UTF8.GetString(xmlBytes);
        }

        static internal AuthorityData FromXml (string xml)
        {
            // Compensate for stupid Mono encoding bugs

            if (xml.StartsWith("?"))
            {
                xml = xml.Substring(1);
            }

            xml = xml.Replace("&#x0;", "");
            xml = xml.Replace("\x00", "");

            XmlSerializer serializer = new XmlSerializer(typeof(AuthorityData));

            MemoryStream stream = new MemoryStream();
            byte[] xmlBytes = Encoding.UTF8.GetBytes(xml);
            stream.Write(xmlBytes, 0, xmlBytes.Length);

            stream.Position = 0;
            AuthorityData result = (AuthorityData)serializer.Deserialize(stream);
            stream.Close();

            return result;
        }
    }
}