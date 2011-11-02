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
using PirateWeb.Logic.Governance;
using PirateWeb.Logic.Pirates;
using PirateWeb.Logic.Structure;

public partial class Pages_Public_Hacks_Ballots2010 : System.Web.UI.Page
{
    private Dictionary<int, bool> documentedCandidatesLookup;

    protected void Page_Load(object sender, EventArgs e)
    {
        documentedCandidatesLookup = new Dictionary<int, bool>();
        People documentedCandidates = Election.September2010.GetDocumentedCandidates(Organization.PPSE);

        foreach (Person person in documentedCandidates)
        {
            documentedCandidatesLookup[person.Identity] = true;
        }

        this.RepaterBallots.DataSource = Ballots.ForElection(Election.September2010);
        this.RepaterBallots.DataBind();
    }


    protected void RepeaterBallots_ItemDataBound(object sender,
    System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        RepeaterItem item = e.Item;
        if ((item.ItemType == ListItemType.Item) ||
            (item.ItemType == ListItemType.AlternatingItem))
        {
            Repeater repeaterCandidates = (Repeater)item.FindControl("RepeaterCandidates");
            Ballot ballot = (Ballot)item.DataItem;

            List<CandidateLine> lines = new List<CandidateLine>();
            People candidates = ballot.Candidates;
            int position = 1;

            foreach (Person candidate in candidates)
            {
                lines.Add( new CandidateLine(position, candidate, documentedCandidatesLookup.ContainsKey(candidate.Identity)));
                position++;
            }

            repeaterCandidates.DataSource = lines;
            repeaterCandidates.DataBind();
        }
    }


    protected class CandidateLine
    {
        public CandidateLine(int position, Person person, bool ok)
        {
            this.position = position;
            this.person = person;
            this.ok = ok;
        }

        private Person person;
        private int position;
        private bool ok;

        public Person Person { get { return this.person; } }
        public string Position { get { return string.Format("{0,2}", this.position); } }
        public string Ok { get { return ok ? "OK" : "--"; } }
        public int Age
        {
            get
            {
                int age = 2010 - person.Birthdate.Year;
                if (new DateTime(2010, 9, 19) < new DateTime(2010, person.Birthdate.Month, person.Birthdate.Day))
                {
                    age--;
                }

                return age;
            }
        }

        public string CityName
        {
            get
            {
                string cityName = string.Empty;

                // Get city name

                Cities cities = Cities.FromPostalCode(person.PostalCode, person.CountryId);

                if (cities.Count == 0)
                {
                    try
                    {
                        cityName = City.FromName(person.CityName, person.CountryId).Name;
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
                    cityName = person.CityName;
                }

                if (cityName.Contains("("))
                {
                    cityName = cityName.Split('(')[0].Trim();
                }

                return cityName;
            }
        }
    }
}
