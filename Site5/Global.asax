<%@ Application Language="C#" %>
<%@ Import Namespace="System.IO" %>


<script runat="server">

    private void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup

        // Set supported cultures
        HttpContext.Current.Application["Cultures"] = new[] {"sv-SE", "en-US", "en-GB", "de-DE", "de-AT", "fi-FI"};

        HttpContext.Current.Application["UserRoleCache"] = new Dictionary<int, string[]>();
    }

    private void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown
    }

    private void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs
    }


    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        // The culture was once set here for localization, but Mono resets CultureInfo before entering the page cycle.
        // Setting the CultureInfo has been moved to PageV5Base.OnPreInit().

        // TODO: PLUGIN REWRITES
        
        // Testing for page rewrite. Tests IN ORDER.

        string requestPath = Request.Path;

        if (requestPath == "/Default.aspx")
        {
            // Mono sends this path instead of the user-submitted "/" received on Windows; compensate for Mono
            requestPath = "/";
        }
        
        string[] rewriteCandidates =
        {
            "{0}/Dashboard.aspx",        // for dev.swarmops.com   -- Dashboard overrides Default in root, if it exists
            "{0}Dashboard.aspx",         // for dev.swarmops.com/  -- Dashboard overrides Default in root, if it exists
            "{0}/Default.aspx",          // for dev.swarmops.com
            "{0}Default.aspx",           // for dev.swarmops.com/
            "{0}.aspx",                  // for dev.swarmops.com/Security/Login
            "/Pages/v5/Public{0}.aspx",  // for dev.swarmops.com/Signup (and others under Public)
            "/Pages/v5/Public{0}",       // for dev.swarmops.com/WizardStyle.css (linked from pages in Public)
            "/Pages/v5{0}.aspx",         // for dev.swarmops.com/User/SelectLanguage
            "/Pages/v5{0}"               // for dev.swarmops.com/Ledgers/Json-SomethingData.aspx
        };

        File.AppendAllText ("/etc/swarmops/debug.txt", "\n---Debugging: PATH is " + requestPath +"\n");
        
        foreach (string stringCandidate in rewriteCandidates)
        {
            string rewrittenCandidate = String.Format (stringCandidate, requestPath);
            
            File.AppendAllText ("/etc/swarmops/debug.txt", "Testing for " + rewrittenCandidate);
            
            if (File.Exists (Server.MapPath (rewrittenCandidate)))
            {
                File.AppendAllText ("/etc/swarmops/debug.txt", " - HIT! Recontexting\n");
                
                Context.RewritePath (rewrittenCandidate);
                break;
            }
            
            File.AppendAllText ("/etc/swarmops/debug.txt", "\n");
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