using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Activizr.Logic.Governance;

public partial class Data_DisplayInternalPollVotes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string parameterString = Request["PollId"];

        if (!String.IsNullOrEmpty(parameterString))
        {
            int pollId = Int32.Parse(parameterString);
            MeetingElection poll = MeetingElection.FromIdentity(pollId);
            Page.Title = "Vote Verification - Internal Poll - " + poll.Name;
            if (poll.VotingCloses > DateTime.Now)
            {
                throw new UnauthorizedAccessException("Voting for this poll is not yet closed.");
            }

            MeetingElectionVotes votes = MeetingElectionVotes.ForInternalPoll(poll);
            personIdLookup = poll.GetCandidatePersonMap();
            this.RepeaterVotes.DataSource = votes;
            this.RepeaterVotes.DataBind();

            this.LabelVoteCount.Text = votes.Count.ToString();
        }
    }

    protected void RepeaterVotes_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        RepeaterItem item = e.Item;
        if ((item.ItemType == ListItemType.Item) ||
            (item.ItemType == ListItemType.AlternatingItem))
        {
            Repeater innerRepeater = (Repeater)item.FindControl("RepeaterVoteDetails");
            MeetingElectionVote vote = (MeetingElectionVote) item.DataItem;

            innerRepeater.DataSource = vote.SelectedCandidateIdsInOrder;
            innerRepeater.DataBind();
        }

    }

    protected Dictionary<int, int> personIdLookup;  // Needs to be protected for page to access it
}
