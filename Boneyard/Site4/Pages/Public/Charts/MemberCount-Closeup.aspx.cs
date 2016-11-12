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

using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;


public partial class Pages_Public_Charts_MemberCount_Closeup : System.Web.UI.Page
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
        Chart.XAxis.TimeScaleLabels.DayFormatString = "dd";
        Chart.XAxis.TimeScaleLabels.MonthFormatString = "dd";
        Chart.XAxis.TimeInterval = TimeInterval.Day;


		// Set the y axis label
		Chart.ChartArea.YAxis.Label.Text = "Member Count Closeup - Piratpartiet SE";

		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

		// Add the random data.
		Chart.SeriesCollection.Add(GetGrowthData());

		//Chart.LegendBox.Position = LegendBoxPosition.None;
		Chart.LegendBox.Template = "%Icon%Name";
        Chart.LegendBox.Visible = false;
		Chart.Debug = false;
		Chart.Mentor = false;
	}



	SeriesCollection GetGrowthData()
	{
        string cacheDataKey = "ChartData-AllMembershipEvents-5min";

        MembershipEvents events = (MembershipEvents)Cache.Get(cacheDataKey);

        if (events == null)
        {
            events = MembershipEvents.LoadAll();
            //Cache.Insert(cacheDataKey, events, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan (0, 5, 0));
        }

		/*
		using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Data/MembershipEvents.xml")))
		{
			string xml = reader.ReadToEnd();

			events = MembershipEvents.FromXml(xml);
		}*/


        DateTime endDate = DateTime.Now.Date;
        SeriesCollection collection = new SeriesCollection();
        DateTime dateIterator = endDate.AddDays(-1);

        string timeFocus = Request.QueryString["DaysHistory"];

		if (timeFocus != null)
		{
			dateIterator = endDate.Date.AddDays(-Int32.Parse(timeFocus));
		}

		Series series = new Series();
		series.Name = "Each event";

		Series seriesAverage = new Series();
		seriesAverage.Name = "Daily avg";

		int maxValue = 0;
		int minValue = Int32.MaxValue;
		int eventIndex = 0;
		int currentCount = 0;

        while (events.Count > 0 &&  events[eventIndex].DateTime < dateIterator && eventIndex < events.Count)
		{
            if (events[eventIndex].OrganizationId == Organization.PPSEid)
			{
				currentCount += events[eventIndex].DeltaCount;
			}

			eventIndex++;
		}

		Element newElement = new Element();
		newElement.XDateTime = dateIterator;
		newElement.YValue = currentCount;
		seriesAverage.Elements.Add(newElement);

		while (dateIterator < endDate.AddDays (1))
		{
			if (eventIndex >= events.Count)
			{
				break;
			}

            if (events[eventIndex].OrganizationId == Organization.PPSEid)
            {
                currentCount += events[eventIndex].DeltaCount;

                if (currentCount < minValue)
                {
                    minValue = currentCount;
                }

                if (currentCount > maxValue)
                {
                    maxValue = currentCount;
                }

                newElement = new Element();
                newElement.XDateTime = events[eventIndex].DateTime;
                newElement.YValue = currentCount;
                series.Elements.Add(newElement);
            }

			eventIndex++;

			if (eventIndex < events.Count && events[eventIndex].DateTime.Date != dateIterator)
			{
				dateIterator = dateIterator.AddDays(1);

				newElement = new Element();
				newElement.XDateTime = dateIterator;
				newElement.YValue = currentCount;
				seriesAverage.Elements.Add(newElement);
			}
		}

		Chart.YAxis.Maximum = (maxValue + 99) / 100 * 100;
        Chart.YAxis.Minimum = minValue / 100 * 100;

		series.Type = SeriesType.Spline;
		series.Line.Width = 5;
		series.DefaultElement.Color = Color.DarkGreen;

		seriesAverage.Type = SeriesType.Spline;
		seriesAverage.Line.Width = 3;
		seriesAverage.DefaultElement.Color = Color.Yellow;

		collection.Add(series);
		collection.Add(seriesAverage);

		return collection;

	}






}
