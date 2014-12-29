using System;
using System.Collections.Generic;
using Swarmops.Logic.Security;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_SearchPeoplePattern : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            string pattern = Request.QueryString["namePattern"];

            People matches = People.FromNamePattern (pattern);

            matches = Authorization.FilterPeopleToMatchAuthority (matches, CurrentUser.GetAuthority());

            if (matches == null)
            {
                matches = new People();
            }

            // TODO: Add filter so the people must have active memberships of this org or any org below it (fixes #1)

            if (matches.Count > 10)
            {
                matches.RemoveRange (10, matches.Count - 10);
            }

            List<string> jsonPeople = new List<string>();

            foreach (Person person in matches)
            {
                string onePerson = '{' +
                                   String.Format ("\"id\":\"{0}\",\"name\":\"{1}\",\"avatar16Url\":\"{2}\"",
                                       person.Identity, JsonSanitize (person.Canonical), person.GetSecureAvatarLink (16)) +
                                   '}';
                jsonPeople.Add (onePerson);
            }

            string result = '[' + String.Join (",", jsonPeople.ToArray()) + ']';

            Response.Output.WriteLine (result);

            Response.End();
        }
    }
}