using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Common.Enums;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;
using Swarmops.Common.Exceptions;

namespace Swarmops.Frontend.Automation
{
    public partial class SwarmFunctions : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static AjaxCallResult AssignPosition (int personId, int positionId, int durationMonths, int geographyId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            Position position = Position.FromIdentity (positionId);
            Person person = Person.FromIdentity (personId);
            Geography geography = (geographyId == 0 ? null : Geography.FromIdentity (geographyId));

            if (position.PositionLevel == PositionLevel.Geography ||
                position.PositionLevel == PositionLevel.GeographyDefault)
            {
                position.AssignGeography (geography);
            }

            if ((position.OrganizationId > 0 && authData.CurrentOrganization.Identity != position.OrganizationId) || person.Identity < 0)
            {
                throw new UnauthorizedAccessException();
            }
            if (position.PositionLevel == PositionLevel.SystemWide && !authData.Authority.HasAccess (new Access (AccessAspect.Administration)))
            {
                // Authority check for systemwide
                throw new UnauthorizedAccessException();
            }
            if ((position.GeographyId == Geography.RootIdentity || position.GeographyId == 0) &&
                !authData.Authority.HasAccess (new Access (authData.CurrentOrganization, AccessAspect.Administration)))
            {
                // Authority check for org-global
                throw new UnauthorizedAccessException();
            }
            if (
                !authData.Authority.HasAccess (new Access (authData.CurrentOrganization, geography,
                    AccessAspect.Administration)))
            {
                // Authority check for org/geo combo
                throw new UnauthorizedAccessException();
            }

            if (position.MaxCount > 0 && position.Assignments.Count >= position.MaxCount)
            {
                return new AjaxCallResult
                {
                    Success = false,
                    DisplayMessage = Resources.Controls.Swarm.Positions_NoMorePeopleOnPosition
                };
            }

            // Deliberate: no requirement for membership (or equivalent) in order to be assigned to position.

            Position currentUserPosition = authData.CurrentUser.PositionAssignment.Position; // excludes acting positions. May throw!
            DateTime? expiresUtc = null;

            if (durationMonths > 0)
            {
                expiresUtc = DateTime.UtcNow.AddMonths (durationMonths);
            }

            try
            {
                PositionAssignment.Create (position, geography, person, authData.CurrentUser, currentUserPosition,
                    expiresUtc, string.Empty);
            }
            catch (DatabaseConcurrencyException)
            {
                return new AjaxCallResult {Success = false, DisplayMessage = Resources.Global.Error_DatabaseConcurrency};
            }

            return new AjaxCallResult {Success = true};
        }


        [WebMethod]
        public static AjaxCallResult TerminatePositionAssignment (int assignmentId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            PositionAssignment assignment = PositionAssignment.FromIdentity (assignmentId);

            if (assignment.OrganizationId == 0)
            {
                if (!authData.Authority.HasAccess (new Access (AccessAspect.Administration))) // System-wide admin
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else // Org-specific assignment
            {
                if (assignment.GeographyId == 0)
                {
                    if (!authData.Authority.HasAccess (new Access(authData.CurrentOrganization, AccessAspect.Administration)))
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else // Org- and geo-specific assignment
                {
                    if (
                        !authData.Authority.HasAccess (new Access (authData.CurrentOrganization,
                            assignment.Position.Geography, AccessAspect.Administration)))
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
            }

            // Ok, go ahead and terminate

            try
            {
                assignment.Terminate(authData.CurrentUser, authData.CurrentUser.GetPrimaryPosition(authData.CurrentOrganization), string.Empty);
            }
            catch (DatabaseConcurrencyException)
            {
                return new AjaxCallResult {Success = false, DisplayMessage = Resources.Global.Error_DatabaseConcurrency};
            }

            return new AjaxCallResult {Success = true};
        }

        [WebMethod]
        static public AssignmentData GetAssignmentData (int assignmentId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            PositionAssignment assignment = PositionAssignment.FromIdentity (assignmentId);

            if (authData.Authority.CanAccess (assignment, AccessType.Read))
            {
                return new AssignmentData
                {
                    Success = true,
                    AssignedPersonCanonical = assignment.Person.Canonical,
                    AssignedPersonId = assignment.PersonId,
                    PositionAssignmentId = assignment.Identity,
                    PositionId = assignment.PositionId,
                    PositionLocalized = assignment.Position.Localized()
                };
            }
            else
            {
                return new AssignmentData {Success = false};
            }

        }

        public class AssignmentData : AjaxCallResult
        {
            public int PositionAssignmentId { get; set; }
            public int PositionId { get; set; }
            public string PositionLocalized { get; set; }
            public int AssignedPersonId { get; set; }
            public string AssignedPersonCanonical { get; set; }
        }

        public class AvatarData : AjaxCallResult
        {
            public int PersonId { get; set; }
            public string Canonical { get; set; }
            public string Avatar16Url { get; set; }
            public string Avatar24Url { get; set; }
        }

        [WebMethod]
        public static AvatarData GetPersonAvatar (int personId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();

            Person person = Person.FromIdentity (personId);
            if (!authData.Authority.CanSeePerson (person))
            {
                throw new ArgumentException(); // can't see, for whatever reason
            }

            return new AvatarData
            {
                PersonId = personId,
                Success = true,
                Canonical = person.Canonical,
                Avatar16Url = person.GetSecureAvatarLink (16),
                Avatar24Url = person.GetSecureAvatarLink (24)
            };
        }

        [WebMethod]
        public static AjaxCallResult GetGeographyName (int geographyId)
        {
            // This is not sensitive, so no access control, just culture setting

            GetAuthenticationDataAndCulture();
            return new AjaxCallResult {Success = true, DisplayMessage = Geography.FromIdentity (geographyId).Name};
        }

        [Serializable]
        public class PersonEditorData : AjaxCallResult
        {
            // Personal Details tab

            public string Name { get; set; }
            public string Mail { get; set; }
            public string Phone { get; set; }
            public string TwitterId { get; set; }

            // Accounts tab

            // (no data)

            // other tabs - fill in as we go
        }

        [WebMethod]
        public static PersonEditorData GetPersonEditorData (int personId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            bool self = false;

            if (personId == 0) // request self record
            {
                self = true; // may make use of this later
                personId = authData.CurrentUser.Identity;
            }

            Person person = Person.FromIdentity (personId);

            if (!authData.Authority.CanSeePerson (person))
            {
                throw new ArgumentException(); // can't see the requested person, for whatever reason
            }

            return new PersonEditorData
            {
                Success = true,
                Name = person.Name,
                Mail = person.Mail,
                Phone = person.Phone,
                TwitterId = person.TwitterId
            };
        }

        [WebMethod]
        public static AjaxInputCallResult SetPersonEditorData(int personId, string field, string newValue)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            bool self = false;

            if (personId == 0) // request self record
            {
                self = true; // may make use of this later
                personId = authData.CurrentUser.Identity;
            }

            if (string.IsNullOrEmpty (newValue))
            {
                if (field != "TwitterId") // These fields may be set to empty; default is disallow
                {
                    return new AjaxInputCallResult
                    {
                        Success = false,
                        DisplayMessage = Resources.Global.Global_FieldCannotBeEmpty,
                        FailReason = AjaxInputCallResult.ErrorInvalidFormat,
                        NewValue = GetPersonValue (personId, field)
                    };
                }
            }

            return new AjaxInputCallResult
            {
                Success = true,
                NewValue = field + ": The change call was successful"
            };
            throw new NotImplementedException();
        }

        private static string GetPersonValue (int personId, string field)
        {
            Person person = Person.FromIdentity (personId);
            switch (field)
            {
                case "Name":
                    return person.Name;
                case "Mail":
                    return person.Mail;
                case "Phone":
                    return person.Phone;
                case "TwitterId":
                    return person.TwitterId;
                default:
                    throw new NotImplementedException();
            }
        }

    }
}