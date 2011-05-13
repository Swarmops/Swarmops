using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
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
using Activizr.Logic.Structure;

using Membership = Activizr.Logic.Pirates.Membership;
using Activizr.Interface.Objects;
using Activizr.Logic.Security;
using Activizr.Logic.Support;

public partial class Pages_Members_SubscriptionSettings : PageV4Base
{
    Person _displayedPerson;

    protected void Page_Load (object sender, EventArgs e)
    {
        this.pagePermissionDefault = new PermissionSet(Permission.CanEditMemberSubscriptions);

        // Get requested person

        _displayedPerson = Person.FromIdentity(Convert.ToInt32("" + Request["id"]));
        this.labelCurrentMember.Text = _displayedPerson.Name + " (#" + _displayedPerson.Identity.ToString() + ")";

        // Authorize
        bool allowed = _authority.CanSeePerson(_displayedPerson) ||
                            _authority.HasRoleAtOrganization(Organization.PPSE, RoleType.OrganizationMemberService,
                             Authorization.Flag.AnyGeographyExactOrganization);
        // If they got this far the have been able to see the person.

        if (!allowed)
        {
            ((MasterV4Base)Master).CurrentPageProhibited = true;
        }


        if (!Page.IsPostBack)
        {
            List<int> organizationIds = new List<int>();
            foreach (Membership membership in _displayedPerson.GetMemberships())
            {
                foreach (Organization org in membership.Organization.GetLine())
                {
                    if (!organizationIds.Contains(org.Identity))
                    {
                        organizationIds.Add(org.Identity);
                    }
                }
            }
            this.ViewState["Organizations"] = organizationIds.ToArray();
        }
    }

    protected void ButtonSave_Click (object sender, EventArgs e)
    {
        // Save the new subscription settings
        foreach (ListItem item in listSubscriptions.Items)
        {
            if (item.Value.ToLower() == "activist")
            {
                if (item.Selected != _displayedPerson.IsActivist)
                {
                    if (_displayedPerson.IsActivist)
                    {
                        ActivistEvents.TerminateActivistWithLogging(_displayedPerson, EventSource.PirateWeb);
                    }
                    else
                    {
                        int partyOrgId = _displayedPerson.NationalPartyOrg(false);
                        ActivistEvents.CreateActivistWithLogging(_displayedPerson.Geography, _displayedPerson, "Registered on Subscriptionpage in PirateWeb", EventSource.PirateWeb, true, true, partyOrgId);
                    }
                }
            }
            else
            {
                int newsletterFeedId = Convert.ToInt32(item.Value);
                _displayedPerson.SetSubscription(newsletterFeedId, item.Selected);
            }
        }
    }

    protected void Page_PreRender (object sender, EventArgs e)
    {
        // Display the current subscription settings
        listSubscriptions.Items.Clear();

        int[] organizationIds = (int[])this.ViewState["Organizations"];
        foreach (int orgId in organizationIds)
        {
            // Retrieve the list of newsletter feeds
            bool allowEdit = _authority.HasPermission(Permission.CanEditMemberSubscriptions, orgId, -1, Authorization.Flag.Default);
            Organization org = Organization.FromIdentity(orgId);
            NewsletterFeeds feeds = org.GetNewsletterFeeds();
            foreach (NewsletterFeed feed in feeds)
            {
                ListItem item = new ListItem(feed.Name, feed.NewsletterFeedId.ToString()); ;
                item.Selected = _displayedPerson.IsSubscribing(feed.NewsletterFeedId);
                listSubscriptions.Items.Add(item);
                item.Enabled = allowEdit;
            }
        }

        ListItem itemA = new ListItem("Aktivist (Får speciella lokala aktivist-meddelanden via mail & SMS)", "activist");
        itemA.Selected = _displayedPerson.IsActivist;
        itemA.Enabled = _authority.HasPermission(Permission.CanEditMemberSubscriptions, Organization.PPSEid, -1, Authorization.Flag.Default);
        listSubscriptions.Items.Add(itemA);
    }
}
