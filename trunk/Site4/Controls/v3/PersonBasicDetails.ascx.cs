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
using Activizr.Basic.Exceptions;
using Activizr.Interface.Localization;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Logic.Security;
using Activizr.Logic.Special.Mail;


public partial class Controls_PersonBasicDetails : System.Web.UI.UserControl
{
    Authority _authority;

    protected void Page_Load (object sender, EventArgs e)
    {
        _authority = Person.FromIdentity(Convert.ToInt32(HttpContext.Current.User.Identity.Name)).GetAuthority();

        if (!Page.IsPostBack)
        {
            // Initialize the fields from the Person object

            this.ButtonSaveChanges.Text = this.GetLocalResourceObject("Interface.Controls.Common.ButtonSaveChanges").ToString()/*Save Changes*/;

            // Member number

            this.TextMemberNumber.Text = Person.Identity.ToString();
            this.LabelMemberNumber.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.MemberNumber").ToString()/*Member #*/ + "&nbsp;&nbsp;";

            // Name

            this.TextName.Text = Person.Name;
            this.TextName.Attributes.Add("onkeypress", "OnTextChange (this);");
            this.TextName.Attributes.Add("onchange", "OnTextChange (this);");
            this.LabelName.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.Name").ToString()/*Name*/ + "&nbsp;&nbsp;";

            // Street

            this.TextStreet.Text = Person.Street;
            this.TextStreet.Attributes.Add("onkeypress", "OnTextChange (this);");
            this.TextStreet.Attributes.Add("onchange", "OnTextChange (this);");
            this.LabelStreet.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.Street").ToString()/*Street*/ + "&nbsp;&nbsp;";

            // Postal code, city

            this.TextPostalCode.Text = Person.PostalCode;
            this.TextPostalCode.Attributes.Add("onkeypress", "OnTextChangePostals();");
            this.TextCity.Text = Person.CityName;
            this.TextCity.Attributes.Add("onkeypress", "OnTextChangePostals();");
            this.TextCity.Attributes.Add("onchange", "OnTextChangePostals();");
            this.LabelPostal.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.Postal").ToString()/*Postal code, CityName*/ + "&nbsp;&nbsp;";

            this.DropDownMunicipalities.Attributes.Add("onchange", "OnChangeMunicipality();");


            // Countries

            this.DropCountries.Attributes.Add("onchange", "OnChangeCountries();");
            this.LabelCountry.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.Country").ToString()/*Country*/ + "&nbsp;&nbsp;";

            Cities cities = Cities.FromPostalCode(Person.PostalCode.Replace(" ", ""), person.Country);
            int personGeography = person.GeographyId;
            DropDownMunicipalities.Items.Clear();
            bool foundCurrent = false;

            foreach (City city in cities)
            {
                Geographies subGeo = city.Geography.GetTree();
                foreach (Geography geo in subGeo)
                {
                    // ignore  if it is not a leaf
                    if (geo.ChildrenCount == 0)
                    {
                        DropDownMunicipalities.Items.Add(new ListItem(geo.Name, geo.GeographyId.ToString()));

                        if (geo.GeographyId == person.GeographyId)
                        {
                            foundCurrent = true;
                        }
                    }
                }
            }


            if (!foundCurrent)
            {
                DropDownMunicipalities.Items.Add(new ListItem(Person.Geography.Name, Person.GeographyId.ToString()));
            }

            DropDownMunicipalities.SelectedValue = Person.GeographyId.ToString();


            // Populate countries

            Countries countries = Countries.GetAll();

            foreach (Country country in countries)
            {
                DropCountries.Items.Add(new ListItem(country.Code + " " + country.Name, country.Code));
            }

            DropCountries.Items.FindByValue(Person.Country.Code).Selected = true;

            // Email

            this.TextEmail.Text = Person.Email;
            this.TextEmail.Attributes.Add("onkeypress", "OnTextChange (this);");
            this.TextEmail.Attributes.Add("onchange", "OnTextChange (this);");
            this.LabelEmail.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.Email").ToString() + "&nbsp;&nbsp;";

            if (Person.MailUnreachable)
            {
                this.LabelEmailMessage.Text = "Unreachable!";
                this.LabelEmailMessage.CssClass = "ErrorMessage";
            }

            // Party Email

            this.TextPartyEmail.Text = Person.PartyEmail;
            if (Person.PartyEmail.Length > 2)
            {
                this.ButtonSendNewPassword.Enabled = true;
            }
            else
            {
                this.ButtonSendNewPassword.Visible = false;
            }

            this.LabelPartyEmail.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.PartyEmail").ToString()/*Party Email*/ + "&nbsp;&nbsp;";

            // Phone

            this.TextPhone.Text = Person.Phone;
            this.TextPhone.Attributes.Add("onkeypress", "OnTextChange(this);");
            this.TextPhone.Attributes.Add("onchange", "OnTextChange(this);");
            this.LabelPhone.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.Phone").ToString()/*Phone #*/ + "&nbsp;&nbsp;";

            // Birthdate

            this.TextBirthdate.Text = Person.Birthdate.ToShortDateString();
            this.TextBirthdate.Attributes.Add("onkeypress", "OnTextChange(this);");
            this.TextBirthdate.Attributes.Add("onchange", "OnTextChange(this);");
            this.LabelBirthdate.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.Birthdate").ToString()/*Birthdate*/ + "&nbsp;&nbsp;";

            // Gender

            this.DropGenders.SelectedValue = Person.Gender.ToString();
            this.DropGenders.Attributes.Add("onclick", "OnTextChange(this);");

            // Handle
            try
            {
                this.TextHandle.Text = Person.Handle;
            }
            catch (Exception ex)
            {
                this.TextHandle.Text = "Connection Error";
                this.TextHandle.ToolTip = ex.Message ;
                this.TextHandle.Enabled = false;
            }

            this.TextHandle.Attributes.Add("onkeypress", "OnTextChange(this);");
            this.TextHandle.Attributes.Add("onchange", "OnTextChange(this);");
            this.LabelHandle.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.ForumHandle").ToString()/*Forum Handle*/ + "&nbsp;&nbsp;";

            // Personal Number

            this.TextPersonalNumber.Text = Person.PersonalNumber;
            this.TextPersonalNumber.Attributes.Add("onkeypress", "OnTextChange (this);");
            this.TextPersonalNumber.Attributes.Add("onchange", "OnTextChange (this);");
            this.LabelPersonalNumber.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.PersonalNumber").ToString()/*Personal #*/ + "&nbsp;&nbsp;";

            // Bank name

            this.TextBankName.Text = Person.BankName;
            this.TextBankName.Attributes.Add("onkeypress", "OnTextChange (this);");
            this.TextBankName.Attributes.Add("onchange", "OnTextChange (this);");
            this.LabelBankName.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.BankName").ToString()/*Bank Name*/ + "&nbsp;&nbsp;";

            // Bank clearing#

            this.TextBankClearing.Text = Person.BankClearing;
            this.TextBankClearing.Attributes.Add("onkeypress", "OnTextChange (this);");
            this.TextBankClearing.Attributes.Add("onchange", "OnTextChange (this);");
            // this.LabelBankClearing.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.BankName").ToString()/*Bank Name*/ + "&nbsp;&nbsp;";

            // Bank account

            this.TextBankAccount.Text = Person.BankAccount;
            this.TextBankAccount.Attributes.Add("onkeypress", "OnTextChange (this);");
            this.TextBankAccount.Attributes.Add("onchange", "OnTextChange (this);");
            this.LabelBankAccount.Text = this.GetLocalResourceObject("Interface.Controls.EditPerson.BankAccount").ToString()/*Bank Account*/ + "&nbsp;&nbsp;";

            // Crypto key

            this.TextCryptoFingerprint.Enabled = false;
            this.TextCryptoFingerprint.ReadOnly = true;

            if (Person.CryptoFingerprint.Length > 4)
            {
                this.TextCryptoFingerprint.Text = Person.CryptoFingerprint;
            }
            else
            {
                this.TextCryptoFingerprint.Text = "--";
            }

            // T-Shirt Size

            this.TextTShirtSize.Enabled = false;
            this.TextTShirtSize.ReadOnly = true;
            this.TextTShirtSize.Text = Person.TShirtSize;


            // If we are looking at ourselves, enable the bank fields, which are otherwise disabled

            if (Person.Identity == Convert.ToInt32(HttpContext.Current.User.Identity.Name))
            {
                this.TextBankName.ReadOnly = false;
                this.TextBankClearing.ReadOnly = false;
                this.TextBankAccount.ReadOnly = false;
            }
            else
            {
                if (!_authority.HasAnyPermission(Permission.CanEditPeople))
                {
                    this.TextBirthdate.ReadOnly = true;
                    this.DropGenders.Enabled = false;
                    this.TextCity.ReadOnly = true;
                    this.TextEmail.ReadOnly = true;
                    this.TextHandle.ReadOnly = true;
                    this.TextMemberNumber.ReadOnly = true;
                    this.TextName.ReadOnly = true;
                    this.TextPartyEmail.ReadOnly = true;
                    this.TextPersonalNumber.ReadOnly = true;
                    this.TextPhone.ReadOnly = true;
                    this.TextPostalCode.ReadOnly = true;
                    this.TextStreet.ReadOnly = true;
                    this.DropCountries.Enabled = false;
                    this.DropDownMunicipalities.Enabled = false;
                    this.ButtonSendNewPassword.Enabled = false;
                    this.ButtonSaveChanges.Visible = false;
                }
            }
        }

        if (_authority.HasPermission(Permission.CanEditMailDB, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization))
        {
            ButtonDeleteMail.Visible = true;
            if (TextPartyEmail.Text.Trim() != "")
                ButtonDeleteMail.Enabled = true;
            else
                ButtonDeleteMail.Enabled = false;
        }

    }


    public event EventHandler DataChanged;


    public Person Person
    {
        get
        {
            return person;
        }
        set
        {
            person = value;
        }
    }

    private Person person;



    protected void ButtonSendNewPassword_Click (object sender, EventArgs e)
    {
        Person viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));

        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.RefreshEmailAccount, viewingPerson.Identity, Organization.PPSEid,
                                                        this.Person.GeographyId, this.Person.Identity, 0, string.Empty);
    }


    private void GeographyFromPostalCode ()
    {
        Cities cities = Cities.FromPostalCode(this.TextPostalCode.Text.Replace(" ", ""), this.DropCountries.SelectedValue);
        if (cities.Count == 0)
        {
            LabelPostalMessage.Text = "Unknown Code";
        }
        else
        {
            LabelPostalMessage.Text = "";
            int personGeography = person.GeographyId;
            this.TextCity.Text = cities[0].Name;
            DropDownMunicipalities.Items.Clear();

            foreach (City city in cities)
            {
                Geographies subGeo = city.Geography.GetTree();
                foreach (Geography geo in subGeo)
                {
                    // ignore  if it is not a leaf
                    if (geo.ChildrenCount == 0)
                    {
                        DropDownMunicipalities.Items.Add(new ListItem(geo.Name, geo.GeographyId.ToString()));

                        if (geo.GeographyId == person.GeographyId)
                        {
                            DropDownMunicipalities.SelectedValue = Person.GeographyId.ToString();
                        }
                    }
                }
            }
        }
    }

    protected void TextPostalCode_Change (object sender, EventArgs e)
    {
        GeographyFromPostalCode();
    }


    protected void ButtonSaveChanges_Click (object sender, EventArgs e)
    {
        // Check for dirty fields, one by one
        int currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
        Person currentUser = Person.FromIdentity(currentUserId);

        AuditedPerson aperson = AuditedPerson.FromPerson(Person, currentUser, "PersonBasicDetails");

        if (this.TextName.Text != Person.Name)
        {
            if (!string.IsNullOrEmpty(aperson.PartyEmail))
            {
                msgAskAboutPPAddress.Visible = true; //Show "alert" panel
                divMessage.InnerHtml =
                    Server.HtmlEncode(
                        this.GetLocalResourceObject("Interface.Controls.EditPerson.MsgAskForChangeMail").ToString()).Replace("[", "<").Replace("]", ">");
                /*"[br/][br/]
                [span style='font-size:1.1em;font-weight: bold']You changed your name.[/span][br/][br/]
                Do you want yor party mail address @piratpartiet.se[br/]
                to be changed as well?[br/]
                [br/]"*/
            }
            aperson.Name = this.TextName.Text;  // causes a database op
        }

        if (this.TextStreet.Text != Person.Street)
        {
            aperson.Street = this.TextStreet.Text; // causes a database op
        }


        if (this.TextCity.Text != Person.CityName)
        {
            aperson.City = this.TextCity.Text; // causes two dbops
        }



        Cities cities = Cities.FromPostalCode(this.TextPostalCode.Text.Replace(" ", ""), this.DropCountries.SelectedValue);
        int personGeography = person.GeographyId;

        if (this.TextPostalCode.Text != Person.PostalCode)
        {
            aperson.PostalCode = this.TextPostalCode.Text; // causes two dbops
        }



        if (this.DropCountries.SelectedValue != Person.Country.Code)
        {
            aperson.Country = Country.FromCode(this.DropCountries.SelectedValue); // causes two dbops
        }

        Person tempPerson = Person.FromIdentity(this.Person.Identity);
        //important that this is after country, city and postcode, since they might resolve wrongly
        if (this.DropDownMunicipalities.SelectedValue != tempPerson.GeographyId.ToString())
        {
            aperson.Geography = Geography.FromIdentity(int.Parse(this.DropDownMunicipalities.SelectedValue)); // causes two dbops
        }

        this.LabelEmailMessage.Text = "";
        this.LabelEmailMessage.CssClass = "";
        if (this.TextEmail.Text != Person.Email)
        {
            if (Formatting.ValidateEmailFormat(this.TextEmail.Text))
            {
                aperson.Email = this.TextEmail.Text; // causes dbop

                if (aperson.MailUnreachable)
                {
                    aperson.MailUnreachable = false;
                    this.LabelEmailMessage.Text = string.Empty;
                }
            }
            else
            {
                this.TextEmail.CssClass = "DirtyInput";
                this.LabelEmailMessage.Text = "Invalid Email Not Saved";
                this.LabelEmailMessage.CssClass = "ErrorMessage";
                this.TextEmail.Focus();
            }

        }

        if (this.TextPhone.Text != Person.Phone)
        {
            string phone = Formatting.CleanNumber(this.TextPhone.Text);
            this.TextPhone.Text = phone;

            aperson.Phone = phone; // causes dbop
        }



        this.LabelBirthdateMessage.Text = "";
        this.LabelBirthdateMessage.CssClass = "";
        DateTime birthdate = DateTime.MinValue;
        if (DateTime.TryParse(this.TextBirthdate.Text, out birthdate))
        {
            if (birthdate != Person.Birthdate)
            {
                aperson.Birthdate = birthdate; // causes dbop
            }
            this.TextBirthdate.CssClass = string.Empty;
            this.LabelBirthdateMessage.Text = string.Empty;
        }
        else
        {
            this.TextBirthdate.CssClass = "DirtyInput";
            this.LabelBirthdateMessage.Text = "Invalid Date Not Saved";
            this.LabelBirthdateMessage.CssClass = "ErrorMessage";
            this.TextBirthdate.Focus();
        }

        PersonGender gender = (PersonGender)Enum.Parse(typeof(PersonGender), this.DropGenders.SelectedValue);
        if (gender != Person.Gender)
        {
            aperson.Gender = gender; // causes dbop
        }
        string currentHandle = this.TextHandle.Text;
        try
        {
            currentHandle = Person.Handle;
            if (currentHandle == null)
            {
                currentHandle = string.Empty;
            }

            this.LabelHandleMessage.Text = string.Empty;
            this.TextHandle.CssClass = string.Empty;

            if (this.TextHandle.Text != currentHandle)
            {
                try
                {
                    aperson.Handle = this.TextHandle.Text;
                }
                catch (HandleException exh)
                {
                    this.TextHandle.CssClass = "DirtyInput";
                    if (exh.ErrorType == HandleErrorType.HandleNotFound)
                    {
                        this.LabelHandleMessage.Text = "Not Found";
                    }
                    else if (exh.ErrorType == HandleErrorType.HandleOccupied)
                    {
                        this.LabelHandleMessage.Text = "Already in use, contact memberservice@piratpartiet.se if it is yours.";
                    }
                    else
                    {
                        this.LabelHandleMessage.Text = "Invalid Handle";
                    }
                    this.LabelHandleMessage.CssClass = "ErrorMessage";
                    this.TextHandle.Focus();
                }
            }
        }
        catch (Exception ex)
        {
            currentHandle = "";
            this.TextHandle.Text = "Connection Error";
            this.TextHandle.ToolTip = ex.Message ;
            this.TextHandle.Enabled = false;
        }


        if (this.TextPersonalNumber.Text != Person.PersonalNumber)
        {
            aperson.PersonalNumber = this.TextPersonalNumber.Text;
        }




        this.LabelBankAccountMessage.CssClass =
            this.LabelBankNameMessage.CssClass = string.Empty;
        this.TextBankAccount.CssClass =
            this.TextBankName.CssClass = string.Empty;


        if (this.TextBankAccount.Text != Person.BankAccount || this.TextBankName.Text != Person.BankName || this.TextBankClearing.Text != Person.BankClearing)
        {
            // If we are saving the bank account, the name and account must either be both present
            // or both empty.


            if (this.TextBankName.Text.Trim() == string.Empty && this.TextBankAccount.Text.Trim() == string.Empty)
            {
                aperson.BankAccount = aperson.BankName = string.Empty;
            }
            else if (this.TextBankName.Text.Trim().Length > 1 && this.TextBankAccount.Text.Trim().Length > 4)
            {
                aperson.BankAccount = Formatting.CleanNumber(this.TextBankAccount.Text.Trim());  // causes dbops
                aperson.BankClearing = Formatting.CleanNumber(this.TextBankClearing.Text.Trim());
                aperson.BankName = this.TextBankName.Text.Trim();
            }
            else
            {
                // If we get here, only one of the fields have been set.

                this.TextBankName.CssClass = "DirtyInput";
                this.TextBankAccount.CssClass = "DirtyInput";
                this.LabelBankNameMessage.Text = "Must specify bank AND account";
                this.LabelBankAccountMessage.Text = "Must specify bank AND account";
                this.LabelBankNameMessage.CssClass = "ErrorMessage";
                this.LabelBankAccountMessage.CssClass = "ErrorMessage";

                if (this.TextBankAccount.Text.Trim().Length > 4)
                {
                    this.TextBankName.Focus();
                }
                else
                {
                    this.TextBankAccount.Focus();
                }
            }
        }

        if (this.TextPersonalNumber.Text != Person.PersonalNumber)
        {
            // TODO: Check for validity

            //aperson.PersonalNumber = this.TextPersonalNumber.Text.Trim();
        }

        if (DataChanged != null)
        {
            DataChanged(this, new EventArgs());
        }
    }

    protected void btnAndraMailJa_Click (object sender, EventArgs e)
    {
        msgAskAboutPPAddress.Visible = false;

        Person viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));

        //This event differs from the std EmailAccountRequested in that it sets parameterInt and parameterText
        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.EmailAccountRequested, viewingPerson.Identity, Organization.PPSEid,
                                                        this.Person.GeographyId, this.Person.Identity, 1, this.Person.PartyEmail);

        bottomMessage.Text = Server.HtmlEncode(LocalizationManager.GetLocalString("Interface.Controls.EditPerson.MsgAskForChangeMail"
                    , "[b]You will recive a mail with your new party mail address and password.[/b]")).Replace("[", "<").Replace("]", ">");
    }


    protected void btnAndraMailNej_Click (object sender, EventArgs e)
    {
        msgAskAboutPPAddress.Visible = false;
    }

    protected void ButtonDeleteMail_Click (object sender, EventArgs e)
    {
        Person _currentUser = Person.FromIdentity(Convert.ToInt32(HttpContext.Current.User.Identity.Name));
        Authority _authority = _currentUser.GetAuthority();

        if (_authority.HasPermission(Permission.CanEditMailDB, Organization.PPSEid, -1, Authorization.Flag.AnyGeographyExactOrganization))
        {
            MailServerDatabase.DeleteAccount(TextPartyEmail.Text);
            PWLog.Write(_currentUser, PWLogItem.MailAccount, 0, PWLogAction.MailAccountChanged, "Deleted account", "Manually changed in PW", TextPartyEmail.Text, "", "");
            TextPartyEmail.Text = "";
        }

    }
}
