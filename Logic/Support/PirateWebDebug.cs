using System.IO;
using System.Web;

namespace Swarmops.Logic.Support
{
    public class PirateWebDebug
    {
        public static string GetDebugInformation()
        {
            string debug = string.Empty;

            debug += "HttpContext is null: " + (HttpContext.Current == null ? "yes" : "no") + "\r\n";
            debug += "Current Directory: " + Directory.GetCurrentDirectory();

            if (HttpContext.Current != null)
            {
                debug += "Application Directory: " + HttpContext.Current.Server.MapPath("~/Test.mdb") + "\r\n";
            }

            return debug;
        }
    }
}