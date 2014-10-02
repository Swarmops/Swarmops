using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.Swarm
{
    public partial class Json_ListFindPeople : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            string pattern = HttpUtility.UrlDecode(Request.QueryString["Pattern"]);

            string geographyString = Request.QueryString["GeographyId"];
            int geographyId = int.Parse(geographyString);
            Geography geography = Geography.FromIdentity(geographyId);

            if (
                !this.CurrentUser.HasAccess(new Access(this.CurrentOrganization, geography, AccessAspect.PersonData,
                    AccessType.Read)))
            {
                throw new UnauthorizedAccessException("nope");
            }

            People matches = People.FromOrganizationAndGeographyWithPattern(this.CurrentOrganization, geography, pattern);

            // matches = Authorization.FilterPeopleToMatchAuthority(matches, CurrentUser.GetAuthority());

            if (matches == null)
            {
                matches = new People();
            }

            if (matches.Count > 1000)
            {
                matches.RemoveRange(1000, matches.Count - 1000);
            }

            List<string> jsonPeople = new List<string>();

            foreach (Person person in matches)
            {
                string onePerson = '{' + String.Format("\"id\":\"{0}\",\"name\":\"{1}\",\"avatar16Url\":\"{2}\",\"geographyName\":\"{3}\",\"mail\":\"{4}\",\"phone\":\"{5}\"", 
                    person.Identity, 
                    JsonSanitize(person.Canonical), 
                    person.GetSecureAvatarLink(16),
                    JsonSanitize(person.Geography.Name),
                    JsonSanitize(person.Mail),
                    JsonSanitize(person.Phone)) + '}';
                jsonPeople.Add(onePerson);
            }

            string result = '[' + String.Join(",", jsonPeople.ToArray()) + ']';

            Response.Output.WriteLine(result);

            Response.End();
        }
    }
}