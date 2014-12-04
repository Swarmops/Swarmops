using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Resources.Pages;
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
                string countryLocalName = GeographyNames.ResourceManager.GetString("Country_" + country.Code);
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
            this.DropGenders.Items.Add(new ListItem(Global.Global_UnknownUndisclosed, "Unknown"));
            this.DropGenders.Items.Add(new ListItem(Global.Global_Female, "Female"));
            this.DropGenders.Items.Add(new ListItem(Global.Global_Male, "Male"));

            this.LabelExpiry.Text = DateTime.Today.AddYears(1).ToString(Global.Global_LongDateFormatSansWeekday);
        }

        private void Localize()
        {
            // TODO

            this.InfoBoxLiteral = People.AddPerson_Info;

            this.TextDateOfBirth.Attributes["placeholder"] = Global.Global_DateFormatShort;
            this.TextName.Attributes["placeholder"] = "Joe Smith";
            this.TextMail.Attributes["placeholder"] = "joe@example.com";
            this.TextPhone.Attributes["placeholder"] = "+1 263 151 1341";
            this.TextStreet1.Attributes["placeholder"] = "78 West Avenue";
            this.TextPostal.Attributes["placeholder"] = "12345";
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            // Register user

            // Send notify

            // Send password in mail
        }
    }
}