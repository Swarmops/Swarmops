using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.ServiceModel.Security;
using System.Web;
using System.Web.Services;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.User
{
    public partial class SelectLanguage : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageIcon = "iconshock-language";

            if (!Page.IsPostBack)
            {
                Localize();
            }

            PopulateRepeater();
        }

        private void Localize()
        {
            this.PageTitle = "Select Language";
            this.InfoBoxLiteral = "Select language&nbsp;/ Seleccione su idioma&nbsp;/ Sélectionner votre langue&nbsp;/ Wählen Sie Ihre Sprache&nbsp;/ Välj språk&nbsp;/ Selecione seu idioma&nbsp;/ " +
                "Velg ditt språk&nbsp;/ Selecteer uw taal&nbsp;/ Vælg dit sprog&nbsp;/ Valitse kieli&nbsp;/ επιλέξτε τη γλώσσα σας&nbsp;/ выберите язык";
        }

        private void PopulateRepeater()
        {
            string[] availableCultures = {"en-US", "sv-SE", "nl-NL"};

            List<LanguageParameters> availableLanguages = new List<LanguageParameters>();
            foreach (string cultureId in availableCultures)
            {
                LanguageParameters newLanguage = new LanguageParameters();
                newLanguage.IconUrl = "/Images/Flags/" + cultureId.Substring(3, 2).ToLowerInvariant() + "-64px.png";
                newLanguage.CultureId = cultureId;
                CultureInfo culture = CultureInfo.CreateSpecificCulture(cultureId);
                newLanguage.DisplayName = culture.NativeName;
                newLanguage.DisplayName = Char.ToUpperInvariant(newLanguage.DisplayName[0]) +
                                          newLanguage.DisplayName.Substring(1); // Capitalize

                if (cultureId.StartsWith("en"))
                {
                    newLanguage.IconUrl = "/Images/Flags/uk-64px.png"; // use "uk" for en-GB and en-US rather than "gb" or "us"
                }
                availableLanguages.Add(newLanguage);
            }

            LanguageParameters localizeParams = new LanguageParameters();
            localizeParams.IconUrl = "/Images/Flags/txl-64px.png";
            localizeParams.CultureId = "af-ZA";
            localizeParams.DisplayName = "Translate Swarmops into Your Language";
            availableLanguages.Add(localizeParams);

            this.RepeaterLanguages.DataSource = availableLanguages;
            this.RepeaterLanguages.DataBind();
        }

        private class LanguageParameters
        {
            public string IconUrl { get; set; }
            public string CultureId { get; set; }
            public string DisplayName { get; set; }
        }
    }
}