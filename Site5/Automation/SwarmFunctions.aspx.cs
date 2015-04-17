using System;
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
        public static AjaxCallResult AssignPosition (int personId, int positionId, int durationMonths, int organizationId, int geographyId)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture();
            Position position = Position.FromIdentity (positionId);
            Person person = Person.FromIdentity (personId);
            Organization organization = (organizationId == 0 ? null : Organization.FromIdentity (organizationId));
            Geography geography = (geographyId == 0 ? null : Geography.FromIdentity (geographyId));

            if ((position.OrganizationId > 0 && authData.CurrentOrganization.Identity != position.OrganizationId) || person.Identity < 0)
            {
                throw new UnauthorizedAccessException();
            }
            if (!authData.CurrentUser.HasAccess (new Access (AccessAspect.Administration)))
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
                PositionAssignment.Create (organization, geography, position, person, authData.CurrentUser, currentUserPosition,
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
                if (!authData.CurrentUser.HasAccess (new Access (AccessAspect.Administration))) // System-wide admin
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else // Org-specific assignment
            {
                if (assignment.GeographyId == 0)
                {
                    if (!authData.CurrentUser.HasAccess (new Access(authData.CurrentOrganization, AccessAspect.Administration)))
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else // Org- and geo-specific assignment
                {
                    if (
                        !authData.CurrentUser.HasAccess (new Access (authData.CurrentOrganization,
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

            People matches = People.FromSingle(Person.FromIdentity(personId));

            // CHange to new functions
            throw new NotImplementedException();
            // matches = Authorization.FilterPeopleToMatchAuthority(matches, authData.CurrentUser.GetAuthority()); // TODO: Change to Access

            if (matches.Count != 1)
            {
                throw new ArgumentException(); // no match, for whatever reason
            }

            return new AvatarData
            {
                PersonId = personId,
                Success = true,
                Canonical = matches[0].Canonical,
                Avatar16Url = matches[0].GetSecureAvatarLink (16),
                Avatar24Url = matches[0].GetSecureAvatarLink (24)
            };
        }
    }
}