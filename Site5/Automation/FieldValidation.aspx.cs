using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

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
    }
}