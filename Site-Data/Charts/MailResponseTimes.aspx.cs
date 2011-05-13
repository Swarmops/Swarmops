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

using Activizr.Interface.Objects;
using Activizr.Logic.Communications;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr;


public partial class Charts_MailResponseTimes : System.Web.UI.Page
{



    protected void Page_Load (object sender, EventArgs e)
    {
        // Get organization metadata.

        OrganizationMetadata metadata = OrganizationMetadata.FromUrl(Request.Url.Host);
        Organization org = Organization.FromIdentity(metadata.OrganizationId);

        // Set the title.
        Chart.Title = "Mail Response Times - " + org.NameShort;

        // Change the shading mode
        Chart.ShadingEffectMode = ShadingEffectMode.Three;


        // Set the x axis label
        Chart.ChartArea.XAxis.Label.Text = "Mail received date (days prior to today)";
        Chart.ChartArea.YAxis.Label.Text = "Average response time in hours";

        Chart.ChartArea.XAxis.Minimum = -90;
        Chart.ChartArea.XAxis.Maximum = 0;

        Chart.Type = ChartType.Combo;
        Chart.YAxis.Scale = Scale.LogarithmicStacked;


        /*
        Chart.XAxis.TimeScaleLabels.Mode = TimeScaleLabelMode.Smart;

        string dayCountString = Request.QueryString["Days"];

        if (dayCountString != null)
        {
            int dayCount = Int32.Parse(dayCountString);

            if (dayCount < 32)
            {
                Chart.XAxis.TimeScaleLabels.DayFormatString = "dd";
                Chart.XAxis.TimeScaleLabels.MonthFormatString = "dd";
                Chart.XAxis.TimeInterval = TimeInterval.Day;
            }
            else if (dayCount < 180)
            {
                Chart.XAxis.TimeInterval = TimeInterval.Month;
            }

        }*/

        // Set the y axis label
        //Chart.ChartArea.YAxis.Label.Text = "Ung Pirat SE - medlemsgraf";

        // Set the directory where the images will be stored.
        Chart.TempDirectory = "temp";

        // Set the chart size.
        Chart.Width = 600;
        Chart.Height = 350;

        // If different sizes were given as parameters, adjust.

        string xSize = Request.QueryString["XSize"];
        string ySize = Request.QueryString["YSize"];
        string format = Request.QueryString["ImageFormat"];

        if (!String.IsNullOrEmpty(xSize))
        {
            Chart.Width = Int32.Parse(xSize);
        }

        if (!String.IsNullOrEmpty(ySize))
        {
            Chart.Height = Int32.Parse(ySize);
        }

        if (!String.IsNullOrEmpty(format))
        {
            Chart.ImageFormat = (ImageFormat)Enum.Parse(typeof(ImageFormat), format);
        }


        Chart.SeriesCollection.Add(GetResponseData(metadata));
        Chart.LegendBox.Template = "%Icon%Name";
        Chart.Debug = false;
        Chart.Mentor = false;

    }



    SeriesCollection GetResponseData (OrganizationMetadata metadata)
    {
        Organization organization = Organization.PPSE;

        CommunicationTurnarounds turnarounds = CommunicationTurnarounds.ForOrganization(organization, true);

        SeriesCollection collection = new SeriesCollection();

        DateTime today = DateTime.Today;
        DateTime dateIterator = today.AddDays(-90);

        int currentIndex = 0;

        // Prepare averages

        DateTime now = DateTime.Now;

        Dictionary<DateTime, int> caseCountOpen = new Dictionary<DateTime, int>();
        Dictionary<DateTime, int> caseCountResponded = new Dictionary<DateTime, int>();
        Dictionary<DateTime, double> caseTimeOpen = new Dictionary<DateTime, double>();
        Dictionary<DateTime, double> caseTimeResponded = new Dictionary<DateTime, double>();

        foreach (CommunicationTurnaround turnaround in turnarounds)
        {
            if (turnaround.Open)
            {
                if (!caseCountOpen.ContainsKey(turnaround.DateTimeOpened.Date))
                {
                    caseCountOpen[turnaround.DateTimeOpened.Date] = 0;
                    caseTimeOpen[turnaround.DateTimeOpened.Date] = 0.0;
                }

                caseCountOpen[turnaround.DateTimeOpened.Date]++;
                caseTimeOpen[turnaround.DateTimeOpened.Date] += (now - turnaround.DateTimeOpened).TotalHours;
            }
            else if (turnaround.Responded)
            {
                if (!caseCountResponded.ContainsKey(turnaround.DateTimeOpened.Date))
                {
                    caseCountResponded[turnaround.DateTimeOpened.Date] = 0;
                    caseTimeResponded[turnaround.DateTimeOpened.Date] = 0.0;
                }

                caseCountResponded[turnaround.DateTimeOpened.Date]++;
                caseTimeResponded[turnaround.DateTimeOpened.Date] += (turnaround.DateTimeFirstResponse-turnaround.DateTimeOpened).TotalHours;
            }
        }

        Series seriesOpen = new Series();
        seriesOpen.Name = "Still open";
        seriesOpen.DefaultElement.Color = Color.Orange;
        Series seriesResponded = new Series();
        seriesResponded.Name = "Responded";
        seriesResponded.DefaultElement.Color = Color.DarkGreen;

        int dateLabel = -90;

        while (dateIterator < today)
        {
            double avgTimeResponded = 0.0;
            double avgTimeOpen = 0.0;

            if (caseCountOpen.ContainsKey(dateIterator))
            {
                avgTimeOpen = caseTimeOpen[dateIterator]/(double) caseCountOpen[dateIterator];
            }
            if (caseCountResponded.ContainsKey(dateIterator))
            {
                avgTimeResponded = caseTimeResponded[dateIterator]/(double) caseCountResponded[dateIterator];
            }

            Element elementOpen = new Element();
            elementOpen.XValue = dateLabel;
            elementOpen.YValue = avgTimeOpen - avgTimeResponded;
            seriesOpen.AddElements(elementOpen);

            Element elementResponded = new Element();
            elementResponded.XValue = dateLabel;
            elementResponded.YValue = avgTimeResponded;
            seriesResponded.AddElements(elementResponded);

            dateIterator = dateIterator.AddDays(1);
            dateLabel++;
        }

        collection.Add(seriesResponded);
        collection.Add(seriesOpen);

        return collection;

    }

}
