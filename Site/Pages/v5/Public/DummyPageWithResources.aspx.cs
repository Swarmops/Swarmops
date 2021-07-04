using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;

namespace Swarmops.Frontend.Pages.v5.Public
{
    public partial class DummyPageWithResources : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //System.Resources.ResourceManager manager = new System.Resources.ResourceManager("Global", System.Reflection.Assembly.Load("Global.resources"));

            this.LabelTest.Text = Resources.Global.Dashboard_Main_Temporary; // manager.Get("Dashboard_Main_Temporary");
        }
    }
}