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

public partial class Pages_v4_Organization_EditBallot : PageV4Base
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.TextCandidates.Style[HtmlTextWriterStyle.Width] = "100%";

        if (_currentUser.Identity != 1 && _currentUser.Identity != 11443 && _currentUser.Identity != 967)
        {
            throw new UnauthorizedAccessException("No access to page");
        }

        if (!Page.IsPostBack)
        {
            Ballots ballots = Ballots.ForElection(Election.September2010);

            foreach (Ballot ballot in ballots)
            {
                this.DropBallots.Items.Add(new ListItem(ballot.Name, ballot.Identity.ToString()));
            }
        }
    }

    protected void ButtonLookup_Click(object sender, EventArgs e)
    {
        string identityString = DropBallots.SelectedValue;
        Ballot ballot = Ballot.FromIdentity(Int32.Parse(identityString));

        ViewState[this.ClientID + "SelectedBallot"] = ballot.Identity;

        this.TextBallotCount.Text = ballot.Count.ToString();
        this.TextDeliveryAddress.Text = ballot.DeliveryAddress;
        this.LabelBallotName.Text = ballot.Name;
        this.LabelGeography.Text = ballot.Geography.Name;

        PopulateCandidates();
    }

    private void PopulateCandidates()
    {
        Ballot ballot = Ballot.FromIdentity((int) ViewState[this.ClientID + "SelectedBallot"]);
        string candidateText = string.Empty;

        People candidates = ballot.Candidates;
        int listPosition = 1;

        foreach (Person candidate in candidates)
        {
            string cityName = string.Empty;

            // Get city name

            Cities cities = Cities.FromPostalCode(candidate.PostalCode, candidate.CountryId);

            if (cities.Count == 0)
            {
                try
                {
                    cityName = City.FromName(candidate.CityName, candidate.CountryId).Name;
                }
                catch (ArgumentException)
                {
                    // ignore
                }
            }
            else
            {
                cityName = cities[0].Name; // This may be adjusted manually, but will work in 99.9% of cases
            }

            if (cityName.Length < 2)
            {
                cityName = candidate.CityName;
            }

            if (cityName.Contains("("))
            {
                cityName = cityName.Split('(')[0].Trim();
            }

            int age = 2010 - candidate.Birthdate.Year;
            if (new DateTime (2010,9,19) < new DateTime(2010, candidate.Birthdate.Month, candidate.Birthdate.Day))
            {
                age--;
            }


            candidateText += string.Format("{0,2}. {1}, {2}, {3} \xE5r\r\n", listPosition++, candidate.Canonical,
                                           cityName, age);
        }

        this.TextCandidates.Text = candidateText;
    }


    protected void ButtonSave_Click(object sender, EventArgs e)
    {
        Organization org = Organization.PPSE;
        Election election = Election.September2010;

        Ballot ballot = Ballot.FromIdentity((int) ViewState[this.ClientID + "SelectedBallot"]);
        ballot.ClearCandidates();

        ballot.DeliveryAddress = this.TextDeliveryAddress.Text;
        ballot.Count = Int32.Parse(this.TextBallotCount.Text);

        string[] candidateNames = this.TextCandidates.Text.Split('\r', '\n');

        foreach (string candidateNameSource in candidateNames)
        {
            if (candidateNameSource.Trim().Length < 3)
            {
                continue;
            }

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
                personId = 1;
            }

            ballot.AddCandidate(Person.FromIdentity(personId));
        }

        PopulateCandidates();

        Page.ClientScript.RegisterStartupScript(typeof(Page), "OkMessage", @"alert ('Valsedel #" + ballot.Identity + " sparades.');", true);
    }
}
