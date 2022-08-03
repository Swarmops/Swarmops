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

            if (culture == null || culture.IsNeutralCulture || string.IsNullOrWhiteSpace(culture.Name) || culture.Name == "en-US")
            {
                return this.BaseName + ".resources";
            }

            return culture.Name + "/" + this.BaseName + "." + culture.Name + ".resources";
        }
    }
}
