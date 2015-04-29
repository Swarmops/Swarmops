using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
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

            if ((position.OrganizationId > 0 && authData.CurrentOrganization.Identity != position.OrganizationId) || person.Identity < 0)
            {
                throw new UnauthorizedAccessException();
            }
            if (!authData.Authority.HasAccess (new Access (AccessAspect.Administration)))
            {
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
    }
}