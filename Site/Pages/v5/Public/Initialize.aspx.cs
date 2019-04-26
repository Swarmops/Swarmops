using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using NBitcoin.Protocol;
using Resources;
using Swarmops.Common;
using Swarmops.Common.Enums;
using Swarmops.Common.Exceptions;
using Swarmops.Common.ExtensionMethods;
using Swarmops.Database;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Automation;
using City = Swarmops.Logic.Automation.City;
using Country = Swarmops.Logic.Automation.Country;
using Geography = Swarmops.Logic.Automation.Geography;
using PostalCode = Swarmops.Logic.Automation.PostalCode;

namespace Swarmops.Frontend.Pages.v5.Public
{
    public partial class Initialize : Page
    {
        private static SwarmDb.Credentials _testReadCredentials;
        private static SwarmDb.Credentials _testWriteCredentials;
        private static SwarmDb.Credentials _testAdminCredentials;
        private static int _initProgress;
        private static bool _testThreadsWork = false;
        private static string _initMessage = "Initializing...";
        private static readonly Random GetRandom = new Random();
        private static readonly object syncLock = new object();

        protected void Page_Load (object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Safety check: If already initialized, throw

                if (SwarmDb.Configuration.IsConfigured())
                {
                    throw new InvalidOperationException (
                        "This installation has already been initialized. Cannot re-initalize on top of existing installation.");
                }

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

                this.TextRandomDbLabel.Text = Authentication.CreateWeakSecret (5);

                // If we're running on Localhost, disable the "is this your server?" question

                if (OnLoopbackInterface())
                {
                    this.TextServerName.Text = @"(localhost)";
                    if (HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"] == "::1")
                    {
                        this.TextServerAddress.Text = @"IPv6 Loopback";
                    }
                    else
                    {
                        this.TextServerAddress.Text = @"IPv4 Loopback";
                    }
                    this.TextServerName.Enabled = false;
                    this.TextServerAddress.Enabled = false;
                }

                Localize();
            }
        }

        protected override void OnPreInit (EventArgs e)
        {
            CommonV5.CulturePreInit (Request);

            base.OnPreInit (e);
        }


        private void Localize()
        {
            this.LabelSidebarInfoHeader.Text = Global.Sidebar_Information;
            this.LabelSidebarInfoContent.Text = @"Welcome to the Swarmops installation pages. This will guide you to an operational Swarmops.";
            this.LabelSidebarActionsHeader.Text = Global.Sidebar_Actions;
            this.LabelSidebarActionsContent.Text = @"No action items.";
            this.LabelSidebarTodoHeader.Text = Global.Sidebar_Todo;
            this.LabelSidebarTodoConnectDatabase.Text = @"Complete setup.";

            this.DropFavoriteColor.Items.Clear();
            this.DropFavoriteColor.Items.Add (" -- Select one --");
            this.DropFavoriteColor.Items.Add ("Blue!");
            this.DropFavoriteColor.Items.Add ("No, wait, yellow!");
        }


        [WebMethod]
        public static void BeginTestThreads()
        {
            Thread checkThread = new Thread(TestServerThreading);
            checkThread.Start();
        }

        public static void TestServerThreading()
        {
            Thread.Sleep (500); // sleep for half a second
            _testThreadsWork = true; // if we get here, we can spawn background threads on server
        }


        [WebMethod]
        public static bool CheckThreadsWork()
        {
            return _testThreadsWork;
        }


        [WebMethod]
        public static AjaxCallResult CreateDatabaseFromRoot (string mysqlHostName, string rootPassword, string serverName,
            string ipAddress, string random)
        {
            if (!(VerifyHostName (serverName) && VerifyHostAddress (ipAddress)))
            {
                if (!Debugger.IsAttached)
                {
                    return true; // Probable hack attempt - fail silently
                }
            }

            try
            {
                random = random.Trim();

                if (random.Length > 5) // if UI-enforced maxlength beaten somehow, limit here
                {
                    random = random.Substring (0, 5); // MySQL will hit a maxlength otherwise
                }

                if (string.IsNullOrEmpty (random))
                {
                    random = Authentication.CreateWeakSecret (5);
                }

                SwarmDb.Credentials rootCredentials = new SwarmDb.Credentials ("mysql",
                    new SwarmDb.ServerSet (mysqlHostName), "root", rootPassword);

                string readPass = GenerateLongPassword();
                string writePass = GenerateLongPassword();
                string adminPass = GenerateLongPassword();

                string[] initInstructions =
                    DbCreateScript.Replace ("[random]", random)
                        .Replace ("[readpass]", readPass)
                        .Replace ("[writepass]", writePass)
                        .Replace ("[adminpass]", adminPass).Split ('#');

                // TODO: Make this function throw a better exception, and catch it
                try
                {
                    SwarmDb.GetTestDatabase(rootCredentials).ExecuteAdminCommands(initInstructions);
                }
                catch (DatabaseExecuteException sqlException)
                {
                    return new AjaxCallResult
                    {
                        Success = false,
                        DisplayMessage = sqlException.AttemptedCommand
                    };
                }

                PermissionsAnalysis permissionsResult = FirstCredentialsTest (
                    "Swarmops-" + random, mysqlHostName, "Swarmops-R-" + random, readPass,
                    "Swarmops-" + random, mysqlHostName, "Swarmops-W-" + random, writePass,
                    "Swarmops-" + random, mysqlHostName, "Swarmops-A-" + random, adminPass,
                    serverName, ipAddress);

                if (!permissionsResult.AllPermissionsOk)
                {
                    // TODO: Return a better exccption detailing exactly what permission isn't set as required

                    return new AjaxCallResult {Success = false};
                }

                return new AjaxCallResult {Success = true};
            }
            catch (Exception)
            {
                return new AjaxCallResult { Success = false };
            }
        }

        private const string DbCreateScript =
            "CREATE DATABASE `Swarmops-[random]`#" +
            "CREATE USER `Swarmops-R-[random]` IDENTIFIED BY '[readpass]'#" +
            "CREATE USER `Swarmops-W-[random]` IDENTIFIED BY '[writepass]'#" +
            "CREATE USER `Swarmops-A-[random]` IDENTIFIED BY '[adminpass]'#" +
            "GRANT SELECT ON mysql.proc TO `Swarmops-W-[random]`#" +
            "GRANT SELECT ON mysql.proc TO `Swarmops-A-[random]`#" +
            "USE `Swarmops-[random]`#" +
            "GRANT ALL ON `Swarmops-[random]`.* TO `Swarmops-A-[random]`#" +
            "GRANT SELECT ON `Swarmops-[random]`.* TO `Swarmops-W-[random]`#" +
            "GRANT EXECUTE ON `Swarmops-[random]`.* TO `Swarmops-W-[random]`#" +
            "GRANT SELECT ON `Swarmops-[random]`.* TO `Swarmops-R-[random]`#" +
            "FLUSH PRIVILEGES";



        private static string GenerateLongPassword()
        {
            return Guid.NewGuid().ToString ("N") + Guid.NewGuid().ToString ("N");
        }

        [WebMethod]
        public static PermissionsAnalysis FirstCredentialsTest
            (string readDatabase, string readServer, string readUser, string readPassword,
                string writeDatabase, string writeServer, string writeUser, string writePassword,
                string adminDatabase, string adminServer, string adminUser, string adminPassword,
                string serverName, string ipAddress)
        {
            if (!(VerifyHostName (serverName) && VerifyHostAddress (ipAddress)))
            {
                if (!Debugger.IsAttached)
                {
                    return null; // Probable hack attempt - fail silently
                }
            }

            _testReadCredentials = new SwarmDb.Credentials (
                readDatabase, new SwarmDb.ServerSet (readServer), readUser, readPassword);
            _testWriteCredentials = new SwarmDb.Credentials (
                writeDatabase, new SwarmDb.ServerSet (writeServer), writeUser, writePassword);
            _testAdminCredentials = new SwarmDb.Credentials (
                adminDatabase, new SwarmDb.ServerSet (adminServer), adminUser, adminPassword);

            return RecheckDatabasePermissions(); // Subsequent tests only call this function
        }


        [WebMethod (true)]
        public static void ResetTestCredentials()
        {
            _testReadCredentials =
                _testWriteCredentials =
                    _testAdminCredentials = null;
        }


        [WebMethod (true)]
        public static PermissionsAnalysis RecheckDatabasePermissions()
        {
            while (_testReadCredentials == null || _testWriteCredentials == null || _testAdminCredentials == null)
            {
                Thread.Sleep (100);
                // A couple of async race conditions happen as this is called, we need to wait for credentials
            }

            PermissionsAnalysis result = new PermissionsAnalysis();

            // First, test ADMIN

            SwarmDb adminDb = SwarmDb.GetTestDatabase (_testAdminCredentials);

            // Drop table, procedure first just in case there's garbage left behind. Ignore result.
            adminDb.TestDropTable();
            adminDb.TestDropProcedure();

            // All these should pass.
            result.AdminCredentialsCanLogin = adminDb.TestLogin();
            result.AdminCredentialsCanAdmin = adminDb.TestCreateTable();
            result.AdminCredentialsCanAdmin &= adminDb.TestDropTable();
            result.AdminCredentialsCanAdmin &= adminDb.TestCreateTable();
            result.AdminCredentialsCanAdmin &= adminDb.TestAlterTable();
            result.AdminCredentialsCanAdmin &= adminDb.TestCreateProcedure(); // AND -- all must succeed
            result.AdminCredentialsCanAdmin &= adminDb.TestDropProcedure();
            // Test DROP before we mess up the state of the table, procedure
            result.AdminCredentialsCanAdmin &= adminDb.TestCreateProcedure(); // therefore, recreate it after the drop

            if (result.AdminCredentialsCanAdmin) // if we have a created table and procedure, otherwise default fail
            {
                result.AdminCredentialsCanExecute = adminDb.TestExecute ("Admin Execute");
                result.AdminCredentialsCanSelect = adminDb.TestSelect();
            }

            // Within the created table, test WRITE and READ accounts before testing them on excessive rights.

            SwarmDb writeDb = SwarmDb.GetTestDatabase (_testWriteCredentials);

            result.WriteCredentialsCanLogin = writeDb.TestLogin();

            if (result.WriteCredentialsCanLogin && result.AdminCredentialsCanAdmin)
            {
                result.WriteCredentialsCanExecute = writeDb.TestExecute ("Write Execute");
                result.WriteCredentialsCanSelect = writeDb.TestSelect();
            }

            SwarmDb readDb = SwarmDb.GetTestDatabase (_testReadCredentials);

            result.ReadCredentialsCanLogin = readDb.TestLogin();

            if (result.ReadCredentialsCanLogin && result.AdminCredentialsCanAdmin)
            {
                result.ReadCredentialsCanExecute = readDb.TestExecute ("Read Execute");
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
                !result.WriteCredentialsCanAdmin && // not this
                result.ReadCredentialsCanLogin &&
                result.ReadCredentialsCanSelect &&
                !result.ReadCredentialsCanExecute && // not this
                !result.ReadCredentialsCanAdmin; // not this

            return result;
        }


        [WebMethod (true)]
        public static void InitDatabase()
        {
            // Make sure we're uninitialized

            bool organizationOneExists = false;
            Organization organizationOne = null;

            try
            {
                organizationOne = Organization.FromIdentity (1);
                organizationOneExists = true;
            }
            catch (Exception)
            {
                // We expect this to throw
            }

            if (organizationOneExists || organizationOne != null)
            {
                throw new InvalidOperationException ("Cannot re-initialize database");
            }

            // Store database credentials

            SwarmDb.Configuration.Set (
                new SwarmDb.Configuration (
                    _testReadCredentials,
                    _testWriteCredentials,
                    _testAdminCredentials));

            // SECURITY: Set machine token crypto keys to randomized values

            if (!Debugger.IsAttached) // (but not while debugging in a non-live environment)
            {
                string machineKeyXml =
                    "<?xml version=\"1.0\" ?>\n" +
                    "<!--\n" +
                    "  The MachineKey is the key used to encrypt/decrypt the cookie containing\n" +
                    "  authentication information. When you have multiple front-end web servers\n" +
                    "  serving a single Swarmops installation, this key must be identical on\n" +
                    "  all front-end machines - otherwise, authentication won't follow from\n" +
                    "  one server to the next.\n\n" +
                    "  This particular MachineKey was randomized at the time of initial\n" +
                    "  installation of this Swarmops database, and is unique. If you have\n" +
                    "  multiple front-ends, you need to copy this file to /etc/swarmops\n" +
                    "  on each of them.\n" +
                    "-->\n" +
                    GetMachineKey() + "\n";

                File.WriteAllText ("/etc/swarmops/machineKey.config", machineKeyXml, Encoding.GetEncoding (1252));
            }

            // Start an async thread that does all the initialization work, then return

            Thread initThread = new Thread (InitDatabaseThread);
            initThread.Start();
        }


        private static string GetMachineKey()
        {
            StringBuilder machineKey = new StringBuilder();
            machineKey.Append ("<machineKey \n");
            machineKey.Append ("validationKey=\"" + SupportFunctions.GenerateSecureRandomKey (64) + "\"\n");
            machineKey.Append ("decryptionKey=\"" + SupportFunctions.GenerateSecureRandomKey (32) + "\"\n");
            machineKey.Append ("validation=\"HMACSHA512\" decryption=\"AES\"\n");
            machineKey.Append ("/>\n");
            return machineKey.ToString();
        }


        /// <summary>
        ///     This function copies the schemas and geography data off an existing Swarmops installation. Runs in its own thread.
        /// </summary>
        public static void InitDatabaseThread()
        {
            // Ignore the session object, that method of sharing data didn't work, but a static variable did.

            _initProgress = 1;
            _initMessage = "Loading schema from Swarmops servers; creating tables and procs...";
            Thread.Sleep (100);

            try
            {
                // Get the schema and initialize the database structures. Requires ADMIN access to database.

                DatabaseMaintenance.FirstInitialization();

                _initProgress = 3;
                _initMessage = "Applying all post-baseline database schema upgrades...";
                DatabaseMaintenance.UpgradeSchemata();
                Thread.Sleep (100);

                // SECURITY: With schemata to hold them in place, initialize the encryption keys

                Authentication.InitializeSymmetricDatabaseKey();
                Authentication.InitializeSymmetricFileSystemKey();

                // Set Geography at baseline (TODO: Ask for what baseline we got)

                Persistence.Key["LastGeographyUpdateId"] = "0";
                Persistence.Key["LastGeographyUpdate"] = Constants.DateTimeLow.ToString("yyyy-MM-dd");

                // Set an installation ID
                // Doubles as start signal to daemons (if installation ID exists, db is ready for processing)

                Persistence.Key["SwarmopsInstallationId"] = Guid.NewGuid().ToString();

                _initProgress = 4;
                _initMessage = "Initializing currencies...";

                // Create initial currencies (European et al)

                Currency.CreateFiat("EUR", "Euros", "€");
                Currency.CreateFiat("USD", "US Dollars", "$");
                Currency.CreateFiat("CAD", "Canadian Dollars", "CA$");
                Currency.CreateFiat("SEK", "Swedish Krona", string.Empty);
                Currency.CreateFiat("NOK", "Norwegian Krona", string.Empty);
                Currency.CreateFiat("DKK", "Danish Krona", string.Empty);
                Currency.CreateFiat("ISK", "Icelandic Krona", string.Empty);
                Currency.CreateFiat("CHF", "Swiss Franc", string.Empty);
                Currency.CreateFiat("GBP", "Pounds Sterling", "£");
                Currency.CreateCrypto("BTC", "Bitcoin Core", "฿");
                Currency.CreateCrypto("BCH", "Bitcoin Cash", "฿");

                // Fetch the first set of exchange rates, completing the currency collection

                ExchangeRateSnapshot.Create();

                // Disable SSL required - the user must turn this on manually

                SystemSettings.RequireSsl = false;

                _initProgress = 5;
                _initMessage = "Getting list of countries from Swarmops servers...";
                Thread.Sleep (100);

                // Create translation lists

                Dictionary<string, int> countryIdTranslation = new Dictionary<string, int>();

                // Initialize the root geography (which becomes #1 if everything works)

                int rootGeographyId = SwarmDb.GetDatabaseForWriting().CreateGeography ("[LOC]World", 0);

                // Create the sandbox

                Organization sandbox = Organization.Create(0, "Sandbox", "Sandbox", "Sandbox", "swarmops.com", "Ops",
                    rootGeographyId, true,
                    true, 0);
                
                sandbox.EnableEconomy(Currency.FromCode("EUR"));

                Positions.CreateOrganizationDefaultPositions(sandbox);

                // Get the list of countries

                GetGeographyData geoDataFetcher = new GetGeographyData();

                Country[] countries = null;

                _initProgress = 7;
                _initMessage = "Creating all countries on local server...";
                Thread.Sleep (100);
                int count = 0;
                int countryRetries = 0;

                try
                {
                    countries = geoDataFetcher.GetCountries();
                }
                catch (Exception)
                {
                    // ignore for now, retrying below                
                }

                while (++countryRetries < 10 && (countries == null || countries.Length < 20))
                {
                    _initMessage = "Network problem, retrying... ";
                    if (countryRetries > 1)
                    {
                        _initMessage += String.Format("({0})", countryRetries);
                    }
                    Thread.Sleep(500);

                    try
                    {
                        countries = geoDataFetcher.GetCountries();
                    }
                    catch (Exception)
                    {
                        if (countryRetries > 8)
                        {
                            throw;
                        }

                        // otherwise ignore for now                    
                    }
                }

                int total = countries.Length;



                // Create all countries in our own database

                foreach (Country country in countries)
                {
                    countryIdTranslation[country.Code] = SwarmDb.GetDatabaseForWriting().CreateCountry (country.Name,
                        country.Code,
                        country.Culture,
                        rootGeographyId, country.PostalCodeLength,
                        string.Empty);

                    count++;
                    _initMessage = String.Format ("Creating all countries on local server... ({0}%)", count*100/total);
                }

                _initProgress = 10;

                // Construct list of countries that have geographic data

                List<string> initializableCountries = new List<string>();

                foreach (Country country in countries)
                {
                    if (country.GeographyId != 1)
                    {
                        initializableCountries.Add (country.Code);
                    }
                }

                float initStepPerCountry = 90f/initializableCountries.Count;
                int countryCount = 0;

                // For each country...

                foreach (string countryCode in initializableCountries)
                {
                    // Get the geography layout

                    _initMessage = "Initializing geography for country " + countryCode + "... ";
                    Thread.Sleep (100);

                    GeographyUpdate.PrimeCountry (countryCode);
                    GuidCache.Set ("DbInitProgress", string.Empty);

                    countryCount++;

                    _initProgress = 10 + (int) (countryCount*initStepPerCountry);
                }

                _initProgress = 100;
                _initMessage = "Complete.";
            }
            catch (Exception failedException)
            {
                // Use initMessage to push info about what went wrong to the user

                _initMessage = failedException.ToString();
            }

            Thread.Sleep (1000);
                // give some time for static var to stick and web interface to react before killing thread
        }


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
            return _initMessage + GuidCache.Get ("DbInitProgress");
        }

        [WebMethod]
        public static bool VerifyHostName (string input)
        {
            if (Debugger.IsAttached)
            {
                return true; // Cannot read "/etc/hostname" in Windows environment
            }

            if (OnLoopbackInterface())
            {
                return true; // Do not validate hostname when localhost
            }


            // validate against /etc/hostname

            string realHostName;

            using (TextReader reader = new StreamReader ("/etc/hostname"))
                // This will throw in a Windows dev environment
            {
                realHostName = reader.ReadLine();
            }

            if (String.Compare (input, realHostName, true, CultureInfo.InvariantCulture) == 0)
                // case-insensitive compare
            {
                return true;
            }

            return false;
        }

        [Serializable]
        public class AjaxCallDaemonResult : AjaxCallResult
        {
            public bool Frontend { get; set; }
            public bool Backend { get; set; }
        }

        [WebMethod]
        public static AjaxCallDaemonResult TestDaemonHeartbeats()
        {
            UInt64 unixNow = DateTime.UtcNow.ToUnix();

            AjaxCallDaemonResult result = new AjaxCallDaemonResult();
            if (SystemSettings.HeartbeatBackend + 60 > unixNow)
            {
                result.Backend = true; // backend heart is beating
            }
            if (SystemSettings.HeartbeatFrontend + 60 > unixNow)
            {
                result.Frontend = true; // frontend heart is beating
            }

            result.Success = true; // call as such succeeded
            return result;
        }

        public static bool OnLoopbackInterface()
        {
            string localAddress = HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];

            if (localAddress == "127.0.0.1" || localAddress == "::1")
            {
                return true;
            }

            return false;
        }

        [WebMethod (true)]
        public static bool VerifyHostAddress (string input)
        {
            if (OnLoopbackInterface())
            {
                return true;
            }

            string localAddress = HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];

            if (input == localAddress)
            {
                return true;
            }

            return false;
        }

        [WebMethod (true)]
        public static bool VerifyHostNameAndAddress (string name, string address)
        {
            if (Path.DirectorySeparatorChar == '\\')
            {
                // Running in development mode on Windows, so accept any response

                return true;
            }

            return VerifyHostName (HttpUtility.UrlDecode (name)) && VerifyHostAddress (HttpUtility.UrlDecode (address));
        }

        [WebMethod (true)]
        public static bool IsConfigurationFileWritable()
        {
            return SwarmDb.Configuration.TestConfigurationWritable();
        }


        private static bool RandomBool()
        {
            return (GetRandomNumber (0, 1) == 0);
        }

        //Function to get random number

        private static int GetRandomNumber (int min, int max)
        {
            lock (syncLock)
            {
                // synchronize
                return GetRandom.Next (min, max + 1);
            }
        }

        [WebMethod (true)]
        public static void CreateFirstUser (string name, string mail, string password)
        {
            // Make sure that no first person exists already, as a security measure

            try
            {
                Person personOne = null;
                bool personOneExists = false;

                try
                {
                    personOne = Person.FromIdentity(1);
                    if (Debugger.IsAttached)
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

                Person newPerson = Person.Create(name, mail, password, string.Empty, string.Empty, string.Empty,
                    string.Empty, string.Empty, new DateTime(1972,1,21), PersonGender.Unknown);

                // Add membership in Sandbox
                newPerson.AddParticipation(Organization.Sandbox, Constants.DateTimeHigh); // expires never

                // Initialize staffing to System and Sandbox with the new user
                Positions.CreateSysadminPositions();

                Positions.ForOrganization(Organization.Sandbox).AtLevel(PositionLevel.OrganizationExecutive)[0].Assign(
                    newPerson, null /* assignedByPerson */, null /* assignedByPosition */, "Initial Sandbox executive",
                    null /* expires */);
            }
            catch (Exception exception)
            {
                try
                {
                    SwarmDb.GetDatabaseForWriting().CreateExceptionLogEntry(DateTime.UtcNow, "Initialization", exception);
                    throw;
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }

        }


        protected void ButtonLogin_Click (object sender, EventArgs args)
        {
            // Check the host names and addresses again as a security measure - after all, we can be called from outside our intended script

            if (!(VerifyHostName (this.TextServerName.Text) && VerifyHostAddress (this.TextServerAddress.Text)))
            {
                if (!Debugger.IsAttached)
                {
                    return; // Probable hack attempt - fail silently
                }
            }

            // Protect against race condition on a really really slow server: wait until there is a first person or 15 seconds have expired

            DateTime utcTimeout = DateTime.UtcNow.AddSeconds (15);
            People people = People.GetAll();
            while (people.Count < 1 && DateTime.UtcNow < utcTimeout)
            {
                Thread.Sleep (500);
                people = People.GetAll();
            }

            if (people.Count < 1)
            {
                throw new InvalidOperationException("First person has not been created despite 15-second timeout; cannot login");
            }

            // Get authenticated person

            Person expectedPersonOne = Authentication.Authenticate ("1",
                this.TextFirstUserPassword1.Text);

            if (expectedPersonOne != null)
            {
                Authority firstAuthority = Authority.FromLogin(expectedPersonOne, Organization.Sandbox);
                FormsAuthentication.RedirectFromLoginPage(firstAuthority.ToEncryptedXml(), true);
                Response.Redirect ("/", true);
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
    }
}