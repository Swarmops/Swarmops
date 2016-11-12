using System;
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
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Logic.Structure;
using Activizr.Basic.Enums;

public partial class Pages_Communications_SendLocalMail : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        int viewerPersonId = Int32.Parse(HttpContext.Current.User.Identity.Name);
        Person viewerPerson = Person.FromIdentity(viewerPersonId);
        Authority authority = viewerPerson.GetAuthority();


        if (!Page.IsPostBack)
        {

            // Populate list of organizations (initial population)

            Organizations organizationList = authority.GetOrganizations(RoleTypes.AllRoleTypes);
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
                    if (authority.HasPermission(Permission.CanSendLocalMail, organizationOption.Identity, -1, Authorization.Flag.AnyGeography))
                        DropOrganizations.Items.Add(new ListItem(orgLabel, organizationOption.OrganizationId.ToString()));
                }
            }

            this.LabelSender.Text = viewerPerson.Name;
            this.ValidatorTextSubject.Text = GetLocalResourceObject("ValidatorTextSubject.Text").ToString();

            ResetOrganizationData();
        }

        this.TextSubject.Focus();

    }

    protected void PopulateGeographies ()
    {
        Person viewingPerson = Person.FromIdentity(Int32.Parse(HttpContext.Current.User.Identity.Name));
        Authority authority = viewingPerson.GetAuthority();
        int orgId = Convert.ToInt32(DropOrganizations.SelectedValue);
        Organization org = Organization.FromIdentity(orgId);
        Geographies geoList = authority.GetGeographiesForOrganization(org);
        if (geoList.Count >= 0)
        {
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
                    if (authority.HasPermission(Permission.CanSendLocalMail, orgId, node.Identity, Authorization.Flag.Default))
                        DropGeographies.Items.Add(new ListItem(nodeLabel, node.GeographyId.ToString()));
                }
            }
            
            if (_authority.HasPermission(Permission.CanSendLocalMail, org.Identity, -1, Authorization.Flag.Default))
            {
                DropGeographies.Items.Add(new ListItem("All", Geography.RootIdentity.ToString()));
                DropGeographies.Items[DropGeographies.Items.Count-1].Selected=true;
            }
            
            ResetGeographyData();
        }
    }


    protected void DropOrganizations_SelectedIndexChanged (object sender, EventArgs e)
    {
        ResetOrganizationData();
    }


    protected void ResetOrganizationData ()
    {
        PopulateGeographies();

        Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
        this.LabelSelectedOrganization.Text = selectedOrg.Name;
        this.LabelSelectedOrganizationMailPrefix.Text = selectedOrg.MailPrefixInherited;
        this.LabelMailAddressSuffix.Text = " (" + selectedOrg.MailPrefixInherited + ")";
        RecalculateRecipientCount();
    }


    protected void RecalculateRecipientCount ()
    {
        Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
        Organizations orgTree = selectedOrg.GetTree();
        this.LabelRecipientCount.Text = "";
        if (this.DropGeographies.SelectedIndex < 0)
            return;

        Geography selectedGeo = Geography.FromIdentity(Convert.ToInt32(this.DropGeographies.SelectedValue));
        Geographies geoTree = selectedGeo.GetTree();

        int memberCount = 0;

        // Optimize a little: if we are at the org's top anchor, do not parse the geography, just select all
        // doesnt work, people live in other geos

        switch (this.DropRecipients.SelectedValue)
        {
            case "Members":
                if (false && selectedGeo.Identity == selectedOrg.AnchorGeographyId)
                {
                    memberCount = orgTree.GetMemberCount();
                }
                else
                {
                    memberCount = orgTree.GetMemberCountForGeographies(geoTree);
                }
                break;

            case "Officers":
                memberCount = orgTree.GetRoleHolderCountForGeographies(geoTree);
                break;
        }

        this.LabelRecipientCount.Text = " (" + memberCount.ToString("#,##0") + " recipients)";
    }


    protected void DropGeographies_SelectedIndexChanged (object sender, EventArgs e)
    {
        ResetGeographyData();
    }

    private void ResetGeographyData ()
    {
        Organization selectedOrg = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
        if (this.DropGeographies.SelectedIndex < 0)
            return;
        Geography selectedGeo = Geography.FromIdentity(Convert.ToInt32(this.DropGeographies.SelectedValue));

        if (selectedOrg.AnchorGeographyId == selectedGeo.Identity)
        {
            this.LabelSelectedGeography.Text = string.Empty;
            this.LabelOrganizationsInGeographies2.Text = string.Empty;
            this.LabelSelectedRecipients.Text = "All " + Char.ToLower(this.DropRecipients.SelectedValue[0]) + this.DropRecipients.SelectedValue.Substring(1) + GetLocalResourceObject("LabelOrganizationsInGeographies2Resource1.Text").ToString();
        }
        else
        {
            this.LabelSelectedGeography.Text = selectedGeo.Name;
            this.LabelOrganizationsInGeographies2.Text = GetLocalResourceObject("LabelOrganizationsInGeographies2Resource1.Text").ToString();
            this.LabelSelectedRecipients.Text = this.DropRecipients.SelectedValue + GetLocalResourceObject("LabelOrganizationsInGeographies2Resource1.Text").ToString();
        }

        RecalculateRecipientCount();
    }


    protected void DropRecipients_SelectedIndexChanged (object sender, EventArgs e)
    {
        // TODO: Localize

        this.LabelSelectedRecipients.Text = this.DropRecipients.SelectedValue + GetLocalResourceObject("LabelOrganizationsInGeographies2Resource1.Text").ToString();
        RecalculateRecipientCount();
    }


    protected void ButtonSend_Click (object sender, EventArgs e)
    {
        if (this.TextBody.Text.Length > 1)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "StartupMessage", "alert('Your mail has been placed on the outbound queue. You will receive status reports in e-mail as the transmission progresses.');", true);

            Person currentPerson = Person.FromIdentity(Convert.ToInt32(HttpContext.Current.User.Identity.Name));
            Organization org = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
            if (this.DropGeographies.SelectedIndex < 0)
                return;

            Geography geo = Geography.FromIdentity(Convert.ToInt32(this.DropGeographies.SelectedValue));

            TypedMailTemplate template;
            if (this.DropRecipients.SelectedValue == "Members")
            {
                MemberMail membermail = new MemberMail();
                template = membermail;
                membermail.pSubject = this.TextSubject.Text;
                membermail.pBodyContent = this.TextBody.Text;
                membermail.pOrgName = org.MailPrefixInherited;
                membermail.pGeographyName = (geo.Identity == Geography.RootIdentity ? org.NameShort : geo.Name);
            }
            else
            {
                OfficerMail officermail = new OfficerMail();
                template = officermail;
                officermail.pSubject = this.TextSubject.Text;
                officermail.pBodyContent = this.TextBody.Text;
                officermail.pOrgName = org.MailPrefixInherited;
                officermail.pGeographyName = (geo.Identity == Geography.RootIdentity ? org.NameShort : geo.Name);
            }

            OutboundMail mail = template.CreateOutboundMail(currentPerson, OutboundMail.PriorityNormal, org, geo);

            // We're not resolving recipients here, but deferring that to PirateBot after pickup
            mail.SetReadyForPickup();
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "StartupMessage", "alert('Nothing to do, the mail is empty...');", true);
        }
    }


    protected void ButtonPreview_Click (object sender, EventArgs e)
    {
        Person currentPerson = Person.FromIdentity(Convert.ToInt32(HttpContext.Current.User.Identity.Name));
        Organization org = Organization.FromIdentity(Convert.ToInt32(this.DropOrganizations.SelectedValue));
        Geography geo = Geography.FromIdentity(Convert.ToInt32(this.DropGeographies.SelectedValue));

        TypedMailTemplate template;
        if (this.DropRecipients.SelectedValue == "Members")
        {
            MemberMail membermail = new MemberMail();
            template = membermail;
            membermail.pSubject = this.TextSubject.Text;
            membermail.pBodyContent = this.TextBody.Text;
            membermail.pOrgName = org.MailPrefixInherited;
            membermail.pGeographyName = (geo.Identity == Geography.RootIdentity ? org.NameShort : geo.Name);
        }
        else
        {
            OfficerMail officermail = new OfficerMail();
            template = officermail;
            officermail.pSubject = this.TextSubject.Text;
            officermail.pBodyContent = this.TextBody.Text;
            officermail.pOrgName = org.MailPrefixInherited;
            officermail.pGeographyName = (geo.Identity == Geography.RootIdentity ? org.NameShort : geo.Name);
        }

        OutboundMail fake = template.CreateOutboundFake(currentPerson, org, geo);

        this.LiteralPreview.Text = fake.RenderHtml(currentPerson, org.DefaultCountry.Culture);
        this.TextBody.Focus();
    }
}
