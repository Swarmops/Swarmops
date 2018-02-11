using System;
using System.Globalization;
using System.Web.Services;
using Swarmops.Logic.Cache;
using Swarmops.Frontend;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_ByGuid : Swarmops.Frontend.DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static int GetProgress (string guid)   // This is a legacy method
        {
            object progressObject = GuidCache.Get (guid + "-Progress");
            // suffix because guid may refer to other data too

            if (progressObject != null)
            {
                return (int) progressObject;
            }

            // if the progress object doesn't exist, assume it hasn't been initialized yet

            return 0;
        }

        [WebMethod]
        public static AjaxCallResult GetNonsocketProgress (string guid)
        {
            object progressObject = GuidCache.Get(guid + "-Progress");
            // suffix because guid may refer to other data too

            if (progressObject != null)
            {
                return new AjaxCallResult {Success = true, DisplayMessage = ((int) progressObject).ToString(CultureInfo.InvariantCulture) };
            }

            // if the progress object doesn't exist, assume it hasn't been initialized yet

            return new AjaxCallResult {Success = false};

        }
    }
}