using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Controls.Swarm
{
    public partial class AjaxComboPeople : ControlV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            Localize();

            this.ComboPeople.OnClientSelect = this.ClientID + "_pvt_onSelectionChange";
        }

        private void Localize()
        {
            this.LiteralServerError.Text = JavascriptEscape(Resources.Global.Error_AjaxCallException);
        }

        public string Cookie { set; get; }
        public string OnChange { set; get; }
        public string AjaxCallbackUrl { get; set; }
        public string Placeholder { get; set; }
    }
}