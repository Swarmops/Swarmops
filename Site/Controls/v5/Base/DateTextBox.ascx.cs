﻿using System;
using System.Globalization;
using Swarmops.Common.Enums;
using System.Web.UI;
using Swarmops.Common;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;

namespace Swarmops.Frontend.Controls.Base
{
    /// <summary>
    ///     This is an ordinary textbox, except it does culture-sensitive and
    ///     asynchronous validation of currency input.
    /// </summary>
    public partial class DateTextBox : ControlV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            this.TextInput.Attributes["role"] = "note"; // disable Lastpass trying to autofill

            if (this.Layout == LayoutDirection.Unknown)
            {
                this.Layout = LayoutDirection.Vertical;
            }

            if (!Page.IsPostBack)
            {
                this.InterpretedDate.Value = DateTime.Today.ToString(CultureInfo.InvariantCulture);
                // initialize to valid value
            }
        }

        public DateTime Value
        {
            get
            {
                DateTime result;
                bool success = DateTime.TryParse(this.InterpretedDate.Value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out result);

                if (success)
                {
                    return result;
                }

                return Constants.DateTimeLow;
            }
            set
            {
                this.InterpretedDate.Value = value.ToString(CultureInfo.InvariantCulture);
                this.TextInput.Text = value.ToString("yyyy MMMM dd");
            }
        }

        public LayoutDirection Layout { get; set; }
    }
}