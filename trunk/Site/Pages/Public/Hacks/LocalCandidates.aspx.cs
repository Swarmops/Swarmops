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
using Activizr.Logic.Pirates;

public partial class Pages_Public_Hacks_LocalCandidates : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Dictionary<int, bool> personLookup = new Dictionary<int, bool>();

        for (int pollId=9; pollId <= 36; pollId++)
        {
            MeetingElectionCandidates candidates = MeetingElectionCandidates.ForPoll(MeetingElection.FromIdentity(pollId));

            foreach (MeetingElectionCandidate candidate in candidates)
            {
                personLookup[candidate.PersonId] = true;
            }
        }

        foreach (int personId in personLookup.Keys)
        {
            this.LiteralMailAddresses.Text += Person.FromIdentity(personId).Email + "<br/>";
        }
    }
}
