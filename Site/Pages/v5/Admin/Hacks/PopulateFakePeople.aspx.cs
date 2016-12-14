using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Services;
using Swarmops.Common.Enums;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Admin.Hacks
{
    public partial class PopulateFakePeople : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            PageTitle = "Populate Data - Fake People";
            PageIcon = "iconshock-battery-drill";
            InfoBoxLiteral = "You've taken a wrong turn if you're even seeing this page. It's for development purposes.";

            PageAccessRequired = new Access(AccessAspect.Administration);
        }


        private void Localize()
        {
        }


        [WebMethod (true)]
        public static void InitializeProcessing (string guid)
        {
            // Start an async thread that does all the work, then return

            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.Authority.HasAccess (new Access (AccessAspect.Administration)))
            {
                throw new UnauthorizedAccessException();
            }

            Thread initThread = new Thread (ProcessUploadThread);
            initThread.Start (guid);
        }

        private static void ProcessUploadThread (object guidObject)
        {
            string guid = (string) guidObject;
            Documents documents = Documents.RecentFromDescription (guid);
            Document uploadedDoc = documents[0];
            string data = string.Empty;
            int count = 0;

            using (StreamReader reader = uploadedDoc.GetReader (Encoding.UTF8))
            {
                data = reader.ReadToEnd();
            }

            string[] lines = data.Split ('\n');

            Random random = new Random();

            // Fields:
            // 0  Firstname
            // 1  Lastname
            // 2  Gender
            // 3  Street
            // 4  Postal
            // 5  City
            // 6  CountryCode
            // 7  DOB
            // 8  Phone
            // 9  NationalID
            // 10 Longitude
            // 11 Latitude


            // below is an N^2 loop but doesn't matter in such a small context

            Dictionary<FakePersonFields,int> fieldNameLookup = new Dictionary<FakePersonFields, int>();
            string[] headerParts = lines[0].Split('\t');

            foreach (string fieldName in Enum.GetValues(typeof(FakePersonFields)))
            {
                bool found = false;
                for (int index = 0; index < headerParts.Length; index++)
                {
                    if (headerParts[index].Trim() == fieldName)
                    {
                        fieldNameLookup[(FakePersonFields) Enum.Parse(typeof(FakePersonFields), fieldName)] = index;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new InvalidOperationException("Field key \"" + fieldName +
                                                         "\" was not supplied or found in data file");
                }
            }

            foreach (string lineRaw in lines)
            {
                count++;
                string line = lineRaw.Trim();
                string[] lineParts = line.Split ('\t');

                if (lineParts.Length < 12)
                {
                    continue;
                }

                if (count == 1)
                {
                    // header line
                    continue;
                }

                int percent = (count*99)/lines.Length;
                if (percent == 0)
                {
                    percent = 1;
                }
                GuidCache.Set (guid + "-Progress", percent);

                string name = lineParts[fieldNameLookup[FakePersonFields.GivenName]] + " " + lineParts[fieldNameLookup[FakePersonFields.Surname]];
                PersonGender gender = lineParts[fieldNameLookup[FakePersonFields.Gender]] == "male" ? PersonGender.Male : PersonGender.Female;
                DateTime dateOfBirth = DateTime.Parse (lineParts[fieldNameLookup[FakePersonFields.Birthday]], new CultureInfo ("en-US"), DateTimeStyles.None);
                Country country = Country.FromCode (lineParts[fieldNameLookup[FakePersonFields.Country]]);


                Person newPerson = Person.Create (name, string.Empty, string.Empty, lineParts[fieldNameLookup[FakePersonFields.TelephoneNumber]], lineParts[fieldNameLookup[FakePersonFields.StreetAddress]],
                    lineParts[fieldNameLookup[FakePersonFields.ZipCode]].Replace (" ", ""), lineParts[fieldNameLookup[FakePersonFields.City]], lineParts[fieldNameLookup[FakePersonFields.Country]], dateOfBirth, gender);

                newPerson.NationalIdNumber = lineParts[fieldNameLookup[FakePersonFields.NationalID]];
                newPerson.Longitude = lineParts[fieldNameLookup[FakePersonFields.Longitude]];
                newPerson.Latitude = lineParts[fieldNameLookup[FakePersonFields.Latitude]];

                newPerson.AddParticipation (Organization.Sandbox, DateTime.Today.AddDays (3650 + random.Next (365)));
            }

            GuidCache.Set (guid + "-Progress", 100);
        }

        // ReSharper disable once InconsistentNaming

        public enum FakePersonFields
        {
            GivenName,
            Surname,
            Gender,
            StreetAddress,
            ZipCode,
            City,
            Country,
            Birthday,
            TelephoneNumber,
            NationalID,
            Longitude,
            Latitude
        }
    }
}