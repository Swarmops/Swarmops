using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.ExtensionMethods;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Support;

namespace Swarmops.Security
{
    public partial class BitId : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string nonce = Request.Params["Nonce"];
            string identity = (string) GuidCache.Get(nonce + "-Identity");

            if (string.IsNullOrEmpty(identity))
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                GuidCache.Delete(nonce + "-Identity");
                Persistence.Key["BitId_AuthComplete"] = DateTime.Now.ToString();
                FormsAuthentication.RedirectFromLoginPage(identity, true);
            }
        }

    }
}



