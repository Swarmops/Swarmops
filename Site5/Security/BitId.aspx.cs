using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.ExtensionMethods;
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
            Persistence.Key["BitIdTest_Raw"] = Request.ToRaw();
            Persistence.Key["BitIdTest_Uri"] = Request.Params["uri"];
            Persistence.Key["BitIdTest_Address"] = Request.Params["address"];
            Persistence.Key["BitIdText_Signature"] = Request.Params["signature"];
        }

        // ReSharper disable once InconsistentNaming
        [WebMethod]
        public static void callback(string uri, string signature, string address)
        {
            Persistence.Key["BitIdTest_Uri"] = uri;
            Persistence.Key["BitIdTest_Address"] = address;
            Persistence.Key["BitIdText_Signature"] = signature;
        }
    }
}



