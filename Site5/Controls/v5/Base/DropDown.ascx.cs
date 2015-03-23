using System;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Controls.v5.Base
{
    public partial class DropDown : ControlV5Base
    {


        public string OnClientChange { get; set; }
        public string DataUrl { get; set; }

        public string ClientControlID
        {
            get { return this.ClientID + "_DropDown"; }
        }

        protected void Page_Load (object sender, EventArgs e)
        {
        }
    }
}