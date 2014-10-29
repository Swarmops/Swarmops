using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Support;

namespace Swarmops.Security
{
    public partial class BitId : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static void Authenticate(string uri, string signature, string address)
        {
            Persistence.Key["BitIdTest_Uri"] = uri;
            Persistence.Key["BitIdTest_Address"] = address;
            Persistence.Key["BitIdText_Signature"] = signature;
        }
    }
}