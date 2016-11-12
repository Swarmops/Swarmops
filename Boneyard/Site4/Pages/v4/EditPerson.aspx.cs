using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;


public partial class Pages_v4_EditPerson : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        int personId = 1;

        Person person = Person.FromIdentity(personId);

        // TODO: Authority check

        // Populate initial fields

        if (!Page.IsPostBack)
        {
            this.TextMemberNumber.Text = person.Identity.ToString();
            this.TextName.Text = person.Name;
            this.TextStreet.Text = person.Street;
            this.TextPostalCode.Text = person.PostalCode;
            this.TextCity.Text = person.CityName;
            this.TextEmail.Text = person.Email;
            this.LabelGeographyLine.Text = string.Empty; //...

            // Populate countries

            Countries countries = Countries.GetAll();
            foreach (Country country in countries)
            {
                DropCountries.Items.Add(new ListItem(country.Code + " " + country.Name, country.Code));
            }

            DropCountries.Items.FindByValue(person.Country.Code).Selected = true;
        }
    }

    protected void TextPostalCode_TextChanged (object sender, EventArgs e)
    {
        if (this.TextPostalCode.Text.Trim().Length == 0)
        {
            return;
        }

        Cities cities = Cities.FromPostalCode(this.TextPostalCode.Text.Replace(" ", ""),
                                              this.DropCountries.SelectedValue);

        // Trigger one of three different mechanisms: Freeform (no match), One choice preset (one match)
        // or dropdown from presets (several matches)

        if (cities.Count == 1)
        {
            this.TextCity.Visible = true;
            this.TextCity.Enabled = false;
            this.TextCity.Text = cities[0].Name;
        }
        else if (cities.Count == 0)
        {
            this.TextCity.Visible = true;
            this.TextCity.Enabled = true;
            this.TextCity.Focus();
        }
    }
}