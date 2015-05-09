using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Converters;

namespace Swarmops.Frontend.Controls.v5.Base
{
    public partial class DropDown : ControlV5Base
    {
        public ListItemCollection Items
        {
            get { return this.DropControl.Items; }
        }

        public string SelectedValue
        {
            get { return this.DropControl.SelectedValue; }
            set { this.DropControl.SelectedValue = value; }
        }

        public string OnClientChange { get; set; }
        public string DataUrl { get; set; }

        public string ClientControlID
        {
            get { return this.ClientID + "_DropControl"; }
        }

        public DropDownList ClientControl
        {
            get { return this.DropControl; }
        }

        protected void Page_Load (object sender, EventArgs e)
        {
        }
    }
}