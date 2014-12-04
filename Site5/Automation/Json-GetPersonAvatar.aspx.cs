using System;
using Swarmops.Logic.Security;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_GetPersonAvatar : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";

            string personId = Request.QueryString["personId"];
            string onePerson = string.Empty;

            People matches = People.FromSingle(Person.FromIdentity(Int32.Parse(personId)));

            matches = Authorization.FilterPeopleToMatchAuthority(matches, CurrentUser.GetAuthority());

            if (matches.Count == 1) // still allowed
            {
                onePerson = '{' +
                            String.Format(
                                "\"id\":\"{0}\",\"name\":\"{1}\",\"avatar16Url\":\"{2}\",\"avatar24Url\":\"{3}\"",
                                matches[0].Identity, JsonSanitize(matches[0].Canonical),
                                matches[0].GetSecureAvatarLink(16), matches[0].GetSecureAvatarLink(24)) + '}';
            }

            Response.Output.WriteLine(onePerson);

            Response.End();
        }
    }
}