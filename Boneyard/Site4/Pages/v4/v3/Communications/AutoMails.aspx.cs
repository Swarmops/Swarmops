using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Logic.Communications;
using Activizr.Interface.Localization;
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;

using Roles = Activizr.Logic.Pirates.Roles;


public partial class Pages_Communications_AutoMails : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {


        if (!Page.IsPostBack)
        {
            // Populate list of organizations (initial population)

            Organizations organizationList = _authority.GetOrganizations(RoleTypes.AllRoleTypes);
            organizationList = organizationList.RemoveRedundant();

            foreach (Organization organization in organizationList)
            {
                Organizations organizationTree = organization.GetTree();

                foreach (Organization organizationOption in organizationTree)
                {
                    string orgLabel = organizationOption.NameShort;

                    /*
                    for (int loop = 0; loop < organizationOption.Generation; loop++)
                    {
                        orgLabel = "|-- " + orgLabel;
                    }*/
                    if (_authority.HasPermission(Permission.CanSetUpAutomail, organizationOption.Identity, -1, Authorization.Flag.AnyGeography))
                        DropOrganizations.Items.Add(new ListItem(orgLabel, organizationOption.OrganizationId.ToString()));
                }
            }

            ResetOrganizationData();
        }
    }


    protected void DropRecipients_SelectedIndexChanged (object sender, EventArgs e)
    {

    }


    protected void PopulateGeographies ()
    {
        Person viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
        Authority authority = viewingPerson.GetAuthority();

        Organization org = Organization.FromIdentity(Convert.ToInt32(DropOrganizations.SelectedValue));
        Geographies geoList = authority.GetGeographiesForOrganization(org);

        geoList = geoList.RemoveRedundant();
        geoList = geoList.FilterAbove(Geography.FromIdentity(org.AnchorGeographyId));


        this.DropGeographies.Items.Clear();

        foreach (Geography nodeRoot in geoList)
        {
            Geographies nodeTree = nodeRoot.GetTree();

            foreach (Geography node in nodeTree)
            {
                string nodeLabel = node.Name;

                for (int loop = 0; loop < node.Generation; loop++)
                {
                    nodeLabel = "|-- " + nodeLabel;
                }
                if (authority.HasPermission(Permission.CanSetUpAutomail, org.Identity, node.Identity, Authorization.Flag.Default))
                    DropGeographies.Items.Add(new ListItem(nodeLabel, node.GeographyId.ToString()));
            }
        }
        
        if (authority.HasPermission(Permission.CanSetUpAutomail, org.Identity, Geography.RootIdentity, Authorization.Flag.Default))
            DropGeographies.Items.Add(new ListItem("Organization", Geography.RootIdentity.ToString()));

        ResetGeographyData();
    }


    protected void DropOrganizations_SelectedIndexChanged (object sender, EventArgs e)
    {
        ResetOrganizationData();
        SetPermissions();
    }


    protected void ResetOrganizationData ()
    {
        PopulateGeographies();

        Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
        this.LabelSelectedOrganization.Text = selectedOrg.Name;
        this.LabelSelectedOrganizationMailPrefix.Text = selectedOrg.MailPrefixInherited;
        this.LabelMailAddressSuffix.Text = " (" + selectedOrg.MailPrefixInherited + ")";
    }


    protected void DropGeographies_SelectedIndexChanged (object sender, EventArgs e)
    {
        ResetGeographyData();
    }

    private void ResetGeographyData ()
    {
        Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
        if (this.DropGeographies.SelectedIndex > -1)
        {
            Geography selectedGeo = Geography.FromIdentity(Convert.ToInt32(this.DropGeographies.SelectedValue));

            if (selectedOrg.AnchorGeographyId == selectedGeo.Identity)
            {
                this.LabelSelectedGeography.Text = string.Empty;
                this.LabelOrganizationsInGeographies2.Text = string.Empty;
            }
            else
            {
                this.LabelSelectedGeography.Text = selectedGeo.Name;
                this.LabelOrganizationsInGeographies2.Text = " in ";
            }
        }
        else
        {
            this.LabelSelectedGeography.Text = string.Empty;
            this.LabelOrganizationsInGeographies2.Text = string.Empty;
        }

        LoadMail();
        SetPermissions();
    }


    private void SetPermissions ()
    {
        bool mayEdit = false;

        Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
        if (this.DropGeographies.SelectedIndex > -1)
        {
            Geography selectedGeo = Geography.FromIdentity(Convert.ToInt32(this.DropGeographies.SelectedValue));

            Authority authority = _currentUser.GetAuthority();

            //TODO:Remove the following 2 if statements, it should be covered by Permission.CanSetUpAutomail
            if (authority.HasLocalRoleAtOrganizationGeography(selectedOrg, selectedGeo, new RoleType[] { RoleType.LocalLead, RoleType.LocalDeputy }, Authorization.Flag.Default))
            {
                mayEdit = true;
            }
            if (authority.HasRoleAtOrganization(selectedOrg, new RoleType[] { RoleType.OrganizationChairman, RoleType.OrganizationSecretary }, Authorization.Flag.Default))
            {
                mayEdit = true;
            }


            if (authority.HasPermission(Permission.CanSetUpAutomail, selectedOrg.Identity, selectedGeo.Identity, Authorization.Flag.Default))
            {
                mayEdit = true;
            }

        }

        this.ButtonPreview.Enabled = mayEdit;
        this.ButtonSave.Enabled = mayEdit;
        this.TextBody.Enabled = mayEdit;
    }



    protected void ButtonPreview_Click (object sender, EventArgs e)
    {
        CreatePreview();
    }


    protected void ButtonSave_Click (object sender, EventArgs e)
    {
        AutoMail mail = (AutoMail)HttpContext.Current.Session["AutoMail"];

        if (mail != null)
        {
            // Setting the properties saves to database.

            mail.Body = this.TextBody.Text.Trim();
        }
        else
        {
            int organizationId = Convert.ToInt32(this.DropOrganizations.SelectedValue);
            int geographyId = Convert.ToInt32(this.DropGeographies.SelectedValue);

            mail = AutoMail.Create(AutoMailType.Welcome, Organization.FromIdentity(organizationId),
                Geography.FromIdentity(geographyId), null, string.Empty, this.TextBody.Text.Trim());


        }
        Page.ClientScript.RegisterStartupScript(this.GetType(), "StartupMessage", "alert('Your changes have been saved.');", true);

        HttpContext.Current.Session["AutoMail"] = mail;
        CreatePreview(); // to reflect saved changes
    }


    protected void ButtonLoad_Click (object sender, EventArgs e)
    {

    }

    private void LoadMail ()
    {
        int organizationId = Convert.ToInt32(this.DropOrganizations.SelectedValue);
        int geographyId = 0;
        if (this.DropGeographies.SelectedIndex > -1)
        {
            geographyId = Convert.ToInt32(this.DropGeographies.SelectedValue);

            AutoMail mail = AutoMail.FromTypeOrganizationAndGeography(
                AutoMailType.Welcome, Organization.FromIdentity(organizationId), Geography.FromIdentity(geographyId));

            HttpContext.Current.Session["AutoMail"] = mail;

            this.PanelMailContents.Visible = true;

            if (mail != null)
            {
                this.TextBody.Text = mail.Body;
            }
            else
            {
                this.TextBody.Text = string.Empty;
            }
        }
        else
        {
            this.LabelSelectedGeography.Text = string.Empty;
            this.LabelOrganizationsInGeographies2.Text = string.Empty;
        }

        try
        {
            Person localLead = Roles.GetLocalLead(organizationId, geographyId);
            this.LabelSender.Text = localLead.Name;
            this.LabelSender.CssClass = string.Empty;
        }
        catch (Exception)
        {
            this.LabelSender.Text = "No Local Lead";
            this.LabelSender.CssClass = "ErrorMessage";
        }

        CreatePreview();
    }


    private void CreatePreview ()
    {
        Person currentPerson = Person.FromIdentity(Convert.ToInt32(HttpContext.Current.User.Identity.Name));
        Organization org = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
        if (this.DropGeographies.SelectedIndex > -1)
        {

            Geography geo = Geography.FromIdentity(Convert.ToInt32(this.DropGeographies.SelectedValue));

            WelcomeMail welcomemail = new WelcomeMail();
            welcomemail.pSubject = this.TextSubject.Text;
            welcomemail.pBodyContent = this.TextBody.Text;
            welcomemail.pOrgName = org.MailPrefixInherited;

            welcomemail.pGeographyName = "";
            if (geo.Identity != Geography.RootIdentity)
            {
                welcomemail.pGeographyName = geo.Name;
            }

            OutboundMail fake = welcomemail.CreateOutboundFake(currentPerson, org, geo);
            this.LiteralPreview.Text = fake.RenderHtml(currentPerson, org.DefaultCountry.Culture);
        }

        this.TextBody.Focus();
    }
}
