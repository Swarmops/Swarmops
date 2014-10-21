using System;
using System.Web.Services;
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
            object progressObject = GuidCache.Get(guid + "-Progress"); // suffix because guid may refer to other data too

            if (progressObject != null)
            {
                return (int) progressObject;
            }

            // if the progress object doesn't exist, assume it hasn't been initialized yet

            return 0;
        }
    }
}