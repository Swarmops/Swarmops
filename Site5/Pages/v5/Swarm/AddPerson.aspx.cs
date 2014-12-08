using System;
using System.Reflection;
using System.Runtime.Versioning;
using System.Web.UI;
using System.Web.UI.WebControls;
using Resources;
using Swarmops.Basic.Enums;
using Swarmops.Logic.Communications;
using Swarmops.Logic.Communications.Transmission;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;

// ReSharper disable once CheckNamespace

namespace Swarmops.Frontend.Pages.v5.Swarm
{
    public partial class AddPerson : PageV5Base
    {
        protected void Page_Load (object sender, EventArgs e)
        {
            // Override style widths - (this will cause problems with a future responsive design; come back here to fix that)

            this.TextPostal.Style[HtmlTextWriterStyle.Width] = "70px";
            this.TextCity.Style[HtmlTextWriterStyle.Width] = "160px";

            this.BoxTitle.Text = PageTitle = String.Format(Resources.Pages.Swarm.AddPerson_Title,
                Participant.Localized(CurrentOrganization.RegularLabel));
            InfoBoxLiteral = String.Format(Resources.Pages.Swarm.AddPerson_Info, Global.Timespan_OneYear,
                Participant.Localized(CurrentOrganization.RegularLabel, TitleVariant.Ship),
                DateTime.Today.AddYears(1).ToLongDateString());

            if (!Page.IsPostBack)
            {
                Localize();
                Populate();

                this.TextName.Focus();
            }

            IncludedControlsUsed = IncludedControl.JsonParameters;
        }

        private void Populate()
        {
            Countries allCountries = Countries.GetAll();
            this.DropCountries.Items.Clear();

            foreach (Country country in allCountries)
            {
                string countryLocalName = GeographyNames.ResourceManager.GetString ("Country_" + country.Code);
                if (string.IsNullOrEmpty (countryLocalName))
                {
                    countryLocalName = country.Name + "*"; // In English. Asterisk indicates resource missing.
                }
                string countryDisplay = country.Code + " " + countryLocalName;
                this.DropCountries.Items.Add (new ListItem (countryDisplay, country.Code));
            }

            if (CurrentOrganization.DefaultCountry != null)
            {
                this.DropCountries.SelectedValue = CurrentOrganization.DefaultCountry.Code;
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
            this.LabelHeaderAddresss.Text = Resources.Global.Global_Address.ToUpperInvariant();
            this.LabelStreet1.Text = Resources.Pages.Swarm.AddPerson_Street1PO;
            this.LabelStreet2.Text = Resources.Pages.Swarm.AddPerson_Street2;
            this.LabelPostalCode.Text = Resources.Global.Global_PostalCode;
            this.LabelCity.Text = Resources.Global.Global_City;
            this.LabelGeographyDetected.Text = Resources.Pages.Swarm.AddPerson_GeographyDetected;
            this.LabelHeaderStatData.Text = Resources.Pages.Swarm.AddPerson_StatisticalData;
            this.LabelDateOfBirth.Text = Resources.Global.Global_DateOfBirth;
            this.LabelLegalGender.Text = Resources.Pages.Swarm.AddPerson_LegalGender;

            this.TextDateOfBirth.Attributes["placeholder"] = Global.Global_DateFormatShortReadable;
            this.TextName.Attributes["placeholder"] = "Joe Smith";
            this.TextMail.Attributes["placeholder"] = "joe@example.com";
            this.TextPhone.Attributes["placeholder"] = "+1 263 151 1341";
            this.TextStreet1.Attributes["placeholder"] = "78 West Avenue";
            this.TextPostal.Attributes["placeholder"] = "12345";
        }

        protected void ButtonSubmit_Click (object sender, EventArgs e)
        {
            DateTime dateOfBirth = new DateTime (1800, 1, 1); // null equivalent

            if (this.TextDateOfBirth.Text.Length > 0)
            {
                dateOfBirth = DateTime.Parse (this.TextDateOfBirth.Text);
            }

            string street = this.TextStreet1.Text;
            if (!string.IsNullOrEmpty (this.TextStreet2.Text))
            {
                street += "|" + this.TextStreet2.Text;
            }

            Person newPerson = Person.Create (this.TextName.Text, this.TextMail.Text, string.Empty, this.TextPhone.Text,
                street, this.TextPostal.Text, this.TextCity.Text, this.DropCountries.SelectedValue, dateOfBirth,
                (PersonGender)Enum.Parse(typeof(PersonGender), this.DropGenders.SelectedValue));

            Membership newMembership = Membership.Create (newPerson, CurrentOrganization, DateTime.Today.AddYears (1));

            OutboundComm.CreateMembershipLetter (ParticipantMailType.MemberAddedWelcome, newMembership, CurrentUser);

            SwarmopsLogEntry logEntry = SwarmopsLog.CreateEntry (newPerson,
                new Swarmops.Logic.Support.LogEntries.PersonAddedLogEntry (newMembership, CurrentUser));

            logEntry.CreateAffectedObject (newMembership);
            logEntry.CreateAffectedObject (CurrentUser);

            // Clear form and make way for next person

            this.TextName.Text = string.Empty;
            this.TextStreet1.Text = string.Empty;
            this.TextStreet2.Text = string.Empty;
            this.TextMail.Text = string.Empty;
            this.TextPhone.Text = string.Empty;
            this.TextPostal.Text = string.Empty;
            this.TextCity.Text = string.Empty;
            this.TextDateOfBirth.Text = string.Empty;
            this.DropGenders.SelectedValue = "Unknown";

            this.TextName.Focus();
            this.LiteralLoadAlert.Text = Resources.Pages.Swarm.AddPerson_PersonSuccessfullyRegistered;
        }
    }
}