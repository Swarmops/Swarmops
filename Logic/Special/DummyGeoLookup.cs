using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using NGeoIP;
//using NGeoIP.Client;

namespace Swarmops.Logic.Special
{
    /// <summary>
    /// The point of this class is to reference NGeoIP, so the DLL gets copied to the output folder for Site to use.
    /// Kind of a kludgy hack, but works.
    /// </summary>
    public class DummyGeoLookup
    {
        /*
        public static string Lookup (string ip)
        {
            NGeoIP.Request request = new Request()
            {
                Format = Format.Json,
                IP = HttpContext.Current.Request.UserHostAddress
            };
            NGeoClient client = new NGeoClient(request);
            NGeoIP.RawData rawData = client.Execute();

            return rawData.CountryCode;
        }*/
    }
}
