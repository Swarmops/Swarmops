using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using Activizr.Basic.Types;
using Activizr.Site.Automation;
using Telerik.Web.UI;

using Activizr.Database;
using Country = Activizr.Logic.Structure.Country;

public partial class Pages_v5_Init_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Unlock Telerik

        this.Application["Telerik.Web.UI.Key"] = "Activizr";

        this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginTop] = "3px";
        this.ImageCultureIndicator.Style[HtmlTextWriterStyle.MarginRight] = "3px";
        this.ImageCultureIndicator.Style[HtmlTextWriterStyle.Cursor] = "pointer";

        this.MainMenu.Style[HtmlTextWriterStyle.Position] = "relative";
        this.MainMenu.Style[HtmlTextWriterStyle.Top] = "7px";
        this.MainMenu.Style[HtmlTextWriterStyle.Left] = "20px";

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

        this.LanguageSelector.LanguageChanged += new EventHandler(LanguageSelector_LanguageChanged);

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

        SetupMenuItems();

        string flagName = "uk";

        string cultureString = Thread.CurrentThread.CurrentCulture.ToString();
        string cultureStringLower = cultureString.ToLowerInvariant();

        if (cultureStringLower != "en-gb" && cultureString.Length > 3)
        {
            flagName = cultureStringLower.Substring(3);
        }
        this.ImageCultureIndicator.ImageUrl = "~/Images/Flags/" + flagName + ".png";
    }


    // This section is copied from Master-v5.master.cs, except all items are disabled:

    private void SetupMenuItems()
    {
        RadMenuItemCollection menuItems = this.MainMenu.Items;
        SetupMenuItemsRecurse(menuItems, true);
    }

    private bool SetupMenuItemsRecurse(RadMenuItemCollection menuItems, bool topLevel)
    {
        // string thisPageUrl = Request.Url.Segments[Request.Url.Segments.Length - 1].ToLower();
        bool anyItemEnabled = false;

        foreach (RadMenuItem item in menuItems)
        {
            int itemUserLevel = Convert.ToInt32(item.Attributes["UserLevel"]);
            string authorization = item.Attributes["Permission"];
            string resourceKey = item.Attributes["GlobalResourceKey"];
            string url = item.NavigateUrl;
            string dynamic = item.Attributes["Dynamic"];

            item.Visible = true; // Modified
            bool enabled = topLevel;

            if (item.IsSeparator)
            {
                continue;
            }

            if (dynamic == "true")
            {
                switch (item.Attributes["Template"])
                {
                    case "Build#":
                        item.Text = GetBuildIdentity(); // only dynamically constructed atm -- if more, switch on "template" field
                        break;
                    case "CloseLedgers":
                        int year = DateTime.Today.Year;
                        item.Text = String.Format(Resources.Menu5.Menu5_Ledgers_CloseBooks, year - 1);  // Modified
                        break;
                    default:
                        throw new InvalidOperationException("No case for dynamic menu item" + item.Attributes["Template"]);
                }
            }
            else
            {
                item.Text = GetGlobalResourceObject("Menu5", resourceKey).ToString();
            }

            if (item.Visible)
            {
                enabled |= SetupMenuItemsRecurse(item.Items, false);
                enabled |= !String.IsNullOrEmpty(url);
            }

            item.Enabled = topLevel;  // Modified
            if (enabled)
            {
                anyItemEnabled = true;
            }
        }

        return anyItemEnabled;
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

    protected void ButtonInitDatabase_Click (object sender, EventArgs args)
    {
        // Store database credentials

        PirateDb.Configuration.Set(
            new PirateDb.Configuration(
                new PirateDb.Credentials(
                    this.TextCredentialsReadDatabase.Text,
                    new PirateDb.ServerSet(this.TextCredentialsReadServer.Text),
                    this.TextCredentialsReadUser.Text,
                    this.TextCredentialsReadPassword.Text),
                new PirateDb.Credentials(this.TextCredentialsWriteDatabase.Text,
                    new PirateDb.ServerSet(this.TextCredentialsWriteServer.Text),
                    this.TextCredentialsWriteUser.Text,
                    this.TextCredentialsWritePassword.Text),
                new PirateDb.Credentials(this.TextCredentialsAdminDatabase.Text,
                    new PirateDb.ServerSet(this.TextCredentialsAdminServer.Text),
                    this.TextCredentialsAdminUser.Text,
                    this.TextCredentialsAdminPassword.Text)));
    }

    [WebMethod]
    public static void InitDatabase()
    {
        // Start an async thread that does all the work, then return

        HttpSessionState currentSessionObject = HttpContext.Current.Session;

        Thread initThread = new Thread(InitDatabaseThread);
        initThread.Start();
    }


    /// <summary>
    /// This function copies the schemas and geography data off an existing Activizr installation. Runs in its own thread.
    /// </summary>
    public static void InitDatabaseThread()
    {
        // Ignore the session object, that method of sharing data didn't work, but a static variable did.

        _initProgress = 1;
        _initMessage = "Loading schema from Activizr servers; creating tables and procs...";

        // Get the schema and initialize the database structures. Requires ADMIN access to database.

        Activizr.Logic.Support.DatabaseMaintenance.FirstInitialization();

        _initProgress = 5;
        _initMessage = "Getting list of countries from Activizr servers...";

        // Create translation lists

        Dictionary<int, int> geographyIdTranslation = new Dictionary<int, int>();
        Dictionary<int, int> cityIdTranslation = new Dictionary<int, int>();
        Dictionary<string, int> countryIdTranslation = new Dictionary<string, int>();
        Dictionary<int, bool> cityIdsUsedLookup = new Dictionary<int, bool>();

        // Initialize the root geography (which becomes #1 if everything works)

        int rootGeographyId = PirateDb.GetDatabaseForWriting().CreateGeography("World", 0);

        // Get the list of countries

        Activizr.Site.Automation.GetGeographyData geoDataFetcher = new GetGeographyData();

        Activizr.Site.Automation.Country[] countries = geoDataFetcher.GetCountries();

        _initProgress = 7;
        _initMessage = "Creating all countries on local server...";

        // Create all countries in our own database

        foreach (Activizr.Site.Automation.Country country in countries)
        {
            countryIdTranslation[country.Code] = PirateDb.GetDatabaseForWriting().CreateCountry(country.Name,
                                                                                                country.Code,
                                                                                                country.Culture,
                                                                                                rootGeographyId, 5,
                                                                                                string.Empty);
        }

        _initProgress = 10;

        // Construct list of countries that have geographic data

        List<string> initializableCountries = new List<string>();

        foreach (Activizr.Site.Automation.Country country in countries)
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

            Activizr.Site.Automation.Geography geography = geoDataFetcher.GetGeographyForCountry(countryCode);

            _initProgress = 10 + (int) (countryCount*initStepPerCountry + initStepPerCountry/6);
            _initMessage = "Setting up geography for " + countryCode + "...";

            // Create the country's root geography

            int countryRootGeographyId = PirateDb.GetDatabaseForWriting().CreateGeography(geography.Name, rootGeographyId);
            geographyIdTranslation[geography.GeographyId] = countryRootGeographyId;
            PirateDb.GetDatabaseForWriting().SetCountryGeographyId(countryIdTranslation[countryCode],
                                                                   countryRootGeographyId);

            InitDatabaseThreadCreateGeographyChildren(geography.Children, countryRootGeographyId, ref geographyIdTranslation);

            _initProgress = 10 + (int) (countryCount*initStepPerCountry + initStepPerCountry/3);
            _initMessage = "Retrieving cities for " + countryCode + "...";

            // Get the postal codes and cities

            Activizr.Site.Automation.City[] cities = null;

            try
            {
                cities = geoDataFetcher.GetCitiesForCountry(countryCode);
            }
            catch (SoapHeaderException)
            {
                // This is typically a country that isn't populated with cities yet. Ignore.
                countryCount++;
                continue;
            }


            _initProgress = 10 + (int) (countryCount*initStepPerCountry + initStepPerCountry/2);
            _initMessage = "Retrieving postal codes for " + countryCode + "...";

            Activizr.Site.Automation.PostalCode[] postalCodes = geoDataFetcher.GetPostalCodesForCountry(countryCode);

            // Find which cities are actually used

            foreach (Activizr.Site.Automation.PostalCode postalCode in postalCodes)
            {
                cityIdsUsedLookup[postalCode.CityId] = true;
            }

            _initProgress = 10 + (int) (countryCount*initStepPerCountry + initStepPerCountry*2/3);

            // Insert cities

            int newCountryId = countryIdTranslation[countryCode];
            int cityCount = 0;

            foreach (Activizr.Site.Automation.City city in cities)
            {
                if (!geographyIdTranslation.ContainsKey(city.GeographyId))
                {
                    cityIdsUsedLookup[city.CityId] = false; // force non-use of invalid city
                }

                if (cityIdsUsedLookup[city.CityId])
                {
                    int newGeographyId = geographyIdTranslation[city.GeographyId];
                    int newCityId = PirateDb.GetDatabaseForWriting().CreateCity(city.Name, newCountryId,
                                                                                newGeographyId);
                    cityIdTranslation[city.CityId] = newCityId;
                }

                if (cityCount % 100 == 0)
                {
                    _initMessage = String.Format("Setting up cities for {0} ({1}/{2})...", countryCode, cityCount,
                                                 cities.Count());
                }

                cityCount++;

            }

            // Insert postal codes

            _initProgress = 10 + (int) (countryCount*initStepPerCountry + initStepPerCountry*5/6);
            int postalCodeCount = 0;

            foreach (Activizr.Site.Automation.PostalCode postalCode in postalCodes)
            {
                if (cityIdsUsedLookup[postalCode.CityId] == false)
                {
                    // Remnants of invalid pointers

                    postalCodeCount++;
                    continue;
                }

                int newCityId = cityIdTranslation[postalCode.CityId];
                PirateDb.GetDatabaseForWriting().CreatePostalCode(postalCode.PostalCode, newCityId, newCountryId);

                if (postalCodeCount % 100 == 0)
                {
                    _initMessage = String.Format("Setting up postal codes for {0} ({1}/{2})...", countryCode, postalCodeCount,
                                                 postalCodes.Count());
                }

                postalCodeCount++;
            }

            countryCount++;

            _initProgress = 10 + (int) (countryCount*initStepPerCountry);
        }

        _initProgress = 100;
        _initMessage = "Complete.";

        Thread.Sleep(1000); // give some time for static var to stick and web interface to react before killing thread
    }

    private static void InitDatabaseThreadCreateGeographyChildren (Activizr.Site.Automation.Geography[] children, int parentGeographyId, ref Dictionary<int,int> geographyIdTranslation)
    {
        foreach (Activizr.Site.Automation.Geography geography in children)
        {
            int newGeographyId = PirateDb.GetDatabaseForWriting().CreateGeography(geography.Name, parentGeographyId);
            geographyIdTranslation[geography.GeographyId] = newGeographyId;

            InitDatabaseThreadCreateGeographyChildren(geography.Children, newGeographyId, ref geographyIdTranslation);
        }
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
            int progress = (int)HttpContext.Current.Session["PercentInitComplete"];

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

        using (TextReader reader = new StreamReader("/etc/hostname"))  // This will throw in a Windows dev environment
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
    public static bool VerifyHostAddress (string input)
    {
        string localAddress = HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];

        if (input == localAddress)
        {
            return true;
        }

        return false;
    }

    [WebMethod(true)]
    public static bool VerifyHostNameAndAddress (string name, string address)
    {
        return VerifyHostName(name) && VerifyHostAddress(address);
    }

    [WebMethod(true)]
    public static bool IsConfigurationFileWritable()
    {
        return PirateDb.Configuration.TestConfigurationWritable();
    }
}