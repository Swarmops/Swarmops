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
            this.ToggleSliderLabel.Text = this.Label;
        }

        public string Label { get; set; }
        public string AjaxCallbackUrl { get; set; }
        public string OnChange { get; set; }
        public string Cookie { get; set; }
        public bool InitialValue { get; set; }


        // ReSharper disable InconsistentNaming
        public string Localized_SwitchLabelOn_Upper
        {
            get { return CommonV5.JavascriptEscape(Resources.Global.Global_On.ToUpperInvariant()); }
        }

        public string Localized_SwitchLabelOff_Upper
        {
            get { return CommonV5.JavascriptEscape(Resources.Global.Global_Off.ToUpperInvariant()); }
        }
    }
}