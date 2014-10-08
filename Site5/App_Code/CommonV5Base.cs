using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

/// <summary>
/// Summary description for CommonV5Base
/// </summary>
public class CommonV5Base
{
	public CommonV5Base()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    static public void CulturePreInit (HttpRequest request)
    {
        // Localization
        // Set default culture (English, United States)

        string preferredCulture = "en-US";

        // -----------  SET CULTURE ------------

        // Does the user have a culture preference?

        if (request.Cookies["PreferredCulture"] != null)
        {
            // Yes, set it
            preferredCulture = request.Cookies["PreferredCulture"].Value;
        }
        else
        {
            // No, determine from browser
            string browserPreference = "en-US";
            if (request.UserLanguages != null && request.UserLanguages.Length > 0)
            {
                browserPreference = request.UserLanguages[0];
                preferredCulture = browserPreference;
            }

            /*
            string[] languages = (string[])Application["Cultures"];
            for (int index = 0; index < languages.Length; index++)
            {
                if (languages[index].StartsWith(browserPreference))
                {
                    preferredCulture = languages[index];
                }
            }*/
        }

        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(preferredCulture);
        }
        catch (Exception exception)
        {
            throw new Exception("Could not set culture \"" + preferredCulture + "\"", exception);
            // Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
    }
}