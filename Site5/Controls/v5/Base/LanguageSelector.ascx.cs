using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Controls.Base
{

    public partial class LanguageSelector : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Localize

            this.LabelSelectLanguage.Text = Resources.Global.CurrentUserInfo_SelectLanguage.ToUpperInvariant();
        }

        protected void LinkDanish_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("da-DK");
            FireLanguageChanged();
        }

        protected void LinkGerman_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
            FireLanguageChanged();
        }

        protected void LinkEnglish_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-GB");
            FireLanguageChanged();
        }

        protected void LinkSpanish_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-ES");
            FireLanguageChanged();
        }

        protected void LinkFrench_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fr-FR");
            FireLanguageChanged();
        }

        protected void LinkItalian_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("it-IT");
            FireLanguageChanged();
        }

        protected void LinkDutch_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("nl-NL");
            FireLanguageChanged();
        }

        protected void LinkPolish_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pl-PL");
            FireLanguageChanged();
        }

        protected void LinkPortuguese_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-PT");
            FireLanguageChanged();
        }

        protected void LinkRussian_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            FireLanguageChanged();
        }

        protected void LinkFinnish_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fi-FI");
            FireLanguageChanged();
        }

        protected void LinkSwedish_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            FireLanguageChanged();
        }


        private void FireLanguageChanged()
        {
            HttpCookie cookieCulture = new HttpCookie("PreferredCulture");
            cookieCulture.Value = System.Threading.Thread.CurrentThread.CurrentCulture.Name.ToString();
            cookieCulture.Expires = DateTime.Now.AddDays(365);
            Response.Cookies.Add(cookieCulture);

            if (LanguageChanged != null)
            {
                LanguageChanged(this, new EventArgs());
            }

            // As we are way too late in the page processing when this language change happens, we need to retrigger another postback for it to take effect.
            // There MUST MUST MUST be a better way to do this. Especially as controls on pages need yet a third postback.

            Page.ClientScript.RegisterStartupScript(this.GetType(), "changedLanguagePostback", Page.ClientScript.GetPostBackEventReference(this, string.Empty), true);
        }

        public event EventHandler LanguageChanged;
    }

}

