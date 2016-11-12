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

public partial class Pages_Public_Charts_Activism2010PerCircuit : System.Web.UI.Page
{
	protected void Page_Load(Object sender, EventArgs e)
	{
		// Set the title.
		this.Chart.Title = "Aktivism 2010 per valkrets";

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
		Chart.ChartArea.XAxis.Label.Text = "Antal aktivismbilder från valkretsen";

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
		SeriesCollection collection = new SeriesCollection();

		// Gather the data

	    Dictionary<int, int> geographyIdCircuit = new Dictionary<int, int>();
	    Dictionary<int, int> circuitActivismCount = new Dictionary<int, int>();

	    ExternalActivities activities = ExternalActivities.ForOrganization(Organization.PPSE);

        foreach (ExternalActivity activity in activities)
        {
            int geographyId = activity.GeographyId;

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

            if (!circuitActivismCount.ContainsKey(circuitId))
            {
                circuitActivismCount[circuitId] = 0;
            }

            circuitActivismCount[circuitId]++;
        }

        List < GeographyBar > barList = new List<GeographyBar>();

		foreach (int geographyId in circuitActivismCount.Keys)
		{
            GeographyBar bar = new GeographyBar();
			bar.Geography = Geography.FromIdentity (geographyId);

		    bar.Value = circuitActivismCount[geographyId];
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
			element.SmartLabel.Text = bar.Value.ToString();

			series.Elements.Add(element);

			count++;
		}

		// Removing the overflow entries afterwards is a bit backwards, but it's due to the sorting

		series.DefaultElement.Color = System.Drawing.Color.DarkViolet;
		series.DefaultElement.ShowValue = true;

		return series;
	}


}
