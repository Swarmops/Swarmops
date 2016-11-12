using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using dotnetCHARTING;

using Activizr.Logic.Media;


public partial class Pages_Public_Charts_MediaCompetitiveTimeline : System.Web.UI.Page
{
    private DateTime startDate;

	protected void Page_Load(object sender, EventArgs e)
	{
        bool blogs = true;
        string cacheSuffix = "-Blogs";
        string titleSuffix = " - Blogs";
	    startDate = new DateTime(2008, 7, 20);

        if (Request.QueryString["MediaType"] == "OldMedia")
        {
            blogs = false;
            cacheSuffix = "-OldMedia";
            titleSuffix = " - Oldmedia";
            startDate = new DateTime(2010, 1, 3);
        }

		// Set the title.
        Chart.Title = "Quantitative mentions - Party names" + titleSuffix;

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Three;


		// Set the x axis label
		Chart.ChartArea.XAxis.Label.Text = "";

		Chart.XAxis.TimeScaleLabels.Mode = TimeScaleLabelMode.Dynamic;

		// Set the y axis label
        Chart.ChartArea.YAxis.Label.Text = string.Empty;

		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

		// Add the data.

        object data = Cache.Get("Media-Competitive" + cacheSuffix);

        if (data == null)
        {
            data = GetMediaData(blogs);
            Cache.Insert("Media-Competitive" + cacheSuffix, data, null, DateTime.Today.ToUniversalTime().AddDays(1), Cache.NoSlidingExpiration);
        }

		Chart.SeriesCollection.Add((SeriesCollection) data);

		//Chart.LegendBox.Position = LegendBoxPosition.None;
		Chart.LegendBox.Template = "%Icon%Name";
		Chart.Debug = false;
		Chart.Mentor = false;

        Chart.CacheDuration = 5;
	}



	SeriesCollection GetMediaData(bool blogs)
	{
        string[] seriesNames = { "Moderaterna", "Folkpartiet", "Centern", "Kristdemokraterna", "Socialdemokraterna", "Milj" + '\xF6' + "partiet", "V" + '\xE4' + "nsterpartiet", "Sverigedemokraterna", "Piratpartiet" };
        Color[] seriesColors = { Color.LightBlue, Color.Blue, Color.LawnGreen, Color.DarkBlue, Color.Salmon, Color.Green, Color.Red, Color.DarkGoldenrod, Color.FromArgb(0x66, 0, 0x87) };
        int seriesCount = seriesNames.Length;

        // TODO: ADD CACHE HERE

        MediaEntries entries = MediaEntries.FromKeywordsSimplified(seriesNames);

        // Build the keyword-id-to-series-index table

        Dictionary<int, int> keywordSeries = new Dictionary<int,int>();
        Dictionary<int, bool> mediaIdIsBlog = MediaEntries.GetMediaTypeTable();

        int index = 0;
        foreach (string keyword in seriesNames)
        {
            // Suboptimal, but wtf

            keywordSeries [MediaEntries.GetKeywordId (keyword)] = index;
            index++;
        }

		SeriesCollection collection = new SeriesCollection();
		DateTime dateIterator = startDate;

		DateTime today = DateTime.Now.Date;
		int entryIndex = 0;

        Series[] series = new Series[seriesCount];
        int[] count = new int[seriesCount];

		while (entries[entryIndex].DateTime < dateIterator)
		{
    		entryIndex++;
		}
        
        while (dateIterator.AddDays (7) < today)
		{
			if (entryIndex >= entries.Count)
			{
				break;
			}

   			DateTime nextDate = dateIterator.AddDays (7);
            count = new int[seriesCount];

			while (entryIndex < entries.Count && entries[entryIndex].DateTime < nextDate)
			{
                int seriesIndex = keywordSeries [entries [entryIndex].KeywordId];
                if (mediaIdIsBlog[Int32.Parse (entries[entryIndex].MediaName)] == blogs)
                {
                    count[seriesIndex]++;
                }
				entryIndex++;
			}

            for (int seriesIndex = 0; seriesIndex < seriesCount; seriesIndex++)
            {
                if (series[seriesIndex] == null)
                {
                    series[seriesIndex] = new Series();  // this init should probably be earlier and not conditional...
                }

                Element newElement = new Element();
                newElement.XDateTime = dateIterator;
                newElement.YValue = count[seriesIndex];
                series[seriesIndex].Elements.Add(newElement);
            }

			entryIndex++;
            dateIterator = nextDate;
		}

        for (int seriesIndex = 0; seriesIndex < seriesCount; seriesIndex++)
        {
            series[seriesIndex].Type = SeriesType.Spline;
            series[seriesIndex].Line.Width = 2;
            
            series[seriesIndex].DefaultElement.Color = seriesColors[seriesIndex];
            series[seriesIndex].DefaultElement.Marker = new ElementMarker(ElementMarkerType.None);
            series[seriesIndex].Name = seriesNames[seriesIndex];

            collection.Add(series[seriesIndex]);
        }

        collection[8].Line.Width = 7;
        
		return collection;

	}






}
