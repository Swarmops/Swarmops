using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Services;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.User
{
    public partial class SetCulture : PageV5Base
    {
        // This minimal page takes the culture on the querystring, sets it for the user, and redirects to Dashboard

        protected void Page_Load(object sender, EventArgs e)
        {
            AuthenticationData authenticationData = GetAuthenticationDataAndCulture();

            if (authenticationData.CurrentUser == null)
            {
                throw new UnauthorizedAccessException("No"); // may cause problems on login screen?
            }

            string cultureId = Request.QueryString["CultureId"];
            Response.SetCookie(new HttpCookie("PreferredCulture", cultureId));
            authenticationData.CurrentUser.PreferredCulture = cultureId;
            Response.Redirect("/"); // to dashboard
        }

    }
}