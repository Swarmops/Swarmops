using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Frontend.Pages.Public
{
    public partial class Signup : System.Web.UI.Page
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            this.Organization = Organization.FromVanityDomain (Request.Url.Host);

            if (this.Organization == null)
            {
                int organizationId = Convert.ToInt32 (Request["OrganizationId"]); // may throw - not pretty but ok for now. // TODO
                this.Organization = Organization.FromIdentity (organizationId); // may also throw, as above
            }

            if (this.Organization == null)
            {
                throw new ArgumentException("Organization not specified");  // TODO: make a friendly landing page instead
            }


            // Override style widths - (this will cause problems with a future responsive design; come back here to fix that)

            this.DropCountries.Style[HtmlTextWriterStyle.Width] = "204px";
            this.DropGenders.Style[HtmlTextWriterStyle.Width] = "204px";

            /*
            this.BoxTitle.Text = PageTitle = String.Format(Resources.Pages.Swarm.AddPerson_Title,
                Participant.Localized(CurrentOrganization.RegularLabel));
            InfoBoxLiteral = String.Format(Resources.Pages.Swarm.AddPerson_Info, Global.Timespan_OneYear,
                Participant.Localized(CurrentOrganization.RegularLabel, TitleVariant.Ship),
                DateTime.Today.AddYears(1).ToLongDateString());*/

            if (!Page.IsPostBack)
            {
                Localize();
                Populate();

                this.TextName.Focus();
            }

        }

        private void Populate()
        {
            Countries allCountries = Countries.All;
            this.DropCountries.Items.Clear();

            foreach (Country country in allCountries)
            {
                string countryLocalName = country.Localized;
                string countryDisplay = country.Code + " " + countryLocalName;
                this.DropCountries.Items.Add (new ListItem (countryDisplay, country.Code));
            }

            if (this.Organization.DefaultCountry != null)
            {
                this.DropCountries.SelectedValue = this.Organization.DefaultCountry.Code;
            }

            this.DropGenders.Items.Clear();
            this.DropGenders.Items.Add (new ListItem (Global.Global_UnknownUndisclosed, "Unknown"));
            this.DropGenders.Items.Add (new ListItem (Global.Global_Female, "Female"));
            this.DropGenders.Items.Add (new ListItem (Global.Global_Male, "Male"));
        }

        private void Localize()
        {
            this.LiteralErrorCity.Text = Resources.Pages.Swarm.AddPerson_ErrorCity;
            this.LiteralErrorMail.Text = Resources.Pages.Swarm.AddPerson_ErrorMail;
            this.LiteralErrorName.Text = Resources.Pages.Swarm.AddPerson_ErrorName;
            this.LiteralErrorStreet.Text = Resources.Pages.Swarm.AddPerson_ErrorStreet;
            this.LiteralErrorDate.Text = Resources.Pages.Swarm.AddPerson_ErrorDate;

            this.LabelName.Text = Resources.Global.Global_Name;
            this.LabelCountry.Text = Resources.Global.Global_Country;
            this.LabelMail.Text = Resources.Global.Global_Mail;
            this.LabelPhone.Text = Resources.Global.Global_Phone;
            this.LabelStreet1.Text = Resources.Pages.Swarm.AddPerson_Street1PO;
            this.LabelStreet2.Text = Resources.Pages.Swarm.AddPerson_Street2;
            this.LabelPostalCode.Text = Resources.Global.Global_PostalCode;
            this.LabelCity.Text = Resources.Global.Global_City;
            this.LabelGeographyDetected.Text = Resources.Pages.Swarm.AddPerson_GeographyDetected;
            this.LabelDateOfBirth.Text = Resources.Global.Global_DateOfBirth;
            this.LabelLegalGender.Text = Resources.Pages.Swarm.AddPerson_LegalGender;

            this.TextDateOfBirth.Attributes["placeholder"] = Global.Global_DateFormatShortReadable;
            this.TextName.Attributes["placeholder"] = "Joe Smith";
            this.TextMail.Attributes["placeholder"] = "joe@example.com";
            this.TextPhone.Attributes["placeholder"] = "+1 263 151 1341";
            this.TextStreet1.Attributes["placeholder"] = "78 West Avenue";
            this.TextPostal.Attributes["placeholder"] = "12345";
        }


        protected Organization Organization;
    }
}