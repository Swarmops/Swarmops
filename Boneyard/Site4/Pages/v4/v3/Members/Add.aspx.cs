using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Basic.Types;
using Activizr.Basic.Enums;
using Activizr.Interface.Localization;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using System.Globalization;


public partial class Pages_Members_Add : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        Person viewingPerson = _currentUser;
        Authority authority = _authority;

        if (!Page.IsPostBack)
        {
            // Localize


            this.LabelBankMessage.Text = "(optional)";
            this.LabelBankAccountMessage.Text = "(optional)";
            this.LabelPersonalNumberMessage.Text = "(optional)";

            this.LabelNameMessage.Text = string.Empty;
            this.LabelBirthDateMessage.Text = "Like 1972-Jan-21";
            this.LabelEmailMessage.Text = string.Empty;
            this.LabelPhoneMessage.Text = string.Empty;
            this.LabelPostalMessage.Text = string.Empty;
            this.LabelStreetMessage.Text = string.Empty;
            this.LabelGenderMessage.Text = string.Empty;


            // Populate organizations

            Organizations organizations = authority.GetOrganizations(RoleTypes.AllRoleTypes ).ExpandAll();

            foreach (Organization org in organizations)
            {
                if (org.AcceptsMembers)
                {
                    this.DropOrganizations.Items.Add(new ListItem(org.Name, org.Identity.ToString()));
                }
            }

            // Populate countries

            Countries countries = Countries.GetAll();

            foreach (Country country in countries)
            {
                DropCountries.Items.Add(new ListItem(country.Code + " " + country.Name, country.Code));
            }

            // Set default country

            Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));

            DropCountries.Items.FindByValue(selectedOrg.DefaultCountry.Code).Selected = true;


            this.TextName.Focus();
        }
    }


    protected void ButtonAdd_Click (object sender, EventArgs e)
    {
        Person viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
        DateTime birthDate = DateTime.MinValue;
        string name = this.TextName.Text.Trim();
        string email = this.TextEmail.Text.Trim();
        string phone = this.TextPhone.Text.Trim();
        string street = this.TextStreet.Text.Trim();
        string postalCode = this.TextPostalCode.Text.Replace(" ", "").Trim();
        string city = this.TextCity.Text.Trim();
        string countryCode = this.DropCountries.SelectedValue;
        string personalNumber = this.TextPersonalNumber.Text.Trim();
        string bankName = this.TextBank.Text.Trim();
        string bankAccount = this.TextBankAccount.Text.Trim();
        PersonGender gender = (PersonGender)Enum.Parse(typeof(PersonGender), this.DropGender.SelectedValue);

        Organization organization = Organization.FromIdentity(Convert.ToInt32(DropOrganizations.SelectedValue));

        // Validate data

        bool dataGood = true;

        // Birthdate
        string[] formats = new string[] { "yyMMdd", "yyyyMMdd", "yy-MM-dd", 
                                            "yyyy-MM-dd", "yyyy-MMM-dd" };

        if (!DateTime.TryParseExact(this.TextBirthDate.Text.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
        {
            this.LabelBirthDateMessage.Text = "Invalid - write like 1972-Jan-21";
            this.LabelBirthDateMessage.CssClass = "ErrorMessage";
            dataGood = false;
        }
        else
        {
            this.LabelBirthDateMessage.Text = string.Empty;
            this.LabelBirthDateMessage.CssClass = "EditComment";
        }

        // Gender

        if (gender == PersonGender.Unknown)
        {
            dataGood = false;
            this.LabelGenderMessage.Text = "Select Gender!";
            this.LabelGenderMessage.CssClass = "ErrorMessage";
        }
        else
        {
            this.LabelGenderMessage.Text = string.Empty;
        }

        // Name

        if (name.Length < 0)
        {
            dataGood = false;
            this.LabelNameMessage.Text = "Must write name!";
            this.LabelNameMessage.CssClass = "ErrorMessage";
        }
        else if (!name.Contains(" "))
        {
            dataGood = false;
            this.LabelNameMessage.Text = "Need full name!";
            this.LabelNameMessage.CssClass = "ErrorMessage";
        }
        else
        {
            this.LabelNameMessage.Text = string.Empty;
        }

        // Street address

        if (street.Length < 4)
        {
            dataGood = false;
            this.LabelStreetMessage.Text = "Need street address!";
            this.LabelStreetMessage.CssClass = "ErrorMessage";
        }
        else
        {
            this.LabelStreetMessage.Text = string.Empty;
        }

        // Postal code, city

        int supposedPostalCodeLength = Country.FromCode(this.DropCountries.SelectedValue).PostalCodeLength;

        if (supposedPostalCodeLength != 0 && supposedPostalCodeLength != postalCode.Length)
        {
            dataGood = false;
            this.LabelPostalMessage.Text = "Bad postal code!";
            this.LabelPostalMessage.CssClass = "ErrorMessage";
        }
        else if (city.Length < 2)
        {
            dataGood = false;
            this.LabelPostalMessage.Text = "Need city!";
            this.LabelPostalMessage.CssClass = "ErrorMessage";
        }
        else
        {
            this.LabelPostalMessage.Text = string.Empty;
        }

        // Phone

        if (phone.Length < 6)
        {
            dataGood = false;
            this.LabelPhoneMessage.Text = "Need phone number!";
            this.LabelPhoneMessage.CssClass = "ErrorMessage";
        }
        else
        {
            this.LabelPhoneMessage.Text = string.Empty;
        }

        // Email

        /* -- this validator has been decommissioned

		AddressValidationResult validationResult = AddressValidator.Validate (email);

		if (validationResult != AddressValidationResult.Valid && validationResult != AddressValidationResult.Unknown && validationResult != AddressValidationResult.AccountInvalid)
		{
			dataGood = false;
			this.LabelEmailMessage.CssClass = "ErrorMessage";

			switch (validationResult)
			{
				case AddressValidationResult.ServerInvalid:
					this.LabelEmailMessage.Text = "Invalid email: Bad domain/host";
					break;
				case AddressValidationResult.AccountInvalid: // Won't come here no more - branch disabled in conditional above
					this.LabelEmailMessage.Text = "Invalid email: No such mailbox";
					break;
				case AddressValidationResult.BadSyntax:
					this.LabelEmailMessage.Text = "Invalid email: Syntax error";
					break;
			}
		}
		else
		{
			this.LabelEmailMessage.Text = string.Empty;
		}
         * 
        */

        // TODO: Check the rest of the data fields

        if (!dataGood)
        {
            Page.ClientScript.RegisterStartupScript(typeof(Page), "SuccessMessage", string.Empty, true);
        }
        else
        {
            string randomPassword = Activizr.Logic.Security.Authentication.CreateRandomPassword(8);

            // Create the person

            Person person = Person.Create(name, email, randomPassword, phone, street, postalCode, city, countryCode,
                                          birthDate, gender);

            // Add optional data

            if (personalNumber.Length > 0)
            {
                person.PersonalNumber = personalNumber;
            }

            if (bankAccount.Length > 0)
            {
                person.BankName = bankName;
                person.BankAccount = bankAccount;
            }

            // Create the membership

            DateTime expiry = DateTime.Now.AddYears(1).Date;

            person.AddMembership(organization, expiry);

            // Create memberships in master organizations

            Organizations masterOrgs = organization.GetMasterOrganizations();

            foreach (Organization masterOrg in masterOrgs)
            {
                person.AddMembership(masterOrg, expiry);
            }

            // Send message about randomized password

            // TODO: Localize

            string mailBody = "A PirateWeb administrator has manually added you to the organization \"" +
                              organization.Name + "\". " +
                              "You are now a member of this organization. In order to access your member profile, you will need to log " +
                              "on to the PirateWeb site at pirateweb.net with your name or email address, together with the following randomized password:\r\n\r\n" +
                              "Password: " + randomPassword + "\r\n\r\n" +
                              "You will be able to change this password once you have logged onto PirateWeb.\r\n\r\n" +
                              "For more information, please contact the organization you just joined. Most welcome to the worldwide pirate movement.\r\n\r\n";

            person.SendNotice("Your PirateWeb password", mailBody, organization.OrganizationId);

            Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.AddedMember,
                                               Convert.ToInt32(HttpContext.Current.User.Identity.Name),
                                               organization.Identity, person.GeographyId, person.Identity, 0,
                                               string.Empty);

            PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MemberAdd,
                            "Added manually to " + organization.NameShort,
                            "The member was added manually by " + viewingPerson.Name + " (#" +
                            viewingPerson.Identity.ToString() + ")");

            string masterOrgString = string.Empty;

            foreach (Organization masterOrg in masterOrgs)
            {
                /*
				 *  -- comment out the extra event when somebody is added to UP
				 * 
				PirateWeb.Logic.Support.PWEvents.CreateEvent (EventSource.PirateWeb, EventType.AddedMembership,
				Convert.ToInt32(HttpContext.Current.User.Identity.Name),
				masterOrg.Identity, person.GeographyId, person.Identity, 0, string.Empty);*/

                masterOrgString += ", " + masterOrg.Name;
            }

            if (masterOrgString.Length > 0)
            {
                masterOrgString = " and [" + masterOrgString.Substring(2) + "]";
            }

            Page.ClientScript.RegisterStartupScript(typeof(Page), "SuccessMessage",
                                                    "alert ('The member [" +
                                                    HttpUtility.HtmlEncode(person.Name.Replace("'", "''")) +
                                                    "] was added to [" +
                                                    HttpUtility.HtmlEncode(organization.Name.Replace("'", "''")) + "]" +
                                                    masterOrgString.Replace("'", "''") + ".');", true);

            // Clear the text boxen, reset the drop fields

            this.TextName.Text = string.Empty;
            this.TextEmail.Text = string.Empty;
            this.TextPhone.Text = string.Empty;
            this.TextPostalCode.Text = string.Empty;
            this.TextStreet.Text = string.Empty;
            this.TextCity.Text = string.Empty;
            this.TextBirthDate.Text = string.Empty;
            this.TextPersonalNumber.Text = string.Empty;
            this.TextBank.Text = string.Empty;
            this.TextBankAccount.Text = string.Empty;

            this.DropGender.SelectedIndex = 0;
            this.TextName.Focus();

            // Reset to default country

            Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
            this.DropCountries.SelectedItem.Selected = false;
            this.DropCountries.Items.FindByValue(selectedOrg.DefaultCountry.Code).Selected = true;
        }
    }


    protected void DropOrganizations_SelectedIndexChanged (object sender, EventArgs e)
    {
        // Set default country

        Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));

        DropCountries.SelectedItem.Selected = false;
        DropCountries.Items.FindByValue(selectedOrg.DefaultCountry.Code).Selected = true;
    }
}