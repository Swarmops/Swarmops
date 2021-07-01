using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Swarmops.Logic.Resources
{
    public class MonoResourceManager: System.Resources.ResourceManager
    {
        public MonoResourceManager(string baseName)
            : base (baseName, Assembly.Load("Swarmops.Frontend." + baseName + ".resources"))
        {
            
        }

        protected override string GetResourceFileName(CultureInfo culture)
        {
            string path = "bin/";
            if (HttpContext.Current != null)
            {
                path = HttpContext.Current.Server.MapPath("/bin/");
            }

            if (culture.IsNeutralCulture || culture.Name == "en-US" || string.IsNullOrWhiteSpace(culture.Name))
            {
                return this.BaseName + ".resources";
            }

            return  this.BaseName + "." + culture.Name + ".resources";
        }
    }
}
