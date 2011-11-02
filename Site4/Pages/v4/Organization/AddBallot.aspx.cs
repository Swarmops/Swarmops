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
using Activizr.Logic.Structure;

public partial class Pages_v4_Organization_AddBallot : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.TextName.Style[HtmlTextWriterStyle.Width] = "300px";
        this.DropPolls.Style[HtmlTextWriterStyle.Width] = "300px";
        this.TextCandidates.Style[HtmlTextWriterStyle.Width] = "100%";

        if (_currentUser.Identity != 1)
        {
            throw new UnauthorizedAccessException("No access to page");
        }

        if (!Page.IsPostBack)
        {
            this.DropGeographies.Root = Country.FromCode("SE").Geography;
            this.DropPolls.Items.Clear();

            MeetingElections polls = MeetingElections.ForOrganization(Organization.PPSE);
            foreach (MeetingElection poll in polls)
            {
                this.DropPolls.Items.Add(new ListItem(poll.Name, poll.Identity.ToString()));
            }
        }

    }

    protected void ButtonAdd_Click(object sender, EventArgs e)
    {
        Organization org = Organization.PPSE;
        Geography geo = this.DropGeographies.SelectedGeography;
        Election election = Election.September2010;
        string ballotName = this.TextName.Text;
        MeetingElectionCandidates candidates =
            MeetingElection.FromIdentity(Int32.Parse(this.DropPolls.SelectedValue)).Candidates;

        Ballot ballot = Ballot.Create(election, org, geo, ballotName, 0, string.Empty);

        Dictionary<string, int> nameLookup = new Dictionary<string, int>();

        foreach (MeetingElectionCandidate candidate in candidates)
        {
            nameLookup[candidate.Person.Name.ToLowerInvariant()] = candidate.PersonId;
        }

        string[] candidateNames = this.TextCandidates.Text.Split('\r', '\n');

        foreach (string candidateNameSource in candidateNames)
        {
            int personId = 0;

            int idStartIndex = candidateNameSource.LastIndexOf("(#");

            if (idStartIndex  > 0)
            {
                string identityString = candidateNameSource.Substring(idStartIndex + 2);
                int idEndIndex = identityString.IndexOf(")");

                identityString = identityString.Substring(0, idEndIndex);

                personId = Int32.Parse(identityString);

            }

            if (personId == 0)
            {
                // Derive from name

                string candidateName = candidateNameSource;
                int parenIndex = candidateName.IndexOfAny(new char[] {'(', ','});
                if (parenIndex >= 0)
                {
                    candidateName = candidateName.Substring(0, parenIndex);
                }
                candidateName = candidateName.ToLowerInvariant().Trim();

                if (candidateName.Length < 1)
                {
                    continue;
                }

                personId = nameLookup[candidateName];
            }

            ballot.AddCandidate(Person.FromIdentity(personId));
        }

        Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('Valsedel #" + ballot.Identity + " registrerades.');", true);
    }
}
