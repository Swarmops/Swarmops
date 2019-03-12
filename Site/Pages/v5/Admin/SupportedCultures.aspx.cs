using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Security;

namespace Swarmops.Frontend.Admin
{
    public partial class SupportedCultures : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.PageAccessRequired = new Access(AccessAspect.Null);

            this.InfoBoxLiteral = "This shows the system cultures (languages), as well as which ones are installed and supported.";
            this.Title = this.PageTitle = @"Supported Languages and Cultures";

        }
    }
}