using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Services;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Automation
{
    public partial class FieldValidation : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
        }

        [WebMethod]
        public static bool IsDateValid (string input)
        {
            AuthenticationData authenticationData = GetAuthenticationDataAndCulture(); // sets culture

            DateTime dummyResult;
            return DateTime.TryParse (input, out dummyResult);
        }

        [WebMethod]
        public static void TestFoo (string input)
        {
            AuthenticationData authenticationData = GetAuthenticationDataAndCulture();

            System.Diagnostics.Trace.WriteLine (input);
        }


        [WebMethod]
        public static bool IsAmountValid(string amount)
        {
            AuthenticationData authData = GetAuthenticationDataAndCulture(); // sets culture

            // Try to parse the string we got as a double

            try
            {
                Double.Parse(amount, NumberStyles.Number);
                return true;
            }
            catch (Exception)
            {
            }

            return false;
        }

        [WebMethod]
        public static bool AreDocumentsUploaded(string guidString)
        {
            Documents documents = Documents.RecentFromDescription(guidString);

            if (documents.Count == 0)
            {
                return false;
            }

            return true;
        }




        [WebMethod]
        public static PostalCodesCities GetPostalCodesCities (string countryCode)
        {
            List<PostalCodeData> postalCodeArray = new List<PostalCodeData>();
            List<CityData> cityArray = new List<CityData>();
            List<GeographyData> geographyArray = new List<GeographyData>();
            PostalCodes codes = PostalCodes.ForCountry (countryCode);
            Country country = Country.FromCode (countryCode);
            int maxPostalCodeLength = 0;

            foreach (PostalCode code in codes)
            {
                PostalCodeData dataPoint = new PostalCodeData
                {
                    Code = code.PostalCode,
                    CityId = code.CityId
                };

                if (code.PostalCode.Length > maxPostalCodeLength)
                {
                    maxPostalCodeLength = code.PostalCode.Length;
                }

                postalCodeArray.Add (dataPoint);
            }

            Cities cities = Cities.ForCountry (countryCode);
            foreach (City city in cities)
            {
                CityData dataPoint = new CityData
                {
                    Id = city.Identity,
                    Name = city.Name,
                    GeographyId = city.GeographyId
                };

                cityArray.Add (dataPoint);
            }

            Geographies geographies = Country.FromCode (countryCode).Geography.ThisAndBelow();
            foreach (Geography geography in geographies)
            {
                GeographyData dataPoint = new GeographyData
                {
                    Id = geography.Identity,
                    Name = geography.Name
                };

                geographyArray.Add (dataPoint);
            }

            PostalCodesCities result = new PostalCodesCities
            {
                PostalCodes = postalCodeArray.ToArray(),
                CityNames = cityArray.ToArray(),
                Geographies = geographyArray.ToArray(),
                PostalCodeLength = country.PostalCodeLength,
                PostalCodeLengthCheck = maxPostalCodeLength
            };

            return result;
        }

        [WebMethod]
        public static int GetPostalCodeLength (string countryCode)
        {
            Country country = Country.FromCode (countryCode);
            return country.PostalCodeLength;
        }
    }
}

[Serializable]
public class PostalCodeData
{
    public string Code { get; set; }
    public int CityId { get; set; }
}

public class CityData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int GeographyId { get; set; }
}

public class GeographyData
{
    public int Id { get; set; }
    public string Name { get; set; }
}

[Serializable]
public class PostalCodesCities
{
    public int PostalCodeLength;
    public int PostalCodeLengthCheck;
    public PostalCodeData[] PostalCodes { get; set; }
    public CityData[] CityNames { get; set; }
    public GeographyData[] Geographies { get; set; }
}