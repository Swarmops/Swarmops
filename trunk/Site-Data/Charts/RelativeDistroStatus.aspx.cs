using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using dotnetCHARTING;
using Activizr.Basic.Enums;
using Activizr.Logic.Special.Sweden;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using System.Collections.Generic;

public partial class Charts_RelativeDistroStatus : System.Web.UI.Page
{
    bool advanceVotingMode = false;
    bool absoluteMode = false;
    string nav = "";
    protected void Page_Load (object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            NavPanel.Visible = false;
            if (("" + Request.QueryString["Nav"]).ToLower() == "y")
            {
                nav = "y";
                NavPanel.Visible = true;
            }

            cbSort.Checked = true;
            if (("" + Request.QueryString["Sort"]).ToLower() == "n")
            {
                cbSort.Checked = false;
            }

            advanceVotingMode = false;
            if (("" + Request.QueryString["VotingMode"]).ToLower() == "advance")
            {
                advanceVotingMode = true;
            }

            cbWeighted.Checked = true;
            if (("" + Request.QueryString["abs"]).ToLower() == "y")
            {
                absoluteMode = true;
                cbWeighted.Checked = false;
            }

            DrawChart();
        }




        this.LiteralXml.Text = "";
    }

    private void DrawChart ()
    {
        ViewState["mode"] = advanceVotingMode;


        SetChartTitles();

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
        Chart.ChartArea.XAxis.Minimum = -100;
        Chart.ChartArea.XAxis.Maximum = 100;

        // Set the y axis label
        Chart.ChartArea.YAxis.Label.Text = "";


        // Set the directory where the images will be stored.
        Chart.TempDirectory = "temp";

        // Set the chart size.
        Chart.Width = 600;
        Chart.Height = 550;

        if (Request.QueryString["Height"] != null)
        {
            Chart.Height = Int32.Parse(Request.QueryString["Height"]);
        }

        // Add the random data.
        if (advanceVotingMode)
        {
            Chart.SeriesCollection.Add(GetAdvanceRankingData());
        }
        else
        {
            Chart.SeriesCollection.Add(GetVotingDayRankingData());
        }

        Chart.LegendBox.Position = LegendBoxPosition.None;
        Chart.LegendBox.Template = "%Icon%Name";
        Chart.Debug = false;
        Chart.Mentor = false;
    }

    private void SetChartTitles ()
    {
        if (absoluteMode)
        {
            if (advanceVotingMode)
            {
                Chart.Title = "Distributionsstatus valsedlar (f\xF6rtidsr\xF6stning)";
                Chart.ChartArea.XAxis.Label.Text = "Procent röstningslokaler klara (oviktat)";
            }
            else
            {
                Chart.Title = "Distributionsstatus valsedlar (vallokaler)";
                Chart.ChartArea.XAxis.Label.Text = "Procent vallokaler klara (oviktat)";
            }
        }
        else
        {
            if (advanceVotingMode)
            {
                Chart.Title = "Distributionsstatus valsedlar (f\xF6rtidsr\xF6stning)";
                Chart.ChartArea.XAxis.Label.Text = "Procent röstningslokaler klara (viktat efter kommunstorlek)";
            }
            else
            {
                Chart.Title = "Distributionsstatus valsedlar (vallokaler)";
                Chart.ChartArea.XAxis.Label.Text = "Procent vallokaler klara (viktat efter kommunstorlek)";
            }
        }
    }

    bool CanZoom (int nodeId)
    {
        if (nodeId > 40)
        {
            return false;
        }

        if (nodeId == 9 || nodeId == 11 || nodeId == 16 || nodeId == 40)
        {
            return false;
        }

        return true;
    }

    private GeographyBallotCoverageLookup GetLookupFromCache ()
    {
        string key = "BallotCoverage-SE";
        GeographyBallotCoverageData data = null;
        lock (GeographyBallotCoverageData.cacheLocker)
        {
            data = (GeographyBallotCoverageData)(Cache.Get(key));

            if (data == null || "" + Request["reload"] != "")
            {
                data = GeographyBallotCoverageData.UpdateBallotDistroData();
                Cache.Insert(key, data, null, DateTime.Now.ToUniversalTime().AddMinutes(15), Cache.NoSlidingExpiration);
            }
        }

        return data.ToLookup();
    }

    SeriesCollection GetAdvanceRankingData ()
    {

        GeographyBallotCoverageLookup lookup = GetLookupFromCache();
        Geographies geos = null;

        string circuitString = Request.QueryString["CircuitId"];
        int circuitId = 0;

        if (String.IsNullOrEmpty(circuitString) || Int32.Parse(circuitString) == Geography.SwedenId)
        {
            geos = Geographies.FromLevel(Country.FromCode("SE"), GeographyLevel.ElectoralCircuit);
        }
        else
        {
            circuitId = Int32.Parse(circuitString);
            Geographies candidateGeos = Geography.FromIdentity(circuitId).GetTree();
            geos = new Geographies();

            foreach (Geography geo in candidateGeos)
            {
                if (lookup.ContainsKey(geo.Identity) && geo.Identity != circuitId)
                {
                    geos.Add(geo);
                }
            }
        }
        Dictionary<int, Geography> geoLookup = new Dictionary<int, Geography>();
        foreach (Geography geo in geos)
            geoLookup[geo.Identity] = geo;

        GeographyBallotCoverageData data = new GeographyBallotCoverageData();

        foreach (Geography geo in geos)
        {
            if (lookup.ContainsKey(geo.Identity))
            {
                data.Add(lookup[geo.Identity]);
            }
        }

        bool sortByValue = cbSort.Checked;
        data.Sort(
            delegate(GeographyBallotCoverageDataPoint p1, GeographyBallotCoverageDataPoint p2)
            {
                if (sortByValue)
                {
                    if (advanceVotingMode)
                    {
                        double i1 = (absoluteMode ? p1.AdvanceVotingCoverage : p1.WWAdvanceVotingCoverage);
                        double i2 = (absoluteMode ? p2.AdvanceVotingCoverage : p2.WWAdvanceVotingCoverage);
                        return i1.CompareTo(i2);
                    }
                    else
                    {
                        double i1 = (absoluteMode ? p1.VotingStationsDistroSingle : p1.WVotingStationsDistroSingle);
                        double i2 = (absoluteMode ? p2.VotingStationsDistroSingle : p2.WVotingStationsDistroSingle);
                        return i1.CompareTo(i2);
                    }
                }
                else
                {
                    return geoLookup[p2.GeographyId].Name.CompareTo(geoLookup[p1.GeographyId].Name);
                }
            });


        SeriesCollection collection = new SeriesCollection();

        Series seriesPositive = new Series();
        seriesPositive.Name = "Positive";

        Series seriesNegative = new Series();
        seriesNegative.Name = "Negative";

        // Add empties, then Sweden

        AddSingleAdvanceDataPoint(lookup[Geography.SwedenId], seriesPositive, seriesNegative);

        if (circuitId != 0)
        {
            AddSingleAdvanceDataPoint(lookup[circuitId], seriesPositive, seriesNegative);
        }

        seriesPositive.Elements.Add(new Element());
        seriesNegative.Elements.Add(new Element());

        foreach (GeographyBallotCoverageDataPoint dataPoint in data)
        {
            AddSingleAdvanceDataPoint(dataPoint, seriesPositive, seriesNegative);
        }

        seriesPositive.DefaultElement.Color = System.Drawing.Color.Green;
        seriesPositive.DefaultElement.ShowValue = true;
        seriesNegative.DefaultElement.Color = System.Drawing.Color.DarkRed;

        SeriesCollection result = new SeriesCollection();
        result.Add(seriesPositive);
        result.Add(seriesNegative);

        return result;
    }

    private void AddSingleAdvanceDataPoint (GeographyBallotCoverageDataPoint dataPoint, Series seriesPositive, Series seriesNegative)
    {
        Geography geo = Geography.FromIdentity(dataPoint.GeographyId);

        Element element = new Element();
        element.YValue = absoluteMode ? dataPoint.AdvanceVotingCoverage : dataPoint.WWAdvanceVotingCoverage;
        element.Name = geo.Name;
        element.SmartLabel = new SmartLabel();
        element.SmartLabel.Text = (absoluteMode ? dataPoint.AdvanceVotingCoverage : dataPoint.WWAdvanceVotingCoverage).ToString("0.0");
        element.SmartLabel.Color = System.Drawing.Color.Black;

        if (CanZoom(dataPoint.GeographyId))
        {
            element.URL = "RelativeDistroStatus.aspx?" + (nav != "" ? "nav=y&" : "") + "VotingMode=Advance&abs=" + (absoluteMode ? "Y" : "N") + "&CircuitId=" + dataPoint.GeographyId;
        }
        else if (!String.IsNullOrEmpty(dataPoint.BookingUrl))
        {
            element.URL = dataPoint.BookingUrl;
        }
        seriesPositive.Elements.Add(element);

        element = new Element();
        element.YValue = (absoluteMode ? dataPoint.AdvanceVotingCoverage : dataPoint.WWAdvanceVotingCoverage) - 100;
        element.Name = geo.Name;
        seriesNegative.Elements.Add(element);

        if (CanZoom(dataPoint.GeographyId))
        {
            element.URL = "RelativeDistroStatus.aspx?" + (nav != "" ? "nav=y&" : "") + "VotingMode=Advance&abs=" + (absoluteMode ? "Y" : "N") + "&CircuitId=" + dataPoint.GeographyId;
        }
        else if (!String.IsNullOrEmpty(dataPoint.BookingUrl))
        {
            element.URL = dataPoint.BookingUrl;
        }
    }

    SeriesCollection GetVotingDayRankingData ()
    {
        GeographyBallotCoverageLookup lookup = GetLookupFromCache();
        Geographies geos = null;

        string circuitString = Request.QueryString["CircuitId"];
        int circuitId = 0;

        if (String.IsNullOrEmpty(circuitString) || Int32.Parse(circuitString) == Geography.SwedenId)
        {
            geos = Geographies.FromLevel(Country.FromCode("SE"), GeographyLevel.ElectoralCircuit);
        }
        else
        {
            circuitId = Int32.Parse(circuitString);
            Geographies candidateGeos = Geography.FromIdentity(circuitId).GetTree();
            geos = new Geographies();

            foreach (Geography geo in candidateGeos)
            {
                if (lookup.ContainsKey(geo.Identity) && geo.Identity != circuitId)
                {
                    geos.Add(geo);
                }
            }
        }
        Dictionary<int, Geography> geoLookup = new Dictionary<int, Geography>();
        foreach (Geography geo in geos)
            geoLookup[geo.Identity] = geo;

        GeographyBallotCoverageData data = new GeographyBallotCoverageData();
        foreach (Geography geo in geos)
        {
            if (lookup.ContainsKey(geo.Identity))
            {
                data.Add(lookup[geo.Identity]);
            }
        }

        bool sortByValue = cbSort.Checked;
        data.Sort(
            delegate(GeographyBallotCoverageDataPoint p1, GeographyBallotCoverageDataPoint p2)
            {
                if (sortByValue)
                {
                    if (advanceVotingMode)
                    {
                        double i1 = (absoluteMode ? p1.AdvanceVotingCoverage : p1.WWAdvanceVotingCoverage);
                        double i2 = (absoluteMode ? p2.AdvanceVotingCoverage : p2.WWAdvanceVotingCoverage);
                        return i1.CompareTo(i2);
                    }
                    else
                    {
                        double i1 = (absoluteMode ? p1.VotingCoverage : p1.WVotingCoverage);
                        double i2 = (absoluteMode ? p2.VotingCoverage : p2.WVotingCoverage);
                        return i1.CompareTo(i2);
                    }
                }
                else
                {
                    return geoLookup[p2.GeographyId].Name.CompareTo(geoLookup[p1.GeographyId].Name);
                }
            });



        SeriesCollection collection = new SeriesCollection();

        Series seriesPositiveSingle = new Series();
        seriesPositiveSingle.Name = "Positive, single distro";

        Series seriesPositiveDouble = new Series();
        seriesPositiveDouble.Name = "Positive, double distro";

        Series seriesPositiveFull = new Series();
        seriesPositiveFull.Name = "Positive, full coverage";

        Series seriesNegative = new Series();
        seriesNegative.Name = "Negative";

        // Add empties, then Sweden

        AddSingleVotingDayDataPoint(lookup[Geography.SwedenId], seriesPositiveSingle, seriesPositiveDouble, seriesPositiveFull, seriesNegative);

        if (circuitId != 0)
        {
            AddSingleVotingDayDataPoint(lookup[circuitId], seriesPositiveSingle, seriesPositiveDouble, seriesPositiveFull, seriesNegative);
        }

        seriesPositiveSingle.Elements.Add(new Element());
        seriesNegative.Elements.Add(new Element());

        foreach (GeographyBallotCoverageDataPoint dataPoint in data)
        {
            AddSingleVotingDayDataPoint(dataPoint, seriesPositiveSingle, seriesPositiveDouble, seriesPositiveFull, seriesNegative);
        }

        seriesPositiveSingle.DefaultElement.Color = System.Drawing.Color.Yellow;
        seriesPositiveDouble.DefaultElement.Color = System.Drawing.Color.YellowGreen;
        seriesPositiveFull.DefaultElement.Color = System.Drawing.Color.Green;
        seriesPositiveSingle.DefaultElement.ShowValue = true;
        seriesNegative.DefaultElement.Color = System.Drawing.Color.DarkRed;

        SeriesCollection result = new SeriesCollection();
        result.Add(seriesPositiveSingle);
        result.Add(seriesPositiveDouble);
        result.Add(seriesPositiveFull);
        result.Add(seriesNegative);

        return result;
    }


    private void AddSingleVotingDayDataPoint (GeographyBallotCoverageDataPoint dataPoint, Series seriesPositiveSingle, Series seriesPositiveDouble, Series seriesPositiveFull, Series seriesNegative)
    {
        Geography geo = Geography.FromIdentity(dataPoint.GeographyId);

        Element element = new Element();
        element.YValue = (((absoluteMode ? dataPoint.VotingStationsDistroSingle : dataPoint.WVotingStationsDistroSingle)
                            - (absoluteMode ? dataPoint.VotingStationsDistroDouble : dataPoint.WVotingStationsDistroDouble)) * 100.0)
                            / (absoluteMode ? dataPoint.VotingStationsTotal : dataPoint.WVotingStationsTotal);
        double distroValue = ((absoluteMode ? dataPoint.VotingStationsDistroSingle : dataPoint.WVotingStationsDistroSingle) * 100.0)
                            / (absoluteMode ? dataPoint.VotingStationsTotal : dataPoint.WVotingStationsTotal);
        element.Name = geo.Name;
        element.SmartLabel = new SmartLabel();
        element.SmartLabel.DynamicDisplay = false;
        element.SmartLabel.Text = (distroValue).ToString("0.0");
        element.SmartLabel.Color = System.Drawing.Color.Black;

        if (CanZoom(dataPoint.GeographyId))
        {
            element.URL = "RelativeDistroStatus.aspx?" + (nav != "" ? "nav=y&" : "") + "VotingMode=ElectionDay&abs=" + (absoluteMode ? "Y" : "N") + "&CircuitId=" + dataPoint.GeographyId;
            // hack så att grafen inte blir helt hoptryckt i höjdled när man vill se status för hela sverige /pvz
            if (dataPoint.GeographyId == Geography.SwedenId)
            {
                element.URL += "&Height=5000";
            }
        }
        else if (!String.IsNullOrEmpty(dataPoint.BookingUrl))
        {
            element.URL = dataPoint.BookingUrl;
        }
        seriesPositiveSingle.Elements.Add(element);

        // Double coverage

        element = new Element();
        element.YValue = (((absoluteMode ? dataPoint.VotingStationsDistroDouble : dataPoint.WVotingStationsDistroDouble)
                            - (absoluteMode ? dataPoint.VotingStationsComplete : dataPoint.WVotingStationsComplete)) * 100.0)
                            / (absoluteMode ? dataPoint.VotingStationsTotal : dataPoint.WVotingStationsTotal);

        element.Name = geo.Name;

        if (CanZoom(dataPoint.GeographyId))
        {
            element.URL = "RelativeDistroStatus.aspx?" + (nav != "" ? "nav=y&" : "") + "VotingMode=ElectionDay&abs=" + (absoluteMode ? "Y" : "N") + "&CircuitId=" + dataPoint.GeographyId;
        }
        else if (!String.IsNullOrEmpty(dataPoint.BookingUrl))
        {
            element.URL = dataPoint.BookingUrl;
        }
        seriesPositiveDouble.Elements.Add(element);

        // full coverage - not yet implemented
        element = new Element();
        element.YValue = ((absoluteMode ? dataPoint.VotingStationsComplete : dataPoint.WVotingStationsComplete) * 100.0)
                            / (absoluteMode ? dataPoint.VotingStationsTotal : dataPoint.WVotingStationsTotal);

        element.Name = geo.Name;

        if (CanZoom(dataPoint.GeographyId))
        {
            element.URL = "RelativeDistroStatus.aspx?" + (nav != "" ? "nav=y&" : "") + "VotingMode=ElectionDay&abs=" + (absoluteMode ? "Y" : "N") + "&CircuitId=" + dataPoint.GeographyId;
        }
        else if (!String.IsNullOrEmpty(dataPoint.BookingUrl))
        {
            element.URL = dataPoint.BookingUrl;
        }
        seriesPositiveFull.Elements.Add(element);

        // neg bar
        element = new Element();
        element.YValue = (absoluteMode ? dataPoint.VotingCoverage : dataPoint.WVotingCoverage) - 100;
        element.Name = geo.Name;
        seriesNegative.Elements.Add(element);

        if (CanZoom(dataPoint.GeographyId))
        {
            element.URL = "RelativeDistroStatus.aspx?" + (nav != "" ? "nav=y&" : "") + "VotingMode=ElectionDay&abs=" + (absoluteMode ? "Y" : "N") + "&CircuitId=" + dataPoint.GeographyId;
        }
        else if (!String.IsNullOrEmpty(dataPoint.BookingUrl))
        {
            element.URL = dataPoint.BookingUrl;
        }
    }

    protected void LinkButton1_Click (object sender, EventArgs e)
    {
        nav = "y";
        advanceVotingMode = true;
        absoluteMode = !cbWeighted.Checked;
        DrawChart();
    }
    protected void LinkButton2_Click (object sender, EventArgs e)
    {
        nav = "y";
        advanceVotingMode = false;
        absoluteMode = !cbWeighted.Checked;
        DrawChart();

    }

    protected void cbWeighted_CheckedChanged (object sender, EventArgs e)
    {
        nav = "y";
        advanceVotingMode = (bool)ViewState["mode"];
        absoluteMode = !cbWeighted.Checked;
        DrawChart();

    }
    protected void cbSort_CheckedChanged (object sender, EventArgs e)
    {
        nav = "y";
        advanceVotingMode = (bool)ViewState["mode"];
        absoluteMode = !cbWeighted.Checked;
        DrawChart();

    }
}
