using System;
using System.Web;

namespace Swarmops.Frontend.Pages.v5.User
{
    public partial class SetCulture : PageV5Base
    {
        // This minimal page takes the culture on the querystring, sets it for the user, and redirects to Dashboard

        protected void Page_Load (object sender, EventArgs e)
        {
            string cultureId = Request.QueryString["CultureId"];

            // Arabic: We present ar-AE but need to set ar-SA for dumb reasons
            if (cultureId == "ar-AE")
            {
                cultureId = "ar-SA";
            }

            Response.SetCookie(new HttpCookie("PreferredCulture", cultureId));
            
            AuthenticationData authenticationData = GetAuthenticationDataAndCulture();
            if (authenticationData.CurrentUser != null)
            {
                authenticationData.CurrentUser.PreferredCulture = cultureId;
            }

            Response.Redirect ("/"); // to dashboard - or maybe back to login page
        }
    }
}