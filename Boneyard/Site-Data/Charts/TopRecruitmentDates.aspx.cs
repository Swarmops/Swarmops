using System;
using System.Collections.Generic;
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

public partial class Pages_Public_Charts_TopRecruitmentDates : System.Web.UI.Page
{
	protected void Page_Load(Object sender, EventArgs e)
	{
		// Set the title.
		Chart.Title = "Topp-20-datum i nyrekrytering";

		string orgIdString = Request.QueryString["OrgId"];

		if (orgIdString == null || orgIdString.Length == 0)
		{
			orgIdString = "1";
		}

		Chart.Title += " - " + Organization.FromIdentity(Int32.Parse(orgIdString)).Name;

		string label = Request.QueryString["Label"];

		if (label != null)
		{
			Chart.Title += " - " + label;
		}

		// Set the chart Type
		Chart.Type = ChartType.ComboHorizontal;

		// Turn 3D off.
		Chart.Use3D = false;

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Three;

		// Set a default transparency
		Chart.DefaultSeries.DefaultElement.Transparency = 20;

		// Set the x axis scale
		Chart.ChartArea.XAxis.Scale = Scale.Stacked;

		// Set the y axis label
		Chart.ChartArea.XAxis.Label.Text = "Nya medlemmar ett visst datum";


		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

		if (Request.QueryString["Height"] != null)
		{
			Chart.Height = Int32.Parse(Request.QueryString["Height"]);
		}

		// Add the random data.
		Chart.SeriesCollection.Add(GetRankingData());

		Chart.LegendBox.Position = LegendBoxPosition.None;
		Chart.LegendBox.Template = "%Icon%Name";
		Chart.Debug = false;
		Chart.Mentor = false;
	}


    Series GetRankingData()
	{
        MembershipEvents events = MembershipEvents.LoadAll();
        DateTime today = DateTime.Today;

        DateTime dateIterator = new DateTime(2006, 1, 1);
        int eventIndex = 0;

        List<DateBar> dateBars = new List<DateBar>();
        while (eventIndex < events.Count)
        {
            int newMembersThisDate = 0;
            DateTime nextDate = dateIterator.AddDays(1);
            while (eventIndex < events.Count && events[eventIndex].DateTime < nextDate)
            {
               if (events[eventIndex].DeltaCount > 0 && events [eventIndex].OrganizationId == 1)
               {
                   newMembersThisDate++;
               }

               eventIndex++;
            }

            DateBar dateBar = new DateBar();
            dateBar.Date = dateIterator;
            dateBar.RecruitedMembers = newMembersThisDate;
            dateBars.Add(dateBar);

            dateIterator = nextDate;
        }

        dateBars.Sort();
        dateBars.Reverse();

        Series series = new Series();

        for (int index = 20; index >= 0; index --)
        {
            DateBar bar = dateBars[index];

            Element element = new Element();
            element.YValue = bar.RecruitedMembers;
            element.Name = bar.Date.ToString("yyyy-MM-dd");

            if (bar.Date == today)
            {
                element.Color = Color.Violet;
                element.Name = "IDAG";
            }

            series.Elements.Add(element);
        }

		series.DefaultElement.Color = System.Drawing.Color.DarkViolet;
		series.DefaultElement.ShowValue = true;

		return series;
	}



    public class DateBar : IComparable
    {
        public DateTime Date;
        public int RecruitedMembers;

        #region IComparable Members

        public int CompareTo(object obj)
        {
            DateBar otherBar = (DateBar)obj;

            return this.RecruitedMembers.CompareTo(otherBar.RecruitedMembers);
        }

        #endregion
    }



}
