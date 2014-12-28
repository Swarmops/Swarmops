using System;
using System.Globalization;
using System.Web.UI;

namespace Swarmops.Frontend.Controls.v5.Financial
{
    /// <summary>
    ///     This is an ordinary textbox, except it does culture-sensitive and
    ///     asynchronous validation of currency input.
    /// </summary>
    public partial class CurrencyTextBox : UserControl
    {
        protected void Page_Load (object sender, EventArgs e)
        {
        }

        public double Value
        {
            get
            {
                // Try to parse the Double in two steps: first as a Culture.CurrentCulture, and if that doesn't work out, as a Culture.InvariantCulture.

                double outParse = 0.0;
                string contents = Input.Text;

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
            set { this.Input.Text = value.ToString ("N2", CultureInfo.CurrentCulture); }
        }
    }
}