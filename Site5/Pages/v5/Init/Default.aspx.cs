using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

using Activizr.Database;

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

        // Initialize database. For now, fake it.

        for (int loop = 0; loop < 100; loop++)
        {
            Thread.Sleep(1000);
            Session["PercentInitComplete"] = loop;
        }
    }

    [WebMethod(true)]  // "true" causes session to be loaded
    public static int GetInitProgress()
    {
        try
        {
            return (int) HttpContext.Current.Session["PercentInitComplete"];
        }
        catch (Exception)
        {
            return 0; // session variable not set yet, so parsing error of a null object
        }

    }

    [WebMethod]
    public static bool VerifyHostName(string input)
    {
        // validate against /etc/hostname
        
        if (input == "/etc/hostname")
        {
            return true;
        }

        return false;
    }

    [WebMethod]
    public static bool VerifyHostAddress (string input)
    {
        if (input == "127.0.0.1")
        {
            return true;
        }

        return false;
    }

    [WebMethod]
    public static bool VerifyHostNameAndAddress (string name, string address)
    {
        return VerifyHostName(name) && VerifyHostAddress(address);
    }
}