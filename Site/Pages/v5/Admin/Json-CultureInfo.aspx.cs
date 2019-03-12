using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Swarmops.Logic.Support;
using WebSocketSharp.Server;

namespace Swarmops.Frontend.Pages.v5.Admin
{
    public partial class Json_CultureInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentType = "application/json";
            string json = AllCulturesAsJson();
            Response.Output.WriteLine(json);
            Response.End();
        }

        private static string AllCulturesAsJson()
        {
            StringBuilder result = new StringBuilder(16384);
            Dictionary<string, bool> cultureFullLookup = new Dictionary<string, bool>();
            Dictionary<string, bool> culturePartialLookup = new Dictionary<string, bool>();

            string[] supportedCultures = Swarmops.Logic.Support.Formatting.SupportedCultures;
            foreach (string culture in supportedCultures)
            {
                cultureFullLookup[culture] = true;
                culturePartialLookup[culture.Substring(0, culture.IndexOf('-'))] = true;
            }

            string yesImage = "<img src='/Images/Icons/iconshock-green-tick-128x96px.png' height='20' width='26' />";
            string halfImage = "<img src='/Images/Icons/iconshock-gold-tick-128x96px.png' height='18' width='24' />";
            string noImage = "<img src='/Images/Icons/iconshock-red-cross-128x96px.png' height='10' width='13' />";

            result.Append("{\"rows\":[");

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

            foreach (CultureInfo culture in cultures)
            {
                // Don't display af-ZA because we're using af-ZA as a pseudoloc culture

                if (culture.Name == "af-ZA")
                {
                    continue;
                }

                RegionInfo region = null;

                try
                {
                    region = new RegionInfo(culture.Name);

                    string flagFile = SupportFunctions.FlagFileFromCultureId(culture.Name);

                    if (!File.Exists(HttpContext.Current.Server.MapPath("~" + flagFile)))
                    {
                        flagFile = string.Empty;
                    }
                    else
                    {
                        flagFile = "<img src='" + flagFile + "' height='24' width='24' />";
                    }

                    result.Append("{");
                    result.AppendFormat(
                        "\"cultureId\":\"{0}\",\"name\":\"{1}\",\"nameInternational\":\"{2}\",\"country\":\"{3}\",\"flag\":\"{4}\",\"supported\":\"{5}\"",
                        culture.Name,
                        culture.NativeName,
                        culture.EnglishName,
                        region.EnglishName,
                        flagFile.Length > 2? flagFile : noImage,
                        cultureFullLookup.ContainsKey(culture.Name)? yesImage: culturePartialLookup.ContainsKey(culture.Name.Substring(0, culture.Name.IndexOf('-')))? halfImage: noImage
                    );

                    result.Append("},");

                }
                catch
                {
                    continue;
                }

            }

            result.Remove(result.Length - 1, 1); // remove last comma
            result.Append("]}");

            return result.ToString();
        }
    }
}