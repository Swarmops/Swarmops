using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for FieldValidation
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class FieldValidation : System.Web.Services.WebService {

    public FieldValidation () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public bool IsAmountValid (string amount)
    {
        // Determine correct culture for parsing

        CultureInfo culture = null;

        // Set default culture (English, United States, but that doesn't work so fake it to GB)

        string preferredCulture = "en-GB";

        // -----------  SET CULTURE ------------

        // Does the user have a culture preference?

        if (HttpContext.Current.Request.Cookies["PreferredCulture"] != null)
        {
            // Yes, set it
            preferredCulture = HttpContext.Current.Request.Cookies["PreferredCulture"].Value;
        }
        else
        {
            // No, determine from browser
            string browserPreference = "en-GB";
            if (HttpContext.Current.Request.UserLanguages != null && HttpContext.Current.Request.UserLanguages.Length > 0)
            {
                browserPreference = HttpContext.Current.Request.UserLanguages[0];
                preferredCulture = browserPreference;
            }
        }

        try
        {
            culture = CultureInfo.CreateSpecificCulture(preferredCulture);
        }
        catch (Exception exception)
        {
            culture = CultureInfo.InvariantCulture;
        }

        // Try to parse the string we got as a double

        try
        {
            Double.Parse(HttpUtility.UrlDecode(amount), NumberStyles.Number, culture);
            return true;
        }
        catch (Exception)
        {
        }

        return false;
    }
}
