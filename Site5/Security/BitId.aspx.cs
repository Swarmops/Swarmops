using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Support;

namespace Swarmops.Security
{
    public partial class BitId : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Persistence.Key["BitIdTest_Addr"] = Request.Params["addr"];
            Persistence.Key["BitIdTest_Sign"] = Request.Params["sign"];
            Persistence.Key["BitIdTest_BitIdUri"] = Request.Params["bitid_uri"];
            Persistence.Key["BitIdTest_CallbackUri"] = Request.Params["callback_uri"];
        }
    }
}