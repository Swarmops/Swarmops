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
using Activizr.Logic.Governance;
using Activizr.Basic.Interfaces;

public partial class HtmlIncludes_InternalPollCandidates : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        MeetingElectionCandidates candidates = MeetingElectionCandidates.ForPoll(MeetingElection.Primaries2010);

        string pollIdString = Request.QueryString["PollId"];

        if (!string.IsNullOrEmpty(pollIdString))
        {
            int pollId = Int32.Parse(pollIdString);
            candidates = MeetingElectionCandidates.ForPoll(MeetingElection.FromIdentity(pollId));
        }


        this.RepeaterCandidates.DataSource = candidates;
        this.RepeaterCandidates.DataBind();
    }
}
