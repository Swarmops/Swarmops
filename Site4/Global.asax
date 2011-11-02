<%@ Application Language="C#" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Security.Principal" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="Activizr.Logic" %>

<script RunAt="server">

    private void Application_Start (object sender, EventArgs e)
    {
        // Code that runs on application startup

        // Set supported cultures
        HttpContext.Current.Application["Cultures"] = new string[] { "sv-SE", "en-US", "de-DE", "de-AT", "fi-FI" };

        HttpContext.Current.Application["UserRoleCache"] = new Dictionary<int, string[]>();
    }

    private void Application_End (object sender, EventArgs e)
    {
        //  Code that runs on application shutdown
    }

    private void Application_Error (object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs
    }


    protected void Application_BeginRequest (Object sender, EventArgs e)
    {
        // Set default culture (English, United States)

        string preferredCulture = "en-US";

        // -----------  SET CULTURE ------------

        // Does the user have a culture preference?

        if (Request.Cookies["PreferredCulture"] != null)
        {
            // Yes, set it
            preferredCulture = Request.Cookies["PreferredCulture"].Value;
        }
        else
        {
            // No, determine from browser
            string browserPreference = "en-US";
            if (Request.UserLanguages != null && Request.UserLanguages.Length > 0)
                browserPreference = Request.UserLanguages[0];

            string[] languages = (string[])Application["Cultures"];
            for (int index = 0; index < languages.Length; index++)
            {
                if (languages[index].StartsWith(browserPreference))
                {
                    preferredCulture = languages[index];
                }
            }
        }

        try
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(preferredCulture);
        }
        catch
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
    }


    protected void Application_AuthenticateRequest (object sender, EventArgs e)
    {
        // Set default roles (none)

        string[] userRoles = new string[0];

        // -----------  SET ROLES  -----------

        // Determine logged-on user (FormsAuthentication)

        if (User != null) // This is ALWAYS null. Why? 
        // Because this is the wrong event. Move the code to Application_AuthenticateRequest
        {
            //int currentUserId = Int32.Parse(User.Identity.Name);

            //// TODO: Look at cache to see if roles are there

            //// Lookup roles in database

            //HttpContext.Current.Session["Authority"] =
            //    PirateWeb.Logic.Security.Authorization.GetPersonAuthority(currentUserId);

            //// TODO: Set CurrentUser to new object, with roles
        }


    }

    private void Session_Start (object sender, EventArgs e)
    {
        // Code that runs when a new session is started
    }

    private void Session_End (object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }
</script>

