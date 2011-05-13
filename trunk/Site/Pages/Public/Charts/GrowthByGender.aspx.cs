using System;
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
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;


public partial class Pages_Public_Charts_GrowthByGender : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		// Set the title.
		Chart.Title = "";

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Three;


		// Set the x axis label
		Chart.ChartArea.XAxis.Label.Text = "";

		Chart.XAxis.TimeScaleLabels.Mode = TimeScaleLabelMode.Hidden;
		Chart.XAxis.TimeScaleLabels.RangeIntervals.Add(TimeInterval.Years);

		// Set the y axis label
		Chart.ChartArea.YAxis.Label.Text = "Growth, Percent Female - Piratpartiet SE";

		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

		// Add the random data.
		Chart.SeriesCollection.Add(GetGrowthData());

		Chart.LegendBox.Position = LegendBoxPosition.None;
		Chart.LegendBox.Template = "%Icon%Name";
		Chart.Debug = false;
		Chart.Mentor = false;
	}



	SeriesCollection GetGrowthData()
	{
        string cacheDataKey = "ChartData-AllMembershipEvents";

        MembershipEvents events = (MembershipEvents)Cache.Get(cacheDataKey);

        if (events == null)
        {
            events = MembershipEvents.LoadAll();
            Cache.Insert(cacheDataKey, events, null, DateTime.Today.AddDays(1).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
        }

		SeriesCollection collection = new SeriesCollection();
		DateTime dateIterator = new DateTime(2006, 1, 2);

		string timeFocus = Request.QueryString["DaysHistory"];

		if (timeFocus != null)
		{
			dateIterator = DateTime.Now.Date.AddDays(-Int32.Parse(timeFocus));
		}

		Series series = new Series();

		DateTime today = DateTime.Now.Date;
		int eventIndex = 0;

		Element newElement = new Element();
		newElement.XDateTime = dateIterator;
		int growthCount = 0;
		int femaleGrowthCount = 0;

		int[] growthSeries = new int[4];
		int[] femaleGrowthSeries = new int[4];
		int seriesIndex = 0;

		while (dateIterator < today)
		{
			if (eventIndex >= events.Count)
			{
				break;
			}

            if (events[eventIndex].OrganizationId == Organization.PPSEid)
			{
				if (events [eventIndex].DeltaCount > 0)
				{
					growthCount += events[eventIndex].DeltaCount;

					if (events [eventIndex].Gender == PersonGender.Female)
					{
						femaleGrowthCount += events[eventIndex].DeltaCount;
					}
				}
			}

			eventIndex++;

			if (eventIndex < events.Count && events[eventIndex].DateTime.Date.AddDays(7) >= dateIterator)
			{
				dateIterator = dateIterator.AddDays(7);

				growthSeries[seriesIndex] = growthCount;
				femaleGrowthSeries[seriesIndex] = femaleGrowthCount;

				seriesIndex++;
				if (seriesIndex >= growthSeries.Length)
				{
					seriesIndex = 0;
				}

				int growthTotal = 0;
				int femaleGrowthTotal = 0;

				for (int seriesLoop = 0; seriesLoop < growthSeries.Length; seriesLoop++)
				{
					growthTotal += growthSeries[seriesLoop];
					femaleGrowthTotal += femaleGrowthSeries[seriesLoop];
				}

				newElement.YValue = (double) femaleGrowthTotal/(double) growthTotal * 100.0;
				series.Elements.Add (newElement);

				newElement = new Element();
				newElement.XDateTime = dateIterator;
				growthCount = femaleGrowthCount = 0;
			}
		}

		Chart.YAxis.Minimum = 0;

		series.Type = SeriesType.Spline;
		series.Line.Width = 5;
		series.DefaultElement.Color = Color.DarkBlue;

		collection.Add(series);

		return collection;

	}






}
