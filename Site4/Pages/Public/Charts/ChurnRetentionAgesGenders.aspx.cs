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

using Activizr.Basic.Enums;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;


public partial class Pages_Public_Charts_ChurnRetentionAgesGenders : System.Web.UI.Page
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
		Chart.ChartArea.YAxis.Label.Text = "Member Churn vs. Growth by Birthyear, Gender";
		Chart.YAxis.Scale = Scale.Stacked;
		Chart.YAxis.Maximum = 100;
		Chart.YAxis.Minimum = -100;
		Chart.XAxis.Minimum = 1940;
		Chart.XAxis.Maximum = 2000;
		Chart.LegendBox.Template = "%Icon%Name";


		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

        object data = Cache.Get(cacheDataKey);

        if (data == null)
        {
            data = GetChurnGrowthData();
            Cache.Insert(cacheDataKey, data, null, DateTime.Today.AddDays(1).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        Chart.SeriesCollection.Add((SeriesCollection)data);

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

		Series seriesMaleGrowth = new Series();
		Series seriesMaleChurn = new Series();
		Series seriesFemaleGrowth = new Series();
		Series seriesFemaleChurn = new Series();
		seriesMaleGrowth.Name = "Male Growth";
		seriesMaleChurn.Name = "Male Churn";
		seriesFemaleGrowth.Name = "Female Growth";
		seriesFemaleChurn.Name = "Female Churn";
		DateTime today = DateTime.Now.Date;
		int eventIndex = 0;

		while (events[eventIndex].DateTime < dateIterator)
		{
			eventIndex++;
		}

		int[] maleGrowth = new int[200];
		int[] maleChurn = new int[200];
		int[] femaleGrowth = new int[200];
		int[] femaleChurn = new int[200];

		while (eventIndex < events.Count)
		{
            if (events[eventIndex].OrganizationId == Organization.PPSEid)  // TODO: dynamic org selection
			{
				int yearIndex = events[eventIndex].BirthYear - 1900;

				if (yearIndex > 0  && yearIndex < 199)
				{
					if (events[eventIndex].DeltaCount > 0)
					{
						if (events[eventIndex].Gender == PersonGender.Male)
						{
							maleGrowth[yearIndex]++;
						}
						else
						{
							femaleGrowth[yearIndex]++;
						}
					}
					else
					{
						if (events[eventIndex].Gender == PersonGender.Male)
						{
							maleChurn[yearIndex]++;
						}
						else
						{
							femaleChurn[yearIndex]++;
						}
					}
				}
			}

			eventIndex++;
		}

		Element newElement = new Element();

		for (int yearIndex = 40; yearIndex <= 100; yearIndex++)
		{
			newElement = new Element();
			newElement.Name = (1900 + yearIndex).ToString();
			newElement.YValue = maleGrowth[yearIndex];
			seriesMaleGrowth.Elements.Add(newElement);

			newElement = new Element();
			newElement.Name = (1900 + yearIndex).ToString();
			newElement.YValue = -maleChurn[yearIndex];
			seriesMaleChurn.Elements.Add(newElement);

			newElement = new Element();
			newElement.Name = (1900 + yearIndex).ToString();
			newElement.YValue = femaleGrowth[yearIndex];
			seriesFemaleGrowth.Elements.Add(newElement);

			newElement = new Element();
			newElement.Name = (1900 + yearIndex).ToString();
			newElement.YValue = -femaleChurn[yearIndex];
			seriesFemaleChurn.Elements.Add(newElement);
		}


		seriesMaleGrowth.DefaultElement.Color = Color.DarkBlue;
		seriesMaleChurn.DefaultElement.Color = Color.LightBlue;
		seriesFemaleGrowth.DefaultElement.Color = Color.DarkRed;
		seriesFemaleChurn.DefaultElement.Color = Color.Pink;

		SeriesCollection collection = new SeriesCollection();
		collection.Add(seriesFemaleGrowth);
		collection.Add(seriesMaleGrowth);
		collection.Add(seriesFemaleChurn);
		collection.Add(seriesMaleChurn);

		return collection;
	}


    private string cacheDataKey = "ChartData-Churn-Retention-Ages-Genders";

}
