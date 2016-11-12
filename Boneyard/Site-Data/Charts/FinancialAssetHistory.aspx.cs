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

using Activizr.Basic.Enums;
using Activizr.Logic.Financial;
using Activizr.Logic.Media;
using Activizr.Logic.Structure;


public partial class Pages_Public_Charts_FinancialAssetHistory : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		// Set the title.
        Chart.Title = "Financial Strength (Assets minus Debts) - Piratpartiet SE";

		// Change the shading mode
		Chart.ShadingEffectMode = ShadingEffectMode.Three;

		// Set the x axis label
		Chart.ChartArea.XAxis.Label.Text = "";

        Chart.XAxis.TimeInterval = TimeInterval.Month;
        Chart.XAxis.TimeScaleLabels.Mode = TimeScaleLabelMode.Dynamic;

		// Set the y axis label
        Chart.ChartArea.YAxis.Label.Text = string.Empty;

		// Set the directory where the images will be stored.
		Chart.TempDirectory = "temp";

		// Set the chart size.
		Chart.Width = 600;
		Chart.Height = 350;

		// Add the data.

        object data = Cache.Get("Piratpartiet-SE-Financials");

        if (data == null)
        {
            data = GetFinancialData();
            Cache.Insert("Piratpartiet-SE-Financials", data, null, DateTime.Now.ToUniversalTime().AddMinutes(5), Cache.NoSlidingExpiration);
        }

		Chart.SeriesCollection.Add((SeriesCollection) data);

		Chart.LegendBox.Position = LegendBoxPosition.None;
		//Chart.LegendBox.Template = "%Icon%Name";
		Chart.Debug = false;
		Chart.Mentor = false;

        Chart.CacheDuration = 5;
	}



	SeriesCollection GetFinancialData()
	{
	    FinancialAccounts accounts = FinancialAccounts.ForOrganization(Organization.PPSE, FinancialAccountType.Balance);

	    accounts.Remove(Organization.PPSE.FinancialAccounts.DebtsCapital);

	    FinancialAccountRows rows = accounts.GetRows(new DateTime(2006, 1, 1), DateTime.Today.AddDays(1));



		SeriesCollection collection = new SeriesCollection();
		DateTime dateIterator = new DateTime(2009, 1, 1);

        DateTime endDate = DateTime.Now.Date;

	    Series series = new Series();
        series.Name = "Each event";

	    decimal maxValue = 0;
        decimal minValue = Decimal.MaxValue;
        int rowIndex = 0;
        decimal currentBalance = 0;

        while (rows[rowIndex].TransactionDateTime < dateIterator && rowIndex < rows.Count)
        {
            currentBalance += rows[rowIndex].Amount;

            rowIndex++;
        }

        Element newElement = new Element();
        newElement.XDateTime = dateIterator;
        newElement.YValue = (double) currentBalance;

        while (dateIterator < endDate)
        {
            if (rowIndex >= rows.Count)
            {
                break;
            }

            DateTime nextDate = dateIterator.AddDays(1);
            while (rowIndex < rows.Count && rows[rowIndex].TransactionDateTime < nextDate )
            {
                currentBalance += rows[rowIndex].Amount;

                rowIndex++;
            }

            if (currentBalance < minValue)
            {
                minValue = currentBalance;
            }

            if (currentBalance > maxValue)
            {
                maxValue = currentBalance;
            }

            newElement = new Element();
            newElement.XDateTime = dateIterator;
            newElement.YValue = (double) currentBalance;
            series.Elements.Add(newElement);
            dateIterator = nextDate;
        }

        /*
        Chart.YAxis.Maximum = (maxValue + 90) / 10000 * 10000;
        Chart.YAxis.Minimum = minValue / 100 * 100;*/

        series.Type = SeriesType.Line;
        series.Line.Width = 5;
        series.DefaultElement.Color = Color.MidnightBlue;
        //series.DefaultElement.Marker = new ElementMarker(ElementMarkerType.None);
        collection.Add(series);

        return collection;


	}






}
