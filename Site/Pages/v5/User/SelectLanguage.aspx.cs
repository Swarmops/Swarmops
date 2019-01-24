using System;
using System.Collections.Generic;
using System.Globalization;
using Swarmops.Logic.Security;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Pages.v5.User
{
    public partial class SelectLanguage : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            PageIcon = "iconshock-language";

            if (!Page.IsPostBack)
            {
                Localize();
            }

            PageAccessRequired = new Access (AccessAspect.Null, AccessType.Unknown);

            PopulateRepeater();
        }

        private void Localize()
        {
            PageTitle = "Select Language";
            InfoBoxLiteral =
                "Select language&nbsp;/ Seleccione su idioma&nbsp;/ Sélectionner votre langue&nbsp;/ Wählen Sie Ihre Sprache&nbsp;/ Välj språk&nbsp;/ Selecione seu idioma&nbsp;/ " +
                "Velg ditt språk&nbsp;/ Selecteer uw taal&nbsp;/ Vælg dit sprog&nbsp;/ Valitse kieli&nbsp;/ επιλέξτε τη γλώσσα σας&nbsp;/ выберите язык";
        }

        private void PopulateRepeater()
        {
            string[] availableCultures = {"ar-AE", "de-DE", "yo-Latn" /* Yoruba writes as Èbè */, "el" /* Greek writes as Ellenika */ , "es-ES", "fr-FR", "fil-Latn", "it-IT", "nl-NL", "pl-PL", "pt-PT", "ru-RU", "sr-Cyrl-RS", "sr-Latn-RS", "sv-SE", "tr-TR", "zh-CHS" };

            // the above locales are sorted by the language NATIVE name, to make the list maximally useful

            List<LanguageParameters> availableLanguages = new List<LanguageParameters>();
            foreach (string cultureId in availableCultures)
            {
                LanguageParameters newLanguage = new LanguageParameters();
                newLanguage.CultureId = cultureId;
                CultureInfo culture = CultureInfo.CreateSpecificCulture (cultureId);

                if (cultureId == "sr-Latn-RS") // special case: Mono wants to write this in Cyrillic
                {
                    newLanguage.DisplayName = "Srpska (Latin)";
                }
                else
                {
                    newLanguage.DisplayName = culture.NativeName;
                    newLanguage.DisplayName = Char.ToUpperInvariant(newLanguage.DisplayName[0]) +
                                              newLanguage.DisplayName.Substring(1); // Capitalize

                    // Do not display country, just the language name
                    newLanguage.DisplayName = newLanguage.DisplayName.Split(' ')[0];
                }

                newLanguage.IconUrl = SupportFunctions.FlagFileFromCultureId(cultureId);

                if (culture.TextInfo.IsRightToLeft)
                {
                    newLanguage.Rtl = "rtl";
                }

                availableLanguages.Add (newLanguage);
            }

            this.RepeaterLanguages.DataSource = availableLanguages;
            this.RepeaterLanguages.DataBind();
        }

        private class LanguageParameters
        {
            public string Rtl { get; set; }
            public string IconUrl { get; set; }
            public string CultureId { get; set; }
            public string DisplayName { get; set; }
        }
    }
}