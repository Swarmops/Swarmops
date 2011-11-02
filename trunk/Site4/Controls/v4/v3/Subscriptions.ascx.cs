using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;
using Activizr.Basic.Enums;
using System.Collections.Generic;
using Activizr.Logic.Structure;
using Membership = Activizr.Logic.Pirates.Membership;
using Activizr.Interface.Objects;
using Activizr.Logic.Communications;
using Activizr.Logic.Support;

public partial class Subscriptions_UserControl : System.Web.UI.UserControl
{
    Person _currentUser;
    Authority _authority;

    Person _displayedPerson;
    public Person DisplayedPerson
    {
        get
        {
            return _displayedPerson;
        }
        set
        {
            _displayedPerson = value;
        }
    }

    int _currentUserId = 0;

    public int CurrentUserId
    {
        get { 
            if (_currentUserId==0)
                _currentUserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
            
            return _currentUserId; 
            }
            
        set { _currentUserId = value; }
    }

    protected void Page_Load (object sender, EventArgs e)
    {

        _currentUser = Person.FromIdentity(CurrentUserId);
        _authority = _currentUser.GetAuthority();

        if (!Page.IsPostBack)
        {
            List<int> organizationIds = new List<int>();
            foreach (Membership membership in DisplayedPerson.GetMemberships())
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
                if (item.Selected != DisplayedPerson.IsActivist)
                {
                    if (DisplayedPerson.IsActivist)
                    {
                        ActivistEvents.TerminateActivistWithLogging(DisplayedPerson, EventSource.PirateWeb);
                    }
                    else
                    {
                        int partyOrgId = DisplayedPerson.NationalPartyOrg(false);
                        ActivistEvents.CreateActivistWithLogging(DisplayedPerson.Geography, DisplayedPerson, "Registered on Subscriptionpage in PirateWeb", EventSource.PirateWeb, true, true, partyOrgId);
                    }
                }
            }
            else
            {
                int newsletterFeedId = Convert.ToInt32(item.Value);
                DisplayedPerson.SetSubscription(newsletterFeedId, item.Selected);
            }
        }
    }

    protected void Page_PreRender (object sender, EventArgs e)
    {
        // Display the current subscription settings
        listSubscriptions.Items.Clear();

        bool allowEditInGeneral = false;

        int[] organizationIds = (int[])this.ViewState["Organizations"];
        foreach (int orgId in organizationIds)
        {
            // Retrieve the list of newsletter feeds
            bool allowEdit = _authority.HasPermission(Permission.CanEditMemberSubscriptions, orgId, -1, Authorization.Flag.Default);
            allowEdit = allowEdit || (DisplayedPerson.Identity == _currentUser.Identity);

            allowEditInGeneral = allowEditInGeneral || allowEdit;
            Organization org = Organization.FromIdentity(orgId);
            NewsletterFeeds feeds = org.GetNewsletterFeeds();
            foreach (NewsletterFeed feed in feeds)
            {
                if (feed.NewsletterFeedId == NewsletterFeed.TypeID.OfficerUpwardCopies) continue;
                if (feed.NewsletterFeedId == NewsletterFeed.TypeID.OfficerNewMembers) continue;

                AddFeed(allowEdit, feed);
            }
        }

        allowEditInGeneral = allowEditInGeneral || (DisplayedPerson == _currentUser);

        //the two below is not organisation dependent
        AddFeed(allowEditInGeneral, NewsletterFeed.FromIdentity(NewsletterFeed.TypeID.OfficerUpwardCopies));
        AddFeed(allowEditInGeneral, NewsletterFeed.FromIdentity(NewsletterFeed.TypeID.OfficerNewMembers));

        string activistText = GetLocalResourceObject("activist").ToString();
        ListItem itemA = new ListItem(activistText, "activist");

        itemA.Selected = DisplayedPerson.IsActivist;
        itemA.Enabled = allowEditInGeneral;

        listSubscriptions.Items.Add(itemA);
    }

    private void AddFeed (bool allowEdit, NewsletterFeed feed)
    {
        // use resource translation for common feeds
        object name = GetLocalResourceObject("feed_" + feed.NewsletterFeedId.ToString());
        if (name == null)
            name = feed.Name;
        ListItem item = new ListItem(name.ToString(), feed.NewsletterFeedId.ToString()); ;
        item.Selected = DisplayedPerson.IsSubscribing(feed.NewsletterFeedId);
        listSubscriptions.Items.Add(item);
        item.Enabled = allowEdit;
    }
}
