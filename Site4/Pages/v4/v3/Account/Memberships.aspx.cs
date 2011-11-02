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

using Membership = Activizr.Logic.Pirates.Membership;

public partial class Pages_Account_Memberships : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        Person displayedPerson = DisplayedPerson();

        // If not in viewstate, initialize page

        if (!this.IsPostBack)
        {
            // Localize

            // Set list of memberships

            ReadMembershipList();

            // Populate list of organizations (initial population)

        }
    }

    private Person DisplayedPerson ()
    {
        return _currentUser;
    }


    protected void GridMemberships_RowCommand (object sender, GridViewCommandEventArgs e)
    {
        // TODO: Re-authorize?

        int index = Convert.ToInt32(e.CommandArgument);
        int viewingPersonId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
        int membershipId = Convert.ToInt32(this.gridMemberships.DataKeys[index].Value);

        Membership membership = Membership.FromIdentity(membershipId);
        BasicPerson person = Person.FromIdentity(membership.PersonId);

        switch (e.CommandName)
        {
            case "Extend":
                if (membership.Expires.Year <= DateTime.Now.Year || membership.Active == false)
                {
                    if (membership.Active == false && membership.Expires < DateTime.Now.AddYears(1))
                        membership.Expires = DateTime.Now.AddYears(1);
                    else if (membership.Expires > DateTime.Now.AddYears(1))
                        membership.Expires = membership.Expires.AddSeconds(1);
                    else
                        membership.Expires = membership.Expires.AddYears(1);
                        
                    PWLog.Write(Person.FromIdentity(viewingPersonId), PWLogItem.Person, person.Identity,
                                    PWLogAction.MembershipRenewed,
                                    "Membership in " + Organization.FromIdentity(membership.OrganizationId).NameShort +
                                    " extended manually.",
                                    "Membership was extended by " + Person.FromIdentity(viewingPersonId).Name + " (#" +
                                    viewingPersonId.ToString() + ") to last until " +
                                    membership.Expires.ToString("yyyy-MMM-dd") + ".");
                    Activizr.Logic.Support.PWEvents.CreateEvent(EventSource.PirateWeb, EventType.ExtendedMembership, viewingPersonId,
                                                       membership.OrganizationId, person.GeographyId, membership.PersonId,
                                                       membershipId, string.Empty);
                }
                ReadMembershipList();
                break;
            case "Terminate":
                if (membership.Active)
                {
                    Membership.FromIdentity(membershipId).Terminate(EventSource.PirateWeb, Person.FromIdentity(viewingPersonId), "Membership in " + Organization.FromIdentity(membership.OrganizationId).NameShort + " removed manually.(Self Service)");
                    ReadMembershipList();
                }
                break;
            default:
                throw new InvalidOperationException("Not supported: Command \"" + e.CommandName + "\"");
        }

    }


    private void ReadMembershipList ()
    {
        Person displayedPerson = DisplayedPerson();
        Memberships memberships = displayedPerson.GetRecentMemberships(Membership.GracePeriod);

        gridMemberships.DataSource = memberships;
        gridMemberships.DataBind();

    }


    protected void gridMemberships_RowDataBound (object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int membershipId = Convert.ToInt32(this.gridMemberships.DataKeys[e.Row.RowIndex].Value);

            Membership membership = Membership.FromIdentity(membershipId);

            // mark expired memberships with *
            Label LabelExpiredFlag = e.Row.FindControl("LabelExpiredFlag") as Label;
            LinkButton LinkButtonTerminate = e.Row.FindControl("LinkButtonTerminate") as LinkButton;
            LinkButton LinkButtonExtend = e.Row.FindControl("LinkButtonExtend") as LinkButton;
            if (LabelExpiredFlag != null
                && DateTime.Now.AddMinutes(10) > membership.DateTerminated
                && membership.DateTerminated > new DateTime(1902, 01, 01))
            {
                LabelExpiredFlag.Text = "*";
                LinkButtonTerminate.Enabled = false;
                LinkButtonTerminate.Text = membership.DateTerminated.ToString("yyyy-MMM-dd");
            }

            //HACK: New functionality: Self service for members to extend and terminate
            // needs to be verified with organisations.

            // onlu PP and UP members are allowed to extend themselves and  terminate themselves
            if (!(membership.Organization.IsOrInherits(Organization.UPSEid) || membership.Organization.IsOrInherits(Organization.PPSEid)))
            {
                LinkButtonTerminate.Visible = false;
                LinkButtonExtend.Visible = false;
            }
        }
    }
}