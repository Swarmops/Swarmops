using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Security;

using Membership = Activizr.Logic.Pirates.Membership;
using Activizr.Logic.Structure;
using System.Text;

public partial class Pages_Special_SE_VerifyForumPoll : PageV4Base
{
    protected void Page_Load (object sender, EventArgs e)
    {
        if (!_authority.HasPermission(Permission.CanValidatePolls, 1, -1, Authorization.Flag.ExactOrganization))
        {
            throw new UnauthorizedAccessException();
        }

        // Lock out Rick specifically. The timing of this is verifiable through the Subversion repository history.

        if (_currentUser.Identity == 1)
        {
            throw new UnauthorizedAccessException();
        }
    }



    protected void ButtonLookup_Click (object sender, EventArgs e)
    {
        string url = this.TextForumUrl.Text;

        Regex re = new Regex(@"t=(?<trheadid>\d+)");
        int topicId = 0;
        Int32.TryParse(re.Match(url).Groups["trheadid"].Value, out topicId);

        if (topicId == 0)
        {
            LiteralResults.Text = "No valid thread Id recognized";
            return;
        }

        DateTime startdate = DateTime.Today;
        if (TextBoxDate.Text.Trim() != "")
            DateTime.TryParse(TextBoxDate.Text, out startdate);
        else
            TextBoxDate.Text = startdate.ToString("yyyy-MM-dd");

        Activizr.Logic.Special.Sweden.IForumDatabase forumDb = Activizr.Logic.Special.Sweden.SwedishForumDatabaseVBulletin.GetDatabase();

        int pollId = forumDb.GetPollIdFromTopicId(topicId);

        Boolean pollStillOpen = false;
        Dictionary<string, People> votes = new Dictionary<string, People>();


        try
        {
            votes = forumDb.GetPollVotes(pollId);
        }
        catch (ArgumentOutOfRangeException)
        {
            pollStillOpen = true;
        }

        StringBuilder resultSB = new StringBuilder();

        if (pollStillOpen)
        {
            resultSB.Append("Sorry, that poll seems to be still open for voting.");
        }
        else
        {
            People allVoters = new People();

            int invalidVotes = 0;
            int validVoters = 0;
            int dateFailVoters = 0;

            foreach (People people in votes.Values)
            {
                foreach (Person person in people)
                {
                    if (person == null) invalidVotes++;
                }
                allVoters = People.LogicalOr(allVoters, people);
            }



            foreach (Person person in allVoters)
            {
                if (person == null)
                {
                }
                else
                {

                    Membership memberSince = null;

                    Memberships memberships = person.GetMemberships(true);

                    bool membershipFound = false;

                    foreach (Membership membership in memberships)
                    {
                        if (membership.OrganizationId == Organization.PPSEid)
                        {
                            // If was menber two months ago, remember that startdate, not the one for the current membership.
                            // (to handle delayed renewals)
                            if (membership.Active == false
                                && membership.DateTerminated > startdate.AddMonths(-2)
                                && membership.MemberSince <= startdate.AddMonths(-2))
                            {
                                if (memberSince == null)
                                    memberSince = membership;

                                if (membership.Active == false
                                && membership.DateTerminated >= startdate
                                && membership.MemberSince <= startdate)
                                {
                                    membershipFound = true; //must be member at startdate
                                }
                            }
                            else if (membership.Active == false
                                && membership.DateTerminated >= startdate
                                && membership.MemberSince <= startdate)
                            {
                                membershipFound = true; //must be member at startdate
                                if (memberSince == null)
                                    memberSince = membership;

                            }
                            else if (membership.Active == true)
                            {
                                if (memberSince == null)
                                    memberSince = membership;

                                membershipFound = true; //must be member now
                            }
                        }
                    }
                    if (membershipFound)
                    {
                        if (memberSince.MemberSince.Date.AddMonths(2) > startdate)
                        {
                            resultSB.Append("Medlem #" + person.Identity + ", " + Server.HtmlEncode(person.Geography.Name) +
                                      ", medlem sedan " + memberSince.MemberSince.ToString("yyyy-MM-dd") + " (mindre &auml;n stadgad tid f&ouml;r r&ouml;str&auml;tt)<br/>");
                            dateFailVoters++;
                        }
                        else
                        {
                            resultSB.Append("Medlem #" + person.Identity + ", " + Server.HtmlEncode(person.Geography.Name) +
                                      ", medlem sedan " + memberSince.MemberSince.ToString("yyyy-MM-dd") + "<br/>");
                            validVoters++;
                        }
                    }
                    else
                    {
                        if (memberSince != null)
                            resultSB.Append("Medlem #" + person.Identity + " har ett nyligen utg&aring;nget medlemskap (" + memberSince.DateTerminated.ToString("yyyy-MM-dd") + "), r&auml;kna bort fr&aring;n antalet ovan.<br/>");
                        else
                            resultSB.Append("Medlem #" + person.Identity + " har ett nyligen utg&aring;nget medlemskap, r&auml;kna bort fr&aring;n antalet ovan.<br/>");
                        dateFailVoters++;
                    }
                }
            }

            if (invalidVotes > 0)
                resultSB.Insert(0, invalidVotes.ToString() + " r&ouml;ster med ok&auml;nt medlemskap<br/><br/>");
            resultSB.Insert(0, validVoters.ToString() + " r&ouml;ster med giltiga medlemskap:<br/><br/>");

            resultSB.Insert(0, "[h3]R&ouml;stl&auml;ngd[/h3]<br>");

            if (true || WSGeographyTreeDropDown1.SelectedGeographyId != Geography.RootIdentity)
            {
                int selectedGeo = WSGeographyTreeDropDown1.SelectedGeographyId;
                resultSB.Append("<br/>R&ouml;ster fr&aring;n geografier:<br/>");
                resultSB.Append("Vald (giltig) geografi: " + WSGeographyTreeDropDown1.SelectedGeography.Name + "<br/>");

                foreach (string voteAlternative in votes.Keys)
                {
                    resultSB.Append(Server.HtmlEncode(voteAlternative) + ": ");
                    Dictionary<string, int> geoResult = new Dictionary<string, int>();
                    geoResult["Korrekt"] = 0;
                    geoResult["Annan"] = 0;
                    geoResult["Under 2 m&aring;n"] = 0;
                    geoResult["Ej medlem"] = 0;
                    geoResult["Totalt"] = 0;

                    foreach (Person person in votes[voteAlternative])
                    {
                        if (person == null) continue;

                        DateTime memberSince = DateTime.MinValue;

                        Memberships memberships = person.GetMemberships(true);
                        bool membershipFound = false;

                        foreach (Membership membership in memberships)
                        {
                            if (membership.OrganizationId == Organization.PPSEid)
                            {
                                // If was menber two months ago, remember that startdate, not the one for the current membership.
                                // (to handle delayed renewals)
                                if (membership.Active == false
                                    && membership.DateTerminated > startdate.AddMonths(-2)
                                    && membership.MemberSince <= startdate.AddMonths(-2))
                                {
                                    if (memberSince == DateTime.MinValue)
                                        memberSince = membership.MemberSince;

                                    if (membership.Active == false
                                        && membership.DateTerminated >= startdate
                                        && membership.MemberSince <= startdate)
                                    {
                                        membershipFound = true; //must be member at startdate
                                    }
                                }
                                else if (membership.Active == false
                                         && membership.DateTerminated >= startdate
                                         && membership.MemberSince <= startdate)
                                {
                                    membershipFound = true; //must be member at startdate
                                    if (memberSince == DateTime.MinValue)
                                        memberSince = membership.MemberSince;

                                }
                                else if (membership.Active == true)
                                {
                                    if (memberSince == DateTime.MinValue)
                                        memberSince = membership.MemberSince;

                                    membershipFound = true; //must be member now

                                }
                            }
                        }

                        if (!membershipFound)
                        {
                            geoResult["Ej medlem"]++;
                        }
                        else if (memberSince.Date.AddMonths(2) > startdate)
                        {
                            geoResult["Under 2 m&aring;n"]++;
                        }
                        else if (person.Geography != null && (person.Geography.Inherits(selectedGeo) || person.Geography.Identity == selectedGeo))
                        {
                            geoResult["Korrekt"]++;
                        }
                        else
                        {
                            geoResult["Annan"]++;
                        }
                        geoResult["Totalt"]++;
                    }
                    foreach (string kat in geoResult.Keys)
                    {
                        resultSB.Append("&nbsp;&nbsp; " + kat + ":&nbsp;" + geoResult[kat]);
                    }
                    resultSB.Append("<br/>");
                }
            }
        }
        this.LiteralResults.Text = resultSB.ToString();
    }
}
