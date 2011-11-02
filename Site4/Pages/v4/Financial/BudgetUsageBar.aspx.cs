using System;
using System.Drawing;
using dotnetCHARTING;
using Activizr.Logic.Financial;

namespace Pages.v4.Financial
{
    public partial class BudgetUsageBar: PageV4Base
    {
// ReSharper disable InconsistentNaming
        protected void Page_Load (object sender, EventArgs e)
// ReSharper restore InconsistentNaming
        {
            // Set the chart Type
            Chart.Type = ChartType.ComboHorizontal;

            // Turn 3D off.
            Chart.Use3D = false;

            // Change the shading mode
            Chart.ShadingEffectMode = ShadingEffectMode.Three;

            // Set a default transparency
            Chart.DefaultSeries.DefaultElement.Transparency = 00;

            // Set the x axis scale
            Chart.ChartArea.XAxis.Scale = Scale.Stacked;
            Chart.ChartArea.XAxis.Minimum = 0;
            Chart.ChartArea.XAxis.Label.Text = "Percent budget used";

            // Set the y axis label
            Chart.ChartArea.YAxis.Label.Text = "";

            // Set the directory where the images will be stored.
            Chart.TempDirectory = "temp";

            // Set the chart size.
            Chart.Width = Int32.Parse(Request.QueryString["Width"]) - 20;
            Chart.Height = 80;

            BudgetData data = GetBudgetData(Int32.Parse(Request.QueryString["AccountId"]),
                                            Int32.Parse(Request.QueryString["Year"]));

            if (data.PercentActual + Math.Max(data.PercentSuballocatedUsed,data.PercentSuballocated) <= 100.0)
            {
                Chart.ChartArea.XAxis.Maximum = 100;
            }

            Chart.SeriesCollection.Add(GetBudgetDataGraphics(data));

            Chart.LegendBox.Position = LegendBoxPosition.Top;
            Chart.LegendBox.Template = "%Icon %Name";
            Chart.LegendBox.CornerSize = 0;
            Chart.Debug = false;
            Chart.Mentor = false;

            Chart.Background.Color = Color.FromArgb(0xFF, 0xF8, 0xE7, 0xFF);
        }

        private BudgetData GetBudgetData (int budgetId, int year)
        {
            FinancialAccount account = FinancialAccount.FromIdentity(budgetId);
            double budget = -account.GetBudget(year);
            Int64 actualCents = account.GetDeltaCents(new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1));
            double actual = actualCents / 100.0;

            // Get suballocated

            FinancialAccounts accountTree = account.GetTree();

            double budgetTotal = -accountTree.GetBudgetSum(year);
            Int64 deltaTotalCents = accountTree.GetDeltaCents(new DateTime(year, 1, 1), new DateTime(year + 1, 1, 1));
            double deltaTotal = deltaTotalCents/100.0;

            BudgetData result = new BudgetData();
            result.AccountName = account.Name;

            if (budgetTotal > 0.0)
            {
                result.PercentActual = actual / budgetTotal * 100.0;
                result.PercentSuballocated = (budgetTotal-budget)/budgetTotal*100.0;
                result.PercentSuballocatedUsed = (deltaTotal-actual)/budgetTotal*100.0;
            }

            return result;
        }

        private SeriesCollection GetBudgetDataGraphics (BudgetData data)
        {

            // TODO: Add sub-accounts from tree

            SeriesCollection collection = new SeriesCollection();
            Element element;

            if (data.PercentSuballocated > 0.0)
            {
                double subAllocatedFirstBar = Math.Min(data.PercentSuballocated, data.PercentSuballocatedUsed);

                Series seriesSuballocatedUsed = new Series();

                element = new Element();
                element.YValue = subAllocatedFirstBar;
                element.Color = Color.DarkBlue;
                element.Name = data.AccountName;

                seriesSuballocatedUsed.Name = "Suballocated Used";
                seriesSuballocatedUsed.Elements.Add(element);
                collection.Add(seriesSuballocatedUsed);

                if (data.PercentSuballocated < data.PercentSuballocatedUsed)
                {
                    // overrun

                    Series seriesSuballocatedOverrun = new Series();

                    element = new Element();
                    element.YValue = data.PercentSuballocatedUsed - data.PercentSuballocated;
                    element.Color = Color.DarkViolet;
                    element.Name = data.AccountName;

                    seriesSuballocatedOverrun.Name = "Suballocated Overrun";
                    seriesSuballocatedOverrun.Elements.Add(element);
                    collection.Add(seriesSuballocatedOverrun);
                }
                else if (data.PercentSuballocatedUsed < data.PercentSuballocated)
                {
                    // overrun

                    Series seriesSuballocatedUnused = new Series();

                    element = new Element();
                    element.YValue = data.PercentSuballocated - data.PercentSuballocatedUsed;
                    element.Color = Color.LightSkyBlue;
                    element.Name = data.AccountName;

                    seriesSuballocatedUnused.Name = "Suballocated Unused";
                    seriesSuballocatedUnused.Elements.Add(element);
                    collection.Add(seriesSuballocatedUnused);
                }
            }

            if (data.PercentActual > 0.0)
            {
                Series seriesUsed = new Series();

                element = new Element();

                if (data.PercentActual + Math.Max(data.PercentSuballocatedUsed, data.PercentSuballocated) > 100.0)
                {
                    element.YValue = 100.0 - (Math.Max(data.PercentSuballocatedUsed, data.PercentSuballocated));
                }
                else
                {
                    element.YValue = data.PercentActual;
                }
                element.Color = Color.DarkOrange;
                element.Name = data.AccountName;

                if (element.YValue > 0.0)
                {
                    seriesUsed.Name = "Used";
                    seriesUsed.Elements.Add(element);
                    collection.Add(seriesUsed);
                }
            }

            if (data.PercentActual + Math.Max(data.PercentSuballocatedUsed, data.PercentSuballocated) > 100.0 && data.PercentActual > 0.0)
            {
                Series seriesOverrun = new Series();

                element = new Element();
                if (Math.Max(data.PercentSuballocated,data.PercentSuballocatedUsed) > 100.0)
                {
                    element.YValue = data.PercentActual;
                }
                else
                {
                    element.YValue = (data.PercentActual + Math.Max(data.PercentSuballocated, data.PercentSuballocatedUsed) - 100.0);   // 10 100 130 should be 10; 10 98 95 should be 8
                }                
                element.Color = Color.Red; 
                element.Name = data.AccountName;

                seriesOverrun.Name = "Overrun";
                seriesOverrun.Elements.Add(element);
                collection.Add(seriesOverrun);
            }


            return collection;
        }


        private class BudgetData
        {
            public string AccountName;
            public double PercentActual;
            public double PercentSuballocated;
            public double PercentSuballocatedUsed;

        }
    }
}
