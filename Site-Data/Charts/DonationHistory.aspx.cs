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
using Activizr.Logic.Financial;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;


public partial class Charts_DonationHistory : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        // Get organization metadata.

        OrganizationMetadata metadata = OrganizationMetadata.FromUrl(Request.Url.Host);
        Organization org = Organization.FromIdentity(metadata.OrganizationId);

        // Set the title.
        Chart.Title = "Donation history - " + org.NameShort;

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Three;

		// Set the x axis label
		Chart.ChartArea.XAxis.Label.Text = "";

        Chart.XAxis.TimeScaleLabels.Mode = TimeScaleLabelMode.Smart;

        Chart.XAxis.TimeInterval = TimeInterval.Month;

		// Set the y axis label
		//Chart.ChartArea.YAxis.Label.Text = "Ung Pirat SE - medlemsgraf";

		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

    	Chart.SeriesCollection.Add(GetDonationData(metadata));

		Chart.LegendBox.Position = LegendBoxPosition.None;
		Chart.Debug = false;
		Chart.Mentor = false;
        
	}



	SeriesCollection GetDonationData (OrganizationMetadata metadata)
	{
        string cacheDataKey = "Financials-DonationData-PiratpartietSE";

	    FinancialAccountRows rows = (FinancialAccountRows) Cache.Get(cacheDataKey);
	    Organization org = Organization.FromIdentity(metadata.OrganizationId);

        if (rows == null)
        {
            rows = org.FinancialAccounts.IncomeDonations.GetRows(new DateTime(2008, 1, 1), DateTime.Today);
            Cache.Insert(cacheDataKey, rows, null, DateTime.Today.AddHours(1).ToUniversalTime(), System.Web.Caching.Cache.NoSlidingExpiration);
        }

		SeriesCollection collection = new SeriesCollection();
		DateTime dateIterator = new DateTime(2008, 1, 1);

        DateTime today = DateTime.Now.Date;
        DateTime endDate = today.AddDays(-today.Day);

		Series series = new Series();
		series.Name = "";
		int rowIndex = 0;

		while (dateIterator <= endDate)
		{
		    DateTime nextDate = dateIterator.AddMonths(1);
		    decimal donationsThisMonth = 0.0m;

			while (rowIndex < rows.Count && rows[rowIndex].TransactionDateTime < nextDate)
			{
			    donationsThisMonth -= rows[rowIndex].Amount;  // Subtracting because the donation account is sign reversed; we want a positive graph

				rowIndex++;
			}

			Element newElement = new Element();
			newElement.XDateTime = dateIterator;
			newElement.YValue = (double) donationsThisMonth;
			series.Elements.Add(newElement);
			dateIterator = nextDate;
		}

		collection.Add(series);

        collection[0].DefaultElement.Color = metadata.Color;

		return collection;

	}

}
