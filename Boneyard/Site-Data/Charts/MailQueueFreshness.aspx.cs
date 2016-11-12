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

using Activizr.Database;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;
using Activizr.Logic.Communications;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;


public partial class Charts_MailQueueStaleness : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        DateTime pieDate = DateTime.Today;

		// Set the title.
        Chart.Title = "Current Age of Unanswered Mail Questions - " + pieDate.ToString ("MMM d, yyyy");

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Two;

		// Set the x axis label
		Chart.ChartArea.XAxis.Label.Text = "";

		// Set the y axis label
        Chart.ChartArea.YAxis.Label.Text = string.Empty;

		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

		// Add the data.

        /*
        object data = Cache.Get("Blog-Influence-" + cacheSuffix);

        if (data == null)
        {
            data = GetMediaData(new DateTime (2008, 12, 13));
            Cache.Insert("Blog-Influence-" + cacheSuffix, data, null, DateTime.Today.ToUniversalTime().AddDays(1), Cache.NoSlidingExpiration);
        }*/

        Chart.Type = ChartType.Pie;

		Chart.SeriesCollection.Add(GetMailQueueData());

		//Chart.LegendBox.Position = LegendBoxPosition.None;
		// Chart.LegendBox.Template = "%Icon%Name";
		Chart.Debug = false;
		Chart.Mentor = false;
	}



	SeriesCollection GetMailQueueData()
	{
	    CommunicationTurnarounds turnarounds = CommunicationTurnarounds.ForOrganization(Organization.PPSE);

        double[] thresholds = new double[9] { 0, 1, 5, 10, 24, 72, 168, 336, 720 };
        Color[] colors = new Color[9] { Color.Lime, Color.LimeGreen, Color.Green, Color.YellowGreen, Color.Gold, Color.DarkOrange, Color.Red, Color.Firebrick, Color.DarkRed };
	    int[] counts = new int[9];
	    string[] names = new string[9]
	                         {
	                             "0-60 minutes", "1-5 hours", "5-10 hours", "10-24 hours", "1-3 days", "3-7 days", "7-14 days"
	                             , "14-30 days", "Over 30 days"
	                         };

   		SeriesCollection collection = new SeriesCollection();

	    DateTime now = DateTime.Now;

        foreach (CommunicationTurnaround turnaround in turnarounds)
        {
            double ageHours = (now - turnaround.DateTimeOpened).TotalHours;

            int index = 8;
            while (ageHours < thresholds[index])
            {
                index--;
            }

            counts[index]++;
        }


        for (int index = 0; index < 9; index++)
        {
            Series series = new Series();
            series.Name = names[index];
            series.DefaultElement.Color = colors[index];

            Element newElement = new Element();
            newElement.YValue = counts[index];
            series.Elements.Add(newElement);

            collection.Add(series);
        }

        
		return collection;

	}






}
