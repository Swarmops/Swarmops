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


public partial class Pages_Account_BasicDetails : PageV4Base
{
	protected void Page_Load(object sender, EventArgs e)
	{
        Person viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));

		this.PersonDetails.Person = viewingPerson;

		/*
		// Populate textboxen

		this.TextBirthDate.Text = person.Birthdate.ToShortDateString();
		this.TextCity.Text = person.CityName;
		this.TextCountry.Text = person.Country.Name;
		this.TextEmail.Text = person.Email;
		this.TextName.Text = person.Name;
		this.TextNumber.Text = person.PersonId.ToString();
		this.TextPhone.Text = person.PhoneNumber;
		this.TextPostalCode.Text = person.PostalCode;
		this.TextCity.Text = person.CityName;
		this.TextStreet.Text = person.Street;

		if (person.OptionalData.ContainsKey (PersonOptionalDataKey.PersonalNumber))
		{
			this.TextPersonalNumber.Text = person.OptionalData[PersonOptionalDataKey.PersonalNumber];
		}

		if (person.OptionalData.ContainsKey(PersonOptionalDataKey.BankAccount))
		{
			this.TextBank.Text = person.OptionalData[PersonOptionalDataKey.BankName];
			this.TextBankAccount.Text = person.OptionalData[PersonOptionalDataKey.BankAccount];
		}

		// Fix links

		this.LinkMemberships.NavigateUrl = "Memberships.aspx?PersonId=" + personId.ToString();
		this.LinkRolesResponsibilities.NavigateUrl = "RolesResponsibilities.aspx?PersonId=" + personId.ToString();
		this.LinkLog.NavigateUrl = "MaintenanceLog.aspx?PersonId=" + personId.ToString();

		// Cheat

		this.LinkLog.Visible = false;

		// Localize

		this.LabelBasicDetailsHeader.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.Header", "Member Details - Basic Details");

		this.LinkMemberships.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.Menu.Memberships", "Memberships");
		this.LabelBasicDetails.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.Menu.BasicDetails", "Basic Details");
		this.LinkRolesResponsibilities.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.Menu.RolesResponsibilities", "Roles and Responsibilities");
		this.LinkLog.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.Menu.MaintenanceLog", "Maintenance Log");

		this.LabelNumber.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.MemberNumber", "Member#");
		this.LabelBirthdate.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.Birthdate", "Birth date");
		this.LabelCountry.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.Country", "Country");
		this.LabelEmail.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.Email", "Email");
		this.LabelName.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.Name", "Name");
		this.LabelPersonalNumber.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.PersonalNumber", "Personal Number (SE)");
		this.LabelPhone.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.MemberNumber", "Phone number");
		this.LabelPostal.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.PostalDetails", "Postal code, City");
		this.LabelStreet.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.Street", "Street address");
		this.LabelBank.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.Bank", "Bank name");
		this.LabelBankAccount.Text = LocalizationManager.GetLocalString("Interface.Pages.Member.BasicDetails.BankAccount", "Bank account#");*/
	}
}
