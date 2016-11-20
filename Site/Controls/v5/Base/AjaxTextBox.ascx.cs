using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Controls.Base
{
    public partial class AjaxTextBox : ControlV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.TextInput.Attributes["Placeholder"] = this.Placeholder;
            this.TextInput.TextMode = this.Mode; // Defaults to SingleLine, so will work uninitialized
        }

        public string Text
        {
            get { return this.TextInput.Text; }
            set { this.TextInput.Text = value; }
        }

        public override void Focus()
        {
            base.Focus();
            this.TextInput.Focus();
        }

        public string Cookie { set; get; }
        public string OnChange { get; set; }
        public string OnChanging { get; set; }
        public string OnKeyDown { get; set; }
        public string AjaxCallbackUrl { set; get; }
        public TextBoxMode Mode { get; set; }

        public string CssClass
        {
            get { return this.TextInput.CssClass; }
            set { this.TextInput.CssClass = value; }
        }

        public bool ReadOnly
        {
            get { return this.TextInput.ReadOnly; }
            set { this.TextInput.ReadOnly = value;  }
        }

        public string Placeholder { get; set; }

        // Ajax result codes and classes
        /*
        public const int CodeUnknown = 0;
        public const int CodeSuccess = 1;
        public const int CodeChanged = 2;
        public const int CodeInvalid = 3;
        public const int CodeNoPermission = 4;

        public class CallbackResult
        {
            public int ResultCode { get; set; }
            public string DisplayMessage { get; set; } // may be empty
            public string NewData { get; set; }
        };*/
    }
}