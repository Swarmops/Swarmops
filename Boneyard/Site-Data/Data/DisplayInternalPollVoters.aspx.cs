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
using Activizr.Logic.Governance;

public partial class Data_DisplayInternalPollVoters : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string parameterString = Request["PollId"];

        if (!String.IsNullOrEmpty(parameterString))
        {
            int pollId = Int32.Parse(parameterString);
            MeetingElection poll = MeetingElection.FromIdentity(pollId);
            Page.Title = "Voter Verification - Internal Poll - " + poll.Name;
            this.LabelPollName.Text = poll.Name;
            if (poll.VotingCloses > DateTime.Now)
            {
                throw new UnauthorizedAccessException("Voting for this poll is not yet closed.");
            }

            MeetingElectionVoters voters = poll.GetClosedVoters();

            List<string> voterList = new List<string>();

            foreach (MeetingElectionVoter voter in voters)
            {
                // Must not disclose member identities -- we use initials and person Id, not full name!

                voterList.Add(voter.Person.Initials + " (#" + voter.PersonId.ToString() + ")");
            }

            CultureInfo previousCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(poll.Organization.DefaultCountry.Culture);
            voterList.Sort();
            System.Threading.Thread.CurrentThread.CurrentCulture = previousCulture;

            this.RepeaterVoters.DataSource = voterList.ToArray();
            this.RepeaterVoters.DataBind();

            this.LabelVoterCount.Text = voters.Count.ToString();
        }

    }
}
