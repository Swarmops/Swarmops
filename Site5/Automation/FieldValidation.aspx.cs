using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Structure;

namespace Swarmops.Frontend.Automation
{
    public partial class FieldValidation : DataV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static void TestFoo (string input)
        {
            AuthenticationData authenticationData = GetAuthenticationDataAndCulture();

            System.Diagnostics.Trace.WriteLine(input);
        }

        [WebMethod]
        public static PostalCodeData[] GetPostalCodes(string countryCode)
        {
            List<PostalCodeData> result = new List<PostalCodeData>();
            PostalCodes codes = PostalCodes.ForCountry(countryCode);

            foreach (PostalCode code in codes)
            {
                PostalCodeData dataPoint = new PostalCodeData
                {
                    Code = code.PostalCode, 
                    Name = code.CityName
                };

                result.Add(dataPoint);
            }

            return result.ToArray();
        }

        [WebMethod]
        public static int GetPostalCodeLength(string countryCode)
        {
            Country country = Country.FromCode(countryCode);
            return country.PostalCodeLength;
        }
    }
}


[Serializable]
public class PostalCodeData
{
    public string Code { get; set; }
    public string Name { get; set; }
}