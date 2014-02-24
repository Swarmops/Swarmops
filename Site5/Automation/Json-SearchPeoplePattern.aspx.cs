using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_SearchPeoplePattern : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            string pattern = Request.QueryString["namePattern"];

            People matches = People.FromNamePattern(pattern);

            matches = matches.GetVisiblePeopleByAuthority(CurrentUser.GetAuthority());

            if (matches.Count > 10)
            {
                matches.RemoveRange(10, matches.Count - 10);
            }

            List<string> jsonPeople = new List<string>();

            foreach (Person person in matches)
            {
                string onePerson = '{' + String.Format("\"id\":\"{0}\",\"name\":\"{1}\",\"avatar16Url\":\"{2}\"", person.Identity, JsonSanitize(person.Canonical), person.GetSecureAvatarLink(16)) + '}';
                jsonPeople.Add(onePerson);
            }

            string result = '[' + String.Join(",", jsonPeople.ToArray()) + ']';

            Response.Output.WriteLine(result);

            Response.End();
        }
    }
}