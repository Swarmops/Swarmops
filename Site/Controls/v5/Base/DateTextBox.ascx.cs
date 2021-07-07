using System;
using System.Globalization;
using Swarmops.Common.Enums;
using System.Web.UI;
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
            this.InterpretedDate.Value = DateTime.Today.ToString(CultureInfo.InvariantCulture); // initialize to valid value

            if (this.Layout == LayoutDirection.Unknown)
            {
                this.Layout = LayoutDirection.Vertical;
            }
        }

        public DateTime Value
        {
            get { return DateTime.Parse(this.InterpretedDate.Value, CultureInfo.InvariantCulture); }
            set
            {
                this.InterpretedDate.Value = Value.ToString(CultureInfo.InvariantCulture);
                this.TextInput.Text = Value.ToString("YYYY MMM dd");
            }
        }

        public LayoutDirection Layout { get; set; }
    }
}