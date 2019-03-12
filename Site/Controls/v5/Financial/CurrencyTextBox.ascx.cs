using System;
using System.Globalization;
using Swarmops.Common.Enums;
using System.Web.UI;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Support;

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
                string contents = this.TextInput.Text;
                return Formatting.ParseDoubleStringAsCents(contents);

            }
            set
            {
                this.TextInput.Text = (value / 100.0).ToString("N2", CultureInfo.CurrentCulture);
                if (value == 0)
                {
                    // if zeroing, also clear the foreign currency
                    this.EnteredCurrency.Value = string.Empty;
                    this.EnteredAmount.Value = string.Empty;
                }
            }
        }

        public Int64 Metacents  // Metacents are cents of cents, so four decimals to the currency unit.
        {
            get
            {
                string contents = this.TextInput.Text;
                return Formatting.ParseDoubleStringAsCents(contents, null, Formatting.ParsingScale.Metacents);
            }
            set { this.TextInput.Text = (value / 10000.0).ToString("N4", CultureInfo.CurrentCulture); }
        }

        [Obsolete ("Use Cents or Metacents", true)]
        public double Value
        {
            get { throw new NotImplementedException(); }
        }

        // TODO: Add other-currency parsing

        public LayoutDirection Layout { get; set; }

        public new void Focus()
        {
            this.TextInput.Focus();
        }

        public bool NonPresentationCurrencyUsed
        {
            get { return this.EnteredCurrency.Value.Length > 0; }
        }

        public Money NonPresentationCurrencyAmount
        {
            get
            {
                return new Money(Swarmops.Logic.Support.Formatting.ParseDoubleStringAsCents (this.EnteredAmount.Value), Currency.FromCode(this.EnteredCurrency.Value));
            }
        }
    }
}