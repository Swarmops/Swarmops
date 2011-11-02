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

public partial class Pages_Public_SE_People_OfficerSignup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Populate geo dropdown

        if (!Page.IsPostBack)
        {
            this.DropGeographies.Items.Add(new ListItem("-- V\xE4lj --", "0"));
            //TODO: Hardcoded Sweden, should maybe be dependent on user
            Geographies geos = Geographies.FromLevel(Country.FromCode("SE"), GeographyLevel.Municipality);

            foreach (Geography geo in geos)
            {
                this.DropGeographies.Items.Add(new ListItem(geo.Name, geo.Identity.ToString()));
            }
        }
    }
    protected void ButtonSubmit_Click(object sender, EventArgs e)
    {
        string responsibilities = string.Empty;
        Geography geography = Geography.FromIdentity (Int32.Parse(this.DropGeographies.SelectedValue));

        // Find: is this an existing person?

        Person person = null;

        //TODO: Hardcoded countrycode = 1
        People people = People.FromPhoneNumber(1, this.TextPhone.Text);

        if (people.Count == 1)
        {
            if (people[0].Name.ToLower() == this.TextName.Text.ToLower())
            {
                person = people[0];
            }
        }

        // If not, create one for this purpose

        if (person == null)
        {
            person = Person.Create(this.TextName.Text, string.Empty, string.Empty, this.TextPhone.Text, string.Empty, string.Empty, string.Empty, "SE", DateTime.Now, PersonGender.Unknown);
            person.Geography = geography;
        }
        Person defaultOwner = Person.FromIdentity(1);
        Volunteer volunteer = Volunteer.Create(person, defaultOwner); // RF owns new volunteers

        if (this.CheckKL1.Checked)
        {
            responsibilities += " KL1";
            volunteer.AddRole(Organization.PPSEid, geography.Identity, RoleType.LocalLead);   
        }

        if (this.CheckKL2.Checked)
        {
            responsibilities += " KL2";
            volunteer.AddRole(Organization.PPSEid, geography.Identity, RoleType.LocalDeputy);
        }


        // Move "geography" up to electoral circuit level
        while ((!geography.AtLevel(GeographyLevel.ElectoralCircuit)) && (geography.Parent != null))
        {
           geography = geography.Parent;
        }

        //Autoassign will try to assign to ElectoralCircuit lead or 
        //if not possible, to its parent org lead, or if not possible to defaultOwner
        //TODO:Hardcoded sweden(30)
        volunteer.AutoAssign(geography, Organization.PPSEid, defaultOwner, Geography.SwedenId );

        if (this.CheckVL1.Checked)
        {
            responsibilities += " VL1";
            volunteer.AddRole(Organization.PPSEid, geography.Identity, RoleType.LocalLead);
        }

        if (this.CheckVL2.Checked)
        {
            responsibilities += " VL2";
            volunteer.AddRole(Organization.PPSEid, geography.Identity, RoleType.LocalDeputy);
        }

        responsibilities = responsibilities.Trim();

        string textParameter = this.TextName.Text.Replace("|", "") + "|" + this.TextPhone.Text.Replace("|", "") + "|" + this.DropMember.SelectedValue + "|" + responsibilities;
        Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.SignupPage, EventType.NewVolunteer, 0, Organization.PPSEid, Int32.Parse(this.DropGeographies.SelectedValue), 0, 0, textParameter);

        this.TextName.Enabled = false;
        this.TextPhone.Enabled = false;
        this.DropGeographies.Enabled = false;
        this.DropMember.Enabled = false;
        this.CheckKL1.Enabled = false;
        this.CheckKL2.Enabled = false;
        this.CheckVL1.Enabled = false;
        this.CheckVL2.Enabled = false;
        this.ButtonSubmit.Enabled = false;

        this.PanelFinished.Visible = true;
    }
}
