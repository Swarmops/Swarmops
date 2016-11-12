using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using dotnetCHARTING;

using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

public partial class Pages_Public_Charts_MembershipDurations : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
      // Set the title.
      Chart.Title="Membership Durations";

      // Set 3D
	  Chart.Use3D = false;

      // Set the chart Type
      Chart.Type = ChartType.Pies;

      Chart.ShadingEffect = true;
	  Chart.ShadingEffectMode = ShadingEffectMode.Two;
      Chart.TempDirectory="temp";
	  Chart.ImageFormat = ImageFormat.Png;

      // Set the chart size.
	  Chart.Width = 600;
	  Chart.Height = 300;

        SeriesCollection data = (SeriesCollection) Cache.Get(cacheDataKey);

        if (data == null)
        {
            data = GetSeriesCollection();
            Cache.Insert(cacheDataKey, data, null, DateTime.Today.AddDays(1).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
        }

		Chart.SeriesCollection.Add(data);

        Chart.CacheDuration = 5;
	}


	SeriesCollection GetSeriesCollection()
	{
		SeriesCollection result = new SeriesCollection();

        Memberships memberships = Memberships.ForOrganization(Organization.PPSE, true);

		string[] seriesNames = {
		                       	"Renewed 3+ times", "Renewed twice", "Renewed once", "Not renewed yet",
		                       	"Churn 6+", "Churn after 3-5th years", "Churn at 2nd renewal", "Churn at 1st renewal", "Churned before 1st year",
		                       	"Still Active"
		                       };

		Color[] colors = {
		                 	Color.Black, Color.DarkGreen, Color.Green, Color.GreenYellow,
		                 	Color.DarkViolet, Color.Crimson, Color.DarkRed, Color.Red, Color.DarkSalmon,
							Color.Green
		                 };

		int seriesDividerIndex = 4;
		int stillActiveIndex = seriesNames.Length - 1;

		int[] data = new int[seriesNames.Length];
		DateTime now = DateTime.Now; // cache this value

		foreach (Membership membership in memberships)
		{
			if (membership.Active)
			{
				data [stillActiveIndex]++;

				TimeSpan span = now - membership.MemberSince;

			    int seriesIndex = seriesDividerIndex - 1 - span.Days/365;

                if (seriesIndex < 0)
                {
                    seriesIndex = 0;
                }

				data[seriesIndex]++;
			}
			else
			{
				TimeSpan span = (DateTime) membership.DateTerminated - membership.MemberSince;

			    int yearCount = (span.Days + 30)/365;
			    int seriesIndex;

                if (yearCount >= 6)
                {
                    seriesIndex = stillActiveIndex - 5;
                }
                else if (yearCount >= 3)
                {
                    seriesIndex = stillActiveIndex - 4;
                }
                else
                {
                    seriesIndex = stillActiveIndex - 1 - yearCount; 
                }

                if (seriesIndex < stillActiveIndex || seriesIndex > stillActiveIndex - 1)
                {
                    seriesIndex = stillActiveIndex - 1;
                }

				data [seriesIndex]++;
			}
		}

		Series series1 = new Series();
		Series series2 = new Series();

		for (int index = 0; index <= stillActiveIndex; index++)
		{
			if (data[index] == 0)
			{
				continue;
			}

			Element element = new Element(seriesNames [index], data[index]);

			if (index < seriesDividerIndex)
			{
				series1.Elements.Add (element);
				element.Color = colors[index];
			}
			else
			{
				series2.Elements.Add (element);
			}
		}

		series1.Name = "Active Memberships";
		series2.Name = "Total Throughput";

		result.Add (series1);
		// result.Add (series2);

		return result;
	}

    private string cacheDataKey = "ChartData-Membership-Durations";
}




