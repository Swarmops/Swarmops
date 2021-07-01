using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Logic.Resources
{
    public class MonoResourceManager: System.Resources.ResourceManager
    {
        public MonoResourceManager(string baseName)
            : base (baseName, Assembly.Load("Swarmops.Frontend." + baseName + ".resources.dll"))
        {
            
        }

        protected override string GetResourceFileName(CultureInfo culture)
        {
            if (culture.IsNeutralCulture || culture.Name == "en-US")
            {
                return "Swarmops.Frontend." + this.BaseName + ".resources.dll";
            }

            return "Swarmops.Frontend." + this.BaseName + "." + culture.Name + ".resources.dll";
        }
    }
}
