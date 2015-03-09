using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Swarmops.Interface.Support
{
    public class DashboardMessage
    {
        public static void Set (string localizedMessage)
        {
            CheckContext();

            HttpCookie setMessage = new HttpCookie("DashboardMessage", Uri.EscapeDataString (localizedMessage).Replace ("+", "%20").Replace ("'", "%27"));
            setMessage.Expires = DateTime.Now.AddDays(7);

            HttpContext.Current.Response.SetCookie(setMessage);
        }

        public static void Reset()
        {
            CheckContext();

            HttpCookie clearMessage = new HttpCookie ("DashboardMessage", string.Empty);
            clearMessage.Expires = DateTime.Now.AddYears(-10);

            HttpContext.Current.Response.SetCookie(clearMessage);
        }

        private static void CheckContext()
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("No active HttpContext");
            }
        }
    }
}
