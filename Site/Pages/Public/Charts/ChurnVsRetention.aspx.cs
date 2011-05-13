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


public partial class Pages_Public_Charts_ChurnVsRetention : System.Web.UI.Page
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
		Chart.ChartArea.YAxis.Label.Text = "Member Churn vs. Retention - Piratpartiet SE";
		Chart.YAxis.Scale = Scale.FullStacked;
		Chart.LegendBox.Template = "%Icon%Name";


		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

		// Add the random data.
		Chart.SeriesCollection.Add(GetChurnRetentionData());

		Chart.LegendBox.Position = LegendBoxPosition.Top;
		Chart.Debug = false;
		//Chart.Mentor = false;

        Chart.CacheDuration = 5;
	}



	SeriesCollection GetChurnRetentionData()
	{
		DateTime dateIterator = new DateTime(2007, 4, 15);

		string timeFocus = Request.QueryString["DaysHistory"];

		if (timeFocus != null)
		{
			dateIterator = DateTime.Now.Date.AddDays(-Int32.Parse(timeFocus));
		}

        string cacheDataKey = "ChartData-All-PpSe-ChurnData";

        ChurnData data = (ChurnData) Cache.Get(cacheDataKey);

        if (data == null)
        {
            data = ChurnData.ForOrganization(Organization.PPSE);
            Cache.Insert(cacheDataKey, data, null, DateTime.Today.AddDays(1).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
        }

		int[] retentionIntervals = { 30, 21, 14, 7, 2 };

		Series[] seriesRetention = new Series[retentionIntervals.Length + 1];

		for (int index = 0; index < retentionIntervals.Length + 1; index++)
		{
			seriesRetention[index] = new Series();
			if (index == 0)
			{
				seriesRetention[index].Name = (retentionIntervals[0]+1).ToString() + "+d";
			}
			else if (index < retentionIntervals.Length)
			{
				seriesRetention[index].Name = retentionIntervals[index].ToString() + "-" + retentionIntervals[index - 1].ToString() + "d";
			}
			else
			{
				seriesRetention[index].Name = "Last day";
			}
		}


		Series seriesChurn = new Series();
		seriesChurn.Name = "Churn";

		DateTime today = DateTime.Now.Date;
		int eventIndex = 0;

		while (data [eventIndex].ExpiryDate < dateIterator)
		{
			eventIndex++;
		}

		while (dateIterator < today)
		{
			int todaysChurn = 0;
			int[] todaysRetention = new int[seriesRetention.Length]; // array elements initialize to 0

			DateTime nextDate = dateIterator.AddDays (1);
			while (eventIndex < data.Count && data[eventIndex].ExpiryDate < nextDate)
			{
				ChurnDataPoint dataPoint = data[eventIndex];

				if (dataPoint.DataType == ChurnDataType.Churn)
				{
					todaysChurn++;
				}
				else
				{
					int dayCount = (dataPoint.ExpiryDate - dataPoint.DecisionDateTime).Days;

					int seriesIndex = 0;

					while (seriesIndex < retentionIntervals.Length && dayCount < retentionIntervals[seriesIndex])
					{
						seriesIndex++;
					}

					todaysRetention[seriesIndex]++;
				}


				eventIndex++;
			}

			Element newElement = new Element();

			for (int seriesIndex = 0; seriesIndex < seriesRetention.Length; seriesIndex++)
			{
				newElement = new Element();
				newElement.XDateTime = dateIterator;
				newElement.YValue = todaysRetention [seriesIndex];
				seriesRetention [seriesIndex].Elements.Add(newElement);
			}


			newElement = new Element();
			newElement.XDateTime = dateIterator;
			newElement.YValue = todaysChurn;
			seriesChurn.Elements.Add(newElement);

			dateIterator = nextDate;
		}


		SeriesCollection collection = new SeriesCollection();
		for (int seriesIndex = 0; seriesIndex < seriesRetention.Length-1; seriesIndex++)
		{
			seriesRetention [seriesIndex].DefaultElement.Color = InterpolateColor (Color.Green, Color.Yellow, (double) (seriesIndex * 100) / (double) (seriesRetention.Length - 2));
			collection.Add(seriesRetention[seriesIndex]);
		}

		seriesRetention[seriesRetention.Length - 1].DefaultElement.Color = Color.Wheat;
		collection.Add(seriesRetention[seriesRetention.Length-1]);

		seriesChurn.DefaultElement.Color = Color.FromArgb(128, 16, 32);
		collection.Add(seriesChurn);

		return collection;
	}

	private Color InterpolateColor(Color color1, Color color2, double percentShift)
	{
		int red = (int)(((int)color1.R) * (100 - percentShift) / 100 + ((int)color2.R) * percentShift / 100);
		int green = (int)(((int)color1.G) * (100 - percentShift) / 100 + ((int)color2.G) * percentShift / 100);
		int blue = (int)(((int)color1.B) * (100 - percentShift) / 100 + ((int)color2.B) * percentShift / 100);

		return Color.FromArgb(red, green, blue);
	}
}
