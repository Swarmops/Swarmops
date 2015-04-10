using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Security;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Automation
{
    public partial class SwarmFunctions : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static AjaxCallResult AssignPosition (string cookie)
        {
            string[] dataParts = cookie.Split ('-');
            int positionId = Convert.ToInt32(dataParts[0]);
            int personId = Convert.ToInt32 (dataParts[2]);

            return new AjaxCallResult();  // TODO
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
            matches = Authorization.FilterPeopleToMatchAuthority(matches, authData.CurrentUser.GetAuthority()); // TODO: Change to Access

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