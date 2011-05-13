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
using Activizr.Logic.Pirates;


public partial class Charts_BlogInfluencePie : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        string dateString = Request.QueryString["Date"];

        DateTime pieDate = DateTime.Today;

        if (!string.IsNullOrEmpty(dateString))
        {
            pieDate = DateTime.Parse(dateString);
        }

		// Set the title.
        Chart.Title = "Blog Influence by Political Affiliation - " + pieDate.ToString ("MMM d, yyyy");
        string cacheSuffix = pieDate.ToString("yyyyMMdd");

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

		Chart.SeriesCollection.Add(GetBlogInfluenceData(pieDate));

		//Chart.LegendBox.Position = LegendBoxPosition.None;
		Chart.LegendBox.Template = "%Icon%Name";
		Chart.Debug = false;
		Chart.Mentor = false;
	}



	SeriesCollection GetBlogInfluenceData(DateTime pieDate)
	{
        Dictionary<PoliticalAffiliation, Color> colorLookup = new Dictionary<PoliticalAffiliation, Color>();
        Dictionary<PoliticalAffiliation, int> score = new Dictionary<PoliticalAffiliation, int>();

        colorLookup[PoliticalAffiliation.Brown] = Color.DarkGoldenrod;
        colorLookup[PoliticalAffiliation.Conservative] = Color.Khaki;
        colorLookup[PoliticalAffiliation.IndependentBlue] = Color.LightBlue;
        colorLookup[PoliticalAffiliation.IndependentGreen] = Color.LightGreen;
        colorLookup[PoliticalAffiliation.IndependentRed] = Color.Salmon;
        colorLookup[PoliticalAffiliation.LoyalBlue] = Color.DarkBlue;
        colorLookup[PoliticalAffiliation.LoyalGreen] = Color.DarkGreen;
        colorLookup[PoliticalAffiliation.LoyalRed] = Color.DarkRed;
        colorLookup[PoliticalAffiliation.NotPolitical] = Color.LightGray;
        colorLookup[PoliticalAffiliation.Pirate] = Color.FromArgb(0x66, 0, 0x87);
        colorLookup[PoliticalAffiliation.Unknown] = Color.DarkGray;


        BasicMedium[] politicalBlogs = PirateDb.GetDatabase().GetBlogTopList(pieDate);

        int position = 0;

        foreach (BasicMedium blog in politicalBlogs)
        {
            position++;
            int thisScore = 51 - position;

            if (blog.PoliticalAffiliation == PoliticalAffiliation.Unknown)
            {
                Person.FromIdentity(1).SendNotice("Unknown political affiliation: " + blog.Name + " (" + blog.Identity.ToString() + ")", "Unknown political affiliation for blog: " + blog.Name + "\r\n", 1);
            }

            if (score.ContainsKey(blog.PoliticalAffiliation))
            {
                score[blog.PoliticalAffiliation] += thisScore;
            }
            else
            {
                score[blog.PoliticalAffiliation] = thisScore;
            }
        }

   		SeriesCollection collection = new SeriesCollection();


        foreach (PoliticalAffiliation key in score.Keys)
        {
            Series series = new Series();
            series.Name = key.ToString();
            series.DefaultElement.Color = colorLookup[key];

            Element newElement = new Element();
            newElement.YValue = score[key];
            series.Elements.Add(newElement);

            collection.Add(series);
        }

        
		return collection;

	}






}
