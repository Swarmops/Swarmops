using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Controls.v5.Base
{
    public partial class DropDown : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public ListItemCollection Items
        {
            get { return this.DropControl.Items; }
        }

        public string SelectedValue
        {
            get { return this.DropControl.SelectedValue; }
        }
    }
}