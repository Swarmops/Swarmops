using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

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
        catch (Exception) // if we can't set the culture, what do we do? ("We send the Marines.")
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            // throw new Exception("Could not set culture \"" + preferredCulture + "\"", exception);
            // Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
    }

    public static AuthenticationData GetAuthenticationDataAndCulture(HttpContext suppliedContext)
    {
        // This function is called from static page methods in AJAX calls to get
        // the current set of authentication data. Static page methods cannot access
        // the instance data of PageV5Base.

        AuthenticationData result = new AuthenticationData();

        // Find various credentials

        string identity = suppliedContext.User.Identity.Name;

        // TODO: If identity is null or empty, set null user + org

        string[] identityTokens = identity.Split(',');

        string userIdentityString = identityTokens[0];
        string organizationIdentityString = identityTokens[1];

        int currentUserId = Convert.ToInt32(userIdentityString);
        int currentOrganizationId = Convert.ToInt32(organizationIdentityString);

        result.CurrentUser = Person.FromIdentity(currentUserId);
        result.CurrentOrganization = Organization.FromIdentity(currentOrganizationId);

        CommonV5Base.CulturePreInit(HttpContext.Current.Request); // OnPreInit() isn't called in the static methods calling this fn

        /*
        string userCultureString = result.CurrentUser.PreferredCulture;

        if (!string.IsNullOrEmpty(userCultureString))
        {
            CultureInfo userCulture = new CultureInfo(userCultureString); // may throw on invalid database data
            Thread.CurrentThread.CurrentCulture = userCulture;
            Thread.CurrentThread.CurrentUICulture = userCulture;
        }*/

        return result;
    }

}

[Flags]
// ReSharper disable once InconsistentNaming
public enum EasyUIControl
{
    Unknown = 0,
    Accordion    = 0x0000001,
    Calendar     = 0x0000002,
    Combo        = 0x0000004,
    ComboBox     = 0x0000008,
    DataGrid     = 0x0000010,
    DateBox      = 0x0000020,
    Dialog       = 0x0000040,
    FileBox      = 0x0000080,
    Layout       = 0x0000100,
    LinkButton   = 0x0000200,
    Menu         = 0x0000400,
    MenuButton   = 0x0000800,
    Messager     = 0x0001000,
    NumberBox    = 0x0002000,
    Pagination   = 0x0004000,
    Panel        = 0x0008000,
    ProgressBar  = 0x0010000,
    PropertyGrid = 0x0020000,
    SearchBox    = 0x0040000,
    Slider       = 0x0080000,
    Spinner      = 0x0100000,
    SplitButton  = 0x0200000,
    Tabs         = 0x0400000,
    TextBox      = 0x0800000,
    ToolTip      = 0x1000000,
    Tree         = 0x2000000,
    ValidateBox  = 0x4000000,
    Window       = 0x8000000
};
