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

using Activizr.Logic.Media;


public partial class Pages_Charts_MediaKeywordFrequency : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        string keyword = Request.QueryString["Keyword"];
        string fromBlogs = Request.QueryString["FromBlogs"];
        string color = Request.QueryString["Color"];

        if (string.IsNullOrEmpty(keyword))
        {
            keyword = "Piratpartiet";
        }

        if (string.IsNullOrEmpty(color))
        {
            color = "Green";
        }

		// Set the title.
		Chart.Title = "";

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Three;


		// Set the x axis label
		Chart.ChartArea.XAxis.Label.Text = "";

		Chart.XAxis.TimeScaleLabels.Mode = TimeScaleLabelMode.Smart;

		// Set the y axis label
		Chart.ChartArea.YAxis.Label.Text = "Bloggat: " + keyword;

		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

		// Add the random data.
		Chart.SeriesCollection.Add(GetMediaData(keyword, color));

		Chart.LegendBox.Position = LegendBoxPosition.None;
		Chart.Debug = false;
		Chart.Mentor = false;
	}



	SeriesCollection GetMediaData(string keyword, string color)
	{
        MediaEntries entries = MediaEntries.FromBlogKeyword(keyword, new DateTime(2008, 7, 14));
        entries.Reverse();

		SeriesCollection collection = new SeriesCollection();
		DateTime dateIterator = new DateTime(2008, 7, 14);

		Series series = new Series();
		series.Name = "";
		DateTime today = DateTime.Now.Date;
		int entryIndex = 0;
		int currentCount = 0;

		while (dateIterator < today)
		{
			DateTime nextDate = dateIterator.AddDays (7);
			while (entryIndex < entries.Count && entries[entryIndex].DateTime < nextDate)
			{
                currentCount++;
				entryIndex++;
			}

			Element newElement = new Element();
			newElement.XDateTime = dateIterator;
			newElement.YValue = currentCount;
			series.Elements.Add(newElement);
			dateIterator = nextDate;
            currentCount = 0;
		}

		collection.Add(series);

        collection[0].DefaultElement.Color = Color.Green; // (Color)Enum.Parse(typeof(Color), color);

		return collection;

	}






}
