using System;
using System.Collections.Generic;
using System.Web;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.Swarm
{
    public partial class Json_ListFindPeople : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            string pattern = HttpUtility.UrlDecode (Request.QueryString["Pattern"]);

            string geographyString = Request.QueryString["GeographyId"];
            int geographyId = int.Parse (geographyString);
            Geography geography = Geography.FromIdentity (geographyId);

            if (
                !CurrentAuthority.HasAccess (new Access (CurrentOrganization, geography, AccessAspect.PersonalData,
                    AccessType.Read)))
            {
                throw new UnauthorizedAccessException ("nope");
            }

            People matches = People.FromOrganizationAndGeographyWithPattern (CurrentOrganization, geography, pattern);

            matches = CurrentAuthority.FilterPeople (matches);

            if (matches == null)
            {
                matches = new People();
            }

            if (matches.Count > 1000)
            {
                matches.RemoveRange (1000, matches.Count - 1000);
            }

            List<string> jsonPeople = new List<string>();

            string editPersonTemplate =
                "\"actions\":\"<a href='javascript:masterBeginEditPerson({0})'><img src='/Images/Icons/iconshock-wrench-128x96px-centered.png' height='16' width='24' /></a>\"";

            Dictionary<int, Applicant> applicantLookup = new Dictionary<int, Applicant>();

            if (CurrentOrganization.Parameters.ParticipationEntry == "ApplicationApproval")
            {
                // There are applications possible for this org; find them and put the Score in the Notes field

                Applicants applicants = Applicants.FromPeopleInOrganization(matches, CurrentOrganization);

                foreach (Applicant applicant in applicants)
                {
                    applicantLookup[applicant.PersonId] = applicant;
                }
            }

            foreach (Person person in matches)
            {
                string notes = Participant.Localized(CurrentOrganization.RegularLabel, person.Gender);

                if (applicantLookup.ContainsKey(person.Identity))
                {
                    // If this is an applicant, use the "Notes" field for score
                    notes = "<span class='alignRight'>" + applicantLookup[person.Identity].ScoreTotal.ToString("N0") + "</span>";
                }

                string onePerson = '{' +
                                   String.Format (
                                       "\"id\":\"{0}\",\"name\":\"<span class='spanUser{0}Name'>{1}</span>\",\"avatar16Url\":\"{2}\",\"geographyName\":\"{3}\",\"mail\":\"<span class='spanUser{0}Mail'>{4}</span>\",\"notes\":\"{6}\",\"phone\":\"<span class='spanUser{0}Phone'>{5}</span>\"",
                                       person.Identity,
                                       JsonSanitize (person.Canonical),
                                       person.GetSecureAvatarLink (16),
                                       JsonSanitize (person.Geography.Name),
                                       JsonSanitize (person.Mail),
                                       JsonSanitize (person.Phone),
                                        JsonSanitize(notes)) + "," +
                                    String.Format(
                                        editPersonTemplate, person.Identity)
                                       + '}';
                jsonPeople.Add (onePerson);
            }

            string result = '[' + String.Join (",", jsonPeople.ToArray()) + ']';

            Response.Output.WriteLine (result);

            Response.End();
        }
    }
}