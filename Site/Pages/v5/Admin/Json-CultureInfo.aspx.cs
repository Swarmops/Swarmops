using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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

            result.Append("{\"rows\":[");

            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);

            foreach (CultureInfo culture in cultures)
            {
                RegionInfo region = null;

                try
                {
                    region = new RegionInfo(culture.Name);

                    result.Append("{");
                    result.AppendFormat(
                        "\"cultureId\":\"{0}\",\"name\":\"{1}\",\"nameInternational\":\"{2}\",\"language\":\"{3}\",\"country\":\"{4}\",\"flag\":\"{5}\",\"supported\":\"{6}\"",
                        culture.Name,
                        culture.NativeName,
                        culture.EnglishName,
                        "Language",
                        region.EnglishName,
                        "Fl",
                        "Sp"
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