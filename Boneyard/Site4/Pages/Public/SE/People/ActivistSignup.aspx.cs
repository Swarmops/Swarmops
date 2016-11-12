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
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;


public partial class Pages_Public_SE_People_ActivistSignup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.TextPostalCode.Focus();
    }


    protected void ButtonSubmitPostalCode_Click(object sender, EventArgs e)
    {
        // Lookup postal code

        string postalCode = this.TextPostalCode.Text.Trim().Replace (" ", "");

        Cities cities = Cities.FromPostalCode(postalCode, Country.FromCode("SE"));

        if (cities.Count == 1)
        {
            // Ok, we're done and resolved!

            this.LabelGeographySecondPrompt.Text = "Postnumret ligger i ";
            this.LabelGeographyDeterminedLocation.Text = cities[0].Name + ", " + cities[0].Geography.Name + ".";

            this.DropCities.Visible = false;
            this.ButtonSubmitCity.Visible = false;
            this.PanelMainQuestions.Visible = true;
            this.TextName.Focus();
        }
        else if (cities.Count == 0)
        {
            // Unable to find postal code

            this.LabelGeographySecondPrompt.Text = "Hittar inte postnumret. Vilken kommun bor du i? ";
            this.DropCities.Items.Clear();
            this.DropCities.Items.Add(new ListItem("--Välj--", "0"));

            Geographies geos = Geographies.FromLevel(Country.FromCode("SE"), GeographyLevel.Municipality);

            foreach (Geography geo in geos)
            {
                this.DropCities.Items.Add(new ListItem(geo.Name, geo.Identity.ToString()));
            }

            this.LabelGeographyDeterminedLocation.Text = string.Empty;
            this.DropCities.Visible = true;
            this.ButtonSubmitCity.Visible = true;
            this.PanelMainQuestions.Visible = false;
            this.DropCities.Focus();
        }
        else
        {
            // More than one candidate

            this.LabelGeographySecondPrompt.Text = "Du har ett krångligt postnummer. Vilken kommun bor du i? ";
            this.DropCities.Items.Clear();
            this.DropCities.Items.Add(new ListItem("--Välj--", "0"));

            foreach (City city in cities)
            {
                this.DropCities.Items.Add(new ListItem(city.Geography.Name, city.GeographyId.ToString()));
            }

            this.LabelGeographyDeterminedLocation.Text = string.Empty; 
            this.DropCities.Visible = true;
            this.ButtonSubmitCity.Visible = true;
            this.PanelMainQuestions.Visible = false;
            this.DropCities.Focus();
        }
    }


    protected void ButtonSubmitCity_Click(object sender, EventArgs e)
    {
        if (this.DropCities.SelectedValue != "0")
        {
            this.PanelMainQuestions.Visible = true;
            this.TextName.Focus();
        }
        else
        {
            this.PanelMainQuestions.Visible = false;
        }
    }

    protected void ButtonSubmit_Click(object sender, EventArgs e)
    {
        // TODO: Validate arguments a bit more, later
        //TODO: Hardcoded Sweden
        Geography geo = null;

        if (this.DropCities.Visible)
        {
            geo = Geography.FromIdentity(Int32.Parse(this.DropCities.SelectedValue));
        }
        else
        {
            /* Fix for #106 - Fel vid aktivistregistrering */
            string postalCode = this.TextPostalCode.Text.Trim().Replace(" ", "");

            geo = Cities.FromPostalCode(postalCode, "SE")[0].Geography;
        }

        Person newActivist = Person.Create(this.TextName.Text, this.TextEmail.Text, Activizr.Logic.Security.Authentication.CreateRandomPassword(16), this.TextPhone.Text,
            string.Empty, this.TextPostalCode.Text, string.Empty, "SE", new DateTime(1900, 1, 1), PersonGender.Unknown);
        newActivist.Geography = geo;

        Activizr.Logic.Support.ActivistEvents.CreateActivistWithLogging(geo, newActivist, "The self-signup came from IP " + Request.UserHostAddress, EventSource.SignupPage, false, false,Organization.PPSEid);

        this.LabelGeographyResult.Text = geo.Name;

        this.PanelFinished.Visible = true;
        this.DropCities.Enabled = false;
        this.TextEmail.Enabled = false;
        this.TextPostalCode.Enabled = false;
        this.TextName.Enabled = false;
        this.TextPhone.Enabled = false;
        this.ButtonSubmit.Enabled = false;
        this.ButtonSubmitCity.Enabled = false;
        this.ButtonSubmitPostalCode.Enabled = false;
    }


}
