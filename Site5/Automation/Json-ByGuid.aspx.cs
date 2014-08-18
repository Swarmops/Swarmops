using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Cache;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_ByGuid : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        static public int GetProgress(string guid)
        {
            return (int) GuidCache.Get(guid + "-Progress"); // suffix because guid may refer to other data too
        }
    }
}