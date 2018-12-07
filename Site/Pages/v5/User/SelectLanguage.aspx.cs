using System;
using System.Collections.Generic;
using System.Globalization;
using Swarmops.Logic.Security;

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
            string[] availableCultures = {"ar-AE", "de-DE", "el", "es-ES", "fr-FR", "fil-Latn", "it-IT", "nl-NL", "pl-PL", "pt-PT", "ru-RU", "tr-TR", "sv-SE", "yo-Latn", "zh-CHS" };

            Dictionary<string,string> nonStandardFlagNames = new Dictionary<string, string>();
            nonStandardFlagNames["en"] = "uk";      // Use UK flag for US English
            nonStandardFlagNames["ar"] = "Arabic";  // Arabic doesn't have a country flag per se
            nonStandardFlagNames["fil"] = "ph";     // Philippines / Filipino
            nonStandardFlagNames["el"] = "gr";      // Greece / Ελληνικά
            nonStandardFlagNames["yo"] = "ng";      // Nigeria / Yoruba
            nonStandardFlagNames["zh"] = "ch";      // China / Chinese


            Array.Sort (availableCultures);
            // sort by locale string, and that's ok, that happens to give the same result as sorting on country name

            List<LanguageParameters> availableLanguages = new List<LanguageParameters>();
            foreach (string cultureId in availableCultures)
            {
                LanguageParameters newLanguage = new LanguageParameters();
                newLanguage.CultureId = cultureId;
                CultureInfo culture = CultureInfo.CreateSpecificCulture (cultureId);
                newLanguage.DisplayName = culture.NativeName;
                newLanguage.DisplayName = Char.ToUpperInvariant (newLanguage.DisplayName[0]) +
                                          newLanguage.DisplayName.Substring (1); // Capitalize

                // Do not display country, just the language name
                newLanguage.DisplayName = newLanguage.DisplayName.Split (' ')[0];

                string cultureFirstPart = cultureId.Split('-')[0];

                if (nonStandardFlagNames.ContainsKey(cultureFirstPart)
                {
                    
                }

                if (cultureId.StartsWith("en"))
                {
                    newLanguage.IconUrl = "/Images/Flags/uk-64px.png";
                    // use "uk" for en-GB and en-US rather than "gb" or "us"
                }
                else if (cultureId.StartsWith ("ar"))
                {
                    newLanguage.IconUrl = "/Images/Flags/Arabic-64px.png";
                    // crowdin's generic culture is ar-SA but Arabic is not country specific
                }
                else if (cultureId.StartsWith("fil"))
                {
                    // Filipino flag can't be deduced from the culture string
                    newLanguage.IconUrl = "/Images/Flags/ph-64px.png";
                }
                else if (cultureId.StartsWith("el"))
                {
                    // Nor the Greek flag
                    newLanguage.IconUrl = "/Images/Flags/gr-64px.png";
                }
                else if (cultureId.StartsWith("zh"))
                {
                    // Nor Chinese
                    newLanguage.IconUrl = "/Images/Flags/cn-64px.png";
                }
                else
                {
                    newLanguage.IconUrl = "/Images/Flags/" + cultureId.Substring(3, 2).ToLowerInvariant() + "-64px.png";
                }
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