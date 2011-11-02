using System;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using dotnetCHARTING;

using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;


public partial class Pages_Public_Charts_MemberCount : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		// Set the title.
		Chart.Title = "";

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Three;


		// Set the x axis label
		Chart.ChartArea.XAxis.Label.Text = "";

		Chart.XAxis.TimeScaleLabels.Mode = TimeScaleLabelMode.Smart;

		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

		// Add the random data.
		Chart.SeriesCollection.Add(GetGrowthData());

		Chart.LegendBox.Position = LegendBoxPosition.None;
		Chart.Debug = false;
		Chart.Mentor = false;
	}



	SeriesCollection GetGrowthData()
	{
		string orgIdString = Request.QueryString["OrgId"];
		string recurseTreeString = Request.QueryString["RecurseTree"];
		string color = Request.QueryString["Color"];

		if (color == null)
		{
			color = "Lavender";
		}

        int orgId = Organization.PPSEid;
		bool recurseTree = false;

		if (orgIdString != null)
		{
			orgId = Int32.Parse(orgIdString);
		}

		if (recurseTreeString == "1")
		{
			recurseTree = true;
		}

        // Do we have this data in cache already?



		Chart.ChartArea.YAxis.Label.Text = "Medlemsantal - " + Organization.FromIdentity(orgId).Name;

		Organizations organizations = null;

		if (recurseTree)
		{
			organizations = Organization.FromIdentity(orgId).GetTree();
		}
		else
		{
			organizations = Organizations.FromSingle(Organization.FromIdentity(orgId));
		}


		MembershipEvents events = MembershipEvents.LoadAll();

		Dictionary<int, bool> lookup = new Dictionary<int, bool>();

		foreach (Organization org in organizations)
		{
			lookup[org.Identity] = true;
		}


		SeriesCollection collection = new SeriesCollection();
		DateTime dateIterator = new DateTime(2006, 1, 1);

		string timeFocus = Request.QueryString["DaysHistory"];

		if (timeFocus != null)
		{
			dateIterator = DateTime.Now.Date.AddDays(-Int32.Parse(timeFocus));
		}

		Series series = new Series();
		series.Name = "";
		DateTime today = DateTime.Now.Date;
		int eventIndex = 0;
		int currentCount = 0;

		while (dateIterator < today)
		{
			DateTime nextDate = dateIterator.AddDays(1);
			while (eventIndex < events.Count && events[eventIndex].DateTime < nextDate)
			{
				if (lookup.ContainsKey(events[eventIndex].OrganizationId))
				{
					currentCount += events[eventIndex].DeltaCount;
				}

				eventIndex++;
			}

			Element newElement = new Element();
			newElement.XDateTime = dateIterator;
			newElement.YValue = currentCount;
			series.Elements.Add(newElement);
			dateIterator = nextDate;
		}

		collection.Add(series);

		collection[0].DefaultElement.Color = Color.FromName (color);

		return collection;

	}






}
