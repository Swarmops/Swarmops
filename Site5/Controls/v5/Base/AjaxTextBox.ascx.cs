﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Swarmops.Frontend.Controls.v5.Base
{
    public partial class AjaxTextBox : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string Text
        {
            get { return this.TextInput.Text; }
            set { this.TextInput.Text = value; }
        }

        public void Focus()
        {
            this.TextInput.Focus();
        }

        public string Cookie { set; get; }
        public string AjaxCallbackUrl { set; get; }

        // Ajax result codes and classes

        public const int CodeUnknown = 0;
        public const int CodeSuccess = 1;
        public const int CodeChanged = 2;
        public const int CodeInvalid = 3;
        public const int CodeNoPermission = 4;

        public class AjaxResult
        {
            public int ResultCode { get; set; }
            public string DisplayMessage { get; set; } // may be empty
            public string NewData { get; set; }
        };
    }
}