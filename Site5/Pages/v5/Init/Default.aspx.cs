using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using Swarmops.Basic.Enums;
using Swarmops.Database;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;
using Swarmops.Site.Automation;

public partial class Pages_v5_Init_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginTop] = "3px";
        this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginRight] = "3px";
        this.ImageCultureIndicator.Style[HtmlTextWriterStyle.Cursor] = "pointer";

        this.TextCredentialsReadDatabase.Style[HtmlTextWriterStyle.Width] = "70px";
        this.TextCredentialsReadServer.Style[HtmlTextWriterStyle.Width] = "70px";
        this.TextCredentialsReadUser.Style[HtmlTextWriterStyle.Width] = "70px";
        this.TextCredentialsReadPassword.Style[HtmlTextWriterStyle.Width] = "70px";

        this.TextCredentialsWriteDatabase.Style[HtmlTextWriterStyle.Width] = "70px";
        this.TextCredentialsWriteServer.Style[HtmlTextWriterStyle.Width] = "70px";
        this.TextCredentialsWriteUser.Style[HtmlTextWriterStyle.Width] = "70px";
        this.TextCredentialsWritePassword.Style[HtmlTextWriterStyle.Width] = "70px";

        this.TextCredentialsAdminDatabase.Style[HtmlTextWriterStyle.Width] = "70px";
        this.TextCredentialsAdminServer.Style[HtmlTextWriterStyle.Width] = "70px";
        this.TextCredentialsAdminUser.Style[HtmlTextWriterStyle.Width] = "70px";
        this.TextCredentialsAdminPassword.Style[HtmlTextWriterStyle.Width] = "70px";

        this.DropFavoriteColor.Style[HtmlTextWriterStyle.Width] = "155px";

        // this.LanguageSelector.LanguageChanged += new EventHandler(LanguageSelector_LanguageChanged);

        Localize();
    }

    protected override void OnPreInit(EventArgs e)
    {
        // This OnPreInit is copied from master page base

        // Localization

        // Set default culture (English, United States, but that doesn't work so fake it to GB)

        string preferredCulture = "en-GB";

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
            string browserPreference = "en-GB";
            if (Request.UserLanguages != null && Request.UserLanguages.Length > 0)
            {
                browserPreference = Request.UserLanguages[0];
                preferredCulture = browserPreference;
            }

            if (preferredCulture == "en-US")
            {
                preferredCulture = "en-GB"; // Hack because of malfunctioning Telerik popup
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

        base.OnPreInit(e);
    }

    public event EventHandler LanguageChanged;

    private void LanguageSelector_LanguageChanged(object sender, EventArgs e)
    {
        // Received event from control - refire

        if (LanguageChanged != null)
        {
            LanguageChanged(this, new EventArgs());
        }

        Localize();
    }



    private void Localize()
    {
        this.LabelCurrentUserName.Text = Resources.Pages.Init.Init_UserInfo_InstallingAdmin;
        this.LabelCurrentOrganizationName.Text = Resources.Pages.Init.Init_UserInfo_NoOrgsYet;
        this.LabelPreferences.Text = Resources.Global.CurrentUserInfo_Preferences;
        this.LabelSidebarInfoHeader.Text = Resources.Global.Sidebar_Information;
        this.LabelSidebarInfoContent.Text = Resources.Pages.Init.Init_SidebarInfo_Welcome;
        this.LabelSidebarActionsHeader.Text = Resources.Global.Sidebar_Actions;
        this.LabelSidebarActionsContent.Text = Resources.Pages.Init.Init_SidebarActions_None;
        this.LabelSidebarTodoHeader.Text = Resources.Global.Sidebar_Todo;
        this.LabelSidebarTodoConnectDatabase.Text = Resources.Pages.Init.Init_SidebarTodo_CompleteSetup;

        this.DropFavoriteColor.Items.Clear();
        this.DropFavoriteColor.Items.Add(" -- Select one --");
        this.DropFavoriteColor.Items.Add("Blue!");
        this.DropFavoriteColor.Items.Add("No, wait, yellow!");

        // SetupMenuItems();

        string flagName = "uk";

        string cultureString = Thread.CurrentThread.CurrentCulture.ToString();
        string cultureStringLower = cultureString.ToLowerInvariant();

        if (cultureStringLower != "en-gb" && cultureString.Length > 3)
        {
            flagName = cultureStringLower.Substring(3);
        }
        this.ImageCultureIndicator.ImageUrl = "~/Images/Flags/" + flagName + ".png";
    }


    private static string _buildIdentity;


    private string GetBuildIdentity()
    {
        // Read build number if not loaded, or set to "Private" if none

        if (_buildIdentity == null)
        {
            try
            {
                using (StreamReader reader = File.OpenText(HttpContext.Current.Request.MapPath("~/BuildIdentity.txt")))
                {
                    _buildIdentity = "Build " + reader.ReadLine();
                }
            }
            catch (Exception)
            {
                _buildIdentity = "Private Build";
            }
        }

        return _buildIdentity;
    }

    [WebMethod]
    static public PermissionsAnalysis FirstCredentialsTest
        (string readDatabase, string readServer, string readUser, string readPassword,
        string writeDatabase, string writeServer, string writeUser, string writePassword,
        string adminDatabase, string adminServer, string adminUser, string adminPassword,
        string serverName, string ipAddress)
    {
        if (!(VerifyHostName(serverName) && VerifyHostAddress(ipAddress)))
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                return null; // Probable hack attempt - fail silently
            }
        }

        _testReadCredentials = new SwarmDb.Credentials(
                    readDatabase, new SwarmDb.ServerSet(readServer), readUser, readPassword);
        _testWriteCredentials = new SwarmDb.Credentials(
                    writeDatabase, new SwarmDb.ServerSet(writeServer), writeUser, writePassword);
        _testAdminCredentials = new SwarmDb.Credentials(
                    adminDatabase, new SwarmDb.ServerSet(adminServer), adminUser, adminPassword);

        return RecheckDatabasePermissions();  // Subsequent tests only call this function
    }

    private static SwarmDb.Credentials _testReadCredentials;
    private static SwarmDb.Credentials _testWriteCredentials;
    private static SwarmDb.Credentials _testAdminCredentials;


    [WebMethod(true)]
    public static void ResetTestCredentials()
    {
        _testReadCredentials =
            _testWriteCredentials =
                _testAdminCredentials = null;
    }


    [WebMethod(true)]
    public static PermissionsAnalysis RecheckDatabasePermissions()
    {
        while (_testReadCredentials == null || _testWriteCredentials == null || _testAdminCredentials == null)
        {
            Thread.Sleep(100); // A couple of async race conditions happen as this is called, we need to wait for credentials
        }

        PermissionsAnalysis result = new PermissionsAnalysis();

        // First, test ADMIN

        SwarmDb adminDb = SwarmDb.GetTestDatabase(_testAdminCredentials);

        // Drop table, procedure first just in case there's garbage left behind. Ignore result.
        adminDb.TestDropTable();
        adminDb.TestDropProcedure();

        // All these should pass.
        result.AdminCredentialsCanLogin = adminDb.TestLogin();
        result.AdminCredentialsCanAdmin = adminDb.TestCreateTable();
        result.AdminCredentialsCanAdmin &= adminDb.TestDropTable();
        result.AdminCredentialsCanAdmin &= adminDb.TestCreateTable();
        result.AdminCredentialsCanAdmin &= adminDb.TestAlterTable();
        result.AdminCredentialsCanAdmin &= adminDb.TestCreateProcedure();  // AND -- all must succeed
        result.AdminCredentialsCanAdmin &= adminDb.TestDropProcedure();    // Test DROP before we mess up the state of the table, procedure
        result.AdminCredentialsCanAdmin &= adminDb.TestCreateProcedure();  // therefore, recreate it after the drop

        if (result.AdminCredentialsCanAdmin) // if we have a created table and procedure, otherwise default fail
        {
            result.AdminCredentialsCanExecute = adminDb.TestExecute("Admin Execute");
            result.AdminCredentialsCanSelect = adminDb.TestSelect();
        }

        // Within the created table, test WRITE and READ accounts before testing them on excessive rights.

        SwarmDb writeDb = SwarmDb.GetTestDatabase(_testWriteCredentials);

        result.WriteCredentialsCanLogin = writeDb.TestLogin();

        if (result.WriteCredentialsCanLogin && result.AdminCredentialsCanAdmin)
        {
            result.WriteCredentialsCanExecute = writeDb.TestExecute("Write Execute");
            result.WriteCredentialsCanSelect = writeDb.TestSelect();
        }

        SwarmDb readDb = SwarmDb.GetTestDatabase(_testReadCredentials);

        result.ReadCredentialsCanLogin = readDb.TestLogin();

        if (result.ReadCredentialsCanLogin && result.AdminCredentialsCanAdmin)
        {
            result.ReadCredentialsCanExecute = readDb.TestExecute("Read Execute");
            result.ReadCredentialsCanSelect = readDb.TestSelect();
        }

        // Finally, test the write and read accounts for admin rights. Note the "OR" here rather than "AND" -
        // any one of these rights present should return a true, because it's a fail.

        if (result.ReadCredentialsCanLogin)
        {
            result.ReadCredentialsCanAdmin = readDb.TestDropProcedure();
            result.ReadCredentialsCanAdmin |= readDb.TestDropTable();
            result.ReadCredentialsCanAdmin |= readDb.TestCreateTable();
            result.ReadCredentialsCanAdmin |= readDb.TestCreateProcedure();
        }

        if (result.WriteCredentialsCanLogin)
        {
            result.WriteCredentialsCanAdmin = writeDb.TestDropProcedure();
            result.WriteCredentialsCanAdmin |= writeDb.TestDropTable();
            result.WriteCredentialsCanAdmin |= writeDb.TestCreateTable();
            result.WriteCredentialsCanAdmin |= writeDb.TestCreateProcedure();
        }

        // Clean up

        adminDb.TestDropTable(); // ignore result
        adminDb.TestDropProcedure();

        result.AllPermissionsOk =
            result.AdminCredentialsCanLogin &&
            result.AdminCredentialsCanSelect &&
            result.AdminCredentialsCanExecute &&
            result.AdminCredentialsCanAdmin &&
            result.WriteCredentialsCanLogin &&
            result.WriteCredentialsCanSelect &&
            result.WriteCredentialsCanExecute &&
            !result.WriteCredentialsCanAdmin &&  // not this
            result.ReadCredentialsCanLogin &&
            result.ReadCredentialsCanSelect &&
            !result.ReadCredentialsCanExecute && // not this
            !result.ReadCredentialsCanAdmin;     // not this

        return result;
    }


    [WebMethod(true)]
    public static void InitDatabase()
    {
        // Make sure we're uninitialized

        bool organizationOneExists = false;
        Swarmops.Logic.Structure.Organization organizationOne = null;

        try
        {
            organizationOne = Swarmops.Logic.Structure.Organization.FromIdentity(1);
            organizationOneExists = true;
        }
        catch (Exception)
        {
            // We expect this to throw
        }

        if (organizationOneExists || organizationOne != null)
        {
            throw new InvalidOperationException("Cannot re-initialize database");
        }

        // Store database credentials

        SwarmDb.Configuration.Set(
            new SwarmDb.Configuration(
                _testReadCredentials,
                _testWriteCredentials,
                _testAdminCredentials));

        // Start an async thread that does all the work, then return

        Thread initThread = new Thread(InitDatabaseThread);
        initThread.Start();
    }


    /// <summary>
    /// This function copies the schemas and geography data off an existing Swarmops installation. Runs in its own thread.
    /// </summary>
    public static void InitDatabaseThread()
    {
        // Ignore the session object, that method of sharing data didn't work, but a static variable did.

        _initProgress = 1;
        _initMessage = "Loading schema from Swarmops servers; creating tables and procs...";
        Thread.Sleep(100);

        try
        {

        // Get the schema and initialize the database structures. Requires ADMIN access to database.

        Swarmops.Logic.Support.DatabaseMaintenance.FirstInitialization();

        _initProgress = 6;
        _initMessage = "Applying all post-baseline database schema upgrades...";
        Swarmops.Logic.Support.DatabaseMaintenance.UpgradeSchemata();
        Thread.Sleep(100);

        _initProgress = 5;
        _initMessage = "Getting list of countries from Swarmops servers...";
        Thread.Sleep(100);

        // Create translation lists

        Dictionary<int, int> geographyIdTranslation = new Dictionary<int, int>();
        Dictionary<int, int> cityIdTranslation = new Dictionary<int, int>();
        Dictionary<string, int> countryIdTranslation = new Dictionary<string, int>();
        Dictionary<int, bool> cityIdsUsedLookup = new Dictionary<int, bool>();

        // Initialize the root geography (which becomes #1 if everything works)

        int rootGeographyId = SwarmDb.GetDatabaseForWriting().CreateGeography("World", 0);

        // Get the list of countries

        Swarmops.Site.Automation.GetGeographyData geoDataFetcher = new GetGeographyData();

        Swarmops.Site.Automation.Country[] countries = geoDataFetcher.GetCountries();

        _initProgress = 7;
        _initMessage = "Creating all countries on local server...";
        Thread.Sleep(100);
        int count = 0;
        int total = countries.Length;

        // Create all countries in our own database

        foreach (Swarmops.Site.Automation.Country country in countries)
        {
            countryIdTranslation[country.Code] = SwarmDb.GetDatabaseForWriting().CreateCountry(country.Name,
                                                                                                country.Code,
                                                                                                country.Culture,
                                                                                                rootGeographyId, 5,
                                                                                                string.Empty);

            count++;
            _initMessage = String.Format("Creating all countries on local server... ({0}%)", count*100/total);
        }

        _initProgress = 10;

        // Construct list of countries that have geographic data

        List<string> initializableCountries = new List<string>();

        foreach (Swarmops.Site.Automation.Country country in countries)
        {
            if (country.GeographyId != 1)
            {
                initializableCountries.Add(country.Code);
            }
        }

        float initStepPerCountry = 90f/initializableCountries.Count;
        int countryCount = 0;

        // For each country...

        foreach (string countryCode in initializableCountries)
        {
            // Get the geography layout

            _initMessage = "Retrieving geography for " + countryCode + "...";
            Thread.Sleep(100);

            Swarmops.Site.Automation.Geography geography = geoDataFetcher.GetGeographyForCountry(countryCode);

            _initProgress = 10 + (int) (countryCount*initStepPerCountry + initStepPerCountry/6);
            _initMessage = "Setting up geography for " + countryCode + "...";
            Thread.Sleep(100);

            // Create the country's root geography

            int countryRootGeographyId = SwarmDb.GetDatabaseForWriting().CreateGeography(geography.Name,
                                                                                          rootGeographyId);
            geographyIdTranslation[geography.GeographyId] = countryRootGeographyId;
            SwarmDb.GetDatabaseForWriting().SetCountryGeographyId(countryIdTranslation[countryCode],
                                                                   countryRootGeographyId);

            count = 0;
            total = InitDatabaseThreadCountGeographyChildren (geography.Children);

            InitDatabaseThreadCreateGeographyChildren(geography.Children, countryRootGeographyId,
                ref geographyIdTranslation, countryCode, ref count, total);

            _initProgress = 10 + (int) (countryCount*initStepPerCountry + initStepPerCountry/3);
            _initMessage = "Retrieving cities for " + countryCode + "...";
            Thread.Sleep(100);

            // Get the postal codes and cities

            Swarmops.Site.Automation.City[] cities = null;

            try
            {
                cities = geoDataFetcher.GetCitiesForCountry(countryCode);
            }
            catch (Exception)
                // This is a SoapHeaderException in VS debugging, but SOMETHING ELSE! in Mono runtime, so make it generic
            {
                // This is typically a country that isn't populated with cities yet. Ignore.
                countryCount++;
                continue;
            }


            _initProgress = 10 + (int) (countryCount*initStepPerCountry + initStepPerCountry/2);
            _initMessage = "Retrieving postal codes for " + countryCode + "...";
            Thread.Sleep(100);

            Swarmops.Site.Automation.PostalCode[] postalCodes = geoDataFetcher.GetPostalCodesForCountry(countryCode);

            // Find which cities are actually used

            foreach (Swarmops.Site.Automation.PostalCode postalCode in postalCodes)
            {
                cityIdsUsedLookup[postalCode.CityId] = true;
            }

            _initProgress = 10 + (int) (countryCount*initStepPerCountry + initStepPerCountry*2/3);

            // Insert cities

            int newCountryId = countryIdTranslation[countryCode];

            int cityIdHighwater = SwarmDb.GetDatabaseForAdmin().ExecuteAdminCommandScalar("SELECT Max(CityId) FROM Cities;");

            _initMessage = string.Format("Setting up {0:N0} cities for {1}...", cities.Length, countryCode);

            StringBuilder sqlCityBuild = new StringBuilder("INSERT INTO Cities (CityName, GeographyId, CountryId, Comment) VALUES ", 65536);
            bool insertComma = false;

            foreach (Swarmops.Site.Automation.City city in cities)
            {
                if (!geographyIdTranslation.ContainsKey(city.GeographyId))
                {
                    cityIdsUsedLookup[city.CityId] = false; // force non-use of invalid city
                }

                if (cityIdsUsedLookup[city.CityId])
                {
                    int newGeographyId = geographyIdTranslation[city.GeographyId];

                    if (insertComma)
                    {
                        sqlCityBuild.Append(",");
                    }

                    sqlCityBuild.Append("('" + city.Name.Replace("'", "\'") + "'," + newGeographyId.ToString() + "," +
                        newCountryId.ToString() + ",'')");
                    insertComma = true;

                    cityIdTranslation[city.CityId] = ++cityIdHighwater;  // Note that we assume the assigned ID here.
                }
            }

            sqlCityBuild.Append(";");

            SwarmDb.GetDatabaseForAdmin().ExecuteAdminCommand(sqlCityBuild.ToString()); // Inserts all cities in one bulk op, to save roundtrips

            // Insert postal codes

            _initProgress = 10 + (int) (countryCount*initStepPerCountry + initStepPerCountry*5/6);
            _initMessage = string.Format("Setting up {0:N0} postal codes for {1}...", postalCodes.Length, countryCode);

            StringBuilder sqlBuild = new StringBuilder("INSERT INTO PostalCodes (PostalCode, CityId, CountryId) VALUES ", 65536);
            insertComma = false;

            foreach (Swarmops.Site.Automation.PostalCode postalCode in postalCodes)
            {
                if (cityIdsUsedLookup[postalCode.CityId] == false)
                {
                    // Remnants of invalid pointers

                    continue;
                }

                int newCityId = cityIdTranslation[postalCode.CityId];

                if (insertComma)
                {
                    sqlBuild.Append(",");
                }

                sqlBuild.Append("('" + postalCode.PostalCode.Replace("'", "\'") + "'," + newCityId.ToString() + "," +
                       newCountryId.ToString() + ")");
                insertComma = true;
            }

            sqlBuild.Append(";");

            SwarmDb.GetDatabaseForAdmin().ExecuteAdminCommand(sqlBuild.ToString()); // Inserts all postal codes in one bulk op, to save roundtrips

            countryCount++;

            _initProgress = 10 + (int) (countryCount*initStepPerCountry);
        }

        // Set Geography at baseline (TODO: Ask for what baseline we got)

        Persistence.Key["LastGeographyUpdateId"] = "0";

        // Set an installation ID

        Persistence.Key["SwarmopsInstallationId"] = Guid.NewGuid().ToString();

        // Create initial currencies (European)

        Currency.Create("EUR", "Euros", "€");
        Currency.Create("USD", "US Dollars", "$");
        Currency.Create("CAD", "Canadian Dollars", "CA$");
        Currency.Create("SEK", "Swedish Krona", string.Empty);
        Currency.Create("NOK", "Norwegian Krona", string.Empty);
        Currency.Create("DKK", "Danish Krona", string.Empty);
        Currency.Create("ISK", "Icelandic Krona", string.Empty);
        Currency.Create("CHF", "Swiss Franc", string.Empty);
        Currency.Create("GBP", "Pounds Sterling", "£");
        Currency.Create("BTC", "Bitcoin", "฿");

        // Create the sandbox

        Swarmops.Logic.Structure.Organization.Create(0, "Sandbox", "Sandbox", "Sandbox", "swarmops.com", "Ops",
                                                     rootGeographyId, true,
                                                     true, 0).EnableEconomy(Swarmops.Logic.Financial.Currency.FromCode("EUR"));

        _initProgress = 100;
        _initMessage = "Complete.";

        }
        catch (Exception failedException)
        {
            // Use initMessage to push info about what went wrong to the user

            _initMessage = failedException.ToString();
        }

        Thread.Sleep(1000); // give some time for static var to stick and web interface to react before killing thread
    }

    private static int InitDatabaseThreadCountGeographyChildren(Swarmops.Site.Automation.Geography[] children)
    {
        int count = 0;

        foreach (Swarmops.Site.Automation.Geography child in children)
        {
            count++;
            count += InitDatabaseThreadCountGeographyChildren(child.Children);
        }

        return count;
    }

    private static void InitDatabaseThreadCreateGeographyChildren(Swarmops.Site.Automation.Geography[] children,
                                                                  int parentGeographyId,
                                                                  ref Dictionary<int, int> geographyIdTranslation, string countryCode, ref int count, int total)
    {
        count++;

        foreach (Swarmops.Site.Automation.Geography geography in children)
        {
            int newGeographyId = SwarmDb.GetDatabaseForWriting().CreateGeography(geography.Name, parentGeographyId);
            geographyIdTranslation[geography.GeographyId] = newGeographyId;

            InitDatabaseThreadCreateGeographyChildren(geography.Children, newGeographyId, ref geographyIdTranslation, countryCode, ref count, total);
        }

        if (total != 0)
        {
            _initMessage = String.Format("Setting up geography for {0}... ({1}%)", countryCode, count*100/total);
                // shouldn't be here but wtf
        }
        Thread.Sleep(50);
    }


    private static int _initProgress = 0;
    private static string _initMessage = "Initializing...";


    [WebMethod]
    public static int GetInitProgress()
    {
        HttpSessionState state = HttpContext.Current.Session;
        object progressObject = state["PercentInitComplete"];

        try
        {
            int progress = (int) HttpContext.Current.Session["PercentInitComplete"];

            return progress;
        }
        catch (Exception)
        {
            return _initProgress;
        }
    }

    [WebMethod]
    public static string GetInitProgressMessage()
    {
        return _initMessage;
    }

    [WebMethod]
    public static bool VerifyHostName(string input)
    {
        if (System.Diagnostics.Debugger.IsAttached)
        {
            return true; // Cannot read "/etc/hostname" in Windows environment
        }

        // validate against /etc/hostname

        string realHostName;

        using (TextReader reader = new StreamReader("/etc/hostname")) // This will throw in a Windows dev environment
        {
            realHostName = reader.ReadLine();
        }

        if (String.Compare(input, realHostName, true, CultureInfo.InvariantCulture) == 0) // case-insensitive compare
        {
            return true;
        }

        return false;
    }

    [WebMethod(true)]
    public static bool VerifyHostAddress(string input)
    {
        string localAddress = HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];

        if (input == localAddress)
        {
            return true;
        }

        return false;
    }

    [WebMethod(true)]
    public static bool VerifyHostNameAndAddress(string name, string address)
    {
        if (Path.DirectorySeparatorChar == '\\')
        {
            // Running in development mode on Windows, so accept any response

            return true;
        }

        return VerifyHostName(HttpUtility.UrlDecode(name)) && VerifyHostAddress(HttpUtility.UrlDecode(address));
    }

    [WebMethod(true)]
    public static bool IsConfigurationFileWritable()
    {
        return SwarmDb.Configuration.TestConfigurationWritable();
    }


    private static bool RandomBool()
    {
        return (GetRandomNumber(0, 1) == 0);
    }

    //Function to get random number
    private static readonly Random GetRandom = new Random();
    private static readonly object syncLock = new object();
    private static int GetRandomNumber(int min, int max)
    {
        lock(syncLock) { // synchronize
            return GetRandom.Next(min, max+1);
        }
    }

    public class PermissionsAnalysis
    {
        public bool AllPermissionsOk { get; set; }
        public bool ReadCredentialsCanLogin { get; set; }
        public bool ReadCredentialsCanSelect { get; set; }
        public bool ReadCredentialsCanExecute { get; set; }
        public bool ReadCredentialsCanAdmin { get; set; }
        public bool WriteCredentialsCanLogin { get; set; }
        public bool WriteCredentialsCanSelect { get; set; }
        public bool WriteCredentialsCanExecute { get; set; }
        public bool WriteCredentialsCanAdmin { get; set; }
        public bool AdminCredentialsCanLogin { get; set; }
        public bool AdminCredentialsCanSelect { get; set; }
        public bool AdminCredentialsCanExecute { get; set; }
        public bool AdminCredentialsCanAdmin { get; set; }
    }

    [WebMethod(true)]
    public static void CreateFirstUser(string name, string mail, string password)
    {
        // Make sure that no first person exists already, as a security measure

        Swarmops.Logic.Swarm.Person personOne = null;
        bool personOneExists = false;

        try
        {
            personOne = Swarmops.Logic.Swarm.Person.FromIdentity(1);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                if (personOne.CityName != "Duckville" || personOne.Mail != "noreply@example.com")
                    // these values are returned in debug environments when no person is found
                {
                    personOneExists = true;
                }
                else
                {
                    personOne = null;
                }
            }
            else
            {
                personOneExists = true;
            }
        }
        catch (Exception)
        {
            // We expect this to throw.
        }

        if (personOneExists || personOne != null)
        {
            throw new InvalidOperationException("Cannot run initialization processes again when initialized.");
        }

        Swarmops.Logic.Swarm.Person newPerson = Swarmops.Logic.Swarm.Person.Create(HttpUtility.UrlDecode(name), HttpUtility.UrlDecode(mail),
                                             HttpUtility.UrlDecode(password), string.Empty, string.Empty, string.Empty,
                                             string.Empty, string.Empty, DateTime.MinValue, PersonGender.Unknown);

        newPerson.AddMembership(1, DateTime.MaxValue); // Add membership in Sandbox
        newPerson.AddRole(RoleType.SystemAdmin, 0, 0); // Add role System Admin
    }



    protected void ButtonLogin_Click(object sender, EventArgs args)
    {
        // Check the host names and addresses again as a security measure - after all, we can be called from outside our intended script

        if (!(VerifyHostName(this.TextServerName.Text) && VerifyHostAddress(this.TextServerAddress.Text)))
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                return; // Probable hack attempt - fail silently
            }
        }

        Swarmops.Logic.Swarm.Person expectedPersonOne = Swarmops.Logic.Security.Authentication.Authenticate("1", this.TextFirstUserPassword1.Text);

        if (expectedPersonOne != null)
        {
            FormsAuthentication.RedirectFromLoginPage("1,1", true);
            Response.Redirect("/", true);
        }
    }
}