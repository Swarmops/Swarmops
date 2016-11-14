using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// ReSharper disable once CheckNamespace
namespace Swarmops.Frontend.Controls.Base
{
    public partial class AjaxToggleSlider : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string Label { get; set; }
        public string CallbackUrl { get; set; }
        public string OnClientChange { get; set; }
        public string Cookie { get; set; }
    }
}