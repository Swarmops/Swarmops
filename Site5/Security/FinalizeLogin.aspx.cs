using System;
using System.Web.Security;
using System.Web.UI;
using Swarmops.Logic.Cache;

namespace Swarmops.Security
{
    public partial class FinalizeLogin : Page
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            string nonce = Request.Params["Nonce"];
            string identity = (string) GuidCache.Get (nonce + "-Identity");

            if (string.IsNullOrEmpty (identity))
            {
                Response.Redirect ("Login");
            }
            else
            {
                GuidCache.Delete (nonce + "-Identity");
                FormsAuthentication.SetAuthCookie (identity, true);
                Response.Redirect ("/");
            }
        }
    }
}