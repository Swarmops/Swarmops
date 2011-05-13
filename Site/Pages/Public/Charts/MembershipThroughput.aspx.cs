using System;
using System.Drawing;
using System.Web.UI;

using dotnetCHARTING;

using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

public partial class Pages_Public_Charts_MembershipThroughput : Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
      // Set the title.
      Chart.Title="Membership Throughput Per Year, 2006 at Center, " + DateTime.Today.Year + " at Rim";

      // Set 3D
	  Chart.Use3D = false;

      // Set the chart Type
      Chart.Type = ChartType.PiesNested;

      Chart.ShadingEffect = true;
	  Chart.ShadingEffectMode = ShadingEffectMode.Two;
      Chart.TempDirectory="temp";
	  Chart.ImageFormat = ImageFormat.Png;

      // Set the chart size.
	  Chart.Width = 600;
	  Chart.Height = 350;

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

		string[] elementNames = { "Still active, renewed", "Still active, unrenewed", "Hesitating, unrenewed", "Churn at 4th+ renewal", "Churn at 3rd renewal", 
                                    "Churn at 2nd renewal", "Churn at 1st renewal", "Churn in 1st year", 
		                       };

		Color[] colors = {
		                 	Color.Green, Color.LimeGreen, Color.Lime, Color.White, Color.Wheat, Color.Orange, 
                            Color.Red, Color.DarkRed
		                 };

		int currentYear = DateTime.Today.Year;

		int seriesCount = currentYear - 2006 + 1;
		int elementCount = elementNames.Length;

		// Create one series per year starting at 2006

		int[,] data = new int[seriesCount,elementCount];

	    foreach (Membership membership in memberships)
		{
			if (membership.Active)
			{
                if (membership.Expires.Year > DateTime.Today.Year) // Renewed this year?
                {
                    data[membership.MemberSince.Year - 2006, 0]++;
                }
                else if (DateTime.Today.AddDays (25) > membership.Expires )
                {
                    data[membership.MemberSince.Year - 2006, 2]++; // Not renewed 3 days after first reminder
                } 
                else
                {
                    data[membership.MemberSince.Year - 2006, 1]++;
                }
			}
			else
			{
				TimeSpan span = (DateTime) membership.DateTerminated - membership.MemberSince;

				int membershipDurationYears = (int) ((span.Days + 30) / 365.25); // Add 30d for margin at renewal process

                if (membershipDurationYears < 0)
                {
                    membershipDurationYears = 0; // compensate for some bogus data in db
                }

                int index = 4 - membershipDurationYears;

                if (index < 0)
                {
                    index = 0;
                }

				data[membership.MemberSince.Year - 2006, index + 3]++;

			}
		}

		for (int year = 2006 + seriesCount - 1; year >= 2006; year--)
		{
			Series series = new Series();
            series.Name = string.Empty;

			for (int elementLoop = 0; elementLoop < elementCount; elementLoop++)
			{
				Element element = new Element(elementNames[elementLoop], data[year - 2006, elementLoop]);
				element.Color = colors[elementLoop];
				series.Elements.Add(element);
			}

			result.Add(series);
		}

		return result;
	}

    private string cacheDataKey = "ChartData-Membership-Throughput";
}




