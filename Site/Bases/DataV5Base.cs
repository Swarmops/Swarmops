using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Swarmops.Common.Enums;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend
{
    /// <summary>
    ///     Summary description for PageV5Base
    ///     Base class to use for all pages that uses the Swarmops-v5 master page
    /// </summary>
    public class DataV5Base : Page
    {
        public Access PageAccessRequired = null; // v5 mechanism

        protected Person CurrentUser
        {
            get
            {
                if (CurrentAuthority != null)
                {
                    return CurrentAuthority.Person;
                }

                return null;
            }
        }

        protected Organization CurrentOrganization
        {
            get
            {
                if (CurrentAuthority != null)
                {
                    return CurrentAuthority.Organization;
                }

                return null;
            }
        }

        protected Authority CurrentAuthority { get; private set; }

        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnInitComplete (EventArgs e)
        {
            base.OnInitComplete (e);

            string identity = HttpContext.Current.User.Identity.Name;

            if (!string.IsNullOrEmpty (identity))
            {
                try
                {
                    CurrentAuthority = CommonV5.GetAuthenticationDataAndCulture (HttpContext.Current).Authority;
                }
                catch (Exception)
                {
                    // if this fails FOR WHATEVER REASON then we're not authenticated
                    this.CurrentAuthority = null;
                    FormsAuthentication.SignOut();
                }
            }
            else
            {
                CurrentAuthority = null; // unauthenticated!
            }

            // Regardless of auth data and culture, set Gregorian calendar
            // (avoids problems with Arabic, etc, calendars and bookkeeping in localization)

            GregorianCalendar normalizedCalendar = new GregorianCalendar();
            normalizedCalendar.CalendarType = GregorianCalendarTypes.USEnglish;

            // Set the calendar to GregorianCalendar.USEnglish. This sometimes fails on Windows but works
            // on Mono. If it fails on Windows, no biggie b/c this is supposed to be the default anyway.
            try
            {
                Thread.CurrentThread.CurrentCulture.DateTimeFormat.Calendar = normalizedCalendar;
                Thread.CurrentThread.CurrentUICulture.DateTimeFormat.Calendar = normalizedCalendar;
            }
            catch (Exception)
            {
                // meh
            }
        }


        protected override void OnPreInit (EventArgs e)
        {
            CommonV5.CulturePreInit (Request);

            base.OnPreInit (e);
        }

        protected override void OnPreRender (EventArgs e)
        {
            // Check security of page against users's credentials

            if (this.PageAccessRequired != null)
            {
                if (!this.CurrentAuthority.HasAccess (this.PageAccessRequired))
                {
                    Response.Redirect ("/Pages/v5/Security/AccessDenied.aspx");
                }
            }

            base.OnPreRender (e);
        }

        protected string LocalizeCount (string resourceString, int count)
        {
            return LocalizeCount (resourceString, count, false);
        }

        protected string LocalizeCount (string resourceString, int count, bool capitalize)
        {
            string result;
            string[] parts = resourceString.Split ('|');

            switch (count)
            {
                case 0:
                    result = parts[0];
                    break;
                case 1:
                    result = parts[1];
                    break;
                default:
                    result = String.Format (parts[2], count);
                    break;
            }

            if (capitalize)
            {
                result = Char.ToUpperInvariant (result[0]) + result.Substring (1);
            }

            return result;
        }


        protected string TryLocalize (string input)
        {
            if (!input.StartsWith ("[Loc]"))
            {
                return input;
            }

            string[] inputParts = input.Split ('|');

            string resourceKey = inputParts[0].Substring (5);
            object translatedResource = GetGlobalResourceObject ("Global", resourceKey);

            if (translatedResource == null)
            {
                throw new NotImplementedException ("Unimplemented localization resource key: \"" + resourceKey + "\"");
            }

            if (inputParts.Length == 1)
            {
                return translatedResource.ToString();
            }
            object argument = null;

            if (inputParts[1].StartsWith ("[Date]"))
            {
                argument = DateTime.Parse (inputParts[1].Substring (6), CultureInfo.InvariantCulture);
            }
            else
            {
                argument = inputParts[1];
            }

            return String.Format (translatedResource.ToString(), argument);
        }


        protected static AuthenticationData GetAuthenticationDataAndCulture()
        {
            return CommonV5.GetAuthenticationDataAndCulture (HttpContext.Current);
        }

        protected static string JsonSanitize (string input)
        {
            return input.Replace ("\"", "\\\"").Replace ("  ", " ").Trim();
        }

        public static string JavascriptEscape (string input)
        {
            return CommonV5.JavascriptEscape (input);
        }

        public string Localize_GenericAjaxError
        {
            get { return JavascriptEscape(Resources.Global.Error_AjaxCallException); }
        }
    }
}