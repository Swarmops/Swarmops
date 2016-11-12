using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using dotnetCHARTING;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Logic.Governance;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;

public partial class Pages_Public_Charts_InternalPollTurnoutPerCircuit : System.Web.UI.Page
{
	protected void Page_Load(Object sender, EventArgs e)
	{
		// Set the title.
		this.Chart.Title = "Valdeltagande per valkrets";

		string pollIdString = Request.QueryString["PollId"];

		if (pollIdString == null || pollIdString.Length == 0)
		{
			pollIdString = "2";
		}

		Chart.Title += " - " + MeetingElection.FromIdentity(Int32.Parse(pollIdString)).Name;

		// Set the chart Type
		Chart.Type = ChartType.ComboHorizontal;

		// Turn 3D off.
		Chart.Use3D = false;

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Three;

		// Set a default transparency
		Chart.DefaultSeries.DefaultElement.Transparency = 20;

		// Set the x axis scale
		Chart.ChartArea.XAxis.Scale = Scale.Stacked;

		// Set the x axis label
		Chart.ChartArea.XAxis.Label.Text = "Valdeltagande i promille";

		// Set the y axis label
		Chart.ChartArea.YAxis.Label.Text = "";


		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 550;

		if (Request.QueryString["Height"] != null)
		{
			Chart.Height = Int32.Parse(Request.QueryString["Height"]);
		}

		// Add the random data.
		Chart.SeriesCollection.Add(GetRankingData());

		Chart.LegendBox.Position = LegendBoxPosition.None;
		Chart.LegendBox.Template = "%Icon%Name";
		Chart.Debug = false;
		Chart.Mentor = false;
	}


	Series GetRankingData()
	{
		string pollIdString = Request.QueryString["PollId"];

		if (String.IsNullOrEmpty(pollIdString))
		{
		    pollIdString = "2";
		}

		int pollId = Int32.Parse(pollIdString);

		SeriesCollection collection = new SeriesCollection();

		// Gather the data

        Dictionary<int,int> totalVoteCount = new Dictionary<int, int>();
	    Dictionary<int, int> geographyIdCircuit = new Dictionary<int, int>();
	    Dictionary<int, int> peopleGeographies = People.GetPeopleGeographies();
	    Dictionary<int, int> closedVoteCount = Optimizations.GetInternalPollVoteCountsPerGeography(pollId);

	    MeetingElectionVoters voters = MeetingElectionVoters.ForPoll(MeetingElection.FromIdentity(pollId), true);

        foreach (MeetingElectionVoter voter in voters)
        {
            if (!peopleGeographies.ContainsKey(voter.PersonId))
            {
                throw new InvalidOperationException("Person not in geography dictionary");
            }

            int geographyId = peopleGeographies[voter.PersonId];

            if (!geographyIdCircuit.ContainsKey(geographyId))
            {
                Geography geo = Geography.FromIdentity(geographyId);

                while (geo.ParentGeographyId != 0 && !geo.AtLevel(GeographyLevel.ElectoralCircuit))
                {
                    geo = geo.Parent;
                }

                if (geo.ParentGeographyId == 0)
                {
                    geographyIdCircuit[geographyId] = 0;
                }
                else
                {
                    geographyIdCircuit[geographyId] = geo.Identity;
                }
            }

            int circuitId = geographyIdCircuit[geographyId];

            if (circuitId == 0)
            {
                continue;
            }

            if (!totalVoteCount.ContainsKey(circuitId))
            {
                totalVoteCount[circuitId] = 0;
            }

            totalVoteCount[circuitId]++;
        }

        List < GeographyBar > barList = new List<GeographyBar>();

		foreach (int geographyId in totalVoteCount.Keys)
		{
            GeographyBar bar = new GeographyBar();
			bar.Geography = Geography.FromIdentity (geographyId);

		    int thisClosedVoteCount = 0;

            if (closedVoteCount.ContainsKey(geographyId))
            {
                thisClosedVoteCount = closedVoteCount[geographyId];
            }

		    bar.Value = thisClosedVoteCount*1000.0 / totalVoteCount[geographyId];

		    barList.Add(bar);
		}

	    GeographyBar[] bars = barList.ToArray();

		Array.Sort(bars);

		// Populate the data

		Series series = new Series();
		series.Name = "No particular name";
		int count = 0;

		foreach (GeographyBar bar in bars)
		{
			Element element = new Element();
			element.YValue = bar.Value;
			element.Name = bar.Geography.Name;
			element.SmartLabel = new SmartLabel();
			element.SmartLabel.Text = bar.Value.ToString ("N2");

			series.Elements.Add(element);

			count++;
		}

		// Removing the overflow entries afterwards is a bit backwards, but it's due to the sorting

		series.DefaultElement.Color = System.Drawing.Color.DarkViolet;
		series.DefaultElement.ShowValue = true;

		return series;
	}


}
