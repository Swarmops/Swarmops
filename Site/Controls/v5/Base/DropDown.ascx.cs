using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Common.Enums;


namespace Swarmops.Frontend.Controls.Base
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

        public LayoutDirection Layout { get; set; }

        protected void Page_Load (object sender, EventArgs e)
        {
            if (this.Layout == LayoutDirection.Unknown)
            {
                this.Layout = LayoutDirection.Vertical;
            }
        }

        public string AjaxCallbackUrl { get; set; }
        public string Cookie { get; set; }

    }
}