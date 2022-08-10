<%@ Application Language="C#" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Web.ExtensionMethods" %>
<%@ Import Namespace="Swarmops.Database" %>
<%@ Import Namespace="Swarmops.Logic.Support" %>


<script runat="server">

    private void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        SupportFunctions.OperatingTopology = OperatingTopology.FrontendWeb;

        // Force TLS 1.2
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        // Set supported cultures
        HttpContext.Current.Application["Cultures"] = Formatting.SupportedCultures;
        HttpContext.Current.Application["UserRoleCache"] = new Dictionary<int, string[]>();
    }

    private void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown
    }

    private void Application_Error(object sender, EventArgs e)
    {
        Exception exc = this.Server.GetLastError();

        Persistence.Key["LastUncaughtException"] = exc.ToString();
        Persistence.Key["LastUncaughtExceptionRequest"] = Request.ToRaw();
        SwarmDb.GetDatabaseForWriting().CreateExceptionLogEntry (DateTime.UtcNow, "Application", exc);  // TODO: Move to Logic
        // Code that runs when an unhandled error occurs
    }


    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        // The culture was once set here for localization, but Mono resets CultureInfo before entering the page cycle.
        // Setting the CultureInfo has been moved to PageV5Base.OnPreInit().

        // TODO: PROPER PLUGIN HANDLING
        
        // Testing for page rewrite. Tests IN ORDER.

        string requestPath = Request.Path;

        if (requestPath == "/Default.aspx")
        {
            // Mono sends us the default path instead of the directly-submitted "/" received on Windows; compensate for Mono
            requestPath = "/";
        }

        // Rewrite of general URLs, candidates in order, assuming host is "dev.swarmops.com" and {0} given path
        
        string[] rewriteCandidates =
        {
            "{0}/Dashboard.aspx",        // for dev.swarmops.com   -- Dashboard overrides Default in root, if it exists
            "{0}Dashboard.aspx",         // for dev.swarmops.com/  -- Dashboard overrides Default in root, if it exists
            "{0}/Default.aspx",          // for dev.swarmops.com
            "{0}Default.aspx",           // for dev.swarmops.com/
            "{0}.aspx",                  // for dev.swarmops.com/Security/Login
            "/Pages/v5/Public{0}.aspx",  // for dev.swarmops.com/Signup (and others under Public)
            "/Pages/v5/Public{0}",       // for dev.swarmops.com/WizardStyle.css (linked from pages in Public)
            "/Pages/v5/{0}SheetSimplified.aspx", // for dev.swarmops.com/Ledgers/Balance -- one-page substitution
            "/Pages/v5{0}View.aspx",     // for dev.swarmops.com/Ledgers/Accountant[View] and other pages that end in View
            "/Pages/v5{0}.aspx",         // for dev.swarmops.com/User/SelectLanguage
            "/Pages/v5{0}"               // for dev.swarmops.com/Ledgers/Json-SomethingData.aspx
        };

        foreach (string stringCandidate in rewriteCandidates)
        {
            string rewrittenCandidate = String.Format (stringCandidate, requestPath);
            
            if (File.Exists (Server.MapPath (rewrittenCandidate)))
            {
                Context.RewritePath (rewrittenCandidate);
                break;  // exits if match found - prevents, say, Default from replacing Dashboard (which happened as a Mono-only bug)
            }
        }
    }


    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
        // Set default roles (none)

        // -----------  SET ROLES  -----------

        // Determine logged-on user (FormsAuthentication)

        /*
        
        if (this.User != null) // This is ALWAYS null. Why? 
        // Because this is the wrong event. Move the code to Application_AuthenticateRequest
        {
            //int currentUserId = Int32.Parse(User.Identity.Name);

            //// TODO: Look at cache to see if roles are there

            //// Lookup roles in database

            //HttpContext.Current.Session["Authority"] =
            //    PirateWeb.Logic.Security.Authorization.GetPersonAuthority(currentUserId);

            //// TODO: Set CurrentUser to new object, with roles
        }
        */
    }

    private void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started

        // This part really should have been in Application_Start, but it doesn't fire for some reason
    }

    private void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }

</script>