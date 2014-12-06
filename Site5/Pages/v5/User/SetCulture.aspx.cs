using System;
using System.Web;

namespace Swarmops.Frontend.Pages.v5.User
{
    public partial class SetCulture : PageV5Base
    {
        // This minimal page takes the culture on the querystring, sets it for the user, and redirects to Dashboard

        protected void Page_Load (object sender, EventArgs e)
        {
            AuthenticationData authenticationData = GetAuthenticationDataAndCulture();

            if (authenticationData.CurrentUser == null)
            {
                throw new UnauthorizedAccessException ("No"); // may cause problems on login screen?
            }

            string cultureId = Request.QueryString["CultureId"];
            Response.SetCookie (new HttpCookie ("PreferredCulture", cultureId));
            authenticationData.CurrentUser.PreferredCulture = cultureId;
            Response.Redirect ("/"); // to dashboard
        }
    }
}