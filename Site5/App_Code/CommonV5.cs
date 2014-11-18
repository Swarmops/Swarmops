using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Web;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

/// <summary>
/// Summary description for CommonV5
/// </summary>
public class CommonV5
{
    public CommonV5()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static void CulturePreInit(HttpRequest request)
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

        CommonV5.CulturePreInit(HttpContext.Current.Request);
        // OnPreInit() isn't called in the static methods calling this fn

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




/* COLOR SCHEME

       #A2BEFF #476EC7 #1C397E #C8D9FF #E4ECFF    -- blue (hue 240)
       #FFBC37 #C78B15 #7F5500 #FFD580 #FFEDC8    -- orange (hue 60)
  
        base    dark    xdark   light   xlight

       S36L100 S64-L78 S78-L49 S22B100 S11L100    -- blue (hue 222)
       S78L100 S89-L78 S100L50 S50L100 S22L100    -- orange (hue 40)
    */




    public static string GetColor(ColorType type, ColorVariant variant, ColorShift shift = ColorShift.None)
    {
        int hue = (type == ColorType.Base ? 222 : 40);
        int saturation = 0;
        int luminosity = 0;

        switch (variant)
        {
            case ColorVariant.Base:
                saturation = 100;
                luminosity = 80;
                break;
            case ColorVariant.Dark:
                saturation = 50;
                luminosity = 50;
                break;
            case ColorVariant.XDark:
                saturation = 65;
                luminosity = 30;
                break;
            case ColorVariant.Light:
                saturation = 100;
                luminosity = 90;
                break;
            case ColorVariant.XLight:
                saturation = 100;
                luminosity = 95;
                break;
            default:
                throw new NotImplementedException();

        }

        if (type == ColorType.Accent)
        {
            saturation *= 2;
            if (saturation > 100)
            {
                saturation = 100;
            }
        }

        switch (shift)
        {
            case ColorShift.SlightlyDarker:
                luminosity -= 2;
                break;
            case ColorShift.SlightlyLighter:
                luminosity += 2;
                break;
            default:
                // do nothing
                break;
        }

        Color color = ColorFromAhsb(100, hue, saturation / 100.0, luminosity / 100.0);
        return String.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
    }


    public static Color ColorFromAhsb(int a, double h, double s, double b)
    {
        if (s < 0.001)
        {
            return Color.FromArgb(a, Convert.ToInt32(b * 255),
                                  Convert.ToInt32(b * 255), Convert.ToInt32(b * 255));
        }

        double fMax, fMid, fMin;
        int iSextant, iMax, iMid, iMin;

        if (0.5 < b)
        {
            fMax = b - (b * s) + s;
            fMin = b + (b * s) - s;
        }
        else
        {
            fMax = b + (b * s);
            fMin = b - (b * s);
        }

        iSextant = (int)Math.Floor(h / 60f);
        if (300f <= h)
        {
            h -= 360f;
        }
        h /= 60f;
        h -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
        if (0 == iSextant % 2)
        {
            fMid = h * (fMax - fMin) + fMin;
        }
        else
        {
            fMid = fMin - h * (fMax - fMin);
        }

        iMax = Convert.ToInt32(fMax * 255);
        iMid = Convert.ToInt32(fMid * 255);
        iMin = Convert.ToInt32(fMin * 255);

        switch (iSextant)
        {
            case 1:
                return Color.FromArgb(a, iMid, iMax, iMin);
            case 2:
                return Color.FromArgb(a, iMin, iMax, iMid);
            case 3:
                return Color.FromArgb(a, iMin, iMid, iMax);
            case 4:
                return Color.FromArgb(a, iMid, iMin, iMax);
            case 5:
                return Color.FromArgb(a, iMax, iMin, iMid);
            default:
                return Color.FromArgb(a, iMax, iMid, iMin);
        }
    }





    protected static string HslToWebColor(int hue, int saturation, int luminosity) // 0-359, 0-100, 0-100
    {
        // This code is adapted from Wikipedia. Believed to be in the public domain or at the very least
        // completely unencumbered.

        double h = hue/360.0;
        double s = saturation/100.0;
        double l = luminosity/100.0;

        double r, g, b;

        if (s < 0.0001) // zero
        {
            r = g = b = l; // achromatic
        }
        else
        {
            double q = l < 0.5 ? l*(1 + s) : l + s - l*s;
            double p = 2*l - q;
            r = Hue2Rgb(p, q, h + 1/3);
            g = Hue2Rgb(p, q, h);
            b = Hue2Rgb(p, q, h - 1/3);
        }

        return String.Format("#{0:x2}{1:x2}{2:x2}", (int) (r*255), (int) (g*255), (int) (b*255));

    }


    private static double Hue2Rgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1/6) return p + (q - p)*6*t;
        if (t < 1/2) return q;
        if (t < 2/3) return p + (q - p)*(2/3 - t)*6;
        return p;
    }
}



public enum ColorType
    {
        Unkown = 0,
        Base,
        Accent
    }

    public enum ColorVariant
    {
        Unknown = 0,
        Base,
        Light,
        XLight,
        Dark,
        XDark
    }

    public enum ColorShift
    {
        Unknown = 0,
        None,
        SlightlyLighter,
        SlightlyDarker
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


[Flags]
public enum IncludedControl
{
    Unknown = 0,
    FileUpload     = 0x00000001,
    SwitchButton   = 0x00000002,
    JsonParameters = 0x00000004
};