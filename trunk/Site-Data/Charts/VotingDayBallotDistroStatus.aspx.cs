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

public partial class Charts_VotingDayBallotDistroStatus : System.Web.UI.Page
{
    bool absoluteMode = true;
    protected void Page_Load (object sender, EventArgs e)
    {
        Chart.Title = "Distributionsstatus valsedlar (valdagen) ";


        // Set the chart Type
        Chart.Type = ChartType.ComboHorizontal;

        // Turn 3D off.
        Chart.Use3D = false;

        // Change the shading mode
        Chart.ShadingEffectMode = ShadingEffectMode.Three;

        // Set a default transparency
        Chart.DefaultSeries.DefaultElement.Transparency = 00;

        // Set the x axis scale
        Chart.ChartArea.XAxis.Scale = Scale.Normal;
        Chart.ChartArea.XAxis.Minimum = 0;
        if (String.IsNullOrEmpty(Request.QueryString["CircuitId"]))
            Chart.ChartArea.XAxis.Maximum = 38000;

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


        Chart.SeriesCollection.Add(GetVotingDayRankingData());

        Chart.LegendBox.Position = LegendBoxPosition.None;
        Chart.LegendBox.Template = "%Icon%Name";
        Chart.Debug = false;
        Chart.Mentor = false;


        this.LiteralXml.Text = "";
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

            if (data == null || ""+Request["reload"]!="")
            {
                data = GeographyBallotCoverageData.UpdateBallotDistroData();
                Response.Write("Reloading");
                Cache.Insert(key, data, null, DateTime.Now.ToUniversalTime().AddMinutes(15), Cache.NoSlidingExpiration);
            }
        }

        return data.ToLookup();
    }

    SeriesCollection GetVotingDayRankingData ()
    {
        GeographyBallotCoverageLookup lookup;
        lookup= GetLookupFromCache();

        Geographies geos = null;

        string circuitString = Request.QueryString["CircuitId"];
        int circuitId = 0;

        if (String.IsNullOrEmpty(Request.QueryString["CircuitId"]))
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

        GeographyBallotCoverageData data = new GeographyBallotCoverageData();
        foreach (Geography geo in geos)
        {
            if (lookup.ContainsKey(geo.Identity))
            {
                data.Add(lookup[geo.Identity]);
            }
        }

        data.Sort(
            delegate(GeographyBallotCoverageDataPoint p1, GeographyBallotCoverageDataPoint p2)
            {
                int i1 = (absoluteMode?p1.VotingStationsTotal: p1.WVotingStationsTotal) 
                        - (absoluteMode?p1.VotingStationsDistroSingle: p1.WVotingStationsDistroSingle);
                int i2 = (absoluteMode?p2.VotingStationsTotal: p2.WVotingStationsTotal) 
                        - (absoluteMode?p2.VotingStationsDistroSingle: p2.WVotingStationsDistroSingle);
                return i1.CompareTo(i2);
            });

        SeriesCollection collection = new SeriesCollection();

        Series seriesPositiveSingle = new Series();
        seriesPositiveSingle.Name = "Positive, single distro";

        Label2.Text = "(I hela Sverige saknar " + (lookup[Geography.SwedenId].VotingStationsTotal - lookup[Geography.SwedenId].VotingStationsDistroSingle).ToString("#,##0") + " väljare Piratpartiets valsedlar.)";

        if (circuitId != 0)
        {
            AddSingleVotingDayDataPoint(lookup[circuitId], seriesPositiveSingle);
        }

        seriesPositiveSingle.Elements.Add(new Element());

        foreach (GeographyBallotCoverageDataPoint dataPoint in data)
        {
            AddSingleVotingDayDataPoint(dataPoint, seriesPositiveSingle);
        }

        seriesPositiveSingle.DefaultElement.Color = System.Drawing.Color.Red;
        seriesPositiveSingle.DefaultElement.ShowValue = true;

        SeriesCollection result = new SeriesCollection();
        result.Add(seriesPositiveSingle);

        return result;
    }



    private void AddSingleVotingDayDataPoint (GeographyBallotCoverageDataPoint dataPoint, Series seriesPositiveSingle)
    {
        if (String.IsNullOrEmpty(Request.QueryString["CircuitId"]) && !CanZoom(dataPoint.GeographyId))
            return;

        Geography geo = Geography.FromIdentity(dataPoint.GeographyId);

        Element element = new Element();
        element.YValue = ((absoluteMode?dataPoint.VotingStationsTotal:dataPoint.WVotingStationsTotal) 
                         - (absoluteMode?dataPoint.VotingStationsDistroSingle:dataPoint.WVotingStationsDistroSingle));
        element.Name = geo.Name;
        element.SmartLabel = new SmartLabel();
        element.SmartLabel.Text = element.YValue.ToString("#,##0");
        element.SmartLabel.Color = System.Drawing.Color.Black;

        if (CanZoom(dataPoint.GeographyId))
        {
            element.URL = "AbsoluteBallotDistroStatus.aspx?VotingMode=ElectionDay&CircuitId=" + dataPoint.GeographyId;
        }
        else if (!String.IsNullOrEmpty(dataPoint.BookingUrl))
        {
            element.URL = dataPoint.BookingUrl;
        }
        seriesPositiveSingle.Elements.Add(element);



    }
}
