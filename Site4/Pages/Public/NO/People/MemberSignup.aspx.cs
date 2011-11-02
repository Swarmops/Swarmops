using System;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

public partial class Pages_Public_NO_MemberSignup : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{

		if (!Page.IsPostBack)
		{
			// Populate Countries

			Countries countries = Countries.GetAll();

			foreach (Country country in countries)
			{
				DropCountries.Items.Add(new ListItem(country.Code + " " + country.Name, country.Code));
			}

			DropCountries.Items.FindByValue("NO").Selected = true;
			Wizard.ActiveStepIndex = 0;
			Wizard.SideBarStyle.CssClass = "Invisible";

            if (Request.UrlReferrer != null)
            {
                LabelReferrer.Text = Request.UrlReferrer.AbsoluteUri;
            }
            else
            {
                LabelReferrer.Text = string.Empty;
            }
		}
	}


	protected void Wizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
	{
		// Validate data for the various steps, or (on page 4) finalize the membership.


		// -------------------------------------
		// VALIDATE PAGE: CHOICE OF ORGANIZATION 
		// -------------------------------------

		if (e.CurrentStepIndex == 0)
		{
			// Validate the choice of organization.

			if (this.RadioParty.Checked == false && this.RadioYouthLeague.Checked == false)
			{
                /*
				this.LabelOrganizationError.Text = "Du m&aring;ste v&auml;lja en organisation!";
				e.Cancel = true;*/
			}
			else
			{
				this.LabelOrganizationError.Text = string.Empty; // Clear on success
			}

			// If youth league is selected, make sure that the payment page is skipped.

			if (this.RadioYouthLeague.Checked == true)
			{
				this.LabelSmsMessageText.Text = "PP UNG";
				this.LabelOrganization.Text = "Ung Pirat (och Piratpartiet)";
				Wizard.ActiveStepIndex = 2; // Remove this on 2007 expiry

				// Set the focus to the name control on the member details page.

				this.TextName.Focus();
			}

            else if (this.RadioParty.Checked == true)
            {
                this.LabelSmsMessageText.Text = "PP MEDLEM";
                this.LabelOrganization.Text = "Piratpartiet";

                Wizard.ActiveStepIndex = 2; // Zero cost payment

                // Set the focus to the payment code control.

                this.TextPaymentCode.Focus();
            }
		}


		// ---------------------------
		// VALIDATE PAGE: PAYMENT CODE 
		// ---------------------------
		
		if (e.CurrentStepIndex == 1)
		{
			// Validate the payment code page.

			PaymentCode code = null;

			try
			{
				code = PaymentCode.FromCode(this.TextPaymentCode.Text.Trim());
			}
			catch (Exception)
			{
				// If we can't get the payment code, it will remain null, which is fine.
			}

			if (code == null || code.Claimed)
			{
				this.LabelPaymentError.Text = "Hittar inte kvittokoden!";
				e.Cancel = true;
			}
			else
			{
				this.LabelPaymentError.Text = string.Empty; // Clear on success

				if (code.IssuedToPhoneNumber.Length > 0)
				{
					this.TextPhone.Text = code.IssuedToPhoneNumber;
				}
			}

			// Set the focus to the name control on the member details page.

			this.TextName.Focus();
		}


		// -----------------------------
		// VALIDATE PAGE: MEMBER DETAILS
		// -----------------------------

		if (e.CurrentStepIndex == 2)
		{
			// Validate member details. This is going to be quite a long validation.

			// Start by clearing all the error labels, for readability later.

			this.LabelNameError.Text = string.Empty;
			this.LabelStreetError.Text = string.Empty;
			this.LabelPostalError.Text = string.Empty;
			this.LabelPhoneError.Text = string.Empty;
			this.LabelEmailError.Text = string.Empty;
			this.LabelBirthdateError.Text = string.Empty;
			this.LabelGenderError.Text = string.Empty;

			// Check for errors.

			// Name

			if (this.TextName.Text.Length < 3 || !this.TextName.Text.Contains(" "))
			{
				this.LabelNameError.Text = "Skriv ditt namn";
				e.Cancel = true;
			}

			// Street

			if (this.TextStreet.Text.Length < 4)
			{
				this.LabelStreetError.Text = "Skriv gatuadress";
				e.Cancel = true;
			}

			// Postal code & city -- also validate postal code length for given country

			if (this.TextPostal.Text.Length < 4)
			{
				this.LabelPostalError.Text = "Skriv postnummer";
				e.Cancel = true;
			}
			else
			{
				Country country = Country.FromCode(DropCountries.SelectedValue);

				if (country.PostalCodeLength != 0)
				{
					string postalCode = this.TextPostal.Text;
					postalCode = postalCode.Replace(" ", "");
					if (postalCode.Length != country.PostalCodeLength)
					{
						this.LabelPostalError.Text = "Felaktigt postnr";
						e.Cancel = true;
					}
				}
			}
			if (this.TextCity.Text.Length < 3 && this.LabelPostalError.Text.Length < 2)
			{
				this.LabelPostalError.Text = "Skriv postort";
				e.Cancel = true;
			}

			// Phone number

			if (this.TextPhone.Text.Length < 7)
			{
				this.LabelPhoneError.Text = "Skriv telefonnummer";
				e.Cancel = true;
			}

			// Email

			/*AddressValidationResult addressValidationResult =
				PirateWeb.Logic.Special.Mail.AddressValidator.Validate (this.TextEmail.Text);*/

			if (this.TextEmail.Text.Length < 5)
			{
				this.LabelEmailError.Text = "Skriv email";
				e.Cancel = true;
			}/*
			else switch (addressValidationResult)
			{
				case AddressValidationResult.Unknown:
				case AddressValidationResult.Valid:
					// Address ok
					break;
				case AddressValidationResult.BadSyntax:
					this.LabelEmailError.Text = "Ogiltig emailadress";
					e.Cancel = true;
					break;
				case AddressValidationResult.ServerInvalid:
					this.LabelEmailError.Text = "Mailservern finns inte";
					e.Cancel = true;
					break;
				case AddressValidationResult.AccountInvalid:
					this.LabelEmailError.Text = "Mailkontot finns inte";
					e.Cancel = true;
					break;
				default:
					throw new NotImplementedException ("Not implemented AddressValidationResult: " + addressValidationResult);
			}*/


			// Birthdate

			try
			{
				int day = Convert.ToInt32(this.TextBirthDay.Text);
				int year = Convert.ToInt32(this.TextBirthYear.Text);
				int month = Convert.ToInt32(this.DropBirthMonths.SelectedValue);

				DateTime test = new DateTime(year, month, day);

				if (test > DateTime.Now)
				{
					throw new Exception("No, you can't be born on a future date.");
				}

				if (test < DateTime.Now.AddYears (-125))
				{
					throw new Exception("And you're not over 125 years old, either.");
				}
			}
			catch (Exception)
			{
				this.LabelBirthdateError.Text = "V&auml;lj f&ouml;delsedatum";
				e.Cancel = true;
			}

			// Gender

			try
			{
				PersonGender gender = (PersonGender)Enum.Parse(typeof(PersonGender), this.DropGenders.SelectedValue);

				if (gender == PersonGender.Unknown)
				{
					throw new Exception(); // Gender not selected - just throw something to produce the error message.
				}
			}
			catch (Exception)
			{
				this.LabelGenderError.Text = "V&auml;lj k&ouml;n";
				e.Cancel = true;
			}


			if (!e.Cancel)
			{
				this.TextPassword1.Focus();
			}
		}



		// ---------------------------------
		// VALIDATE PAGE: PIRATEWEB PASSWORD
		// ---------------------------------

		if (e.CurrentStepIndex == 3)
		{
			string password1 = this.TextPassword1.Text;
			string password2 = this.TextPassword2.Text;

			if (password1 != password2)
			{
				this.LabelPasswordError.Text = "Skriv samma l&ouml;sen i b&aring;da rutorna!";
				e.Cancel = true;
			}
			else if (password1 == string.Empty)
			{
				this.LabelPasswordError.Text = "V&auml;lj ett l&ouml;senord";
				e.Cancel = true;
			}
			else if (password1.Length < 5)
			{
				this.LabelPasswordError.Text = "L&ouml;senordet &auml;r f&ouml;r kort";
				e.Cancel = true;
			}

			if (e.Cancel == true)
			{
				this.TextPassword1.Focus(); // Set focus to first (now empty) text box
			}


			if (e.Cancel == false)
			{

				// This is the final page. When we get here, all data is good. This code
				// creates and commits the member.

				// If youthOrg is true, then the person opted for youth league membership.

				bool youthOrgSelected = this.RadioYouthLeague.Checked;

				PaymentCode paymentCode = null;

                /*
                 * // Payments disabled
                 * 
				if (!youthOrgSelected) 
				{
					paymentCode = PaymentCode.FromCode(this.TextPaymentCode.Text);
				}*/

				string name = this.TextName.Text;
				string password = this.TextPassword1.Text;
				string email = this.TextEmail.Text;
				string phone = this.TextPhone.Text;
				string street = this.TextStreet.Text;
				string postal = this.TextPostal.Text;
				string city = this.TextCity.Text;
				string countryCode = this.DropCountries.SelectedValue;

				int birthDay = Convert.ToInt32(this.TextBirthDay.Text);
				int birthMonth = Convert.ToInt32(this.DropBirthMonths.SelectedValue);
				int birthYear = Convert.ToInt32(this.TextBirthYear.Text);

				DateTime birthdate = new DateTime(birthYear, birthMonth, birthDay);
				PersonGender gender = (PersonGender)Enum.Parse(typeof(PersonGender), this.DropGenders.SelectedValue);

				// We are ready to create the new person.

				Person person = Person.Create(name, email, password, phone, street, postal, city, countryCode, birthdate, gender);

				// The creation resolves the appropriate geography, so we can determine the correct
				// organization to join the person to.

				// Add memberships, as appropriate.

				Organization youthOrg = null;
				this.LabelMemberOfOrganizations.Text = string.Empty;

				if (youthOrgSelected)
				{
                    youthOrg = Organizations.GetMostLocalOrganization(person.GeographyId, Organization.UPSEid);
					person.AddMembership(youthOrg.Identity, DateTime.Now.AddYears(1));
					this.LabelMemberOfOrganizations.Text = "<b>" + Server.HtmlEncode(youthOrg.Name) + "</b> och<br/>";
				}

                //TODO: hardcoded norway
                person.AddMembership(Organization.PPNOid, DateTime.Now.AddYears(1));
				this.LabelMemberOfOrganizations.Text += "<b>Piratpartiet NO</b>";

				// Create events.

				if (youthOrgSelected)
				{
					Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SignupPage, EventType.AddedMember,
                        person.Identity, youthOrg.Identity, person.GeographyId, person.Identity, 0, youthOrg.Identity.ToString() + " " + Organization.PPNOid.ToString() + "," + Request.UserHostAddress + "," + this.LabelReferrer.Text);
                    PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MemberAdd, "New member joined organizations " + youthOrg.NameShort + " and Piratpartiet SE.", "The self-signup came from IP address " + Request.UserHostAddress + ".");
				}
				else
				{
					Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SignupPage, EventType.AddedMember,
                        person.Identity, Organization.PPNOid, person.GeographyId, person.Identity, 0,  Organization.PPNOid.ToString() + "," + Request.UserHostAddress + "," + this.LabelReferrer.Text);
                    PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MemberAdd, "New member joined Piratpartiet NO.", "The self-signup came from IP address " + Request.UserHostAddress + ".");
                }

				// Claim the payment.

				if (paymentCode != null)
				{
					paymentCode.Claim(person);
				}
			}

		}
	}


	protected void Wizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
	{
		// Once we get here, we're all done, so just redirect to the home page.

		Response.Redirect("http://www.piratpartiet.biz");
	}


	protected void ButtonAbort_Click(object sender, EventArgs e)
	{
		Response.Redirect("http://www.piratpartiet.biz");
	}
}
