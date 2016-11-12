using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Basic.Types;
using Activizr.Logic.Governance;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Telerik.Web.UI;

public partial class Pages_v4_Admin_PlaceInternalVote : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            string pollIdParameter = Request.QueryString["PollId"];

            if (string.IsNullOrEmpty(pollIdParameter))
            {
                PopulatePollGrid();
                this.PanelPollList.Visible = true;
                this.PanelVoting.Visible = false;
                this.PanelPollIntro.Visible = false;
            }
            else
            {
                MeetingElection poll = MeetingElection.FromIdentity(Int32.Parse(pollIdParameter));
                this.LabelPollName1.Text = poll.Name;

                InternalPollVoterStatus voterStatus = poll.GetVoterStatus(_currentUser);

                if (!poll.VotingOpen)
                {
                    this.PanelVoting.Visible = false;
                    this.PanelVotingClosed.Visible = true;
                }
                else if (voterStatus == InternalPollVoterStatus.NotEligibleForPoll)
                {
                    this.PanelVoting.Visible = false;
                    this.PanelNoVote.Visible = true;
                }
                else if (voterStatus == InternalPollVoterStatus.HasAlreadyVoted)
                {
                    string verificationCode = Request.QueryString["VerificationCode"];

                    if (string.IsNullOrEmpty(verificationCode))
                    {
                        this.PanelVoting.Visible = false;
                        this.PanelComplete.Visible = true;
                        this.PanelEnterCode.Visible = true;
                    }
                    else
                    {
                        // The voter wishes to redo his/her vote

                        PopulateLists(poll, verificationCode);
                        this.ListVote.DataBind();
                        this.ListCandidates.DataBind();
			this.ButtonVote.Enabled = true;
                    }
                }
                else // open; the voter has not yet voted
                {
                    PopulateLists(poll, string.Empty);

                    this.ListCandidates.DataBind();
                }
            }
        }
    }


    public void PopulateLists(MeetingElection poll, string verificationCode)
    {
        MeetingElectionCandidates candidates = poll.Candidates;
        Dictionary<int, bool> lookup = new Dictionary<int, bool>();

        if (!string.IsNullOrEmpty(verificationCode))
        {
            MeetingElectionVote vote = MeetingElectionVote.FromVerificationCode(verificationCode);

            if (poll.Identity != vote.InternalPollId)
            {
                throw new ArgumentException("Verification Code does not exist or does not match Poll Identity");
            }

            int[] candidateIds = vote.SelectedCandidateIdsInOrder;

            foreach (int candidateId in candidateIds)
            {
                lookup[candidateId] = true;

                MeetingElectionCandidate candidate = MeetingElectionCandidate.FromIdentity(candidateId);

                this.ListVote.Items.Add(new RadListBoxItem(candidate.Person.Canonical,
                                                             candidate.Identity.ToString()));
            }
        }


        foreach (MeetingElectionCandidate candidate in candidates)
        {
            if (!lookup.ContainsKey(candidate.Identity))
            {

                this.ListCandidates.Items.Add(new RadListBoxItem(candidate.Person.Canonical,
                                                                 candidate.Identity.ToString()));
            }
        }

        this.ListCandidates.DataBind();
    }


    public int SortCandidates (MeetingElectionCandidate candidate1, MeetingElectionCandidate candidate2)
    {
        return String.Compare(candidate1.PersonName, candidate2.PersonName, true, new CultureInfo("sv-SE"));
    }


    protected int countPlace;


    /*
    protected void ListCandidates_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.ListCandidates.SelectedIndex >= 0)
        {
            int candidateId = Int32.Parse(this.ListCandidates.SelectedValue);

            FocusOnCandidate(candidateId);
        }
    }

    protected void ListVote_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.ListVote.SelectedIndex >= 0)
        {
            int candidateId = Int32.Parse(this.ListVote.SelectedValue);

            FocusOnCandidate(candidateId);
        }
    }*/

    /*
    private void FocusOnCandidate(int candidateId)
    {
        MeetingElectionCandidate candidate = MeetingElectionCandidate.FromIdentity(candidateId);

        this.LiteralSelectedCandidatePhoto.Text =
            "<img src=\"http://data.piratpartiet.se/Handlers/DisplayPortrait.aspx?YSize=320&PersonId=" +
            candidate.PersonId.ToString() + "\" />";

        this.LabelSelectedCandidateStatement.Text = candidate.CandidacyStatement;
    }*/

    protected void ListCandidates_ItemCreated(object sender, Telerik.Web.UI.RadListBoxItemEventArgs e)
    {
        if (e == null)
        {
            return;
        }

        string itemValue = e.Item.Value;

        if (itemValue.Length < 1)
        {
            return;
        }

        int identity = Int32.Parse(itemValue);

        MeetingElectionCandidate candidate = MeetingElectionCandidate.FromIdentity(identity);
        /*
        if (candidate == null)
        {
            return;
        }

        Label label = (Label) e.Item.FindControl("LabelCandidate");
        label.Text = candidate.Person.Canonical;*/
    }

    /*
    protected void ListCandidates_Transferred(object sender, Telerik.Web.UI.RadListBoxTransferredEventArgs e)
    {
        // this.ListCandidates.SortItems();

        foreach (RadListBoxItem item in e.Items)
        {
            //Databind the item in order to evaluate the databinding expressions from the template
            item.DataBind();
        }

        this.ButtonVote.Text = "Färdig!";
        this.ButtonVote.Enabled = this.ListVote.Items.Count > 0 && this.ListVote.Items.Count <= 22 ? true : false;

        this.ListVote.DataBind();
    }*/

    /*
    protected void ListVote_Transferred(object sender, Telerik.Web.UI.RadListBoxTransferredEventArgs e)
    {
        foreach (RadListBoxItem item in e.Items)
        {
            //Databind the item in order to evaluate the databinding expressions from the template
            item.DataBind();
        }

        this.ListVote.DataBind(); // Re-bind for ordering

        this.ButtonVote.Text = "Färdig!";
        this.ButtonVote.Enabled = this.ListVote.Items.Count > 0 && this.ListVote.Items.Count <= 22 ? true : false;
    }*/

    protected void ListVote_Reordered(object sender, RadListBoxEventArgs e)
    {
        this.ListVote.DataBind();
    }

    protected void ButtonVote_Click(object sender, EventArgs e)
    {
        MeetingElection poll = MeetingElection.FromIdentity(Int32.Parse(Request.QueryString["PollId"]));
        if (!poll.VotingOpen)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "nope",
                                                "alert ('This poll has closed. You can not cast the vote.');",
                                                true);
            return;
        }


        if (this.ListVote.Items.Count == 0)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "nope",
                                                "alert ('You need to pick one or more candidates in order to cast a vote.');",
                                                true);
            return;
        }

        if (this.ListVote.Items.Count > poll.MaxVoteLength)
        {
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "nope",
                                                "alert ('You have chosen too many candidates. The maximum number is " + poll.MaxVoteLength.ToString() + ". Please remove at least " + (this.ListVote.Items.Count - poll.MaxVoteLength).ToString() +  ".');",
                                                true);
            return;
        }

        string verificationCode = Request.QueryString["VerificationCode"];
        InternalPollVoterStatus voterStatus = poll.GetVoterStatus(_currentUser);

        if (voterStatus == InternalPollVoterStatus.CanVote || (!string.IsNullOrEmpty(verificationCode) && voterStatus == InternalPollVoterStatus.HasAlreadyVoted))
        {
            this.PanelPollIntro.Visible = false;
            this.PanelVoting.Visible = false;
            this.PanelComplete.Visible = true;
            this.PanelCode.Visible = true;

            MeetingElectionVote vote = null;
            
            if (string.IsNullOrEmpty(verificationCode) && voterStatus == InternalPollVoterStatus.CanVote)
            {
                vote = poll.CreateVote(_currentUser, Request.UserHostAddress.ToString());    
            }
            else
            {
                vote = MeetingElectionVote.FromVerificationCode(verificationCode);
                vote.Clear();
            }
            

            this.LabelReference.Text = vote.VerificationCode;

            for (int index = 0; index < this.ListVote.Items.Count; index++)
            {
                MeetingElectionCandidate candidate =
                    MeetingElectionCandidate.FromIdentity(Int32.Parse(this.ListVote.Items[index].Value));

                vote.AddDetail(index + 1, candidate);
            }
        }
    }

    protected void ButtonChangeVote_Click(object sender, EventArgs e)
    {
        MeetingElection poll = MeetingElection.FromIdentity(Int32.Parse(Request.QueryString["PollId"]));
        InternalPollVoterStatus voterStatus = poll.GetVoterStatus(_currentUser);

        if (voterStatus == InternalPollVoterStatus.HasAlreadyVoted)
        {
            string verificationCode = this.TextVerificationCode.Text;

            try
            {
                MeetingElectionVote vote = MeetingElectionVote.FromVerificationCode(verificationCode);

                if (vote.InternalPollId == poll.Identity)
                {
                    Response.Redirect(Request.RawUrl + "&VerificationCode=" + verificationCode);
                }
            }
            catch (Exception)
            {
                ScriptManager.RegisterStartupScript(this, Page.GetType(), "nope",
                                                    "alert ('There is no such verification code associated with this poll.');",
                                                    true);
            }
        }
    }

    private void PopulatePollGrid()
    {
        MeetingElections polls = MeetingElections.GetAll();

        this.GridPolls.DataSource = polls;
        this.GridPolls.DataBind();
    }


    protected void GridPolls_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            MeetingElection poll = (MeetingElection) e.Item.DataItem;

            if (poll == null)
            {
                return;
            }

            HyperLink editLink = (HyperLink)e.Item.FindControl("LinkVote");
            if (editLink != null)
            {
                editLink.Attributes["href"] = "PlaceInternalVote.aspx?PollId=" + poll.Identity.ToString();
            }

            Label labelOpen = (Label) e.Item.FindControl("LabelVotingOpen");
            labelOpen.Text = poll.VotingOpen? "Ja!" : "Nej";
        }

    }


}
