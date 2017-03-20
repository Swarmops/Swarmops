using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Resources;

namespace Swarmops.Logic.Support
{
    public class ErrorMessages
    {
        public static string Localized(string errorMessage, string culture = null)
        {
            if (String.IsNullOrEmpty(culture))
            {
                return @"<strong>" + Logic_Support_ErrorMessages.ResourceManager.GetString(errorMessage + "_Title") +
                       @":</strong> " +
                       Logic_Support_ErrorMessages.ResourceManager.GetString(errorMessage + "_Text");
            }
            else
            {
                return @"<strong>" +
                       Logic_Support_ErrorMessages.ResourceManager.GetString(errorMessage + "_Title",
                           CultureInfo.CreateSpecificCulture(culture)) +
                       @":</strong> " +
                       Logic_Support_ErrorMessages.ResourceManager.GetString(errorMessage + "_Text",
                           CultureInfo.CreateSpecificCulture(culture));
            }
        }
    }
}
