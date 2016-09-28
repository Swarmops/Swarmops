using System;
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

            foreach (string lineRaw in lines)
            {
                count++;
                string line = lineRaw.Trim();
                string[] lineParts = line.Split ('\t');

                if (lineParts.Length < 12)
                {
                    continue;
                }

                int percent = (count*99)/lines.Length;
                if (percent == 0)
                {
                    percent = 1;
                }
                GuidCache.Set (guid + "-Progress", percent);

                string name = lineParts[0] + " " + lineParts[1];
                PersonGender gender = lineParts[2] == "male" ? PersonGender.Male : PersonGender.Female;
                DateTime dateOfBirth = DateTime.Parse (lineParts[7], new CultureInfo ("en-US"), DateTimeStyles.None);
                Country country = Country.FromCode (lineParts[6]);


                Person newPerson = Person.Create (name, string.Empty, string.Empty, lineParts[8], lineParts[3],
                    lineParts[4].Replace (" ", ""), lineParts[5], lineParts[6], dateOfBirth, gender);

                newPerson.NationalIdNumber = lineParts[9];
                newPerson.Longitude = lineParts[10];
                newPerson.Latitude = lineParts[11];

                newPerson.AddParticipation (Organization.Sandbox, DateTime.Today.AddDays (random.Next (365)));
            }

            GuidCache.Set (guid + "-Progress", 100);
        }
    }
}