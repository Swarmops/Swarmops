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


public partial class Pages_Public_Charts_ChurnVsGrowth : System.Web.UI.Page
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

		// Set the y axis label
		Chart.ChartArea.YAxis.Label.Text = "Member Churn vs. Growth - Piratpartiet SE";
		Chart.YAxis.Scale = Scale.Stacked;
		Chart.YAxis.Maximum = 20;
		Chart.YAxis.Minimum = -20;
		Chart.LegendBox.Template = "%Icon%Name";

		string scaleString = Request.QueryString["Scale"];

		if (scaleString != null)
		{
			int scale = Int32.Parse (scaleString);
			Chart.YAxis.Maximum = scale;
			Chart.YAxis.Minimum = -scale;
		}


		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

		Chart.SeriesCollection.Add(GetChurnGrowthData());
		Chart.LegendBox.Position = LegendBoxPosition.Top;
		Chart.Debug = false;
		//Chart.Mentor = false;
	}



	SeriesCollection GetChurnGrowthData()
	{
        string cacheDataKey = "ChartData-AllMembershipEvents";

        MembershipEvents events = (MembershipEvents)Cache.Get(cacheDataKey);

        if (events == null)
        {
            events = MembershipEvents.LoadAll();
            Cache.Insert(cacheDataKey, events, null, DateTime.Today.AddDays(1).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
        }

		DateTime dateIterator = new DateTime(2007, 1, 1);

		string timeFocus = Request.QueryString["DaysHistory"];

		if (timeFocus != null)
		{
			dateIterator = DateTime.Now.Date.AddDays(-Int32.Parse(timeFocus));
		}

		Series seriesGrowth = new Series();
		Series seriesChurn = new Series();
		Series seriesRegrowth = new Series();
		Series seriesDelta = new Series();
		seriesRegrowth.Name = "Regrowth";
		seriesGrowth.Name = "Growth";
		seriesChurn.Name = "Churn";
		seriesDelta.Name = "Delta";
		DateTime today = DateTime.Now.Date;
		int eventIndex = 0;

		Dictionary<int, bool> regrowthLookup = new Dictionary<int, bool>();

		while (events[eventIndex].DateTime < dateIterator)
		{
			eventIndex++;

			if (events[eventIndex].DeltaCount < 0)
			{
				regrowthLookup[events[eventIndex].PersonId] = true;
			}
		}

		while (dateIterator < today)
		{
			int todaysRegrowth = 0;
			int todaysChurn = 0;
			int todaysGrowth = 0;

			DateTime nextDate = dateIterator.AddDays (1);
			while (eventIndex < events.Count && events[eventIndex].DateTime < nextDate)
			{
				if (events[eventIndex].OrganizationId == 1)  // TODO: dynamic org selection
				{
					if (events[eventIndex].DeltaCount > 0)
					{
						if (regrowthLookup.ContainsKey (events[eventIndex].PersonId))
						{
							todaysRegrowth++;
						}
						else
						{
							todaysGrowth++;
						}
					}
					else
					{
						todaysChurn++;
						regrowthLookup[events[eventIndex].PersonId] = true;
					}
				}

				eventIndex++;
			}

			Element newElement = new Element();
			newElement.XDateTime = dateIterator;
			newElement.YValue = todaysGrowth;
			seriesGrowth.Elements.Add(newElement);

			newElement = new Element();
			newElement.XDateTime = dateIterator;
			newElement.YValue = todaysRegrowth;
			seriesRegrowth.Elements.Add(newElement);

			newElement = new Element();
			newElement.XDateTime = dateIterator;
			newElement.YValue = -todaysChurn;
			seriesChurn.Elements.Add(newElement);

			newElement = new Element();
			newElement.XDateTime = dateIterator;
			newElement.YValue = todaysGrowth + todaysRegrowth - todaysChurn;
			seriesDelta.Elements.Add(newElement);

			dateIterator = nextDate;
		}

		seriesRegrowth.DefaultElement.Color = Color.FromArgb(96, 255, 32);
		seriesGrowth.DefaultElement.Color = Color.FromArgb(32, 128, 64);
		seriesChurn.DefaultElement.Color = Color.FromArgb(128, 16, 32);
		seriesDelta.DefaultElement.Color = Color.Yellow;
		seriesDelta.Type = SeriesType.Spline;
		seriesDelta.DefaultElement.Marker = new ElementMarker(ElementMarkerType.None);
		seriesDelta.Line.Width = 1;

		SeriesCollection collection = new SeriesCollection();
		collection.Add(seriesRegrowth);
		collection.Add(seriesGrowth);
		collection.Add(seriesChurn);
		collection.Add(seriesDelta);

		return collection;
	}




}
