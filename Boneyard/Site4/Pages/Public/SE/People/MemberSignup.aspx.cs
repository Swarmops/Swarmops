using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;
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

public partial class Pages_Public_SE_People_MemberSignup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Set a couple of input box widths

        this.TextFirstName.Style[HtmlTextWriterStyle.Width] = "100px";
        this.TextStreet.Style[HtmlTextWriterStyle.Width] = "250px";
        this.TextPostalCode.Style[HtmlTextWriterStyle.Width] = "50px";

        this.DropBirthDay.Style[HtmlTextWriterStyle.Width] = "45px";
        this.DropBirthMonth.Style[HtmlTextWriterStyle.Width] = "100px";
        this.DropBirthYear.Style[HtmlTextWriterStyle.Width] = "60px";

        if (!Page.IsPostBack)
        {
            // Populate dropdowns

            // Countries

            Countries countries = Countries.GetAll();
            foreach (Country country in countries)
            {
                DropCountries.Items.Add(new ListItem(country.Code + " " + country.Name, country.Code));
            }
            DropCountries.Items.FindByValue("SE").Selected = true;

            // Months

            CultureInfo previousCulture = Thread.CurrentThread.CurrentCulture;

            Thread.CurrentThread.CurrentCulture = new CultureInfo(Country.FromCode("SE").Culture);

            for (int month = 1; month <= 12; month++)
            {
                DropBirthMonth.Items.Add(new ListItem(new DateTime(2010,month,1).ToString("MMMM"), month.ToString("0#")));
            }

            Thread.CurrentThread.CurrentCulture = previousCulture;

            // Days

            for (int day = 1; day <= 31; day++)
            {
                DropBirthDay.Items.Add(new ListItem(day.ToString("0#")));
            }

            // Years. People need to sign up of their own free will -- assume people under 3 can't read yet.

            int maxYear = DateTime.Today.Year - 3;

            for (int year=1900; year<= maxYear; year++)
            {
                DropBirthYear.Items.Add(new ListItem(year.ToString()));
            }

            DropBirthYear.SelectedIndex = 85;

            // Assume no postback means we're at first page, so set focus

            this.TextFirstName.Focus();

            // Remember the referrer before the first postback

            if (Request.UrlReferrer != null)
            {
                ViewState["Referrer"] = Request.UrlReferrer.ToString();
            }
            else
            {
                ViewState["Referrer"] = string.Empty;
            }
        }
    }

    protected void WizardNext_Click(object sender, WizardNavigationEventArgs e)
    {
        if (this.Wizard.ActiveStepIndex == 3)
        {
            // Save password in viewstate

            ViewState["pwpass"] = this.TextPassword.Text;
        }

        bool below26 = false;
        
        if (this.Wizard.ActiveStepIndex > 1)
        {
            if (DateTime.Today.Year - Convert.ToInt32(this.DropBirthYear.SelectedValue) < 26)
            {
                below26 = true;
            }

            if (this.Wizard.ActiveStepIndex == 6)
            {
                this.Wizard.ActiveStepIndex = 7; // necessary for CompleteSignup() to fire at end of this function
            }
        }

        if (this.Wizard.ActiveStepIndex == 4)
        {
            // Check for officer volunteer. If not, move to partner deal step

            if (!this.RadioActivityOfficer.Checked)
            {
                this.Wizard.ActiveStepIndex = below26? 6 : 7;
            }
        }

        if (this.Wizard.ActiveStepIndex == 5)
        {
            if (!below26)
            {
                this.Wizard.ActiveStepIndex = 7;
            }
        }

        // If we're at the end of the line, complete the signup

        if (this.Wizard.ActiveStepIndex > 6 && Page.IsValid)
        {
            CompleteSignup();
        }
    }



    private void CompleteSignup()
    {
        string name = this.TextFirstName.Text + "|" + this.TextLastName.Text;
        string password = ViewState["pwpass"] as string;
        string email = this.TextEmail.Text.Replace(",", ".");  // for some reason 1 in 500 mistypes this, even with repeats
        string phone = this.TextPhone.Text;
        string street = this.TextStreet.Text;
        string postal = this.TextPostalCode.Text;
        string city = this.TextCity.Text;
        string countryCode = this.DropCountries.SelectedValue;

        int birthDay = Convert.ToInt32(this.DropBirthDay.SelectedValue);
        int birthMonth = Convert.ToInt32(this.DropBirthMonth.SelectedValue);
        int birthYear = Convert.ToInt32(this.DropBirthYear.SelectedValue);

        DateTime birthdate = new DateTime(birthYear, birthMonth, birthDay);
        PersonGender gender = (PersonGender)Enum.Parse(typeof(PersonGender), this.DropGenders.SelectedValue);

        // We are ready to create the new person.

        Person person = Person.Create(name, email, password, phone, street, postal, city, countryCode, birthdate, gender);

        // The creation resolves the appropriate geography, so we can determine the correct
        // organization to join the person to.

        // In some cases PostalCode does not indicate the geography uniquely
        if (DropGeographies.SelectedIndex >= 0)
        {
            person.Geography = Geography.FromIdentity(int.Parse(DropGeographies.SelectedValue));
        }
        else
        {
            person.Geography = Country.FromCode(countryCode).Geography;                
        }

        person.AddMembership(Organization.PPSEid, DateTime.Now.AddYears(1));
        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SignupPage, EventType.AddedMember,
            person.Identity, Organization.PPSEid, person.GeographyId, person.Identity, 0, Organization.PPSEid.ToString() + "," + Request.UserHostAddress + "," + ViewState["Referrer"] as string);
        PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MemberAdd, "New member joined Piratpartiet SE.", "The self-signup came from IP address " + Request.UserHostAddress + ".");

        // If activity level was not passive (activist or officer), then register as activist.

        if (!this.RadioActivityPassive.Checked)
        {
            person.CreateActivist(false, false);

            Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SignupPage, EventType.NewActivist,
                person.Identity, Organization.PPSEid, person.GeographyId, person.Identity, 0, string.Empty);
            PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MemberAdd, "Joined as activist.", string.Empty);
        }

        // If activity level was officer, register as officer volunteer.

        if (this.RadioActivityOfficer.Checked)
        {
            Person defaultOwner = Person.FromIdentity(1);
            Volunteer volunteer = Volunteer.Create(person, defaultOwner); // RF owns new volunteers

            //Autoassign will try to assign to ElectoralCircuit lead or 
            //if not possible, to its parent org lead, or if not possible to defaultOwner
            //TODO:Ugly hack sweden hardcoded (30)
            volunteer.AutoAssign(person.Geography, Organization.PPSEid, defaultOwner, Geography.SwedenId);

            volunteer.AddRole(Organization.PPSEid, person.GeographyId, RoleType.LocalLead);
            volunteer.AddRole(Organization.PPSEid, person.GeographyId, RoleType.LocalDeputy);
            volunteer.AddRole(Organization.PPSEid, person.GeographyId, RoleType.LocalAdmin);

            // TODO: Add more specifics about how the volunteer would like to work.

            string textParameter = person.Name.Replace("|", "") + "|" +
                                   person.Phone.Replace("|", "") +
                                   "|Yes|KL1 KL2 KLA";

            Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SignupPage, EventType.NewVolunteer, 0, Organization.PPSEid, person.GeographyId, 0, 0, textParameter);
            PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MemberAdd, "Volunteered for local officership.", string.Empty);
        }

        // If Ung Pirat was checked, process.

        if (this.CheckUngPirat.Checked)
        {
            Organization youthOrg = Organizations.GetMostLocalOrganization(person.GeographyId, Organization.UPSEid);
            person.AddMembership(youthOrg.Identity, DateTime.Now.AddYears(1));
            Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SignupPage, EventType.AddedMember,
                                                         person.Identity, youthOrg.Identity, person.GeographyId,
                                                         person.Identity, 0,
                                                         youthOrg.Identity.ToString() + " " +
                                                         Organization.PPSEid.ToString() + "," +
                                                         Request.UserHostAddress + "," + ViewState["Referrer"] as string);
            PWLog.Write(PWLogItem.Person, person.Identity, PWLogAction.MemberAdd,
                        "New member joined organization " + youthOrg.NameShort + ".",
                        "The self-signup came from IP address " + Request.UserHostAddress + ".");
        }
    }



    protected void TextPostalCode_Changed(object sender, EventArgs e)
    {
        this.TextPostalCode.Text = this.TextPostalCode.Text.Replace(" ", "");
        RebuildGeographyOptions();
    }

    private void RebuildGeographyOptions()
    {
        Cities citylist = Cities.FromPostalCode(TextPostalCode.Text, DropCountries.SelectedValue);
        string selectedGeographyName = DropGeographies.SelectedValue;
        LabelGeographies.Text = "Kommun";
        DropGeographies.Items.Clear();
        foreach (City city in citylist)
        {
            Geographies subGeo = city.Geography.GetTree();
            if (!city.Geography.Name.ToLower().Contains("kommun"))
            {
                LabelGeographies.Text = "Stadsdel";
            }
            TextCity.Text = city.Name;
            foreach (Geography geo in subGeo)
            {
                // ignore  if it is not a leaf
                if (geo.ChildrenCount == 0)
                {
                    if (DropGeographies.Items.FindByValue(geo.GeographyId.ToString()) == null)
                    {
                        ListItem newItem = new ListItem(geo.Name, geo.GeographyId.ToString());
                        DropGeographies.Items.Add(newItem);
                        if (selectedGeographyName == geo.GeographyId.ToString())
                            newItem.Selected = true;
                    }
                }
            }
        }
    }

    protected void Validate_Birthdate_ServerValidate(object source, ServerValidateEventArgs args)
    {
        // This is an easier method than constructing a DateTime from Int32.Parse() and catching a possible exception.

        DateTime dummy;
        args.IsValid =
            DateTime.TryParse(
                this.DropBirthYear.SelectedValue + "-" + this.DropBirthMonth.SelectedValue + "-" +
                this.DropBirthDay.SelectedValue, out dummy);
    }
}
