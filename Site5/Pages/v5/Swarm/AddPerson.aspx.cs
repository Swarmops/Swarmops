using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Swarmops.Logic.Structure;

// ReSharper disable once CheckNamespace
namespace Swarmops.Frontend.Pages.v5.Swarm
{
    public partial class AddPerson : PageV5Base
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Override style widths - (this will cause problems with a future responsive design; come back here to fix that)

            this.TextPostal.Style[HtmlTextWriterStyle.Width] = "70px";
            this.TextCity.Style[HtmlTextWriterStyle.Width] = "160px";

            if (!Page.IsPostBack)
            {
                Populate();
                Localize();

                this.TextName.Focus();
            }

            this.IncludedControlsUsed = IncludedControl.JsonParameters;
        }

        private void Populate()
        {
            Countries allCountries = Countries.GetAll();
            this.DropCountries.Items.Clear();

            foreach (Country country in allCountries)
            {
                string countryLocalName = Resources.GeographyNames.ResourceManager.GetString("Country_" + country.Code);
                if (string.IsNullOrEmpty(countryLocalName))
                {
                    countryLocalName = country.Name + "*"; // In English. Asterisk indicates resource missing.
                }
                string countryDisplay = country.Code + " " + countryLocalName;
                this.DropCountries.Items.Add(new ListItem(countryDisplay, country.Code));
            }

            if (this.CurrentOrganization.DefaultCountry != null)
            {
                this.DropCountries.SelectedValue = this.CurrentOrganization.DefaultCountry.Code;
            }

            this.DropGenders.Items.Clear();
            this.DropGenders.Items.Add(new ListItem(Resources.Global.Global_UnknownUndisclosed, "Unknown"));
            this.DropGenders.Items.Add(new ListItem(Resources.Global.Global_Female, "Female"));
            this.DropGenders.Items.Add(new ListItem(Resources.Global.Global_Male, "Male"));
        }

        private void Localize()
        {
            // TODO

            this.InfoBoxLiteral = "<strong>Work in progress</strong>";

            this.TextDateOfBirth.Attributes["placeholder"] = Resources.Global.Global_DateFormatShort;
            this.TextName.Attributes["placeholder"] = "Joe Smith";
            this.TextMail.Attributes["placeholder"] = "joe@example.com";
            this.TextPhone.Attributes["placeholder"] = "+1 263 151 1341";
            this.TextStreet1.Attributes["placeholder"] = "78 West Avenue";
            this.TextPostal.Attributes["placeholder"] = "123456";
        }
    }
}