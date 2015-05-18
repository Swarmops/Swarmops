using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Common.Enums;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.v5.Public
{
    public partial class Json_SignupVolunteerPositions : DataV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            // No access control on page - this is called from Signup

            _organization = Organization.FromIdentity (Convert.ToInt32 (Request["OrganizationId"]));
            int geographyId = Convert.ToInt32 (Request["GeographyId"]);
            if (geographyId != 0)
            {
                _geography = Geography.FromIdentity (geographyId);
            }

            Positions positions = Positions.ForOrganization (_organization).AtLevel (PositionLevel.GeographyDefault);

            // Format as JSON and return

            CommonV5.CulturePreInit (Request); // Sets the culture

            Response.ContentType = "application/json";
            string json = FormatAsJson (positions);
            Response.Output.WriteLine (json);
            Response.End();
        }

        private Organization _organization;
        private Geography _geography;

        private string FormatAsJson (Positions positions)
        {
            StringBuilder result = new StringBuilder (16384);

            result.Append ("[");

            foreach (Position position in positions)
            {
                if (position.Volunteerable)
                {
                    StringBuilder extraTags = new StringBuilder();

                    result.Append ("{");
                    result.AppendFormat (
                        "\"positionId\":\"{0}\",\"positionTitle\":\"{1}\",\"highestGeography\":\"{2}\",\"positionDescription\":\"{3}\"",
                        position.Identity, JsonSanitize(position.Localized()), string.Empty /* geography */, string.Empty /* description */);
                    result.Append ("},");
                }
            }

            if (result.Length > 0)
            {
                result.Remove (result.Length - 1, 1); // remove last comma
            }

            result.Append ("]");

            return result.ToString();
        }
    }
}