using System;
using System.Data;
using System.Drawing;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using dotnetCHARTING;

using Activizr.Logic.Financial;
using Activizr.Interface.Objects;
using Activizr.Logic.Structure;


public partial class Charts_RecentDonations : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        // Get organization metadata.

        OrganizationMetadata metadata = OrganizationMetadata.FromUrl(Request.Url.Host);
        Organization org = Organization.FromIdentity(metadata.OrganizationId);

        // TODO: Get donation account from org, somehow

	    FinancialAccount donationAccount = org.FinancialAccounts.IncomeDonations;

        // Find scope.
        DateTime month = DateTime.Today.AddMonths(-1);
        /*
        if (month.Day < 5)
        {
            month = month.AddMonths(-1);
        }*/

        // Set the title.
        Chart.Title = "Donations for " + month.ToString ("MMMM yyyy") + " - SEK ";

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Three;


		// Set the x axis label
		Chart.ChartArea.XAxis.Label.Text = "";

        Chart.XAxis.TimeScaleLabels.Mode = TimeScaleLabelMode.Smart;

        Chart.XAxis.TimeScaleLabels.DayFormatString = "dd";
        Chart.XAxis.TimeScaleLabels.MonthFormatString = "dd";
        Chart.XAxis.TimeScaleLabels.YearFormatString = "dd";
        Chart.XAxis.TimeInterval = TimeInterval.Day;

		// Set the y axis label
		//Chart.ChartArea.YAxis.Label.Text = "Ung Pirat SE - medlemsgraf";

		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

        decimal donationsTotal;
        int donationCount;

    	Chart.SeriesCollection.Add(GetDonationData(metadata, out donationsTotal, out donationCount));
        Chart.Title += donationsTotal.ToString(CultureInfo.CreateSpecificCulture (org.DefaultCountry.Culture)) + " in " + donationCount.ToString() + " donations";

		Chart.LegendBox.Position = LegendBoxPosition.None;
		Chart.Debug = false;
		Chart.Mentor = false;
	}



	SeriesCollection GetDonationData (OrganizationMetadata metadata, out decimal donationsTotal, out int donationCount)
	{
		SeriesCollection collection = new SeriesCollection();
        DateTime dateIterator = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
	    Organization org = Organization.FromIdentity(metadata.OrganizationId);

        if (DateTime.Now.Day > 4)
        {
            dateIterator = dateIterator.AddMonths(-1);
        }
        else
        {
            dateIterator = dateIterator.AddMonths(-1);  // -2
        }

        DateTime dateStop = dateIterator.AddMonths(1);

        FinancialAccountRows rows = org.FinancialAccounts.IncomeDonations.GetRows(dateIterator, dateIterator.AddMonths(1));

		Series series = new Series();
		series.Name = "";
		DateTime today = DateTime.Now.Date;
		int rowIndex = 0;
        donationsTotal = 0.0m;
        donationCount = rows.Count;

        Dictionary<int, int> personMembershipCountLookup = new Dictionary<int, int>();

		while (dateIterator < dateStop)
		{
            decimal donationsToday = 0.0m;

			DateTime nextDate = dateIterator.AddDays (1);
			while (rowIndex < rows.Count && rows[rowIndex].TransactionDateTime < nextDate)
			{
                donationsToday -= rows[rowIndex].Amount;
                donationsTotal -= rows[rowIndex].Amount;

				rowIndex++;
			}

			Element newElement = new Element();
			newElement.XDateTime = dateIterator;
			newElement.YValue = (double) donationsToday;
			series.Elements.Add(newElement);
			dateIterator = nextDate;
		}

		collection.Add(series);

        collection[0].DefaultElement.Color = metadata.Color;

		return collection;
	}

}
