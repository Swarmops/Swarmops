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
using Activizr.Basic.Enums;
using Activizr.Logic.Governance;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

public partial class Pages_v4_Organization_ReceivedCandidateDocumentation : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.TextPersonalNumber.Style[HtmlTextWriterStyle.Width] = "300px";
        this.TextPhone.Style[HtmlTextWriterStyle.Width] = "300px";
        this.ComboCandidate.Authority = _currentUser.GetAuthority();

        if (_currentUser.Identity != 1 && _currentUser.Identity != 11443)
        {
            throw new UnauthorizedAccessException("No access to page");
        }

        if (!Page.IsPostBack)
        {
            // nothing for now
        }

    }

    protected void ButtonCandidateLookup_Click(object sender, EventArgs e)
    {
        Person candidate = this.ComboCandidate.SelectedPerson;

        ViewState[this.ClientID + "SelectedCandidate"] = candidate.Identity;
        Dictionary<int, int> ballotLookup = Ballots.GetBallotsForPerson(candidate);

        this.LabelBallots.Text = string.Empty;

        foreach (int ballotId in ballotLookup.Keys)
        {
            Ballot ballot = Ballot.FromIdentity(ballotId);
            this.LabelBallots.Text += String.Format("{0} (position {1})\r\n", ballot.Name, ballotLookup[ballotId]);
        }

        this.TextPersonalNumber.Text = candidate.PersonalNumber;
        this.TextPhone.Text = candidate.Phone;
        this.LabelCandidateName.Text = candidate.Canonical;
    }


    protected void ButtonGo_Click(object sender, EventArgs e)
    {
        Person candidate = Person.FromIdentity((int) ViewState[this.ClientID + "SelectedCandidate"]);
        Election.September2010.SetCandidateDocumented(Organization.PPSE, candidate);

        candidate.PersonalNumber = Formatting.CleanNumber(this.TextPersonalNumber.Text);
        candidate.Phone = Formatting.CleanNumber(this.TextPhone.Text);

        PWEvents.CreateEvent(EventSource.PirateWeb, EventType.CandidateDocumentationReceived,
                                                   _currentUser.Identity, Organization.PPSEid, 1, candidate.Identity,
                                                   Election.September2010.Identity, string.Empty);

        this.TextPersonalNumber.Text = string.Empty;
        this.TextPhone.Text = string.Empty;
        this.LabelBallots.Text = string.Empty;
        this.ComboCandidate.Text = string.Empty;
        this.LabelCandidateName.Text = "---";

        Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('" + HttpUtility.HtmlEncode(candidate.Canonical + " är klarerad att gå till val.").Replace("'", "''") + "');", true);
    }
}
