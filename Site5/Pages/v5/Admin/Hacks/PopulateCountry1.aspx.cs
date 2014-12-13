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
    public partial class PopulateCountry1 : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            PageTitle = "Populate Country 1 - Geotree";
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
            // [countrycode] [tab] [geo1] .. [geo_n]
            //
            // example:
            // 
            // NL [Tab] Greater Amsterdam
            // NL [Tab] Greater Amsterdam [Tab] Amsterdam
            // NL [Tab] Greater Amsterdam [Tab] Amsterdam [Tab] Wallen
            //
            // To populate all four nodes, only the last line is necessary, but all three lines are valid.
            //
            // Nodes are not re-populated on repetition. Node names within a country scope are required to
            // be unique.


            if (!PilotInstallationIds.IsPilot (PilotInstallationIds.SwarmopsLive) && !Debugger.IsAttached)
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

            Dictionary<string, int> geoNameLookup = new Dictionary<string, int>();

            foreach (string lineRaw in lines)
            {
                count++;
                string line = lineRaw.Trim();
                string[] lineParts = line.Split ('\t');

                if (lineParts.Length < 2)
                {
                    continue;  // at least country and one geo required
                }

                // set progress

                int percent = (count*99)/lines.Length;
                if (percent == 0)
                {
                    percent = 1;
                }
                GuidCache.Set (guid + "-Progress", percent);

                // process

                string countryCode = lineParts[0].Trim().ToUpperInvariant();

                int countryGeoId = Geography.RootIdentity;
                if (geoNameLookup.ContainsKey (countryCode))
                {
                    countryGeoId = geoNameLookup[countryCode];
                }
                else
                {
                    Country country = Country.FromCode (countryCode);
                    if (countryGeoId == Geography.RootIdentity) // country not initialized as node
                    {
                        countryGeoId =
                            SwarmDb.GetDatabaseForWriting().CreateGeography (country.Name + " (" + country.Name + ")",
                                Geography.RootIdentity); // TODO: Locate country
                        SwarmDb.GetDatabaseForWriting().SetCountryGeographyId (country.Identity, countryGeoId);
                        geoNameLookup[countryCode] = countryGeoId;
                    }

                    Geographies countryGeographies = Geography.FromIdentity(countryGeoId).GetTree();

                    foreach (Geography geography in countryGeographies)
                    {
                        geoNameLookup[countryCode + geography.Name] = geography.Identity;
                    }
                }

                int lastGeoId = countryGeoId;
                for (int partIndex = 1; partIndex < lineParts.Length; partIndex++)
                {
                    if (geoNameLookup.ContainsKey (countryCode + lineParts[partIndex]))
                    {
                        // geography exists
                        lastGeoId = geoNameLookup[countryCode + lineParts[partIndex]];
                    }
                    else
                    {
                        // geography does not exist yet: create

                        int newGeoId = SwarmDb.GetDatabaseForWriting().CreateGeography (lineParts[partIndex], lastGeoId);
                        geoNameLookup[countryCode + lineParts[partIndex]] = newGeoId;
                        lastGeoId = newGeoId;
                    }

                }

            }

            GuidCache.Set (guid + "-Progress", 100);
        }
    }
}