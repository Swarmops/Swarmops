using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Admin.Hacks
{
    public partial class PopulateData : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageTitle = "Populate Data";
            this.PageIcon = "iconshock-battery-drill";
            this.InfoBoxLiteral = "You've taken a wrong turn if you're even seeing this page. It's for development purposes.";
        }


        private void Localize()
        {

        }


        protected void ButtonProcess_Click(object sender, EventArgs e)
        {

            string data = this.TextData.Text;

            string[] lines = data.Split('\n');

            Random random = new Random();

            foreach (string lineRaw in lines)
            {
                string line = lineRaw.Trim();
                string[] lineParts = line.Split('\t');

                if (lineParts.Length < 12)
                {
                    continue;
                }

                string name = lineParts[0] + " " + lineParts[1];
                PersonGender gender = lineParts[2] == "male" ? PersonGender.Male : PersonGender.Female;
                DateTime dateOfBirth = DateTime.Parse(lineParts[7], new CultureInfo("en-US"), DateTimeStyles.None);
                Country country = Country.FromCode(lineParts[6]);
                

                Person newPerson = Person.Create(name, string.Empty, string.Empty, lineParts[8], lineParts[3],
                    lineParts[4].Replace(" ", ""), lineParts[5], lineParts[6], dateOfBirth, gender);

                newPerson.NationalIdNumber = lineParts[9];
                newPerson.Longitude = lineParts[10];
                newPerson.Latitude = lineParts[11];

                newPerson.AddMembership(1, DateTime.Today.AddDays(random.Next(365)));
            }



            Response.AppendCookie(new HttpCookie("DashboardMessage", "Data was successfully processed."));

            // Redirect to dashboard

            Response.Redirect("/", true);
        }

    }
}