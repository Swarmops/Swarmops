using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Activizr.Basic.Enums;
using dotnetCHARTING;

using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;

public partial class Pages_Public_Charts_MemberRetention : System.Web.UI.Page
{
    protected void Page_Load (object sender, EventArgs e)
    {
        // Set the title.
        Chart.Title = "Member Churn vs. Retention per Renewal Cycle, First at Center";

        // Set 3D
        Chart.Use3D = false;

        // Set the chart Type
        Chart.Type = ChartType.PiesNested;

        Chart.ShadingEffect = true;
        Chart.ShadingEffectMode = ShadingEffectMode.Two;
        Chart.TempDirectory = "temp";
        Chart.ImageFormat = ImageFormat.Png;

        // Set the chart size.
        Chart.Width = 600;
        Chart.Height = 350;

        SeriesCollection data = (SeriesCollection)Cache.Get(cacheDataKey);

        if (data == null)
        {
            data = GetSeriesCollection();
            Cache.Insert(cacheDataKey, data, null, DateTime.Today.AddDays(1).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        Chart.SeriesCollection.Add(data);

        Chart.CacheDuration = 5;
    }


    SeriesCollection GetSeriesCollection ()
    {
        SeriesCollection result = new SeriesCollection();

        ChurnData churnData = ChurnData.ForOrganization(Organization.PPSE);
        Memberships memberships = Memberships.ForOrganization(Organization.PPSE, true);

        // Build a hash table with memberships, based on person id

        Dictionary<int, Membership> membershipHash = new Dictionary<int, Membership>();

        foreach (Membership membership in memberships)
        {
            membershipHash[membership.PersonId] = membership;
        }

        string[] elementNames = { "Churn", "Retention" };

        Color[] colors = {
		                 	Color.Red, Color.Green
		                 };

        int currentYear = DateTime.Today.Year;

        int seriesCount = currentYear - 2006;

        // Create one series per year starting at 2006

        int[,] data = new int[seriesCount, 2];

        DateTime now = DateTime.Now; // cache this value

        foreach (ChurnDataPoint churnDataPoint in churnData)
        {
            if (membershipHash.ContainsKey(churnDataPoint.PersonId))
            {
                Membership membership = membershipHash[churnDataPoint.PersonId];

                // If not part of the renewal cycle, ignore - statistically uninteresting

                if ((churnDataPoint.ExpiryDate - churnDataPoint.DecisionDateTime).Days > 35)
                {
                    if (churnDataPoint.DecisionDateTime < new DateTime(2006, 12, 1) || churnDataPoint.DecisionDateTime > new DateTime(2007, 1, 15))
                    {
                        continue;
                    }
                }


                TimeSpan span = churnDataPoint.ExpiryDate - membership.MemberSince;
                int membershipDurationYears = (int)((span.Days + 40) / 365.25); // Add 40d for margin at renewal process
                if (membershipDurationYears < 0)
                {
                    membershipDurationYears = 0; // some bogus data in db
                }

                int dataIndex = 0;

                if (churnDataPoint.DataType == Activizr.Basic.Enums.ChurnDataType.Retention)
                {
                    dataIndex = 1;
                }

                if (membershipDurationYears == 0)
                {
                    membershipDurationYears = 1; // pretend first-year events happen at first cycle
                }

                data[membershipDurationYears - 1, dataIndex]++;
            }
            else
                Response.Write("<!-- " + churnDataPoint.PersonId + " -->");
        }


        for (int renewalIndex = seriesCount - 1; renewalIndex >= 0; renewalIndex--)
        {
            Series series = new Series();
            series.Name = "Renewal #" + (renewalIndex + 1).ToString();

            for (int elementLoop = 0; elementLoop < 2; elementLoop++)
            {
                Element element = new Element(elementNames[elementLoop], data[renewalIndex, elementLoop]);
                element.Color = colors[elementLoop];
                series.Elements.Add(element);
            }

            result.Add(series);
        }

        return result;
    }

    private string cacheDataKey = "ChartData-MemberRetention";
}




