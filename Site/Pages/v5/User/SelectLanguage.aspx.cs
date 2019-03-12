using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
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
            string[] availableCultures = Formatting.SupportedCultures;

            Dictionary<string, string> specialNameLookup = new Dictionary<string, string>();
            Dictionary<string, bool> suppressLookup = new Dictionary<string, bool>();

            specialNameLookup["sr-Cyrl-RS"] = "Српски (ћирилица)";
            specialNameLookup["sr-Latn-RS"] = "Srpski (latinica)";
            specialNameLookup["es-VE"] = "Español (Venezuela)";

            suppressLookup["en-US"] = true;
            suppressLookup["zh-CN"] = true;
            // suppressLookup["ar-SA"] = true;

            List<LanguageParameters> availableLanguages = new List<LanguageParameters>();
            foreach (string cultureId in availableCultures)
            {
                if (suppressLookup.ContainsKey(cultureId))
                {
                    continue;
                }

                LanguageParameters newLanguage = new LanguageParameters();
                newLanguage.CultureId = cultureId;
                CultureInfo culture = CultureInfo.CreateSpecificCulture (cultureId);

                if (specialNameLookup.ContainsKey(cultureId))
                {
                    newLanguage.DisplayName = specialNameLookup[cultureId];
                }
                else
                {
                    newLanguage.DisplayName = culture.NativeName;
                    newLanguage.DisplayName = Char.ToUpperInvariant(newLanguage.DisplayName[0]) +
                                              newLanguage.DisplayName.Substring(1); // Capitalize

                    // Do not display country, just the language name
                    newLanguage.DisplayName = newLanguage.DisplayName.Substring(0, newLanguage.DisplayName.IndexOf(" (")).Trim();
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