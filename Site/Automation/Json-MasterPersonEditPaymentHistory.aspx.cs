using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Automation
{
    public partial class Json_MasterPersonEditPaymentHistory : DataV5Base
    {
        private AuthenticationData _authenticationData;
        private Person _person;

        protected void Page_Load(object sender, EventArgs e)
        {
            this._authenticationData = GetAuthenticationDataAndCulture();

            int personId = Int32.Parse(Request.QueryString["PersonId"]); // may throw and that's okay
            this._person = Person.FromIdentity(personId);

            /*

            THIS ENTIRE STUFF IS TODO

            Response.ContentType = "application/json";

            Response.Output.WriteLine("{\"rows\": " + RecurseReport(report.ReportLines) + ", \"footer\": [" +
                                       WriteFooter() + "]}");


            Response.End();*/
        }
    }
}