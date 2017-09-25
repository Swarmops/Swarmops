using System;
using System.Globalization;
using Swarmops.Common.Enums;
using System.Web.UI;

namespace Swarmops.Frontend.Controls.Financial
{
    /// <summary>
    ///     This is an ordinary textbox, except it does culture-sensitive and
    ///     asynchronous validation of currency input.
    /// </summary>
    public partial class CurrencyTextBox : ControlV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            this.TextInput.Attributes["role"] = "note"; // disable Lastpass trying to autofill

            if (this.Layout == LayoutDirection.Unknown)
            {
                this.Layout = LayoutDirection.Vertical;
            }

            this.DefaultCurrency = this.CurrentAuthority.Organization.Currency;
        }

        public Logic.Financial.Currency DefaultCurrency { get; set; }

        public Int64 Cents
        {
            get 
            { 
                return (Int64) (InternalValue * 100.0 + 0.5); // the +0.5 is to compensate for floating point errors. And yes, THEY HAPPEN. 
            }
            set { this.TextInput.Text = (value / 100.0).ToString("N2", CultureInfo.CurrentCulture); }
        }

        public Int64 Metacents  // Metacents are cents of cents, so four decimals to the currency unit.
        {
            get
            {
                return (Int64)(InternalValue * 10000.0 + 0.5); // the +0.5 is to compensate for floating point errors. And yes, THEY HAPPEN. 
            }
            set { this.TextInput.Text = (value / 10000.0).ToString("N4", CultureInfo.CurrentCulture); }
        }

        [Obsolete ("Use Cents or Metacents", true)]
        public double Value
        {
            get { throw new NotImplementedException(); }
        }

        internal double InternalValue
        {
            get
            {
                // Try to parse the Double in two steps: first as a Culture.CurrentCulture, and if that doesn't work out, as a Culture.InvariantCulture.

                double outParse = 0.0;
                string contents = this.TextInput.Text;

                if (Double.TryParse (contents, NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out outParse))
                {
                    return outParse;
                }
                if (Double.TryParse(contents, NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out outParse))
                {
                    return outParse;
                }

                // Unparsable. Sorry.

                throw new InvalidCastException("The text field cannot be parsed to a Double: \"" + contents + "\"");

            }
        }

        // TODO: Add other-currency parsing

        public LayoutDirection Layout { get; set; }

        public new void Focus()
        {
            this.TextInput.Focus();
        }
    }
}