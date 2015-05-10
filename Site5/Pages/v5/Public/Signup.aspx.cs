using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        protected override void OnPreInit(EventArgs e)
        {
            CommonV5.CulturePreInit (Request);

 	        base.OnPreInit(e);
        }

        protected void Page_Load (object sender, EventArgs e)
        {
            // Was this a quick callback to set the culture?

            if (Request["Culture"] != null)
            {
                Response.SetCookie(new HttpCookie("PreferredCulture", Request["Culture"]));
                Response.Redirect(Request.RawUrl.Substring (0, Request.RawUrl.IndexOf ("&Culture=")), true);
            }

            // Find what organization we're supposed to sign up to

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

            this.LiteralErrorNeedPassword.Text = Resources.Pages.Public.Signup_Error_NeedPassword;
            this.LiteralErrorPasswordMismatch.Text = Resources.Pages.Public.Signup_Error_PasswordMismatch;

            this.LabelWelcomeHeader.Text = String.Format (Resources.Pages.Public.Signup_Welcome, Organization.Name);
            this.LabelHeader.Text = String.Format(Resources.Pages.Public.Signup_SigningUp, Organization.Name).ToUpperInvariant();
            this.LabelYourLogon.Text = Resources.Pages.Public.Signup_YourLogon;
            this.LabelYourLogonText.Text = Resources.Pages.Public.Signup_YourLogonText;

            this.LabelStep1Header.Text = Resources.Pages.Public.Signup_Step1Header;
            this.LabelStep1Text.Text = Resources.Pages.Public.Signup_Step1Text;
            this.LabelStep2Header.Text = Resources.Pages.Public.Signup_Step2Header;
            this.LabelStep2Text.Text = Resources.Pages.Public.Signup_Step2Text;
            this.LabelStep3Header.Text = Resources.Pages.Public.Signup_Step3Header;
            this.LabelStep3Text.Text = Resources.Pages.Public.Signup_Step3Text;
            this.LabelStep4Header.Text = Resources.Pages.Public.Signup_Step4Header;
            this.LabelStep4Text.Text = Resources.Pages.Public.Signup_Step4Text;
            this.LabelStep5Header.Text = Resources.Pages.Public.Signup_Step5Header;
            this.LabelStep5Text.Text = Resources.Pages.Public.Signup_Step5Text;
            this.LabelStep6Header.Text = Resources.Pages.Public.Signup_Step6Header;
            this.LabelStep6Text.Text = Resources.Pages.Public.Signup_Step6Text;

            this.LabelName.Text = Resources.Pages.Public.Signup_YourName;
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

            this.LabelLoginKey.Text = Resources.Pages.Public.Signup_MailLoginKey;
            this.LabelPassword1.Text = Resources.Pages.Public.Signup_Password1;
            this.LabelPassword2.Text = Resources.Pages.Public.Signup_Password2;

            this.TextDateOfBirth.Attributes["placeholder"] = Global.Global_DateFormatShortReadable;
            this.TextName.Attributes["placeholder"] = "Joe Smith";
            this.TextMail.Attributes["placeholder"] = "joe@example.com";
            this.TextPhone.Attributes["placeholder"] = "+1 263 151 1341";
            this.TextStreet1.Attributes["placeholder"] = "78 West Avenue";
            this.TextPostal.Attributes["placeholder"] = "12345";

            // Enable support for RTL languages

            if (Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft)
            {
                this.LiteralBodyAttributes.Text = @"dir='rtl' class='rtl'";
            }

        }


        protected Organization Organization;
    }
}