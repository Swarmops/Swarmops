using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Controls.v5.Base
{
    public partial class WebUserControlTest : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.HiddenTest.Value = "Foo";
        }
    }
}