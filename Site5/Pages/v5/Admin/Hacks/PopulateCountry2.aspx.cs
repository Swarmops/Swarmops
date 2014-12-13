using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web.Services;
using Swarmops.Basic.Enums;
using Swarmops.Database;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Admin.Hacks
{
    public partial class PopulateCountry2 : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            PageTitle = "Populate Country 2 - Postal Codes, Cities";
            PageIcon = "iconshock-battery-drill";
            InfoBoxLiteral = "You've taken a wrong turn if you're even seeing this page. It's for development purposes.";
        }


        private void Localize()
        {
        }


        [WebMethod (true)]
        public static void InitializeProcessing (string guid)
        {
            // Start an async thread that does all the work, then return


            // FORMAT OF FILE
            //
            // Tab separated fields
            //
            // [countrycode] [tab] [postalCode] [tab] [cityName] [tab] [geoNodeName]
            //
            // example:
            // 
            // NL [Tab] 1026 [Tab] Amsterdam-Zuid [Tab] Amsterdam
            //
            // For countries that don't use postal codes, leave the postal code field empty.
            //
            // Node names within a country scope are required to be unique.


            if (!PilotInstallationIds.IsPilot(PilotInstallationIds.SwarmopsLive) && !Debugger.IsAttached)
            {
                throw new UnauthorizedAccessException("This may only run on Swarmops Master");
            }



            AuthenticationData authData = GetAuthenticationDataAndCulture();

            if (
                !authData.CurrentUser.HasAccess (new Access (authData.CurrentOrganization, AccessAspect.Unknown,
                    AccessType.Write)))
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

            using (StreamReader reader = uploadedDoc.GetReader (1252))
            {
                data = reader.ReadToEnd();
            }

            string[] lines = data.Split ('\n');

            Random random = new Random();

            Dictionary<string,bool> postalCodeDupes = new Dictionary<string, bool>();

            foreach (string lineRaw in lines)
            {
                count++;
                string line = lineRaw.Trim();
                string[] lineParts = line.Split ('\t');

                if (lineParts.Length != 4)
                {
                    continue;
                }

                int percent = (count * 99) / lines.Length;
                if (percent == 0)
                {
                    percent = 1;
                }
                GuidCache.Set(guid + "-Progress", percent);


                string countryCode = lineParts[0].Trim().ToUpperInvariant();
                string postalCode = lineParts[1].Trim();
                string cityName = lineParts[2].Trim();
                string nodeName = lineParts[3].Trim();

                // Dupecheck

                if (postalCodeDupes.ContainsKey (postalCode))
                {
                    continue;
                }

                Geography geography = Geography.FromName (nodeName); // may dupe!

                // First, make sure country exists

                Country country = Country.FromCode (countryCode);

                // Then, check if the city name exists

                City city = null;

                try
                {
                    city = City.FromName (cityName, country.Identity);
                }
                catch (ArgumentException)
                {
                    city = City.Create (cityName, country.Identity, geography.Identity);
                }

                // Last, add the postal code, if there is any

                if (!string.IsNullOrEmpty (postalCode))
                {
                    SwarmDb.GetDatabaseForWriting().CreatePostalCode (postalCode, city.Identity, country.Identity);
                    postalCodeDupes[postalCode] = true;
                }
            }

            GuidCache.Set (guid + "-Progress", 100);
        }
    }
}